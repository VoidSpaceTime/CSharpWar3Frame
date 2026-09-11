using System.Numerics;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Systems;

namespace War3Frame.Src.Systems.Unit;

/// <summary>
/// 移动系统。
/// 负责推进 ECS 移动语义并产生 MoveOutcome；原生命令只通过 MoveNativeRequest 交给 Native 层。
/// </summary>
[SystemRegister(SystemKind.Interval)]
public class MoveSystem : QuerySystem<MoveCommand, Position>, ITimedSystem
{
    private static int _nextCommandToken = 1;

    public float Interval => 0.1f; // 每 0.1 秒检查一次

    /// <summary>
    /// 生成新的移动命令令牌。
    /// </summary>
    public static int NextCommandToken() => _nextCommandToken++;

    protected override void OnUpdate()
    {
        // 移动推进涉及增删 Tag/Component（结构变更），不能在 Query 迭代内执行：
        // 先收集本轮的推进决策，循环外统一应用。
        var interrupted = new List<(Entity unit, int token)>();
        var overridden = new List<(Entity unit, int token)>();
        var arrived = new List<(Entity unit, MoveCommand move)>();
        var moving = new List<(Entity unit, MoveCommand move, Position pos)>();

        Query.ForEachEntity((ref MoveCommand move, ref Position pos, Entity unit) =>
        {
            // 如果被打断或控制
            if (ControlHelper.IsIncapacitated(unit))
            {
                interrupted.Add((unit, move.commandToken));
                return;
            }

            //  其他移动命令顶替
            if (unit.TryGetComponent<MoveExecutionState>(out var execution)
                && execution.commandToken != move.commandToken)
            {
                overridden.Add((unit, move.commandToken));
                return;
            }

            // 计算到目标的距离
            float dist = Vector2.Distance(new Vector2(pos.x, pos.y), new Vector2(move.targetX, move.targetY));

            if (dist <= move.arrivalDistance)
            {
                arrived.Add((unit, move));
            }
            else
            {
                moving.Add((unit, move, pos));
            }
        });

        foreach (var (unit, token) in interrupted)
        {
            if (unit.IsNull) continue;
            EmitOutcome(unit, token, MoveOutcomeType.Interrupted);
            unit.RemoveTag<MovingTag>();
            unit.RemoveComponent<MoveCommand>();
            unit.RemoveComponent<MoveExecutionState>();
        }

        foreach (var (unit, token) in overridden)
        {
            if (unit.IsNull) continue;
            EmitOutcome(unit, token, MoveOutcomeType.Overridden);
            unit.RemoveComponent<MoveCommand>();
            unit.RemoveComponent<MoveExecutionState>();
        }

        foreach (var (unit, move) in arrived)
        {
            if (!unit.IsNull) HandleArrival(unit, move);
        }

        foreach (var (unit, move, pos) in moving)
        {
            if (!unit.IsNull) ExecuteMove(unit, move, pos);
        }
    }

    /// <summary>
    /// 继续移动。若单位有 native 句柄，只发出一次原生命令请求，后续由位置同步反馈到 ECS。
    /// </summary>
    private void ExecuteMove(Entity unit, MoveCommand move, Position pos)
    {
        // 更新移动标记
        if (!unit.Tags.Has<MovingTag>())
        {
            unit.AddTag<MovingTag>();
        }

        // 发布原生命令请求，由执行层消费
        if (unit.TryGetComponent<UnitNative>(out _) && !move.issued)
        {
            move.issued = true;
            unit.AddComponent(move);
            unit.AddComponent(new MoveExecutionState
            {
                commandToken = move.commandToken,
                hasStarted = true
            });
            UnitHelper.RequestMoveCommand(unit, move.orderType, move.targetX, move.targetY, move.commandToken);
        }
    }

    /// <summary>
    /// 到达目标后发出 stop 请求，并用 MoveOutcome 通知上层工作流。
    /// </summary>
    private void HandleArrival(Entity unit, MoveCommand move)
    {
        // 到达后发布 stop 命令
        UnitHelper.RequestMoveCommand(unit, MoveOrderType.Stop, move.targetX, move.targetY, move.commandToken);

        unit.RemoveTag<MovingTag>();

        EmitOutcome(unit, move.commandToken, MoveOutcomeType.Arrived);

        // 移除移动命令（任务完成）
        unit.RemoveComponent<MoveCommand>();
        unit.RemoveComponent<MoveExecutionState>();
    }

    /// <summary>
    /// 统一写入移动结果，让施法、任务等上层系统自行消费。
    /// </summary>
    private static void EmitOutcome(Entity unit, int commandToken, MoveOutcomeType outcome)
    {
        unit.AddComponent(new MoveOutcome
        {
            commandToken = commandToken,
            outcome = outcome
        });
    }
}

/// <summary>
/// 移动后任务桥接系统。
/// 证明 move outcome 不只服务施法，也可以驱动通用任务流。
/// </summary>
[SystemRegister(SystemKind.Immediate)]
public class MoveToTaskSystem : QuerySystem<MoveOutcome, MoveContinuation>
{
    protected override void OnUpdate()
    {
        // 收尾涉及增删组件（结构变更）：先收集，循环外应用。
        var toComplete = new List<Entity>();
        var toCancel = new List<Entity>();

        Query.ForEachEntity((ref MoveOutcome outcome, ref MoveContinuation continuation, Entity unit) =>
        {
            if (continuation.kind != MoveContinuationKind.ExecuteTask)
            {
                return;
            }

            // 到达
            if (outcome.outcome is MoveOutcomeType.Arrived)
            {
                toComplete.Add(unit);
                return;
            }

            // 非到达（取消 / 被覆盖 / 打断 / 失败）
            if (outcome.outcome is MoveOutcomeType.Cancelled or MoveOutcomeType.Overridden
                or MoveOutcomeType.Interrupted or MoveOutcomeType.Failed)
            {
                toCancel.Add(unit);
            }
        });

        foreach (var unit in toComplete)
        {
            if (unit.IsNull) continue;
            unit.AddComponent(new MoveTaskState
            {
                completed = true,
                cancelled = false
            });
            unit.RemoveComponent<MoveOutcome>();
            unit.RemoveComponent<MoveContinuation>();
        }

        foreach (var unit in toCancel)
        {
            if (unit.IsNull) continue;
            unit.AddComponent(new MoveTaskState
            {
                completed = false,
                cancelled = true
            });
            unit.RemoveComponent<MoveOutcome>();
            unit.RemoveComponent<MoveContinuation>();
        }
    }
}

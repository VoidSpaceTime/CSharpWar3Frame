using System.Numerics;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Systems;

namespace War3Frame.Src.Systems.Unit;

/// <summary>
/// 移动系统 - 处理单位的移动命令
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
        Query.ForEachEntity((ref MoveCommand move, ref Position pos, Entity unit) =>
        {
            // 如果被打断或控制
            if (ControlHelper.IsIncapacitated(unit))
            {
                EmitOutcome(unit, move.commandToken, MoveOutcomeType.Interrupted);
                unit.RemoveTag<MovingTag>();
                unit.RemoveComponent<MoveCommand>();
                unit.RemoveComponent<MoveExecutionState>();
                return;
            }

            //  其他移动命令顶替
            if (unit.TryGetComponent<MoveExecutionState>(out var execution)
                && execution.commandToken != move.commandToken)
            {
                EmitOutcome(unit, move.commandToken, MoveOutcomeType.Overridden);
                unit.RemoveComponent<MoveCommand>();
                unit.RemoveComponent<MoveExecutionState>();
                return;
            }

            // 计算到目标的距离
            float dist = Vector2.Distance(new Vector2(pos.x, pos.y), new Vector2(move.targetX, move.targetY));

            if (dist <= move.arrivalDistance)
            {
                // 到达目标
                HandleArrival(unit, move);
            }
            else
            {
                // 继续移动
                ExecuteMove(unit, move, pos);
            }
            Game.FlushImmediateSystems();
        });
    }

    // 继续移动
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
            unit.AddComponent(new MoveNativeCommandRequest
            {
                commandToken = move.commandToken,
                orderType = move.orderType,
                targetX = move.targetX,
                targetY = move.targetY
            });
        }
    }

    // 到达目标
    private void HandleArrival(Entity unit, MoveCommand move)
    {
        // 到达后发布 stop 命令，由执行层负责执行
        unit.AddComponent(new MoveNativeCommandRequest
        {
            commandToken = move.commandToken,
            orderType = MoveOrderType.Stop,
            targetX = move.targetX,
            targetY = move.targetY
        });

        unit.RemoveTag<MovingTag>();

        EmitOutcome(unit, move.commandToken, MoveOutcomeType.Arrived);

        // 移除移动命令（任务完成）
        unit.RemoveComponent<MoveCommand>();
        unit.RemoveComponent<MoveExecutionState>();
    }

    // 替换
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
        Query.ForEachEntity((ref MoveOutcome outcome, ref MoveContinuation continuation, Entity unit) =>
        {
            if (continuation.kind != MoveContinuationKind.ExecuteTask)
            {
                return;
            }

            // 到达
            if (outcome.outcome is MoveOutcomeType.Arrived)
            {
                unit.AddComponent(new MoveTaskState
                {
                    completed = true,
                    cancelled = false
                });
                unit.RemoveComponent<MoveOutcome>();
                unit.RemoveComponent<MoveContinuation>();
                return;
            }

            // 非到达 取消
            if (outcome.outcome is MoveOutcomeType.Arrived)
            {
                unit.AddComponent(new MoveTaskState
                {
                    completed = false,
                    cancelled = true
                });
                unit.RemoveComponent<MoveOutcome>();
                unit.RemoveComponent<MoveContinuation>();
            }
        });
    }
}
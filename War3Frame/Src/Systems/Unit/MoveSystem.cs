using System.Numerics;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace War3Frame.Src.Systems.Unit;

/// <summary>
/// 移动系统 - 处理单位的移动命令
/// </summary>
public class MoveSystem : QuerySystem<MoveCommand, Position>, ITimedSystem
{
    public float Interval => 0.1f; // 每 0.1 秒检查一次

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref MoveCommand move, ref Position pos, Entity unit) =>
        {
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
        });
    }

    private void ExecuteMove(Entity unit, MoveCommand move, Position pos)
    {
        // 更新移动标记
        if (!unit.Tags.Has<MovingTag>())
        {
            unit.AddTag<MovingTag>();
        }

        // 移除到达标记（如果有）
        if (unit.Tags.Has<ArrivedTag>())
        {
            unit.RemoveTag<ArrivedTag>();
        }

        // 发送 War3 原生移动命令
        if (unit.TryGetComponent<UnitNative>(out var native))
        {
            JassApi.IssuePointOrder(native.unit, "move", move.targetX, move.targetY);
        }
    }

    private void HandleArrival(Entity unit, MoveCommand move)
    {
        // 停止移动
        if (unit.TryGetComponent<UnitNative>(out var native))
        {
            JassApi.IssueImmediateOrder(native.unit, "stop");
        }

        // 更新标记
        unit.RemoveTag<MovingTag>();
        unit.AddTag<ArrivedTag>();

        // 移除移动命令（任务完成）
        unit.RemoveComponent<MoveCommand>();
    }
}
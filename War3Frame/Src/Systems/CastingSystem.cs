using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;


namespace War3Frame.Src.Systems;

/// <summary>
/// 施法请求处理系统 - 处理玩家/AI 的施法请求
/// </summary>
public class CastRequestSystem : QuerySystem<CastRequest, Position> , ITimedSystem
{
    public float Interval { get; } = 0.02f;
    
    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref CastRequest request, ref Position pos, Entity unit) =>
        {
            var ability = request.ability;
            if (ability.IsNull || !ability.TryGetComponent<AbilityBase>(out var abilityBase))
            {
                unit.RemoveComponent<CastRequest>();
                return;
            }

            // 检查技能是否可用
            if (abilityBase.state != AbilityState.Ready)
            {
                unit.RemoveComponent<CastRequest>();
                return;
            }

            // 检查资源消耗（通用）
            if (!AbilityCostHelper.CheckCost(unit, request.ability))
            {
                unit.RemoveComponent<CastRequest>();
                return; // 资源不足
            }

            // 计算到目标的距离
            float targetX = request.targetX;
            float targetY = request.targetY;

            // 如果是对单位施法，使用单位坐标
            if (!request.targetUnit.IsNull && request.targetUnit.TryGetComponent<Position>(out var targetPos))
            {
                targetX = targetPos.x;
                targetY = targetPos.y;
            }

            float dist = CalcDistance(pos.x, pos.y, targetX, targetY);
            float castRange = abilityBase.castRange;

            if (dist <= castRange)
            {
                // 在范围内，直接开始施法
                StartCasting(unit, request, abilityBase);
            }
            else
            {
                // 不在范围内，发出移动命令
                unit.AddComponent(new CastState
                {
                    phase = CastPhase.MovingToCast,
                    ability = ability,
                    targetUnit = request.targetUnit,
                    targetX = targetX,
                    targetY = targetY,
                    timer = 0
                });
                unit.AddTag<MovingForCastTag>();

                // 发出移动命令（由 MoveSystem 处理）
                unit.AddComponent(new MoveCommand
                {
                    targetX = targetX,
                    targetY = targetY,
                    arrivalDistance = castRange * 0.9f, // 留一点余量
                    reason = MoveReason.CastingAbility
                });
            }

            // 移除请求
            unit.RemoveComponent<CastRequest>();
        });
    }

    private void StartCasting(Entity unit, CastRequest request, AbilityBase abilityBase)
    {
        unit.AddComponent(new CastState
        {
            phase = CastPhase.Casting,
            ability = request.ability,
            targetUnit = request.targetUnit,
            targetX = request.targetX,
            targetY = request.targetY,
            timer = abilityBase.castTime
        });

        // 扣除资源消耗
        AbilityCostHelper.ApplyCost(unit, request.ability);

        // 更新技能状态
        abilityBase.state = AbilityState.Casting;
        request.ability.AddComponent(abilityBase);
    }

    private float CalcDistance(float x1, float y1, float x2, float y2)
    {
        float dx = x2 - x1;
        float dy = y2 - y1;
        return (float)Math.Sqrt(dx * dx + dy * dy);
    }

}

/// <summary>
/// 移动到施法范围系统 - 监听移动到达事件
/// </summary>
public class MoveToCastSystem : QuerySystem<CastState>
{
    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref CastState cast, Entity unit) =>
        {
            if (cast.phase != CastPhase.MovingToCast) return;
            if (!unit.Tags.Has<MovingForCastTag>()) return;

            var ability = cast.ability;
            if (ability.IsNull || !ability.TryGetComponent<AbilityBase>(out var abilityBase)) return;

            // ========================================
            // 检查是否被打断
            // ========================================
            
            // 1. 检查控制效果打断（眩晕/击飞/囚禁）
            if (IsControlled(unit))
            {
                CancelCastMovement(unit, cast);
                return;
            }

            // 2. 检查玩家手动取消（发出了新的移动命令或停止命令）
            if (unit.TryGetComponent<MoveCommand>(out var moveCmd) && moveCmd.reason != MoveReason.CastingAbility)
            {
                // 玩家发出了新的非施法移动命令，取消施法移动
                CancelCastMovement(unit, cast);
                return;
            }

            // 3. 检查是否被打断标记
            if (unit.Tags.Has<CastInterruptedTag>())
            {
                CancelCastMovement(unit, cast);
                unit.RemoveTag<CastInterruptedTag>();
                return;
            }

            // ========================================
            // 正常流程
            // ========================================

            // 检查是否到达目标（由 MoveSystem 设置的标记）
            if (unit.Tags.Has<ArrivedTag>())
            {
                // 到达范围，开始施法
                unit.RemoveTag<MovingForCastTag>();
                unit.RemoveTag<ArrivedTag>();

                cast.phase = CastPhase.Casting;
                cast.timer = abilityBase.castTime;
                unit.AddComponent(cast);

                // 扣除资源消耗
                AbilityCostHelper.ApplyCost(unit, cast.ability);

                // 更新技能状态
                abilityBase.state = AbilityState.Casting;
                ability.AddComponent(abilityBase);
            }
            else if (!cast.targetUnit.IsNull && cast.targetUnit.TryGetComponent<Position>(out var targetPos))
            {
                // 如果目标移动了，更新移动命令
                if (unit.TryGetComponent<MoveCommand>(out var cmd) && cmd.reason == MoveReason.CastingAbility)
                {
                    float newX = targetPos.x;
                    float newY = targetPos.y;

                    // 只有目标移动超过一定距离才更新
                    float dx = newX - cmd.targetX;
                    float dy = newY - cmd.targetY;
                    if (dx * dx + dy * dy > 100 * 100) // 100 单位
                    {
                        cmd.targetX = newX;
                        cmd.targetY = newY;
                        unit.AddComponent(cmd);

                        cast.targetX = newX;
                        cast.targetY = newY;
                        unit.AddComponent(cast);
                    }
                }
            }
        });
    }

    /// <summary>
    /// 检查单位是否处于控制效果中（使用属性系统）
    /// </summary>
    private bool IsControlled(Entity unit)
    {
        return ControlHelper.IsIncapacitated(unit);
    }

    /// <summary>
    /// 取消施法移动
    /// </summary>
    private void CancelCastMovement(Entity unit, CastState cast)
    {
        // 清理施法状态
        unit.RemoveTag<MovingForCastTag>();
        unit.RemoveComponent<CastState>();

        // 移除施法移动命令
        if (unit.TryGetComponent<MoveCommand>(out var moveCmd) && moveCmd.reason == MoveReason.CastingAbility)
        {
            unit.RemoveComponent<MoveCommand>();
        }

        // 重置技能状态
        if (cast.ability.TryGetComponent<AbilityBase>(out var abilityBase))
        {
            abilityBase.state = AbilityState.Ready;
            cast.ability.AddComponent(abilityBase);
        }
    }
}

/// <summary>
/// 施法吟唱系统
/// </summary>
public class CastingSystem : QuerySystem<CastState>, ITimedSystem
{
    public float Interval => 0.05f; // 每 50ms 更新一次

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref CastState cast, Entity unit) =>
        {
            if (cast.phase != CastPhase.Casting) return;

            // 检查是否被打断
            if (unit.Tags.Has<CastInterruptedTag>())
            {
                InterruptCast(unit, cast);
                return;
            }

            cast.timer -= Tick.deltaTime;

            if (cast.timer <= 0)
            {
                // 吟唱完成，执行技能效果
                ExecuteAbility(unit, cast);

                // 检查是否有持续施法
                if (cast.ability.TryGetComponent<AbilityBase>(out var abilityBase) 
                    && abilityBase.channelDuration > 0)
                {
                    // 进入持续施法阶段
                    cast.phase = CastPhase.Channeling;
                    cast.timer = abilityBase.channelDuration;
                    unit.AddComponent(cast);

                    unit.AddComponent(new ChannelState
                    {
                        remaining = abilityBase.channelDuration,
                        duration = abilityBase.channelDuration,
                        ability = cast.ability
                    });
                }
                else
                {
                    // 施法完成，进入冷却
                    FinishCast(unit, cast);
                }
            }
            else
            {
                unit.AddComponent(cast);
            }
        });
    }

    private void ExecuteAbility(Entity unit, CastState cast)
    {
        // TODO: 这里实现技能效果逻辑
        // 例如：创建伤害、治疗、Buff 等
    }

    private void FinishCast(Entity unit, CastState cast)
    {
        if (cast.ability.TryGetComponent<AbilityBase>(out var abilityBase))
        {
            abilityBase.state = AbilityState.Cooldown;
            abilityBase.currentCd = abilityBase.cooldown;
            cast.ability.AddComponent(abilityBase);
        }

        unit.RemoveComponent<CastState>();
    }

    private void InterruptCast(Entity unit, CastState cast)
    {
        if (cast.ability.TryGetComponent<AbilityBase>(out var abilityBase))
        {
            abilityBase.state = AbilityState.Ready;
            cast.ability.AddComponent(abilityBase);
        }

        unit.RemoveComponent<CastState>();
        unit.RemoveTag<CastInterruptedTag>();
    }
}

/// <summary>
/// 持续施法系统
/// </summary>
public class ChannelingSystem : QuerySystem<ChannelState, CastState>, ITimedSystem
{
    public float Interval => 0.05f;

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref ChannelState channel, ref CastState cast, Entity unit) =>
        {
            if (cast.phase != CastPhase.Channeling) return;

            // 检查是否被打断
            if (unit.Tags.Has<CastInterruptedTag>())
            {
                InterruptChannel(unit, cast, channel);
                return;
            }

            channel.remaining -= Tick.deltaTime;
            cast.timer -= Tick.deltaTime;

            // 持续施法期间的效果（例如持续治疗）
            OnChannelTick(unit, channel);

            if (channel.remaining <= 0)
            {
                // 持续施法结束
                FinishChannel(unit, cast, channel);
            }
            else
            {
                unit.AddComponent(channel);
                unit.AddComponent(cast);
            }
        });
    }

    private void OnChannelTick(Entity unit, ChannelState channel)
    {
        // TODO: 实现持续施法每帧的效果
        // 例如：每 0.5 秒治疗一次
    }

    private void FinishChannel(Entity unit, CastState cast, ChannelState channel)
    {
        if (cast.ability.TryGetComponent<AbilityBase>(out var abilityBase))
        {
            abilityBase.state = AbilityState.Cooldown;
            abilityBase.currentCd = abilityBase.cooldown;
            cast.ability.AddComponent(abilityBase);
        }

        unit.RemoveComponent<ChannelState>();
        unit.RemoveComponent<CastState>();
    }

    private void InterruptChannel(Entity unit, CastState cast, ChannelState channel)
    {
        if (cast.ability.TryGetComponent<AbilityBase>(out var abilityBase))
        {
            abilityBase.state = AbilityState.Ready;
            cast.ability.AddComponent(abilityBase);
        }

        unit.RemoveComponent<ChannelState>();
        unit.RemoveComponent<CastState>();
        unit.RemoveTag<CastInterruptedTag>();
    }
}

using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;
using War3Frame.Helpers;

namespace War3Frame.Src.Systems;

/// <summary>
/// 施法请求处理系统，负责处理玩家或 AI 的施法意图。
/// </summary>
public class CastRequestSystem : QuerySystem<CastRequest, Position>, ITimedSystem
{
    /// <summary>
    /// 施法请求检查与启动的执行间隔。
    /// </summary>
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

            if (abilityBase.state != AbilityState.Ready)
            {
                unit.RemoveComponent<CastRequest>();
                return;
            }

            // 请求阶段只做预检查，不扣资源；真正扣除发生在 OnEffect 条件通过后。
            if (!AbilityCostHelper.CheckCost(unit, ability))
            {
                unit.RemoveComponent<CastRequest>();
                return;
            }

            var targetX = request.targetX;
            var targetY = request.targetY;
            if (!request.targetUnit.IsNull && request.targetUnit.TryGetComponent<Position>(out var targetPos))
            {
                targetX = targetPos.x;
                targetY = targetPos.y;
            }

            var dist = CalcDistance(pos.x, pos.y, targetX, targetY);
            var castRange = AbilityHelper.GetCastRange(ability);

            if (dist <= castRange)
            {
                ability.AddComponent(new AbilityTriggerInfo
                {
                    triggerType = AbilityTriggerType.ActiveCast
                });
                StartCasting(unit, request, abilityBase);
            }
            else
            {
                var commandToken = War3Frame.Src.Systems.Unit.MoveSystem.NextCommandToken();

                unit.AddComponent(new CastState
                {
                    phase = CastPhase.MovingToCast,
                    ability = ability,
                    targetUnit = request.targetUnit,
                    targetX = targetX,
                    targetY = targetY,
                    timer = 0f,
                    effectCommitted = false
                });

                unit.AddComponent(new MoveCommand
                {
                    targetX = targetX,
                    targetY = targetY,
                    arrivalDistance = castRange * 0.9f,
                    reason = MoveReason.CastingAbility,
                    orderType = MoveOrderType.Move,
                    commandToken = commandToken,
                    issued = false
                });

                unit.AddComponent(new MoveContinuation
                {
                    kind = MoveContinuationKind.CastAbility,
                    ability = ability,
                    targetUnit = request.targetUnit,
                    targetX = request.targetX,
                    targetY = request.targetY
                });
            }

            unit.RemoveComponent<CastRequest>();
        });
    }

    /// <summary>
    /// 开始施法流程，只进入前摇状态，不扣除资源。
    /// </summary>
    private static void StartCasting(Entity unit, CastRequest request, AbilityBase abilityBase)
    {
        var castTime = AbilityHelper.GetCastTime(request.ability);

        unit.AddComponent(new CastState
        {
            phase = CastPhase.Casting,
            ability = request.ability,
            targetUnit = request.targetUnit,
            targetX = request.targetX,
            targetY = request.targetY,
            timer = castTime,
            effectCommitted = false
        });

        request.ability.AddComponent(new AbilityFlowNodeInfo
        {
            nodeType = AbilityFlowNodeType.Cast
        });

        abilityBase.state = AbilityState.Casting;
        request.ability.AddComponent(abilityBase);
    }

    /// <summary>
    /// 计算两点之间的平面距离。
    /// </summary>
    private static float CalcDistance(float x1, float y1, float x2, float y2)
    {
        var dx = x2 - x1;
        var dy = y2 - y1;
        return (float)Math.Sqrt(dx * dx + dy * dy);
    }
}

/// <summary>
/// 移动后施法桥接系统。
/// </summary>
public class MoveToCastSystem : QuerySystem<CastState, MoveOutcome, MoveContinuation>
{
    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref CastState cast, ref MoveOutcome outcome, ref MoveContinuation continuation, Entity unit) =>
        {
            if (cast.phase != CastPhase.MovingToCast) return;
            if (continuation.kind != MoveContinuationKind.CastAbility) return;

            var ability = cast.ability;
            if (ability.IsNull || !ability.TryGetComponent<AbilityBase>(out var abilityBase)) return;

            if (IsControlled(unit))
            {
                CancelCastMovement(unit, cast);
                return;
            }

            if (unit.Tags.Has<CastInterruptedTag>())
            {
                CancelCastMovement(unit, cast);
                unit.RemoveTag<CastInterruptedTag>();
                return;
            }

            if (outcome.outcome == MoveOutcomeType.Arrived)
            {
                cast.phase = CastPhase.Casting;
                cast.timer = AbilityHelper.GetCastTime(ability);
                cast.effectCommitted = false;
                unit.AddComponent(cast);

                abilityBase.state = AbilityState.Casting;
                ability.AddComponent(abilityBase);

                unit.RemoveComponent<MoveOutcome>();
                unit.RemoveComponent<MoveContinuation>();
            }
            else if (outcome.outcome is MoveOutcomeType.Cancelled or MoveOutcomeType.Overridden or MoveOutcomeType.Interrupted or MoveOutcomeType.Failed)
            {
                CancelCastMovement(unit, cast);
                unit.RemoveComponent<MoveOutcome>();
                unit.RemoveComponent<MoveContinuation>();
            }
            else if (!cast.targetUnit.IsNull && cast.targetUnit.TryGetComponent<Position>(out var targetPos))
            {
                if (unit.TryGetComponent<MoveCommand>(out var cmd) && cmd.reason == MoveReason.CastingAbility)
                {
                    var newX = targetPos.x;
                    var newY = targetPos.y;
                    var dx = newX - cmd.targetX;
                    var dy = newY - cmd.targetY;
                    if (dx * dx + dy * dy > 100 * 100)
                    {
                        cmd.targetX = newX;
                        cmd.targetY = newY;
                        cmd.issued = false;
                        unit.AddComponent(cmd);

                        continuation.targetX = newX;
                        continuation.targetY = newY;
                        unit.AddComponent(continuation);

                        cast.targetX = newX;
                        cast.targetY = newY;
                        unit.AddComponent(cast);
                    }
                }
            }
        });
    }

    /// <summary>
    /// 检查单位是否处于控制效果中。
    /// </summary>
    private static bool IsControlled(Entity unit)
    {
        return ControlHelper.IsIncapacitated(unit);
    }

    /// <summary>
    /// 取消施法移动。
    /// </summary>
    private static void CancelCastMovement(Entity unit, CastState cast)
    {
        unit.RemoveComponent<CastState>();

        if (unit.TryGetComponent<MoveCommand>(out var moveCmd) && moveCmd.reason == MoveReason.CastingAbility)
            unit.RemoveComponent<MoveCommand>();

        if (cast.ability.TryGetComponent<AbilityBase>(out var abilityBase))
        {
            abilityBase.state = AbilityState.Ready;
            cast.ability.AddComponent(abilityBase);
        }
    }
}

/// <summary>
/// 施法前摇推进系统。
/// </summary>
public class CastingSystem : QuerySystem<CastState>, ITimedSystem
{
    /// <summary>
    /// 前摇推进间隔。
    /// </summary>
    public float Interval => 0.05f;

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref CastState cast, Entity unit) =>
        {
            if (cast.phase is not (CastPhase.Casting or CastPhase.Backswing)) return;

            if (unit.Tags.Has<CastInterruptedTag>())
            {
                InterruptCast(unit, cast);
                return;
            }

            cast.timer -= Tick.deltaTime;
            if (cast.timer > 0)
            {
                unit.AddComponent(cast);
                return;
            }

            if (cast.phase == CastPhase.Casting)
                CompleteCastPoint(unit, cast);
            else
                FinishCast(unit, cast);
        });
    }

    private static void CompleteCastPoint(Entity unit, CastState cast)
    {
        if (!TryCommitEffect(unit, cast))
        {
            ResetAbility(unit, cast);
            return;
        }

        var channelDuration = AbilityHelper.GetChannelDuration(cast.ability);
        if (channelDuration > 0f)
        {
            var tickInterval = AbilityHelper.GetChannelTickInterval(cast.ability);
            cast.phase = CastPhase.Channeling;
            cast.timer = channelDuration;
            cast.effectCommitted = true;
            unit.AddComponent(cast);

            unit.AddComponent(new ChannelState
            {
                remaining = channelDuration,
                duration = channelDuration,
                ability = cast.ability,
                tickInterval = tickInterval,
                tickTimer = tickInterval
            });

            SetAbilityState(cast.ability, AbilityState.Channeling);
            return;
        }

        StartBackswingOrFinish(unit, cast);
    }

    private static bool TryCommitEffect(Entity unit, CastState cast)
    {
        if (!AbilityCostHelper.CheckCost(unit, cast.ability))
            return false;

        AbilityCostHelper.ApplyCost(unit, cast.ability);
        TriggerBehaviorEffect(unit, cast, AbilityBehaviorTrigger.OnEffect);
        return true;
    }

    internal static void TriggerBehaviorEffect(Entity unit, CastState cast, AbilityBehaviorTrigger trigger)
    {
        if (!TryGetBehaviorEffect(cast.ability, trigger, out var effect))
            return;

        var previous = GetCurrentEffectSpec(cast.ability, out var hadPrevious);
        AbilityHelper.SetEffectSpec(cast.ability, effect.Inner);
        AbilityEffectHelper.CreateEffectEntity(unit, cast.ability, cast.targetUnit, cast.targetX, cast.targetY);
        RestoreEffectSpec(cast.ability, previous, hadPrevious);
    }

    private static bool TryGetBehaviorEffect(Entity ability, AbilityBehaviorTrigger trigger, out AbilityEffectSpec effect)
    {
        if (ability.TryGetComponent<AbilityBehaviorData>(out var data) && data.behaviors != null)
        {
            foreach (var behavior in data.behaviors)
            {
                if (behavior.trigger == trigger && behavior.effect != null)
                {
                    effect = behavior.effect;
                    return true;
                }
            }
        }

        effect = null!;
        return false;
    }

    private static EffectSpec? GetCurrentEffectSpec(Entity ability, out bool exists)
    {
        if (AbilityHelper.TryGetEffectSpec(ability, out var spec))
        {
            exists = true;
            return spec;
        }

        exists = false;
        return null;
    }

    private static void RestoreEffectSpec(Entity ability, EffectSpec? previous, bool hadPrevious)
    {
        if (hadPrevious && previous != null)
            AbilityHelper.SetEffectSpec(ability, previous);
    }

    internal static void StartBackswingOrFinish(Entity unit, CastState cast)
    {
        var backswing = AbilityHelper.GetBackswingDuration(cast.ability);
        if (backswing > 0f)
        {
            cast.phase = CastPhase.Backswing;
            cast.timer = backswing;
            cast.effectCommitted = true;
            unit.AddComponent(cast);
            SetAbilityState(cast.ability, AbilityState.Backswing);
            return;
        }

        FinishCast(unit, cast);
    }

    internal static void FinishCast(Entity unit, CastState cast)
    {
        TriggerBehaviorEffect(unit, cast, AbilityBehaviorTrigger.OnFinished);
        SetAbilityState(cast.ability, AbilityState.Cooldown);
        cast.ability.AddComponent(new AbilityCooldownState
        {
            remaining = AbilityHelper.GetCooldown(cast.ability)
        });
        unit.RemoveComponent<CastState>();
    }

    private static void InterruptCast(Entity unit, CastState cast)
    {
        TriggerBehaviorEffect(unit, cast, AbilityBehaviorTrigger.OnInterrupted);
        ResetAbility(unit, cast);
        unit.RemoveTag<CastInterruptedTag>();
    }

    private static void ResetAbility(Entity unit, CastState cast)
    {
        SetAbilityState(cast.ability, AbilityState.Ready);
        unit.RemoveComponent<CastState>();
    }

    private static void SetAbilityState(Entity ability, AbilityState state)
    {
        if (ability.TryGetComponent<AbilityBase>(out var abilityBase))
        {
            abilityBase.state = state;
            ability.AddComponent(abilityBase);
        }
    }
}

/// <summary>
/// 持续施法系统。
/// </summary>
public class ChannelingSystem : QuerySystem<ChannelState, CastState>, ITimedSystem
{
    /// <summary>
    /// 持续施法推进间隔。
    /// </summary>
    public float Interval => 0.05f;

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref ChannelState channel, ref CastState cast, Entity unit) =>
        {
            if (cast.phase != CastPhase.Channeling) return;

            if (unit.Tags.Has<CastInterruptedTag>())
            {
                InterruptChannel(unit, cast);
                return;
            }

            channel.remaining -= Tick.deltaTime;
            cast.timer -= Tick.deltaTime;
            AdvanceChannelTick(unit, cast, ref channel, Tick.deltaTime);

            if (channel.remaining <= 0)
            {
                FinishChannel(unit, cast);
                return;
            }

            unit.AddComponent(channel);
            unit.AddComponent(cast);
        });
    }

    private static void AdvanceChannelTick(Entity unit, CastState cast, ref ChannelState channel, float deltaTime)
    {
        if (channel.tickInterval <= 0f)
            return;

        channel.tickTimer -= deltaTime;
        while (channel.tickTimer <= 0f && channel.remaining > 0f)
        {
            CastingSystem.TriggerBehaviorEffect(unit, cast, AbilityBehaviorTrigger.OnChannelTick);
            channel.tickTimer += channel.tickInterval;
        }
    }

    private static void FinishChannel(Entity unit, CastState cast)
    {
        unit.RemoveComponent<ChannelState>();
        CastingSystem.StartBackswingOrFinish(unit, cast);
    }

    private static void InterruptChannel(Entity unit, CastState cast)
    {
        CastingSystem.TriggerBehaviorEffect(unit, cast, AbilityBehaviorTrigger.OnInterrupted);
        SetAbilityState(cast.ability, AbilityState.Ready);
        unit.RemoveComponent<ChannelState>();
        unit.RemoveComponent<CastState>();
        unit.RemoveTag<CastInterruptedTag>();
    }

    private static void SetAbilityState(Entity ability, AbilityState state)
    {
        if (ability.TryGetComponent<AbilityBase>(out var abilityBase))
        {
            abilityBase.state = state;
            ability.AddComponent(abilityBase);
        }
    }
}

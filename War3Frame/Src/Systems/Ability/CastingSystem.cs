using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;
using War3Frame.Helpers;
using War3Frame.Systems;

namespace War3Frame.Src.Systems;

/// <summary>
/// 施法请求处理系统，负责处理玩家或 AI 的施法意图。
/// </summary>
[SystemRegister(SystemKind.Immediate, 3)]
public class CastRequestSystem : QuerySystem<CastRequest, Position>, ITimedSystem
{
    private readonly List<(Entity unit, CastRequest request, Position position)> _pending = new();

    /// <summary>
    /// 施法请求检查与启动的执行间隔。
    /// </summary>
    public float Interval { get; } = 0.02f;

    protected override void OnUpdate()
    {
        _pending.Clear();
        Query.ForEachEntity((ref CastRequest request, ref Position pos, Entity unit) =>
        {
            _pending.Add((unit, request, pos));
        });

        foreach (var pending in _pending)
            Process(pending.unit, pending.request, pending.position);
    }

    /// <summary>
    /// 在请求查询结束后校验并启动施法，避免查询期间执行结构变更。
    /// </summary>
    private static void Process(Entity unit, CastRequest request, Position pos)
    {
        var ability = request.ability;
        if (ability.IsNull || !ability.TryGetComponent<AbilityBase>(out var abilityBase)
            || !IsValidItemCast(unit, request)
            || abilityBase.state != AbilityState.Ready
            || !AbilityCostHelper.CheckCost(unit, ability))
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

        var castRange = AbilityHelper.GetCastRange(ability);
        if (IsInRange(pos.x, pos.y, targetX, targetY, castRange))
        {
            ability.AddComponent(new AbilityTriggerInfo { triggerType = AbilityTriggerType.ActiveCast });
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
                effectCommitted = false,
                itemOrigin = request.itemOrigin
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
            effectCommitted = false,
            itemOrigin = request.itemOrigin
        });

        request.ability.AddComponent(new AbilityFlowNodeInfo
        {
            nodeType = AbilityFlowNodeType.Cast
        });

        abilityBase.state = AbilityState.Casting;
        request.ability.AddComponent(abilityBase);
    }

    /// <summary>
    /// Item 来源请求在消费时重新校验 owner、Item Link 和删除等待状态。
    /// </summary>
    private static bool IsValidItemCast(Entity unit, CastRequest request)
    {
        var origin = request.itemOrigin;
        if (origin.item.IsNull)
        {
            return !request.ability.TryGetComponent<AbilityMountInfo>(out var mount)
                   || mount.mountType != AbilityMountType.ItemGranted;
        }
        if (origin.user != unit
            || !ReferenceEquals(unit.Store, origin.item.Store)
            || !ReferenceEquals(unit.Store, request.ability.Store)
            || origin.item.Tags.Has<ItemDestroyPendingTag>())
            return false;
        if (!origin.item.TryGetComponent<ItemActiveAbility>(out var active) || active.ability != request.ability)
            return false;
        return request.ability.TryGetComponent<AbilityMountInfo>(out var companionMount)
               && companionMount.mountType == AbilityMountType.ItemGranted
               && !request.ability.HasComponent<AbilitySlotIndex>()
               && request.ability.TryGetComponent<AbilityOwner>(out var owner)
               && owner.owner == unit;
    }

    /// <summary>
    /// 判断两点是否在范围内，用平方距离比较避免 sqrt。
    /// </summary>
    private static bool IsInRange(float x1, float y1, float x2, float y2, float range)
    {
        var dx = x2 - x1;
        var dy = y2 - y1;
        return dx * dx + dy * dy <= range * range;
    }
}

/// <summary>
/// 移动后施法桥接系统。
/// </summary>
[SystemRegister(SystemKind.Immediate, 4)]
public class MoveToCastSystem : QuerySystem<CastState, MoveOutcome, MoveContinuation>
{
    private readonly List<Entity> _pending = new();

    protected override void OnUpdate()
    {
        _pending.Clear();
        Query.ForEachEntity((ref CastState cast, ref MoveOutcome outcome, ref MoveContinuation continuation, Entity unit) =>
        {
            _pending.Add(unit);
        });

        foreach (var unit in _pending)
            Process(unit);
    }

    /// <summary>
    /// 在查询结束后消费移动结果并推进施法状态。
    /// </summary>
    private static void Process(Entity unit)
    {
        if (!unit.TryGetComponent<CastState>(out var cast)
            || !unit.TryGetComponent<MoveOutcome>(out var outcome)
            || !unit.TryGetComponent<MoveContinuation>(out var continuation)
            || cast.phase != CastPhase.MovingToCast
            || continuation.kind != MoveContinuationKind.CastAbility)
        {
            return;
        }

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
[SystemRegister(SystemKind.Interval, 20)]
public class CastingSystem : QuerySystem<CastState>, ITimedSystem
{
    private readonly List<(Entity unit, CastState cast)> _pending = new();

    /// <summary>
    /// 前摇推进间隔。
    /// </summary>
    public float Interval => 0.05f;

    protected override void OnUpdate()
    {
        _pending.Clear();
        Query.ForEachEntity((ref CastState cast, Entity unit) =>
        {
            _pending.Add((unit, cast));
        });

        foreach (var pending in _pending)
        {
            var unit = pending.unit;
            var cast = pending.cast;
            if (cast.phase is not (CastPhase.Casting or CastPhase.Backswing)) continue;

            if (unit.Tags.Has<CastInterruptedTag>())
            {
                InterruptCast(unit, cast);
                continue;
            }

            cast.timer -= Tick.deltaTime;
            if (cast.timer > 0)
            {
                unit.AddComponent(cast);
                continue;
            }

            if (cast.phase == CastPhase.Casting)
                CompleteCastPoint(unit, cast);
            else
                FinishCast(unit, cast);
        }
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
        AbilityEffectHelper.TriggerBehaviorEffect(
            unit,
            cast.ability,
            trigger,
            cast.targetUnit,
            cast.targetX,
            cast.targetY,
            cast.itemOrigin);
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
        AbilityHelper.EnterCooldownOrReady(cast.ability);
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
[SystemRegister(SystemKind.Interval, 21)]
public class ChannelingSystem : QuerySystem<ChannelState, CastState>, ITimedSystem
{
    private readonly List<(Entity unit, ChannelState channel, CastState cast)> _pending = new();

    /// <summary>
    /// 持续施法推进间隔。
    /// </summary>
    public float Interval => 0.05f;

    protected override void OnUpdate()
    {
        _pending.Clear();
        Query.ForEachEntity((ref ChannelState channel, ref CastState cast, Entity unit) =>
        {
            _pending.Add((unit, channel, cast));
        });

        foreach (var pending in _pending)
        {
            var unit = pending.unit;
            var channel = pending.channel;
            var cast = pending.cast;
            if (cast.phase != CastPhase.Channeling) continue;

            if (unit.Tags.Has<CastInterruptedTag>())
            {
                InterruptChannel(unit, cast);
                continue;
            }

            channel.remaining -= Tick.deltaTime;
            cast.timer -= Tick.deltaTime;
            AdvanceChannelTick(unit, cast, ref channel, Tick.deltaTime);

            if (channel.remaining <= 0)
            {
                FinishChannel(unit, cast);
                continue;
            }

            unit.AddComponent(channel);
            unit.AddComponent(cast);
        }
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

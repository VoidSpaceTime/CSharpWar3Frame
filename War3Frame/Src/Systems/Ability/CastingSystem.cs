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
            || UnitHelper.IsDead(unit)
            || !IsValidItemCast(unit, request)
            || abilityBase.state != AbilityState.Ready
            || !AbilityCostHelper.CheckCost(unit, ability)
            || !ControlHelper.CanCast(unit))
        {
            // 无效请求直接丢弃，不影响当前施法。
            unit.RemoveComponent<CastRequest>();
            return;
        }

        // 新的有效请求打断当前施法（War3 新命令语义），避免旧技能卡在非 Ready 状态。
        if (unit.TryGetComponent<CastState>(out var current))
        {
            CastInterruption.End(unit, current);
            abilityBase = ability.GetComponent<AbilityBase>();
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
            StartCasting(unit, request, abilityBase, targetX, targetY);
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
                targetY = request.targetY,
                commandToken = commandToken
            });
        }

        unit.RemoveComponent<CastRequest>();
    }

    /// <summary>
    /// 开始施法流程，只进入前摇状态，不扣除资源。
    /// </summary>
    private static void StartCasting(Entity unit, CastRequest request, AbilityBase abilityBase, float targetX, float targetY)
    {
        var castTime = AbilityHelper.GetCastTime(request.ability);

        unit.AddComponent(new CastState
        {
            phase = CastPhase.Casting,
            ability = request.ability,
            targetUnit = request.targetUnit,
            targetX = targetX,
            targetY = targetY,
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
public class MoveToCastSystem : QuerySystem<CastState>
{
    private readonly List<Entity> _pending = new();

    protected override void OnUpdate()
    {
        _pending.Clear();
        Query.ForEachEntity((ref CastState cast, Entity unit) =>
        {
            // 走位阶段的施法，以及仍挂着施法后续的单位（施法已推进时清理残留）。
            if (cast.phase == CastPhase.MovingToCast
                || unit.TryGetComponent<MoveContinuation>(out var continuation)
                   && continuation.kind == MoveContinuationKind.CastAbility)
            {
                _pending.Add(unit);
            }
        });

        foreach (var unit in _pending)
            Process(unit);
    }

    /// <summary>
    /// 在查询结束后推进走位施法：取消条件优先；只消费本次移动 token 的结果；
    /// 本次移动命令已不存在或已被替换时取消；仍在移动时跟随移动的目标。
    /// </summary>
    private static void Process(Entity unit)
    {
        if (!unit.TryGetComponent<CastState>(out var cast))
            return;

        var hasOwnContinuation = unit.TryGetComponent<MoveContinuation>(out var continuation)
            && continuation.kind == MoveContinuationKind.CastAbility
            && continuation.ability == cast.ability;

        if (cast.phase != CastPhase.MovingToCast)
        {
            // 施法已推进：只清理遗留的施法后续与移动残留。
            if (continuation.kind == MoveContinuationKind.CastAbility)
                CastMovement.Clear(unit, continuation.ability);
            return;
        }

        // 走位阶段却没有本技能的后续：后续已被其他移动工作流（如移动任务）或其他技能替换，施法随之取消。
        if (!hasOwnContinuation)
        {
            CancelCastMovement(unit, cast);
            return;
        }

        var ability = cast.ability;
        if (ability.IsNull || !ability.TryGetComponent<AbilityBase>(out var abilityBase)
            || UnitHelper.IsDead(unit) || IsControlled(unit))
        {
            CancelCastMovement(unit, cast);
            return;
        }

        if (unit.Tags.Has<CastInterruptedTag>())
        {
            CancelCastMovement(unit, cast);
            return;
        }

        if (unit.TryGetComponent<MoveOutcome>(out var outcome))
        {
            if (continuation.commandToken != 0 && outcome.commandToken != continuation.commandToken)
            {
                // 更早命令的结果：丢弃，继续等待本次移动。
                unit.RemoveComponent<MoveOutcome>();
            }
            else if (outcome.outcome == MoveOutcomeType.Arrived)
            {
                cast.phase = CastPhase.Casting;
                cast.timer = AbilityHelper.GetCastTime(ability);
                cast.effectCommitted = false;
                unit.AddComponent(cast);

                abilityBase.state = AbilityState.Casting;
                ability.AddComponent(abilityBase);

                CastMovement.Clear(unit, ability);
                return;
            }
            else
            {
                CancelCastMovement(unit, cast);
                return;
            }
        }

        // 本次移动命令已被移除或被其他 token 替换，且没有可消费的结果：再等也不会到达，取消施法。
        // MoveSystem 移除本命令时一定写入同 token 结果，会先被上面的分支处理，因此这里不会误判；跟随目标重下发时 token 不变。
        if (continuation.commandToken != 0
            && (!unit.TryGetComponent<MoveCommand>(out var command) || command.commandToken != continuation.commandToken))
        {
            CancelCastMovement(unit, cast);
            return;
        }

        FollowTarget(unit, cast, continuation);
    }

    /// <summary>
    /// 目标单位移动超过阈值时同步更新移动、后续与施法目标；token 不变，只重新下发原生移动。
    /// </summary>
    private static void FollowTarget(Entity unit, CastState cast, MoveContinuation continuation)
    {
        if (cast.targetUnit.IsNull || !cast.targetUnit.TryGetComponent<Position>(out var targetPos)
            || !unit.TryGetComponent<MoveCommand>(out var cmd) || cmd.reason != MoveReason.CastingAbility)
        {
            return;
        }

        var dx = targetPos.x - cmd.targetX;
        var dy = targetPos.y - cmd.targetY;
        if (dx * dx + dy * dy <= 100 * 100)
            return;

        cmd.targetX = targetPos.x;
        cmd.targetY = targetPos.y;
        cmd.issued = false;
        unit.AddComponent(cmd);
        continuation.targetX = targetPos.x;
        continuation.targetY = targetPos.y;
        unit.AddComponent(continuation);
        cast.targetX = targetPos.x;
        cast.targetY = targetPos.y;
        unit.AddComponent(cast);
    }

    /// <summary>
    /// 检查单位是否处于控制效果中。
    /// </summary>
    private static bool IsControlled(Entity unit)
    {
        return ControlHelper.IsIncapacitated(unit);
    }

    /// <summary>
    /// 取消走位施法：清理施法状态、打断标记与本次移动残留，技能回到 Ready（不触发 OnInterrupted，与既有取消语义一致）。
    /// </summary>
    internal static void CancelCastMovement(Entity unit, CastState cast)
    {
        unit.RemoveComponent<CastState>();
        unit.RemoveTag<CastInterruptedTag>();
        CastMovement.Clear(unit, cast.ability);

        if (!cast.ability.IsNull && cast.ability.TryGetComponent<AbilityBase>(out var abilityBase))
        {
            abilityBase.state = AbilityState.Ready;
            cast.ability.AddComponent(abilityBase);
        }
    }
}

/// <summary>
/// 走位施法的移动残留清理：该技能的后续、施法原因的移动命令及其执行状态与移动标记、本次移动结果。
/// </summary>
internal static class CastMovement
{
    internal static void Clear(Entity unit, Entity ability)
    {
        var removedContinuation = false;
        if (unit.TryGetComponent<MoveContinuation>(out var continuation)
            && continuation.kind == MoveContinuationKind.CastAbility
            && continuation.ability == ability)
        {
            unit.RemoveComponent<MoveContinuation>();
            removedContinuation = true;
        }

        if (unit.TryGetComponent<MoveCommand>(out var move) && move.reason == MoveReason.CastingAbility)
        {
            unit.RemoveComponent<MoveCommand>();
            unit.RemoveComponent<MoveExecutionState>();
            unit.RemoveTag<MovingTag>();
        }

        // 其他工作流（如移动任务）仍持有后续时，保留它的移动结果。
        if (removedContinuation || !unit.HasComponent<MoveContinuation>())
            unit.RemoveComponent<MoveOutcome>();
    }
}

/// <summary>
/// 按施法阶段结束单位当前施法，供新施法请求替换当前施法时使用。
/// </summary>
internal static class CastInterruption
{
    internal static void End(Entity unit, CastState cast)
    {
        switch (cast.phase)
        {
            case CastPhase.MovingToCast:
                MoveToCastSystem.CancelCastMovement(unit, cast);
                break;
            case CastPhase.Channeling:
                ChannelingSystem.InterruptChannel(unit, cast);
                break;
            default:
                CastingSystem.InterruptCast(unit, cast);
                break;
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

            // 技能实体已被删除：只移除施法状态，不再读取技能组件或触发行为效果。
            if (cast.ability.IsNull)
            {
                unit.RemoveComponent<CastState>();
                unit.RemoveTag<CastInterruptedTag>();
                continue;
            }

            if (UnitHelper.IsDead(unit) || unit.Tags.Has<CastInterruptedTag>())
            {
                InterruptCast(unit, cast);
                continue;
            }

            // 受控打断：眩晕/击飞（禁止行动）期间前摇吟唱应被打断。
            // 仅在 Casting 阶段打断；Backswing（后摇，效果已提交）保持现状推进完成。
            if (cast.phase == CastPhase.Casting && ControlHelper.IsIncapacitated(unit))
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
        // 目标合法性复查：单位目标技能在吟唱完成（生效点提交）前若目标已死亡，取消施法。
        // 点目标/无目标技能（targetUnit 为空）不受影响。
        if (!cast.targetUnit.IsNull && !UnitHelper.IsAlive(cast.targetUnit))
        {
            InterruptCast(unit, cast);
            return;
        }

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

    /// <summary>
    /// 打断前摇或后摇。效果已提交（后摇）时按冷却值进入冷却；前摇未提交时回到 Ready、不进冷却。
    /// </summary>
    internal static void InterruptCast(Entity unit, CastState cast)
    {
        if (!cast.ability.IsNull)
            TriggerBehaviorEffect(unit, cast, AbilityBehaviorTrigger.OnInterrupted);

        if (cast.effectCommitted && !cast.ability.IsNull)
        {
            AbilityHelper.EnterCooldownOrReady(cast.ability);
            unit.RemoveComponent<CastState>();
        }
        else
        {
            ResetAbility(unit, cast);
        }

        unit.RemoveTag<CastInterruptedTag>();
    }

    private static void ResetAbility(Entity unit, CastState cast)
    {
        SetAbilityState(cast.ability, AbilityState.Ready);
        unit.RemoveComponent<CastState>();
    }

    private static void SetAbilityState(Entity ability, AbilityState state)
    {
        if (!ability.IsNull && ability.TryGetComponent<AbilityBase>(out var abilityBase))
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

            // 技能实体已被删除：只移除施法与引导状态。
            if (cast.ability.IsNull)
            {
                unit.RemoveComponent<ChannelState>();
                unit.RemoveComponent<CastState>();
                unit.RemoveTag<CastInterruptedTag>();
                continue;
            }

            if (UnitHelper.IsDead(unit) || unit.Tags.Has<CastInterruptedTag>())
            {
                InterruptChannel(unit, cast);
                continue;
            }

            // 受控打断引导：眩晕/沉默/击飞期间持续引导应被打断（沉默可打断引导，语义区别于前摇吟唱）。
            if (!ControlHelper.CanCast(unit))
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

    /// <summary>浮点累计误差容差，避免整数倍边界上的 tick 因舍入被提前或推迟。</summary>
    private const float TickEpsilon = 0.0001f;

    /// <summary>
    /// 推进引导 tick：按“下一跳时刻不超过引导总时长”判定，使 tick 次数等于 floor(总时长 / 间隔)，末跳不被边界吞掉。
    /// </summary>
    private static void AdvanceChannelTick(Entity unit, CastState cast, ref ChannelState channel, float deltaTime)
    {
        if (channel.tickInterval <= 0f)
            return;

        channel.tickTimer -= deltaTime;
        while (channel.tickTimer <= TickEpsilon)
        {
            // 下一跳距引导开始的时刻 = 已经过时长 + 剩余等待（tickTimer ≤ 0 时即本跳时刻）。
            var tickAt = channel.duration - channel.remaining + channel.tickTimer;
            if (tickAt > channel.duration + TickEpsilon)
                break;

            CastingSystem.TriggerBehaviorEffect(unit, cast, AbilityBehaviorTrigger.OnChannelTick);
            channel.tickTimer += channel.tickInterval;
        }
    }

    private static void FinishChannel(Entity unit, CastState cast)
    {
        unit.RemoveComponent<ChannelState>();
        CastingSystem.StartBackswingOrFinish(unit, cast);
    }

    /// <summary>
    /// 打断引导。进入引导前效果已提交，按冷却值进入冷却（冷却为 0 时回到 Ready）。
    /// </summary>
    internal static void InterruptChannel(Entity unit, CastState cast)
    {
        if (!cast.ability.IsNull)
        {
            CastingSystem.TriggerBehaviorEffect(unit, cast, AbilityBehaviorTrigger.OnInterrupted);
            AbilityHelper.EnterCooldownOrReady(cast.ability);
        }

        unit.RemoveComponent<ChannelState>();
        unit.RemoveComponent<CastState>();
        unit.RemoveTag<CastInterruptedTag>();
    }
}

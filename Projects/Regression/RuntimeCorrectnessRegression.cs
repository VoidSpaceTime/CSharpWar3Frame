using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;
using War3Frame.Components.Item;
using War3Frame.Helpers;
using War3Frame.Src.Components;
using War3Frame.Src.Systems;
using War3Frame.Src.Systems.Time;
using War3Frame.Src.Systems.Unit;
using War3Frame.Systems;
using War3Frame.Systems.Native;
using War3Frame.Systems.Unit;

namespace War3Frame.Regression;

/// <summary>fix-runtime-correctness 的缺陷回归：每条断言对应 proposal 中的一个编号。</summary>
internal static class RuntimeCorrectnessRegression
{
    private const string Group = "correctness";

    /// <summary>注册运行时正确性回归场景。</summary>
    internal static void Register(List<RegressionCase> tests)
    {
        tests.Add(new(Group, "A1-move-override-keeps-new-command", MoveOverrideKeepsNewCommand));
        tests.Add(new(Group, "A1-move-task-ignores-stale-override", MoveTaskIgnoresStaleOverride));
        tests.Add(new(Group, "A2-stale-arrival-does-not-start-cast", StaleArrivalDoesNotStartCast));
        tests.Add(new(Group, "A3-cancelled-cast-move-leaves-no-residue", CancelledCastMoveLeavesNoResidue));
        tests.Add(new(Group, "A3-cast-move-follows-target", CastMoveFollowsTarget));
        tests.Add(new(Group, "A3-replaced-cast-move-cancels-cast", ReplacedCastMoveCancelsCast));
        tests.Add(new(Group, "A4-new-request-replaces-current-cast", NewRequestReplacesCurrentCast));
        tests.Add(new(Group, "A5-dead-caster-and-mover", DeadCasterAndMover));
        tests.Add(new(Group, "A6-committed-interrupt-enters-cooldown", CommittedInterruptEntersCooldown));
        tests.Add(new(Group, "A7-in-range-cast-uses-target-position", InRangeCastUsesTargetPosition));
        tests.Add(new(Group, "A8-removing-ability-mid-cast", RemovingAbilityMidCast));
        tests.Add(new(Group, "B1-remove-by-key-outside-query", RemoveByKeyOutsideQuery));
        tests.Add(new(Group, "B2-sourceless-damage", SourcelessDamage));
        tests.Add(new(Group, "B3-sync-request-deleted-on-throw", SyncRequestDeletedOnThrow));
        tests.Add(new(Group, "C1-no-health-not-killed", NoHealthNotKilled));
        tests.Add(new(Group, "C2-corpse-no-regen-no-heal", CorpseNoRegenNoHeal));
        tests.Add(new(Group, "C3-reborn-and-pool-restore-health", RebornAndPoolRestoreHealth));
        tests.Add(new(Group, "C4-aura-skips-dead", AuraSkipsDead));
        tests.Add(new(Group, "C5-chain-skips-corpse", ChainSkipsCorpse));
        tests.Add(new(Group, "C6-current-clamped-when-max-drops", CurrentClampedWhenMaxDrops));
        tests.Add(new(Group, "D1-permanent-buff-stacks", PermanentBuffStacks));
        tests.Add(new(Group, "D2-same-buff-id-different-attrs", SameBuffIdDifferentAttrs));
        tests.Add(new(Group, "D3-ability-contribution-removed", AbilityContributionRemoved));
        tests.Add(new(Group, "E1-parabolic-projectile", ParabolicProjectile));
        tests.Add(new(Group, "E2-line-parent-does-not-settle", LineParentDoesNotSettle));
        tests.Add(new(Group, "E3-child-effect-target-position", ChildEffectTargetPosition));
        tests.Add(new(Group, "E4-effect-pipeline-uses-owner-store", EffectPipelineUsesOwnerStore));
        tests.Add(new(Group, "E5-periodic-timing", PeriodicTiming));
        tests.Add(new(Group, "F1-group-query-reentrant", GroupQueryReentrant));
        tests.Add(new(Group, "F2-bootstrap-has-no-debug-output", BootstrapHasNoDebugOutput));
        tests.Add(new(Group, "F3-pump-drops-input-without-root", PumpDropsInputWithoutRoot));
        tests.Add(new(Group, "F4-pickup-same-slot", PickupSameSlot));
    }

    // ---------------- A 施法 / 移动 ----------------

    /// <summary>A1：原生单位移动中写入新命令，旧 token 被顶替，新命令保留并下发。</summary>
    private static void MoveOverrideKeepsNewCommand()
    {
        var store = new EntityStore();
        var unit = Caster(store);
        unit.AddComponent(new UnitNative());
        var root = new SystemRoot(store);
        root.Add(new MoveSystem());

        UnitHelper.MoveToTask(unit, 1000, 0, 10);
        var first = unit.GetComponent<MoveCommand>().commandToken;
        root.Update(new UpdateTick(.1f, .1f));
        Check.That(unit.GetComponent<MoveExecutionState>().commandToken == first, "first command is issued");

        UnitHelper.MoveToTask(unit, 0, 1000, 10);
        var second = unit.GetComponent<MoveCommand>().commandToken;
        root.Update(new UpdateTick(.1f, .2f));

        Check.That(unit.TryGetComponent<MoveCommand>(out var command) && command.commandToken == second,
            "the replacing command survives");
        Check.That(unit.GetComponent<MoveOutcome>().commandToken == first
            && unit.GetComponent<MoveOutcome>().outcome == MoveOutcomeType.Overridden,
            "the override outcome carries the old token");
        Check.That(unit.GetComponent<MoveNativeRequest>().commandToken == second,
            "the replacing command is issued to native");
        Check.That(unit.GetComponent<MoveExecutionState>().commandToken == second,
            "execution state follows the new command");
    }

    /// <summary>A1/A2：移动任务被新任务替换时，旧 token 的顶替结果不取消新任务。</summary>
    private static void MoveTaskIgnoresStaleOverride()
    {
        var store = new EntityStore();
        var unit = Caster(store);
        unit.AddComponent(new UnitNative());
        var root = new SystemRoot(store);
        root.Add(new MoveSystem());
        root.Add(new MoveToTaskSystem());

        UnitHelper.MoveToTask(unit, 1000, 0, 10);
        root.Update(new UpdateTick(.1f, .1f));
        UnitHelper.MoveToTask(unit, 0, 1000, 10);
        root.Update(new UpdateTick(.1f, .2f));
        root.Update(new UpdateTick(.1f, .3f));

        Check.That(!unit.HasComponent<MoveTaskState>(), "the second task is neither completed nor cancelled");
        Check.That(unit.HasComponent<MoveCommand>() && unit.HasComponent<MoveContinuation>(),
            "the second task keeps moving");

        unit.GetComponent<Position>().y = 1000;
        root.Update(new UpdateTick(.1f, .4f));
        root.Update(new UpdateTick(.1f, .5f));
        Check.That(unit.TryGetComponent<MoveTaskState>(out var state) && state.completed,
            "the second task completes on arrival");
    }

    /// <summary>A2：单位身上遗留的旧到达结果不能让超距施法原地开始吟唱。</summary>
    private static void StaleArrivalDoesNotStartCast()
    {
        var store = new EntityStore();
        var unit = Caster(store);
        var ability = Ability(store, "a2", range: 100, castPoint: .1f);
        unit.AddComponent(new MoveOutcome { commandToken = -42, outcome = MoveOutcomeType.Arrived });
        var root = CastRoot(store);

        unit.AddComponent(new CastRequest { ability = ability, targetX = 2000 });
        root.Update(new UpdateTick(.05f, .05f));

        Check.That(unit.TryGetComponent<CastState>(out var cast) && cast.phase == CastPhase.MovingToCast,
            "out-of-range cast moves first");
        Check.That(unit.HasComponent<MoveCommand>(), "the move command is kept");
    }

    /// <summary>A3：走位施法被眩晕取消后不留残留，解除眩晕后可以再次走位施法。</summary>
    private static void CancelledCastMoveLeavesNoResidue()
    {
        var store = new EntityStore();
        var unit = Caster(store);
        var ability = Ability(store, "a3", range: 100, castPoint: 5);
        var root = CastRoot(store);

        unit.AddComponent(new CastRequest { ability = ability, targetX = 1000 });
        root.Update(new UpdateTick(.05f, .05f));
        Check.That(unit.GetComponent<CastState>().phase == CastPhase.MovingToCast, "fixture walks to cast");

        var stun = AttributeHelper.GetOrCreateAttr(unit, AttributeHelper.Stun, 1);
        root.Update(new UpdateTick(.05f, .1f));
        root.Update(new UpdateTick(.05f, .15f));
        Check.That(!unit.HasComponent<CastState>(), "stun cancels the cast");
        Check.That(!unit.HasComponent<MoveOutcome>() && !unit.HasComponent<MoveContinuation>()
            && !unit.HasComponent<MoveCommand>(), "no move residue after cancellation");
        Check.That(ability.GetComponent<AbilityBase>().state == AbilityState.Ready, "ability returns to ready");

        ref var stunValue = ref stun.GetComponent<AttrValue>();
        stunValue.baseValue = 0;
        stunValue.finalValue = 0;
        unit.AddComponent(new CastRequest { ability = ability, targetX = 1000 });
        root.Update(new UpdateTick(.05f, .2f));
        Check.That(unit.TryGetComponent<CastState>(out var again) && again.phase == CastPhase.MovingToCast,
            "a second cast walks again");
        unit.GetComponent<Position>().x = 950;
        root.Update(new UpdateTick(.05f, .25f));
        root.Update(new UpdateTick(.05f, .3f));
        Check.That(unit.TryGetComponent<CastState>(out var arrived) && arrived.phase == CastPhase.Casting,
            "the second cast starts after arrival");
    }

    /// <summary>A3：走位施法期间目标移动超过阈值时，移动目标跟随更新。</summary>
    private static void CastMoveFollowsTarget()
    {
        var store = new EntityStore();
        var unit = Caster(store);
        var target = Caster(store, 1000);
        var ability = Ability(store, "a3-follow", range: 100, castPoint: 5);
        var root = CastRoot(store);

        unit.AddComponent(new CastRequest { ability = ability, targetUnit = target });
        root.Update(new UpdateTick(.05f, .05f));
        Check.Near(unit.GetComponent<MoveCommand>().targetX, 1000, "walks toward the target");

        target.GetComponent<Position>().x = 1500;
        root.Update(new UpdateTick(.05f, .1f));
        Check.Near(unit.GetComponent<MoveCommand>().targetX, 1500, "move target follows the unit target");
        Check.Near(unit.GetComponent<CastState>().targetX, 1500, "cast target follows the unit target");
    }

    /// <summary>
    /// A3：走位施法的移动被其他命令替换时取消施法，不残留 MovingToCast：
    /// (1) 外部直接写入新 token 的 MoveCommand；(2) 移动任务替换了施法后续。
    /// </summary>
    private static void ReplacedCastMoveCancelsCast()
    {
        var store = new EntityStore();
        var root = CastRoot(store);
        root.Add(new MoveToTaskSystem());

        var external = Caster(store);
        var first = Ability(store, "a3-external", range: 100, castPoint: 5);
        external.AddComponent(new CastRequest { ability = first, targetX = 1000 });
        root.Update(new UpdateTick(.05f, .05f));
        Check.That(external.GetComponent<CastState>().phase == CastPhase.MovingToCast, "fixture walks to cast");
        external.AddComponent(new MoveCommand
        {
            targetX = 0,
            targetY = 0,
            arrivalDistance = 10,
            reason = MoveReason.PlayerCommand,
            orderType = MoveOrderType.Move,
            commandToken = MoveSystem.NextCommandToken()
        });
        root.Update(new UpdateTick(.05f, .1f));
        root.Update(new UpdateTick(.05f, .15f));
        Check.That(!external.HasComponent<CastState>(), "an externally replaced move cancels the cast");
        Check.That(first.GetComponent<AbilityBase>().state == AbilityState.Ready, "the cancelled ability is ready");

        var tasked = Caster(store);
        var second = Ability(store, "a3-task", range: 100, castPoint: 5);
        tasked.AddComponent(new CastRequest { ability = second, targetX = 1000 });
        root.Update(new UpdateTick(.05f, .2f));
        UnitHelper.MoveToTask(tasked, 0, 500, 10);
        root.Update(new UpdateTick(.05f, .25f));
        Check.That(!tasked.HasComponent<CastState>(), "a move task replacing the cast move cancels the cast");
        Check.That(tasked.TryGetComponent<MoveContinuation>(out var continuation)
            && continuation.kind == MoveContinuationKind.ExecuteTask && tasked.HasComponent<MoveCommand>(),
            "the move task keeps its own command and continuation");
    }

    /// <summary>A4：施法中收到有效请求时打断当前施法；无效请求不打断。</summary>
    private static void NewRequestReplacesCurrentCast()
    {
        var store = new EntityStore();
        var unit = Caster(store);
        var a = Ability(store, "a4-a", range: 1000, castPoint: 5);
        var b = Ability(store, "a4-b", range: 1000, castPoint: 5);
        var cooling = Ability(store, "a4-c", range: 1000, castPoint: 5);
        cooling.GetComponent<AbilityBase>().state = AbilityState.Cooldown;
        var root = CastRoot(store);

        unit.AddComponent(new CastRequest { ability = a });
        root.Update(new UpdateTick(.05f, .05f));
        unit.AddComponent(new CastRequest { ability = cooling });
        root.Update(new UpdateTick(.05f, .1f));
        Check.That(unit.GetComponent<CastState>().ability == a
            && a.GetComponent<AbilityBase>().state == AbilityState.Casting, "an invalid request does not interrupt");

        unit.AddComponent(new CastRequest { ability = b });
        root.Update(new UpdateTick(.05f, .15f));
        Check.That(unit.GetComponent<CastState>().ability == b, "the valid request starts casting");
        Check.That(a.GetComponent<AbilityBase>().state == AbilityState.Ready, "the replaced ability returns to ready");
    }

    /// <summary>A5：死亡单位不完成前摇、不接受新请求，移动命令被打断。</summary>
    private static void DeadCasterAndMover()
    {
        var store = new EntityStore();
        var unit = Caster(store);
        AttributeHelper.GetOrCreateAttr(unit, AttributeHelper.Mana, 100);
        var ability = Ability(store, "a5", range: 1000, castPoint: .1f, manaCost: 30);
        var root = CastRoot(store);

        unit.AddComponent(new CastRequest { ability = ability });
        root.Update(new UpdateTick(.05f, .05f));
        UnitHelper.KillUnit(unit);
        root.Update(new UpdateTick(.1f, .15f));
        Check.That(!unit.HasComponent<CastState>(), "death ends the cast");
        Check.Near(AttributeHelper.GetCurrent(unit, AttributeHelper.Mana), 100, "death before the cast point pays no cost");

        unit.AddComponent(new CastRequest { ability = ability });
        root.Update(new UpdateTick(.05f, .2f));
        Check.That(!unit.HasComponent<CastState>() && !unit.HasComponent<CastRequest>(),
            "a dead unit's request is rejected");

        var mover = Caster(store);
        UnitHelper.MoveToTask(mover, 1000, 0, 10);
        UnitHelper.KillUnit(mover);
        var move = new SystemRoot(store);
        move.Add(new MoveSystem());
        move.Update(new UpdateTick(.1f, .3f));
        Check.That(!mover.HasComponent<MoveCommand>()
            && mover.GetComponent<MoveOutcome>().outcome == MoveOutcomeType.Interrupted,
            "a dead mover's command is interrupted");
    }

    /// <summary>A6：引导或后摇被打断进入冷却；前摇被打断回 Ready 且不进冷却。</summary>
    private static void CommittedInterruptEntersCooldown()
    {
        var store = new EntityStore();
        var root = CastRoot(store);

        var channeler = Caster(store);
        var channel = Ability(store, "a6-channel", range: 1000, castPoint: 0, channel: 5, cooldown: 10);
        channeler.AddComponent(new CastRequest { ability = channel });
        root.Update(new UpdateTick(.05f, .05f));
        Check.That(channeler.GetComponent<CastState>().phase == CastPhase.Channeling, "fixture is channeling");
        AttributeHelper.GetOrCreateAttr(channeler, AttributeHelper.Silence, 1);
        root.Update(new UpdateTick(.05f, .1f));
        Check.That(!channeler.HasComponent<CastState>(), "silence interrupts the channel");
        Check.That(channel.GetComponent<AbilityBase>().state == AbilityState.Cooldown
            && channel.TryGetComponent<AbilityCooldownState>(out var channelCd) && Math.Abs(channelCd.remaining - 10) < .001f,
            "an interrupted channel enters cooldown");

        var swinger = Caster(store);
        var backswing = Ability(store, "a6-backswing", range: 1000, castPoint: 0, backswing: 1, cooldown: 10);
        swinger.AddComponent(new CastRequest { ability = backswing });
        root.Update(new UpdateTick(.05f, .15f));
        Check.That(swinger.GetComponent<CastState>().phase == CastPhase.Backswing, "fixture is in backswing");
        swinger.AddTag<CastInterruptedTag>();
        root.Update(new UpdateTick(.05f, .2f));
        Check.That(backswing.GetComponent<AbilityBase>().state == AbilityState.Cooldown,
            "an interrupted backswing enters cooldown");

        var caster = Caster(store);
        var windup = Ability(store, "a6-windup", range: 1000, castPoint: 5, cooldown: 10);
        caster.AddComponent(new CastRequest { ability = windup });
        root.Update(new UpdateTick(.05f, .25f));
        AttributeHelper.GetOrCreateAttr(caster, AttributeHelper.Stun, 1);
        root.Update(new UpdateTick(.05f, .3f));
        Check.That(windup.GetComponent<AbilityBase>().state == AbilityState.Ready
            && !windup.HasComponent<AbilityCooldownState>(), "an interrupted windup returns to ready without cooldown");
    }

    /// <summary>A7：范围内对单位施法，施法状态记录目标当前位置。</summary>
    private static void InRangeCastUsesTargetPosition()
    {
        var store = new EntityStore();
        var unit = Caster(store);
        var target = Caster(store, 100, 50);
        var ability = Ability(store, "a7", range: 1000, castPoint: 5);
        var root = CastRoot(store);

        unit.AddComponent(new CastRequest { ability = ability, targetUnit = target });
        root.Update(new UpdateTick(.05f, .05f));

        var cast = unit.GetComponent<CastState>();
        Check.That(cast.phase == CastPhase.Casting, "in-range cast starts immediately");
        Check.Near(cast.targetX, 100, "cast target X is the unit position");
        Check.Near(cast.targetY, 50, "cast target Y is the unit position");
    }

    /// <summary>A8：施法中移除技能（销毁或保留）都清理施法状态。</summary>
    private static void RemovingAbilityMidCast()
    {
        var store = new EntityStore();
        var root = CastRoot(store);
        root.Add(new AbilityRemoveWorkflowSystem());

        var unit = Caster(store);
        unit.AddComponent(AbilitySlotContainer.WithSlots(2));
        var destroyed = Ability(store, "a8-destroy", range: 1000, castPoint: 1);
        AbilitySlotHelper.AttachAbilityToSlot(unit, destroyed, 0);
        unit.AddComponent(new CastRequest { ability = destroyed });
        root.Update(new UpdateTick(.05f, .05f));
        AbilitySlotHelper.RemoveAbilityFromSlot(unit, 0);
        root.Update(new UpdateTick(.05f, .1f));
        Check.That(!unit.HasComponent<CastState>(), "destroying the ability ends its cast");
        for (var i = 0; i < 30; i++) root.Update(new UpdateTick(.05f, .15f + i * .05f));

        var kept = Ability(store, "a8-keep", range: 1000, castPoint: 1);
        AbilitySlotHelper.AttachAbilityToSlot(unit, kept, 1);
        unit.AddComponent(new CastRequest { ability = kept });
        root.Update(new UpdateTick(.05f, 2f));
        store.CreateEntity(new AbilityRemoveRequest { unit = unit, slotIndex = 1, destroyAbility = false });
        root.Update(new UpdateTick(.05f, 2.05f));
        Check.That(!unit.HasComponent<CastState>(), "detaching the ability ends its cast");
        Check.That(kept.GetComponent<AbilityBase>().state == AbilityState.Ready, "the detached ability is ready");
    }

    // ---------------- B tick 中断异常 ----------------

    /// <summary>B1：RemoveByKey 不在查询循环内做结构变更。</summary>
    private static void RemoveByKeyOutsideQuery()
    {
        var store = Game.Store;
        var owner = store.CreateEntity();
        var wings = store.CreateEntity(new EffectVisualLink { owner = owner, key = "wings" });
        var other = store.CreateEntity(new EffectVisualLink { owner = owner, key = "other" });
        var effect = store.CreateEntity(
            new EffectSource { caster = owner },
            new EffectTargetInfo(),
            new EffectVisualData { kind = EffectVisualKind.RemoveByKey, key = "wings" });
        effect.AddTag<EffectPendingTag>();
        try
        {
            var root = new SystemRoot(store);
            root.Add(new EffectVisualSystem());
            root.Update(new UpdateTick(.05f, .05f));
            Check.That(wings.HasComponent<EffectDestroyRequest>(), "the matching visual is destroyed");
            Check.That(!other.HasComponent<EffectDestroyRequest>(), "other keys are kept");
            Check.That(effect.Tags.Has<EffectCompleted>(), "the remove step completes");
        }
        finally
        {
            foreach (var entity in new[] { effect, wings, other, owner })
                if (!entity.IsNull) entity.DeleteEntity();
        }
    }

    /// <summary>B2：无来源的近战伤害正常结算，不影响同轮其他请求。</summary>
    private static void SourcelessDamage()
    {
        var store = new EntityStore();
        var target = Caster(store);
        AttributeHelper.GetOrCreateAttr(target, AttributeHelper.Health, 100);
        var attacker = Caster(store);
        store.CreateEntity(new DamageRequest
        {
            target = target,
            damage = new DamageBase { damage = 10, damageType = DamageType.Physical, damageSrc = DamageSrc.Melee, target = target }
        });
        store.CreateEntity(new DamageRequest
        {
            source = attacker,
            target = target,
            damage = new DamageBase { damage = 10, damageType = DamageType.Physical, damageSrc = DamageSrc.Melee, source = attacker, target = target }
        });
        var root = new SystemRoot(store);
        root.Add(new DamageResolveSystem());
        root.Update(new UpdateTick(.05f, .05f));

        Check.Near(AttributeHelper.GetCurrent(target, AttributeHelper.Health), 80, "both requests settle");
        Check.That(store.Query<DamageRequest>().Count == 0, "no request is left behind");
    }

    /// <summary>B3：同步处理器抛异常后请求仍被删除，不会下一轮重放。</summary>
    private static void SyncRequestDeletedOnThrow()
    {
        UISyncBridge.Reset();
        try
        {
            var calls = 0;
            UISyncBridge.Consume("correctness-throw", _ =>
            {
                calls++;
                throw new InvalidOperationException("handler failure");
            });
            var store = new EntityStore();
            var request = store.CreateEntity(new UISyncRequest { Prefix = "correctness-throw", Data = "x" });
            var root = new SystemRoot(store);
            root.Add(new UISyncResolveSystem());
            try { root.Update(new UpdateTick(.01f, .01f)); } catch (InvalidOperationException) { }
            Check.That(request.IsNull, "the request is deleted even when the handler throws");
            try { root.Update(new UpdateTick(.01f, .02f)); } catch (InvalidOperationException) { }
            Check.That(calls == 1, "the request is not replayed");
        }
        finally
        {
            UISyncBridge.Reset();
        }
    }

    // ---------------- C 死亡 / 生命 ----------------

    /// <summary>C1：没有 Health 属性的存活单位不会被 0 伤害击杀；有生命的单位照常死亡。</summary>
    private static void NoHealthNotKilled()
    {
        var store = new EntityStore();
        var unit = Caster(store);
        var mortal = Caster(store);
        AttributeHelper.GetOrCreateAttr(mortal, AttributeHelper.Health, 10);
        store.CreateEntity(new DamageRequest { target = unit, damage = new DamageBase { damageType = DamageType.Real, target = unit } });
        store.CreateEntity(new DamageRequest { target = mortal, damage = new DamageBase { damage = 10, damageType = DamageType.Real, target = mortal } });
        var root = new SystemRoot(store);
        root.Add(new DamageResolveSystem());
        root.Update(new UpdateTick(.05f, .05f));

        Check.That(UnitHelper.IsAlive(unit), "a unit without Health survives");
        Check.That(!UnitHelper.IsAlive(mortal), "a unit whose Health reaches zero dies");
    }

    /// <summary>C2：尸体不回血、不回蓝、不被治疗；存活单位照常回复。</summary>
    private static void CorpseNoRegenNoHeal()
    {
        var store = new EntityStore();
        var corpse = Caster(store);
        AttributeHelper.GetOrCreateAttr(corpse, AttributeHelper.Health, 100);
        AttributeHelper.GetOrCreateAttr(corpse, AttributeHelper.HealthRegen, 10);
        AttributeHelper.GetOrCreateAttr(corpse, AttributeHelper.Mana, 100);
        AttributeHelper.GetOrCreateAttr(corpse, AttributeHelper.ManaRegen, 10);
        AttributeHelper.SetCurrent(corpse, AttributeHelper.Health, 0);
        AttributeHelper.SetCurrent(corpse, AttributeHelper.Mana, 0);
        UnitHelper.KillUnit(corpse);
        var living = Caster(store);
        AttributeHelper.GetOrCreateAttr(living, AttributeHelper.Health, 100);
        AttributeHelper.GetOrCreateAttr(living, AttributeHelper.HealthRegen, 10);
        AttributeHelper.SetCurrent(living, AttributeHelper.Health, 0);

        var root = new SystemRoot(store);
        root.Add(new HealthSystem());
        root.Add(new ManaSystem());
        root.Add(new HealResolveSystem());
        root.Update(new UpdateTick(1f, 1f));
        Check.Near(AttributeHelper.GetCurrent(corpse, AttributeHelper.Health), 0, "a corpse does not regenerate health");
        Check.Near(AttributeHelper.GetCurrent(corpse, AttributeHelper.Mana), 0, "a corpse does not regenerate mana");
        Check.Near(AttributeHelper.GetCurrent(living, AttributeHelper.Health), 10, "a living unit regenerates");

        store.CreateEntity(new HealRequest { source = living, target = corpse, amount = 50 });
        root.Update(new UpdateTick(.01f, 1.01f));
        Check.Near(AttributeHelper.GetCurrent(corpse, AttributeHelper.Health), 0, "a corpse cannot be healed");
        Check.That(store.Query<HealEvent>().Count == 0 && store.Query<HealRequest>().Count == 0,
            "the heal request is dropped without an event");
    }

    /// <summary>C3：复活与出池时 ECS 生命回满，复活后第一次受击不会直接死亡。</summary>
    private static void RebornAndPoolRestoreHealth()
    {
        var store = new EntityStore();
        var unit = store.CreateEntity(new UnitLifeState { isAlive = true, lifePhase = UnitLifecyclePhase.Alive, rebornTime = 1f });
        AttributeHelper.GetOrCreateAttr(unit, AttributeHelper.Health, 100);
        AttributeHelper.SetCurrent(unit, AttributeHelper.Health, 0);
        UnitHelper.KillUnit(unit);
        var root = new SystemRoot(store);
        root.Add(new TimerTaskSystem());
        root.Add(new UnitRemoveNativeSystem());
        root.Add(new UnitLifecycleTransitionSystem());
        root.Add(new UnitRebornNativeSystem());
        root.Add(new UnitLifecycleDisposeSystem());
        root.Add(new DamageResolveSystem());
        root.Update(new UpdateTick(.1f, .1f));
        root.Update(new UpdateTick(3.1f, 3.2f));
        root.Update(new UpdateTick(1.1f, 4.3f));
        Check.That(UnitHelper.IsAlive(unit), "fixture is reborn");
        Check.Near(AttributeHelper.GetCurrent(unit, AttributeHelper.Health), 100, "reborn restores ECS health");

        store.CreateEntity(new DamageRequest { target = unit, damage = new DamageBase { damage = 30, damageType = DamageType.Real, target = unit } });
        root.Update(new UpdateTick(.01f, 4.31f));
        Check.That(UnitHelper.IsAlive(unit), "the first hit after reborn does not kill");
        Check.Near(AttributeHelper.GetCurrent(unit, AttributeHelper.Health), 70, "health after reborn hit");

        var pooled = Caster(store);
        AttributeHelper.GetOrCreateAttr(pooled, AttributeHelper.Health, 80);
        AttributeHelper.SetCurrent(pooled, AttributeHelper.Health, 5);
        UnitHelper.RequestPool(pooled);
        UnitHelper.AcquirePooled(pooled);
        Check.Near(AttributeHelper.GetCurrent(pooled, AttributeHelper.Health), 80, "acquiring from the pool restores health");
    }

    /// <summary>C4：光环不作用于死亡目标；owner 死亡时回收全部贡献。</summary>
    private static void AuraSkipsDead()
    {
        var store = new EntityStore();
        var player = Player(store, 0);
        var owner = Owned(store, player, 0);
        var target = Owned(store, player, 10);
        var root = new SystemRoot(store);
        root.Add(new AuraSystem());
        AuraHelper.CreateAura(store, owner, "correctness", 50, AttributeHelper.Health, ModifyType.Flat, 10);
        root.Update(new UpdateTick(.5f, .5f));
        Check.That(store.Query<Buff>().Count == 1, "fixture applies the aura to the ally");

        UnitHelper.KillUnit(target);
        root.Update(new UpdateTick(.5f, 1f));
        Check.That(store.Query<Buff>().Count == 0, "a dead target loses the aura");

        ref var life = ref target.GetComponent<UnitLifeState>();
        life.isAlive = true;
        life.lifePhase = UnitLifecyclePhase.Alive;
        root.Update(new UpdateTick(.5f, 1.5f));
        Check.That(store.Query<Buff>().Count == 1, "a revived target regains the aura");

        UnitHelper.KillUnit(owner);
        root.Update(new UpdateTick(.5f, 2f));
        Check.That(store.Query<Buff>().Count == 0, "a dead owner stops the aura");
    }

    /// <summary>C5：闪电链不跳向尸体。</summary>
    private static void ChainSkipsCorpse()
    {
        var store = new EntityStore();
        var attackerPlayer = Player(store, 0);
        var enemyPlayer = Player(store, 1);
        PlayerNative[] players = [attackerPlayer, enemyPlayer];
        PlayerHelper.InitializePlayers(ref players);
        PlayerHelper.SetAlliance(attackerPlayer, enemyPlayer, false);
        var attacker = Owned(store, attackerPlayer, 0);
        var first = Owned(store, enemyPlayer, 10);
        var corpse = Owned(store, enemyPlayer, 40);
        UnitHelper.KillUnit(corpse);
        AttributeHelper.GetOrCreateAttr(attacker, AttributeHelper.AttackDamage, 100);
        AttributeHelper.GetOrCreateAttr(attacker, AttributeHelper.AttackRange, 200);
        AttackHelper.SetAttackType(attacker, AttackType.Chain);
        var root = new SystemRoot(store);
        root.Add(new SpatialGridSystem());
        root.Add(new AttackSimulationSystem());

        store.CreateEntity(new UnitAttackedRequest { attacker = attacker, target = first });
        root.Update(new UpdateTick(.01f, .01f));

        var targets = new List<Entity>();
        store.Query<DamageRequest>().ForEachEntity((ref DamageRequest request, Entity _) => targets.Add(request.target));
        Check.That(targets.Count == 1 && targets[0] == first, "the chain stops instead of jumping to a corpse");
    }

    /// <summary>C6：最终值下降后当前值向下夹紧，过期护盾不再吸收。</summary>
    private static void CurrentClampedWhenMaxDrops()
    {
        var store = new EntityStore();
        var unit = Caster(store);
        AttributeHelper.GetOrCreateAttr(unit, AttributeHelper.DamageShield, 0);
        var source = store.CreateEntity();
        ModifyHelper.AddModifierToUnit(unit, AttributeHelper.DamageShield, source, ModifyType.Flat, 100);
        var root = new SystemRoot(store);
        root.Add(new AttrCalculationSystem());
        root.Update(new UpdateTick(.05f, .05f));
        AttributeHelper.SetCurrent(unit, AttributeHelper.DamageShield, 100);
        Check.Near(AttributeHelper.GetCurrent(unit, AttributeHelper.DamageShield), 100, "fixture fills the shield");

        ModifyHelper.RemoveModifiersFromSource(source);
        root.Update(new UpdateTick(.05f, .1f));
        Check.Near(AttributeHelper.GetCurrent(unit, AttributeHelper.DamageShield), 0, "current follows the lowered maximum");
    }

    // ---------------- D Buff / 属性贡献 ----------------

    /// <summary>D1：永久可叠层 Buff 重复施加时叠层。</summary>
    private static void PermanentBuffStacks()
    {
        var store = new EntityStore();
        var unit = Caster(store);
        BuffHelper.AddStackableBuff(store, unit, unit, "permanent-stack", AttributeHelper.Health, ModifyType.Flat, 1, 3, -1);
        BuffHelper.AddStackableBuff(store, unit, unit, "permanent-stack", AttributeHelper.Health, ModifyType.Flat, 1, 3, -1);
        Check.That(BuffHelper.GetBuffStacks(unit, "permanent-stack") == 2, "a permanent stackable buff stacks");
    }

    /// <summary>D2：同 buffId 作用于不同属性时各自成立；按 buffId 移除全部。</summary>
    private static void SameBuffIdDifferentAttrs()
    {
        var store = new EntityStore();
        var unit = Caster(store);
        AttributeHelper.GetOrCreateAttr(unit, AttributeHelper.Health, 100);
        AttributeHelper.GetOrCreateAttr(unit, AttributeHelper.Mana, 50);
        BuffHelper.AddTimedBuff(store, unit, unit, "shared", AttributeHelper.Health, ModifyType.Flat, 10, 5);
        BuffHelper.AddTimedBuff(store, unit, unit, "shared", AttributeHelper.Mana, ModifyType.Flat, 5, 5);
        var root = new SystemRoot(store);
        root.Add(new AttrCalculationSystem());
        root.Update(new UpdateTick(.05f, .05f));

        Check.That(store.Query<Buff>().Count == 2, "each attribute gets its own instance");
        Check.Near(AttributeHelper.GetFinalValue(unit, AttributeHelper.Health), 110, "health keeps its value");
        Check.Near(AttributeHelper.GetFinalValue(unit, AttributeHelper.Mana), 55, "mana receives its value");

        BuffHelper.RemoveBuff(unit, "shared");
        Check.That(store.Query<Buff>().Count == 0, "removing by id removes every instance");
    }

    /// <summary>D3：移除技能（销毁或保留）后其属性贡献被回收。</summary>
    private static void AbilityContributionRemoved()
    {
        var store = new EntityStore();
        var unit = Caster(store);
        unit.AddComponent(AbilitySlotContainer.WithSlots(2));
        AttributeHelper.GetOrCreateAttr(unit, AttributeHelper.ManaRegen, 1);
        var root = new SystemRoot(store);
        root.Add(new AbilityRemoveWorkflowSystem());
        root.Add(new AbilityAttributeApplySystem());
        root.Add(new AbilityAttributeRemoveSystem());
        root.Add(new AttrCalculationSystem());

        var destroyed = Talent(store);
        AbilitySlotHelper.AttachAbilityToSlot(unit, destroyed, 0);
        root.Update(new UpdateTick(.05f, .05f));
        Check.Near(AttributeHelper.GetFinalValue(unit, AttributeHelper.ManaRegen), 1.75, "fixture applies the contribution");
        AbilitySlotHelper.RemoveAbilityFromSlot(unit, 0);
        root.Update(new UpdateTick(.05f, .1f));
        Check.Near(AttributeHelper.GetFinalValue(unit, AttributeHelper.ManaRegen), 1, "destroying the ability removes its contribution");

        var kept = Talent(store);
        AbilitySlotHelper.AttachAbilityToSlot(unit, kept, 1);
        root.Update(new UpdateTick(.05f, .15f));
        store.CreateEntity(new AbilityRemoveRequest { unit = unit, slotIndex = 1, destroyAbility = false });
        root.Update(new UpdateTick(.05f, .2f));
        Check.Near(AttributeHelper.GetFinalValue(unit, AttributeHelper.ManaRegen), 1, "detaching the ability removes its contribution");
    }

    // ---------------- E 效果 / 弹道 ----------------

    /// <summary>E1：抛物线弹道按固定起点推进，到达时回到起点高度。</summary>
    private static void ParabolicProjectile()
    {
        var store = new EntityStore();
        var effect = store.CreateEntity(
            new ProjectileData { trajectoryType = ProjectileTrajectoryType.Parabolic, speed = 100f, arrivalThreshold = 1f },
            new EffectSource(),
            new EffectTargetInfo { targetX = 1000, targetY = 0 },
            new Position(0, 0, 0),
            new ProjectileRuntimeState { phase = ProjectileLifecyclePhase.PendingStart });
        effect.AddTag<EffectPendingTag>();
        var root = new SystemRoot(store);
        root.Add(new ProjectileSystem());
        var steps = 0;
        while (steps < 500 && effect.GetComponent<ProjectileRuntimeState>().phase
                   is ProjectileLifecyclePhase.PendingStart or ProjectileLifecyclePhase.InFlight)
        {
            steps++;
            root.Update(new UpdateTick(.1f, steps * .1f));
        }

        Check.That(steps is >= 95 and <= 105, $"arrival takes distance / speed (steps={steps})");
        Check.Near(effect.GetComponent<Position>().z, 0, "the arc lands at the start height", .01);
    }

    /// <summary>E2：线形搜索父效果不自行结算，主目标只受一次伤害。</summary>
    private static void LineParentDoesNotSettle()
    {
        var store = new EntityStore();
        var (caster, target) = Enemies(store, 0, 200);
        var effect = store.CreateEntity(
            new EffectSource { caster = caster },
            new EffectTargetInfo { targetUnit = target, targetX = 200, targetY = 0 },
            new LineSearchData { range = 600, width = 100, filter = TargetFilter.EnemyAlive },
            new DamageEffectData { value = EffectValueSpec.Constant(10), damageType = DamageType.Real, damageSrc = DamageSrc.Skill });
        effect.AddTag<EffectPendingTag>();
        var root = new SystemRoot(store);
        root.Add(new SpatialGridSystem());
        root.Add(new LineSearchSystem());
        root.Add(new DamageEffectSystem());
        root.Update(new UpdateTick(.05f, .05f));

        var hits = 0;
        store.Query<DamageRequest>().ForEachEntity((ref DamageRequest request, Entity _) =>
        {
            if (request.target == target) hits++;
        });
        Check.That(hits == 1, $"the primary target is hit once (hits={hits})");
    }

    /// <summary>E3：子效果目标坐标取命中目标位置。</summary>
    private static void ChildEffectTargetPosition()
    {
        var store = new EntityStore();
        var caster = Caster(store);
        var hit = Caster(store, 300, 40);
        var parent = store.CreateEntity(new EffectSource { caster = caster }, new EffectTargetInfo { targetX = 500, targetY = 500 });
        var child = AbilityEffectHelper.CreateChildEffect(parent, hit);
        var info = child.GetComponent<EffectTargetInfo>();
        Check.Near(info.targetX, 300, "child target X is the hit position");
        Check.Near(info.targetY, 40, "child target Y is the hit position");
    }

    /// <summary>E4：效果管线把请求与事件写入效果/目标所属 Store。</summary>
    private static void EffectPipelineUsesOwnerStore()
    {
        var store = new EntityStore();
        var caster = Caster(store);
        var target = Caster(store, 10);
        var effect = store.CreateEntity(
            new EffectSource { caster = caster },
            new EffectTargetInfo { targetUnit = target },
            new DamageEffectData { value = EffectValueSpec.Constant(10), damageType = DamageType.Real, damageSrc = DamageSrc.Skill });
        effect.AddTag<EffectPendingTag>();
        store.CreateEntity(new BuffApplyRequest
        {
            source = caster,
            target = target,
            buffId = "store",
            duration = 5,
            attrTypeId = AttributeHelper.Health,
            modifyType = ModifyType.Flat,
            value = 5,
            kind = BuffKind.Attribute
        });
        var root = new SystemRoot(store);
        root.Add(new DamageEffectSystem());
        root.Add(new BuffApplyResolveSystem());
        root.Update(new UpdateTick(.05f, .05f));

        Check.That(store.Query<DamageRequest>().Count == 1, "the damage request is written to the effect store");
        Check.That(store.Query<Buff>().Count == 1 && store.Query<BuffAppliedEvent>().Count == 1,
            "the buff and its event are written to the target store");
    }

    /// <summary>E5：地面周期伤害保留余数；引导整数倍时长触发完整 tick 次数。</summary>
    private static void PeriodicTiming()
    {
        var store = new EntityStore();
        var caster = Caster(store);
        Caster(store, 10);
        var area = store.CreateEntity(
            new GroundAreaData { radius = 100 },
            new GroundAreaSource { caster = caster },
            new GroundAreaPeriodicDamageData
            {
                enabled = true,
                damageValue = EffectValueSpec.Constant(1),
                tickInterval = 1,
                damageType = DamageType.Real,
                damageSrc = DamageSrc.Skill,
                filter = TargetFilter.Alive
            },
            new Position(0, 0, 0));
        var ground = new SystemRoot(store);
        ground.Add(new SpatialGridSystem());
        ground.Add(new GroundAreaPeriodicDamageSystem());
        for (var i = 1; i <= 4; i++) ground.Update(new UpdateTick(.75f, i * .75f));
        Check.That(store.Query<DamageRequest>().Count == 3, $"3 seconds of 1-second ticks hit 3 times (hits={store.Query<DamageRequest>().Count})");
        area.DeleteEntity();

        var channelStore = new EntityStore();
        var channeler = Caster(channelStore);
        var ability = channelStore.CreateEntity();
        AbilitySpecBuilder.Create("e5-channel")
            .BaseValue(AbilityHelper.Range, 1000)
            .Channel(2f, .5f)
            .OnChannelTick(e => e.Damage(EffectValueSpec.Constant(1), DamageType.Real))
            .BuildTo(ability, 1);
        var cast = CastRoot(channelStore);
        channeler.AddComponent(new CastRequest { ability = ability });
        for (var i = 1; i <= 12; i++) cast.Update(new UpdateTick(.25f, i * .25f));
        Check.That(!channeler.HasComponent<CastState>(), "fixture finishes the channel");
        Check.That(channelStore.Query<EffectSource>().Count == 4,
            $"a 2s channel with 0.5s ticks fires 4 times (ticks={channelStore.Query<EffectSource>().Count})");
    }

    // ---------------- F 维护 ----------------

    /// <summary>F1：自定义筛选回调内嵌套查询不影响外层结果。</summary>
    private static void GroupQueryReentrant()
    {
        var store = new EntityStore();
        for (var i = 0; i < 5; i++) store.CreateEntity(new Position(i * 10, 0, 0));
        var root = new SystemRoot(store);
        root.Add(new SpatialGridSystem());
        root.Update(new UpdateTick(.01f, .01f));
        const string plain = "correctness-plain";
        const string nested = "correctness-nested";
        TargetFilterRegistry.Register(plain, (_, _) => true);
        TargetFilterRegistry.Register(nested, (_, _) => GroupHelper.FindInCircle(default, 0, 0, 100).Count >= 0);
        try
        {
            var expected = GroupHelper.FindInCircle(default, 0, 0, 100, TargetFilter.None, plain);
            var actual = GroupHelper.FindInCircle(default, 0, 0, 100, TargetFilter.None, nested);
            Check.That(expected.Count == 5 && actual.Count == expected.Count && actual.All(expected.Contains),
                "a nested query does not corrupt the outer result");
        }
        finally
        {
            TargetFilterRegistry.Unregister(plain);
            TargetFilterRegistry.Unregister(nested);
        }
    }

    /// <summary>F2：主计时器启动路径不向控制台输出调试信息（源码断言）。</summary>
    private static void BootstrapHasNoDebugOutput()
    {
        var source = CompilationTest.Read("War3Frame/Src/Systems/Native/War3NativeBootstrap.cs");
        Check.That(!source.Contains("Console.WriteLine", StringComparison.Ordinal), "bootstrap has no Console.WriteLine");
    }

    /// <summary>F3：输入根为空时积压输入被丢弃，不会在之后的根上触发点击或双击。</summary>
    private static void PumpDropsInputWithoutRoot()
    {
        UIHitRegistry.Reset();
        UIInputQueue.Reset();
        UIInput.ResetDispatch();
        var panel = new UIFrame(0);
        try
        {
            panel.Size = new UIVec(0.8f, 0.6f);
            panel.Position = new UIVec(0.4f, 0.3f);
            var button = new UIButton(0);
            panel.AddChild(button);
            button.Size = new UIVec(0.1f, 0.1f);
            button.Position = new UIVec(0.4f, 0.3f);
            panel.FlushLayout();
            var pressed = 0;
            button.Pressed += _ => pressed++;
            button.DoublePressed += _ => pressed += 100;

            foreach (var kind in new[] { UIRawKind.LeftDown, UIRawKind.LeftUp, UIRawKind.LeftDown, UIRawKind.LeftUp })
                UIInputQueue.Enqueue(new UIRawInput(kind, 0.4f, 0.3f, 0));
            UIInput.Pump(0.01f);
            UIInput.Root = panel;
            UIInput.Pump(0.02f);
            Check.That(pressed == 0, $"stale input is not dispatched (pressed={pressed})");
        }
        finally
        {
            panel.Destroy();
            UIInput.ResetDispatch();
            UIInputQueue.Reset();
            UIHitRegistry.Reset();
        }
    }

    /// <summary>F4：同一轮两件物品抢同一槽位，只挂载一件，另一件保留在地面且视觉不变。</summary>
    private static void PickupSameSlot()
    {
        var store = new EntityStore();
        var owner = store.CreateEntity(new ItemSlotContainer { maxSlots = 1 }, new Position { x = 0, y = 0 });
        var first = GroundItem(store);
        var second = GroundItem(store);
        var root = new SystemRoot(store);
        root.Add(new ItemPickupWorkflowSystem());
        root.Add(new ItemAttachWorkflowSystem());

        ItemHelper.RequestPickup(owner, first, 0);
        ItemHelper.RequestPickup(owner, second, 0);
        root.Update(new UpdateTick(.01f, .01f));
        root.Update(new UpdateTick(.01f, .02f));

        var equipped = new[] { first, second }.Where(item => item.Tags.Has<ItemEquippedTag>()).ToArray();
        Check.That(equipped.Length == 1, "exactly one item is attached");
        var loser = equipped[0] == first ? second : first;
        var visual = loser.GetComponent<ItemGroundVisual>().effect;
        Check.That(loser.Tags.Has<ItemGroundTag>() && !visual.IsNull && !visual.HasComponent<EffectDestroyRequest>(),
            "the other item stays on the ground with its visual");
        Check.That(store.Query<ItemAttachRequest>().Count == 0, "no attach request is left behind");
    }

    // ---------------- 夹具 ----------------

    private static SystemRoot CastRoot(EntityStore store)
    {
        var root = new SystemRoot(store);
        root.Add(new CastRequestSystem());
        root.Add(new MoveToCastSystem());
        root.Add(new CastingSystem());
        root.Add(new ChannelingSystem());
        root.Add(new MoveSystem());
        return root;
    }

    private static Entity Caster(EntityStore store, float x = 0, float y = 0)
        => store.CreateEntity(new Position(x, y, 0),
            new UnitLifeState { isAlive = true, lifePhase = UnitLifecyclePhase.Alive });

    private static Entity Ability(EntityStore store, string name, float range, float castPoint,
        float channel = 0, float backswing = 0, float cooldown = 0, float manaCost = 0)
    {
        var ability = store.CreateEntity();
        var builder = AbilitySpecBuilder.Create(name)
            .BaseValue(AbilityHelper.Range, range)
            .BaseValue(AbilityHelper.CooldownDuration, cooldown)
            .BaseValue(AbilityHelper.ManaCost, manaCost)
            .CastPoint(castPoint)
            .Backswing(backswing);
        if (channel > 0) builder.Channel(channel);
        builder.BuildTo(ability, 1);
        return ability;
    }

    private static Entity Talent(EntityStore store)
    {
        var ability = AbilityHelper.CreateAbility("correctness-talent", store: store);
        ability.AddComponent(new AttributeContributionEntry { attrTypeId = AttributeHelper.ManaRegen, modifyType = ModifyType.Flat, value = .75f });
        return ability;
    }

    private static PlayerNative Player(EntityStore store, int index)
    {
        var entity = store.CreateEntity();
        var player = new PlayerNative { index = index, getentity = entity };
        entity.AddComponent(player);
        return player;
    }

    private static Entity Owned(EntityStore store, PlayerNative player, float x)
        => store.CreateEntity(new Position(x, 0, 0), new UnitOwner { player = player.getentity },
            new UnitLifeState { isAlive = true, lifePhase = UnitLifecyclePhase.Alive });

    private static (Entity caster, Entity target) Enemies(EntityStore store, float casterX, float targetX)
    {
        var a = Player(store, 0);
        var b = Player(store, 1);
        PlayerNative[] players = [a, b];
        PlayerHelper.InitializePlayers(ref players);
        PlayerHelper.SetAlliance(a, b, false);
        return (Owned(store, a, casterX), Owned(store, b, targetX));
    }

    private static Entity GroundItem(EntityStore store)
    {
        var item = store.CreateEntity(new ItemBase { name = "ground" });
        item.AddTag<ItemGroundTag>();
        item.AddComponent(new Position { x = 0, y = 0 });
        var effect = store.CreateEntity(new EffectBase(), new Position { x = 0, y = 0 });
        item.AddComponent(new ItemGroundVisual { model = "ground.mdl", effect = effect });
        item.AddComponent(new ItemGroundPickRange { radius = 160 });
        return item;
    }
}

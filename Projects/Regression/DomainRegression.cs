using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;
using War3Frame.Helpers;
using War3Frame.Src.Components;
using War3Frame.Src.Systems;
using War3Frame.Systems;
using War3Frame.Systems.Time;

namespace War3Frame.Regression;

internal static class DomainRegression
{
    internal static void Register(List<RegressionCase> tests)
    {
        tests.Add(new("runtime", "relation-and-filter-matrix", Relations));
        tests.Add(new("runtime", "aura-source-and-range", Auras));
        tests.Add(new("runtime", "equipment-roundtrip", Equipment));
        tests.Add(new("runtime", "ability-level-phases", AbilityLevel));
        tests.Add(new("runtime", "buff-final-ticks", BuffFinalTicks));
        tests.Add(new("runtime", "modifier-multiple-removal", Modifiers));
        tests.Add(new("runtime", "sync-identity-and-input", SyncIdentity));
        tests.Add(new("runtime", "spatial-search", SpatialSearch));
        tests.Add(new("runtime", "pause-synthesis", PauseSynthesis));
    }

    private static PlayerNative Player(EntityStore store, int index)
    {
        var entity = store.CreateEntity();
        var player = new PlayerNative { index = index, getentity = entity };
        entity.AddComponent(player);
        return player;
    }

    private static Entity Unit(EntityStore store, PlayerNative player, float x = 0)
        => store.CreateEntity(new Position(x, 0, 0), new UnitOwner { player = player.getentity },
            new UnitLifeState { isAlive = true, lifePhase = UnitLifecyclePhase.Alive });

    private static void Relations()
    {
        var store = new EntityStore();
        var a = Player(store, 0);
        var b = Player(store, 1);
        PlayerNative[] players = [a, b];
        PlayerHelper.InitializePlayers(ref players);
        var source = Unit(store, a);
        var target = Unit(store, b);
        foreach (var allied in new[] { false, true })
        {
            PlayerHelper.SetAlliance(a, b, allied);
            PlayerHelper.SetNeutral(a, b, true);
            Check.That(PlayerHelper.GetRelation(a, b) == PlayerTeamState.Neutral, "neutral on");
            Check.That(TargetFilterRegistry.PassPresetFilter(TargetFilter.Neutral, source, target), "neutral filter");
            PlayerHelper.SetNeutral(a, b, false);
            Check.That(PlayerHelper.IsAlly(a, b) == allied && PlayerHelper.IsEnemy(a, b) != allied, "neutral off restores basic");
            PlayerHelper.SetNeutral(a, b, true);
            PlayerHelper.SetAlliance(a, b, allied);
            Check.That(PlayerHelper.GetRelation(a, b) == PlayerHelper.GetRelation(b, a)
                && PlayerHelper.IsAlly(a, b) == allied, "alliance switch clears neutral both ways");
        }
        Check.That(!TargetFilterRegistry.PassPresetFilter(TargetFilter.Enemy, source, store.CreateEntity()), "unknown owner rejected");
        Check.That(!TargetFilterRegistry.PassPresetFilter(TargetFilter.Ally, source, source), "self is distinct from ally");
        Check.That(TargetFilterRegistry.PassPresetFilter(TargetFilter.Self | TargetFilter.Alive, source, source), "self alive");
        Check.That(TargetFilterRegistry.PassPresetFilter(TargetFilter.Ally | TargetFilter.Alive, source, target), "ally alive");
        Check.That(!TargetFilterRegistry.PassPresetFilter(TargetFilter.Dead, source, target), "alive target excluded from dead filter");
        target.GetComponent<UnitLifeState>().isAlive = false;
        Check.That(TargetFilterRegistry.PassPresetFilter(TargetFilter.Alive | TargetFilter.Dead, source, target), "life group OR");
        Check.That(!TargetFilterRegistry.PassPresetFilter(TargetFilter.Alive, source, target), "dead target excluded");
    }

    private static void Auras()
    {
        var store = new EntityStore();
        var player = Player(store, 0);
        var owner = Unit(store, player);
        var target = Unit(store, player, 10);
        var root = new SystemRoot(store);
        root.Add(new AuraSystem());
        var first = AuraHelper.CreateAura(store, owner, "same", 50, AttributeHelper.Health, ModifyType.Flat, 10);
        var second = AuraHelper.CreateAura(store, owner, "same", 50, AttributeHelper.Health, ModifyType.Flat, 20);
        root.Update(new UpdateTick(.5f, .5f));
        Check.That(AuraHelper.HasAura(owner, "same") && store.Query<Buff>().Count == 2, "one contribution per source");
        root.Update(new UpdateTick(.5f, 1));
        Check.That(store.Query<Buff>().Count == 2, "no duplicate on subsequent scan");
        target.GetComponent<Position>().x = 100;
        root.Update(new UpdateTick(.5f, 1.5f));
        Check.That(store.Query<Buff>().Count == 0, "leaving radius removes both contributions");
        target.GetComponent<Position>().x = 10;
        root.Update(new UpdateTick(.5f, 2));
        AuraHelper.RemoveAura(owner, "same");
        Check.That(store.Query<Buff>().Count == 1, "removing one source preserves the other");
        AuraHelper.RemoveAllAuras(owner);
        Check.That(first.IsNull && second.IsNull && store.Query<Buff>().Count == 0, "owner cleanup includes all buffs");
    }

    private static void Equipment()
    {
        var store = new EntityStore();
        var owner = store.CreateEntity(new ItemSlotContainer { maxSlots = 2 });
        var item = store.CreateEntity(new ItemBase());
        var root = new SystemRoot(store);
        root.Add(new ItemAttachWorkflowSystem());
        ItemHelper.EquipToUnit(item, owner, 0);
        root.Update(new UpdateTick(.01f, .01f));
        ItemHelper.UnequipToInventory(item);
        ItemHelper.EquipToUnit(item, owner, 0);
        root.Update(new UpdateTick(.01f, .02f));
        Check.That(item.Tags.Has<ItemEquippedTag>() && item.HasComponent<ItemAttrApplyRequest>()
            && !item.HasComponent<ItemAttrRemoveRequest>(), "same-slot re-equip restores contribution intent");
        Check.That(owner.GetComponent<ItemSlotContainer>().currentCount == 1, "re-equip does not consume another slot");
    }

    private static void AbilityLevel()
    {
        var store = new EntityStore();
        var ability = store.CreateEntity();
        AbilitySpecBuilder.Create("phase-level").CastPoint(LevelValue.PerLevel(1, 2))
            .Backswing(LevelValue.PerLevel(2, 2)).Channel(LevelValue.PerLevel(3, 2), LevelValue.PerLevel(4, 2))
            .BuildTo(ability, 1);
        ability.GetComponent<AbilityBase>().level = 3;
        ability.AddTag<LevelStatDirty>();
        var root = new SystemRoot(store);
        root.Add(new AbilityLevelStatRebuildSystem());
        root.Update(new UpdateTick(.01f, .01f));
        Check.Near(AbilityHelper.GetBaseValue(ability, AbilityHelper.CastTime), 5, "cast point after level change");
        Check.Near(AbilityHelper.GetBaseValue(ability, AbilityHelper.BackswingDuration), 6, "backswing after level change");
        Check.Near(AbilityHelper.GetBaseValue(ability, AbilityHelper.ChannelDuration), 7, "channel after level change");
        Check.Near(AbilityHelper.GetBaseValue(ability, AbilityHelper.ChannelTickInterval), 8, "channel interval after level change");
    }

    private static void BuffFinalTicks()
    {
        var store = new EntityStore();
        var target = store.CreateEntity();
        var buff = BuffHelper.ApplyDoT(store, target, target, "short", 1, .1f, .25f);
        var root = new SystemRoot(store);
        root.Add(new DurationSystem());
        root.Add(new BuffTickSystem());
        root.Add(new BuffDurationSystem());
        root.Add(new BuffExpireSystem());
        root.Update(new UpdateTick(1, 1));
        Check.That(store.Query<DamageRequest>().Count == 2 && buff.IsNull, "no ticks outside the .25s lifetime");
        BuffHelper.ApplyDoT(store, target, target, "permanent", 1, .1f, -1);
        root.Update(new UpdateTick(.3f, 1.3f));
        Check.That(store.Query<DamageRequest>().Count == 5, "permanent ticks keep advancing");
    }

    private static void Modifiers()
    {
        var store = new EntityStore();
        var target = store.CreateEntity();
        var source = store.CreateEntity();
        for (var i = 0; i < 5; i++)
            ModifyHelper.AddModifierToUnit(target, AttributeHelper.Health, source, ModifyType.Flat, 1);
        Check.That(store.Query<ModifyValue>().Count == 5, "local store modifiers created");
        ModifyHelper.RemoveModifiersFromSource(source);
        Check.That(store.Query<ModifyValue>().Count == 0, "all linked modifiers removed without skipped links");
    }

    private static void SyncIdentity()
    {
        var previous = SyncHelper.Store;
        try
        {
            var store = new EntityStore();
            SyncHelper.Store = store;
            var entity = store.CreateEntity();
            var id = entity.Id;
            var token = SyncHelper.EncodeEntity(entity);
            Check.That(!token.Contains('|') && SyncHelper.DecodeEntity(token) == entity, "identity roundtrip");
            entity.DeleteEntity();
            var reused = store.CreateEntity(id);
            Check.That(reused.Id == id && SyncHelper.DecodeEntity(token).IsNull, "recycled ID cannot accept stale token");
            foreach (var malformed in new[] { "1", "e1:0:0", "e1:?:0", "e1:zzzzzzz:0", "e1:1:0:x", "e1:1:-", "e2:1:0" })
                Check.That(SyncHelper.DecodeEntity(malformed).IsNull, "invalid token: " + malformed);
            foreach (var value in new[] { int.MinValue, -1, 0, 1, int.MaxValue })
                Check.That(SyncHelper.Base36ToInt(SyncHelper.IntToBase36(value)) == value, "integer limits roundtrip");
            var rejected = false;
            try { SyncHelper.EncodeEntity(new EntityStore().CreateEntity()); }
            catch (ArgumentException) { rejected = true; }
            Check.That(rejected, "cross-store encode rejected");
            SyncHelper.Store = null;
            Check.That(SyncHelper.DecodeEntity(token).IsNull, "missing store rejected");
        }
        finally { SyncHelper.Store = previous; }
    }

    /// <summary>
    /// 验证 Pause 由 Stun / CrackFly / 纯 Pause 属性合成驱动 PauseUnit：
    /// 同帧多控制只发一次请求、多来源叠加全归零才恢复、免疫不穿透到 Pause、纯 Pause 不可免疫。
    /// </summary>
    private static void PauseSynthesis()
    {
        var store = new EntityStore();
        var root = new TimedSystemRoot(store);
        root.Add(new AttrCalculationSystem(), 0f);
        root.Add(new ControlStateTransitionSystem(), 0f);

        var unit = store.CreateEntity();
        var stunSource = store.CreateEntity();
        var stunSource2 = store.CreateEntity();
        var flySource = store.CreateEntity();
        var immunitySource = store.CreateEntity();
        var pauseSource = store.CreateEntity();

        // A. 眩晕生效 → 合成一次暂停进入；快照 Pause 位（序号 10）不因位宽溢出失效
        ModifyHelper.AddModifierToUnit(unit, AttributeHelper.Stun, stunSource, ModifyType.Flat, 1);
        root.Update(new UpdateTick(.02f, .02f));
        Check.That(CountNativeRequests(store, ControlType.Pause, true) == 1, "stun synthesizes one pause enter");
        Check.That(unit.GetComponent<ControlStateSnapshot>().IsActive(ControlType.Pause), "pause bit survives snapshot width");
        Check.That(CountNativeRequests(store, ControlType.Stun, true) == 0
            && CountNativeRequests(store, ControlType.CrackFly, true) == 0,
            "stun/crackfly stop emitting their own native request");
        Check.That(CountEvents(store, ControlType.Stun, true) == 1, "stun still broadcasts its changed event");

        // B. 同帧叠加（第二来源 + 击飞）→ 已暂停，不重复发请求
        ModifyHelper.AddModifierToUnit(unit, AttributeHelper.Stun, stunSource2, ModifyType.Flat, 1);
        ModifyHelper.AddModifierToUnit(unit, AttributeHelper.CrackFly, flySource, ModifyType.Flat, 1);
        root.Update(new UpdateTick(.02f, .04f));
        Check.That(CountNativeRequests(store, ControlType.Pause, true) == 1, "same-frame changes synthesize a single pause");

        // C. 解除眩晕、击飞仍在 → 保持暂停
        ModifyHelper.RemoveModifiersFromSource(stunSource);
        ModifyHelper.RemoveModifiersFromSource(stunSource2);
        root.Update(new UpdateTick(.02f, .06f));
        Check.That(CountNativeRequests(store, ControlType.Pause, false) == 0, "pause holds while crackfly remains");

        // D. 解除击飞 → 恰好一次暂停解除
        ModifyHelper.RemoveModifiersFromSource(flySource);
        root.Update(new UpdateTick(.02f, .08f));
        Check.That(CountNativeRequests(store, ControlType.Pause, false) == 1, "all controls released resumes unit once");

        // E. 免疫压制 → 不暂停；免疫移除 → 恢复暂停
        ModifyHelper.AddModifierToUnit(unit, AttributeHelper.Stun, stunSource, ModifyType.Flat, 1);
        root.Update(new UpdateTick(.02f, .10f));
        Check.That(CountNativeRequests(store, ControlType.Pause, true) == 2, "stun re-enters pause");
        ModifyHelper.AddModifierToUnit(unit, AttributeHelper.StunImmunity, immunitySource, ModifyType.Flat, 1);
        root.Update(new UpdateTick(.02f, .12f));
        Check.That(CountNativeRequests(store, ControlType.Pause, true) == 2
            && CountNativeRequests(store, ControlType.Pause, false) == 2, "immunity suppresses stun pause");
        ModifyHelper.RemoveModifiersFromSource(immunitySource);
        root.Update(new UpdateTick(.02f, .14f));
        Check.That(CountNativeRequests(store, ControlType.Pause, true) == 3, "immunity removal restores pause");
        ModifyHelper.RemoveModifiersFromSource(stunSource);
        root.Update(new UpdateTick(.02f, .16f));
        Check.That(CountNativeRequests(store, ControlType.Pause, false) == 3, "stun release after immunity resumes");

        // F. 纯 Pause 不经免疫，独立驱动暂停
        ModifyHelper.AddModifierToUnit(unit, AttributeHelper.StunImmunity, immunitySource, ModifyType.Flat, 1);
        root.Update(new UpdateTick(.02f, .18f));
        ModifyHelper.AddModifierToUnit(unit, AttributeHelper.Pause, pauseSource, ModifyType.Flat, 1);
        root.Update(new UpdateTick(.02f, .20f));
        Check.That(CountNativeRequests(store, ControlType.Pause, true) == 4, "pure pause ignores immunity and enters");
        ModifyHelper.RemoveModifiersFromSource(pauseSource);
        root.Update(new UpdateTick(.02f, .22f));
        Check.That(CountNativeRequests(store, ControlType.Pause, false) == 4, "pure pause exits");
    }

    private static int CountNativeRequests(EntityStore store, ControlType controlType, bool entered)
    {
        var count = 0;
        store.Query<ControlStateNativeRequest>().ForEachEntity((ref ControlStateNativeRequest req, Entity _) =>
        {
            if (req.controlType == controlType && req.entered == entered)
                count++;
        });
        return count;
    }

    private static int CountEvents(EntityStore store, ControlType controlType, bool entered)
    {
        var count = 0;
        store.Query<ControlStateChangedEvent>().ForEachEntity((ref ControlStateChangedEvent evt, Entity _) =>
        {
            if (evt.controlType == controlType && evt.entered == entered)
                count++;
        });
        return count;
    }

    private static void SpatialSearch()
    {
        var store = new EntityStore();
        var source = store.CreateEntity(new Position(0, 0, 0));
        var near = store.CreateEntity(new Position(10, 0, 0));
        store.CreateEntity(new Position(100, 0, 0));
        var root = new SystemRoot(store);
        root.Add(new SpatialGridSystem());
        root.Update(new UpdateTick(.04f, .04f));
        var found = GroupHelper.FindInCircle(source, 0, 0, 20);
        Check.That(found.Count == 2 && found.Contains(near), "registered index supplies real range search");
        GroupHelper.Grid.Clear();
    }
}

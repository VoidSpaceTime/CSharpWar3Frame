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

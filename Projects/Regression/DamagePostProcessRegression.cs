using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;
using War3Frame.Src.Components;

namespace War3Frame.Regression;

/// <summary>验证伤害后处理内置：护盾吸收、吸血、反射的数值、顺序与互斥，以及主流程不内联业务特例。</summary>
internal static class DamagePostProcessRegression
{
    /// <summary>注册伤害后处理回归场景。</summary>
    internal static void Register(List<RegressionCase> tests)
    {
        tests.Add(new("runtime", "伤害后处理数值", PostProcessValues));
        tests.Add(new("runtime", "反射不双重结算", ReflectNoDoubleSettlement));
        tests.Add(new("runtime", "无敌与零伤互斥", ImmuneAndZeroDamage));
        tests.Add(new("runtime", "无属性基线不变", BaselineWithoutAttributes));
        tests.Add(new("runtime", "主流程无内置特例", NoInlinedSpecialCases));
    }

    /// <summary>护盾部分吸收 100/40 → 扣 60、吸收 40；吸血 60×0.25=15；反射 60×0.5=30 且为 Real 并跳过后处理。</summary>
    private static void PostProcessValues()
    {
        var store = new EntityStore();
        var attacker = Unit(store, hp: 1000f);
        CreateAttr(store, attacker, AttributeHelper.Lifesteal, 0.25f);
        var defender = Unit(store, hp: 1000f);
        CreatePool(store, defender, AttributeHelper.DamageShield, current: 40f, max: 100f);
        CreateAttr(store, defender, AttributeHelper.DamageReflect, 0.5f);
        var root = Resolve(store);

        Hit(store, root, attacker, defender, 100f, DamageType.Physical);

        Check.Near(AttributeHelper.GetCurrent(defender, AttributeHelper.DamageShield), 0f,
            "shield is drained to zero");
        Check.Near(Hp(defender), 940f, "defender loses the post-absorb amount of 60");
        var evt = LastDamageEvent(store, defender);
        Check.That(evt.HasValue, "defender receives one damage event");
        Check.Near(evt!.Value.finalDamage, 60f, "event reports actual 60");
        Check.Near(evt.Value.absorbed, 40f, "event reports absorbed 40");

        var heal = Single<HealRequest>(store);
        Check.Near(heal.amount, 15f, "lifesteal heals 60 * 0.25");
        Check.That(heal.source == attacker && heal.target == attacker, "lifesteal heals the attacker");

        var reflect = Single<DamageRequest>(store);
        Check.Near(reflect.damage.damage, 30f, "reflect deals 60 * 0.5");
        Check.That(reflect.damage.damageType == DamageType.Real, "reflect is real damage");
        Check.That(reflect.skipPostProcess, "reflect skips post-process");
        Check.That(reflect.source == defender && reflect.target == attacker, "reflect points back at the attacker");
    }

    /// <summary>反射结算可被对方护盾吸收，但不再吸血、不再反射，排除双重结算。</summary>
    private static void ReflectNoDoubleSettlement()
    {
        var store = new EntityStore();
        var attacker = Unit(store, hp: 1000f);
        CreateAttr(store, attacker, AttributeHelper.Lifesteal, 0.5f);
        CreatePool(store, attacker, AttributeHelper.DamageShield, current: 30f, max: 100f);
        var defender = Unit(store, hp: 1000f);
        CreateAttr(store, defender, AttributeHelper.DamageReflect, 1f);
        var root = Resolve(store);

        // 第一帧：原伤害 100 → 受击者扣 100，反射 100 打回攻击者，攻击者吸血 50（请求未结算）。
        Hit(store, root, attacker, defender, 100f, DamageType.Physical);
        Check.Near(Hp(defender), 900f, "original damage lands");
        Check.Near(Hp(attacker), 1000f, "heal request is not yet resolved");
        Check.That(Count<HealRequest>(store) == 1, "one lifesteal request from the original hit");
        Check.That(Count<DamageRequest>(store) == 1, "one reflect request from the original hit");

        // 第二帧：结算反射请求（skipPostProcess）：先被攻击者护盾吸收 30，再扣 70；不再产生吸血或反射。
        root.Update(new UpdateTick(0f, 0f));
        Check.Near(AttributeHelper.GetCurrent(attacker, AttributeHelper.DamageShield), 0f,
            "reflect is absorbed by the attacker shield");
        Check.Near(Hp(attacker), 930f, "attacker loses the post-absorb reflect amount of 70");
        var reflectEvt = LastDamageEvent(store, attacker);
        Check.That(reflectEvt.HasValue, "the reflect lands as a damage event");
        Check.Near(reflectEvt!.Value.finalDamage, 70f, "reflect event reports actual 70");
        Check.Near(reflectEvt.Value.absorbed, 30f, "reflect event reports absorbed 30");
        Check.That(Count<HealRequest>(store) == 1, "reflect does not lifesteal again");
        Check.That(Count<DamageRequest>(store) == 0, "reflect does not reflect again");
    }

    /// <summary>无敌不消耗护盾、不吸血、不反射；护盾完全吸收导致实际扣血为 0 时不吸血不反射。</summary>
    private static void ImmuneAndZeroDamage()
    {
        var immuneStore = new EntityStore();
        var immuneAttacker = Unit(immuneStore, hp: 1000f);
        CreateAttr(immuneStore, immuneAttacker, AttributeHelper.Lifesteal, 0.5f);
        var immuneDefender = Unit(immuneStore, hp: 1000f, invulnerable: true);
        CreatePool(immuneStore, immuneDefender, AttributeHelper.DamageShield, current: 80f, max: 100f);
        CreateAttr(immuneStore, immuneDefender, AttributeHelper.DamageReflect, 0.5f);
        var immuneRoot = Resolve(immuneStore);

        Hit(immuneStore, immuneRoot, immuneAttacker, immuneDefender, 100f, DamageType.Physical);
        Check.Near(Hp(immuneDefender), 1000f, "invincible target keeps its life");
        Check.Near(AttributeHelper.GetCurrent(immuneDefender, AttributeHelper.DamageShield), 80f,
            "invincible does not consume the shield");
        Check.That(Count<HealRequest>(immuneStore) == 0, "invincible does not lifesteal");
        Check.That(Count<DamageRequest>(immuneStore) == 0, "invincible does not reflect");
        var immuneEvt = LastDamageEvent(immuneStore, immuneDefender);
        Check.That(immuneEvt is { isImmune: true }, "invincible damage is flagged immune");
        Check.Near(immuneEvt!.Value.absorbed, 0f, "invincible absorbs nothing");

        var zeroStore = new EntityStore();
        var zeroAttacker = Unit(zeroStore, hp: 1000f);
        CreateAttr(zeroStore, zeroAttacker, AttributeHelper.Lifesteal, 0.5f);
        var zeroDefender = Unit(zeroStore, hp: 1000f);
        CreatePool(zeroStore, zeroDefender, AttributeHelper.DamageShield, current: 80f, max: 100f);
        CreateAttr(zeroStore, zeroDefender, AttributeHelper.DamageReflect, 0.5f);
        var zeroRoot = Resolve(zeroStore);

        Hit(zeroStore, zeroRoot, zeroAttacker, zeroDefender, 50f, DamageType.Physical);
        Check.Near(Hp(zeroDefender), 1000f, "fully absorbed damage leaves life unchanged");
        Check.Near(AttributeHelper.GetCurrent(zeroDefender, AttributeHelper.DamageShield), 30f,
            "fully absorbing shield retains 30");
        var zeroEvt = LastDamageEvent(zeroStore, zeroDefender);
        Check.Near(zeroEvt!.Value.finalDamage, 0f, "fully absorbed actual damage is 0");
        Check.Near(zeroEvt.Value.absorbed, 50f, "fully absorbed event reports 50");
        Check.That(Count<HealRequest>(zeroStore) == 0, "zero actual damage does not lifesteal");
        Check.That(Count<DamageRequest>(zeroStore) == 0, "zero actual damage does not reflect");
    }

    /// <summary>未配置护盾/吸血/反射时，扣血、事件与请求产生与基线一致。</summary>
    private static void BaselineWithoutAttributes()
    {
        var store = new EntityStore();
        var attacker = Unit(store, hp: 1000f);
        var defender = Unit(store, hp: 1000f);
        var root = Resolve(store);

        Hit(store, root, attacker, defender, 100f, DamageType.Physical);

        Check.Near(Hp(defender), 900f, "baseline damage applies in full");
        var evt = LastDamageEvent(store, defender);
        Check.Near(evt!.Value.finalDamage, 100f, "baseline event keeps the raw amount");
        Check.Near(evt.Value.absorbed, 0f, "baseline absorbs nothing");
        Check.That(Count<HealRequest>(store) == 0, "baseline produces no heal request");
        Check.That(Count<DamageRequest>(store) == 0, "baseline produces no reflect request");
    }

    /// <summary>源码断言：伤害主流程不内联护盾/吸血/反射分支，注册表不调用 War3 原生 API。</summary>
    private static void NoInlinedSpecialCases()
    {
        var resolve = CompilationTest.Read("War3Frame/Src/Systems/Ability/AbilityEffectSystems.cs");
        foreach (var token in new[] { "AttributeHelper.Lifesteal", "AttributeHelper.DamageReflect", "AttributeHelper.DamageShield" })
            Check.That(!resolve.Contains(token, StringComparison.Ordinal),
                "main resolve flow must not inline " + token);

        var registry = CompilationTest.Read("War3Frame/Src/Helpers/DamagePostProcessRegistry.cs");
        foreach (var native in new[] { "JassApi", "KKApi", "YDApi", "DzApi" })
            Check.That(!registry.Contains(native, StringComparison.Ordinal),
                "damage post-process registry must not call " + native);
        Check.That(registry.Contains("RunAbsorb", StringComparison.Ordinal)
            && registry.Contains("RunPost", StringComparison.Ordinal),
            "registry exposes the two-stage entry points");
    }

    /// <summary>构建仅含伤害结算的本地系统根。</summary>
    private static SystemRoot Resolve(EntityStore store)
    {
        var root = new SystemRoot(store);
        root.Add(new DamageResolveSystem());
        return root;
    }

    /// <summary>创建带生命的测试单位。</summary>
    private static Entity Unit(EntityStore store, float hp, bool invulnerable = false)
    {
        var unit = store.CreateEntity();
        CreateAttr(store, unit, AttributeHelper.Health, hp);
        if (invulnerable)
            CreateAttr(store, unit, AttributeHelper.Invulnerable, 1f);
        return unit;
    }

    /// <summary>创建 DamageRequest 并驱动一帧结算。</summary>
    private static void Hit(EntityStore store, SystemRoot root, Entity source, Entity target, float amount,
        DamageType type)
    {
        store.CreateEntity(new DamageRequest
        {
            source = source,
            target = target,
            damage = new DamageBase
            {
                damage = amount,
                damageType = type,
                damageSrc = DamageSrc.Melee,
                source = source,
                target = target
            }
        });
        root.Update(new UpdateTick(0f, 0f));
    }

    private static float Hp(Entity unit) => AttributeHelper.GetCurrent(unit, AttributeHelper.Health);

    /// <summary>取最后一次针对该目标的 DamageEvent。</summary>
    private static DamageEvent? LastDamageEvent(EntityStore store, Entity target)
    {
        DamageEvent? result = null;
        store.Query<DamageEvent>().ForEachEntity((ref DamageEvent evt, Entity _) =>
        {
            if (evt.target == target)
                result = evt;
        });
        return result;
    }

    private static T Single<T>(EntityStore store) where T : struct, IComponent
    {
        T result = default;
        var count = 0;
        store.Query<T>().ForEachEntity((ref T value, Entity _) =>
        {
            result = value;
            count++;
        });
        Check.That(count == 1, $"expected exactly one {typeof(T).Name}, got {count}");
        return result;
    }

    private static int Count<T>(EntityStore store) where T : struct, IComponent => store.Query<T>().Count;

    /// <summary>本地建属性实体（finalValue = baseValue）。</summary>
    private static Entity CreateAttr(EntityStore store, Entity unit, int typeId, float baseValue)
    {
        var attr = store.CreateEntity(
            new AttrTypeId { typeId = typeId },
            new AttrValue { baseValue = baseValue, finalValue = baseValue, current = baseValue },
            new AttrOwner(unit));
        unit.AddRelation(new HasAttr(attr, typeId));
        return attr;
    }

    /// <summary>本地建资源池属性实体（current 与上限 finalValue 分离，用于护盾）。</summary>
    private static Entity CreatePool(EntityStore store, Entity unit, int typeId, float current, float max)
    {
        var attr = store.CreateEntity(
            new AttrTypeId { typeId = typeId },
            new AttrValue { baseValue = max, finalValue = max, current = current },
            new AttrOwner(unit));
        unit.AddRelation(new HasAttr(attr, typeId));
        return attr;
    }
}

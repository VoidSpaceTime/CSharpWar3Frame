using System;
using System.Collections.Generic;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;
using War3Frame.Src.Systems;

namespace War3Frame.Scripts.Process;

/// <summary>
/// 本地 ECS 验证属性实体生命周期治理（enforce-attr-entity-lifecycle）。
/// 覆盖回收判据：模板未声明 + 无 modifier + 处于初始值（base/current/flat/percent 均为 0）→ 回收；
/// 声明属性、资源现值非 0 属性、无模板单位属性 → 保守保留。
///
/// 说明：AttributeHelper 硬编码 Game.Store，本地场景直接建属性实体（同 ControlStateValidationScenario 范式）；
/// 回收要求 owner 带 UnitSpecData，故本场景为单位挂最小 UnitSpecData（对应真实模板单位）。
/// </summary>
public static class AttributeLifecycleValidationScenario
{
    private const string ScenarioName = "AttributeLifecycleValidationScenario";
    private static readonly List<Entity> _modifiers = new();

    /// <summary>运行不依赖 War3 Native 句柄的同步 ECS 验证。</summary>
    public static void Initialize(JPlayer player)
    {
        _ = player;
        RunValidation();
    }

    /// <summary>同步验证已在 Initialize 完成，运行时无需操作。</summary>
    public static void Update()
    {
    }

    private static void RunValidation()
    {
        var store = new EntityStore();
        var root = CreateSystemRoot(store);

        // 单位 + 模板声明（Health base 100、Mana base 0），对应真实模板单位。
        var unit = store.CreateEntity();
        var spec = new UnitSpec();
        spec.attributes.Add(new UnitAttributeSpec(AttributeHelper.Health, LevelValue.Fixed(100f)));
        spec.attributes.Add(new UnitAttributeSpec(AttributeHelper.Mana, LevelValue.Fixed(0f)));
        unit.AddComponent(new UnitSpecData { spec = spec });

        // 声明属性：base 100 → 必须保留。
        var healthAttr = CreateAttr(store, unit, AttributeHelper.Health, 100f);

        // 声明属性：base 0、无 modifier → 必须保留（声明保留，不因归零被回收）。
        var manaAttr = CreateAttr(store, unit, AttributeHelper.Mana, 0f);

        // 未声明属性：base 0、带 modifier → 有贡献者时保留。
        var shieldAttr = CreateAttr(store, unit, AttributeHelper.Armor, 0f);

        // 未声明属性：base 0、但 current=5（资源现值残留）→ 必须保留（判据 current==0）。
        var resourceAttr = CreateAttr(store, unit, AttributeHelper.MagicResist, 0f);
        OverwriteCurrent(resourceAttr, 5f);

        var source = store.CreateEntity();
        AddModifier(store, shieldAttr, source, ModifyType.Flat, 100f);

        MarkAllDirty(healthAttr, manaAttr, shieldAttr, resourceAttr);
        root.Update(new UpdateTick(0f, 0f));

        // ---- Phase 1：全部保留（声明 / 有贡献者 / 现值非 0）----
        Require(HasAttrType(unit, AttributeHelper.Health), "phase1/declaredHealthKept");
        Require(HasAttrType(unit, AttributeHelper.Mana), "phase1/declaredManaKept");
        Require(GetFinal(unit, AttributeHelper.Armor) == 100f, "phase1/shieldContribution");
        Require(HasAttrType(unit, AttributeHelper.Armor), "phase1/shieldWithModifierKept");
        Require(HasAttrType(unit, AttributeHelper.MagicResist), "phase1/resourceCurrentKept");

        // ---- Phase 2：移除护盾贡献者 → 未声明 + 归零 → 回收 ----
        RemoveModifier(source, shieldAttr);
        root.Update(new UpdateTick(0f, 0f));

        Require(!HasAttrType(unit, AttributeHelper.Armor), "phase2/shieldReclaimed");
        Require(CountAttrEntitiesByType(store, AttributeHelper.Armor) == 0, "phase2/shieldEntityDeleted");
        Require(HasAttrType(unit, AttributeHelper.Health), "phase2/healthStillKept");
        Require(HasAttrType(unit, AttributeHelper.Mana), "phase2/manaStillKept");
        Require(HasAttrType(unit, AttributeHelper.MagicResist), "phase2/resourceStillKept");

        // ---- Phase 3：资源现值归零后，未声明属性可回收 ----
        OverwriteCurrent(resourceAttr, 0f);
        resourceAttr.AddTag<AttrDirty>();
        root.Update(new UpdateTick(0f, 0f));
        Require(!HasAttrType(unit, AttributeHelper.MagicResist), "phase3/resourceReclaimedAfterZero");

        Console.WriteLine($"{ScenarioName}: PASS");
    }

    /// <summary>本地系统树：仅属性计算（重算 + 回收），不注册 Native 系统。</summary>
    private static TimedSystemRoot CreateSystemRoot(EntityStore store)
    {
        var root = new TimedSystemRoot(store);
        root.Add(new AttrCalculationSystem(), 0f);
        return root;
    }

    /// <summary>本地建属性实体（对应 AttributeHelper.CreateAttr，绕开 Game.Store）。</summary>
    private static Entity CreateAttr(EntityStore store, Entity unit, int typeId, float baseValue)
    {
        var attr = store.CreateEntity(
            new AttrTypeId { typeId = typeId },
            new AttrValue { baseValue = baseValue, finalValue = baseValue, current = baseValue },
            new AttrOwner(unit));
        unit.AddRelation(new HasAttr(attr, typeId));
        return attr;
    }

    /// <summary>改写属性的 current（用于构造资源现值残留场景）。</summary>
    private static void OverwriteCurrent(Entity attr, float current)
    {
        if (!attr.TryGetComponent<AttrValue>(out var value))
            return;

        value.current = current;
        attr.AddComponent(value);
    }

    /// <summary>本地建 Flat 修改器并打 AttrDirty（对应 ModifyHelper.AddModifier）。</summary>
    private static void AddModifier(EntityStore store, Entity attr, Entity source, ModifyType type, float value)
    {
        var mod = store.CreateEntity(
            new ModifyValue { modifyType = type, value = value },
            new ModifyTarget(attr),
            new ModifySource(source));
        _modifiers.Add(mod);
        attr.AddTag<AttrDirty>();
    }

    /// <summary>按 source 移除修改器并打脏，驱动下一轮重算。</summary>
    private static void RemoveModifier(Entity source, Entity attr)
    {
        for (var i = _modifiers.Count - 1; i >= 0; i--)
        {
            if (_modifiers[i].IsNull)
                continue;

            if (_modifiers[i].TryGetComponent<ModifySource>(out var modSource) && modSource.source == source)
            {
                _modifiers[i].DeleteEntity();
                _modifiers.RemoveAt(i);
            }
        }

        attr.AddTag<AttrDirty>();
    }

    private static void MarkAllDirty(params Entity[] attrs)
    {
        foreach (var attr in attrs)
        {
            if (!attr.IsNull)
                attr.AddTag<AttrDirty>();
        }
    }

    private static bool HasAttrType(Entity unit, int typeId)
    {
        foreach (ref var rel in unit.GetRelations<HasAttr>())
        {
            if (rel.typeId == typeId)
                return true;
        }

        return false;
    }

    private static float GetFinal(Entity unit, int typeId)
    {
        foreach (ref var rel in unit.GetRelations<HasAttr>())
        {
            if (rel.typeId == typeId)
                return rel.attrEntity.GetComponent<AttrValue>().finalValue;
        }

        return 0f;
    }

    private static int CountAttrEntitiesByType(EntityStore store, int typeId)
    {
        var count = 0;
        store.Query<AttrValue>().ForEachEntity((ref AttrValue _, Entity entity) =>
        {
            if (entity.TryGetComponent<AttrTypeId>(out var attrType) && attrType.typeId == typeId)
                count++;
        });
        return count;
    }

    private static void Require(bool condition, string invariantContext)
    {
        if (!condition)
            throw new InvalidOperationException($"{ScenarioName}: {invariantContext}");
    }
}

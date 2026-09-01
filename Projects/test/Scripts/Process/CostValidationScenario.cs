using System;
using Friflo.Engine.ECS;
using War3Frame.Components;
using War3Frame.Helpers;

namespace War3Frame.Scripts.Process;

/// <summary>
/// 以本地 ECS Store 同步验证消耗类型注册表（CostConditionRegistry）：
/// Mana 组件化双读、ItemCost 补齐、ApplyCost 单项不足不扣。
/// 说明：AttributeHelper 硬编码 Game.Store，本地场景直接用 store 建属性实体（同 ControlStateValidationScenario）。
/// </summary>
public static class CostValidationScenario
{
    private const string ScenarioName = "CostValidationScenario";

    public static void Initialize(JPlayer player)
    {
        _ = player;
        RunValidation();
    }

    public static void Update()
    {
        // 同步验证已在 Initialize 中完成。
    }

    private static void RunValidation()
    {
        var store = new EntityStore();
        var unit = store.CreateEntity();
        var manaAttr = CreateAttr(store, unit, AttributeHelper.Mana, 100f);
        var healthAttr = CreateAttr(store, unit, AttributeHelper.Health, 100f);

        // ---- Phase 1：Mana 组件路径（组件化蓝耗）----
        var abilityA = store.CreateEntity(new ManaCost { value = 50f });
        Require(AbilityCostHelper.CheckCost(unit, abilityA), "phase1/check: 蓝量 100 >= 50 应通过");
        AbilityCostHelper.ApplyCost(unit, abilityA);
        Require(Math.Abs(AttributeHelper.GetCurrent(unit, AttributeHelper.Mana) - 50f) < 0.001f,
            "phase1/apply: 扣除后蓝量应为 50");
        AbilityCostHelper.ApplyCost(unit, abilityA);
        Require(Math.Abs(AttributeHelper.GetCurrent(unit, AttributeHelper.Mana)) < 0.001f,
            "phase1/apply2: 再次扣除后蓝量应为 0");
        Require(!AbilityCostHelper.CheckCost(unit, abilityA), "phase1/insufficient: 蓝量 0 < 50 应拒绝");

        // ---- Phase 2：无蓝耗组件/Stat 时回退（GetManaCost 双读兼容）----
        var abilityB = store.CreateEntity();
        Require(AbilityHelper.GetManaCost(abilityB) <= 0f, "phase2/fallback: 无组件无 Stat 蓝耗应为 0");
        Require(AbilityCostHelper.CheckCost(unit, abilityB), "phase2/check: 蓝耗项不存在应视为满足");

        // ---- Phase 3：ItemCost 正常扣除 ----
        var abilityC = store.CreateEntity(new ItemCost { templateName = "potion", count = 2 });
        unit.AddComponent(new ItemSlotContainer { maxSlots = 6, currentCount = 0 });
        var potion1 = CreateItem(store, unit, "potion", 0);
        var potion2 = CreateItem(store, unit, "potion", 1);
        Require(CountItems(unit, "potion") == 2, "phase3/seed: 背包应有 2 个 potion");
        Require(AbilityCostHelper.CheckCost(unit, abilityC), "phase3/check: 数量 2 >= 2 应通过");
        AbilityCostHelper.ApplyCost(unit, abilityC);
        Require(CountItems(unit, "potion") == 0, "phase3/apply: 扣除后背包应剩 0");
        Require(!AbilityCostHelper.CheckCost(unit, abilityC), "phase3/insufficient: 扣除后数量 0 < 2 应拒绝");

        // ---- Phase 4：ItemCost 数量不足不扣（原子）----
        var abilityD = store.CreateEntity(new ItemCost { templateName = "potion", count = 2 });
        var potion3 = CreateItem(store, unit, "potion", 0);
        Require(!AbilityCostHelper.CheckCost(unit, abilityD), "phase4/check: 数量 1 < 2 应拒绝");
        AbilityCostHelper.ApplyCost(unit, abilityD); // 绕过 CheckCost 直接扣
        Require(CountItems(unit, "potion") == 1, "phase4/noPartial: 数量不足应完全不扣");

        // ---- Phase 5：AttributeCost 任意属性消耗 ----
        var abilityE = store.CreateEntity(new AttributeCost { attrId = AttributeHelper.Health, value = 30f });
        Require(AbilityCostHelper.CheckCost(unit, abilityE), "phase5/check: 血量 100 >= 30 应通过");
        AbilityCostHelper.ApplyCost(unit, abilityE);
        Require(Math.Abs(AttributeHelper.GetCurrent(unit, AttributeHelper.Health) - 70f) < 0.001f,
            "phase5/apply: 扣除后血量应为 70");
        Require(AbilityCostHelper.CheckCost(unit, abilityE), "phase5/again: 血量 70 >= 30 第二轮仍应通过");
        AbilityCostHelper.ApplyCost(unit, abilityE); // 70 -> 40
        Require(AbilityCostHelper.CheckCost(unit, abilityE), "phase5/again2: 血量 40 >= 30 第三轮仍应通过");
        AbilityCostHelper.ApplyCost(unit, abilityE); // 40 -> 10
        Require(!AbilityCostHelper.CheckCost(unit, abilityE), "phase5/insufficient: 血量 10 < 30 应拒绝");

        // 清理未用实体（potion3 仍在背包，无碍）
        _ = manaAttr;
        _ = healthAttr;
        _ = potion1;
        _ = potion2;
        _ = potion3;

        Console.WriteLine($"{ScenarioName}: PASS");
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

    /// <summary>本地建背包物品实体（挂归属/槽位/背包标记）。</summary>
    private static Entity CreateItem(EntityStore store, Entity unit, string templateName, int slotIndex)
    {
        var item = store.CreateEntity(new ItemBase
        {
            templateName = templateName,
            name = templateName,
            stackCount = 1,
            maxStack = 1,
        });
        item.AddComponent(new ItemOwner(unit));
        item.AddComponent(new ItemSlotIndex { index = slotIndex });
        item.AddTag<ItemInventoryTag>();
        return item;
    }

    /// <summary>统计单位背包内指定模板名物品数量。</summary>
    private static int CountItems(Entity unit, string templateName)
    {
        var count = 0;
        foreach (var link in unit.GetIncomingLinks<ItemOwner>())
        {
            var item = link.Entity;
            if (!item.Tags.Has<ItemInventoryTag>())
                continue;
            if (item.TryGetComponent<ItemBase>(out var itemBase) && itemBase.templateName == templateName)
                count++;
        }

        return count;
    }

    private static void Require(bool condition, string invariantContext)
    {
        if (!condition)
            throw new InvalidOperationException($"{ScenarioName}: {invariantContext}");
    }
}
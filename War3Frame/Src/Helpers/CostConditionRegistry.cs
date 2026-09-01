using System.Collections.Generic;
using Friflo.Engine.ECS;
using War3Frame.Components;
using War3Frame.Helpers;

namespace War3Frame;

/// <summary>
/// 消耗检查结果：CheckCost 用 satisfied，ApplyCost 用 applied。
/// 项不存在（未声明该消耗）时返回 None（satisfied=true, applied=false）。
/// </summary>
public readonly struct CostResult
{
    public readonly bool satisfied;   // CheckCost：资源是否足够
    public readonly bool applied;     // ApplyCost：是否实际扣除
    public readonly float value;      // 消耗数值（UI 显示）

    public CostResult(bool satisfied, bool applied, float value)
    {
        this.satisfied = satisfied;
        this.applied = applied;
        this.value = value;
    }

    /// <summary>项不存在：判定视为满足，扣除视为无操作。</summary>
    public static readonly CostResult None = new(true, false, 0f);
}

/// <summary>消耗判定委托：单位是否满足该项消耗（项不存在返回满足）。</summary>
public delegate CostResult CostCheckHandler(Entity unit, Entity ability);

/// <summary>消耗扣除委托：执行扣减；单项不足跳过（applied=false），不扣成负数。</summary>
public delegate CostResult CostDepleteHandler(Entity unit, Entity ability);

/// <summary>已注册消耗项（判定 + 扣除）。</summary>
public sealed class CostConditionEntry
{
    public readonly string Name;
    public readonly CostCheckHandler Check;
    public readonly CostDepleteHandler Deplete;

    public CostConditionEntry(string name, CostCheckHandler check, CostDepleteHandler deplete)
    {
        Name = name;
        Check = check;
        Deplete = deplete;
    }
}

/// <summary>
/// 消耗类型注册表（有序检查器列表，形态对齐 EffectFormulaRegistry）。
/// 注册顺序 = CheckCost 短路判定顺序 与 ApplyCost 扣除顺序。
/// </summary>
public static class CostConditionRegistry
{
    private static readonly List<CostConditionEntry> _entries = new();

    static CostConditionRegistry()
    {
        RegisterBuiltIn();
    }

    /// <summary>注册自定义消耗类型（地图/模组初始化时调用）。</summary>
    public static void Register(string name, CostCheckHandler check, CostDepleteHandler deplete)
    {
        _entries.Add(new CostConditionEntry(name, check, deplete));
    }

    /// <summary>全部已注册消耗项（按注册顺序）。</summary>
    public static IReadOnlyList<CostConditionEntry> Entries => _entries;

    private static void RegisterBuiltIn()
    {
        Register("Mana", CheckMana, DepleteMana);
        Register("Health", CheckHealth, DepleteHealth);
        Register("Attribute", CheckAttribute, DepleteAttribute);
        Register("Item", CheckItem, DepleteItem);
    }

    // ---- Mana（GetManaCost 双读：ManaCost 组件优先，回退 AbilityStat）----

    private static CostResult CheckMana(Entity unit, Entity ability)
    {
        var cost = AbilityHelper.GetManaCost(ability);
        if (cost <= 0f)
            return CostResult.None;

        var current = AttributeHelper.GetCurrent(unit, AttributeHelper.Mana);
        return new CostResult(current >= cost, false, cost);
    }

    private static CostResult DepleteMana(Entity unit, Entity ability)
    {
        var cost = AbilityHelper.GetManaCost(ability);
        if (cost <= 0f)
            return CostResult.None;

        var current = AttributeHelper.GetCurrent(unit, AttributeHelper.Mana);
        if (current < cost)
            return new CostResult(false, false, cost);

        AttributeHelper.ModifyCurrent(unit, AttributeHelper.Mana, -cost);
        return new CostResult(true, true, cost);
    }

    // ---- Health（HealthCost 组件）----

    private static CostResult CheckHealth(Entity unit, Entity ability)
    {
        if (!ability.TryGetComponent<HealthCost>(out var cost))
            return CostResult.None;

        var current = AttributeHelper.GetCurrent(unit, AttributeHelper.Health);
        return new CostResult(current >= cost.value, false, cost.value);
    }

    private static CostResult DepleteHealth(Entity unit, Entity ability)
    {
        if (!ability.TryGetComponent<HealthCost>(out var cost))
            return CostResult.None;

        var current = AttributeHelper.GetCurrent(unit, AttributeHelper.Health);
        if (current < cost.value)
            return new CostResult(false, false, cost.value);

        AttributeHelper.ModifyCurrent(unit, AttributeHelper.Health, -cost.value);
        return new CostResult(true, true, cost.value);
    }

    // ---- Attribute（AttributeCost 组件，任意属性）----

    private static CostResult CheckAttribute(Entity unit, Entity ability)
    {
        if (!ability.TryGetComponent<AttributeCost>(out var cost))
            return CostResult.None;

        var current = AttributeHelper.GetCurrent(unit, cost.attrId);
        return new CostResult(current >= cost.value, false, cost.value);
    }

    private static CostResult DepleteAttribute(Entity unit, Entity ability)
    {
        if (!ability.TryGetComponent<AttributeCost>(out var cost))
            return CostResult.None;

        var current = AttributeHelper.GetCurrent(unit, cost.attrId);
        if (current < cost.value)
            return new CostResult(false, false, cost.value);

        AttributeHelper.ModifyCurrent(unit, cost.attrId, -cost.value);
        return new CostResult(true, true, cost.value);
    }

    // ---- Item（ItemCost 组件，按模板名匹配背包物品）----

    private static CostResult CheckItem(Entity unit, Entity ability)
    {
        if (!ability.TryGetComponent<ItemCost>(out var cost))
            return CostResult.None;

        if (string.IsNullOrEmpty(cost.templateName) || cost.count <= 0)
            return CostResult.None;

        return new CostResult(CountItems(unit, cost.templateName) >= cost.count, false, cost.count);
    }

    private static CostResult DepleteItem(Entity unit, Entity ability)
    {
        if (!ability.TryGetComponent<ItemCost>(out var cost))
            return CostResult.None;

        if (string.IsNullOrEmpty(cost.templateName) || cost.count <= 0)
            return CostResult.None;
        // 原子语义：数量不足则完全不扣（applied=false）；充足则全扣。
        if (CountItems(unit, cost.templateName) < cost.count)
            return new CostResult(false, false, cost.count);

        var removed = RemoveItems(unit, cost.templateName, cost.count);
        return new CostResult(removed >= cost.count, removed > 0, cost.count);
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

    /// <summary>
    /// 移除指定数量物品（简化解除：减槽位计数 + 清归属 + 删实体）。
    /// 返回实际移除数；不足则移除可用部分（配合 CheckCost 短路，正常路径数量必然充足）。
    /// 有 companion 的消耗物品建议走 ItemDestroyRequest 受控流程，本路径不处理 companion。
    /// </summary>
    private static int RemoveItems(Entity unit, string templateName, int count)
    {
        var removed = 0;
        var hasContainer = unit.TryGetComponent<ItemSlotContainer>(out var container);

        foreach (var link in unit.GetIncomingLinks<ItemOwner>())
        {
            if (removed >= count)
                break;

            var item = link.Entity;
            if (!item.Tags.Has<ItemInventoryTag>())
                continue;
            if (!item.TryGetComponent<ItemBase>(out var itemBase) || itemBase.templateName != templateName)
                continue;

            if (hasContainer)
            {
                container.currentCount = System.Math.Max(0, container.currentCount - 1);
                unit.AddComponent(container);
            }

            item.RemoveComponent<ItemOwner>();
            item.RemoveComponent<ItemSlotIndex>();
            item.RemoveTag<ItemInventoryTag>();
            item.DeleteEntity();
            removed++;
        }

        return removed;
    }
}
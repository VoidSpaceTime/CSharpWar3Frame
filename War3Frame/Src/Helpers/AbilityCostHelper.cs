using Friflo.Engine.ECS;
using War3Frame.Helpers;
using War3Frame.Src.Systems;


namespace War3Frame;

/// <summary>
/// 技能消耗帮助类 - 检查和扣除技能消耗
/// </summary>
public static class AbilityCostHelper
{
    private static int _manaAttrId = AttributeHelper.Mana;
    private static int _healthAttrId = AttributeHelper.Health;

    /// <summary>
    /// 检查资源是否足够施放技能
    /// 支持 Mana, Health, 以及通过 AttributeCost 指定的任意属性
    /// </summary>
    public static bool CheckCost(Entity unit, Entity ability)
    {
        // 1. 检查蓝耗
        var manaCost = AbilityHelper.GetManaCost(ability);
        if (manaCost > 0)
        {
            float mana = AttributeHelper.GetCurrent(unit, AttributeHelper.Mana);
            if (mana < manaCost) return false;
        }

        // 2. 检查血耗
        if (ability.TryGetComponent<HealthCost>(out var hpCost))
        {
            float hp = AttributeHelper.GetCurrent(unit, AttributeHelper.Health);
            if (hp < hpCost.value) return false;

            // 可选：如果不允许自杀
            // if (hp - hpCost.value <= 0) return false;
        }

        // 3. 检查通用属性消耗
        if (ability.TryGetComponent<AttributeCost>(out var attrCost))
        {
            float current = AttributeHelper.GetCurrent(unit, attrCost.attrId);
            if (current < attrCost.value) return false;
        }

        return true;
    }

    /// <summary>
    /// 扣除技能消耗
    /// </summary>
    public static void ApplyCost(Entity unit, Entity ability)
    {
        // 1. 扣蓝
        var manaCost = AbilityHelper.GetManaCost(ability);
        if (manaCost > 0)
        {
            AttributeHelper.ModifyCurrent(unit, AttributeHelper.Mana, -manaCost);
        }

        // 2. 扣血
        if (ability.TryGetComponent<HealthCost>(out var hpCost))
        {
            AttributeHelper.ModifyCurrent(unit, AttributeHelper.Health, -hpCost.value);
        }

        // 3. 扣除通用属性消耗
        if (ability.TryGetComponent<AttributeCost>(out var attrCost))
        {
            AttributeHelper.ModifyCurrent(unit, attrCost.attrId, -attrCost.value);
        }
    }

    // 扩展方法：检查是否拥有属性 (临时)
    private static bool TryGetAttr(this Entity unit, int attrTypeId, out Entity attrEntity)
    {
        attrEntity = AttributeHelper.GetAttr(unit, attrTypeId) ?? default;
        return !attrEntity.IsNull;
    }
}

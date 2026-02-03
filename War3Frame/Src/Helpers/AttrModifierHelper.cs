using Friflo.Engine.ECS;

namespace War3Frame;

/// <summary>
///     属性修改器辅助类 - 提供便捷的修改器操作方法
/// </summary>
public static class AttrModifierHelper
{
    #region 添加修改器

    /// <summary>
    ///     为单位添加属性修改器
    /// </summary>
    /// <param name="store">EntityStore</param>
    /// <param name="unit">目标单位</param>
    /// <param name="source">来源实体（物品/技能/Buff）</param>
    /// <param name="attrType">属性类型</param>
    /// <param name="modifyType">修改方式</param>
    /// <param name="value">修改值</param>
    /// <param name="sourceType">来源类型</param>
    /// <param name="priority">优先级</param>
    /// <returns>创建的修改器 Entity</returns>
    public static Entity AddModifier(
        EntityStore store,
        Entity unit,
        Entity source,
        AttrType attrType,
        ModifyType modifyType,
        float value,
        ModifierSourceType sourceType = ModifierSourceType.Other,
        int priority = 0)
    {
        var modifier = store.CreateEntity(
            new AttrModifier
            {
                attrType = attrType,
                modifyType = modifyType,
                value = value,
                priority = priority,
                sourceType = sourceType
            },
            new ModifierTarget(unit),
            new ModifierSource(source)
        );

        // 标记单位属性需要重算
        unit.AddTag<AttrsDirty>();

        return modifier;
    }

    /// <summary>
    ///     添加固定加成修改器
    /// </summary>
    public static Entity AddFlatModifier(
        EntityStore store,
        Entity unit,
        Entity source,
        AttrType attrType,
        float value,
        ModifierSourceType sourceType = ModifierSourceType.Other)
    {
        return AddModifier(store, unit, source, attrType, ModifyType.Flat, value, sourceType);
    }

    /// <summary>
    ///     添加百分比加成修改器
    /// </summary>
    public static Entity AddPercentModifier(
        EntityStore store,
        Entity unit,
        Entity source,
        AttrType attrType,
        float percent,
        ModifierSourceType sourceType = ModifierSourceType.Other)
    {
        return AddModifier(store, unit, source, attrType, ModifyType.PercentAdd, percent, sourceType);
    }

    #endregion

    #region 移除修改器

    /// <summary>
    ///     移除来自指定来源的所有修改器
    /// </summary>
    /// <param name="store">EntityStore</param>
    /// <param name="unit">目标单位</param>
    /// <param name="source">来源实体</param>
    public static void RemoveModifiersFromSource(EntityStore store, Entity unit, Entity source)
    {
        // 获取指向该来源的所有修改器
        var modifiers = source.GetIncomingLinks<ModifierSource>();

        foreach (var link in modifiers)
        {
            var modifierEntity = link.Entity;
            // 确认该修改器确实指向目标单位
            if (modifierEntity.TryGetComponent<ModifierTarget>(out var target) && target.target == unit)
            {
                modifierEntity.DeleteEntity();
            }
        }

        // 标记单位属性需要重算
        if (!unit.IsNull)
        {
            unit.AddTag<AttrsDirty>();
        }
    }

    /// <summary>
    ///     移除单位的所有修改器
    /// </summary>
    public static void RemoveAllModifiers(EntityStore store, Entity unit)
    {
        var modifiers = unit.GetIncomingLinks<ModifierTarget>();

        foreach (var link in modifiers)
        {
            link.Entity.DeleteEntity();
        }

        if (!unit.IsNull)
        {
            unit.AddTag<AttrsDirty>();
        }
    }

    /// <summary>
    ///     移除单位指定类型的所有修改器
    /// </summary>
    public static void RemoveModifiersByType(EntityStore store, Entity unit, ModifierSourceType sourceType)
    {
        var modifiers = unit.GetIncomingLinks<ModifierTarget>();

        foreach (var link in modifiers)
        {
            var modifierEntity = link.Entity;
            if (modifierEntity.TryGetComponent<AttrModifier>(out var mod) && mod.sourceType == sourceType)
            {
                modifierEntity.DeleteEntity();
            }
        }

        if (!unit.IsNull)
        {
            unit.AddTag<AttrsDirty>();
        }
    }

    #endregion

    #region 批量添加

    /// <summary>
    ///     批量添加多个修改器（如物品属性）
    /// </summary>
    public static void AddModifierBatch(
        EntityStore store,
        Entity unit,
        Entity source,
        ModifierSourceType sourceType,
        params (AttrType attr, ModifyType type, float value)[] modifiers)
    {
        foreach (var (attr, type, value) in modifiers)
        {
            AddModifier(store, unit, source, attr, type, value, sourceType);
        }
    }

    #endregion

    #region 查询

    /// <summary>
    ///     获取单位某个属性的所有修改器
    /// </summary>
    public static List<(Entity entity, AttrModifier modifier)> GetModifiersForAttr(Entity unit, AttrType attrType)
    {
        var result = new List<(Entity, AttrModifier)>();
        var modifiers = unit.GetIncomingLinks<ModifierTarget>();

        foreach (var link in modifiers)
        {
            var modifierEntity = link.Entity;
            if (modifierEntity.TryGetComponent<AttrModifier>(out var mod) && mod.attrType == attrType)
            {
                result.Add((modifierEntity, mod));
            }
        }

        return result;
    }

    /// <summary>
    ///     获取单位的所有修改器总数
    /// </summary>
    public static int GetModifierCount(Entity unit)
    {
        return unit.GetIncomingLinks<ModifierTarget>().Count;
    }

    #endregion
}

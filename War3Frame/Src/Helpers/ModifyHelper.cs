using Friflo.Engine.ECS;
using War3Frame.Components;

namespace War3Frame;

public class ModifyHelper
{
    /// <summary>添加修改器到属性</summary>
    // 创建一个长期 modifier 实体，并通过 ModifyTarget / ModifySource 建立双向可查询关系。
    public static Entity AddModifier(
        Entity attrEntity,
        Entity source,
        ModifyType type,
        float value,
        int priority = 0)
    {
        var mod = Game.Store.CreateEntity(
            new ModifyValue { modifyType = type, value = value, priority = priority },
            new ModifyTarget(attrEntity),
            new ModifySource(source)
        );

        // modifier 只写入贡献项，最终值由 AttrCalculationSystem 统一重算。
        attrEntity.AddTag<AttrDirty>();
        return mod;
    }

    /// <summary>为 Unit 的某属性添加修改器</summary>
    // 对单位添加 modifier 的便利入口：找不到属性时不创建悬挂 modifier。
    public static Entity? AddModifierToUnit(
        Entity unit,
        int attrTypeId,
        Entity source,
        ModifyType type,
        float value)
    {
        var attr = AttributeHelper.GetAttr(unit, attrTypeId);
        if (attr == null) return null;

        return AddModifier(attr.Value, source, type, value);
    }

    /// <summary>移除来源的所有修改器</summary>
    // 以 source 为维度批量移除 modifier，适合 Buff/Item/Ability 卸载时回收贡献。
    public static void RemoveModifiersFromSource(Entity source)
    {
        var links = source.GetIncomingLinks<ModifySource>();
        var affectedAttrs = new HashSet<Entity>();

        foreach (var link in links)
        {
            var modEntity = link.Entity;
            if (modEntity.TryGetComponent<ModifyTarget>(out var target))
            {
                affectedAttrs.Add(target.target);
            }

            modEntity.DeleteEntity();
        }

        // 标记受影响属性需重算
        // 被移除 modifier 影响过的属性需要重新计算，避免面板和 native 同步读到旧值。
        foreach (var attr in affectedAttrs)
        {
            if (!attr.IsNull)
                attr.AddTag<AttrDirty>();
        }
    }
}

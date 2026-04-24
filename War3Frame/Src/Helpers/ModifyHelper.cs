using Friflo.Engine.ECS;
using War3Frame.Components;

namespace War3Frame;

public class ModifyHelper
{
    /// <summary>添加修改器到属性</summary>
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

        attrEntity.AddTag<AttrDirty>();
        return mod;
    }

    /// <summary>为 Unit 的某属性添加修改器</summary>
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
        foreach (var attr in affectedAttrs)
        {
            if (!attr.IsNull)
                attr.AddTag<AttrDirty>();
        }
    }
}
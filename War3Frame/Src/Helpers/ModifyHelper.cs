using Friflo.Engine.ECS;
using War3Frame.Components;

namespace War3Frame;

public class ModifyHelper
{
    /// <summary>添加修改器到属性（同程序集内部原语，要求调用方已解析出唯一属性实体）</summary>
    // 创建一个长期 modifier 实体，并通过 ModifyTarget / ModifySource 建立双向可查询关系。
    // 收窄为 internal：对外统一走 AddModifierToUnit，避免传入重复或不属于该单位的裸 attrEntity。
    internal static Entity AddModifier(
        Entity attrEntity,
        Entity source,
        ModifyType type,
        float value,
        int priority = 0)
    {
        if (!source.IsNull && !ReferenceEquals(attrEntity.Store, source.Store))
            throw new ArgumentException("修改器与来源必须属于同一 Store", nameof(source));
        var mod = attrEntity.Store.CreateEntity(
            new ModifyValue { modifyType = type, value = ResolveAbsorbedValue(attrEntity, value), priority = priority },
            new ModifyTarget(attrEntity),
            new ModifySource(source)
        );

        // modifier 只写入贡献项，最终值由 AttrCalculationSystem 统一重算。
        attrEntity.AddTag<AttrDirty>();
        return mod;
    }

    /// <summary>
    /// 控制属性在无敌/免疫期间施加时把贡献降为 0（吸收），避免无敌结束后延迟生效。
    /// 非控制属性、零值、或无法解析所属单位时原样返回，不影响其它属性的贡献语义。
    /// </summary>
    private static float ResolveAbsorbedValue(Entity attrEntity, float value)
    {
        if (value == 0f) return 0f;
        if (!attrEntity.TryGetComponent<AttrTypeId>(out var attrType)) return value;
        if (!attrEntity.TryGetComponent<AttrOwner>(out var attrOwner) || attrOwner.owner.IsNull) return value;

        return ControlHelper.ShouldAbsorbControl(attrOwner.owner, attrType.typeId) ? 0f : value;
    }

    /// <summary>为 Unit 的某属性添加修改器</summary>
    // 对单位添加 modifier 的便利入口：属性实体不存在时自动创建 base=0 的属性实体，消除静默丢弃。
    public static Entity? AddModifierToUnit(
        Entity unit,
        int attrTypeId,
        Entity source,
        ModifyType type,
        float value)
    {
        var attr = AttributeHelper.GetOrCreateAttr(unit, attrTypeId);
        if (attr.IsNull) return null;

        return AddModifier(attr, source, type, value);
    }

    /// <summary>移除来源的所有修改器</summary>
    // 以 source 为维度批量移除 modifier，适合 Buff/Item/Ability 卸载时回收贡献。
    public static void RemoveModifiersFromSource(Entity source)
    {
        var links = source.GetIncomingLinks<ModifySource>();
        var affectedAttrs = new HashSet<Entity>();
        var toDelete = new List<Entity>();

        foreach (var link in links)
        {
            var modEntity = link.Entity;
            if (modEntity.TryGetComponent<ModifyTarget>(out var target))
            {
                affectedAttrs.Add(target.target);
            }

            toDelete.Add(modEntity);
        }
        foreach (var modifier in toDelete)
            if (!modifier.IsNull) modifier.DeleteEntity();

        // 标记受影响属性需重算
        // 被移除 modifier 影响过的属性需要重新计算，避免面板和 native 同步读到旧值。
        foreach (var attr in affectedAttrs)
        {
            if (!attr.IsNull)
                attr.AddTag<AttrDirty>();
        }
    }
}

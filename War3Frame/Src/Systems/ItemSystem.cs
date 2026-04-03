using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;
using War3Frame.Systems;

namespace War3Frame.Src.Systems;

/// <summary>
/// 物品属性应用系统。
/// 仅在装备态下，将物品定义的属性映射为单位属性修改器。
/// </summary>
[SystemRegister(SystemKind.Interval, 0)]
public class ItemAttributeApplySystem : QuerySystem<ItemOwner, ItemAttrModifier>
{
    public ItemAttributeApplySystem()
    {
        Filter.AnyTags(Tags.Get<ItemAttrApplyRequest>());
    }

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref ItemOwner owner, ref ItemAttrModifier modifier, Entity item) =>
        {
            if (!item.Tags.Has<ItemEquippedTag>())
            {
                item.RemoveTag<ItemAttrApplyRequest>();
                return;
            }

            if (owner.unit.IsNull)
            {
                item.RemoveTag<ItemAttrApplyRequest>();
                return;
            }

            ModifyHelper.RemoveModifiersFromSource(item);
            ModifyHelper.AddModifierToUnit(owner.unit, modifier.attrTypeId, item, modifier.modifyType, modifier.value);
            item.RemoveTag<ItemAttrApplyRequest>();
        });
    }
}

/// <summary>
/// 物品属性移除系统。
/// 用于卸下、丢弃或销毁物品时撤销其带来的属性修改。
/// </summary>
[SystemRegister(SystemKind.Interval, 0)]
public class ItemAttributeRemoveSystem : QuerySystem<ItemOwner>
{
    public ItemAttributeRemoveSystem()
    {
        Filter.AnyTags(Tags.Get<ItemAttrRemoveRequest>());
    }

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref ItemOwner owner, Entity item) =>
        {
            ModifyHelper.RemoveModifiersFromSource(item);
            item.RemoveTag<ItemAttrRemoveRequest>();
        });
    }
}
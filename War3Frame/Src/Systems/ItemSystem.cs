using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;
using War3Frame.Systems;

namespace War3Frame.Src.Systems;

/// <summary>
/// 物品属性应用系统。
/// 仅在装备态下，将物品定义的统一属性贡献条目映射为单位属性修改器。
/// </summary>
[SystemRegister(SystemKind.Interval, 0)]
public class ItemAttributeApplySystem : QuerySystem<ItemOwner, AttributeContributionEntry>
{
    public ItemAttributeApplySystem()
    {
        Filter.AnyTags(Tags.Get<ItemAttrApplyRequest>());
    }

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref ItemOwner owner, ref AttributeContributionEntry contribution, Entity item) =>
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
            ModifyHelper.AddModifierToUnit(owner.unit, contribution.attrTypeId, item, contribution.modifyType, contribution.value);
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

/// <summary>
/// 挂载技能属性应用系统。
/// 将 ability 的统一贡献条目映射为所属单位的属性修改器。
/// </summary>
[SystemRegister(SystemKind.Interval, 0)]
public class AbilityAttributeApplySystem : QuerySystem<AbilityOwner, AttributeContributionEntry>
{
    public AbilityAttributeApplySystem()
    {
        Filter.AnyTags(Tags.Get<AbilityAttrApplyRequest>());
    }

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref AbilityOwner owner, ref AttributeContributionEntry contribution, Entity ability) =>
        {
            if (owner.owner.IsNull)
            {
                ability.RemoveTag<AbilityAttrApplyRequest>();
                return;
            }

            ModifyHelper.RemoveModifiersFromSource(ability);
            ModifyHelper.AddModifierToUnit(owner.owner, contribution.attrTypeId, ability, contribution.modifyType, contribution.value);
            ability.RemoveTag<AbilityAttrApplyRequest>();
        });
    }
}

/// <summary>
/// 挂载技能属性移除系统。
/// 用于技能卸下或移除时撤销其带来的属性修改。
/// </summary>
[SystemRegister(SystemKind.Interval, 0)]
public class AbilityAttributeRemoveSystem : QuerySystem<AbilityOwner>
{
    public AbilityAttributeRemoveSystem()
    {
        Filter.AnyTags(Tags.Get<AbilityAttrRemoveRequest>());
    }

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref AbilityOwner owner, Entity ability) =>
        {
            ModifyHelper.RemoveModifiersFromSource(ability);
            ability.RemoveTag<AbilityAttrRemoveRequest>();
        });
    }
}

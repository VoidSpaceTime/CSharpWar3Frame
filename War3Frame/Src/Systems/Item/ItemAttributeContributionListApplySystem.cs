using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;
using War3Frame.Systems;

namespace War3Frame.Src.Systems;

/// <summary>
/// 物品多条属性贡献应用系统。
/// </summary>
[SystemRegister(SystemKind.Interval, 0)]
public class ItemAttributeContributionListApplySystem : QuerySystem<ItemOwner, ItemAttributeContributionListData, ItemAttrApplyRequest>
{
    // 多条贡献按同一物品 source 写入 modifier，保持与单条 AttributeContributionEntry 兼容。

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref ItemOwner owner, ref ItemAttributeContributionListData contributions, ref ItemAttrApplyRequest request, Entity item) =>
        {
            if (!item.Tags.Has<ItemEquippedTag>())
            {
                item.RemoveComponent<ItemAttrApplyRequest>();
                return;
            }

            if (owner.unit.IsNull)
            {
                item.RemoveComponent<ItemAttrApplyRequest>();
                return;
            }

            ModifyHelper.RemoveModifiersFromSource(item);
            foreach (var contribution in contributions.attributes)
            {
                ModifyHelper.AddModifierToUnit(owner.unit, contribution.attrTypeId, item, contribution.modifyType,
                    contribution.value.Resolve(1));
            }

            item.RemoveComponent<ItemAttrApplyRequest>();
        });
    }
}

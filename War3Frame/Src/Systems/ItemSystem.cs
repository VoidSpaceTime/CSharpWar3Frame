using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;
using War3Frame.Systems;

namespace War3Frame.Src.Systems;

/// <summary>
/// 物品挂载工作流系统。
/// </summary>
[SystemRegister(SystemKind.Immediate)]
public class ItemAttachWorkflowSystem : QuerySystem<ItemAttachRequest>
{
    // 物品挂载只更新归属、槽位和属性应用请求；实际属性修改由 ItemAttributeApplySystem 完成。

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref ItemAttachRequest request, Entity requestEntity) =>
        {
            AttachItem(request.owner, request.item, request.slotIndex);
            requestEntity.DeleteEntity();
        });
    }

    private static void AttachItem(Entity owner, Entity item, int slotIndex)
    {
        if (!owner.TryGetComponent<ItemSlotContainer>(out var container))
            throw new InvalidOperationException($"实体 {owner.Id} 没有 ItemSlotContainer 组件");

        if (item.IsNull || !item.TryGetComponent<ItemBase>(out _))
            throw new InvalidOperationException($"实体 {item.Id} 不是合法物品实体");

        if (slotIndex < 0 || slotIndex >= container.maxSlots)
            throw new InvalidOperationException($"槽位索引 {slotIndex} 超出范围 [0, {container.maxSlots})");

        if (IsSlotOccupied(owner, slotIndex))
            throw new InvalidOperationException($"物品槽位 {slotIndex} 已被占用");

        if (item.TryGetComponent<ItemOwner>(out var ownerInfo) && !ownerInfo.unit.IsNull)
            throw new InvalidOperationException($"物品 {item.Id} 已经归属到实体 {ownerInfo.unit.Id}");

        item.RemoveTag<ItemGroundTag>();
        item.RemoveTag<ItemStoredTag>();
        item.AddTag<ItemInventoryTag>();
        item.AddTag<ItemEquippedTag>();
        item.AddTag<ItemAttrApplyRequest>();
        item.RemoveTag<ItemAttrRemoveRequest>();
        item.AddComponent(new AttributeContributionSource
        {
            kind = War3Frame.Components.ModifierSourceType.Item
        });
        item.AddComponent(new ItemOwner(owner));
        item.AddComponent(new ItemSlotIndex { index = slotIndex });

        container.currentCount++;
        owner.AddComponent(container);
    }

    private static Entity? GetItemAtSlot(Entity owner, int slotIndex)
    {
        var links = owner.GetIncomingLinks<ItemOwner>();
        foreach (var link in links)
        {
            var itemEntity = link.Entity;
            if (itemEntity.TryGetComponent<ItemSlotIndex>(out var index) && index.index == slotIndex)
                return itemEntity;
        }

        return null;
    }

    private static bool IsSlotOccupied(Entity owner, int slotIndex)
    {
        return GetItemAtSlot(owner, slotIndex) != null;
    }
}

/// <summary>
/// 物品移除工作流系统。
/// </summary>
[SystemRegister(SystemKind.Immediate)]
public class ItemRemoveWorkflowSystem : QuerySystem<ItemRemoveRequest>
{
    // 物品移除只撤销归属/槽位并发出属性移除请求；属性层由后续系统统一清理。

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref ItemRemoveRequest request, Entity requestEntity) =>
        {
            RemoveItem(request.owner, request.slotIndex, request.dropToGround, request.x, request.y, request.z);
            requestEntity.DeleteEntity();
        });
    }

    private static void RemoveItem(Entity owner, int slotIndex, bool dropToGround, float x, float y, float z)
    {
        var item = GetItemAtSlot(owner, slotIndex);
        if (item == null) return;

        if (owner.TryGetComponent<ItemSlotContainer>(out var container))
        {
            container.currentCount = Math.Max(0, container.currentCount - 1);
            owner.AddComponent(container);
        }

        item.Value.RemoveTag<ItemEquippedTag>();
        item.Value.RemoveTag<ItemInventoryTag>();
        item.Value.RemoveTag<ItemStoredTag>();
        item.Value.AddTag<ItemAttrRemoveRequest>();
        item.Value.RemoveTag<ItemAttrApplyRequest>();
        item.Value.RemoveComponent<ItemOwner>();
        item.Value.RemoveComponent<ItemSlotIndex>();

        if (dropToGround)
        {
            item.Value.AddTag<ItemGroundTag>();
            item.Value.AddComponent(new Position { x = x, y = y, z = z });
        }
    }

    private static Entity? GetItemAtSlot(Entity owner, int slotIndex)
    {
        var links = owner.GetIncomingLinks<ItemOwner>();
        foreach (var link in links)
        {
            var itemEntity = link.Entity;
            if (itemEntity.TryGetComponent<ItemSlotIndex>(out var index) && index.index == slotIndex)
                return itemEntity;
        }

        return null;
    }
}

/// <summary>
/// 物品属性应用系统。
/// 仅在装备态下，将物品定义的统一属性贡献条目映射为单位属性修改器。
/// </summary>
[SystemRegister(SystemKind.Interval, 0)]
public class ItemAttributeApplySystem : QuerySystem<ItemOwner, AttributeContributionEntry>
{
    // 将装备物品的 AttributeContributionEntry 映射为单位属性 modifier。

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
    // 按物品实体作为 source 移除 modifier，避免误删其他来源的属性贡献。

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
    // 挂载型技能的属性贡献与物品走同一 modifier 层，保持数值来源可追踪。

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
    // 技能卸下或移除时，以 ability 实体作为 source 撤销其属性贡献。

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

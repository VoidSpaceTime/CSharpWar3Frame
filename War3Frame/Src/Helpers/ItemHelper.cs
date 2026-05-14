using Friflo.Engine.ECS;
using War3Frame.Components;

namespace War3Frame;

/// <summary>
/// 物品状态切换帮助类。
/// 仅负责写入状态与请求，不负责属性计算本身。
/// </summary>
public static class ItemHelper
{
    /// <summary>
    /// 将物品装备到单位身上。
    /// </summary>
    public static void EquipToUnit(Entity item, Entity unit, int slotIndex)
    {
        if (item.IsNull || unit.IsNull) return;

        // 直接实现逻辑
        if (!unit.TryGetComponent<ItemSlotContainer>(out var container))
            throw new InvalidOperationException($"实体 {unit.Id} 没有 ItemSlotContainer 组件");

        if (!item.TryGetComponent<ItemBase>(out _))
            throw new InvalidOperationException($"实体 {item.Id} 不是合法物品实体");

        if (slotIndex < 0 || slotIndex >= container.maxSlots)
            throw new InvalidOperationException($"槽位索引 {slotIndex} 超出范围 [0, {container.maxSlots})");

        if (IsItemSlotOccupied(unit, slotIndex))
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
            kind = Components.ModifierSourceType.Item
        });
        item.AddComponent(new ItemOwner(unit));
        item.AddComponent(new ItemSlotIndex { index = slotIndex });

        container.currentCount++;
        unit.AddComponent(container);
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

    private static bool IsItemSlotOccupied(Entity owner, int slotIndex)
    {
        return GetItemAtSlot(owner, slotIndex) != null;
    }

    /// <summary>
    /// 将物品从单位身上卸下，但仍保留在背包中。
    /// </summary>
    public static void UnequipToInventory(Entity item)
    {
        if (item.IsNull) return;

        item.RemoveTag<ItemEquippedTag>();
        item.AddTag<ItemInventoryTag>();
        item.AddTag<ItemAttrRemoveRequest>();
        item.RemoveTag<ItemAttrApplyRequest>();
    }

    /// <summary>
    /// 将物品丢到地上。
    /// </summary>
    public static void DropToGround(Entity item, float x, float y, float z = 0)
    {
        if (item.IsNull) return;

        if (!item.TryGetComponent<ItemOwner>(out var owner) || !item.TryGetComponent<ItemSlotIndex>(out var slotIndex))
        {
            item.RemoveTag<ItemEquippedTag>();
            item.RemoveTag<ItemInventoryTag>();
            item.RemoveTag<ItemStoredTag>();
            item.AddTag<ItemGroundTag>();
            item.AddTag<ItemAttrRemoveRequest>();
            item.RemoveTag<ItemAttrApplyRequest>();
            item.RemoveComponent<ItemOwner>();
            item.RemoveComponent<ItemSlotIndex>();
            item.AddComponent(new Position { x = x, y = y, z = z });
            return;
        }

        // 直接实现逻辑
        var itemEntity = GetItemAtSlot(owner.unit, slotIndex.index);
        if (itemEntity == null) return;

        if (owner.unit.TryGetComponent<ItemSlotContainer>(out var container))
        {
            container.currentCount = Math.Max(0, container.currentCount - 1);
            owner.unit.AddComponent(container);
        }

        itemEntity.Value.RemoveTag<ItemEquippedTag>();
        itemEntity.Value.RemoveTag<ItemInventoryTag>();
        itemEntity.Value.RemoveTag<ItemStoredTag>();
        itemEntity.Value.AddTag<ItemAttrRemoveRequest>();
        itemEntity.Value.RemoveTag<ItemAttrApplyRequest>();
        itemEntity.Value.RemoveComponent<ItemOwner>();
        itemEntity.Value.RemoveComponent<ItemSlotIndex>();
        itemEntity.Value.AddTag<ItemGroundTag>();
        itemEntity.Value.AddComponent(new Position { x = x, y = y, z = z });
    }
}

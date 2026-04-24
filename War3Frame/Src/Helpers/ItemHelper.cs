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

        unit.Store.CreateEntity(new ItemAttachRequest
        {
            owner = unit,
            item = item,
            slotIndex = slotIndex
        });
        Game.FlushImmediateSystems();
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

        item.Store.CreateEntity(new ItemRemoveRequest
        {
            owner = owner.unit,
            slotIndex = slotIndex.index,
            dropToGround = true,
            x = x,
            y = y,
            z = z
        });
        Game.FlushImmediateSystems();
    }
}

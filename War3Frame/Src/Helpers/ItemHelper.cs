using Friflo.Engine.ECS;
using War3Frame.Components;
using War3Frame.Components.Item;

namespace War3Frame;

/// <summary>
/// 物品状态切换帮助类。
/// 仅负责写入状态与请求，不负责属性计算本身。
/// </summary>
public static class ItemHelper
{
    private const float DefaultGroundPickRadius = 160f;

    /// <summary>创建带独立特效表现请求的地面物品实体。</summary>
    public static Entity CreateOnGround(ItemBase data, string model, float x, float y, float z = 0,
        float pickRadius = DefaultGroundPickRadius)
    {
        var item = Game.Store.CreateEntity(data);
        item.AddTag<ItemGroundTag>();
        item.AddComponent(new Position { x = x, y = y, z = z });
        item.AddComponent(new ItemGroundVisual { model = model });
        item.AddComponent(new ItemGroundPickRange { radius = pickRadius });
        item.AddComponent(new ItemCreateNativeRequest
        {
            item = item,
            x = x,
            y = y,
            z = z,
            model = model
        });
        return item;
    }

    /// <summary>创建只记录拾取意图的请求实体，实际校验由工作流完成。</summary>
    public static Entity RequestPickup(Entity user, Entity item, int slotIndex)
    {
        if (user.IsNull || item.IsNull || !ReferenceEquals(user.Store, item.Store))
            throw new InvalidOperationException("ItemPickup 的 user 与 item 必须位于同一个 EntityStore");

        return item.Store.CreateEntity(new ItemPickupRequest
        {
            user = user,
            item = item,
            slotIndex = slotIndex
        });
    }

    /// <summary>
    /// 创建无显式目标的物品使用请求；运行时会将目标规范化为使用者。
    /// </summary>
    public static Entity RequestUse(Entity user, Entity item)
    {
        return RequestUse(user, item, new ItemUseTarget { kind = AbilityTargetType.None });
    }

    /// <summary>
    /// 创建带显式目标意图的物品使用请求。
    /// </summary>
    public static Entity RequestUse(Entity user, Entity item, ItemUseTarget target)
    {
        if (user.IsNull || item.IsNull || !ReferenceEquals(user.Store, item.Store))
            throw new InvalidOperationException("ItemUse 的 user 与 item 必须位于同一个 EntityStore");
        if (!target.targetUnit.IsNull && !ReferenceEquals(item.Store, target.targetUnit.Store))
            throw new InvalidOperationException("ItemUse 的 targetUnit 必须与 item 位于同一个 EntityStore");

        return item.Store.CreateEntity(
            new ItemUseRequest
            {
                user = user,
                item = item
            },
            target);
    }

    /// <summary>
    /// 将物品装备到单位身上。
    /// </summary>
    public static void EquipToUnit(Entity item, Entity unit, int slotIndex)
    {
        if (item.IsNull || unit.IsNull) return;

        if (item.Tags.Has<ItemGroundTag>())
            DestroyGroundVisual(item);

        item.Store.CreateEntity(new ItemAttachRequest
        {
            owner = unit,
            item = item,
            slotIndex = slotIndex
        });
    }

    /// <summary>
    /// 将物品从单位身上卸下，但仍保留在背包中。
    /// </summary>
    public static void UnequipToInventory(Entity item)
    {
        if (item.IsNull) return;

        item.RemoveTag<ItemEquippedTag>();
        item.AddTag<ItemInventoryTag>();
        item.AddComponent(new ItemAttrRemoveRequest());
        item.RemoveComponent<ItemAttrApplyRequest>();
    }

    /// <summary>
    /// 将物品丢到地上。
    /// </summary>
    public static void DropToGround(Entity item, float x, float y, float z = 0)
    {
        if (item.IsNull) return;

        if (!item.TryGetComponent<ItemOwner>(out var owner)
            || owner.unit.IsNull
            || !item.TryGetComponent<ItemSlotIndex>(out var slotIndex))
            return;

        item.Store.CreateEntity(new ItemRemoveRequest
        {
            owner = owner.unit,
            slotIndex = slotIndex.index,
            dropToGround = true,
            x = x,
            y = y,
            z = z
        });
    }

    /// <summary>
    /// 请求通过受控流程销毁物品及其 companion ability。
    /// </summary>
    public static Entity RequestDestroy(Entity item)
    {
        if (item.IsNull)
            return default;

        return item.Store.CreateEntity(new ItemDestroyRequest { item = item });
    }

    /// <summary>清理地面视觉并取消尚未消费的创建请求。</summary>
    internal static void DestroyGroundVisual(Entity item)
    {
        if (item.IsNull)
            return;

        if (item.TryGetComponent<ItemGroundVisual>(out var visual))
        {
            if (!visual.effect.IsNull)
                EffectHelper.Destroy(visual.effect);

            visual.effect = default;
            item.AddComponent(visual);
        }

        item.RemoveComponent<ItemCreateNativeRequest>();
    }
}

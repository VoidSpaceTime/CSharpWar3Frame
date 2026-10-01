using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;
using War3Frame.Components.Item;
using War3Frame.Helpers;
using War3Frame.Src.Systems;
using War3Frame.Systems.Native;

namespace War3Frame.Regression;

/// <summary>验证地面物品的创建、拾取与视觉清理闭环。</summary>
internal static class ItemGroundRegression
{
    /// <summary>注册地面物品回归场景。</summary>
    internal static void Register(List<RegressionCase> tests)
    {
        tests.Add(new("item-ground", "创建", Create));
        tests.Add(new("item-ground", "拾取成功", PickupSuccess));
        tests.Add(new("item-ground", "同帧 Create+Pickup", SameFrameCreateAndPickup));
        tests.Add(new("item-ground", "超距失败", OutOfRange));
        tests.Add(new("item-ground", "无空槽失败", NoEmptySlot));
        tests.Add(new("item-ground", "地面直接 Attach", GroundDirectAttach));
    }

    private static void Create()
    {
        var item = ItemHelper.CreateOnGround(new ItemBase { name = "ground" }, "ground.mdl", 10, 20);
        var root = new SystemRoot(Game.Store);
        root.Add(new ItemCreateNativeSystem());
        root.Update(new UpdateTick(.01f, .01f));

        Check.That(item.Tags.Has<ItemGroundTag>(), "ground tag is set");
        Check.That(item.GetComponent<Position>().x == 10 && item.GetComponent<Position>().y == 20,
            "ground position is stored");
        Check.That(item.GetComponent<ItemGroundVisual>().model == "ground.mdl",
            "visual model is stored");
        Check.That(item.GetComponent<ItemGroundPickRange>().radius == 160,
            "default pick radius is 160");
        Check.That(!item.HasComponent<ItemCreateNativeRequest>(), "create request is consumed");
        Check.That(!item.GetComponent<ItemGroundVisual>().effect.IsNull,
            "native consumer writes an independent visual entity");
    }

    private static void PickupSuccess()
    {
        var store = new EntityStore();
        var owner = store.CreateEntity(new ItemSlotContainer { maxSlots = 1 }, new Position { x = 0, y = 0 });
        var item = GroundItem(store, 0, 0, withEffect: true);
        var root = PickupRoot(store);

        ItemHelper.RequestPickup(owner, item, 0);
        root.Update(new UpdateTick(.01f, .01f));

        Check.That(item.Tags.Has<ItemInventoryTag>() && item.Tags.Has<ItemEquippedTag>(),
            "pickup attaches item to inventory state");
        Check.That(!item.Tags.Has<ItemGroundTag>(), "pickup removes ground tag");
        Check.That(item.GetComponent<ItemGroundVisual>().effect.IsNull, "pickup clears visual reference");
        Check.That(item.HasComponent<ItemOwner>() && item.GetComponent<ItemSlotIndex>().index == 0,
            "pickup uses caller slot");
    }

    private static void SameFrameCreateAndPickup()
    {
        var store = new EntityStore();
        var owner = store.CreateEntity(new ItemSlotContainer { maxSlots = 1 }, new Position { x = 0, y = 0 });
        var item = GroundItem(store, 0, 0, withEffect: false);
        item.AddComponent(new ItemCreateNativeRequest
        {
            item = item,
            model = "ground.mdl",
            x = 0,
            y = 0,
            z = 0
        });
        var root = PickupRoot(store);

        ItemHelper.RequestPickup(owner, item, 0);
        root.Update(new UpdateTick(.01f, .01f));

        Check.That(item.Tags.Has<ItemInventoryTag>() && !item.Tags.Has<ItemGroundTag>(),
            "same-frame pickup attaches item");
        Check.That(!item.HasComponent<ItemCreateNativeRequest>(),
            "same-frame pickup removes unconsumed create request");
        Check.That(item.GetComponent<ItemGroundVisual>().effect.IsNull,
            "same-frame pickup has no orphan visual reference");
    }

    private static void OutOfRange()
    {
        var store = new EntityStore();
        var owner = store.CreateEntity(new ItemSlotContainer { maxSlots = 1 }, new Position { x = 0, y = 0 });
        var item = GroundItem(store, 161, 0, withEffect: true);
        var visual = item.GetComponent<ItemGroundVisual>();
        var root = PickupRoot(store);

        ItemHelper.RequestPickup(owner, item, 0);
        root.Update(new UpdateTick(.01f, .01f));

        Check.That(item.Tags.Has<ItemGroundTag>(), "out-of-range item remains on ground");
        Check.That(item.GetComponent<ItemGroundVisual>().effect == visual.effect,
            "out-of-range pickup preserves visual");
        Check.That(!item.HasComponent<ItemOwner>() && !item.HasComponent<ItemAttachRequest>(),
            "out-of-range pickup does not attach");
    }

    private static void NoEmptySlot()
    {
        var store = new EntityStore();
        var owner = store.CreateEntity(new ItemSlotContainer { maxSlots = 1 }, new Position { x = 0, y = 0 });
        store.CreateEntity(new ItemBase(), new ItemOwner(owner), new ItemSlotIndex { index = 0 });
        var item = GroundItem(store, 0, 0, withEffect: true);
        var visual = item.GetComponent<ItemGroundVisual>();
        var root = PickupRoot(store);

        ItemHelper.RequestPickup(owner, item, 0);
        root.Update(new UpdateTick(.01f, .01f));

        Check.That(item.Tags.Has<ItemGroundTag>(), "occupied slot rejects pickup");
        Check.That(item.GetComponent<ItemGroundVisual>().effect == visual.effect,
            "occupied slot preserves visual");
    }

    private static void GroundDirectAttach()
    {
        var store = new EntityStore();
        var owner = store.CreateEntity(new ItemSlotContainer { maxSlots = 1 });
        var item = GroundItem(store, 0, 0, withEffect: true);
        var visual = item.GetComponent<ItemGroundVisual>();
        var root = new SystemRoot(store);
        root.Add(new ItemAttachWorkflowSystem());

        ItemHelper.EquipToUnit(item, owner, 0);
        root.Update(new UpdateTick(.01f, .01f));

        Check.That(item.Tags.Has<ItemEquippedTag>() && !item.Tags.Has<ItemGroundTag>(),
            "direct attach enters equipped state");
        Check.That(item.GetComponent<ItemGroundVisual>().effect.IsNull,
            "direct attach clears visual reference");
        Check.That(visual.effect.HasComponent<EffectDestroyRequest>(),
            "direct attach sends visual destroy request");
    }

    private static Entity GroundItem(EntityStore store, float x, float y, bool withEffect)
    {
        var item = store.CreateEntity(new ItemBase { name = "ground" });
        item.AddTag<ItemGroundTag>();
        item.AddComponent(new Position { x = x, y = y });
        var effect = withEffect
            ? store.CreateEntity(new EffectBase(), new Position { x = x, y = y })
            : default;
        item.AddComponent(new ItemGroundVisual { model = "ground.mdl", effect = effect });
        item.AddComponent(new ItemGroundPickRange { radius = 160 });
        return item;
    }

    private static SystemRoot PickupRoot(EntityStore store)
    {
        var root = new SystemRoot(store);
        root.Add(new ItemPickupWorkflowSystem());
        root.Add(new ItemAttachWorkflowSystem());
        return root;
    }
}

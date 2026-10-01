using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;
using War3Frame.Helpers;
using War3Frame.Src.Systems;

namespace War3Frame.Regression;

/// <summary>验证物品消耗走受控销毁：请求延迟解除归属，并安全释放 companion ability。</summary>
internal static class ItemCostDestroyRegression
{
    /// <summary>注册物品消耗受控销毁回归场景。</summary>
    internal static void Register(List<RegressionCase> tests)
    {
        tests.Add(new("item-cost", "受控销毁请求", RequestCreated));
        tests.Add(new("item-cost", "伴随技能安全删除", CompanionSafeDelete));
    }

    /// <summary>消耗命中物品只产生销毁请求，由流程执行归属解除与实体删除。</summary>
    private static void RequestCreated()
    {
        var store = new EntityStore();
        var unit = store.CreateEntity(new ItemSlotContainer { maxSlots = 2, currentCount = 2 });
        var potionA = InventoryItem(store, unit, "potion", 0);
        var potionB = InventoryItem(store, unit, "potion", 1);
        var costAbility = store.CreateEntity(new ItemCost { templateName = "potion", count = 2 });
        var root = DestroyRoot(store);

        Check.That(AbilityCostHelper.CheckCost(unit, costAbility), "cost satisfied before depletion");
        AbilityCostHelper.ApplyCost(unit, costAbility);

        Check.That(store.Query<ItemDestroyRequest>().Count == 2,
            "one controlled destroy request is raised per consumed item");
        Check.That(CountItems(unit, "potion") == 2,
            "items stay owned until the destroy workflow runs");
        Check.That(unit.GetComponent<ItemSlotContainer>().currentCount == 2,
            "slot count is untouched before the workflow runs");

        root.Update(new UpdateTick(0f, 0f));

        Check.That(CountItems(unit, "potion") == 0, "workflow releases inventory ownership");
        Check.That(unit.GetComponent<ItemSlotContainer>().currentCount == 0,
            "workflow decrements the slot count once per consumed item");
        Check.That(potionA.IsNull && potionB.IsNull, "workflow deletes the consumed item entities");
        Check.That(store.Query<ItemDestroyRequest>().Count == 0, "consumed requests are removed");
    }

    /// <summary>带 companion 的消耗物品经受控流程释放 companion 后再删除，不遗留技能实体。</summary>
    private static void CompanionSafeDelete()
    {
        var store = new EntityStore();
        var unit = store.CreateEntity(new ItemSlotContainer { maxSlots = 1, currentCount = 0 });
        var item = store.CreateEntity();
        ItemSpecBuilder.Create("cost_companion_item")
            .Name("Cost Companion Item")
            .Stack(count: 1, max: 1)
            .Usable(consumable: true)
            .UseAbility(ability => ability
                .Name("Cost Companion Ability")
                .TargetType(AbilityTargetType.None)
                .CastPoint(0f)
                .Channel(0f, 0f)
                .Backswing(0f)
                .BaseValue(AbilityHelper.ManaCost, 0f)
                .BaseValue(AbilityHelper.CooldownDuration, 0f))
            .BuildTo(item);

        var root = new SystemRoot(store);
        root.Add(new ItemAttachWorkflowSystem());
        root.Add(new ItemDestroyRequestSystem());
        root.Add(new ItemCompanionDeferredDeleteSystem());

        ItemHelper.EquipToUnit(item, unit, 0);
        root.Update(new UpdateTick(0f, 0f));

        Check.That(item.TryGetComponent<ItemActiveAbility>(out var active) && !active.ability.IsNull,
            "item companion exists after attach");
        var companion = active.ability;

        var costAbility = store.CreateEntity(new ItemCost { templateName = "cost_companion_item", count = 1 });
        AbilityCostHelper.ApplyCost(unit, costAbility);
        Check.That(store.Query<ItemDestroyRequest>().Count == 1,
            "consume raises one controlled destroy request");
        Check.That(unit.GetComponent<ItemSlotContainer>().currentCount == 1,
            "equipped item is still counted before the workflow runs");

        root.Update(new UpdateTick(0f, 0f));

        Check.That(item.IsNull, "consumed item entity is deleted through the controlled path");
        Check.That(companion.IsNull, "companion ability is released with the consumed item");
        Check.That(unit.GetComponent<ItemSlotContainer>().currentCount == 0,
            "slot is released after companion-safe removal");
    }

    private static SystemRoot DestroyRoot(EntityStore store)
    {
        var root = new SystemRoot(store);
        root.Add(new ItemDestroyRequestSystem());
        root.Add(new ItemCompanionDeferredDeleteSystem());
        return root;
    }

    private static Entity InventoryItem(EntityStore store, Entity unit, string templateName, int slotIndex)
    {
        var item = store.CreateEntity(new ItemBase
        {
            templateName = templateName,
            name = templateName,
            stackCount = 1,
            maxStack = 1,
        });
        item.AddComponent(new ItemOwner(unit));
        item.AddComponent(new ItemSlotIndex { index = slotIndex });
        item.AddTag<ItemInventoryTag>();
        return item;
    }

    private static int CountItems(Entity unit, string templateName)
    {
        var count = 0;
        foreach (var link in unit.GetIncomingLinks<ItemOwner>())
        {
            var item = link.Entity;
            if (!item.Tags.Has<ItemInventoryTag>())
                continue;
            if (item.TryGetComponent<ItemBase>(out var itemBase) && itemBase.templateName == templateName)
                count++;
        }

        return count;
    }
}

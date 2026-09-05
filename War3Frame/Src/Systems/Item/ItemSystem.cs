using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;
using War3Frame.Helpers;
using War3Frame.Systems;

namespace War3Frame.Src.Systems;

/// <summary>
/// 物品挂载工作流系统。
/// </summary>
[SystemRegister(SystemKind.Immediate)]
public class ItemAttachWorkflowSystem : QuerySystem<ItemAttachRequest>
{
    // 物品挂载只更新归属、槽位和属性应用请求；实际属性修改由 ItemAttributeApplySystem 完成。
    private readonly List<(Entity entity, ItemAttachRequest request)> _pending = new();

    protected override void OnUpdate()
    {
        _pending.Clear();
        Query.ForEachEntity((ref ItemAttachRequest request, Entity requestEntity) =>
        {
            _pending.Add((requestEntity, request));
        });

        foreach (var pending in _pending)
        {
            try
            {
                if (!pending.request.owner.IsNull
                    && !pending.request.item.IsNull
                    && ReferenceEquals(pending.entity.Store, pending.request.owner.Store)
                    && ReferenceEquals(pending.entity.Store, pending.request.item.Store))
                {
                    ItemLifecycleOperations.Attach(pending.request.owner, pending.request.item, pending.request.slotIndex);
                }
            }
            finally
            {
                if (!pending.entity.IsNull)
                    pending.entity.DeleteEntity();
            }
        }
    }
}

/// <summary>
/// 物品移除工作流系统。
/// </summary>
[SystemRegister(SystemKind.Immediate)]
public class ItemRemoveWorkflowSystem : QuerySystem<ItemRemoveRequest>
{
    // 物品移除只撤销归属/槽位并发出属性移除请求；属性层由后续系统统一清理。
    private readonly List<(Entity entity, ItemRemoveRequest request)> _pending = new();

    protected override void OnUpdate()
    {
        _pending.Clear();
        Query.ForEachEntity((ref ItemRemoveRequest request, Entity requestEntity) =>
        {
            _pending.Add((requestEntity, request));
        });

        foreach (var pending in _pending)
        {
            try
            {
                if (!pending.request.owner.IsNull
                    && ReferenceEquals(pending.entity.Store, pending.request.owner.Store))
                {
                    ItemLifecycleOperations.Remove(
                        pending.request.owner,
                        pending.request.slotIndex,
                        pending.request.dropToGround,
                        pending.request.x,
                        pending.request.y,
                        pending.request.z);
                }
            }
            finally
            {
                if (!pending.entity.IsNull)
                    pending.entity.DeleteEntity();
            }
        }
    }
}

/// <summary>
/// 消费受控物品销毁请求，先进入 pending 并通过统一生命周期解除 owner。
/// </summary>
[SystemRegister(SystemKind.Immediate, 1)]
public sealed class ItemDestroyRequestSystem : QuerySystem<ItemDestroyRequest>
{
    private readonly List<(Entity entity, ItemDestroyRequest request)> _pending = new();

    protected override void OnUpdate()
    {
        _pending.Clear();
        Query.ForEachEntity((ref ItemDestroyRequest request, Entity requestEntity) =>
        {
            _pending.Add((requestEntity, request));
        });

        foreach (var pending in _pending)
        {
            try
            {
                if (!pending.request.item.IsNull
                    && ReferenceEquals(pending.entity.Store, pending.request.item.Store))
                    BeginDestroy(pending.request.item);
            }
            finally
            {
                if (!pending.entity.IsNull)
                    pending.entity.DeleteEntity();
            }
        }
    }

    private static void BeginDestroy(Entity item)
    {
        if (item.IsNull || !item.TryGetComponent<ItemBase>(out _))
            return;

        item.AddTag<ItemDestroyPendingTag>();
        ItemLifecycleOperations.Detach(item, false, 0f, 0f, 0f);
    }
}

/// <summary>
/// 等待 companion 的 Cast、Effect 和 GroundArea 引用释放后，按 companion 到 Item 的顺序删除。
/// </summary>
[SystemRegister(SystemKind.Interval, 131)]
public sealed class ItemCompanionDeferredDeleteSystem : QuerySystem<ItemBase>
{
    private readonly List<(Entity item, Entity companion)> _pending = new();

    public ItemCompanionDeferredDeleteSystem()
    {
        Filter.AnyTags(Tags.Get<ItemDestroyPendingTag>());
    }

    protected override void OnUpdate()
    {
        _pending.Clear();
        Query.ForEachEntity((ref ItemBase itemBase, Entity item) =>
        {
            var companion = item.TryGetComponent<ItemActiveAbility>(out var active)
                ? active.ability
                : default;
            _pending.Add((item, companion));
        });

        if (_pending.Count == 0)
            return;

        var referenced = ItemCompanionCastCleanup.CollectReferencedAbilityIds(_pending[0].item.Store);
        foreach (var entry in _pending)
        {
            if (!entry.companion.IsNull && referenced.Contains(entry.companion.Id))
                continue;

            if (!entry.companion.IsNull)
            {
                if (ItemCompanionAbilityHelper.IsOwnedCompanion(entry.item, entry.companion))
                    AbilityHelper.RemoveAbility(entry.companion);
            }
            ModifyHelper.RemoveModifiersFromSource(entry.item);
            entry.item.DeleteEntity();
        }
    }
}

/// <summary>
/// Item attach/remove 工作流共享的权威状态变更操作。
/// </summary>
internal static class ItemLifecycleOperations
{
    /// <summary>
    /// 将物品挂到目标槽位；跨 owner 时先按 companion identity 清理旧施法。
    /// </summary>
    public static void Attach(Entity owner, Entity item, int slotIndex)
    {
        if (owner.IsNull || item.IsNull || !item.TryGetComponent<ItemBase>(out _))
            throw new InvalidOperationException("Item attach 请求包含无效实体");
        if (!ReferenceEquals(owner.Store, item.Store))
            throw new InvalidOperationException("Item 与 owner 必须位于同一个 EntityStore");
        if (item.Tags.Has<ItemDestroyPendingTag>())
            return;
        if (!owner.TryGetComponent<ItemSlotContainer>(out var container))
            throw new InvalidOperationException($"实体 {owner.Id} 没有 ItemSlotContainer 组件");
        if (slotIndex < 0 || slotIndex >= container.maxSlots)
            throw new InvalidOperationException($"槽位索引 {slotIndex} 超出范围 [0, {container.maxSlots})");

        var occupied = GetItemAtSlot(owner, slotIndex);
        if (occupied.HasValue && occupied.Value != item)
            throw new InvalidOperationException($"物品槽位 {slotIndex} 已被占用");

        if (item.TryGetComponent<ItemOwner>(out var currentOwner)
            && item.TryGetComponent<ItemSlotIndex>(out var currentSlot))
        {
            if (currentOwner.unit == owner && currentSlot.index == slotIndex)
            {
                ItemCompanionAbilityHelper.TryEnsureCompanion(item, out _);
                return;
            }

            Detach(item, false, 0f, 0f, 0f);
            container = owner.GetComponent<ItemSlotContainer>();
        }

        item.RemoveTag<ItemGroundTag>();
        item.RemoveTag<ItemStoredTag>();
        item.AddTag<ItemInventoryTag>();
        item.AddTag<ItemEquippedTag>();
        item.AddComponent(new ItemAttrApplyRequest());
        item.RemoveComponent<ItemAttrRemoveRequest>();
        item.AddComponent(new AttributeContributionSource
        {
            kind = War3Frame.Components.ModifierSourceType.Item
        });
        item.AddComponent(new ItemOwner(owner));
        item.AddComponent(new ItemSlotIndex { index = slotIndex });

        container.currentCount++;
        owner.AddComponent(container);
        ItemCompanionAbilityHelper.TryEnsureCompanion(item, out _);
    }

    /// <summary>
    /// 从指定 owner 槽位移除物品，并按请求决定是否落地。
    /// </summary>
    public static void Remove(Entity owner, int slotIndex, bool dropToGround, float x, float y, float z)
    {
        var item = GetItemAtSlot(owner, slotIndex);
        if (item.HasValue)
            Detach(item.Value, dropToGround, x, y, z);
    }

    /// <summary>
    /// 解除物品 owner 和槽位；companion 在 owner Link 移除前完成阶段清理。
    /// </summary>
    public static void Detach(Entity item, bool dropToGround, float x, float y, float z)
    {
        ItemCompanionAbilityHelper.UnbindOwner(item);
        ModifyHelper.RemoveModifiersFromSource(item);

        if (item.TryGetComponent<ItemOwner>(out var owner)
            && !owner.unit.IsNull
            && owner.unit.TryGetComponent<ItemSlotContainer>(out var container))
        {
            container.currentCount = Math.Max(0, container.currentCount - 1);
            owner.unit.AddComponent(container);
        }

        item.RemoveTag<ItemEquippedTag>();
        item.RemoveTag<ItemInventoryTag>();
        item.RemoveTag<ItemStoredTag>();
        item.RemoveComponent<ItemAttrRemoveRequest>();
        item.RemoveComponent<ItemAttrApplyRequest>();
        item.RemoveComponent<ItemOwner>();
        item.RemoveComponent<ItemSlotIndex>();

        if (dropToGround)
        {
            item.AddTag<ItemGroundTag>();
            item.AddComponent(new Position { x = x, y = y, z = z });
        }
        else
        {
            item.RemoveTag<ItemGroundTag>();
        }
    }

    private static Entity? GetItemAtSlot(Entity owner, int slotIndex)
    {
        if (owner.IsNull)
            return null;

        foreach (var link in owner.GetIncomingLinks<ItemOwner>())
        {
            var item = link.Entity;
            if (item.TryGetComponent<ItemSlotIndex>(out var index) && index.index == slotIndex)
                return item;
        }
        return null;
    }
}

/// <summary>
/// 物品属性贡献统一应用系统。
/// 仅在装备态下，将物品定义的单条/多条属性贡献映射为单位属性修改器。
/// 统一消费 ItemAttrApplyRequest，按物品携带的载荷（AttributeContributionEntry 或
/// ItemAttributeContributionListData）分支应用；两分支都完成才移除请求，
/// 避免旧双系统“先到者 Remove 请求、后到者静默跳过”的竞态。
/// </summary>
[SystemRegister(SystemKind.Interval, 0)]
public class ItemAttributeContributionApplySystem : QuerySystem<ItemOwner, ItemAttrApplyRequest>
{
    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref ItemOwner owner, ref ItemAttrApplyRequest request, Entity item) =>
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

            if (item.TryGetComponent<ItemAttributeContributionListData>(out var contributions))
            {
                foreach (var contribution in contributions.attributes)
                {
                    ModifyHelper.AddModifierToUnit(owner.unit, contribution.attrTypeId, item, contribution.modifyType,
                        contribution.value.Resolve(1));
                }
            }
            else if (item.TryGetComponent<AttributeContributionEntry>(out var contribution))
            {
                ModifyHelper.AddModifierToUnit(owner.unit, contribution.attrTypeId, item, contribution.modifyType, contribution.value);
            }

            item.RemoveComponent<ItemAttrApplyRequest>();
        });
    }
}

/// <summary>
/// 物品属性移除系统。
/// 用于卸下、丢弃或销毁物品时撤销其带来的属性修改。
/// </summary>
[SystemRegister(SystemKind.Interval, 0)]
public class ItemAttributeRemoveSystem : QuerySystem<ItemOwner, ItemAttrRemoveRequest>
{
    // 按物品实体作为 source 移除 modifier，避免误删其他来源的属性贡献。

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref ItemOwner owner, ref ItemAttrRemoveRequest request, Entity item) =>
        {
            ModifyHelper.RemoveModifiersFromSource(item);
            item.RemoveComponent<ItemAttrRemoveRequest>();
        });
    }
}

/// <summary>
/// 挂载技能属性应用系统。
/// 将 ability 的统一贡献条目映射为所属单位的属性修改器。
/// </summary>
[SystemRegister(SystemKind.Interval, 0)]
public class AbilityAttributeApplySystem : QuerySystem<AbilityOwner, AttributeContributionEntry, AbilityAttrApplyRequest>
{
    // 挂载型技能的属性贡献与物品走同一 modifier 层，保持数值来源可追踪。

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref AbilityOwner owner, ref AttributeContributionEntry contribution, ref AbilityAttrApplyRequest request, Entity ability) =>
        {
            if (owner.owner.IsNull)
            {
                ability.RemoveComponent<AbilityAttrApplyRequest>();
                return;
            }

            ModifyHelper.RemoveModifiersFromSource(ability);
            ModifyHelper.AddModifierToUnit(owner.owner, contribution.attrTypeId, ability, contribution.modifyType, contribution.value);
            ability.RemoveComponent<AbilityAttrApplyRequest>();
        });
    }
}

/// <summary>
/// 挂载技能属性移除系统。
/// 用于技能卸下或移除时撤销其带来的属性修改。
/// </summary>
[SystemRegister(SystemKind.Interval, 0)]
public class AbilityAttributeRemoveSystem : QuerySystem<AbilityOwner, AbilityAttrRemoveRequest>
{
    // 技能卸下或移除时，以 ability 实体作为 source 撤销其属性贡献。

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref AbilityOwner owner, ref AbilityAttrRemoveRequest request, Entity ability) =>
        {
            ModifyHelper.RemoveModifiersFromSource(ability);
            ability.RemoveComponent<AbilityAttrRemoveRequest>();
        });
    }
}

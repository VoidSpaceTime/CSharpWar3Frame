using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;

namespace War3Frame.Systems;

/// <summary>
///     技能槽系统 - 管理技能槽相关逻辑
/// </summary>
public class AbilitySlotSystem : QuerySystem<AbilitySlotContainer>
{
    protected override void OnUpdate()
    {
        // 技能槽容器本身不需要每帧更新
        // 这个系统可以用于处理槽位相关的事件或同步
    }
}

/// <summary>
/// 技能挂载工作流系统。
/// </summary>
[SystemRegister(SystemKind.Immediate)]
public class AbilityAttachWorkflowSystem : QuerySystem<AbilityAttachRequest>
{
    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref AbilityAttachRequest request, Entity requestEntity) =>
        {
            AttachAbility(request.unit, request.ability, request.slotIndex);
            requestEntity.DeleteEntity();
        });
    }

    private static void AttachAbility(Entity unit, Entity ability, int slotIndex)
    {
        if (!unit.TryGetComponent<AbilitySlotContainer>(out var container))
            throw new InvalidOperationException($"单位 {unit.Id} 没有 AbilitySlotContainer 组件");

        if (ability.IsNull || !ability.TryGetComponent<AbilityBase>(out _))
            throw new InvalidOperationException($"实体 {ability.Id} 不是合法技能实体");

        if (slotIndex < 0 || slotIndex >= container.maxSlots)
            throw new InvalidOperationException($"槽位索引 {slotIndex} 超出范围 [0, {container.maxSlots})");

        if (AbilitySlotHelper.IsSlotOccupied(unit, slotIndex))
            throw new InvalidOperationException($"槽位 {slotIndex} 已被占用");

        if (ability.TryGetComponent<AbilityOwner>(out var owner) && !owner.owner.IsNull)
            throw new InvalidOperationException($"技能 {ability.Id} 已经装配到单位 {owner.owner.Id}");

        ability.AddComponent(new AbilitySlotIndex { slotIndex = slotIndex });
        ability.AddComponent(new AbilityOwner(unit));
        ability.AddComponent(new AbilityMountInfo
        {
            mountType = AbilityMountType.Slot
        });

        if (ability.TryGetComponent<AttributeContributionEntry>(out _))
        {
            ability.AddComponent(new AttributeContributionSource
            {
                kind = War3Frame.Components.ModifierSourceType.Ability
            });
            ability.AddTag<AbilityAttrApplyRequest>();
            ability.RemoveTag<AbilityAttrRemoveRequest>();
        }

        container.currentCount++;
        unit.AddComponent(container);
    }
}

/// <summary>
/// 技能移除工作流系统。
/// </summary>
[SystemRegister(SystemKind.Immediate)]
public class AbilityRemoveWorkflowSystem : QuerySystem<AbilityRemoveRequest>
{
    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref AbilityRemoveRequest request, Entity requestEntity) =>
        {
            RemoveAbility(request.unit, request.slotIndex, request.destroyAbility);
            requestEntity.DeleteEntity();
        });
    }

    private static void RemoveAbility(Entity unit, int slotIndex, bool destroyAbility)
    {
        var ability = AbilitySlotHelper.GetAbilityAtSlot(unit, slotIndex);
        if (ability == null) return;

        if (unit.TryGetComponent<AbilitySlotContainer>(out var container))
        {
            container.currentCount = Math.Max(0, container.currentCount - 1);
            unit.AddComponent(container);
        }

        ability.Value.AddTag<AbilityAttrRemoveRequest>();
        ability.Value.RemoveComponent<AbilityOwner>();
        ability.Value.RemoveComponent<AbilitySlotIndex>();
        ability.Value.AddComponent(new AbilityMountInfo
        {
            mountType = AbilityMountType.NonSlot
        });

        if (destroyAbility)
        {
            Helpers.AbilityHelper.RemoveAbility(ability.Value);
        }
    }
}

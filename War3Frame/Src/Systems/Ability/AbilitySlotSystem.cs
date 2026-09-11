using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;
using War3Frame.Helpers;

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
    // Friflo 约束：Query 迭代内禁止结构变更，先收集请求，循环外挂载。
    private readonly List<(Entity requestEntity, AbilityAttachRequest request)> _pending = new();

    protected override void OnUpdate()
    {
        _pending.Clear();
        Query.ForEachEntity((ref AbilityAttachRequest request, Entity requestEntity) =>
        {
            _pending.Add((requestEntity, request));
        });

        foreach (var (requestEntity, request) in _pending)
        {
            try
            {
                if (!request.unit.IsNull && !request.ability.IsNull)
                    AttachAbility(request.unit, request.ability, request.slotIndex);
            }
            finally
            {
                if (!requestEntity.IsNull)
                    requestEntity.DeleteEntity();
            }
        }
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
            ability.AddComponent(new AbilityAttrApplyRequest());
            ability.RemoveComponent<AbilityAttrRemoveRequest>();
        }

        AbilityEffectHelper.TriggerBehaviorEffect(unit, ability, AbilityBehaviorTrigger.OnGranted, unit);

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
    // Friflo 约束：Query 迭代内禁止结构变更，先收集请求，循环外移除。
    private readonly List<(Entity requestEntity, AbilityRemoveRequest request)> _pending = new();

    protected override void OnUpdate()
    {
        _pending.Clear();
        Query.ForEachEntity((ref AbilityRemoveRequest request, Entity requestEntity) =>
        {
            _pending.Add((requestEntity, request));
        });

        foreach (var (requestEntity, request) in _pending)
        {
            try
            {
                if (!request.unit.IsNull)
                    RemoveAbility(request.unit, request.slotIndex, request.destroyAbility);
            }
            finally
            {
                if (!requestEntity.IsNull)
                    requestEntity.DeleteEntity();
            }
        }
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

        AbilityEffectHelper.TriggerBehaviorEffect(unit, ability.Value, AbilityBehaviorTrigger.OnRemoved, unit);

        ability.Value.AddComponent(new AbilityAttrRemoveRequest());
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

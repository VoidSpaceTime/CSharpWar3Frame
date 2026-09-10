using System.Collections.Generic;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Src.Components;

namespace War3Frame.Systems;

/// <summary>
///     技能加点工作流系统。
///     消费 AbilityUpgradeRequest：校验归属/槽位/上限/点数后，扣点、提升技能等级、打 LevelStatDirty 并发 AbilityUpgradedEvent。
///     不重算等级数值（由 AbilityLevelStatRebuildSystem 消费 LevelStatDirty 处理），不调用 War3 原生。
/// </summary>
[SystemRegister(SystemKind.Immediate)]
public class AbilityUpgradeWorkflowSystem : QuerySystem<AbilityUpgradeRequest>
{
    // Friflo 约束：Query 迭代内禁止结构变更，先收集请求快照，循环外统一处理。
    private readonly List<(AbilityUpgradeRequest request, Entity requestEntity)> _pending = new();

    protected override void OnUpdate()
    {
        _pending.Clear();
        Query.ForEachEntity((ref AbilityUpgradeRequest request, Entity requestEntity) =>
        {
            _pending.Add((request, requestEntity));
        });

        foreach (var (request, requestEntity) in _pending)
        {
            TryUpgrade(request);
            if (!requestEntity.IsNull)
                requestEntity.DeleteEntity();
        }
    }

    /// <summary>校验并执行一次加点；任何校验失败都保持技能等级与点数不变。</summary>
    private static void TryUpgrade(in AbilityUpgradeRequest request)
    {
        if (request.unit.IsNull || request.ability.IsNull || request.levels <= 0)
            return;

        if (!request.ability.TryGetComponent<AbilityBase>(out var abilityBase))
            return;

        // 仅允许单位自己槽位内装配的技能。
        if (!request.ability.TryGetComponent<AbilityOwner>(out var owner) || owner.owner != request.unit)
            return;

        if (!request.ability.HasComponent<AbilitySlotIndex>()
            || !request.ability.TryGetComponent<AbilityMountInfo>(out var mount)
            || mount.mountType != AbilityMountType.Slot)
        {
            return;
        }

        // 加点上限：spec.maxLevel <= 0 表示该技能不可加点。
        if (!request.ability.TryGetComponent<AbilitySpecData>(out var specData)
            || specData.spec.maxLevel <= 0)
        {
            return;
        }

        var nextLevel = abilityBase.level + request.levels;
        if (nextLevel > specData.spec.maxLevel)
            return;

        // 扣点（含点池不足校验）。
        if (!SkillPointHelper.SpendPoints(request.unit, request.levels))
            return;

        var fromLevel = abilityBase.level;
        abilityBase.level = nextLevel;
        request.ability.AddComponent(abilityBase);
        request.ability.AddTag<LevelStatDirty>();

        var upgradeEvent = request.unit.Store.CreateEntity(new AbilityUpgradedEvent
        {
            unit = request.unit,
            ability = request.ability,
            templateName = abilityBase.templateName,
            fromLevel = fromLevel,
            toLevel = nextLevel,
            pointsSpent = request.levels
        });
        upgradeEvent.AddComponent(new TriggerEventMarker
        {
            eventTypeId = EventTypeRegistry.Get<AbilityUpgradedEvent>()
        });
    }
}

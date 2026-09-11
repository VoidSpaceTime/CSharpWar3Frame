using System.Collections.Generic;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;
using War3Frame.Helpers;
using War3Frame.Src.Components;

namespace War3Frame.Systems;

/// <summary>
/// 单位等级基础数值重算系统，只把模板中的 LevelValue 解析为当前等级基础属性。
/// </summary>
[SystemRegister(SystemKind.Interval, 0)]
public class UnitLevelStatRebuildSystem : QuerySystem<UnitSpecData, UnitLevel>
{
    public UnitLevelStatRebuildSystem()
    {
        Filter.AnyTags(Tags.Get<LevelStatDirty>());
    }

    protected override void OnUpdate()
    {
        // 重算涉及创建属性实体/打脏（结构变更）：先收集单位，循环外重算。
        var pending = new List<Entity>();
        Query.ForEachEntity((ref UnitSpecData specData, ref UnitLevel level, Entity unit) =>
        {
            pending.Add(unit);
        });

        foreach (var unit in pending)
        {
            if (unit.IsNull)
                continue;
            if (!unit.TryGetComponent<UnitSpecData>(out var specData) || !unit.TryGetComponent<UnitLevel>(out var level))
                continue;

            foreach (var attribute in specData.spec.attributes)
            {
                if (!AttributeHelper.TryGetAttr(unit, attribute.attrTypeId, out var attr))
                {
                    AttributeHelper.CreateAttr(unit, attribute.attrTypeId, attribute.baseValue.Resolve(level.level));
                    continue;
                }

                var value = attr.GetComponent<AttrValue>();
                value.baseValue = attribute.baseValue.Resolve(level.level);
                attr.AddComponent(value);
                attr.AddTag<AttrDirty>();
            }

            unit.RemoveTag<LevelStatDirty>();
        }
    }
}

/// <summary>
/// 物品等级属性贡献重算系统，只刷新物品自身的等级贡献值。
/// </summary>
[SystemRegister(SystemKind.Interval, 0)]
public class ItemLevelStatRebuildSystem : QuerySystem<ItemSpecData, ItemLevel>
{
    public ItemLevelStatRebuildSystem()
    {
        Filter.AnyTags(Tags.Get<LevelStatDirty>());
    }

    protected override void OnUpdate()
    {
        // 刷新物品等级贡献涉及 AddComponent/RemoveTag（结构变更）：先收集物品，循环外处理。
        var pending = new List<Entity>();
        Query.ForEachEntity((ref ItemSpecData specData, ref ItemLevel level, Entity item) =>
        {
            pending.Add(item);
        });

        foreach (var item in pending)
        {
            if (item.IsNull)
                continue;
            if (!item.TryGetComponent<ItemSpecData>(out var specData) || !item.TryGetComponent<ItemLevel>(out var level))
                continue;

            if (!ItemCompanionAbilityHelper.SynchronizeLevel(item))
                continue;

            ApplyItemAttributes(item, specData.spec, level.level);
            item.RemoveTag<LevelStatDirty>();
        }
    }

    private static void ApplyItemAttributes(Entity item, ItemSpec spec, int level)
    {
        if (spec.attributes.Count == 0)
            return;

        if (spec.attributes.Count == 1)
        {
            var contribution = spec.attributes[0];
            item.AddComponent(new AttributeContributionEntry
            {
                attrTypeId = contribution.attrTypeId,
                modifyType = contribution.modifyType,
                value = contribution.value.Resolve(level),
                priority = contribution.priority
            });
        }
        else
        {
            item.AddComponent(new ItemAttributeContributionListData
            {
                attributes = ResolveItemAttributes(spec.attributes, level)
            });
        }

        item.AddComponent(new ItemAttrApplyRequest());
    }

    private static List<ItemAttributeContributionSpec> ResolveItemAttributes(List<ItemAttributeContributionSpec> attributes,
        int level)
    {
        var resolved = new List<ItemAttributeContributionSpec>(attributes.Count);
        foreach (var attribute in attributes)
        {
            resolved.Add(new ItemAttributeContributionSpec(attribute.attrTypeId, attribute.modifyType,
                LevelValue.Fixed(attribute.value.Resolve(level)), attribute.priority));
        }

        return resolved;
    }
}

/// <summary>
/// 技能等级基础数值重算系统，只刷新技能基础数值。
/// </summary>
[SystemRegister(SystemKind.Interval, 0)]
public class AbilityLevelStatRebuildSystem : QuerySystem<AbilitySpecData, AbilityBase>
{
    public AbilityLevelStatRebuildSystem()
    {
        Filter.AnyTags(Tags.Get<LevelStatDirty>());
    }

    // Friflo 约束：Query 迭代内禁止结构变更，先收集快照，循环外统一重算。
    private readonly List<(AbilitySpecData specData, AbilityBase abilityBase, Entity ability)> _pending = new();

    protected override void OnUpdate()
    {
        _pending.Clear();
        Query.ForEachEntity((ref AbilitySpecData specData, ref AbilityBase abilityBase, Entity ability) =>
        {
            _pending.Add((specData, abilityBase, ability));
        });

        foreach (var (specData, abilityBase, ability) in _pending)
        {
            foreach (var (statId, value) in specData.spec.baseValues)
            {
                AbilityHelper.SetBaseValue(ability, statId, value.Resolve(abilityBase.level));
            }

            ability.RemoveTag<LevelStatDirty>();
        }
    }
}

/// <summary>
/// 经验系统，消费经验获得请求并在升级后添加 LevelStatDirty。
/// Unit 升级时同步发放技能点并广播 UnitLeveledEvent。
/// </summary>
[SystemRegister(SystemKind.Interval, 0)]
public class ExperienceSystem : QuerySystem<ExperienceGainRequest>
{
    // Friflo 约束：Query 迭代内禁止结构变更，先收集请求快照，循环外统一结算与删除。
    private readonly List<(ExperienceGainRequest request, Entity requestEntity)> _pending = new();

    protected override void OnUpdate()
    {
        _pending.Clear();
        Query.ForEachEntity((ref ExperienceGainRequest request, Entity requestEntity) =>
        {
            _pending.Add((request, requestEntity));
        });

        foreach (var (request, requestEntity) in _pending)
        {
            ApplyExperience(request.target, request.amount * request.multiplier);
            if (!requestEntity.IsNull)
                requestEntity.DeleteEntity();
        }
    }

    private static void ApplyExperience(Entity target, float amount)
    {
        if (target.IsNull || amount <= 0f || !target.TryGetComponent<ExperienceData>(out var experience))
            return;

        experience.currentExp += amount;
        experience.totalExp += amount;
        var before = TryGetLevel(target, out var beforeLevel) ? beforeLevel : 0;
        var leveled = TryLevelUp(target, ref experience);
        target.AddComponent(experience);

        if (!leveled)
            return;

        target.AddTag<LevelStatDirty>();

        // 仅 Unit 升级：同步发点 + 对外广播一条升级事件（Item/Ability 不发）。
        if (target.HasComponent<UnitLevel>() && TryGetLevel(target, out var afterLevel) && afterLevel > before)
        {
            SkillPointHelper.GrantLevels(target, afterLevel - before);

            var leveledEvent = target.Store.CreateEntity(new UnitLeveledEvent
            {
                unit = target,
                fromLevel = before,
                toLevel = afterLevel
            });
            leveledEvent.AddComponent(new TriggerEventMarker
            {
                eventTypeId = EventTypeRegistry.Get<UnitLeveledEvent>()
            });
        }
    }

    private static bool TryLevelUp(Entity target, ref ExperienceData experience)
    {
        var leveled = false;
        while (TryGetLevel(target, out var level) && CanLevelUp(target, experience, level))
        {
            var required = experience.curve.RequiredForNextLevel(level);
            if (required <= 0f || experience.currentExp < required)
                break;

            experience.currentExp -= required;
            SetLevel(target, level + 1);
            leveled = true;
        }

        return leveled;
    }

    private static bool CanLevelUp(Entity target, ExperienceData experience, int level)
    {
        if (experience.maxLevel > 0 && level >= experience.maxLevel)
            return false;

        // 加点成长技能（AbilitySpec.maxLevel > 0）由技能点驱动，经验系统不做熟练度自动升级。
        if (target.HasComponent<AbilityBase>()
            && target.TryGetComponent<AbilitySpecData>(out var specData)
            && specData.spec.maxLevel > 0)
        {
            return false;
        }

        return true;
    }

    private static bool TryGetLevel(Entity target, out int level)
    {
        if (target.TryGetComponent<UnitLevel>(out var unitLevel))
        {
            level = Math.Max(unitLevel.level, 1);
            return true;
        }

        if (target.TryGetComponent<ItemLevel>(out var itemLevel))
        {
            level = Math.Max(itemLevel.level, 1);
            return true;
        }

        if (target.TryGetComponent<AbilityBase>(out var abilityBase))
        {
            level = Math.Max(abilityBase.level, 1);
            return true;
        }

        level = 0;
        return false;
    }

    private static void SetLevel(Entity target, int level)
    {
        if (target.TryGetComponent<UnitLevel>(out var unitLevel))
        {
            unitLevel.level = level;
            target.AddComponent(unitLevel);
            return;
        }

        if (target.TryGetComponent<ItemLevel>(out var itemLevel))
        {
            itemLevel.level = level;
            target.AddComponent(itemLevel);
            return;
        }

        if (target.TryGetComponent<AbilityBase>(out var abilityBase))
        {
            abilityBase.level = level;
            target.AddComponent(abilityBase);
        }
    }
}

using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;
using War3Frame.Helpers;

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
        Query.ForEachEntity((ref UnitSpecData specData, ref UnitLevel level, Entity unit) =>
        {
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
        });
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
        Query.ForEachEntity((ref ItemSpecData specData, ref ItemLevel level, Entity item) =>
        {
            if (!ItemCompanionAbilityHelper.SynchronizeLevel(item))
                return;

            ApplyItemAttributes(item, specData.spec, level.level);
            item.RemoveTag<LevelStatDirty>();
        });
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

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref AbilitySpecData specData, ref AbilityBase abilityBase, Entity ability) =>
        {
            foreach (var (statId, value) in specData.spec.baseValues)
            {
                AbilityHelper.SetBaseValue(ability, statId, value.Resolve(abilityBase.level));
            }

            ability.RemoveTag<LevelStatDirty>();
        });
    }
}

/// <summary>
/// 经验系统，消费经验获得请求并在升级后添加 LevelStatDirty。
/// </summary>
[SystemRegister(SystemKind.Interval, 0)]
public class ExperienceSystem : QuerySystem<ExperienceGainRequest>
{
    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref ExperienceGainRequest request, Entity requestEntity) =>
        {
            ApplyExperience(request.target, request.amount * request.multiplier);
            requestEntity.DeleteEntity();
        });
    }

    private static void ApplyExperience(Entity target, float amount)
    {
        if (target.IsNull || amount <= 0f || !target.TryGetComponent<ExperienceData>(out var experience))
            return;

        experience.currentExp += amount;
        experience.totalExp += amount;
        var leveled = TryLevelUp(target, ref experience);
        target.AddComponent(experience);

        if (leveled)
            target.AddTag<LevelStatDirty>();
    }

    private static bool TryLevelUp(Entity target, ref ExperienceData experience)
    {
        var leveled = false;
        while (TryGetLevel(target, out var level) && CanLevelUp(experience, level))
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

    private static bool CanLevelUp(ExperienceData experience, int level)
    {
        return experience.maxLevel <= 0 || level < experience.maxLevel;
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

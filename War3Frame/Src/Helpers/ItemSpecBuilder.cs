using Friflo.Engine.ECS;
using War3Frame.Components;

namespace War3Frame.Helpers;

/// <summary>
/// 物品模板 Builder，负责把物品静态配置写入现有模板实体。
/// </summary>
public sealed class ItemSpecBuilder
{
    private readonly ItemSpec _spec = new();

    private ItemSpecBuilder(string templateName)
    {
        _spec.templateName = templateName;
        _spec.name = templateName;
    }

    public static ItemSpecBuilder Create(string templateName)
    {
        return new ItemSpecBuilder(templateName);
    }

    public ItemSpecBuilder Name(string name)
    {
        _spec.name = name;
        return this;
    }

    public ItemSpecBuilder Stack(int count = 1, int max = 1)
    {
        _spec.stackCount = count;
        _spec.maxStack = max;
        return this;
    }

    public ItemSpecBuilder Usable(bool consumable)
    {
        _spec.isUsable = true;
        _spec.isConsumable = consumable;
        return this;
    }

    public ItemSpecBuilder Instantiate(bool enabled = true)
    {
        _spec.isInstantiate = enabled;
        return this;
    }

    public ItemSpecBuilder Attr(int attrTypeId, ModifyType modifyType, float value, int priority = 0)
    {
        _spec.attributes.Add(new ItemAttributeContributionSpec(attrTypeId, modifyType, value, priority));
        return this;
    }

    public ItemSpecBuilder UseAbility(string abilityTemplateName)
    {
        _spec.useAbilityTemplateName = abilityTemplateName;
        _spec.isUsable = true;
        return this;
    }

    public ItemSpecBuilder UseEffect(AbilityEffectSpec effectSpec)
    {
        _spec.useEffectSpec = effectSpec;
        _spec.isUsable = true;
        return this;
    }

    public ItemSpec Build()
    {
        return _spec;
    }

    public Entity BuildTo(Entity item)
    {
        Apply(item, _spec);
        return item;
    }

    private static void Apply(Entity item, ItemSpec spec)
    {
        item.AddComponent(new ItemBase
        {
            templateName = spec.templateName,
            name = spec.name,
            stackCount = spec.stackCount,
            maxStack = spec.maxStack,
            isUsable = spec.isUsable,
            isConsumable = spec.isConsumable,
            isInstantiate = spec.isInstantiate
        });

        if (spec.attributes.Count == 1)
        {
            var contribution = spec.attributes[0];
            item.AddComponent(new AttributeContributionEntry
            {
                attrTypeId = contribution.attrTypeId,
                modifyType = contribution.modifyType,
                value = contribution.value,
                priority = contribution.priority
            });
        }
        else if (spec.attributes.Count > 1)
        {
            item.AddComponent(new ItemAttributeContributionListData
            {
                attributes = new List<ItemAttributeContributionSpec>(spec.attributes)
            });
        }

        if (!string.IsNullOrWhiteSpace(spec.useAbilityTemplateName))
        {
            item.AddComponent(new ItemUseAbilityData
            {
                abilityTemplateName = spec.useAbilityTemplateName
            });
        }

        if (spec.useEffectSpec != null)
            item.AddComponent(new ItemUseEffectData { effectSpec = spec.useEffectSpec });

        item.AddComponent(new ItemSpecData { spec = spec });
    }
}

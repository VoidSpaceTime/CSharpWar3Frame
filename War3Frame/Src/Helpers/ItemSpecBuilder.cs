using Friflo.Engine.ECS;
using War3Frame.Components;
using War3Frame.TemplateInit;

namespace War3Frame.Helpers;

/// <summary>
/// 物品模板 Builder，负责把物品静态配置写入现有模板实体。
/// </summary>
public sealed class ItemSpecBuilder
{
    private readonly ItemSpec _spec = new();
    private bool _hasUseAbility;

    private ItemSpecBuilder(string templateName)
    {
        _spec.templateName = templateName;
        _spec.name = templateName;
    }

    /// <summary>
    /// 创建指定模板名的物品模板 Builder。
    /// </summary>
    public static ItemSpecBuilder Create(string templateName)
    {
        return new ItemSpecBuilder(templateName);
    }

    /// <summary>
    /// 设置物品显示名称。
    /// </summary>
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

    /// <summary>
    /// 设置物品固定属性贡献。
    /// </summary>
    public ItemSpecBuilder Attr(int attrTypeId, ModifyType modifyType, float value, int priority = 0)
    {
        return Attr(attrTypeId, modifyType, LevelValue.Fixed(value), priority);
    }

    /// <summary>
    /// 设置物品按等级解析的属性贡献。
    /// </summary>
    public ItemSpecBuilder Attr(int attrTypeId, ModifyType modifyType, LevelValue value, int priority = 0)
    {
        _spec.attributes.Add(new ItemAttributeContributionSpec(attrTypeId, modifyType, value, priority));
        return this;
    }

    /// <summary>
    /// 设置物品经验曲线和最高等级。
    /// </summary>
    public ItemSpecBuilder Experience(ExperienceCurve curve, int maxLevel = 0, float currentExp = 0f)
    {
        _spec.experience = new ExperienceData
        {
            currentExp = currentExp,
            totalExp = currentExp,
            maxLevel = maxLevel,
            curve = curve
        };
        return this;
    }

    /// <summary>
    /// 引用已注册的共享 Ability template。
    /// </summary>
    public ItemSpecBuilder UseAbility(string abilityTemplateName)
    {
        EnsureAbilityNotConfigured();
        ArgumentException.ThrowIfNullOrWhiteSpace(abilityTemplateName);
        CommitUseAbility(abilityTemplateName);
        return this;
    }

    /// <summary>
    /// 立即构建并注册当前物品私有的完整 AbilitySpec。
    /// </summary>
    public ItemSpecBuilder UseAbility(Func<AbilitySpecBuilder, AbilitySpecBuilder> configure)
    {
        EnsureAbilityNotConfigured();
        ArgumentNullException.ThrowIfNull(configure);

        var templateName = AbilityTemplate.GetInlineItemAbilityName(_spec.templateName);
        var configured = configure(AbilitySpecBuilder.Create(templateName))
                         ?? throw new InvalidOperationException("Inline Ability 配置必须返回 AbilitySpecBuilder");
        var abilitySpec = configured.Build();
        ValidateFullInlineBehaviors(abilitySpec);
        var registeredName = AbilityTemplate.RegisterInlineItemAbility(_spec.templateName, abilitySpec);
        CommitUseAbility(registeredName);
        return this;
    }

    /// <summary>
    /// 立即构建 None 目标、零施法阶段和零冷却的物品私有 Effect Ability。
    /// </summary>
    public ItemSpecBuilder UseAbility(Func<EffectChainBuilder, EffectChainBuilder> configure)
    {
        EnsureAbilityNotConfigured();
        ArgumentNullException.ThrowIfNull(configure);

        var templateName = AbilityTemplate.GetInlineItemAbilityName(_spec.templateName);
        var configured = configure(EffectChainBuilder.Chain())
                         ?? throw new InvalidOperationException("Inline Effect 配置必须返回 EffectChainBuilder");
        var abilitySpec = AbilitySpecBuilder.Create(templateName)
            .TargetType(AbilityTargetType.None)
            .CastPoint(0f)
            .Channel(0f, 0f)
            .Backswing(0f)
            .BaseValue(AbilityHelper.CooldownDuration, 0f)
            .Behavior(new AbilityBehaviorSpec
            {
                trigger = AbilityBehaviorTrigger.OnEffect,
                effect = configured.Build()
            })
            .Build();
        var registeredName = AbilityTemplate.RegisterInlineItemAbility(_spec.templateName, abilitySpec);
        CommitUseAbility(registeredName);
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

        if (!item.TryGetComponent<ItemLevel>(out var level))
        {
            level = new ItemLevel { level = 1 };
            item.AddComponent(level);
        }

        ApplyAttributes(item, spec, level.level);

        if (spec.experience.HasValue)
            item.AddComponent(spec.experience.Value);

        if (!string.IsNullOrWhiteSpace(spec.useAbilityTemplateName))
        {
            item.AddComponent(new ItemUseAbilityData
            {
                abilityTemplateName = spec.useAbilityTemplateName
            });
        }

        item.AddComponent(new ItemSpecData { spec = spec });
    }

    private static void ApplyAttributes(Entity item, ItemSpec spec, int level)
    {
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
        else if (spec.attributes.Count > 1)
        {
            item.AddComponent(new ItemAttributeContributionListData
            {
                attributes = ResolveAttributes(spec.attributes, level)
            });
        }
    }

    private static List<ItemAttributeContributionSpec> ResolveAttributes(List<ItemAttributeContributionSpec> attributes,
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

    private void EnsureAbilityNotConfigured()
    {
        if (_hasUseAbility)
            throw new InvalidOperationException("同一 ItemSpecBuilder 只能配置一次 UseAbility");
    }

    private void CommitUseAbility(string abilityTemplateName)
    {
        _spec.useAbilityTemplateName = abilityTemplateName;
        _spec.isUsable = true;
        _hasUseAbility = true;
    }

    /// <summary>
    /// 在注册前拒绝不适用于 item companion 生命周期的行为。
    /// </summary>
    private static void ValidateFullInlineBehaviors(AbilitySpec spec)
    {
        foreach (var behavior in spec.behaviors)
        {
            if (behavior?.trigger is AbilityBehaviorTrigger.OnGranted or AbilityBehaviorTrigger.OnRemoved)
            {
                throw new InvalidOperationException(
                    "Item inline Ability 不允许 OnGranted 或 OnRemoved 行为");
            }
        }
    }
}

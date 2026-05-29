using Friflo.Engine.ECS;

namespace War3Frame.Helpers;

/// <summary>
/// 完整技能 Builder，统一写入基础信息、数值和行为配置。
/// </summary>
public sealed class AbilitySpecBuilder
{
    private readonly AbilitySpec _spec = new();

    private AbilitySpecBuilder(string templateName)
    {
        _spec.templateName = templateName;
    }

    /// <summary>
    /// 创建指定模板名的技能规格 Builder。
    /// </summary>
    public static AbilitySpecBuilder Create(string templateName)
    {
        return new AbilitySpecBuilder(templateName);
    }

    /// <summary>
    /// 设置技能显示名称。
    /// </summary>
    public AbilitySpecBuilder Name(string name)
    {
        _spec.name = name;
        return this;
    }

    /// <summary>
    /// 设置技能说明文本。
    /// </summary>
    public AbilitySpecBuilder Description(string description)
    {
        _spec.description = description;
        return this;
    }

    /// <summary>
    /// 设置技能目标类型。
    /// </summary>
    public AbilitySpecBuilder TargetType(AbilityTargetType targetType)
    {
        _spec.targetType = targetType;
        return this;
    }

    /// <summary>
    /// 设置技能基础数值。
    /// </summary>
    public AbilitySpecBuilder BaseValue(int statId, float value)
    {
        _spec.baseValues[statId] = value;
        return this;
    }

    /// <summary>
    /// 添加完整行为规格，适合需要显式控制触发时机和流程的技能。
    /// </summary>
    public AbilitySpecBuilder Behavior(AbilityBehaviorSpec behavior)
    {
        _spec.behaviors.Add(behavior);
        return this;
    }

    /// <summary>
    /// 添加主动施法触发的效果链，隐藏 OnCast、Chain 和 Build 的样板代码。
    /// </summary>
    public AbilitySpecBuilder OnCast(Func<AbilityEffectSpecBuilder, AbilityEffectSpecBuilder> configure)
    {
        return AddEffectBehavior(AbilityBehaviorTrigger.OnCast, configure);
    }

    /// <summary>
    /// 添加技能授予时触发的效果链。
    /// </summary>
    public AbilitySpecBuilder OnGranted(Func<AbilityEffectSpecBuilder, AbilityEffectSpecBuilder> configure)
    {
        return AddEffectBehavior(AbilityBehaviorTrigger.OnGranted, configure);
    }

    /// <summary>
    /// 添加技能移除时触发的效果链。
    /// </summary>
    public AbilitySpecBuilder OnRemoved(Func<AbilityEffectSpecBuilder, AbilityEffectSpecBuilder> configure)
    {
        return AddEffectBehavior(AbilityBehaviorTrigger.OnRemoved, configure);
    }

    /// <summary>
    /// 构建完整技能规格数据。
    /// </summary>
    public AbilitySpec Build()
    {
        return _spec;
    }

    /// <summary>
    /// 将技能规格写入现有 ability entity。
    /// </summary>
    public Entity BuildTo(Entity ability, int level)
    {
        Apply(ability, level, _spec);
        return ability;
    }

    private AbilitySpecBuilder AddEffectBehavior(AbilityBehaviorTrigger trigger,
        Func<AbilityEffectSpecBuilder, AbilityEffectSpecBuilder> configure)
    {
        var effect = configure(AbilityEffectSpecBuilder.Chain()).Build();
        _spec.behaviors.Add(new AbilityBehaviorSpec
        {
            trigger = trigger,
            effect = effect
        });
        return this;
    }

    private static void Apply(Entity ability, int level, AbilitySpec spec)
    {
        ability.AddComponent(new AbilityBase
        {
            templateName = spec.templateName,
            level = level,
            Name = spec.name,
            Description = spec.description,
            state = AbilityState.Ready,
            targetType = spec.targetType
        });

        foreach (var (statId, value) in spec.baseValues)
            AbilityHelper.SetBaseValue(ability, statId, value);

        ability.AddComponent(new AbilitySpecData { spec = spec });
        if (spec.behaviors.Count > 0)
        {
            ability.AddComponent(new AbilityBehaviorData
            {
                behaviors = spec.behaviors
            });

            var castEffect = FindCastEffect(spec.behaviors);
            if (castEffect != null)
                AbilityHelper.SetEffectSpec(ability, castEffect.Inner);
        }
    }

    private static AbilityEffectSpec? FindCastEffect(List<AbilityBehaviorSpec> behaviors)
    {
        foreach (var behavior in behaviors)
        {
            if (behavior.trigger == AbilityBehaviorTrigger.OnCast && behavior.effect != null)
                return behavior.effect;
        }

        return null;
    }
}

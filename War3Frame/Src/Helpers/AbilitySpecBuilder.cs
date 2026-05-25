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

    public static AbilitySpecBuilder Create(string templateName)
    {
        return new AbilitySpecBuilder(templateName);
    }

    public AbilitySpecBuilder Name(string name)
    {
        _spec.name = name;
        return this;
    }

    public AbilitySpecBuilder Description(string description)
    {
        _spec.description = description;
        return this;
    }

    public AbilitySpecBuilder TargetType(AbilityTargetType targetType)
    {
        _spec.targetType = targetType;
        return this;
    }

    public AbilitySpecBuilder BaseValue(int statId, float value)
    {
        _spec.baseValues[statId] = value;
        return this;
    }

    public AbilitySpecBuilder Behavior(AbilityBehaviorSpec behavior)
    {
        _spec.behaviors.Add(behavior);
        return this;
    }

    public AbilitySpec Build()
    {
        return _spec;
    }

    public Entity BuildTo(Entity ability, int level)
    {
        Apply(ability, level, _spec);
        return ability;
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

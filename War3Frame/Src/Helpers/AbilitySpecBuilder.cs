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
    /// 设置技能固定基础数值。
    /// </summary>
    public AbilitySpecBuilder BaseValue(int statId, float value)
    {
        return BaseValue(statId, LevelValue.Fixed(value));
    }

    /// <summary>
    /// 设置技能按等级解析的基础数值。
    /// </summary>
    public AbilitySpecBuilder BaseValue(int statId, LevelValue value)
    {
        _spec.baseValues[statId] = value;
        return this;
    }

    /// <summary>
    /// 设置释放前摇时长，完成后进入技能生效点。
    /// </summary>
    public AbilitySpecBuilder CastPoint(float seconds)
    {
        return CastPoint(LevelValue.Fixed(seconds));
    }

    /// <summary>
    /// 设置按等级解析的释放前摇时长。
    /// </summary>
    public AbilitySpecBuilder CastPoint(LevelValue seconds)
    {
        _spec.castPoint = seconds;
        return this;
    }

    /// <summary>
    /// 设置释放后摇时长，技能生效后进入该阶段。
    /// </summary>
    public AbilitySpecBuilder Backswing(float seconds)
    {
        return Backswing(LevelValue.Fixed(seconds));
    }

    /// <summary>
    /// 设置按等级解析的释放后摇时长。
    /// </summary>
    public AbilitySpecBuilder Backswing(LevelValue seconds)
    {
        _spec.backswing = seconds;
        return this;
    }

    /// <summary>
    /// 设置持续吟唱时长和 tick 间隔。
    /// </summary>
    public AbilitySpecBuilder Channel(float duration, float tickInterval = 0f)
    {
        return Channel(LevelValue.Fixed(duration), LevelValue.Fixed(tickInterval));
    }

    /// <summary>
    /// 设置按等级解析的持续吟唱时长和 tick 间隔。
    /// </summary>
    public AbilitySpecBuilder Channel(LevelValue duration, LevelValue tickInterval = default)
    {
        _spec.channelDuration = duration;
        _spec.channelTickInterval = tickInterval.kind == default && tickInterval.Resolve(1) == 0f
            ? LevelValue.Fixed(0f)
            : tickInterval;
        return this;
    }

    /// <summary>
    /// 设置技能经验曲线和最高等级。
    /// </summary>
    public AbilitySpecBuilder Experience(ExperienceCurve curve, int maxLevel = 0, float currentExp = 0f)
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
    /// 添加完整行为规格，适合需要显式控制触发时机和流程的技能。
    /// </summary>
    public AbilitySpecBuilder Behavior(AbilityBehaviorSpec behavior)
    {
        _spec.behaviors.Add(behavior);
        return this;
    }

    /// <summary>
    /// 添加技能真正生效点触发的效果链。
    /// </summary>
    public AbilitySpecBuilder OnEffect(Func<EffectChainBuilder, EffectChainBuilder> configure)
    {
        return AddEffectBehavior(AbilityBehaviorTrigger.OnEffect, configure);
    }

    /// <summary>
    /// 添加持续吟唱每跳触发的效果链。
    /// </summary>
    public AbilitySpecBuilder OnChannelTick(Func<EffectChainBuilder, EffectChainBuilder> configure)
    {
        return AddEffectBehavior(AbilityBehaviorTrigger.OnChannelTick, configure);
    }

    /// <summary>
    /// 添加施法被打断时触发的效果链。
    /// </summary>
    public AbilitySpecBuilder OnInterrupted(Func<EffectChainBuilder, EffectChainBuilder> configure)
    {
        return AddEffectBehavior(AbilityBehaviorTrigger.OnInterrupted, configure);
    }

    /// <summary>
    /// 添加技能完整结束时触发的效果链。
    /// </summary>
    public AbilitySpecBuilder OnFinished(Func<EffectChainBuilder, EffectChainBuilder> configure)
    {
        return AddEffectBehavior(AbilityBehaviorTrigger.OnFinished, configure);
    }

    /// <summary>
    /// 添加技能授予时触发的效果链。
    /// </summary>
    public AbilitySpecBuilder OnGranted(Func<EffectChainBuilder, EffectChainBuilder> configure)
    {
        return AddEffectBehavior(AbilityBehaviorTrigger.OnGranted, configure);
    }

    /// <summary>
    /// 添加技能移除时触发的效果链。
    /// </summary>
    public AbilitySpecBuilder OnRemoved(Func<EffectChainBuilder, EffectChainBuilder> configure)
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
        Func<EffectChainBuilder, EffectChainBuilder> configure)
    {
        var effect = configure(EffectChainBuilder.Chain()).Build();
        _spec.behaviors.Add(new AbilityBehaviorSpec
        {
            trigger = trigger,
            effect = effect
        });
        return this;
    }

    /// <summary>
    /// 将已完成的技能规格按指定等级写入 ability，供内部模板复用统一应用逻辑。
    /// </summary>
    internal static void Apply(Entity ability, int level, AbilitySpec spec)
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
            AbilityHelper.SetBaseValue(ability, statId, value.Resolve(level));

        AbilityHelper.SetBaseValue(ability, AbilityHelper.CastTime, spec.castPoint.Resolve(level));
        AbilityHelper.SetBaseValue(ability, AbilityHelper.BackswingDuration, spec.backswing.Resolve(level));
        AbilityHelper.SetBaseValue(ability, AbilityHelper.ChannelDuration, spec.channelDuration.Resolve(level));
        AbilityHelper.SetBaseValue(ability, AbilityHelper.ChannelTickInterval, spec.channelTickInterval.Resolve(level));

        if (spec.experience.HasValue)
            ability.AddComponent(spec.experience.Value);

        ability.AddComponent(new AbilitySpecData { spec = spec });
        if (spec.behaviors.Count > 0)
        {
            ability.AddComponent(new AbilityBehaviorData
            {
                behaviors = spec.behaviors
            });

            var effect = FindEffect(spec.behaviors, AbilityBehaviorTrigger.OnEffect);
            if (effect != null)
                AbilityHelper.SetEffectSpec(ability, effect);
        }
    }

    private static EffectSpec? FindEffect(List<AbilityBehaviorSpec> behaviors, AbilityBehaviorTrigger trigger)
    {
        foreach (var behavior in behaviors)
        {
            if (behavior.trigger == trigger && behavior.effect != null)
                return behavior.effect;
        }

        return null;
    }
}

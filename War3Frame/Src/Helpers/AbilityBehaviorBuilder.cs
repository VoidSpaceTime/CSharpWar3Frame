namespace War3Frame.Helpers;

/// <summary>
/// 技能行为 Builder，表达触发入口和调用的效果规格。
/// </summary>
public sealed class AbilityBehaviorBuilder
{
    private readonly AbilityBehaviorSpec _spec = new();

    private AbilityBehaviorBuilder(AbilityBehaviorTrigger trigger)
    {
        _spec.trigger = trigger;
    }

    /// <summary>
    /// 创建技能真正生效点触发的行为配置。
    /// </summary>
    public static AbilityBehaviorBuilder OnEffect()
    {
        return new AbilityBehaviorBuilder(AbilityBehaviorTrigger.OnEffect);
    }

    /// <summary>
    /// 创建持续吟唱每跳触发的行为配置。
    /// </summary>
    public static AbilityBehaviorBuilder OnChannelTick()
    {
        return new AbilityBehaviorBuilder(AbilityBehaviorTrigger.OnChannelTick);
    }

    /// <summary>
    /// 创建技能被打断时触发的行为配置。
    /// </summary>
    public static AbilityBehaviorBuilder OnInterrupted()
    {
        return new AbilityBehaviorBuilder(AbilityBehaviorTrigger.OnInterrupted);
    }

    /// <summary>
    /// 创建技能完整结束时触发的行为配置。
    /// </summary>
    public static AbilityBehaviorBuilder OnFinished()
    {
        return new AbilityBehaviorBuilder(AbilityBehaviorTrigger.OnFinished);
    }

    /// <summary>
    /// 创建技能授予到单位时触发的行为配置。
    /// </summary>
    public static AbilityBehaviorBuilder OnGranted()
    {
        return new AbilityBehaviorBuilder(AbilityBehaviorTrigger.OnGranted);
    }

    /// <summary>
    /// 创建技能从单位移除时触发的行为配置。
    /// </summary>
    public static AbilityBehaviorBuilder OnRemoved()
    {
        return new AbilityBehaviorBuilder(AbilityBehaviorTrigger.OnRemoved);
    }

    /// <summary>
    /// 设置该行为触发后要执行的技能效果规格。
    /// </summary>
    public AbilityBehaviorBuilder Do(AbilityEffectSpec effect)
    {
        _spec.effect = effect;
        return this;
    }

    /// <summary>
    /// 构建只保存触发和效果引用的行为规格。
    /// </summary>
    public AbilityBehaviorSpec Build()
    {
        return _spec;
    }
}

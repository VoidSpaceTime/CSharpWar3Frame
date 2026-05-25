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

    public static AbilityBehaviorBuilder OnCast()
    {
        return new AbilityBehaviorBuilder(AbilityBehaviorTrigger.OnCast);
    }

    public static AbilityBehaviorBuilder OnGranted()
    {
        return new AbilityBehaviorBuilder(AbilityBehaviorTrigger.OnGranted);
    }

    public static AbilityBehaviorBuilder OnRemoved()
    {
        return new AbilityBehaviorBuilder(AbilityBehaviorTrigger.OnRemoved);
    }

    public AbilityBehaviorBuilder Do(AbilityEffectSpec effect)
    {
        _spec.effect = effect;
        return this;
    }

    public AbilityBehaviorSpec Build()
    {
        return _spec;
    }
}

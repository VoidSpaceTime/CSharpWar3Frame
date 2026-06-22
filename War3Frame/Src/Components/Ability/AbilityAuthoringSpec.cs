using Friflo.Engine.ECS;

namespace War3Frame;

/// <summary>
/// 技能数值描述，统一表达常量、技能参数和单位属性来源。
/// </summary>
public readonly struct AbilityValue
{
    public EffectValueSpec EffectValue { get; }

    private AbilityValue(EffectValueSpec effectValue)
    {
        EffectValue = effectValue;
    }

    public static implicit operator EffectValueSpec(AbilityValue value) => value.EffectValue;

    public static AbilityValue Constant(float value)
    {
        return new AbilityValue(EffectValueSpec.Constant(value));
    }

    public static AbilityValue AbilityStat(int statId, float scale = 1f, float bonus = 0f)
    {
        return new AbilityValue(EffectValueSpec.Formula(
            EffectFormulaIds.Linear,
            statId,
            parameters: BuildScaleParameters(scale, bonus)));
    }

    public static AbilityValue OwnerAttr(int attrId, float scale = 1f, float bonus = 0f)
    {
        return UnitAttr(EffectFormulaIds.OwnerAttrFinal, attrId, scale, bonus);
    }

    public static AbilityValue CasterAttr(int attrId, float scale = 1f, float bonus = 0f)
    {
        return UnitAttr(EffectFormulaIds.CasterAttrFinal, attrId, scale, bonus);
    }

    public static AbilityValue TargetAttr(int attrId, float scale = 1f, float bonus = 0f)
    {
        return UnitAttr(EffectFormulaIds.TargetAttrFinal, attrId, scale, bonus);
    }

    public static AbilityValue Formula(string formulaId, int? statId = null, float? amount = null,
        Dictionary<string, float>? parameters = null)
    {
        return new AbilityValue(EffectValueSpec.Formula(formulaId, statId, amount, parameters));
    }

    private static AbilityValue UnitAttr(string formulaId, int attrId, float scale, float bonus)
    {
        return new AbilityValue(EffectValueSpec.Formula(
            formulaId,
            attrId,
            parameters: BuildScaleParameters(scale, bonus)));
    }

    private static Dictionary<string, float> BuildScaleParameters(float scale, float bonus)
    {
        return new Dictionary<string, float>
        {
            ["scale"] = scale,
            ["bonus"] = bonus
        };
    }
}

/// <summary>
/// 技能行为触发类型，描述行为从哪个生命周期入口启动。
/// </summary>
public enum AbilityBehaviorTrigger
{
    OnEffect,
    OnChannelTick,
    OnInterrupted,
    OnFinished,
    OnGranted,
    OnRemoved,
    OnOwnerDamaged,
    OnOwnerDealDamage
}

/// <summary>
/// 技能行为规格，负责保存触发与流程数据，不直接执行结算。
/// </summary>
public sealed class AbilityBehaviorSpec
{
    public AbilityBehaviorTrigger trigger;
    public EffectSpec? effect;
}

/// <summary>
/// 挂在 ability 实体上的技能行为配置。
/// </summary>
public struct AbilityBehaviorData : IComponent
{
    public List<AbilityBehaviorSpec> behaviors;
}

/// <summary>
/// 完整技能 authoring 规格，BuildTo 时写入现有 ability entity。
/// </summary>
public sealed class AbilitySpec
{
    public string templateName = string.Empty;
    public string name = string.Empty;
    public string description = string.Empty;
    public AbilityTargetType targetType;
    public readonly Dictionary<int, LevelValue> baseValues = new();
    public readonly List<AbilityBehaviorSpec> behaviors = new();
    public ExperienceData? experience;

    /// <summary>释放前摇时长，完成后进入真正生效点。</summary>
    public LevelValue castPoint = LevelValue.Fixed(0f);

    /// <summary>释放后摇时长，技能已生效后才进入该阶段。</summary>
    public LevelValue backswing = LevelValue.Fixed(0f);

    /// <summary>持续吟唱总时长。</summary>
    public LevelValue channelDuration = LevelValue.Fixed(0f);

    /// <summary>持续吟唱 tick 间隔，0 表示不触发 tick。</summary>
    public LevelValue channelTickInterval = LevelValue.Fixed(0f);
}

/// <summary>
/// 挂在 ability 实体上的完整技能规格数据。
/// </summary>
public struct AbilitySpecData : IComponent
{
    public AbilitySpec spec;
}

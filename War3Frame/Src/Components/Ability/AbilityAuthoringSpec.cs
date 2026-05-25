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
/// 技能效果规格的新命名包装，迁移期内部复用现有 EffectSpec。
/// </summary>
public sealed class AbilityEffectSpec
{
    public EffectSpec Inner { get; }

    internal AbilityEffectSpec(EffectSpec inner)
    {
        Inner = inner;
    }

    public static implicit operator EffectSpec(AbilityEffectSpec spec) => spec.Inner;
}

/// <summary>
/// 技能行为触发类型，描述行为从哪个生命周期入口启动。
/// </summary>
public enum AbilityBehaviorTrigger
{
    OnCast,
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
    public AbilityEffectSpec? effect;
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
    public readonly Dictionary<int, float> baseValues = new();
    public readonly List<AbilityBehaviorSpec> behaviors = new();
}

/// <summary>
/// 挂在 ability 实体上的完整技能规格数据。
/// </summary>
public struct AbilitySpecData : IComponent
{
    public AbilitySpec spec;
}

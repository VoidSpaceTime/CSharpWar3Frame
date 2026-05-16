using Friflo.Engine.ECS;

namespace War3Frame.Helpers;

public delegate float EffectFormulaFunc(EffectFormulaContext context);

public struct EffectFormulaContext
{
    public Entity caster;
    public Entity ability;
    public Entity target;
    public Entity effectEntity;
    public AbilityEffectContext effectContext;
    public EffectValueSpec value;

    public float GetParameter(string key, float defaultValue = 0f)
    {
        return value.parameters != null && value.parameters.TryGetValue(key, out var parameter)
            ? parameter
            : defaultValue;
    }
}

public static class EffectFormulaRegistry
{
    private static readonly Dictionary<string, EffectFormulaFunc> _formulas = new(StringComparer.OrdinalIgnoreCase);

    static EffectFormulaRegistry()
    {
        Register(EffectFormulaIds.StatFinal, ResolveStatFinal);
        Register(EffectFormulaIds.Constant, ResolveConstant);
        Register(EffectFormulaIds.Linear, ResolveLinear);
    }

    public static void Register(string formulaId, EffectFormulaFunc formula)
    {
        if (string.IsNullOrWhiteSpace(formulaId))
            throw new ArgumentException("Formula id cannot be empty.", nameof(formulaId));

        _formulas[formulaId] = formula ?? throw new ArgumentNullException(nameof(formula));
    }

    public static bool TryResolve(string formulaId, out EffectFormulaFunc formula)
    {
        return _formulas.TryGetValue(formulaId, out formula!);
    }

    public static float Resolve(Entity caster, Entity ability, Entity target, Entity effectEntity,
        AbilityEffectContext effectContext, EffectValueSpec value, Func<float> fallback)
    {
        if (!value.hasValue)
            return fallback();

        var formulaId = value.formulaId;
        if (string.IsNullOrWhiteSpace(formulaId))
        {
            if (value.hasStatId)
                formulaId = EffectFormulaIds.StatFinal;
            else if (value.hasAmount)
                formulaId = EffectFormulaIds.Constant;
            else
                return fallback();
        }

        if (!TryResolve(formulaId, out var formula))
            throw new InvalidOperationException($"Unknown effect formula id '{formulaId}'.");

        return formula(new EffectFormulaContext
        {
            caster = caster,
            ability = ability,
            target = target,
            effectEntity = effectEntity,
            effectContext = effectContext,
            value = value
        });
    }

    public static float Resolve(Entity caster, Entity ability, Entity target, Entity effectEntity,
        AbilityEffectContext effectContext, EffectValueSpec value, float fallbackValue)
    {
        return Resolve(caster, ability, target, effectEntity, effectContext, value, () => fallbackValue);
    }

    public static float Resolve(Entity caster, Entity ability, Entity target, Entity effectEntity,
        EffectValueSpec value, Func<float> fallback)
    {
        var effectContext = effectEntity.TryGetComponent<AbilityEffectContext>(out var context)
            ? context
            : default;

        return Resolve(caster, ability, target, effectEntity, effectContext, value, fallback);
    }

    public static float Resolve(Entity caster, Entity ability, Entity target, Entity effectEntity,
        EffectValueSpec value, float fallbackValue)
    {
        return Resolve(caster, ability, target, effectEntity, value, () => fallbackValue);
    }

    private static float ResolveStatFinal(EffectFormulaContext context)
    {
        if (!context.value.hasStatId)
            throw new InvalidOperationException("Formula 'stat.final' requires a statId.");

        return AbilityHelper.GetFinalValue(context.ability, context.value.statId);
    }

    private static float ResolveConstant(EffectFormulaContext context)
    {
        if (context.value.hasAmount)
            return context.value.amount;

        if (context.value.parameters != null && context.value.parameters.TryGetValue("value", out var value))
            return value;

        throw new InvalidOperationException("Formula 'constant' requires an amount or a 'value' parameter.");
    }

    private static float ResolveLinear(EffectFormulaContext context)
    {
        var baseValue = context.value.hasAmount
            ? context.value.amount
            : context.GetParameter("base");
        var scale = context.GetParameter("scale", 1f);
        var bonus = context.GetParameter("bonus");
        var statValue = context.value.hasStatId
            ? AbilityHelper.GetFinalValue(context.ability, context.value.statId)
            : 0f;

        return baseValue + statValue * scale + bonus;
    }
}

using Friflo.Engine.ECS;
using War3Frame.Components;
using War3Frame.Src.Components;

namespace War3Frame;

public static class EffectFormulaIds
{
    public const string StatFinal = "stat.final";
    public const string Constant = "constant";
    public const string Linear = "linear";
}

public struct EffectValueSpec
{
    public bool hasValue;
    public string? formulaId;
    public bool hasStatId;
    public int statId;
    public bool hasAmount;
    public float amount;
    public Dictionary<string, float>? parameters;

    public static EffectValueSpec Empty => default;

    public static EffectValueSpec Stat(int statId, string formulaId = EffectFormulaIds.StatFinal,
        Dictionary<string, float>? parameters = null)
    {
        return new EffectValueSpec
        {
            hasValue = true,
            formulaId = formulaId,
            hasStatId = true,
            statId = statId,
            parameters = parameters
        };
    }

    public static EffectValueSpec Constant(float amount)
    {
        return new EffectValueSpec
        {
            hasValue = true,
            formulaId = EffectFormulaIds.Constant,
            hasAmount = true,
            amount = amount
        };
    }

    public static EffectValueSpec Formula(string formulaId, int? statId = null, float? amount = null,
        Dictionary<string, float>? parameters = null)
    {
        return new EffectValueSpec
        {
            hasValue = true,
            formulaId = formulaId,
            hasStatId = statId.HasValue,
            statId = statId.GetValueOrDefault(),
            hasAmount = amount.HasValue,
            amount = amount.GetValueOrDefault(),
            parameters = parameters
        };
    }
}

public enum EffectStepKind
{
    Damage,
    Heal,
    Buff,
    AreaSearch,
    Projectile
}

public sealed class EffectSpec
{
    public List<EffectStepSpec> steps { get; } = new();
}

public struct EffectSpecData : IComponent
{
    public EffectSpec spec;
}

public struct EffectStepSpec
{
    public EffectStepKind kind;
    public DamageEffectStepSpec damage;
    public HealEffectStepSpec heal;
    public BuffEffectStepSpec buff;
    public AreaSearchEffectStepSpec areaSearch;
    public ProjectileEffectStepSpec projectile;

    public static EffectStepSpec Damage(DamageEffectStepSpec damage)
    {
        return new EffectStepSpec
        {
            kind = EffectStepKind.Damage,
            damage = damage
        };
    }

    public static EffectStepSpec Heal(HealEffectStepSpec heal)
    {
        return new EffectStepSpec
        {
            kind = EffectStepKind.Heal,
            heal = heal
        };
    }

    public static EffectStepSpec Buff(BuffEffectStepSpec buff)
    {
        return new EffectStepSpec
        {
            kind = EffectStepKind.Buff,
            buff = buff
        };
    }

    public static EffectStepSpec AreaSearch(AreaSearchEffectStepSpec areaSearch)
    {
        return new EffectStepSpec
        {
            kind = EffectStepKind.AreaSearch,
            areaSearch = areaSearch
        };
    }

    public static EffectStepSpec Projectile(ProjectileEffectStepSpec projectile)
    {
        return new EffectStepSpec
        {
            kind = EffectStepKind.Projectile,
            projectile = projectile
        };
    }
}

public struct DamageEffectStepSpec
{
    public EffectValueSpec value;
    public DamageType damageType;
    public DamageSrc damageSrc;
}

public struct HealEffectStepSpec
{
    public EffectValueSpec value;
    public int valueTypeId;
    public float amount;
}

public struct BuffEffectStepSpec
{
    public string buffId;
    public EffectValueSpec duration;
    public int attrTypeId;
    public ModifyType modifyType;
    public EffectValueSpec value;
    public BuffRefreshBehavior refreshBehavior;
}

public struct AreaSearchEffectStepSpec
{
    public float centerX;
    public float centerY;
    public EffectValueSpec radius;
    public int maxTargets;
    public TargetFilter filter;
    public string? customFilterId;
}

public struct ProjectileEffectStepSpec
{
    public ProjectileTrajectoryType trajectoryType;
    public string model;
    public EffectValueSpec speed;
    public Entity effectEntity;
    public EffectValueSpec arrivalThreshold;
    public EffectValueSpec maxDistance;
    public EffectValueSpec hitRadius;
    public TargetFilter hitFilter;
    public bool canHitSameTarget;
}

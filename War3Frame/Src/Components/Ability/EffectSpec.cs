using Friflo.Engine.ECS;
using War3Frame.Components;
using War3Frame.Src.Components;

namespace War3Frame;

public static class EffectFormulaIds
{
    /// <summary>读取技能数值层中指定 statId 的最终值。</summary>
    public const string StatFinal = "stat.final";

    /// <summary>直接返回常量值，适合配置表中写死的简单数值。</summary>
    public const string Constant = "constant";

    /// <summary>按 base + stat * scale + bonus 计算，适合普通线性成长公式。</summary>
    public const string Linear = "linear";

    /// <summary>读取技能拥有者单位的属性最终值。</summary>
    public const string OwnerAttrFinal = "owner.attr.final";

    /// <summary>读取施法者单位的属性最终值。</summary>
    public const string CasterAttrFinal = "caster.attr.final";

    /// <summary>读取目标单位的属性最终值。</summary>
    public const string TargetAttrFinal = "target.attr.final";
}

/// <summary>
/// 配置/编辑器友好的数值描述。
/// 普通技能优先使用 formulaId + statId + parameters，delegate 仅作为高级自定义路径。
/// </summary>
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
    Projectile,
    LineSearch,
    GroundAreaCreate
}

/// <summary>
/// 技能效果链根对象。
/// 只保存数据描述，运行时由 AbilityEffectHelper 展开成当前 ECS effect components。
/// </summary>
public sealed class EffectSpec
{
    public List<EffectStepSpec> steps { get; } = new();
}

/// <summary>
/// 挂在 ability 实体上的效果规格数据。
/// 这是配置/编辑器入口，不直接执行副作用。
/// </summary>
public struct EffectSpecData : IComponent
{
    public EffectSpec spec;
}

/// <summary>
/// 效果链中的单个步骤。
/// 为保持配置结构简单，使用 kind 选择对应 payload。
/// </summary>
public struct EffectStepSpec
{
    public EffectStepKind kind;
    public DamageEffectStepSpec damage;
    public HealEffectStepSpec heal;
    public BuffEffectStepSpec buff;
    public AreaSearchEffectStepSpec areaSearch;
    public ProjectileEffectStepSpec projectile;
    public LineSearchEffectStepSpec lineSearch;
    public GroundAreaCreateEffectStepSpec groundAreaCreate;

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

    public static EffectStepSpec LineSearch(LineSearchEffectStepSpec lineSearch)
    {
        return new EffectStepSpec
        {
            kind = EffectStepKind.LineSearch,
            lineSearch = lineSearch
        };
    }

    public static EffectStepSpec GroundAreaCreate(GroundAreaCreateEffectStepSpec groundAreaCreate)
    {
        return new EffectStepSpec
        {
            kind = EffectStepKind.GroundAreaCreate,
            groundAreaCreate = groundAreaCreate
        };
    }
}

public struct DamageEffectStepSpec
{
    /// <summary>伤害数值，可由 statId、公式 id 或参数表解析。</summary>
    public EffectValueSpec value;
    public DamageType damageType;
    public DamageSrc damageSrc;
}

public struct HealEffectStepSpec
{
    /// <summary>治疗数值，可由 statId、公式 id 或参数表解析。</summary>
    public EffectValueSpec value;

    /// <summary>兼容旧治疗字段，未配置 value 时可回退到该技能数值类型。</summary>
    public int valueTypeId;

    /// <summary>兼容旧治疗字段，未配置 value 时可作为直接治疗量。</summary>
    public float amount;
}

public struct BuffEffectStepSpec
{
    public string buffId;
    /// <summary>Buff 持续时间，优先走公式解析。</summary>
    public EffectValueSpec duration;
    public int attrTypeId;
    public ModifyType modifyType;
    /// <summary>Buff 带来的属性修改值，优先走公式解析。</summary>
    public EffectValueSpec value;
    public BuffRefreshBehavior refreshBehavior;
}

public struct AreaSearchEffectStepSpec
{
    public float centerX;
    public float centerY;
    /// <summary>范围半径；未配置时回退到技能 Radius stat。</summary>
    public EffectValueSpec radius;
    public int maxTargets;
    public TargetFilter filter;
    public string? customFilterId;
}

public struct LineSearchEffectStepSpec
{
    /// <summary>线形搜索长度；未配置时回退到技能 Range stat。</summary>
    public EffectValueSpec range;
    public float fallbackRange;
    /// <summary>线形搜索宽度。</summary>
    public EffectValueSpec width;
    public float fallbackWidth;
    public int maxTargets;
    public TargetFilter filter;
    public string? customFilterId;
    public GroundAreaTag reactionTag;
}

public struct GroundAreaCreateEffectStepSpec
{
    public GroundAreaTag tags;
    /// <summary>区域半径；未配置时回退到技能 Radius stat。</summary>
    public EffectValueSpec radius;
    public float fallbackRadius;
    /// <summary>区域持续时间。</summary>
    public EffectValueSpec duration;
    public float fallbackDuration;
    public GroundAreaBuffData buff;
    public GroundAreaPeriodicDamageData periodicDamage;
    public GroundAreaReactionData reaction;
}

public struct ProjectileEffectStepSpec
{
    public ProjectileTrajectoryType trajectoryType;
    public string model;
    /// <summary>弹道速度；未配置时回退到 ProjectileSpeed stat 或旧字段。</summary>
    public EffectValueSpec speed;
    public Entity effectEntity;
    /// <summary>到达判定阈值；未配置时使用旧字段或默认值。</summary>
    public EffectValueSpec arrivalThreshold;
    public EffectValueSpec maxDistance;
    public EffectValueSpec hitRadius;
    public TargetFilter hitFilter;
    public bool canHitSameTarget;
}

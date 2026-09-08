using Friflo.Engine.ECS;
using War3Frame.Src.Components;

namespace War3Frame.Helpers;

/// <summary>
/// 伤害计算器委托（ExecCalc 映射，对应 GAS GameplayEffectExecutionCalculation）。
/// 在一次伤害结算的 DamageContext 上原地修改：决定 preMitigation（暴击后）、mitigation（减免量）、finalDamage。
/// </summary>
public delegate void DamageCalculator(ref DamageContext ctx, EntityStore store);

/// <summary>
/// 伤害计算器注册表：按 calcKey（int）注册/取用；内置默认按 damageType 分派。
/// 默认计算器内聚执行：暴击判定（War3 同步 RNG）→ 按类型取护甲/魔抗公式减免 → 输出 finalDamage。
/// </summary>
public static class DamageCalculatorRegistry
{
    private const int DefaultPhysical = 1;
    private const int DefaultMagical = 2;
    private const int DefaultReal = 3;

    private static readonly System.Collections.Generic.SortedDictionary<int, DamageCalculator> _calculators = new();

    static DamageCalculatorRegistry()
    {
        // 内置默认计算器：Physical/Magical/Real 三条默认实现（暴击+减免内聚）。
        _calculators.Add(DefaultPhysical, DefaultCalculator.Physical);
        _calculators.Add(DefaultMagical, DefaultCalculator.Magical);
        _calculators.Add(DefaultReal, DefaultCalculator.Real);
    }

    /// <summary>注册自定义计算器，返回分配的 key（>= 100）。</summary>
    public static int Register(DamageCalculator calculator)
    {
        var key = _nextId++;
        _calculators.Add(key, calculator);
        return key;
    }

    private static int _nextId = 100;

    /// <summary>按 calcKey 取计算器；未注册返回 null。</summary>
    public static DamageCalculator? TryGet(int calcKey)
    {
        return _calculators.TryGetValue(calcKey, out var calc) ? calc : null;
    }

    /// <summary>按伤害类型取默认计算器 key。</summary>
    public static int GetDefaultKey(DamageType damageType)
    {
        return damageType switch
        {
            DamageType.Physical => DefaultPhysical,
            DamageType.Magical => DefaultMagical,
            _ => DefaultReal,
        };
    }
}

/// <summary>
/// 内置默认伤害计算器（ExecCalc 内聚实现）。
/// 流程：暴击判定（仅 Melee/Ranged 允许，Skill 默认禁暴）→ 减免 → finalDamage。
/// 无暴击属性 / 无护甲抗性时输出与历史"裸扣 max(0,damage)"一致，保证无回归。
/// </summary>
public static class DefaultCalculator
{
    /// <summary>物理默认：可暴击 + 按目标 Armor 走物理减免公式。</summary>
    public static readonly DamageCalculator Physical = (ref DamageContext ctx, EntityStore store) =>
    {
        var critMultiplier = RollCrit(ref ctx, store);
        var factor = ArmorFormulaRegistry.Physical(GetTargetFinal(ctx.target, AttributeHelper.Armor), ctx.preMitigation);
        ApplyMitigation(ref ctx, factor, critMultiplier);
    };

    /// <summary>魔法默认：可暴击 + 按目标 MagicResist 走魔法减免公式。</summary>
    public static readonly DamageCalculator Magical = (ref DamageContext ctx, EntityStore store) =>
    {
        var critMultiplier = RollCrit(ref ctx, store);
        var factor = ArmorFormulaRegistry.Magical(GetTargetFinal(ctx.target, AttributeHelper.MagicResist), ctx.preMitigation);
        ApplyMitigation(ref ctx, factor, critMultiplier);
    };

    /// <summary>真实默认：可暴击（Real 罕见但允许配置），不查护甲/魔抗减免，满额。</summary>
    public static readonly DamageCalculator Real = (ref DamageContext ctx, EntityStore store) =>
    {
        RollCrit(ref ctx, store);
        ctx.mitigation = 0f;
        ctx.finalDamage = MathF.Max(0f, ctx.preMitigation);
    };

    /// <summary>
    /// 暴击判定（War3 同步 RNG，无 System.Random 等非确定性源）。
    /// Melee/Ranged 默认允许暴击；Skill 来源默认禁暴（需带 calcKey 的自定义计算器另行开启）。
    /// 命中时 ctx.isCrit=true 且 preMitigation = base × max(1, CritMultiplier)。
    /// </summary>
    private static float RollCrit(ref DamageContext ctx, EntityStore store)
    {
        ctx.preMitigation = ctx.baseDamage;

        if (ctx.damageSrc == DamageSrc.Skill)
            return 1f;

        var critChance = GetSourceFinal(ctx.source, AttributeHelper.CritChance);
        if (critChance <= 0f)
            return 1f;

        // War3 同步随机：引擎保证全端同种子序列，锁步安全。
        var roll = War3Random.Next01();
        if (roll >= critChance)
            return 1f;

        var critMultiplier = GetSourceFinal(ctx.source, AttributeHelper.CritMultiplier);
        if (critMultiplier <= 1f)
            critMultiplier = 1f;

        ctx.isCrit = true;
        ctx.preMitigation = ctx.baseDamage * critMultiplier;
        return critMultiplier;
    }

    /// <summary>按减免乘子落地：mitigation = preMitigation×(1-factor)；finalDamage = preMitigation×factor。</summary>
    private static void ApplyMitigation(ref DamageContext ctx, float factor, float critMultiplier)
    {
        // factor = 减免后乘子（1=零减免）。mitigation 记录总减少量（暴击扩大后的基数 × 减少比例）。
        var pre = ctx.preMitigation;
        ctx.mitigation = pre * (1f - factor);
        ctx.finalDamage = MathF.Max(0f, pre * factor);
    }

    private static float GetSourceFinal(Entity unit, int attrId)
    {
        return AttributeHelper.GetFinalValue(unit, attrId);
    }

    private static float GetTargetFinal(Entity unit, int attrId)
    {
        return AttributeHelper.GetFinalValue(unit, attrId);
    }
}

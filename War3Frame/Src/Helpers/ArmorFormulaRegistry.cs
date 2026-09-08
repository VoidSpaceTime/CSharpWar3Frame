using System.Collections.Generic;

namespace War3Frame.Helpers;

/// <summary>
/// 减免公式委托：给定护甲/抗性值与入伤，返回减免比例乘子（0~1 区间含义为"减免后仍承受的比例"）。
/// 调用方以 effective = incoming × formula(armor, incoming) 落地；armor=0 时必须返回 1（零减免）。
/// </summary>
public delegate float DamageReductionFormula(float armorOrResist, float incoming);

/// <summary>
/// 护甲/魔抗减免公式注册表。
/// 物理槽（对 Armor）与魔法槽（对 MagicResist）可分别注册并运行时切换生效公式。
/// 内置 War3 公式与 Dota2 式公式两种，默认物理/魔法槽都指向 War3。
/// </summary>
public static class ArmorFormulaRegistry
{
    private sealed class Entry
    {
        public readonly string Name;
        public readonly DamageReductionFormula Formula;

        public Entry(string name, DamageReductionFormula formula)
        {
            Name = name;
            Formula = formula;
        }
    }

    /// <summary>已注册公式（按注册名索引）。</summary>
    private static readonly SortedDictionary<string, Entry> _formulas = new();

    /// <summary>物理减免槽当前生效公式。</summary>
    private static DamageReductionFormula _physical = War3Formula;

    /// <summary>魔法减免槽当前生效公式。</summary>
    private static DamageReductionFormula _magical = War3Formula;

    static ArmorFormulaRegistry()
    {
        // 内置两种公式：War3（默认）与 Dota2 式。
        _formulas.Add("War3", new Entry("War3", War3Formula));
        _formulas.Add("Dota2", new Entry("Dota2", Dota2Formula));
    }

    /// <summary>注册自定义公式；重名覆盖。</summary>
    public static void Register(string name, DamageReductionFormula formula)
    {
        _formulas[name] = new Entry(name, formula);
    }

    /// <summary>将物理减免槽切到指定公式；未注册名则忽略并保持现状。</summary>
    public static void SetPhysicalFormula(string name)
    {
        if (_formulas.TryGetValue(name, out var entry))
            _physical = entry.Formula;
    }

    /// <summary>将魔法减免槽切到指定公式；未注册名则忽略并保持现状。</summary>
    public static void SetMagicalFormula(string name)
    {
        if (_formulas.TryGetValue(name, out var entry))
            _magical = entry.Formula;
    }

    /// <summary>当前物理减免公式。</summary>
    public static DamageReductionFormula Physical => _physical;

    /// <summary>当前魔法减免公式。</summary>
    public static DamageReductionFormula Magical => _magical;

    /// <summary>
    /// War3 原版近似公式：乘子 = 1 - k·armor/(1 + k·|armor|)，k=0.06。
    /// armor=0 → 1（零减免）；armor&gt;0 → 递减；armor&lt;0 → 增伤（&gt;1）。
    /// </summary>
    public static float War3Formula(float armorOrResist, float incoming)
    {
        const float k = 0.06f;
        return 1f - k * armorOrResist / (1f + k * System.MathF.Abs(armorOrResist));
    }

    /// <summary>
    /// Dota2 式公式：同结构但系数 0.052。armor=0 → 1（零减免）。
    /// </summary>
    public static float Dota2Formula(float armorOrResist, float incoming)
    {
        const float k = 0.052f;
        return 1f - k * armorOrResist / (1f + k * System.MathF.Abs(armorOrResist));
    }
}

using Friflo.Engine.ECS;

namespace War3Frame;

/// <summary>
/// 按等级解析的数值类型。
/// </summary>
public enum LevelValueKind
{
    Fixed,
    PerLevel,
    LevelTable
}

/// <summary>
/// 可按等级解析的通用数值规格，供单位、物品和技能模板复用。
/// </summary>
public readonly struct LevelValue
{
    public readonly LevelValueKind kind;
    public readonly float fixedValue;
    public readonly float baseValue;
    public readonly float perLevel;
    public readonly float[]? table;

    private LevelValue(LevelValueKind kind, float fixedValue = 0f, float baseValue = 0f, float perLevel = 0f,
        float[]? table = null)
    {
        this.kind = kind;
        this.fixedValue = fixedValue;
        this.baseValue = baseValue;
        this.perLevel = perLevel;
        this.table = table;
    }

    /// <summary>
    /// 创建固定值，纯数字 authoring 默认等价于该模式。
    /// </summary>
    public static LevelValue Fixed(float value)
    {
        return new LevelValue(LevelValueKind.Fixed, fixedValue: value);
    }

    /// <summary>
    /// 创建线性等级值，1 级为 baseValue，每级额外增加 perLevel。
    /// </summary>
    public static LevelValue PerLevel(float baseValue, float perLevel)
    {
        return new LevelValue(LevelValueKind.PerLevel, baseValue: baseValue, perLevel: perLevel);
    }

    /// <summary>
    /// 创建等级表，1 级读取第 0 项，超过表长时沿用最后一项。
    /// </summary>
    public static LevelValue LevelTable(params float[] values)
    {
        return new LevelValue(LevelValueKind.LevelTable, table: values.Length == 0 ? [0f] : values);
    }

    /// <summary>
    /// 按指定等级解析最终数值。
    /// </summary>
    public float Resolve(int level)
    {
        var safeLevel = Math.Max(level, 1);
        return kind switch
        {
            LevelValueKind.PerLevel => baseValue + (safeLevel - 1) * perLevel,
            LevelValueKind.LevelTable => ResolveTable(safeLevel),
            _ => fixedValue
        };
    }

    public static implicit operator LevelValue(float value) => Fixed(value);

    private float ResolveTable(int level)
    {
        if (table == null || table.Length == 0)
            return 0f;

        var index = Math.Min(level - 1, table.Length - 1);
        return table[index];
    }
}

/// <summary>
/// 等级变化后触发等级基础数值重算的标记。
/// </summary>
public struct LevelStatDirty : ITag
{
}

/// <summary>
/// 单位等级组件。
/// </summary>
public struct UnitLevel : IComponent
{
    public int level;
}

/// <summary>
/// 物品等级组件。
/// </summary>
public struct ItemLevel : IComponent
{
    public int level;
}

/// <summary>
/// 经验曲线类型。
/// </summary>
public enum ExperienceCurveKind
{
    FixedStep,
    Linear,
    LevelTable
}

/// <summary>
/// 用于计算下一等级所需经验的曲线规格。
/// </summary>
public readonly struct ExperienceCurve
{
    public readonly ExperienceCurveKind kind;
    public readonly float fixedStep;
    public readonly float baseExp;
    public readonly float perLevel;
    public readonly float[]? table;

    private ExperienceCurve(ExperienceCurveKind kind, float fixedStep = 0f, float baseExp = 0f, float perLevel = 0f,
        float[]? table = null)
    {
        this.kind = kind;
        this.fixedStep = fixedStep;
        this.baseExp = baseExp;
        this.perLevel = perLevel;
        this.table = table;
    }

    /// <summary>
    /// 创建每级固定需求的经验曲线。
    /// </summary>
    public static ExperienceCurve FixedStep(float value)
    {
        return new ExperienceCurve(ExperienceCurveKind.FixedStep, fixedStep: value);
    }

    /// <summary>
    /// 创建线性经验曲线，1 级升 2 级为 baseExp，每级需求增加 perLevel。
    /// </summary>
    public static ExperienceCurve Linear(float baseExp, float perLevel)
    {
        return new ExperienceCurve(ExperienceCurveKind.Linear, baseExp: baseExp, perLevel: perLevel);
    }

    /// <summary>
    /// 创建手填等级经验表，当前等级读取对应升级需求。
    /// </summary>
    public static ExperienceCurve LevelTable(params float[] values)
    {
        return new ExperienceCurve(ExperienceCurveKind.LevelTable, table: values.Length == 0 ? [0f] : values);
    }

    /// <summary>
    /// 计算当前等级升到下一等级所需经验。
    /// </summary>
    public float RequiredForNextLevel(int currentLevel)
    {
        var safeLevel = Math.Max(currentLevel, 1);
        return kind switch
        {
            ExperienceCurveKind.Linear => baseExp + (safeLevel - 1) * perLevel,
            ExperienceCurveKind.LevelTable => ResolveTable(safeLevel),
            _ => fixedStep
        };
    }

    private float ResolveTable(int level)
    {
        if (table == null || table.Length == 0)
            return 0f;

        var index = Math.Min(level - 1, table.Length - 1);
        return table[index];
    }
}

/// <summary>
/// 经验运行时数据，只保存经验状态和升级曲线，不持有属性重算结果。
/// </summary>
public struct ExperienceData : IComponent
{
    public float currentExp;
    public float totalExp;
    public int maxLevel;
    public ExperienceCurve curve;
}

/// <summary>
/// 获得经验请求，由经验系统消费并转换为等级变化。
/// </summary>
public struct ExperienceGainRequest : IComponent
{
    public Entity target;
    public float amount;
    public float multiplier;
    public Entity source;
    public string sourceType;
}

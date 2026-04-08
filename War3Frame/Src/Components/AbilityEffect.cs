using Friflo.Engine.ECS;
using War3Frame.Components;
using War3Frame.Src.Components;

namespace War3Frame;

// ============================================================================
// 技能效果 Entity 组件
// 当技能释放后，创建一个"效果 Entity"携带以下组件，由各个 Effect System 处理
// ============================================================================

#region 核心组件（每个效果 Entity 都有）

/// <summary>
/// 效果来源 - 记录是谁施放的、来自哪个技能
/// </summary>
public struct EffectSource : IComponent
{
    /// <summary>施法者单位 Entity</summary>
    public Entity caster;

    /// <summary>来源技能 Entity</summary>
    public Entity ability;
}

/// <summary>
/// 效果目标 - 记录目标信息
/// </summary>
public struct EffectTargetInfo : IComponent
{
    /// <summary>目标单位（单体施法时）</summary>
    public Entity targetUnit;

    /// <summary>目标点 X</summary>
    public float targetX;

    /// <summary>目标点 Y</summary>
    public float targetY;
}

/// <summary>
/// 效果待处理标记 - 表示这个效果 Entity 尚未被处理
/// 处理完毕后移除此标记或删除 Entity
/// </summary>
public struct EffectPending : ITag;

#endregion

#region 伤害效果

/// <summary>
/// 伤害效果组件 - 附加到效果 Entity 上表示需要造成伤害
/// </summary>
public struct DamageEffectData : IComponent
{
    /// <summary>伤害数值</summary>
    public float amount;

    /// <summary>伤害类型（物理/魔法/真实）</summary>
    public DamageType damageType;

    /// <summary>伤害来源类型（近战/远程/技能）</summary>
    public DamageSrc damageSrc;
}

#endregion

#region 治疗效果

/// <summary>
/// 治疗效果组件
/// </summary>
public struct HealEffectData : IComponent
{
    /// <summary>治疗量</summary>
    public float amount;
}

#endregion

#region Buff 施加效果

/// <summary>
/// Buff 施加效果组件 - 表示需要给目标添加 Buff
/// </summary>
public struct ApplyBuffData : IComponent
{
    /// <summary>Buff 模板 ID</summary>
    public string buffId;

    /// <summary>持续时间（秒）</summary>
    public float duration;

    /// <summary>作用的属性类型 ID</summary>
    public int attrTypeId;

    /// <summary>修改类型（固定加成/百分比等）</summary>
    public ModifyType modifyType;

    /// <summary>修改值</summary>
    public float value;

    /// <summary>刷新行为</summary>
    public BuffRefreshBehavior refreshBehavior;
}

#endregion

#region 范围搜索

/// <summary>
/// 目标筛选器（Flags 枚举）- 可自由组合多个条件
/// 例如：TargetFilter.Enemy | TargetFilter.Hero 表示只搜索敌方英雄
/// </summary>
[Flags]
public enum TargetFilter
{
    /// <summary>无筛选</summary>
    None = 0,

    // ========== 阵营 ==========
    /// <summary>自己</summary>
    Self = 1 << 0,
    /// <summary>友方（不含自己）</summary>
    Ally = 1 << 1,
    /// <summary>敌方</summary>
    Enemy = 1 << 2,
    /// <summary>中立</summary>
    Neutral = 1 << 3,

    // ========== 单位类型 ==========
    /// <summary>英雄</summary>
    Hero = 1 << 4,
    /// <summary>普通单位</summary>
    Normal = 1 << 5,
    /// <summary>建筑</summary>
    Building = 1 << 6,
    /// <summary>召唤物</summary>
    Summon = 1 << 7,
    /// <summary>守卫（无敌单位）</summary>
    Ward = 1 << 8,

    // ========== 状态 ==========
    /// <summary>存活的</summary>
    Alive = 1 << 9,
    /// <summary>已死亡的</summary>
    Dead = 1 << 10,
    /// <summary>无敌的</summary>
    Invulnerable = 1 << 11,
    /// <summary>隐身的</summary>
    Invisible = 1 << 12,
    /// <summary>魔免的</summary>
    MagicImmune = 1 << 13,

    // ========== 常用预设组合 ==========
    /// <summary>所有敌方存活目标</summary>
    EnemyAlive = Enemy | Alive,
    /// <summary>所有友方存活目标</summary>
    AllyAlive = Ally | Alive,
    /// <summary>所有存活目标（不含自己）</summary>
    AllAlive = Enemy | Ally | Alive,
    /// <summary>所有存活目标（含自己）</summary>
    AllAliveIncludeSelf = Enemy | Ally | Self | Alive,
    /// <summary>敌方英雄</summary>
    EnemyHero = Enemy | Hero | Alive,
    /// <summary>敌方非建筑</summary>
    EnemyNonBuilding = Enemy | Hero | Normal | Summon | Alive,
}

/// <summary>
/// 范围搜索组件 - 表示需要在指定区域内搜索目标
/// 搜索完成后，系统会为每个找到的目标创建子效果 Entity
/// </summary>
public struct AreaSearchData : IComponent
{
    /// <summary>搜索中心 X（0 表示使用 EffectTargetInfo 的坐标）</summary>
    public float centerX;

    /// <summary>搜索中心 Y</summary>
    public float centerY;

    /// <summary>最大目标数量（0 表示无限制）</summary>
    public int maxTargets;

    /// <summary>预设目标筛选（Flags 组合）</summary>
    public TargetFilter filter;

    /// <summary>
    /// 自定义筛选器 ID（可选）
    /// 如果不为空，则通过 TargetFilterRegistry 查找注册的自定义筛选函数
    /// 自定义筛选在预设筛选之后执行（AND 关系）
    /// </summary>
    public string? customFilterId;
}

#endregion

#region 弹道效果

/// <summary>
/// 弹道效果组件 - 表示效果以弹道形式飞向目标
/// 到达目标后，弹道系统会移除此组件并触发其余效果（如伤害）
/// </summary>
public struct ProjectileData : IComponent
{
    /// <summary>弹道特效模型</summary>
    public string model;

    /// <summary>飞行速度</summary>
    public float speed;

    /// <summary>弹道特效 Entity（运行时由系统填充）</summary>
    public Entity effectEntity;

    /// <summary>到达距离阈值</summary>
    public float arrivalThreshold;
}

/// <summary>
/// 线性弹道组件 - 朝指定方向飞行，沿途命中所有目标
/// 与 ProjectileData 的区别：
///   ProjectileData = 追踪弹道（飞向一个目标）
///   LinearProjectileData = 方向弹道（朝一个方向飞，沿途伤害）
/// </summary>
public struct LinearProjectileData : IComponent
{
    /// <summary>弹道特效模型路径</summary>
    public string model;

    /// <summary>飞行速度</summary>
    public float speed;

    /// <summary>最大飞行距离</summary>
    public float maxDistance;

    /// <summary>碰撞半径（检测沿途目标）</summary>
    public float hitRadius;

    /// <summary>飞行方向 X（归一化）</summary>
    public float dirX;

    /// <summary>飞行方向 Y（归一化）</summary>
    public float dirY;

    /// <summary>已飞行距离（运行时，系统自动更新）</summary>
    public float traveled;

    /// <summary>弹道特效 Entity（运行时由系统填充）</summary>
    public Entity effectEntity;

    /// <summary>沿途目标筛选</summary>
    public TargetFilter hitFilter;

    /// <summary>是否可以多次命中同一目标（false = 每个目标只命中一次）</summary>
    public bool canHitSameTarget;
}

/// <summary>
/// 弹道已到达标记 - 由 ProjectileSystem 添加
/// </summary>
public struct ProjectileArrived : ITag;

#endregion

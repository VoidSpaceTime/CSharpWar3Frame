using System.Numerics;
using Friflo.Engine.ECS;
using War3Frame.Components;
using War3Frame.Src.Components;

namespace War3Frame;

/// <summary>
/// 记录一次效果的来源。
/// caster 是执行者；ability 始终指向提供配置的技能，包括物品的 companion ability。
/// </summary>
public struct EffectSource : IComponent
{
    public Entity caster;
    public Entity ability;
}

/// <summary>
/// 记录物品来源施法的显式上下文，延迟结算不得从当前 AbilityOwner 反推 user。
/// </summary>
public struct ItemEffectOrigin : IComponent
{
    public Entity item;
    public Entity user;
}

/// <summary>
/// 记录效果目标。targetUnit 为空时表示点目标或纯区域效果。
/// </summary>
public struct EffectTargetInfo : IComponent
{
    public Entity targetUnit;
    public float targetX;
    public float targetY;
}

/// <summary>
/// 单次效果执行上下文。
/// sourceEffect 用于标记区域搜索产生的子效果，effectId 用于调试和追踪链路。
/// </summary>
public struct AbilityEffectContext : IComponent
{
    public Entity caster;
    public Entity ability;
    public Entity sourceEffect;
    public Entity targetUnit;
    public float targetX;
    public float targetY;
    public int effectId;
}

/// <summary>效果仍在等待 projectile / area / settlement 系统处理。</summary>
public struct EffectPending : ITag;

/// <summary>效果已经完成所有结算，可由生命周期系统清理。</summary>
public struct EffectCompleted : ITag;

/// <summary>效果被请求过期，通常由弹道或生命周期提前结束触发。</summary>
public struct EffectExpired : ITag;

public delegate float DamageFormulaFunc(Entity caster, Entity ability, Entity target, DamageEffectData damage);

/// <summary>
/// 伤害效果 payload。
/// damageFunc 是高级覆盖；普通技能优先使用 value 中的 formulaId/statId。
/// </summary>
public struct DamageEffectData : IComponent
{
    public DamageFormulaFunc damageFunc;
    public EffectValueSpec value;
    public DamageType damageType;
    public DamageSrc damageSrc;
}

public delegate float HealFormulaFunc(Entity caster, Entity ability, Entity target, HealEffectData heal);

/// <summary>
/// 治疗效果 payload。
/// healFunc 是高级覆盖；普通技能优先使用 value，旧 amount/valueTypeId 作为兼容回退。
/// </summary>
public struct HealEffectData : IComponent
{
    public HealFormulaFunc healFunc;
    public EffectValueSpec value;
    public int valueTypeId;
    public float amount;
}

/// <summary>
/// Buff 应用 payload。
/// durationValue 和 modifyValue 允许配置表用公式表达持续时间和属性值。
/// </summary>
public struct ApplyBuffData : IComponent
{
    public string buffId;
    public EffectValueSpec durationValue;
    public float duration;
    public int attrTypeId;
    public ModifyType modifyType;
    public EffectValueSpec modifyValue;
    public float value;
    public BuffRefreshBehavior refreshBehavior;
}

[Flags]
/// <summary>
/// 区域搜索目标过滤条件。
/// 这些标记描述语义意图，具体阵营/类型判断由 TargetFilterRegistry 和 GroupHelper 实现。
/// </summary>
public enum TargetFilter
{
    None = 0,

    Self = 1 << 0,
    Ally = 1 << 1,
    Enemy = 1 << 2,
    Neutral = 1 << 3,

    Hero = 1 << 4,
    Normal = 1 << 5,
    Building = 1 << 6,
    Summon = 1 << 7,
    Ward = 1 << 8,

    Alive = 1 << 9,
    Dead = 1 << 10,
    Invulnerable = 1 << 11,
    Invisible = 1 << 12,
    MagicImmune = 1 << 13,

    EnemyAlive = Enemy | Alive,
    AllyAlive = Ally | Alive,
    AllAlive = Enemy | Ally | Alive,
    AllAliveIncludeSelf = Enemy | Ally | Self | Alive,
    EnemyHero = Enemy | Hero | Alive,
    EnemyNonBuilding = Enemy | Hero | Normal | Summon | Alive,
}

/// <summary>
/// 区域搜索 payload。搜索结果会生成子 effect，让伤害/治疗/Buff 对每个目标单独结算。
/// </summary>
public struct AreaSearchData : IComponent
{
    public float centerX;
    public float centerY;
    public EffectValueSpec radiusValue;
    public int maxTargets;
    public TargetFilter filter;
    public string? customFilterId;
}

/// <summary>线形搜索 payload，用于喷火、穿刺等沿线命中的效果。</summary>
public struct LineSearchData : IComponent
{
    public EffectValueSpec rangeValue;
    public float range;
    public EffectValueSpec widthValue;
    public float width;
    public int maxTargets;
    public TargetFilter filter;
    public string? customFilterId;
    public GroundAreaTag reactionTag;
}

/// <summary>地面区域语义标签，用于区分油污、火焰和燃烧区域等反应类型。</summary>
[Flags]
public enum GroundAreaTag
{
    None = 0,
    Oil = 1 << 0,
    Fire = 1 << 1,
    Burning = 1 << 2
}

/// <summary>地面区域基础数据；位置由同实体的 Position 组件提供。</summary>
public struct GroundAreaData : IComponent
{
    public GroundAreaTag tags;
    public EffectValueSpec radiusValue;
    public float radius;
}

/// <summary>地面区域来源，用于伤害、Buff 和反应追踪。</summary>
public struct GroundAreaSource : IComponent
{
    public Entity caster;
    public Entity ability;
    public Entity sourceEffect;
}

/// <summary>地面区域生命周期；到期后由系统清理相关 Buff 和区域实体。</summary>
public struct GroundAreaLifetime : IComponent
{
    public float duration;
    public float remaining;
}

/// <summary>地面区域创建 payload；效果结算时会生成独立的 ground area entity。</summary>
public struct GroundAreaCreateData : IComponent
{
    public GroundAreaTag tags;
    public EffectValueSpec radiusValue;
    public float radius;
    public EffectValueSpec durationValue;
    public float duration;
    public GroundAreaBuffData buff;
    public GroundAreaPeriodicDamageData periodicDamage;
    public GroundAreaReactionData reaction;
}

/// <summary>地面区域 Buff 配置，由区域系统按范围应用和移除。</summary>
public struct GroundAreaBuffData : IComponent
{
    public bool enabled;
    public string buffId;
    public int attrTypeId;
    public ModifyType modifyType;
    public EffectValueSpec value;
    public float fallbackValue;
}

/// <summary>地面区域周期伤害配置；系统只发 DamageRequest，不直接扣血。</summary>
public struct GroundAreaPeriodicDamageData : IComponent
{
    public bool enabled;
    public EffectValueSpec damageValue;
    public float fallbackDamage;
    public float tickInterval;
    public float timeSinceTick;
    public DamageType damageType;
    public DamageSrc damageSrc;
    public TargetFilter filter;
    public string? customFilterId;
}

/// <summary>地面区域反应配置，例如油污遇火后生成燃烧地面。</summary>
public struct GroundAreaReactionData : IComponent
{
    public bool enabled;
    public GroundAreaTag triggerTag;
    public GroundAreaTag resultTags;
    public EffectValueSpec resultDuration;
    public float fallbackDuration;
    public GroundAreaPeriodicDamageData resultPeriodicDamage;
}

/// <summary>地面区域反应请求，表示某类效果接触到了指定区域。</summary>
public struct GroundAreaReactionRequest : IComponent
{
    public Entity source;
    public Entity groundArea;
    public GroundAreaTag incomingTag;
}

/// <summary>
/// 视觉特效 payload。系统会把它转换成 EffectBase/EffectAttachment 等 ECS 视觉实体。
/// </summary>
public struct EffectVisualData : IComponent
{
    public EffectVisualKind kind;
    public string model;
    public string? key;
    public EffectAttachType attachPoint;
    public EffectValueSpec durationValue;
    public float duration;
    public bool hasPoint;
    public float x;
    public float y;
    public float z;
    public List<EffectVisualStepSpec>? steps;
    public int nextIndex;
}

/// <summary>标记某个 Buff 由指定地面区域产生，便于离开范围或区域消失时清理。</summary>
public struct GroundAreaBuffLink : ILinkComponent
{
    public Entity area;

    public GroundAreaBuffLink(Entity area)
    {
        this.area = area;
    }

    public Entity GetIndexedValue() => area;
}

/// <summary>
/// 弹道生命周期阶段。
/// Request 阶段用于跨系统交接，避免在移动系统里直接执行到达/过期副作用。
/// </summary>
public enum ProjectileLifecyclePhase
{
    PendingStart,
    InFlight,
    ArriveRequested,
    Arrived,
    ExpireRequested,
    Expired
}

/// <summary>弹道轨迹类型。Custom 保留给模板 hook 或后续扩展。</summary>
public enum ProjectileTrajectoryType
{
    Tracking,
    Linear,
    Bezier,
    Parabolic,
    Sinusoidal,
    Spiral,
    Custom
}

/// <summary>
/// 弹道运行时状态，只属于运行时 effect 实体，不应写回 ability 模板。
/// </summary>
public struct ProjectileRuntimeState : IComponent
{
    public ProjectileLifecyclePhase phase;
    public float elapsedTime;
    public float traveled;
    public float normalizedProgress;
    public Vector3 controlPoint1;
    public Vector3 controlPoint2;
    public float phaseOffset;
    public float dirX;
    public float dirY;
    public Entity visualEntity;
}

/// <summary>
/// 弹道效果 payload。
/// 数值字段优先支持 EffectValueSpec，旧 float 字段保留为兼容回退。
/// </summary>
public struct ProjectileData : IComponent
{
    public ProjectileTrajectoryType trajectoryType;
    public string model;
    public EffectValueSpec speedValue;
    public float speed;
    public Entity effectEntity;
    public EffectValueSpec arrivalThresholdValue;
    public float arrivalThreshold;
    public EffectValueSpec maxDistanceValue;
    public float maxDistance;
    public EffectValueSpec hitRadiusValue;
    public float hitRadius;
    public TargetFilter hitFilter;
    public bool canHitSameTarget;
    public EffectSpec? arriveEffect;
}

public struct ProjectileArriveRequest : ITag;

public struct ProjectileExpireRequest : ITag;

public struct ProjectileArrived : ITag;

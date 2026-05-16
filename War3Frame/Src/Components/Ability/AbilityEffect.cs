using System.Numerics;
using Friflo.Engine.ECS;
using War3Frame.Components;
using War3Frame.Src.Components;

namespace War3Frame;

public struct EffectSource : IComponent
{
    public Entity caster;
    public Entity ability;
}

public struct EffectTargetInfo : IComponent
{
    public Entity targetUnit;
    public float targetX;
    public float targetY;
}

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

public struct EffectPending : ITag;

public struct EffectCompleted : ITag;

public struct EffectExpired : ITag;

public delegate float DamageFormulaFunc(Entity caster, Entity ability, Entity target, DamageEffectData damage);

public struct DamageEffectData : IComponent
{
    public DamageFormulaFunc damageFunc;
    public EffectValueSpec value;
    public DamageType damageType;
    public DamageSrc damageSrc;
}

public delegate float HealFormulaFunc(Entity caster, Entity ability, Entity target, HealEffectData heal);

public struct HealEffectData : IComponent
{
    public HealFormulaFunc healFunc;
    public EffectValueSpec value;
    public int valueTypeId;
    public float amount;
}

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

public struct AreaSearchData : IComponent
{
    public float centerX;
    public float centerY;
    public EffectValueSpec radiusValue;
    public int maxTargets;
    public TargetFilter filter;
    public string? customFilterId;
}

public enum ProjectileLifecyclePhase
{
    PendingStart,
    InFlight,
    ArriveRequested,
    Arrived,
    ExpireRequested,
    Expired
}

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
}

public struct ProjectileArriveRequest : ITag;

public struct ProjectileExpireRequest : ITag;

public struct ProjectileArrived : ITag;

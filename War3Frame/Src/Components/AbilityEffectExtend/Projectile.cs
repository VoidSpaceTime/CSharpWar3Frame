using System.Numerics;
using Friflo.Engine.ECS;

namespace War3Frame.Components.AbilityEffectExtend;

public enum TrajectoryType
{
    Linear,           // 直线
    Tracking,         // 追踪目标
    Bezier,          // 贝塞尔曲线
    Parabolic,       // 抛物线
    Sinusoidal,      // 蛇形
    Spiral           // 螺旋
}

public struct ProjectileBase : IComponent
{
    public TrajectoryType trajectoryType;
    public Entity? TargetEntity;
    public Entity SourceAbility;
    public Entity SourceEntity;
    public float targetX;
    public float targetY;
    public float targetZ;
    public float speed;
    public float distance;
    public float height;
    public float startX;
    public float startY;
}

public struct ProjectileModel : IComponent
{
    public string name;
    public float animateScale;
    public float scale;
}

public struct ProjectileOnStart : ITag;

public struct ProjectileOnTravel : ITag;

public struct ProjectileOnArrive : ITag;

public struct ProjectilePositionDirty : ITag;
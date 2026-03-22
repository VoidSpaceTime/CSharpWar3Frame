using Friflo.Engine.ECS;

namespace War3Frame;

/// <summary>
/// 单位生命周期阶段
/// </summary>
public enum UnitLifecyclePhase
{
    Alive,
    Dying,
    Corpse,
    RebornPending,
    Pooled
}

/// <summary>
/// 单位生命周期状态
/// </summary>
public struct UnitState : IComponent
{
    public bool isAlive;
    public float rebornTime;
    public UnitLifecyclePhase lifePhase;
}

/// <summary>
/// 尸体清理到期标记
/// </summary>
public struct CorpseExpired : ITag;

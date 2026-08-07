using Friflo.Engine.ECS;

namespace War3Frame;

/// <summary>
/// 单位生命周期阶段
/// </summary>
public enum UnitLifecyclePhase
{
    Alive, //存活
    Death, //死亡待执行
    Corpse, //尸体
    ClearCorpse, //清理尸体
    Remove, //删除
    RebornPending, // 复活等待 — 待实现
    Pooled         // 单位池   — 待实现
}

/// <summary>
/// 单位生命周期状态
/// </summary>
public struct UnitLifeState : IComponent
{
    public bool isAlive;
    public float rebornTime;
    public UnitLifecyclePhase lifePhase;
}

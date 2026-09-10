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

/// <summary>
/// 单位死亡事实（对外广播）。由 KillUnit 在 Alive→Death 当次创建一条。
/// source 为击杀者；空 = 非击杀死亡（环境/脚本无源），奖励等击杀语义不适用但死亡事实仍广播。
/// 移除单位不产生本事件（移除 ≠ 死亡，用 RemoveUnit）。
/// </summary>
public struct UnitDiedEvent : IComponent
{
    public Entity unit;
    public Entity source;
}

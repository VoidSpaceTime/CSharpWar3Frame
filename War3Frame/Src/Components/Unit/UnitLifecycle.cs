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
    Disposing, // 原生已移除，ECS 待销毁（原生移除与实体删除之间的确认阶段）
    RebornPending, // 复活等待：保留实体与句柄，等待 rebornTime 到期
    Pooled         // 单位池：实体与句柄挂起，等待复用或放弃
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

/// <summary>
/// 单位复活/显隐原生请求。show=true 表示复位生命并显示（复活或出池），show=false 表示仅隐藏（入池）。
/// 一次性副作用意图，由 Native 层消费后删除，不承载长期状态。
/// </summary>
public struct UnitRebornNativeRequest : IComponent
{
    public bool show;
}

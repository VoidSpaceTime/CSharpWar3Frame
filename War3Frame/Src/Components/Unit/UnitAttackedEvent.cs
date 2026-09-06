using Friflo.Engine.ECS;
using War3Frame;

namespace War3Frame;

/// <summary>
/// 模拟攻击请求（原生 EVENT_PLAYER_UNIT_ATTACKED 桥接产物）。
/// 由 NativePlayerEventBridge 回调创建独立请求实体，AttackSimulationSystem 唯一消费：
/// 近战直发 DamageRequest、远程/闪电链走后续投射物增量；处理后删除请求实体。
/// </summary>
public struct UnitAttackedRequest : IComponent
{
    /// <summary>发起攻击的单位（GetAttacker 反查）。</summary>
    public Entity attacker;

    /// <summary>被攻击的单位（GetTriggerUnit 反查）。</summary>
    public Entity target;
}

/// <summary>
/// 单位被攻击事件（对外广播）。
/// 由 NativePlayerEventBridge 回调创建独立事件实体（带 TriggerEventMarker），
/// 供触发器体系（TriggerSpec）只读匹配；由 EventCleanupSystem 统一清理，监听者不得删除。
/// attacker/target 均为已反查的 ECS 单位实体（未登记的 native 单位不产生本事件）。
/// </summary>
public struct UnitAttackedEvent : IComponent
{
    /// <summary>发起攻击的单位（GetAttacker 反查）。</summary>
    public Entity attacker;

    /// <summary>被攻击的单位（GetTriggerUnit 反查）。</summary>
    public Entity target;
}

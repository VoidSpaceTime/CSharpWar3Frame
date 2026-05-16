using Friflo.Engine.ECS;
using War3Frame.Components;

namespace War3Frame.Src.Components;

/// <summary>
/// 治疗结算请求。
/// 由效果系统产生，由 HealResolveSystem 统一修改生命值。
/// </summary>
public struct HealRequest : IComponent
{
    public Entity source;
    public Entity target;
    public float amount;
}

/// <summary>
/// 治疗结算结果事件。
/// 用于 UI、日志或后续响应系统读取，不应再次修改生命值。
/// </summary>
public struct HealEvent : IComponent
{
    public Entity source;
    public Entity target;
    public float baseHeal;
    public float finalHeal;
    public float remainingHealth;
}

/// <summary>
/// Buff 应用请求。
/// 这里只描述意图，实际 Buff 实体创建和刷新由 BuffApplyResolveSystem 处理。
/// </summary>
public struct BuffApplyRequest : IComponent
{
    public Entity source;
    public Entity target;
    public string buffId;
    public float duration;
    public int attrTypeId;
    public ModifyType modifyType;
    public float value;
    public BuffRefreshBehavior refreshBehavior;
}

/// <summary>
/// Buff 应用完成事件。
/// 记录最终创建或刷新的 Buff 实体。
/// </summary>
public struct BuffAppliedEvent : IComponent
{
    public Entity source;
    public Entity target;
    public Entity buff;
    public string buffId;
}

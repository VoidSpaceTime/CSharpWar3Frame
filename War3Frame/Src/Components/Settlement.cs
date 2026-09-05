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
    /// <summary>UI 图标路径（供 buff 图标显示）。</summary>
    public string? icon;
    /// <summary>周期 tick 间隔（秒，0 = 不 tick）。</summary>
    public float tickInterval;
    /// <summary>Tick 行为 ID（DoT 用 "DealDamage"）。</summary>
    public string? tickActionId;
    /// <summary>每跳数值；>0 表示 DoT 型（不产生属性贡献）。</summary>
    public float tickValue;
    /// <summary>分类标签（位组合：Debuff/Control/DoT 等）。</summary>
    public BuffTag tags;
    /// <summary>Buff 实体类型（Attribute/Tick/PureTag）。</summary>
    public BuffKind kind;
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

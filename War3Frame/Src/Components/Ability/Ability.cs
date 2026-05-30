using Friflo.Engine.ECS;

namespace War3Frame;

public enum AbilityState
{
    Ready, // 就绪
    Casting, // 吟唱中
    Channeling, // 持续施法中
    Backswing, // 后摇中
    Cooldown, // 冷却中
    Ban, // 被禁用
}

/// <summary>
/// 技能类型。
/// </summary>
public enum AbilityType
{
    Active, // 主动
    Passive // 被动
}

/// <summary>
/// 技能目标类型。
/// </summary>
public enum AbilityTargetType
{
    None, // 无目标
    Unit, // 单位
    Point, // 点
    Area // 区域
}

/// <summary>
/// 能力挂载类型。
/// </summary>
public enum AbilityMountType
{
    Slot,
    NonSlot,
    ItemGranted,
    SystemGranted
}

/// <summary>
/// 能力触发类型。
/// </summary>
public enum AbilityTriggerType
{
    ActiveCast,
    OnDamaged,
    OnDeath,
    OnHit,
    OnEquip,
    OnUnequip,
    Periodic
}

/// <summary>
/// 能力执行流节点类型。
/// </summary>
public enum AbilityFlowNodeType
{
    Cast,
    Projectile,
    AreaSearch,
    Periodic,
    Damage,
    Heal,
    Buff,
    AttributeContribution,
    Move,
    Lifecycle
}

/// <summary>
/// 能力结算类型。
/// </summary>
public enum AbilitySettlementType
{
    Damage,
    Heal,
    Buff,
    AttributeContribution,
    Move,
    Lifecycle
}

/// <summary>
///     技能基础组件（所有技能都有）
/// </summary>
public struct AbilityBase : IComponent
{
    /// <summary>技能类型 ID（用于识别技能模板）</summary>
    public string templateName;

    /// <summary>技能等级</summary>
    public int level;

    /// <summary>技能名称。</summary>
    public string Name;

    /// <summary>技能描述。</summary>
    public string Description;

    /// <summary>技能状态</summary>
    public AbilityState state;

    /// <summary>目标类型</summary>
    public AbilityTargetType targetType;
}

/// <summary>
/// 能力挂载元信息。
/// 用于表达能力通过何种方式属于某个来源。
/// </summary>
public struct AbilityMountInfo : IComponent
{
    public AbilityMountType mountType;
}

/// <summary>
/// 能力触发元信息。
/// 用于表达当前能力主要通过何种方式启动。
/// </summary>
public struct AbilityTriggerInfo : IComponent
{
    public AbilityTriggerType triggerType;
}

/// <summary>
/// 能力执行流节点元信息。
/// 用于标记当前 ability/effect 所处的节点语义。
/// </summary>
public struct AbilityFlowNodeInfo : IComponent
{
    public AbilityFlowNodeType nodeType;
}

/// <summary>
/// 能力结算元信息。
/// 用于表达最终效果应落到哪类结算层。
/// </summary>
public struct AbilitySettlementInfo : IComponent
{
    public AbilitySettlementType settlementType;
}

/// <summary>
/// 技能冷却运行时状态。
/// </summary>
public struct AbilityCooldownState : IComponent
{
    /// <summary>
    /// 剩余冷却时间。
    /// </summary>
    public float remaining;
}

/// <summary>
/// 技能挂载请求。
/// </summary>
public struct AbilityAttachRequest : IComponent
{
    public Entity unit;
    public Entity ability;
    public int slotIndex;
}

/// <summary>
/// 技能移除请求。
/// </summary>
public struct AbilityRemoveRequest : IComponent
{
    public Entity unit;
    public int slotIndex;
    public bool destroyAbility;
}

// AbilityOwner 和 SkillItem 已移动到 AbilitySlotBinding.cs 中
// 使用 AbilityOwnerRelation : IRelation<Entity> 替代
public struct AbilityBan : IComponent
{
    /// <summary>禁用原因。</summary>
    public string banReason;

    /// <summary>禁用总时长。</summary>
    public float banDurtion;

    /// <summary>当前已禁用时长。</summary>
    public float banCurrent;

    /// <summary>当前是否处于禁用状态。</summary>
    public bool isBan;
}

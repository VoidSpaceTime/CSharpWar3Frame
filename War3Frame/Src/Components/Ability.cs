using Friflo.Engine.ECS;

namespace War3Frame;

public enum AbilityState
{
    Ready, // 就绪
    Casting, // 吟唱中
    Channeling, // 持续施法中
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
/// 技能冷却运行时状态。
/// </summary>
public struct AbilityCooldownState : IComponent
{
    /// <summary>
    /// 剩余冷却时间。
    /// </summary>
    public float remaining;
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

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

public enum AbilityType
{
    Active, // 主动
    Passive // 被动
}

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

    public string Name;
    public string Description;

    /// <summary>技能状态</summary>
    public AbilityState state;

    /// <summary>目标类型</summary>
    public AbilityTargetType targetType;
}

// AbilityOwner 和 SkillItem 已移动到 AbilitySlotBinding.cs 中
// 使用 AbilityOwnerRelation : IRelation<Entity> 替代
public struct AbilityBan : IComponent
{
    public string banReason;
    public float banDurtion;
    public float banCurrent;
    public bool isBan;
}

using Friflo.Engine.ECS;

namespace War3Frame;

public enum AbilityState
{
    Ready, // 就绪
    Casting, // 吟唱中
    Channeling, // 持续施法中
    Cooldown, // 冷却中
    Ban
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

    /// <summary>技能等级</summary>
    public int level;

    /// <summary>技能状态</summary>
    public AbilityState state;

    /// <summary>基础 CD（从模板/等级表获取，升级时更新）</summary>
    public float cooldown;

    /// <summary>当前 CD 剩余</summary>
    public float currentCd;

    /// <summary>魔法消耗</summary>
    public float manaCost;

    /// <summary>施法时间</summary>
    public float castTime;

    /// <summary>施法距离</summary>
    public float castRange;
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
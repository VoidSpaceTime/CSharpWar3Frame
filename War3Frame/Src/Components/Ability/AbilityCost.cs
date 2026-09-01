using Friflo.Engine.ECS;

namespace War3Frame;

// ============================================================================
// 技能消耗组件
// ============================================================================

/// <summary>
/// 魔法消耗组件
/// </summary>
public struct ManaCost : IComponent
{
    public float value;
}

/// <summary>
/// 生命消耗组件
/// </summary>
public struct HealthCost : IComponent
{
    public float value;
}

/// <summary>
/// 通用属性消耗组件 - 可以指定消耗任意属性（如怒气、能量等）
/// </summary>
public struct AttributeCost : IComponent
{
    /// <summary>属性 ID (AttributeHelper.xxx)</summary>
    public int attrId;
    
    /// <summary>消耗数值</summary>
    public float value;
}

/// <summary>
/// 物品消耗组件 - 需要消耗特定物品
/// </summary>
public struct ItemCost : IComponent
{
    /// <summary>物品类型模板名（对应 ItemBase.templateName）</summary>
    public string templateName;
    
    /// <summary>消耗数量</summary>
    public int count;
}

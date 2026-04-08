using Friflo.Engine.ECS;

namespace War3Frame.Components;

/// <summary>
/// 属性贡献来源标识。
/// 用于标记某个实体属于哪一类长期属性贡献来源。
/// </summary>
public struct AttributeContributionSource : IComponent
{
    public ModifierSourceType kind;
}

/// <summary>
/// 属性贡献条目。
/// 描述某个来源需要对单位属性施加的单条长期贡献。
/// </summary>
public struct AttributeContributionEntry : IComponent
{
    public int attrTypeId;
    public ModifyType modifyType;
    public float value;
    public int priority;
}

/// <summary>
/// 挂载技能属性应用请求。
/// </summary>
public struct AbilityAttrApplyRequest : ITag { }

/// <summary>
/// 挂载技能属性移除请求。
/// </summary>
public struct AbilityAttrRemoveRequest : ITag { }

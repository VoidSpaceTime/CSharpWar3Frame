using Friflo.Engine.ECS;

namespace War3Frame;

#region 枚举定义


/// <summary>
///     修改器来源类型
/// </summary>
public enum ModifierSourceType
{
    Item, // 物品
    Ability, // 技能
    Buff, // Buff/Debuff
    Aura, // 光环
    Talent, // 天赋
    Other // 其他
}

#endregion

#region 组件定义

/// <summary>
/// 属性值组件 - 每个属性 Entity 都有
/// </summary>
public struct AttrValue : IComponent
{
    public float baseValue; // 基础值（模板，不变）
    public float finalValue; // 计算后的最终值

    // 可选：缓存计算中间值，用于调试/UI
    public float flatBonus; // 固定加成总和
    public float percentBonus; // 百分比加成总和
}

/// <summary>
/// 属性类型标识 - 用于区分属性种类
/// </summary>
public struct AttrTypeId : IComponent
{
    public int typeId; // 用整数而非字符串，利于比较和同步

    // 或者使用枚举，但会限制扩展性
    // public AttrType type;
}

/// <summary>
/// 属性归属关系 - 指向拥有者单位
/// </summary>
public struct AttrOwner : ILinkComponent
{
    public Entity GetIndexedValue() => owner;
    public Entity owner;

    public AttrOwner(Entity owner) => this.owner = owner;
}

/// <summary>
/// Unit 拥有属性的关系 (1:N)
/// 使用 IRelation 允许一个 Unit 拥有多个属性
/// </summary>
public struct HasAttr : IRelation<Entity>
{
    public Entity GetRelationKey() => attrEntity;

    /// <summary>属性 Entity</summary>
    public Entity attrEntity;

    /// <summary>属性类型 ID（冗余，方便快速查找）</summary>
    public int typeId;

    public HasAttr(Entity attr, int typeId)
    {
        this.attrEntity = attr;
        this.typeId = typeId;
    }
}

/// <summary>
///     属性脏标记 - 表示需要重新计算
/// </summary>
public struct AttrDirty : ITag;

#endregion
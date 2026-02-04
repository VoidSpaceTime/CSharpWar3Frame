using Friflo.Engine.ECS;

namespace War3Frame.Components;

/// <summary>
/// 修改器值
/// </summary>
public struct ModifyValue : IComponent
{
    public ModifyType modifyType; // Flat / PercentAdd / PercentMul
    public float value;
    public int priority;
}

/// <summary>
/// 修改器目标 - 指向属性 Entity (N:1)
/// 使用 ILinkComponent 支持反向查询
/// </summary>
public struct ModifyTarget : ILinkComponent
{
    public Entity GetIndexedValue() => target;
    public Entity target;

    public ModifyTarget(Entity attrEntity) => target = attrEntity;
}

/// <summary>
/// 修改器来源 - 指向 Buff/Item/Ability (N:1)
/// </summary>
public struct ModifySource : ILinkComponent
{
    public Entity GetIndexedValue() => source;
    public Entity source;

    public ModifySource(Entity source) => this.source = source;
}

public enum ModifyType
{
    Flat, // +100
    PercentAdd, // +10% (加法叠加)
    PercentMul // ×1.1 (乘法叠加)
}
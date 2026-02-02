using Friflo.Engine.ECS;

namespace War3Frame;

/// <summary>
///     技能槽位索引 - 附加到技能 Entity 上，标识在单位的哪个槽位
/// </summary>
public struct AbilitySlotIndex : IComponent
{
    /// <summary>槽位索引 (0-based)</summary>
    public int slotIndex;

    /// <summary>快捷键（可选覆盖默认快捷键）</summary>
    public string? hotkey;
}

/// <summary>
///     技能所有者关系 - 使用 Friflo Link 关联技能到单位
///     通过这个关系可以查询单位拥有的所有技能
/// </summary>
public struct AbilityOwnerRelation : ILinkComponent
{
    public Entity GetIndexedValue()
    {
        return owner;
    }

    /// <summary>拥有此技能的单位 Entity</summary>
    public Entity owner;

    public AbilityOwnerRelation(Entity owner)
    {
        this.owner = owner;
    }
}
using Friflo.Engine.ECS;

namespace War3Frame;

/// <summary>
///     技能槽容器 - 附加到单位 Entity 上
///     用于管理单位可以拥有的技能槽位数量
/// </summary>
public struct AbilitySlotContainer : IComponent
{
    /// <summary>最大槽位数（可动态扩展）</summary>
    public int maxSlots;

    /// <summary>当前已占用槽位数</summary>
    public int currentCount;

    /// <summary>默认 8 槽位</summary>
    public static AbilitySlotContainer Default => new() { maxSlots = 8, currentCount = 0 };

    /// <summary>自定义槽位数</summary>
    public static AbilitySlotContainer WithSlots(int maxSlots)
    {
        return new AbilitySlotContainer { maxSlots = maxSlots, currentCount = 0 };
    }
}

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
public struct AbilityOwner : ILinkComponent
{
    public Entity GetIndexedValue()
    {
        return owner;
    }

    /// <summary>拥有此技能的单位 Entity</summary>
    public Entity owner;

    public AbilityOwner(Entity owner)
    {
        this.owner = owner;
    }
}
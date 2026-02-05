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

    /// <summary>默认 4 槽位</summary>
    public static AbilitySlotContainer Default => new() { maxSlots = 8, currentCount = 0 };

    /// <summary>自定义槽位数</summary>
    public static AbilitySlotContainer WithSlots(int maxSlots)
    {
        return new AbilitySlotContainer { maxSlots = maxSlots, currentCount = 0 };
    }
}
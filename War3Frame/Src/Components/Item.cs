using Friflo.Engine.ECS;

namespace War3Frame.Components;

public struct ItemOwner : ILinkComponent
{
    public Entity GetIndexedValue()
    {
        return unit;
    }

    public ItemOwner(Entity unit)
    {
        this.unit = unit;
    }

    public Entity unit;
}

public struct ItemSlotIndex : IComponent
{
    public int index;
}

/// <summary>
/// 物品基础组件 - 定义物品的静态数据
/// </summary>
public struct ItemBase : IComponent
{
    public string id;           // 物品模板ID
    public string name;         // 名称
    public string icon;         // 图标路径
    public string description;  // 描述
    public int stackCount;      // 堆叠数量
    public int maxStack;        // 最大堆叠
    public bool isUsable;       // 是否可使用
}
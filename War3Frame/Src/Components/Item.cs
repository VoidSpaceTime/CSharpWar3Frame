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
/// 地上物品状态。
/// </summary>
public struct ItemGroundTag : ITag
{
}

/// <summary>
/// 背包物品状态。
/// </summary>
public struct ItemInventoryTag : ITag
{
}

/// <summary>
/// 装备物品状态。
/// </summary>
public struct ItemEquippedTag : ITag
{
}

/// <summary>
/// 仓库物品状态。
/// </summary>
public struct ItemStoredTag : ITag
{
}

/// <summary>
/// 物品属性应用请求。
/// </summary>
public struct ItemAttrApplyRequest : ITag
{
}

/// <summary>
/// 物品属性移除请求。
/// </summary>
public struct ItemAttrRemoveRequest : ITag
{
}

/// <summary>
/// 物品基础组件 - 定义物品的静态数据
/// </summary>
public struct ItemBase : IComponent
{
    public string templateName;
    public string name; // 名称
    public int stackCount; // 堆叠数量
    public int maxStack; // 最大堆叠
    public bool isUsable; // 是否可使用
    public bool isConsumable; // 是否可消耗
    public bool isInstantiate; // 物品实体化
}

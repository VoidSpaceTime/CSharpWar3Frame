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

/// <summary>
/// 物品到主动 companion ability 的唯一链接，source 为物品，target 为 companion。
/// </summary>
public struct ItemActiveAbility : ILinkComponent
{
    public Entity ability;

    public ItemActiveAbility(Entity ability)
    {
        this.ability = ability;
    }

    public Entity GetIndexedValue()
    {
        return ability;
    }
}

/// <summary>
/// 请求通过受控流程销毁物品及其 companion ability。
/// </summary>
public struct ItemDestroyRequest : IComponent
{
    public Entity item;
}

/// <summary>
/// 标记物品正在等待 companion 引用释放，期间拒绝新的使用请求。
/// </summary>
public struct ItemDestroyPendingTag : ITag
{
}

public struct ItemSlotIndex : IComponent
{
    public int index;
}

/// <summary>
/// 物品槽容器。
/// </summary>
public struct ItemSlotContainer : IComponent
{
    public int maxSlots;
    public int currentCount;
}

/// <summary>
/// 物品挂载请求。
/// </summary>
public struct ItemAttachRequest : IComponent
{
    public Entity owner;
    public Entity item;
    public int slotIndex;
}

/// <summary>
/// 物品移除请求。
/// </summary>
public struct ItemRemoveRequest : IComponent
{
    public Entity owner;
    public int slotIndex;
    public bool dropToGround;
    public float x;
    public float y;
    public float z;
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
public struct ItemAttrApplyRequest : IComponent
{
}

/// <summary>
/// 物品属性移除请求。
/// </summary>
public struct ItemAttrRemoveRequest : IComponent
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

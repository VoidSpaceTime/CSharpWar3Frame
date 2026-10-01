using Friflo.Engine.ECS;

namespace War3Frame.Components.Item;

/// <summary>确保地面物品具备独立特效表现的一次性请求。</summary>
public struct ItemCreateNativeRequest : IComponent
{
    public Entity item;
    public float x;
    public float y;
    public float z;
    public string model;
}

/// <summary>地面物品的模型配置与独立特效实体引用；物品实体不得挂载 EffectBase。</summary>
public struct ItemGroundVisual : IComponent
{
    public string model;
    public Entity effect;
}

/// <summary>地面物品的拾取半径。</summary>
public struct ItemGroundPickRange : IComponent
{
    public float radius;
}

/// <summary>请求将地面物品挂载到指定使用者槽位的一次性意图。</summary>
public struct ItemPickupRequest : IComponent
{
    public Entity user;
    public Entity item;
    public int slotIndex;
}

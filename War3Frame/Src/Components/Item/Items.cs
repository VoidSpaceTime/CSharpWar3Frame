using Friflo.Engine.ECS;

namespace War3Frame.Components.Item;

/// <summary>
/// 原生物品创建请求。
/// </summary>
public struct ItemCreateNativeRequest : IComponent
{
    public float x;
    public float y;
    public float facing;
    public int itemTypeId;
}

using Friflo.Engine.ECS;

namespace War3Frame.Components.Attribute;

public struct ManaAttr : IComponent
{
    public float current;
}

public struct ManaNativeDirty : ITag;
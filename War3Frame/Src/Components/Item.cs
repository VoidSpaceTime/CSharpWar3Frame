using Friflo.Engine.ECS;

namespace War3Frame.Components;

public struct ItemOwner : IComponent
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
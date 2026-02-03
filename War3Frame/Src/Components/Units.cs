using Friflo.Engine.ECS;

namespace War3Frame;

public struct Dead : ITag
{
}

public struct RealDead : ITag
{
}

public struct UnitOwner : ILinkComponent
{
    public Entity GetIndexedValue()
    {
        return player;
    }

    public UnitOwner(Entity player)
    {
        this.player = player;
    }

    public Entity player;
}

public struct UnitNative : IComponent
{
    public JUnit unit;
    public JPlayer player;
}

public struct UnitState : IComponent
{
    public bool isAlive;
    public float rebornTime;
}
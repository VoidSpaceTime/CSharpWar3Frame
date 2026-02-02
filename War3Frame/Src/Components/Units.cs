using Friflo.Engine.ECS;

namespace War3Frame;

public struct Dead : ITag
{
}

public struct RealDead : ITag
{
}

public struct UnitOwner : IComponent
{
    public Entity GetRelationKey()
    {
        return player;
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
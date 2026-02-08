using Friflo.Engine.ECS;

namespace War3Frame;

public struct UnitDeadTag : ITag;

public struct UnitFalseDeadTag : ITag;

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

public struct UnitNativeDirty : IComponent
{
    public UnitNativeDirtyFlags flags;
}
[Flags]
public enum UnitNativeDirtyFlags
{
    None = 0,
    Health = 1 << 0,
    Mana = 1 << 1,
    Poison = 1 << 2,
    Move = 1 << 3,
    Death = 1 << 4,
}
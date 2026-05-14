using Friflo.Engine.ECS;

namespace War3Frame;

public enum PlayerState
{
    Playing,
    Leave,
    Empty,
    Computer
}

public enum PlayerTeamState
{
    Allie,
    Enemy,
    Neutral
}

public struct PlayerNative : IComponent
{
    public string name;
    public int color;
    public JPlayer player;
    public int index;
    public Entity getentity;
}

public struct PlayerNameNativeRequest : IComponent
{
    public string name;
}

public struct PlayerColorNativeRequest : IComponent
{
    public int color;
}

public struct PlayerAllianceNativeRequest : IComponent
{
    public Entity target;
    public PlayerAllianceNativeKind kind;
    public bool flag;
}

public enum PlayerAllianceNativeKind
{
    BasicAlliance,
    Vision,
    Control,
    FullControl,
    Neutral
}

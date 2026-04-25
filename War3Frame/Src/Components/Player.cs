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
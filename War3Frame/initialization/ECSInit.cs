using Friflo.Engine.ECS;

namespace War3Frame;

public static partial class Game
{
    public static TimedSystemRoot Root { get; private set; }
    public static EntityStore Store { get; private set; } = new EntityStore();

    public static void ECSInit()
    {
        Root = new TimedSystemRoot(Store);
    }
}
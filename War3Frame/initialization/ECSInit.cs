using Friflo.Engine.ECS;

namespace War3Frame;

public static partial class Game
{
    public static TimedSystemRoot Root { get; private set; }
    public static EntityStore Store { get; private set; }

    public static void ECSInit()
    {
        Store = new EntityStore();
        Root = new TimedSystemRoot(Store);
    }
}
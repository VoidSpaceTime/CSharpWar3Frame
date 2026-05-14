using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace War3Frame;

public static partial class Game
{
    public static EntityStore Store { get; private set; } = new EntityStore();
    public static TimedSystemRoot Root { get; private set; }
    public static float DefaultCorpseDuration { get; set; } = 3f;

    static partial void RegisterGeneratedSystems();

    public static void ECSInit()
    {
        Root = new TimedSystemRoot(Store);
        RegisterGeneratedSystems();
    }
}

using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace War3Frame;

public static partial class Game
{
    public static EntityStore Store { get; private set; } = new EntityStore();
    public static TimedSystemRoot Root { get; private set; }
    public static SystemRoot ImmediateRoot { get; private set; }

    static partial void RegisterGeneratedSystems();

    public static void ECSInit()
    {
        Root = new TimedSystemRoot(Store);
        ImmediateRoot = new SystemRoot(Store);
        // 注册system
        RegisterGeneratedSystems();
    }
}
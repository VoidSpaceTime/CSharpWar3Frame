using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace War3Frame;

public static partial class Game
{
    public static EntityStore Store { get; private set; } = new EntityStore();
    public static TimedSystemRoot Root { get; private set; }
    public static SystemRoot ImmediateRoot { get; private set; }
    public static float DefaultCorpseDuration { get; set; } = 3f;

    static partial void RegisterGeneratedSystems();

    public static void ECSInit()
    {
        Root = new TimedSystemRoot(Store);
        ImmediateRoot = new SystemRoot(Store);
        // 注册system
        RegisterGeneratedSystems();
    }

    /// <summary>
    /// 立即刷新 ImmediateRoot 下的系统
    /// </summary>
    public static void FlushImmediateSystems()
    {
        ImmediateRoot?.Update(default);
    }
}

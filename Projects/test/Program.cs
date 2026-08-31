extern alias War3FrameRuntime;

using System.Runtime.InteropServices;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Scripts.Process;
using RuntimeGame = War3FrameRuntime::War3Frame.Game;

namespace War3Frame;

/// <summary>
/// War3 测试客户端入口，负责初始化框架并驱动统一 ECS 时钟。
/// </summary>
public static partial class Game
{
    public const float TickInterval = 0.01f;
    private static float _elapsed;

    /// <summary>
    /// Native AOT 导出入口。
    /// </summary>
    [UnmanagedCallersOnly(EntryPoint = "main")]
    public static int AotMain()
    {
        Main(true);
        return 0;
    }

    /// <summary>
    /// C++/CLI bridge 调用的托管入口。
    /// </summary>
    public static int BridgeMain()
    {
        Main(false);
        return 0;
    }

    /// <summary>
    /// 初始化框架与 Item companion ability 客户端验证场景。
    /// </summary>
    public static void Main(bool isAot)
    {
        War3.EnableConsole();
        Console.WriteLine($"War3 test client started. isAot: {isAot}");

        RuntimeGame.ECSInit();
        RuntimeGame.Root.SetMonitorPerf(true);

        ItemCompanionAbilityValidationScenario.Initialize(JassApi.Player(0));
        ControlStateValidationScenario.Initialize(JassApi.Player(0));

        var timer = War3.CallNative<int>(War3.GetNativeFunction("CreateTimer"));
        War3.CallNative<int>(War3.GetNativeFunction("TimerStart"), timer, TickInterval, true, () =>
        {
            RuntimeGame.Root.Update(new UpdateTick(TickInterval, _elapsed));
            ItemCompanionAbilityValidationScenario.Update();
            _elapsed += TickInterval;
        });
    }
}

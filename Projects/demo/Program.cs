using System.Runtime.InteropServices;
using Friflo.Engine.ECS;

namespace War3Frame;

/// <summary>demo 与新建项目共用的 AOT/JIT 启动入口。</summary>
public static class Bootstrap
{
    [UnmanagedCallersOnly(EntryPoint = "main")]
    public static int AotMain() => Main(true);

    // BridgeToJIT 固定调用此签名；程序集名称由 demo.csproj 固定为 project。
    public static int BridgeMain() => Main(false);

    private static int Main(bool isAot)
    {
        War3.EnableConsole();
        Game.ECSInit();
        global::War3Frame.Generated.ProjectTemplateRegistration.Initialize();
        Console.WriteLine($"War3 demo started. isAot: {isAot}");
        var elapsed = 0f;
        const float interval = .01f;
        var timer = War3.CallNative<int>(War3.GetNativeFunction("CreateTimer"));
        War3.CallNative<int>(War3.GetNativeFunction("TimerStart"), timer, interval, true, () =>
        {
            elapsed += interval;
            Game.Root.Update(new UpdateTick(interval, elapsed));
        });
        return 0;
    }
}

using War3Frame.Initialization;
using War3Frame.Systems.Native;

namespace War3Frame;

public static partial class Game
{
    public const float TICK_RATE = 0.01f;
    public static float TimeSpan;

    public static void War3Init()
    {
        var players = War3NativeBootstrap.CreatePlayers(Store, 16);
        // 初始化玩家
        PlayerHelper.InitializePlayers(ref players);
        // 注册原生玩家单位事件桥（攻击事件等 → ECS 事件实体）
        NativePlayerEventBridge.Initialize();
        //启动中心计时器
        War3NativeBootstrap.StartMainTimer(TICK_RATE, tick =>
        {
            Root.Update(tick);
            TimeSpan += TICK_RATE;
        });
    }
}
using War3Frame.Systems.Native;

namespace War3Frame;

public static partial class Game
{
    public const float TICK_RATE = 0.01f;
    public static float TimeSpan;

    public static void War3Init()
    {
        var players = War3NativeBootstrap.CreatePlayers(Store, 16);
        PlayerHelper.InitializePlayers(ref players);

        War3NativeBootstrap.StartMainTimer(TICK_RATE, tick =>
        {
            Root.Update(tick);
            TimeSpan += TICK_RATE;
        });
    }
}

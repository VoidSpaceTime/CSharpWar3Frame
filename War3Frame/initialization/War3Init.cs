using System.Numerics;
using Friflo.Engine.ECS;

namespace War3Frame;

public static partial class Game
{
    public const float TICK_RATE = 0.01f; // War3 中心计时器频率
    public static float TimeSpan;
    public static void War3Init()
    {
        // 初始化玩家关系管理器
        var players = new PlayerNative[16];
        for (var i = 0; i < 16; i++)
        {
            var entity = Store.CreateEntity();
            var player = JassApi.Player(i);
            players[i] = new PlayerNative
            {
                player = player,
                color = i,
                name = JassApi.GetPlayerName(player),
                index = i,
                getentity = entity
            };
            entity.AddComponent(players[i]);
        }

        // 默认所有玩家全部相互敌对
        PlayerHelper.InitializePlayers();

        #region 计时器

        // 中心计时器
        var func = War3.GetNativeFunction("CreateTimer");
        var t = War3.CallNative<int>(func);
        Console.WriteLine($"timer = {t}");
        func = War3.GetNativeFunction("TimerStart");


        War3.CallNative<int>(func, t, TICK_RATE, true, () =>
        {
            Root.Update(new UpdateTick(TICK_RATE, TimeSpan));
            TimeSpan += TICK_RATE;
        });

        #endregion
    }
}
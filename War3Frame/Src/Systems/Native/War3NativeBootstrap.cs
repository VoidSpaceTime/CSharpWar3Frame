using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace War3Frame.Systems.Native;

public static class War3NativeBootstrap
{
    /// <summary>
    /// 初始化玩家
    /// </summary>
    /// <param name="store"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static PlayerNative[] CreatePlayers(EntityStore store, int count)
    {
        var players = new PlayerNative[count];

        for (var i = 0; i < count; i++)
        {
            var entity = store.CreateEntity();
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

        return players;
    }

    /// <summary>
    /// 启动中心计时器
    /// </summary>
    /// <param name="tickRate"></param>
    /// <param name="onTick"></param>
    public static void StartMainTimer(float tickRate, Action<UpdateTick> onTick)
    {
        var createTimer = War3.GetNativeFunction("CreateTimer");
        var timer = War3.CallNative<int>(createTimer);
        Console.WriteLine($"timer = {timer}");

        var timerStart = War3.GetNativeFunction("TimerStart");
        War3.CallNative<int>(timerStart, timer, tickRate, true,
            () => { onTick(new UpdateTick(tickRate, Game.TimeSpan)); });
    }
}
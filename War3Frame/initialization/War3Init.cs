namespace War3Frame;

public static partial class Game
{
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
        PlayerHelper.InitializePlayers(ref players);
    }
}
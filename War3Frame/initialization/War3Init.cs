using Friflo.Engine.ECS;
using System;
using System.Collections.Generic;
using System.Text;

namespace War3Frame
{
    public static partial class Game
    {

        public static void War3Init()
        {
            // 初始化玩家关系管理器
            PlayerNative[] players = new PlayerNative[16];
            for (int i = 0; i < 16; i++)
            {
                var entity = Store.CreateEntity();
                players[i] = new PlayerNative
                {
                    player = JassApi.Player(i),
                    index = i,
                    getentity = entity,
                };
                entity.AddComponent<PlayerNative>(players[i]);
            }
            PlayerHelper.InitializePlayers(players);
        }

    }
}

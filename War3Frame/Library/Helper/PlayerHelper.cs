using Friflo.Engine.ECS;
using System;
using System.Collections.Generic;
using System.Text;

namespace War3Frame
{
    public static class PlayerHelper
    {
        /// <summary>
        /// 全局玩家关系管理器 - 用二维数组存储所有玩家间的关系
        /// </summary>

        private const int MaxPlayers = 16; // 魔兽经典最大玩家数
        private static PlayerTeamState[,] _relations = new PlayerTeamState[MaxPlayers, MaxPlayers];
        public static PlayerNative[] Players { get; private set; }


        public static void InitializePlayers(PlayerNative[] players)
        {
            Players = players;
        }

        /// <summary>
        /// 设置单向关系 (玩家A对玩家B的态度)
        /// </summary>
        public static void SetRelation(int playerA, int playerB, PlayerTeamState state)
        {
            _relations[playerA, playerB] = state;
        }

        /// <summary>
        /// 设置双向结盟 (互相都是盟友)
        /// </summary>
        public static void SetAlliance(int playerA, int playerB, bool allied)
        {
            var state = allied ? PlayerTeamState.Allie : PlayerTeamState.Enemy;
            _relations[playerA, playerB] = state;
            _relations[playerB, playerA] = state;
        }

        /// <summary>
        /// 查询关系
        /// </summary>
        public static PlayerTeamState GetRelation(int playerA, int playerB)
        {
            return _relations[playerA, playerB];
        }

        /// <summary>
        /// 判断是否是盟友 (双向都必须是Allie才算真正的盟友)
        /// </summary>
        public static bool IsAlly(int playerA, int playerB)
        {
            return _relations[playerA, playerB] == PlayerTeamState.Allie
                && _relations[playerB, playerA] == PlayerTeamState.Allie;
        }

        /// <summary>
        /// 判断是否是敌人 (任一方认为对方是敌人就算敌对)
        /// </summary>
        public static bool IsEnemy(int playerA, int playerB)
        {
            return _relations[playerA, playerB] == PlayerTeamState.Enemy
                || _relations[playerB, playerA] == PlayerTeamState.Enemy;
        }
    }
}

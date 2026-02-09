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

        // 使用字段而非属性，支持 ref 访问
        private static PlayerNative[] _players = Array.Empty<PlayerNative>();

        /// <summary>
        /// 通过索引获取玩家的引用（可修改原数据）
        /// </summary>
        public static ref PlayerNative GetPlayer(int index) => ref _players[index];

        /// <summary>
        /// 获取玩家数组的只读访问（如果只是遍历读取）
        /// </summary>
        public static ReadOnlySpan<PlayerNative> Players => _players;


        public static void InitializePlayers(ref PlayerNative[] players)
        {
            _players = players;

            // 默认全敌对
            foreach (var sourcePlayer in _players)
            {
                foreach (var targetPlayer in _players)
                {
                    if (sourcePlayer.index == targetPlayer.index)
                    {
                        _relations[sourcePlayer.index, targetPlayer.index] = PlayerTeamState.Allie;
                    }
                    else
                    {
                        _relations[sourcePlayer.index, targetPlayer.index] = PlayerTeamState.Enemy;
                    }
                }
            }
        }

        public static void SetName(ref PlayerNative player, string name)
        {
            JassApi.SetPlayerName(player.player, name);
            player.name = name;
        }

        public static void SetColor(ref PlayerNative player, int color)
        {
            JassApi.SetPlayerColor(player.player, new JPlayerColor(color));
            player.color = color;
        }

        /// <summary>
        /// 设置单向结盟 (只设置 playerA 对 playerB 的态度，不影响 playerB 对 playerA)
        /// </summary>
        public static void SetAlliance(PlayerNative playerA, PlayerNative playerB, bool allied)
        {
            var state = allied ? PlayerTeamState.Allie : PlayerTeamState.Enemy;
            _relations[playerA.index, playerB.index] = state;
            _relations[playerB.index, playerA.index] = state;
            // 设置不主动攻击
            JassApi.SetPlayerAlliance(playerA.player, playerB.player, new JAllianceType(Blizzard.ALLIANCE_PASSIVE), allied);
            // 设置请求援助
            JassApi.SetPlayerAlliance(playerA.player, playerB.player, new JAllianceType(Blizzard.ALLIANCE_HELP_REQUEST), allied);
            // 设置援助响应
            JassApi.SetPlayerAlliance(playerA.player, playerB.player, new JAllianceType(Blizzard.ALLIANCE_HELP_RESPONSE), allied);
            // 设置共享法术
            JassApi.SetPlayerAlliance(playerA.player, playerB.player, new JAllianceType(Blizzard.ALLIANCE_SHARED_SPELLS), allied);
        }
        #region 废弃代码
        /*        /// <summary>
               /// 设置双向结盟 (互相都是盟友)
               /// </summary>
               public static void SetTwoWayAlliance(PlayerNative playerA, PlayerNative playerB, bool allied)
               {
                   var state = allied ? PlayerTeamState.Allie : PlayerTeamState.Enemy;
                   _relations[playerA.index, playerB.index] = state;
                   _relations[playerB.index, playerA.index] = state;
                   // 设置不主动攻击
                   JassApi.SetPlayerAlliance(playerA.player, playerB.player, new JAllianceType(Blizzard.ALLIANCE_PASSIVE), allied);
                   // 设置请求援助
                   JassApi.SetPlayerAlliance(playerA.player, playerB.player, new JAllianceType(Blizzard.ALLIANCE_HELP_REQUEST), allied);
                   // 设置援助响应
                   JassApi.SetPlayerAlliance(playerA.player, playerB.player, new JAllianceType(Blizzard.ALLIANCE_HELP_RESPONSE), allied);
                   // 设置共享经验
                   // JassApi.SetPlayerAlliance(playerA.player, playerB.player, new JAllianceType(Blizzard.ALLIANCE_SHARED_XP), allied);
                   // 设置共享法术
                   JassApi.SetPlayerAlliance(playerA.player, playerB.player, new JAllianceType(Blizzard.ALLIANCE_SHARED_SPELLS), allied);

                   // 设置不主动攻击
                   JassApi.SetPlayerAlliance(playerB.player, playerA.player, new JAllianceType(Blizzard.ALLIANCE_PASSIVE), allied);
                   // 设置请求援助
                   JassApi.SetPlayerAlliance(playerB.player, playerA.player, new JAllianceType(Blizzard.ALLIANCE_HELP_REQUEST), allied);
                   // 设置援助响应
                   JassApi.SetPlayerAlliance(playerB.player, playerA.player, new JAllianceType(Blizzard.ALLIANCE_HELP_RESPONSE), allied);
                   // 设置共享经验
                   // JassApi.SetPlayerAlliance(playerB.player, playerA.player, new JAllianceType(Blizzard.ALLIANCE_SHARED_XP), allied);
                   // 设置共享法术
                   JassApi.SetPlayerAlliance(playerB.player, playerA.player, new JAllianceType(Blizzard.ALLIANCE_SHARED_SPELLS), allied);
               } */
        #endregion
        /// <summary>
        /// 设置视野共享
        /// </summary>
        /// <param name="playerA"></param>
        /// <param name="playerB"></param>
        /// <param name="flag"></param>
        public static void SetVision(PlayerNative playerA, PlayerNative playerB, bool flag)
        {
            JassApi.SetPlayerAlliance(playerA.player, playerB.player, new JAllianceType(Blizzard.ALLIANCE_SHARED_VISION), flag);
        }
        /// <summary>
        /// 设置可控制
        /// </summary>
        /// <param name="playerA"></param>
        /// <param name="playerB"></param>
        /// <param name="flag"></param>
        public static void SetControl(PlayerNative playerA, PlayerNative playerB, bool flag)
        {
            JassApi.SetPlayerAlliance(playerA.player, playerB.player, new JAllianceType(Blizzard.ALLIANCE_SHARED_CONTROL), flag);
        }
        /// <summary>
        /// 完全控制
        /// </summary>
        /// <param name="playerA"></param>
        /// <param name="playerB"></param>
        /// <param name="flag"></param>
        public static void SetFullControl(PlayerNative playerA, PlayerNative playerB, bool flag)
        {
            JassApi.SetPlayerAlliance(playerA.player, playerB.player, new JAllianceType(Blizzard.ALLIANCE_SHARED_ADVANCED_CONTROL), flag);

        }

        /// <summary>
        /// 设置中立
        /// </summary>
        /// <param name="playerA"></param>
        /// <param name="playerB"></param>
        /// <param name="flag"></param>
        public static void SetNeutral(PlayerNative playerA, PlayerNative playerB, bool flag)
        {
            _relations[playerA.index, playerB.index] = PlayerTeamState.Neutral;
            JassApi.SetPlayerAlliance(playerA.player, playerB.player, new JAllianceType(Blizzard.ALLIANCE_PASSIVE), flag);
        }

        /// <summary>
        /// 查询关系
        /// </summary>
        public static PlayerTeamState GetRelation(PlayerNative playerA, PlayerNative playerB)
        {
            return _relations[playerA.index, playerB.index];
        }

        /// <summary>
        /// 判断是否是盟友  A->B
        /// </summary>
        public static bool IsAlly(PlayerNative playerA, PlayerNative playerB)
        {
            return _relations[playerA.index, playerB.index] == PlayerTeamState.Allie;
        }

        /// <summary>
        /// 判断是否是敌人 A->B
        /// </summary>
        public static bool IsEnemy(PlayerNative playerA, PlayerNative playerB)
        {
            return _relations[playerA.index, playerB.index] == PlayerTeamState.Enemy;
        }
    }
}

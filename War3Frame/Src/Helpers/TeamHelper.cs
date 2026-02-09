using System;
using System.Collections.Generic;
using System.Text;

namespace War3Frame.Library.Helper
{
    public static class TeamHelper
    {
        public static SortedDictionary<string, List<int>> Teams = new();
        public static void CreateTeam(string teamName, int color, bool nameSync, bool colorSync, List<int> members)
        {
            if (!Teams.ContainsKey(teamName))
            {
                Teams[teamName] = new List<int>();
            }
            Teams[teamName].AddRange(members);

            foreach (var playerA in members)
            {
                ref PlayerNative pla = ref PlayerHelper.GetPlayer(playerA);
                if (colorSync)
                {
                    PlayerHelper.SetColor(ref pla, color);
                }
                if (nameSync)
                {
                    PlayerHelper.SetName(ref pla, teamName);

                }
                foreach (var playerB in members)
                {
                    var plb = PlayerHelper.GetPlayer(playerB);
                    if (playerA != playerB)
                    {
                        PlayerHelper.SetAlliance(pla, plb, true);
                        PlayerHelper.SetVision(pla, plb, true);
                        PlayerHelper.SetControl(pla, plb, false);
                        PlayerHelper.SetFullControl(pla, plb, false);
                    }
                }
            }

        }

        public static bool TeamIs(int playerIndex, string teamName)
        {
            if (Teams.ContainsKey(teamName))
            {
                return Teams[teamName].Contains(playerIndex);
            }
            return false;
        }

        // 默认队伍初始化后不进行修改, 修改通过结盟自行修改
        /*     public static void RemovePlayer(int playerIndex, string teamName)
        {
            if (Teams.ContainsKey(teamName))
            {
                var pla = PlayerHelper.GetPlayer(playerIndex);
                Teams[teamName].Remove(playerIndex);
                foreach (var item in Teams[teamName])
                {
                    var plb = PlayerHelper.GetPlayer(item);
                    PlayerHelper.SetAlliance(pla, plb, true);
                    PlayerHelper.SetVision(pla, plb, true);
                }
            }
        }
        public static void AddTeam(int playerIndex, string teamName)
        {
            if (Teams.ContainsKey(teamName))
            {
                Teams[teamName].Add(playerIndex);
                var pla = PlayerHelper.GetPlayer(playerIndex);
                Teams[teamName].Remove(playerIndex);
                foreach (var item in Teams[teamName])
                {
                    var plb = PlayerHelper.GetPlayer(item);
                    PlayerHelper.SetAlliance(pla, plb, true);
                    PlayerHelper.SetVision(pla, plb, true);
                    PlayerHelper.SetControl(pla, plb, false);
                    PlayerHelper.SetFullControl(pla, plb, false);
                }
            }
        }*/

    }
}

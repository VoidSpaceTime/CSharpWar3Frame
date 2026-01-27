using Friflo.Engine.ECS;
using System;
using System.Collections.Generic;
using System.Text;

namespace War3Frame
{
    public enum PlayerState
    {
        Playing,
        Leave,
        Empty,
        Computer,

    }
    public enum PlayerTeamState
    {
        Allie,
        Enemy,
        Neutral,
    }
    public struct PlayerNative : IComponent
    {
        public string name;
        public int color;
        public JPlayer player;
        public int index;
        public Entity getentity;
    }
}

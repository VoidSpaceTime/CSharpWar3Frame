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
    public struct Player
    {
        public JPlayer player;
    }
}

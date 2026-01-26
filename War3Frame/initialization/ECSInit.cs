using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace War3Frame
{
    public static partial class Game
    {
        public static TimedSystemRoot Root { get; private set; }
        public static EntityStore Store { get; private set; }

        public static void ECSInit()
        {
            Store = new EntityStore();
            Root = new TimedSystemRoot(Store);
        }
    }
}

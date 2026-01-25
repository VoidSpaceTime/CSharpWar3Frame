using Friflo.Engine.ECS;
using System;
using System.Collections.Generic;
using System.Text;

namespace War3Frame.Src.Components
{
    public struct MissileBase : IComponent
    {
        public Position start;
        public Position end;
        public Entity target;
        public float speed;
        public float acceleration;

    }
    public struct MissilMode : IComponent
    {
        public string model;
        public float animateScale;
        public float scale;
    }
}

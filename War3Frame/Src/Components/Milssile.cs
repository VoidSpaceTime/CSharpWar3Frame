using Friflo.Engine.ECS;

namespace War3Frame.Src.Components;

public struct MissileBase : IComponent
{
    public Position start;
    public Position end;
    public Entity target;
    public float speed;
    public float acceleration;
}

public struct MissileMode : IComponent
{
    public string model;
    public float animateScale;
    public float scale;
}
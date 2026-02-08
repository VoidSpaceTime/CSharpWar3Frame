using System.Numerics;
using Friflo.Engine.ECS;

namespace War3Frame;

public enum EffectType
{
    Position,
    Attach
}
public enum EffectAttachType
{
    Head,
    Origin,
    Weapon,
    Chest,
}
public struct EffectNative : IComponent
{
    public JEffect effect;
}

public struct EffectBase : IComponent
{
    public string model;
    public int teamColor;
    public float sizeScale;
    public float speed;
    public bool visible;
    public int alpha;
    public float duration;
    public int red;
    public int green;
    public int blue;
    public EffectType effectType;
    public EffectAttachType effectAttachType;

}

// 脏标记
[Flags]
public enum EffectDirtyFlags
{
    None = 0,
    Color = 1 << 0,
    Scale = 1 << 1,
    Speed = 1 << 2,
    Visible = 1 << 3,
    Alpha = 1 << 4,
    TeamColor = 1 << 5,
}
public struct EffectDirty : IComponent
{
    public EffectDirtyFlags flags;
}

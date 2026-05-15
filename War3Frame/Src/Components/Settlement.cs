using Friflo.Engine.ECS;
using War3Frame.Components;

namespace War3Frame.Src.Components;

public struct HealRequest : IComponent
{
    public Entity source;
    public Entity target;
    public float amount;
}

public struct HealEvent : IComponent
{
    public Entity source;
    public Entity target;
    public float baseHeal;
    public float finalHeal;
    public float remainingHealth;
}

public struct BuffApplyRequest : IComponent
{
    public Entity source;
    public Entity target;
    public string buffId;
    public float duration;
    public int attrTypeId;
    public ModifyType modifyType;
    public float value;
    public BuffRefreshBehavior refreshBehavior;
}

public struct BuffAppliedEvent : IComponent
{
    public Entity source;
    public Entity target;
    public Entity buff;
    public string buffId;
}

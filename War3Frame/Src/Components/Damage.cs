using Friflo.Engine.ECS;

namespace War3Frame.Src.Components;

public enum DamageType
{
    Physical,
    Magical,
    Real
}

public enum DamageSrc
{
    Melee,
    Ranged,
    Skill
}

public struct DamageBase
{
    public float damage;
    public DamageType damageType;
    public DamageSrc damageSrc;
    public Entity source;
    public Entity target;
}

/// <summary>
/// Input command asking the combat pipeline to apply damage.
/// </summary>
public struct DamageRequest : IComponent
{
    public DamageBase damage;
    public Entity source;
    public Entity target;
}

/// <summary>
/// Result event emitted after damage has been resolved.
/// </summary>
public struct DamageEvent : IComponent
{
    public DamageBase damage;
    public float finalDamage;
    public float remainingHealth;
    public Entity source;
    public Entity target;
}

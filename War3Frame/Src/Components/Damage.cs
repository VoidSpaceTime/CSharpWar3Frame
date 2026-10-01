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

    /// <summary>计算器 key：0 = 按 damageType 取默认计算器；&gt;0 = 显式自定义计算器（DamageCalculatorRegistry）。</summary>
    public int calcKey;
}

/// <summary>
/// Input command asking the combat pipeline to apply damage.
/// </summary>
public struct DamageRequest : IComponent
{
    public DamageBase damage;
    public Entity source;
    public Entity target;

    /// <summary>跳过 Event 后处理（吸血/反射及外部钩子）。用于反射伤害，避免互反射与二次吸血。</summary>
    public bool skipPostProcess;
}

/// <summary>
/// 一次伤害结算的流转上下文（结算管线内使用，非 ECS 组件）。
/// </summary>
public struct DamageContext
{
    public Entity source;
    public Entity target;
    public DamageType damageType;
    public DamageSrc damageSrc;

    /// <summary>进入计算器前的初始值（管线不做减法，语义不变）。</summary>
    public float baseDamage;

    /// <summary>减免前（暴击后）伤害。</summary>
    public float preMitigation;

    /// <summary>减免量。</summary>
    public float mitigation;

    /// <summary>实际扣血值：护盾吸收与减免之后的剩余，用于写入生命。</summary>
    public float finalDamage;

    /// <summary>护盾吸收量（内置护盾处理器在扣血前写入）。</summary>
    public float absorbed;

    /// <summary>本次是否暴击。</summary>
    public bool isCrit;

    /// <summary>是否被免疫拦截（无敌压制，伤害为 0）。</summary>
    public bool isImmune;
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

    /// <summary>本次是否暴击（计算器内确定性 roll 判定）。</summary>
    public bool isCrit;

    /// <summary>减免量：preMitigation（暴击后）- finalDamage（含护盾吸收与减伤）。</summary>
    public float mitigatedAmount;

    /// <summary>是否被免疫拦截（无敌）：伤害结算为 0，HP 不变。</summary>
    public bool isImmune;

    /// <summary>护盾吸收量（本次实际被伤害护盾吸收的部分，不并入 mitigatedAmount）。</summary>
    public float absorbed;
}

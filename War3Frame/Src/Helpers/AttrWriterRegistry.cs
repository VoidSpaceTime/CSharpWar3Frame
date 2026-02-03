using Friflo.Engine.ECS;

namespace War3Frame;

/// <summary>
///     属性写入器接口 - 定义如何从 BaseAttrs 读取基础值，写入到目标组件
/// </summary>
public interface IAttrWriter
{
    /// <summary>处理的属性类型</summary>
    AttrType AttrType { get; }

    /// <summary>从 BaseAttrs 读取基础值</summary>
    float ReadBase(Entity entity);

    /// <summary>将计算后的值写入到目标组件</summary>
    void Write(Entity entity, float value);
}

/// <summary>
///     属性写入器注册表 - 管理所有属性写入器
///     地图层可以注册自定义属性写入器
/// </summary>
public static class AttrWriterRegistry
{
    private static readonly Dictionary<AttrType, IAttrWriter> _writers = new();

    static AttrWriterRegistry()
    {
        // 注册框架内置的属性写入器
        RegisterDefaults();
    }

    /// <summary>注册内置属性写入器</summary>
    private static void RegisterDefaults()
    {
        Register(new MaxHealthWriter());
        Register(new HealthRegenWriter());
        Register(new MaxManaWriter());
        Register(new ManaRegenWriter());
        Register(new DamageWriter());
        Register(new AttackSpeedWriter());
        Register(new AttackRangeWriter());
        Register(new ArmorWriter());
        Register(new MagicResistWriter());
        Register(new MoveSpeedWriter());
        Register(new SightRangeWriter());
        Register(new CritChanceWriter());
        Register(new CritMultiplierWriter());
    }

    /// <summary>
    ///     注册自定义属性写入器（地图层调用）
    /// </summary>
    public static void Register(IAttrWriter writer)
    {
        _writers[writer.AttrType] = writer;
    }

    /// <summary>
    ///     获取所有已注册的写入器
    /// </summary>
    public static IEnumerable<IAttrWriter> GetAllWriters() => _writers.Values;

    /// <summary>
    ///     获取指定属性类型的写入器
    /// </summary>
    public static IAttrWriter? GetWriter(AttrType attrType)
    {
        return _writers.TryGetValue(attrType, out var writer) ? writer : null;
    }
}

#region 内置属性写入器

// ============ 生命相关 ============

public class MaxHealthWriter : IAttrWriter
{
    public AttrType AttrType => AttrType.MaxHealth;

    public float ReadBase(Entity entity)
    {
        return entity.TryGetComponent<BaseAttrs>(out var attrs) ? attrs.maxHealth : 0;
    }

    public void Write(Entity entity, float value)
    {
        if (!entity.TryGetComponent<Health>(out var health)) return;

        var oldMax = health.max;
        health.max = value;

        // 最大值增加时，当前值也增加
        if (value > oldMax && oldMax > 0)
        {
            health.current += (value - oldMax);
        }
        // 确保当前值不超过最大值
        if (health.current > health.max)
        {
            health.current = health.max;
        }

        entity.AddComponent(health);
    }
}

public class HealthRegenWriter : IAttrWriter
{
    public AttrType AttrType => AttrType.HealthRegen;

    public float ReadBase(Entity entity)
    {
        return entity.TryGetComponent<BaseAttrs>(out var attrs) ? attrs.healthRegen : 0;
    }

    public void Write(Entity entity, float value)
    {
        if (!entity.TryGetComponent<Health>(out var health)) return;
        health.regen = value;
        entity.AddComponent(health);
    }
}

// ============ 魔法相关 ============

public class MaxManaWriter : IAttrWriter
{
    public AttrType AttrType => AttrType.MaxMana;

    public float ReadBase(Entity entity)
    {
        return entity.TryGetComponent<BaseAttrs>(out var attrs) ? attrs.maxMana : 0;
    }

    public void Write(Entity entity, float value)
    {
        if (!entity.TryGetComponent<Mana>(out var mana)) return;

        var oldMax = mana.max;
        mana.max = value;

        if (value > oldMax && oldMax > 0)
        {
            mana.current += (value - oldMax);
        }
        if (mana.current > mana.max)
        {
            mana.current = mana.max;
        }

        entity.AddComponent(mana);
    }
}

public class ManaRegenWriter : IAttrWriter
{
    public AttrType AttrType => AttrType.ManaRegen;

    public float ReadBase(Entity entity)
    {
        return entity.TryGetComponent<BaseAttrs>(out var attrs) ? attrs.manaRegen : 0;
    }

    public void Write(Entity entity, float value)
    {
        if (!entity.TryGetComponent<Mana>(out var mana)) return;
        mana.regen = value;
        entity.AddComponent(mana);
    }
}

// ============ 攻击相关 ============

public class DamageWriter : IAttrWriter
{
    public AttrType AttrType => AttrType.Damage;

    public float ReadBase(Entity entity)
    {
        return entity.TryGetComponent<BaseAttrs>(out var attrs) ? attrs.damage : 0;
    }

    public void Write(Entity entity, float value)
    {
        if (!entity.TryGetComponent<Attack>(out var attack)) return;
        attack.damage = value;
        entity.AddComponent(attack);
    }
}

public class AttackSpeedWriter : IAttrWriter
{
    public AttrType AttrType => AttrType.AttackSpeed;

    public float ReadBase(Entity entity)
    {
        return entity.TryGetComponent<BaseAttrs>(out var attrs) ? attrs.attackSpeed : 0;
    }

    public void Write(Entity entity, float value)
    {
        if (!entity.TryGetComponent<Attack>(out var attack)) return;
        attack.attackSpeed = value;
        entity.AddComponent(attack);
    }
}

public class AttackRangeWriter : IAttrWriter
{
    public AttrType AttrType => AttrType.AttackRange;

    public float ReadBase(Entity entity)
    {
        return entity.TryGetComponent<BaseAttrs>(out var attrs) ? attrs.attackRange : 0;
    }

    public void Write(Entity entity, float value)
    {
        if (!entity.TryGetComponent<Attack>(out var attack)) return;
        attack.range = value;
        entity.AddComponent(attack);
    }
}

// ============ 防御相关 ============

public class ArmorWriter : IAttrWriter
{
    public AttrType AttrType => AttrType.Armor;

    public float ReadBase(Entity entity)
    {
        return entity.TryGetComponent<BaseAttrs>(out var attrs) ? attrs.armor : 0;
    }

    public void Write(Entity entity, float value)
    {
        if (!entity.TryGetComponent<Defend>(out var defend)) return;
        defend.armor = value;
        entity.AddComponent(defend);
    }
}

public class MagicResistWriter : IAttrWriter
{
    public AttrType AttrType => AttrType.MagicResist;

    public float ReadBase(Entity entity)
    {
        return entity.TryGetComponent<BaseAttrs>(out var attrs) ? attrs.magicResist : 0;
    }

    public void Write(Entity entity, float value)
    {
        if (!entity.TryGetComponent<MagicResist>(out var mr)) return;
        mr.magicResist = value;
        entity.AddComponent(mr);
    }
}

// ============ 移动相关 ============

public class MoveSpeedWriter : IAttrWriter
{
    public AttrType AttrType => AttrType.MoveSpeed;

    public float ReadBase(Entity entity)
    {
        return entity.TryGetComponent<BaseAttrs>(out var attrs) ? attrs.moveSpeed : 0;
    }

    public void Write(Entity entity, float value)
    {
        if (!entity.TryGetComponent<Move>(out var move)) return;
        move.speed = value;
        entity.AddComponent(move);
    }
}

// ============ 视野相关 ============

public class SightRangeWriter : IAttrWriter
{
    public AttrType AttrType => AttrType.SightRange;

    public float ReadBase(Entity entity)
    {
        return entity.TryGetComponent<BaseAttrs>(out var attrs) ? attrs.sightRange : 0;
    }

    public void Write(Entity entity, float value)
    {
        if (!entity.TryGetComponent<Sight>(out var sight)) return;
        sight.sightRange = value;
        entity.AddComponent(sight);
    }
}

// ============ 暴击相关 ============

public class CritChanceWriter : IAttrWriter
{
    public AttrType AttrType => AttrType.CritChance;

    public float ReadBase(Entity entity)
    {
        return entity.TryGetComponent<BaseAttrs>(out var attrs) ? attrs.critChance : 0;
    }

    public void Write(Entity entity, float value)
    {
        if (!entity.TryGetComponent<Crit>(out var crit)) return;
        crit.chance = value;
        entity.AddComponent(crit);
    }
}

public class CritMultiplierWriter : IAttrWriter
{
    public AttrType AttrType => AttrType.CritMultiplier;

    public float ReadBase(Entity entity)
    {
        return entity.TryGetComponent<BaseAttrs>(out var attrs) ? attrs.critMultiplier : 0;
    }

    public void Write(Entity entity, float value)
    {
        if (!entity.TryGetComponent<Crit>(out var crit)) return;
        crit.multiplier = value;
        entity.AddComponent(crit);
    }
}

#endregion

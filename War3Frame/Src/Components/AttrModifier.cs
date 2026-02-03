using Friflo.Engine.ECS;

namespace War3Frame;

#region 枚举定义

/// <summary>
///     属性类型
/// </summary>
public enum AttrType
{
    MaxHealth,      // 最大生命
    HealthRegen,    // 生命回复
    MaxMana,        // 最大魔法
    ManaRegen,      // 魔法回复
    Damage,         // 攻击力
    Armor,          // 护甲
    MagicResist,    // 魔抗
    MoveSpeed,      // 移动速度
    AttackSpeed,    // 攻击速度
    AttackRange,    // 攻击范围
    SightRange,     // 视野范围
    CritChance,     // 暴击率
    CritMultiplier  // 暴击倍率
}

/// <summary>
///     修改类型
/// </summary>
public enum ModifyType
{
    /// <summary>固定加成: +100</summary>
    Flat,

    /// <summary>百分比加成: +10% (加法叠加)</summary>
    PercentAdd,

    /// <summary>最终百分比: *1.1 (乘法叠加)</summary>
    PercentMul
}

/// <summary>
///     修改器来源类型
/// </summary>
public enum ModifierSourceType
{
    Item,       // 物品
    Ability,    // 技能
    Buff,       // Buff/Debuff
    Aura,       // 光环
    Talent,     // 天赋
    Other       // 其他
}

#endregion

#region 组件定义

/// <summary>
///     属性修改器组件 - 每个修改器是独立的 Entity
/// </summary>
public struct AttrModifier : IComponent
{
    /// <summary>修改的属性类型</summary>
    public AttrType attrType;

    /// <summary>修改方式</summary>
    public ModifyType modifyType;

    /// <summary>修改值</summary>
    public float value;

    /// <summary>优先级（用于计算顺序，值越小越先计算）</summary>
    public int priority;

    /// <summary>来源类型</summary>
    public ModifierSourceType sourceType;
}

/// <summary>
///     修改器目标关系 - 指向被修改的单位
/// </summary>
public struct ModifierTarget : ILinkComponent
{
    public Entity GetIndexedValue() => target;

    public Entity target;

    public ModifierTarget(Entity target)
    {
        this.target = target;
    }
}

/// <summary>
///     修改器来源关系 - 指向修改器的来源（物品/技能/Buff）
/// </summary>
public struct ModifierSource : ILinkComponent
{
    public Entity GetIndexedValue() => source;

    public Entity source;

    public ModifierSource(Entity source)
    {
        this.source = source;
    }
}

/// <summary>
///     基础属性组件 - 存储单位原始属性（模板值，不会被修改器改变）
///     修改器系统会读取这里的值，计算后写入到 Health, Attack 等组件
/// </summary>
public struct BaseAttrs : IComponent
{
    // 生命相关
    public float maxHealth;
    public float healthRegen;

    // 魔法相关
    public float maxMana;
    public float manaRegen;

    // 攻击相关
    public float damage;
    public float attackSpeed;
    public float attackRange;

    // 防御相关
    public float armor;
    public float magicResist;

    // 移动相关
    public float moveSpeed;

    // 视野相关
    public float sightRange;
    public float nightSightRange;

    // 暴击相关（可选）
    public float critChance;
    public float critMultiplier;
}

/// <summary>
///     属性脏标记 - 表示需要重新计算
/// </summary>
public struct AttrsDirty : ITag;

#endregion

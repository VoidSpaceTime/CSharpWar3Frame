using Friflo.Engine.ECS;

namespace War3Frame;

// ============================================================================
// 战斗相关属性 ID 注册
// 这些属性会作为独立的 Attribute Entity 存在，支持修改器叠加
// ============================================================================

public static partial class AttributeHelper
{
    // 攻击属性
    public static readonly int AttackDamage = Register("AttackDamage");         // 攻击力
    public static readonly int AttackSpeed = Register("AttackSpeed");           // 攻击速度
    public static readonly int AttackRange = Register("AttackRange");           // 攻击范围
    public static readonly int AttackRangeMin = Register("AttackRangeMin");     // 最小攻击范围
    public static readonly int AttackRangeAcquire = Register("AttackRangeAcquire"); // 主动攻击范围
    public static readonly int AttackInterval = Register("AttackInterval");     // 攻击间隔

    // 防御属性
    public static readonly int Armor = Register("Armor");                       // 护甲
    public static readonly int MagicResist = Register("MagicResist");          // 魔法抗性

    // 暴击属性
    public static readonly int CritChance = Register("CritChance");             // 暴击率
    public static readonly int CritMultiplier = Register("CritMultiplier");     // 暴击倍率

    // 视野属性
    public static readonly int SightRange = Register("SightRange");             // 日间视野
    public static readonly int NightSightRange = Register("NightSightRange");   // 夜间视野

    // 移动属性
    public static readonly int MoveSpeed = Register("MoveSpeed");               // 移动速度
}

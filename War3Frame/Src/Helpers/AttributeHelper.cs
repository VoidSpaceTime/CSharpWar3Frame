using System.Collections.Specialized;

namespace War3Frame;

public static partial class AttributeHelper
{
    private static SortedDictionary<int, string> _types = new();
    private static int _nextId = 0;


    // 框架内置 - 基础属性
    public static readonly int Health = Register("Health");
    public static readonly int HealthRegen = Register("HealthRegen");
    public static readonly int Mana = Register("Mana");
    public static readonly int ManaRegen = Register("ManaRegen");
    public static readonly int Damage = Register("Damage");

    // ============================================================================
    // 控制效果属性（值 > 0 表示效果生效）
    // ============================================================================
    public static readonly int Stun = Register("Stun");             // 眩晕（禁止一切）
    public static readonly int Silence = Register("Silence");       // 沉默（禁止施法）
    public static readonly int Disarm = Register("Disarm");         // 缴械（禁止攻击）
    public static readonly int Root = Register("Root");             // 定身（禁止移动）
    public static readonly int Knockback = Register("Knockback");   // 击飞/击退

    // ============================================================================
    // 免疫属性（值 > 0 可以免疫对应控制）
    // ============================================================================
    public static readonly int StunImmunity = Register("StunImmunity");
    public static readonly int SilenceImmunity = Register("SilenceImmunity");
    public static readonly int DisarmImmunity = Register("DisarmImmunity");
    public static readonly int RootImmunity = Register("RootImmunity");
    public static readonly int KnockbackImmunity = Register("KnockbackImmunity");

    /// <summary>项目层注册新属性类型</summary>
    public static int Register(string name)
    {
        var id = _nextId++;
        _types.Add(id, name);
        return id;
    }

    /// <summary>获取属性名称</summary>
    public static string? GetName(int attrId)
    {
        return _types.TryGetValue(attrId, out var name) ? name : null;
    }
}

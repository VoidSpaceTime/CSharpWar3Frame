using System.Collections.Specialized;

namespace War3Frame;

public static partial class AttributeHelper
{
    private static SortedDictionary<int, string> _types = new();
    private static int _nextId = 0;


    // 框架内置
    public static readonly int Health = Register("Health");
    public static readonly int HealthRegen = Register("HealthRegen");
    public static readonly int Mana = Register("Mana");
    public static readonly int ManaRegen = Register("ManaRegen");
    public static readonly int Damage = Register("Damage");

    /// <summary>项目层注册新属性类型</summary>
    public static int Register(string name)
    {
        var id = _nextId++;
        _types.Add(id, name);
        return id;
    }
}
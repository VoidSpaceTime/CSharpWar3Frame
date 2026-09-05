using System.Collections.Specialized;
using Friflo.Engine.ECS;
using War3Frame.Components;

namespace War3Frame;

public static partial class AttributeHelper
{
    #region MyRegion init

    /// <summary>
    /// 已注册的属性类型名称表。
    /// </summary>
    // 属性 ID -> 名称 的注册表只用于调试、UI 和序列化可读性，不参与运行时计算。
    private static SortedDictionary<int, string> _types = new();

    /// <summary>
    /// 下一个属性类型 ID。
    /// </summary>
    // 递增 ID 便于稳定比较，避免把字符串当作运行时主键。
    private static int _nextId = 0;


    // 框架内置 - 基础属性
    // 核心资源/战斗属性按固定 ID 注册，后续系统直接通过数值访问。
    public static readonly int Health = Register("Health");
    public static readonly int HealthRegen = Register("HealthRegen");
    public static readonly int HealthRegenPercent = Register("HealthRegenPercent");
    public static readonly int Mana = Register("Mana");
    public static readonly int ManaRegen = Register("ManaRegen");
    public static readonly int ManaRegenPercent = Register("ManaRegenPercent");

    // ============================================================================
    // 控制效果属性（值 > 0 表示效果生效）
    // ============================================================================
    public static readonly int Stun = Register("Stun"); // 眩晕（禁止一切）
    public static readonly int Silence = Register("Silence"); // 沉默（禁止施法）
    public static readonly int NoAttack = Register("NoAttack"); // 缴械（禁止攻击）
    public static readonly int Root = Register("Root"); // 定身（禁止移动）
    public static readonly int CrackFly = Register("CrackFly"); // 击飞/击退

    // ============================================================================
    // 免疫属性（值 > 0 可以免疫对应控制）
    // ============================================================================
    // 免疫类属性按同样的 ID 体系登记，便于效果系统统一判断。
    public static readonly int StunImmunity = Register("StunImmunity");
    public static readonly int SilenceImmunity = Register("SilenceImmunity");
    public static readonly int NoAttackImmunity = Register("NoAttackImmunity");
    public static readonly int RootImmunity = Register("RootImmunity");
    public static readonly int CrackFlyImmunity = Register("CrackFlyImmunity");

/// <summary>项目层注册新属性类型</summary>
    // 运行时注册入口：只负责建立可读名称，不做去重，调用方需保证注册顺序稳定。
    public static int Register(string name)
    {
        // partial 类静态字段初始化顺序跨文件未定义（Combat.cs 的字段注册可能先于本文件字段初始化器执行），
        // 此处懒初始化避免 .cctor 阶段 NRE。
        _types ??= new SortedDictionary<int, string>();
        var id = _nextId++;
        _types.Add(id, name);
        return id;
    }

    /// <summary>获取属性名称</summary>
    // 返回名称用于调试和面板展示；找不到时返回 null，调用方自行兜底。
    public static string? GetName(int attrId)
    {
        return _types != null && _types.TryGetValue(attrId, out var name) ? name : null;
    }

    #endregion

    /// <summary>为 Entity 创建属性 Entity 并建立关系</summary>
    // 创建属性实体并反向挂回单位关系，保证属性真相始终在 ECS 中可追踪。
    public static Entity CreateAttr(Entity entity, int typeId, float baseValue)
    {
        // 创建属性 Entity
        var attr = Game.Store.CreateEntity(
            new AttrTypeId { typeId = typeId },
            new AttrValue { baseValue = baseValue, finalValue = baseValue, current = baseValue },
            new AttrOwner(entity)
        );

        // 建立 Entity → Attr 关系
        entity.AddRelation(new HasAttr(attr, typeId));

        return attr;
    }

    /// <summary>获取 Entity 的某个属性 Entity</summary>
    // 从关系表里找具体属性实体，避免每次都扫描整个 Store。
    public static Entity? GetAttr(Entity unit, int typeId)
    {
        var relations = unit.GetRelations<HasAttr>();
        foreach (ref var rel in relations)
        {
            if (rel.typeId == typeId)
                return rel.attrEntity;
        }

        return null;
    }

    /// <summary>
    /// 尝试获取实体的指定属性实体。
    /// </summary>
    // Try 版本用于避免临时分配，适合热路径查询。
    public static bool TryGetAttr(Entity entity, int typeId, out Entity attr)
    {
        var relations = GetAttr(entity, typeId);
        if (relations != null)
        {
            attr = relations.Value;
            return true;
        }

        attr = default;
        return false;
    }

    /// <summary>获取实体的指定属性实体；不存在时自动创建（baseValue 默认 0）。</summary>
    // 供修改器/buff 贡献路径使用：单位模板未声明某属性时，运行时首个贡献会自动建一个 base=0 的属性实体，
    // 避免“加了个寂寞”的静默丢弃。base=0 语义：本框架不预设底子，纯由修改器贡献（Flat 加法正确；
    // Percent 系需要作者先在模板声明 base，否则按 0 计算是作者责任）。
    public static Entity GetOrCreateAttr(Entity unit, int typeId, float baseValue = 0f)
    {
        if (TryGetAttr(unit, typeId, out var attr))
            return attr;

        return CreateAttr(unit, typeId, baseValue);
    }

    /// <summary>获取 entity 某属性的最终值</summary>
    // 直接读取最终值；用于再计算、同步和恢复等热路径。
    public static float GetFinalValue(Entity entity, int typeId)
    {
        var attr = GetAttr(entity, typeId);
        return attr?.GetComponent<AttrValue>().finalValue ?? 0;
    }

    /// <summary>获取 entity 某属性的当前值 (HP/Mana)</summary>
    // current 只针对资源类属性有意义，其他属性按 0 处理。
    public static float GetCurrent(Entity entity, int typeId)
    {
        if (TryGetAttr(entity, typeId, out var attr))
        {
            if (attr.TryGetComponent<AttrValue>(out var val))
            {
                return val.current;
            }
        }

        return 0;
    }

    /// <summary>设置 entity 某属性的当前值（自动 Clamp 到 [0, finalValue]）</summary>
    // 写回 current 时强制夹紧，避免恢复/扣减把资源写出合法区间。
    public static void SetCurrent(Entity entity, int typeId, float value)
    {
        if (TryGetAttr(entity, typeId, out var attr))
        {
            if (attr.TryGetComponent<AttrValue>(out var val))
            {
                val.current = Math.Clamp(value, 0, val.finalValue);
                attr.AddComponent(val);
            }
        }
    }

    /// <summary>修改 entity 某属性的当前值 (返回修改后的值)</summary>
    // 在现值基础上做增量修改，常用于治疗、消耗和周期恢复。
    public static float ModifyCurrent(Entity unit, int typeId, float delta)
    {
        if (TryGetAttr(unit, typeId, out var attr))
        {
            if (attr.TryGetComponent<AttrValue>(out var val))
            {
                val.current += delta;
                val.current = Math.Clamp(val.current, 0, val.finalValue);
                attr.AddComponent(val);
                return val.current;
            }
        }

        return 0;
    }

    /// <summary>获取 entity 的所有属性</summary>
    // 枚举所有属性关系，供清理和调试使用。
    public static IEnumerable<(int typeId, Entity attrEntity)> GetAllAttrs(Entity unit)
    {
        var relations = unit.GetRelations<HasAttr>();
        foreach (ref var rel in relations)
        {
            yield return (rel.typeId, rel.attrEntity);
        }
    }

    /// <summary>删除 entity 的所有属性及其 Modifier（清理用）</summary>
    // 清理前先删除依赖于属性的 modifier，再移除属性本体，避免悬挂关系。
    public static void RemoveAllAttrs(Entity entity)
    {
        var relations = entity.GetRelations<HasAttr>();
        var toDelete = new List<Entity>();

        foreach (ref var rel in relations)
        {
            toDelete.Add(rel.attrEntity);
        }

        foreach (var attr in toDelete)
        {
            // 先清理指向该属性的所有 Modifier Entity
            var modifiers = attr.GetIncomingLinks<ModifyTarget>();
            var modsToDelete = new List<Entity>();
            foreach (var link in modifiers)
                modsToDelete.Add(link.Entity);
            foreach (var mod in modsToDelete)
                mod.DeleteEntity();

            entity.RemoveRelation<HasAttr, Entity>(attr);
            attr.DeleteEntity();
        }
    }
}

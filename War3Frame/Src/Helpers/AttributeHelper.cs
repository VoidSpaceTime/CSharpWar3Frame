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
    private static SortedDictionary<int, string> _types = new();

    /// <summary>
    /// 下一个属性类型 ID。
    /// </summary>
    private static int _nextId = 0;


    // 框架内置 - 基础属性
    public static readonly int Health = Register("Health");
    public static readonly int HealthRegen = Register("HealthRegen");
    public static readonly int HealthRegenPercent = Register("HealthRegenPercent");
    public static readonly int Mana = Register("Mana");
    public static readonly int ManaRegen = Register("ManaRegen");
    public static readonly int ManaRegenPercent = Register("ManaRegenPercent");
    public static readonly int Damage = Register("Damage");

    // ============================================================================
    // 控制效果属性（值 > 0 表示效果生效）
    // ============================================================================
    public static readonly int Stun = Register("Stun"); // 眩晕（禁止一切）
    public static readonly int Silence = Register("Silence"); // 沉默（禁止施法）
    public static readonly int Disarm = Register("Disarm"); // 缴械（禁止攻击）
    public static readonly int Root = Register("Root"); // 定身（禁止移动）
    public static readonly int Knockback = Register("Knockback"); // 击飞/击退

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

    #endregion

    /// <summary>为 Entity 创建属性 Entity 并建立关系</summary>
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

    /// <summary>获取 entity 某属性的最终值</summary>
    public static float GetFinalValue(Entity entity, int typeId)
    {
        var attr = GetAttr(entity, typeId);
        return attr?.GetComponent<AttrValue>().finalValue ?? 0;
    }

    /// <summary>获取 entity 某属性的当前值 (HP/Mana)</summary>
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
    public static IEnumerable<(int typeId, Entity attrEntity)> GetAllAttrs(Entity unit)
    {
        var relations = unit.GetRelations<HasAttr>();
        foreach (ref var rel in relations)
        {
            yield return (rel.typeId, rel.attrEntity);
        }
    }

    /// <summary>删除 entity 的所有属性及其 Modifier（清理用）</summary>
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

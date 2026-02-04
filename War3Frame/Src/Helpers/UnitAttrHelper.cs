using Friflo.Engine.ECS;

namespace War3Frame;

public static class UnitAttrHelper
{
    /// <summary>为 Unit 创建属性 Entity 并建立关系</summary>
    public static Entity CreateAttr(EntityStore store, Entity unit, int typeId, float baseValue)
    {
        // 创建属性 Entity
        var attr = store.CreateEntity(
            new AttrTypeId { typeId = typeId },
            new AttrValue { baseValue = baseValue, finalValue = baseValue }
        );

        // 建立 Unit → Attr 关系
        unit.AddRelation(new HasAttr(attr, typeId));

        return attr;
    }

    /// <summary>获取 Unit 的某个属性 Entity</summary>
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

    public static bool TryGetAttr(Entity unit, int typeId, out Entity? attr)
    {
        var relations = GetAttr(unit, typeId);
        attr = null;
        if (relations != null)
        {
            attr = relations.Value;
            return true;
        }

        return false;
    }

    /// <summary>获取 Unit 某属性的最终值</summary>
    public static float GetFinalValue(Entity unit, int typeId)
    {
        var attr = GetAttr(unit, typeId);
        return attr?.GetComponent<AttrValue>().finalValue ?? 0;
    }

    /// <summary>获取 Unit 的所有属性</summary>
    public static IEnumerable<(int typeId, Entity attrEntity)> GetAllAttrs(Entity unit)
    {
        var relations = unit.GetRelations<HasAttr>();
        foreach (ref var rel in relations)
        {
            yield return (rel.typeId, rel.attrEntity);
        }
    }

    /// <summary>删除 Unit 的所有属性（清理用）</summary>
    public static void RemoveAllAttrs(Entity unit)
    {
        var relations = unit.GetRelations<HasAttr>();
        var toDelete = new List<Entity>();

        foreach (ref var rel in relations)
        {
            toDelete.Add(rel.attrEntity);
        }

        foreach (var attr in toDelete)
        {
            unit.RemoveRelation<HasAttr, Entity>(attr);
            attr.DeleteEntity();
        }
    }
}
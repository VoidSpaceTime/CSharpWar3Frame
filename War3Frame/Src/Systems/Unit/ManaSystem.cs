using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace War3Frame.Src.Systems;

public class ManaSystem : QuerySystem<AttrValue, AttrTypeId, AttrOwner>
{
    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref AttrValue val, ref AttrTypeId type, ref AttrOwner owner, Entity attrEntity) =>
        {
            // 只处理 Mana 属性
            if (type.typeId != AttributeHelper.Mana) return;

            var unit = owner.owner;
            if (unit.IsNull) return;

            // 获取魔法恢复属性
            float regen = AttrHelper.GetFinalValue(unit, AttributeHelper.ManaRegen);
            
            var before = val.current;
            val.current += regen * Tick.deltaTime;
            val.current = Math.Min(val.current, val.finalValue);
            
            if (!val.current.Equals(before))
            {
                unit.AddTag<ManaNativeDirty>();
            }
        });
    }
}
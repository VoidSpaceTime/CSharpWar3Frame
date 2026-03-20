using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Systems;

namespace War3Frame.Src.Systems;

[SystemRegister(SystemKind.Interval)]
public class HealthSystem : QuerySystem<AttrValue, AttrTypeId, AttrOwner>
{
    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref AttrValue val, ref AttrTypeId type, ref AttrOwner owner, Entity attrEntity) =>
        {
            // 只处理 Health 属性
            if (type.typeId != AttributeHelper.Health) return;

            var unit = owner.owner;
            if (unit.IsNull) return;

            // 获取生命恢复属性
            float regen = AttributeHelper.GetFinalValue(unit, AttributeHelper.HealthRegen);

            var before = val.current;
            val.current += regen * Tick.deltaTime;
            val.current = Math.Min(val.current, val.finalValue);

            if (!val.current.Equals(before))
            {
                unit.AddTag<NativeealthDirty>();
            }
        });
    }
}
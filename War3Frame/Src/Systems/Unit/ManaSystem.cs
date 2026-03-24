using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Systems;

namespace War3Frame.Src.Systems;

[SystemRegister(SystemKind.Interval)]
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

            // 获取魔法恢复属性（固定值 + 最大魔法百分比）
            float flatRegen = AttributeHelper.GetFinalValue(unit, AttributeHelper.ManaRegen);
            float percentRegen = AttributeHelper.GetFinalValue(unit, AttributeHelper.ManaRegenPercent);
            float regen = flatRegen + val.finalValue * percentRegen;

            val.current += regen * Tick.deltaTime;
            val.current = Math.Min(val.current, val.finalValue);
        });
    }
}

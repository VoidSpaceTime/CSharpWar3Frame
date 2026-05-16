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

            // 获取生命恢复属性（固定值 + 最大生命百分比）
            float flatRegen = AttributeHelper.GetFinalValue(unit, AttributeHelper.HealthRegen);
            float percentRegen = AttributeHelper.GetFinalValue(unit, AttributeHelper.HealthRegenPercent);
            float regen = flatRegen + val.finalValue * percentRegen;

            // 资源恢复只改 ECS current；原生生命同步由 UnitNativeSystem 统一执行。
            val.current += regen * Tick.deltaTime;
            val.current = Math.Min(val.current, val.finalValue);
        });
    }
}

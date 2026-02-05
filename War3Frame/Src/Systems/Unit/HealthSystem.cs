using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components.Attribute;

namespace War3Frame.Src.Systems;

public class HealthSystem : QuerySystem<HealthAttr>, ITimedSystem
{
    public float Interval => 0.04f;

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref HealthAttr hp, Entity unit) =>
        {
            // 查找生命恢复属性
            if (!UnitAttrHelper.TryGetAttr(unit, AttributeHelper.HealthRegen, out var regenEntity) ||
                !UnitAttrHelper.TryGetAttr(unit, AttributeHelper.Health, out var healthEntity)) return;
            var before = hp.current;
            var regen = regenEntity.Value.GetComponent<AttrValue>().finalValue;
            var health = healthEntity.Value.GetComponent<AttrValue>().finalValue;

            hp.current += regen * Tick.deltaTime;
            hp.current = Math.Min(hp.current, health);

            if (!hp.current.Equals(before))
            {
                unit.AddTag<HealthNativeDirty>();
            }
        });
    }
}
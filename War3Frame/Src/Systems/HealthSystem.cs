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
            if (UnitAttrHelper.TryGetAttr(unit, AttributeHelper.HealthRegen, out var regenAttr) ||
                UnitAttrHelper.TryGetAttr(unit, AttributeHelper.Health, out var healthAttr)) return;

            var regen = regenAttr.Value.GetComponent<AttrValue>().finalValue;
            var health = healthAttr.Value.GetComponent<AttrValue>().finalValue;

            hp.current += regen * Tick.deltaTime;
            hp.current = Math.Min(hp.current, health);
        });
    }
}
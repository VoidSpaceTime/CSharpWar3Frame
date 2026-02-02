using Friflo.Engine.ECS.Systems;

namespace War3Frame.Src.Systems;

public class HealthSystem : QuerySystem<Health>, ITimedSystem
{
    public float Interval => 0.02f;

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref health, entity) =>
        {
            var update = health.current;
            if (health.current <= 0)
            {
                health.current = -1;
                return;
            }

            update += health.regen * Tick.deltaTime;
            if (update > health.max) update = health.max;
            if (health.current != update)
            {
                health.current = update;
                entity.AddTag<HealthDirty>();
            }
        });
    }
}
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace War3Frame.Src.Systems
{
    public class HealthSystem : QuerySystem<Health>, ITimedSystem
    {
        public float Interval => 0.02f;
        protected override void OnUpdate()
        {
            Query.ForEachEntity((ref Health health, Entity entity) =>
                  {
                      var update = health.current;
                      if (health.current <= 0)
                      {
                          health.current = -1;
                          return;
                      }
                      update += health.regen * Tick.deltaTime;
                      if (update > health.max)
                      {
                          update = health.max;
                      }
                      if (health.current != update)
                      {
                          health.current = update;
                          entity.AddTag<HealthDirty>();
                      }
                  });
        }
    }
}

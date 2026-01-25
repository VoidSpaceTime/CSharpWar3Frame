using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace War3Frame.Src.Systems
{
    public class ManaSystem : QuerySystem<Mana>
    {
        protected override void OnUpdate()
        {
            Query.ForEachEntity((ref Mana mana, Entity entity) =>
            {
                var update = mana.current;

                update += mana.regen * Tick.deltaTime;
                if (update > mana.max)
                {
                    update = mana.max;
                }
                if (mana.current <= 0)
                {
                    mana.current = 0;
                }
                if (mana.current != update)
                {
                    mana.current = update;
                    entity.AddTag<ManaDirty>();
                }
            });
        }
    }
}

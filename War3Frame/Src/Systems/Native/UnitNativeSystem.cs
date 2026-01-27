using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using System;
using System.Collections.Generic;
using System.Text;


namespace War3Frame
{
    public class UnitNativeSystem : QuerySystem<UnitNative>, ITimedSystem
    {
        public float Interval => 0.03125f;

        protected override void OnUpdate()
        {

            Query.ForEachEntity((ref UnitNative unit, Entity entity) =>
            {
                // 同步单位血量              
                if (entity.Tags.Has<HealthDirty>())
                {
                    ref var health = ref entity.GetComponent<Health>();
                    var set = health.current / 10000f;
                    JassApi.SetUnitState(unit.unit, new JUnitState(Blizzard.UNIT_STATE_LIFE), set);
                    entity.RemoveTag<HealthDirty>();
                }
                // 同步单位魔法
                if (entity.Tags.Has<ManaDirty>())
                {
                    ref var mana = ref entity.GetComponent<Mana>();
                    {
                        var set = mana.current / 10000f;
                        JassApi.SetUnitState(unit.unit, new JUnitState(Blizzard.UNIT_STATE_MANA), set);
                    }
                    entity.RemoveTag<ManaDirty>();
                }
                // 同步单位位置
                if (entity.TryGetComponent<Position>(out var position))
                {
                    position.x = JassApi.GetUnitX(unit.unit);
                    position.y = JassApi.GetUnitY(unit.unit);
                }
            });
        }
    }
}
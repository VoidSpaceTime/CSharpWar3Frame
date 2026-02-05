using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components.Attribute;

namespace War3Frame;

public class UnitNativeSystem : QuerySystem<UnitNative>, ITimedSystem
{
    public float Interval => 0.03125f;

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref native, entity) =>
        {
            // 同步单位血量              
            if (entity.Tags.Has<HealthNativeDirty>() &&
                UnitAttrHelper.TryGetAttr(entity, AttributeHelper.Health, out var health))
            {
                var hpCur = entity.GetComponent<HealthAttr>();
                var set = (hpCur.current / health.Value.GetComponent<AttrValue>().finalValue) * 10000f;
                JassApi.SetUnitState(native.unit, new JUnitState(Blizzard.UNIT_STATE_LIFE), set);
                entity.RemoveTag<HealthNativeDirty>();
            }

            // 同步单位魔法
            if (entity.Tags.Has<ManaNativeDirty>() &&
                UnitAttrHelper.TryGetAttr(entity, AttributeHelper.Mana, out var mana))
            {
                var manaCur = entity.GetComponent<ManaAttr>();
                var set = (manaCur.current / mana.Value.GetComponent<AttrValue>().finalValue) * 10000f;
                JassApi.SetUnitState(native.unit, new JUnitState(Blizzard.UNIT_STATE_MANA), set);
                entity.RemoveTag<ManaNativeDirty>();
            }

            // 同步单位位置
            if (entity.TryGetComponent<Position>(out var position))
            {
                position.x = JassApi.GetUnitX(native.unit);
                position.y = JassApi.GetUnitY(native.unit);
            }
        });
    }
}
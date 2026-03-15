using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace War3Frame.Src.Systems;

public struct NativeealthDirty : ITag
{
}

public struct NativeManaDirty : ITag
{
}

public class UnitNativeSystem : QuerySystem<UnitNative>, ITimedSystem
{
    public float Interval => 0.03125f;

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref UnitNative native, Entity entity) =>
        {
            // 同步单位死亡
            if (entity.Tags.Has<NativeUnitDeathDirty>())
            {
                JassApi.KillUnit(native.unit);
                HandleHelper.HandleRemove(native.unit);
                entity.RemoveTag<NativeUnitDeathDirty>();
                return;
            }

            // 同步单位血量              
            if (entity.Tags.Has<NativeealthDirty>() &&
                AttributeHelper.TryGetAttr(entity, AttributeHelper.Health, out var health))
            {
                if (health.TryGetComponent<AttrValue>(out var hpVal))
                {
                    var set = (hpVal.current / hpVal.finalValue) * 10000f;
                    JassApi.SetUnitState(native.unit, new JUnitState(Blizzard.UNIT_STATE_LIFE), set);
                }

                entity.RemoveTag<NativeealthDirty>();
            }

            // 同步单位魔法
            if (entity.Tags.Has<NativeManaDirty>() &&
                AttributeHelper.TryGetAttr(entity, AttributeHelper.Mana, out var mana))
            {
                if (mana.TryGetComponent<AttrValue>(out var manaVal))
                {
                    var set = (manaVal.current / manaVal.finalValue) * 10000f;
                    JassApi.SetUnitState(native.unit, new JUnitState(Blizzard.UNIT_STATE_MANA), set);
                }

                entity.RemoveTag<NativeManaDirty>();
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
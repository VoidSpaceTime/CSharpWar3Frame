using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace War3Frame.Systems.Native;

[SystemRegister(SystemKind.Immediate)]
public class UnitNativeRemoveSystem : QuerySystem<UnitNative>
{
    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref UnitNative native, Entity entity) =>
        {
            if (entity.Tags.Has<UnitRemoveTag>())
            {
                JassApi.RemoveUnit(native.unit);
                HandleHelper.HandleRemove(native.unit);
            }
            else if (UnitNativeDirtyHelper.Has(entity, UnitNativeDirtyFlags.Death))
            {
                JassApi.KillUnit(native.unit);
                UnitNativeDirtyHelper.Clear(entity, UnitNativeDirtyFlags.Death);
            }
        });
    }
}

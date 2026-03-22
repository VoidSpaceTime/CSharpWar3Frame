using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Systems;

namespace War3Frame.Src.Systems.Unit;

/// <summary>
/// 尸体清理系统
/// </summary>
[SystemRegister(SystemKind.Interval, 1)]
public class CorpseCleanupSystem : QuerySystem<UnitNative>
{
    public CorpseCleanupSystem()
    {
        Filter.AnyTags(Tags.Get<CorpseExpired>());
    }

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref UnitNative native, Entity entity) =>
        {
            JassApi.RemoveUnit(native.unit);
            HandleHelper.HandleRemove(native.unit);
            entity.RemoveTag<CorpseExpired>();
            entity.RemoveTag<TimerExpired>();
            entity.DeleteEntity();
        });
    }
}

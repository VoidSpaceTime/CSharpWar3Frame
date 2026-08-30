using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;
using War3Frame.Systems;

namespace War3Frame.Src.Systems;

[SystemRegister(SystemKind.Interval, 40)]
public class BuffDurationSystem : QuerySystem<BuffDuration, Duration>, ITimedSystem
{
    public float Interval => 0.1f;

    public BuffDurationSystem()
    {
        Filter.AnyTags(Tags.Get<Buff>());
    }

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref BuffDuration duration, ref Duration runtime, Entity entity) =>
        {
            if (runtime.remaining < 0f)
            {
                return; // 永久 Buff
            }

            if (entity.Tags.Has<DurationExpired>())
            {
                entity.AddTag<BuffExpired>();
            }
        });
    }
}

[SystemRegister(SystemKind.Interval, 41)]
public class BuffExpireSystem : QuerySystem<ModifyValue, ModifyTarget>
{
    public BuffExpireSystem()
    {
        Filter.AnyTags(Tags.Get<BuffExpired>());
    }

    protected override void OnUpdate()
    {
        var toDelete = new List<Entity>();
        var attrsToRefresh = new HashSet<int>();

        Query.ForEachEntity((ref ModifyValue mod, ref ModifyTarget target, Entity entity) =>
        {
            attrsToRefresh.Add(target.target.Id);
            toDelete.Add(entity);
        });

        foreach (var entity in toDelete)
        {
            entity.DeleteEntity();
        }

        foreach (var attrId in attrsToRefresh)
        {
            var attr = CommandBuffer.EntityStore.GetEntityById(attrId);
            if (!attr.IsNull)
            {
                attr.AddTag<AttrDirty>();
            }
        }
    }
}

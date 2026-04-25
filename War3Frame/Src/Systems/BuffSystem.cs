using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;

namespace War3Frame.Src.Systems;

/// <summary>
///     Buff 持续时间系统 - 处理 Buff 倒计时和过期
/// </summary>
public class BuffDurationSystem : QuerySystem<BuffDuration>, ITimedSystem
{
    public float Interval => 0.1f;  // 每 0.1 秒更新一次

    public BuffDurationSystem()
    {
        // 只处理有 Buff 标记的实体
        Filter.AnyTags(Tags.Get<Buff>());
    }

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref BuffDuration duration, Entity entity) =>
        {
            // 永久 Buff 不处理
            if (duration.isPermanent) return;

            if (entity.Tags.Has<BuffExpired>())
            {
                duration.remaining = 0;
                entity.AddComponent(duration);
                return;
            }

            if (!entity.TryGetComponent<TimerTask>(out var timer) || timer.kind != TimerTaskKind.BuffExpire)
            {
                entity.AddComponent(new TimerTask
                {
                    mode = TimerTaskMode.Once,
                    interval = duration.remaining,
                    remaining = duration.remaining,
                    paused = false,
                    owner = entity,
                    kind = TimerTaskKind.BuffExpire,
                    triggerCount = 0,
                    maxTriggerCount = 1
                });
                return;
            }

            duration.remaining = Math.Max(0, timer.remaining);
            entity.AddComponent(duration);
        });
    }
}

/// <summary>
///     Buff 过期清理系统 - 移除过期的 Buff
/// </summary>
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
            // 记录需要刷新的属性 Entity
            attrsToRefresh.Add(target.target.Id);

            // 记录要删除的 Buff
            toDelete.Add(entity);
        });

        // 删除所有过期 Buff
        foreach (var entity in toDelete)
        {
            entity.DeleteEntity();
        }

        // 刷新受影响的属性
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

using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame;
using War3Frame.Components;
using War3Frame.Systems;
using War3Frame.Helpers;

namespace War3Frame.Src.Systems;

[SystemRegister(SystemKind.Interval, 40)]
public class BuffDurationSystem : QuerySystem<Buff, BuffBehavior, Duration>, ITimedSystem
{
    public float Interval => 0.1f;

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref Buff buff, ref BuffBehavior behavior, ref Duration runtime, Entity entity) =>
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
public class BuffExpireSystem : QuerySystem<Buff, BuffBehavior, ModifyTarget>
{
    public BuffExpireSystem()
    {
        Filter.AnyTags(Tags.Get<BuffExpired>());
    }

    protected override void OnUpdate()
    {
        var toDelete = new List<Entity>();
        var attrsToRefresh = new HashSet<int>();

        Query.ForEachEntity((ref Buff buff, ref BuffBehavior behavior, ref ModifyTarget target, Entity entity) =>
        {
            // 普通 buff 删除时需打脏属性重算；DoT（无 ModifyValue）只删除实体本身
            if (entity.HasComponent<ModifyValue>())
            {
                attrsToRefresh.Add(target.target.Id);
            }
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

[SystemRegister(SystemKind.Interval, 39)]
public class BuffTickSystem : QuerySystem<Buff, Duration>, ITimedSystem
{
    public float Interval => 0.05f; // 每 50ms 检查一次

    protected override void OnUpdate()
    {
        var deltaTime = Interval;
        // tick 行为可能创建实体/删除实体（结构变更），Friflo 禁止在 Query 迭代内做，
        // 先收集到点的事件，循环外执行。
        var dueTicks = new List<(Entity buffEntity, Entity targetUnit, float tickValue, string actionId, int hitCount)>();

        Query.ForEachEntity((ref Buff buff, ref Duration duration, Entity entity) =>
        {
            // 跳过无 tick 的 buff
            if (buff.tickInterval <= 0f || string.IsNullOrEmpty(buff.tickActionId))
                return;

            // 跳过永久 buff（不 tick）
            if (duration.remaining < 0f)
                return;

            // 累加经过时间
            buff.lastTick += deltaTime;

            // 记录到点次数（不在此直接执行，避免迭代内结构变更）
            var hitCount = 0;
            while (buff.lastTick >= buff.tickInterval)
            {
                buff.lastTick -= buff.tickInterval;
                hitCount++;
            }

            if (hitCount > 0)
            {
                // 获取目标单位（从 ModifyTarget → AttrOwner）
                if (!entity.TryGetComponent<ModifyTarget>(out var modifyTarget))
                    return;

                var attrEntity = modifyTarget.target;
                if (!attrEntity.TryGetComponent<AttrOwner>(out var attrOwner))
                    return;

                dueTicks.Add((entity, attrOwner.owner, buff.tickValue, buff.tickActionId, hitCount));
            }

            // ref 参数直接写回内存，无需 AddComponent
        });

        // 循环外执行 tick 行为（结构变更安全）
        foreach (var (buffEntity, targetUnit, tickValue, actionId, hitCount) in dueTicks)
        {
            var action = BuffTickActionRegistry.Get(actionId);
            if (action == null)
                continue;

            // 同一帧内多次到点则执行多次
            for (var i = 0; i < hitCount; i++)
            {
                action.Execute(buffEntity, targetUnit);
            }
        }
    }
}

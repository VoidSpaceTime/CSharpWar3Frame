using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Systems;

namespace War3Frame.Src.Systems.Time;

/// <summary>
/// 通用计时任务推进系统
/// </summary>
[SystemRegister(SystemKind.Interval, 0)]
public class TimerTaskSystem : QuerySystem<TimerTask>, ITimedSystem
{
    /// <summary>
    /// 计时任务推进间隔。
    /// </summary>
    public float Interval => 0.05f;

    protected override void OnUpdate()
    {
        // Friflo 禁止在 Query 迭代内增删组件/Tag（结构变更）：先收集，循环外统一应用。
        var toDelete = new List<Entity>();
        var toMarkExpired = new List<Entity>();
        var toMarkBuffExpired = new List<Entity>();
        var toRemoveTimer = new List<Entity>();
        var ownerStateUpdates = new List<(Entity owner, UnitLifeState state)>();

        Query.ForEachEntity((ref TimerTask timer, Entity entity) =>
        {
            if (timer.paused)
            {
                return;
            }

            if (timer.owner.IsNull)
            {
                // owner 已不存在时，计时任务没有继续推进的语义，直接回收。
                toDelete.Add(entity);
                return;
            }

            timer.remaining -= Tick.deltaTime;
            if (timer.remaining > 0)
            {
                return; // ref 原地写已持久化
            }

            timer.triggerCount++;
            toMarkExpired.Add(entity);

            switch (timer.kind)
            {
                case TimerTaskKind.CorpseCleanup:
                    // 计时器只把尸体标记为可清理，具体生命周期推进由 UnitLifecycleTransitionSystem 完成。
                    if (!timer.owner.IsNull && timer.owner.TryGetComponent<UnitLifeState>(out UnitLifeState state)
                        && state.lifePhase == UnitLifecyclePhase.Corpse)
                    {
                        state.lifePhase = UnitLifecyclePhase.ClearCorpse;
                        ownerStateUpdates.Add((timer.owner, state));
                    }
                    break;
                case TimerTaskKind.BuffExpire:
                    // Buff 到期用标签表达结果，实际移除逻辑由 Buff 系统消费。
                    toMarkBuffExpired.Add(entity);
                    break;
            }

            var reachedMax = timer.maxTriggerCount > 0 && timer.triggerCount >= timer.maxTriggerCount;
            if (timer.mode == TimerTaskMode.Once || reachedMax)
            {
                toRemoveTimer.Add(entity);
                return;
            }

            timer.remaining += timer.interval;
            // 周期任务保留同一个实体并刷新剩余时间，便于外部通过 source/owner 继续追踪（ref 已持久化）。
        });

        // ---- 循环外应用结构变更 ----
        foreach (var entity in toDelete)
        {
            if (!entity.IsNull)
                entity.DeleteEntity();
        }

        foreach (var entity in toMarkExpired)
        {
            if (!entity.IsNull)
                entity.AddTag<TimerExpired>();
        }

        foreach (var entity in toMarkBuffExpired)
        {
            if (!entity.IsNull)
                entity.AddTag<BuffExpired>();
        }

        foreach (var (owner, state) in ownerStateUpdates)
        {
            if (!owner.IsNull)
                owner.AddComponent(state);
        }

        foreach (var entity in toRemoveTimer)
        {
            if (!entity.IsNull)
                entity.RemoveComponent<TimerTask>();
        }
    }
}

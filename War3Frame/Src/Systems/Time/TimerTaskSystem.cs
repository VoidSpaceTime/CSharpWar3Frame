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
        Query.ForEachEntity((ref TimerTask timer, Entity entity) =>
        {
            if (timer.paused)
            {
                return;
            }

            if (timer.owner.IsNull)
            {
                entity.DeleteEntity();
                return;
            }

            timer.remaining -= Tick.deltaTime;
            if (timer.remaining > 0)
            {
                entity.AddComponent(timer);
                return;
            }

            timer.triggerCount++;
            entity.AddTag<TimerExpired>();

            switch (timer.kind)
            {
                case TimerTaskKind.CorpseCleanup:
                    if (!timer.owner.IsNull && timer.owner.TryGetComponent<UnitLifeState>(out UnitLifeState state)
                        && state.lifePhase == UnitLifecyclePhase.Corpse)
                    {
                        state.lifePhase = UnitLifecyclePhase.ClearCorpse;
                        timer.owner.AddComponent(state);
                    }
                    break;
                case TimerTaskKind.BuffExpire:
                    entity.AddTag<BuffExpired>();
                    break;
            }

            var reachedMax = timer.maxTriggerCount > 0 && timer.triggerCount >= timer.maxTriggerCount;
            if (timer.mode == TimerTaskMode.Once || reachedMax)
            {
                entity.RemoveComponent<TimerTask>();
                return;
            }

            timer.remaining += timer.interval;
            entity.AddComponent(timer);
        });
    }
}

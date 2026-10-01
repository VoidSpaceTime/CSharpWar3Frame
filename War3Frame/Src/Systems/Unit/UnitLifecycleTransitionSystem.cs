using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace War3Frame.Systems.Unit;

/// <summary>
/// 生命周期阶段推进系统。
/// 仅负责主路径 phase progression，不执行 native side effects 或终态 ECS 清理。
/// </summary>
/// <remarks>
/// order 20：晚于 UnitRemoveNativeSystem(10)——保证 Death 的 KillUnit 已执行后才转 Corpse，
/// 早于 UnitLifecycleDisposeSystem(30)。
/// </remarks>
[SystemRegister(SystemKind.Immediate, 20)]
public class UnitLifecycleTransitionSystem : QuerySystem<UnitLifeState>
{
    protected override void OnUpdate()
    {
        // 挂 RebornWait 计时器是 AddComponent（结构变更）：先收集，循环外统一应用。
        var rebornWaits = new List<(Entity entity, float wait)>();

        Query.ForEachEntity((ref UnitLifeState state, Entity entity) =>
        {
            if (state.lifePhase == UnitLifecyclePhase.Death)
            {
                // 这里只推进 ECS 生命周期状态；尸体创建/原生隐藏等副作用交给 Native/Execution 层。
                // ref 参数直接写回内存即可，无需 AddComponent（循环内 AddComponent 会抛结构变更异常）。
                state.lifePhase = UnitLifecyclePhase.Corpse;
                return;
            }

            if (state.lifePhase == UnitLifecyclePhase.ClearCorpse)
            {
                if (state.rebornTime > 0f)
                {
                    // 已声明复活：离开死亡主路径进入复活等待，保留实体与原生句柄，绝不进入 Remove。
                    state.lifePhase = UnitLifecyclePhase.RebornPending;
                    rebornWaits.Add((entity, state.rebornTime));
                    return;
                }

                // 无复活语义：维持现状，交由 Native 移除与 Dispose。
                state.lifePhase = UnitLifecyclePhase.Remove;
                return;
            }
        });

        foreach (var (entity, wait) in rebornWaits)
        {
            if (entity.IsNull)
                continue;

            // 复活等待真相在 TimerTask；Transition 是 Immediate 系统，不自行按帧扣减。
            entity.AddComponent(new TimerTask
            {
                mode = TimerTaskMode.Once,
                interval = wait,
                remaining = wait,
                paused = false,
                owner = entity,
                kind = TimerTaskKind.RebornWait,
                triggerCount = 0,
                maxTriggerCount = 1
            });
        }
    }
}

using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace War3Frame.Systems.Native;

/// <summary>
/// 生命周期原生副作用执行系统。
/// 仅根据生命周期阶段执行 native side effects，不拥有 phase 推进或终态 ECS 清理。
/// </summary>
/// <remarks>
/// order 10：必须是生命周期三系统中最先执行的——先把 Death 消费为 KillUnit、把 Remove 消费为
/// RemoveUnit 并置 Disposing，随后 Transition(20) 才能安全推进阶段、Dispose(30) 才能安全删除实体。
/// </remarks>
[SystemRegister(SystemKind.Immediate, 10)]
public class UnitRemoveNativeSystem : QuerySystem<UnitLifeState>
{
    // 生命周期系统只推进 phase；这里根据 phase 执行 KillUnit/RemoveUnit 这类 native 副作用。

    protected override void OnUpdate()
    {
        // 设置 Disposing 需要 AddComponent（结构变更）：先收集，循环外执行。
        var toKill = new List<Entity>();
        var toRemove = new List<Entity>();

        Query.ForEachEntity((ref UnitLifeState state, Entity entity) =>
        {
            if (state.lifePhase == UnitLifecyclePhase.Death)
            {
                toKill.Add(entity);
            }
            else if (state.lifePhase == UnitLifecyclePhase.Remove)
            {
                toRemove.Add(entity);
            }
        });

        // Death：执行原生击杀（在 Transition 把 Death 推进为 Corpse 之前）。
        foreach (var entity in toKill)
        {
            if (entity.IsNull)
                continue;

            if (entity.TryGetComponent<UnitNative>(out var native))
            {
                JassApi.KillUnit(native.unit);
            }
        }

        // Remove：执行原生移除并置 Disposing，交由 Dispose 系统删除实体。
        foreach (var entity in toRemove)
        {
            if (entity.IsNull)
                continue;

            if (entity.TryGetComponent<UnitNative>(out var native))
            {
                NativeEntityIndex.Unregister(native.unit);
                // 配对规则：销毁原生对象前相邻注销句柄引用（AGENTS.md）。
                HandleHelper.HandleRemove(native.unit);
                JassApi.RemoveUnit(native.unit);
            }

            if (entity.TryGetComponent<UnitLifeState>(out var state))
            {
                state.lifePhase = UnitLifecyclePhase.Disposing;
                entity.AddComponent(state);
            }
        }
    }
}

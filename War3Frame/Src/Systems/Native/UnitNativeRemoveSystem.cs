using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace War3Frame.Systems.Native;

/// <summary>
/// 生命周期原生副作用执行系统。
/// 仅根据生命周期阶段执行 native side effects，不拥有 phase 推进或终态 ECS 清理。
/// </summary>
[SystemRegister(SystemKind.Immediate)]
public class UnitNativeRemoveSystem : QuerySystem<UnitLifeState>
{
    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref UnitLifeState state, Entity entity) =>
        {
            if (state.lifePhase == UnitLifecyclePhase.Death)
            {
                if (entity.TryGetComponent<UnitNative>(out var native))
                {
                    JassApi.KillUnit(native.unit);
                }
                return;
            }

            if (state.lifePhase == UnitLifecyclePhase.Remove)
            {
                if (entity.TryGetComponent<UnitNative>(out var native))
                {
                    JassApi.RemoveUnit(native.unit);
                    HandleHelper.HandleRemove(native.unit);
                }
            }
        });
    }
}

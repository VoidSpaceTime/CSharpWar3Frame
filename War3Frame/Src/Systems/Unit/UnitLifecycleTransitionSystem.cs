using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace War3Frame.Systems.Unit;

/// <summary>
/// 生命周期阶段推进系统。
/// 仅负责主路径 phase progression，不执行 native side effects 或终态 ECS 清理。
/// </summary>
[SystemRegister(SystemKind.Immediate)]
public class UnitLifecycleTransitionSystem : QuerySystem<UnitLifeState>
{
    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref UnitLifeState state, Entity entity) =>
        {
            if (state.lifePhase == UnitLifecyclePhase.Death)
            {
                state.lifePhase = UnitLifecyclePhase.Corpse;
                entity.AddComponent(state);
                return;
            }

            if (state.lifePhase == UnitLifecyclePhase.ClearCorpse)
            {
                state.lifePhase = UnitLifecyclePhase.Remove;
                entity.AddComponent(state);
            }
        });
    }
}

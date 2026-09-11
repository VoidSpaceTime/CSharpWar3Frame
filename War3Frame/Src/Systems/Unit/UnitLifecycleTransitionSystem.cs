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
                // 计时器到期后先进入 Remove 阶段，由后续系统完成原生移除和 ECS dispose。
                state.lifePhase = UnitLifecyclePhase.Remove;
            }
        });
    }
}

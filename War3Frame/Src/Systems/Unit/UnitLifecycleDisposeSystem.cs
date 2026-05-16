using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace War3Frame.Systems.Unit;

/// <summary>
/// 生命周期终态 ECS 清理系统。
/// 仅负责 Remove 阶段的最终 ECS 收尾与实体销毁。
/// </summary>
[SystemRegister(SystemKind.Immediate)]
public class UnitLifecycleDisposeSystem : QuerySystem<UnitLifeState>
{
    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref UnitLifeState state, Entity entity) =>
        {
            if (state.lifePhase != UnitLifecyclePhase.Remove)
            {
                return;
            }

            // 终态清理由 helper 统一收口，避免属性、技能、计时器留下悬挂实体。
            UnitHelper.CleanupFinalizeEntityDispose(entity);
        });
    }
}

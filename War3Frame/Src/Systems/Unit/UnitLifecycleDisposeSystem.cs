using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace War3Frame.Systems.Unit;

/// <summary>
/// 生命周期终态 ECS 清理系统。
/// 仅负责 Disposing 阶段的最终 ECS 收尾与实体销毁。
/// </summary>
/// <remarks>
/// order 30：晚于 UnitRemoveNativeSystem(10) 与 UnitLifecycleTransitionSystem(20)。
/// 只在 Disposing（原生已移除）阶段删除实体，杜绝“ECS 删除抢在原生移除之前”导致的原生句柄泄漏。
/// </remarks>
[SystemRegister(SystemKind.Immediate, 30)]
public class UnitLifecycleDisposeSystem : QuerySystem<UnitLifeState>
{
    protected override void OnUpdate()
    {
        // 终态清理由 helper 统一收口；helper 会产生结构变更，必须收集后在循环外执行。
        var toDispose = new List<Entity>();

        Query.ForEachEntity((ref UnitLifeState state, Entity entity) =>
        {
            if (state.lifePhase == UnitLifecyclePhase.Disposing)
            {
                toDispose.Add(entity);
            }
        });

        foreach (var entity in toDispose)
        {
            if (!entity.IsNull)
                UnitHelper.CleanupFinalizeEntityDispose(entity);
        }
    }
}

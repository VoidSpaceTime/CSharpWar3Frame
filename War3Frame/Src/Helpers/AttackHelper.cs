using Friflo.Engine.ECS;
using War3Frame;

namespace War3Frame.Helpers;

/// <summary>
/// 攻击形态操作入口：运行时改写单位攻击形态（近战 ↔ 远程等形态切换）。
/// 只写 ECS AttackTypeState 状态，不直接调用原生 API；
/// 由装备/技能/buff 等效果在需要临时变更攻击方式时调用，结束需复原时再调回原形态。
/// </summary>
public static class AttackHelper
{
    /// <summary>读取单位当前攻击形态；无 AttackTypeState 组件视为近战（缺省 Melee）。</summary>
    public static AttackType GetAttackType(Entity unit)
    {
        return unit.TryGetComponent<AttackTypeState>(out var state) ? state.value : AttackType.Melee;
    }

    /// <summary>改写单位攻击形态（挂/更新 AttackTypeState 组件）。</summary>
    public static void SetAttackType(Entity unit, AttackType type)
    {
        unit.AddComponent(new AttackTypeState { value = type });
    }
}

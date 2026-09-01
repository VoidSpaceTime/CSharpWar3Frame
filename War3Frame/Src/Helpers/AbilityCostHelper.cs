using Friflo.Engine.ECS;
using War3Frame.Helpers;


namespace War3Frame;

/// <summary>
/// 技能消耗帮助类 - 检查和扣除技能消耗
/// </summary>
public static class AbilityCostHelper
{
    /// <summary>
    /// 检查资源是否足够施放技能。
    /// 按 CostConditionRegistry 注册顺序短路判定；未声明的消耗项视为满足。
    /// </summary>
    public static bool CheckCost(Entity unit, Entity ability)
    {
        foreach (var entry in CostConditionRegistry.Entries)
        {
            if (!entry.Check(unit, ability).satisfied)
                return false;
        }

        return true;
    }

    /// <summary>
    /// 扣除技能消耗。
    /// 按注册顺序逐项执行；单项不足则跳过该项（不扣成负数）。
    /// </summary>
    public static void ApplyCost(Entity unit, Entity ability)
    {
        foreach (var entry in CostConditionRegistry.Entries)
        {
            entry.Deplete(unit, ability);
        }
    }

}

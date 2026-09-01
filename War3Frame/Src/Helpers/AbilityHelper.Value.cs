using Friflo.Engine.ECS;

namespace War3Frame.Helpers;

/// <summary>
/// 技能数值读取帮助类。
/// 当前统一从 AbilityHelper.Stat 读取能力参数值。
/// </summary>
public static partial class AbilityHelper
{
    /// <summary>
    /// 获取技能蓝耗。
    /// </summary>
    public static float GetManaCost(Entity ability)
    {
        // 双读：ManaCost 组件优先（组件化蓝耗），无组件回退 AbilityStat 统计值。
        if (ability.TryGetComponent<ManaCost>(out var cost))
            return cost.value;
        return GetFinalValue(ability, ManaCost);
    }

    /// <summary>
    /// 获取技能冷却时长。
    /// </summary>
    public static float GetCooldown(Entity ability)
    {
        return GetFinalValue(ability, CooldownDuration);
    }

    /// <summary>
    /// 获取技能施法距离。
    /// </summary>
    public static float GetCastRange(Entity ability)
    {
        return GetFinalValue(ability, Range);
    }

    /// <summary>
    /// 获取技能施法前摇时间。
    /// </summary>
    public static float GetCastTime(Entity ability)
    {
        return GetFinalValue(ability, CastTime);
    }

    /// <summary>
    /// 获取技能引导时长。
    /// </summary>
    public static float GetChannelDuration(Entity ability)
    {
        return GetFinalValue(ability, ChannelDuration);
    }

    /// <summary>
    /// 获取技能伤害数值。
    /// </summary>
    public static float GetDamageAmount(Entity ability)
    {
        return GetFinalValue(ability, DamageAmount);
    }

    /// <summary>
    /// 获取技能治疗数值。
    /// </summary>
    public static float GetHealAmount(Entity ability)
    {
        return GetFinalValue(ability, HealAmount);
    }

    /// <summary>
    /// 获取技能范围半径。
    /// </summary>
    public static float GetRadius(Entity ability)
    {
        return GetFinalValue(ability, Radius);
    }

    /// <summary>
    /// 获取技能引导 tick 间隔。
    /// </summary>
    public static float GetChannelTickInterval(Entity ability)
    {
        return GetFinalValue(ability, ChannelTickInterval);
    }

    /// <summary>
    /// 获取技能释放后摇时长。
    /// </summary>
    public static float GetBackswingDuration(Entity ability)
    {
        return GetFinalValue(ability, BackswingDuration);
    }
}

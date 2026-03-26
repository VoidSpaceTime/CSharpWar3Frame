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
    /// 获取弹道速度。
    /// </summary>
    public static float GetProjectileSpeed(Entity ability)
    {
        return GetFinalValue(ability, ProjectileSpeed);
    }

    /// <summary>
    /// 获取弹道最大距离。
    /// </summary>
    public static float GetProjectileDistance(Entity ability)
    {
        return GetFinalValue(ability, ProjectileDistance);
    }

    /// <summary>
    /// 获取命中宽度。
    /// </summary>
    public static float GetHitWidth(Entity ability)
    {
        return GetFinalValue(ability, HitWidth);
    }

    /// <summary>
    /// 获取到达判定阈值。
    /// </summary>
    public static float GetArrivalThreshold(Entity ability)
    {
        return GetFinalValue(ability, ArrivalThreshold);
    }

    /// <summary>
    /// 获取最大目标数量。
    /// </summary>
    public static int GetMaxTargets(Entity ability)
    {
        return (int)GetFinalValue(ability, MaxTargets);
    }
}

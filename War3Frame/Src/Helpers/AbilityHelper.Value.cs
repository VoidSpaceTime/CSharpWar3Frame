using Friflo.Engine.ECS;

namespace War3Frame.Helpers;

/// <summary>
/// 技能数值读取帮助类。
/// 当前统一从 AbilityHelper.Stat 读取能力参数值。
/// </summary>
public static partial class AbilityHelper
{
    public static float GetManaCost(Entity ability)
    {
        return GetFinalValue(ability, ManaCost);
    }

    public static float GetCooldown(Entity ability)
    {
        return GetFinalValue(ability, CooldownDuration);
    }

    public static float GetCastRange(Entity ability)
    {
        return GetFinalValue(ability, Range);
    }

    public static float GetCastTime(Entity ability)
    {
        return GetFinalValue(ability, CastTime);
    }

    public static float GetChannelDuration(Entity ability)
    {
        return GetFinalValue(ability, ChannelDuration);
    }

    public static float GetDamageAmount(Entity ability)
    {
        return GetFinalValue(ability, DamageAmount);
    }

    public static float GetHealAmount(Entity ability)
    {
        return GetFinalValue(ability, HealAmount);
    }

    public static float GetRadius(Entity ability)
    {
        return GetFinalValue(ability, Radius);
    }

    public static float GetProjectileSpeed(Entity ability)
    {
        return GetFinalValue(ability, ProjectileSpeed);
    }

    public static float GetProjectileDistance(Entity ability)
    {
        return GetFinalValue(ability, ProjectileDistance);
    }

    public static float GetHitWidth(Entity ability)
    {
        return GetFinalValue(ability, HitWidth);
    }

    public static float GetArrivalThreshold(Entity ability)
    {
        return GetFinalValue(ability, ArrivalThreshold);
    }

    public static int GetMaxTargets(Entity ability)
    {
        return (int)GetFinalValue(ability, MaxTargets);
    }
}
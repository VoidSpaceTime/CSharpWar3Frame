using Friflo.Engine.ECS;
using War3Frame.Components;
using War3Frame.Src.Components;

namespace War3Frame.Helpers;

/// <summary>
/// 技能效果 Builder 的新命名入口；迁移期委托给 EffectSpecBuilder。
/// </summary>
public sealed class AbilityEffectSpecBuilder
{
    private readonly EffectSpecBuilder _inner;

    private AbilityEffectSpecBuilder(EffectSpecBuilder inner)
    {
        _inner = inner;
    }

    public static AbilityEffectSpecBuilder Chain()
    {
        return new AbilityEffectSpecBuilder(EffectSpecBuilder.Chain());
    }

    public AbilityEffectSpec Build()
    {
        return new AbilityEffectSpec(_inner.Build());
    }

    public AbilityEffectSpecBuilder Damage(AbilityValue value, DamageType damageType = DamageType.Magical,
        DamageSrc damageSrc = DamageSrc.Skill)
    {
        _inner.Damage(value.EffectValue, damageType, damageSrc);
        return this;
    }

    public AbilityEffectSpecBuilder Heal(AbilityValue value, int valueTypeId = 0, float amount = 0f)
    {
        _inner.Heal(value.EffectValue, valueTypeId, amount);
        return this;
    }

    public AbilityEffectSpecBuilder Buff(string buffId, AbilityValue duration, int attrTypeId, ModifyType modifyType,
        AbilityValue value, BuffRefreshBehavior refreshBehavior = BuffRefreshBehavior.RefreshDuration)
    {
        _inner.Buff(buffId, duration.EffectValue, attrTypeId, modifyType, value.EffectValue, refreshBehavior);
        return this;
    }

    public AbilityEffectSpecBuilder Area(TargetFilter filter, int maxTargets = 0, AbilityValue radius = default,
        string? customFilterId = null, float centerX = 0f, float centerY = 0f)
    {
        _inner.Area(filter, maxTargets, radius.EffectValue, customFilterId, centerX, centerY);
        return this;
    }

    public AbilityEffectSpecBuilder Line(TargetFilter filter, AbilityValue range = default, float fallbackRange = 0f,
        AbilityValue width = default, float fallbackWidth = 0f, int maxTargets = 0,
        string? customFilterId = null, GroundAreaTag reactionTag = GroundAreaTag.None)
    {
        _inner.Line(filter, range.EffectValue, fallbackRange, width.EffectValue, fallbackWidth, maxTargets,
            customFilterId, reactionTag);
        return this;
    }

    public AbilityEffectSpecBuilder GroundArea(GroundAreaTag tags, AbilityValue radius = default,
        float fallbackRadius = 0f, AbilityValue duration = default, float fallbackDuration = 0f,
        GroundAreaBuffData buff = default, GroundAreaPeriodicDamageData periodicDamage = default,
        GroundAreaReactionData reaction = default)
    {
        _inner.GroundArea(tags, radius.EffectValue, fallbackRadius, duration.EffectValue, fallbackDuration,
            buff, periodicDamage, reaction);
        return this;
    }

    public AbilityEffectSpecBuilder Projectile(string model, AbilityValue speed,
        ProjectileTrajectoryType trajectoryType = ProjectileTrajectoryType.Tracking,
        AbilityValue arrivalThreshold = default, AbilityValue maxDistance = default,
        AbilityValue hitRadius = default, TargetFilter hitFilter = TargetFilter.None,
        bool canHitSameTarget = false, Entity effectEntity = default)
    {
        _inner.Projectile(model, speed.EffectValue, trajectoryType, arrivalThreshold.EffectValue,
            maxDistance.EffectValue, hitRadius.EffectValue, hitFilter, canHitSameTarget, effectEntity);
        return this;
    }
}

using Friflo.Engine.ECS;
using War3Frame.Components;
using War3Frame.Src.Components;

namespace War3Frame.Helpers;

public sealed class EffectSpecBuilder
{
    private readonly EffectSpec _spec = new();

    private EffectSpecBuilder()
    {
    }

    public static EffectSpecBuilder Chain()
    {
        return new EffectSpecBuilder();
    }

    public EffectSpec Build()
    {
        return _spec;
    }

    public EffectSpecBuilder Damage(EffectValueSpec value, DamageType damageType = DamageType.Magical,
        DamageSrc damageSrc = DamageSrc.Skill)
    {
        _spec.steps.Add(EffectStepSpec.Damage(new DamageEffectStepSpec
        {
            value = value,
            damageType = damageType,
            damageSrc = damageSrc
        }));
        return this;
    }

    public EffectSpecBuilder Damage(string formulaId, int statId, DamageType damageType = DamageType.Magical,
        DamageSrc damageSrc = DamageSrc.Skill, Dictionary<string, float>? parameters = null)
    {
        return Damage(EffectValueSpec.Stat(statId, formulaId, parameters), damageType, damageSrc);
    }

    public EffectSpecBuilder Heal(EffectValueSpec value, int valueTypeId = 0, float amount = 0f)
    {
        _spec.steps.Add(EffectStepSpec.Heal(new HealEffectStepSpec
        {
            value = value,
            valueTypeId = valueTypeId,
            amount = amount
        }));
        return this;
    }

    public EffectSpecBuilder Heal(string formulaId, int statId, Dictionary<string, float>? parameters = null)
    {
        return Heal(EffectValueSpec.Stat(statId, formulaId, parameters), statId);
    }

    public EffectSpecBuilder Buff(string buffId, EffectValueSpec duration, int attrTypeId, ModifyType modifyType,
        EffectValueSpec value, BuffRefreshBehavior refreshBehavior = BuffRefreshBehavior.RefreshDuration)
    {
        _spec.steps.Add(EffectStepSpec.Buff(new BuffEffectStepSpec
        {
            buffId = buffId,
            duration = duration,
            attrTypeId = attrTypeId,
            modifyType = modifyType,
            value = value,
            refreshBehavior = refreshBehavior
        }));
        return this;
    }

    public EffectSpecBuilder Area(TargetFilter filter, int maxTargets = 0, EffectValueSpec radius = default,
        string? customFilterId = null, float centerX = 0f, float centerY = 0f)
    {
        _spec.steps.Add(EffectStepSpec.AreaSearch(new AreaSearchEffectStepSpec
        {
            centerX = centerX,
            centerY = centerY,
            radius = radius,
            maxTargets = maxTargets,
            filter = filter,
            customFilterId = customFilterId
        }));
        return this;
    }

    public EffectSpecBuilder Projectile(string model, EffectValueSpec speed,
        ProjectileTrajectoryType trajectoryType = ProjectileTrajectoryType.Tracking,
        EffectValueSpec arrivalThreshold = default, EffectValueSpec maxDistance = default,
        EffectValueSpec hitRadius = default, TargetFilter hitFilter = TargetFilter.None,
        bool canHitSameTarget = false, Entity effectEntity = default)
    {
        _spec.steps.Add(EffectStepSpec.Projectile(new ProjectileEffectStepSpec
        {
            trajectoryType = trajectoryType,
            model = model,
            speed = speed,
            effectEntity = effectEntity,
            arrivalThreshold = arrivalThreshold,
            maxDistance = maxDistance,
            hitRadius = hitRadius,
            hitFilter = hitFilter,
            canHitSameTarget = canHitSameTarget
        }));
        return this;
    }
}

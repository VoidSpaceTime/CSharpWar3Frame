using Friflo.Engine.ECS;
using War3Frame.Helpers;

namespace War3Frame;

public static class AbilityEffectHelper
{
    private static int _nextEffectId = 1;

    public static Entity CreateEffectEntity(Entity caster, Entity ability,
        Entity targetUnit, float targetX, float targetY)
    {
        var effectEntity = Game.Store.CreateEntity(
            new EffectSource { caster = caster, ability = ability },
            new EffectTargetInfo
            {
                targetUnit = targetUnit,
                targetX = targetX,
                targetY = targetY
            },
            new AbilityEffectContext
            {
                caster = caster,
                ability = ability,
                sourceEffect = default,
                targetUnit = targetUnit,
                targetX = targetX,
                targetY = targetY,
                effectId = _nextEffectId++
            });
        effectEntity.AddTag<EffectPending>();

        if (ability.TryGetComponent<HealEffectData>(out var heal))
        {
            if (heal.valueTypeId == 0)
                heal.valueTypeId = AbilityHelper.HealAmount;
            effectEntity.AddComponent(heal);
        }

        if (ability.TryGetComponent<ApplyBuffData>(out var buff))
            effectEntity.AddComponent(buff);

        if (ability.TryGetComponent<AreaSearchData>(out var area))
        {
            if (area.centerX == 0f && area.centerY == 0f)
            {
                area.centerX = targetX;
                area.centerY = targetY;
            }

            effectEntity.AddComponent(area);
        }

        if (ability.TryGetComponent<DamageEffectData>(out var damage))
            effectEntity.AddComponent(damage);

        if (ability.TryGetComponent<ProjectileData>(out var projectile))
        {
            if (projectile.arrivalThreshold <= 0f)
                projectile.arrivalThreshold = 30f;

            if (projectile.trajectoryType == default)
                projectile.trajectoryType = ProjectileTrajectoryType.Tracking;

            effectEntity.AddComponent(projectile);
            EnsureProjectilePosition(effectEntity, caster);
            EnsureProjectileRuntimeState(effectEntity);
        }

        return effectEntity;
    }

    public static Entity CreateChildEffect(Entity parentEffect, Entity target)
    {
        var source = parentEffect.GetComponent<EffectSource>();

        var childEntity = Game.Store.CreateEntity(
            source,
            new EffectTargetInfo
            {
                targetUnit = target,
                targetX = 0,
                targetY = 0
            },
            new AbilityEffectContext
            {
                caster = source.caster,
                ability = source.ability,
                sourceEffect = parentEffect,
                targetUnit = target,
                targetX = 0,
                targetY = 0,
                effectId = _nextEffectId++
            });
        childEntity.AddTag<EffectPending>();

        if (parentEffect.TryGetComponent<DamageEffectData>(out var damage))
            childEntity.AddComponent(damage);

        if (parentEffect.TryGetComponent<HealEffectData>(out var heal))
            childEntity.AddComponent(heal);

        if (parentEffect.TryGetComponent<ApplyBuffData>(out var buff))
            childEntity.AddComponent(buff);

        return childEntity;
    }

    private static void EnsureProjectilePosition(Entity effectEntity, Entity caster)
    {
        if (effectEntity.TryGetComponent<Position>(out _))
            return;

        if (caster.TryGetComponent<Position>(out var casterPos))
        {
            effectEntity.AddComponent(new Position
            {
                x = casterPos.x,
                y = casterPos.y,
                z = casterPos.z
            });
        }
    }

    private static void EnsureProjectileRuntimeState(Entity effectEntity)
    {
        if (!effectEntity.TryGetComponent<ProjectileRuntimeState>(out _))
        {
            effectEntity.AddComponent(new ProjectileRuntimeState
            {
                phase = ProjectileLifecyclePhase.PendingStart
            });
        }
    }
}

using System.Numerics;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components.AbilityEffectExtend;
using War3Frame.Helpers;
using War3Frame.Src.Components;
using War3Frame.Systems;
using War3Frame.TemplateInit;

namespace War3Frame;

[SystemRegister(SystemKind.Interval, 100)]
public class ProjectileSystem : QuerySystem<ProjectileData, EffectSource, EffectTargetInfo, Position, ProjectileRuntimeState>
{
    public ProjectileSystem()
    {
        Filter.AnyTags(Tags.Get<EffectPending>());
    }

    protected override void OnUpdate()
    {
        var arriveRequests = new List<Entity>();
        var expireRequests = new List<Entity>();

        Query.ForEachEntity((ref ProjectileData projectile, ref EffectSource source,
            ref EffectTargetInfo target, ref Position position,
            ref ProjectileRuntimeState runtimeState, Entity effectEntity) =>
        {
            if (!ProjectileFlowHelper.ShouldProcess(runtimeState))
                return;

            NormalizeProjectileDefaults(ref projectile);

            if (runtimeState.phase == ProjectileLifecyclePhase.PendingStart)
            {
                InitializeProjectileRuntime(ref projectile, ref target, ref position, ref runtimeState);
                ProjectileHookBridge.DispatchStartHooks(effectEntity, ref projectile, ref source, ref target, ref position, ref runtimeState);
                runtimeState.phase = ProjectileLifecyclePhase.InFlight;
            }

            var decision = ProjectileHookBridge.DispatchTravelHooks(effectEntity, ref projectile, ref source, ref target, ref position, ref runtimeState);
            if (decision == ProjectileTravelDecision.RequestExpire)
            {
                runtimeState.phase = ProjectileLifecyclePhase.ExpireRequested;
                expireRequests.Add(effectEntity);
                return;
            }

            var arrived = projectile.trajectoryType switch
            {
                ProjectileTrajectoryType.Linear => UpdateLinear(ref projectile, ref position, ref runtimeState, Tick.deltaTime),
                ProjectileTrajectoryType.Bezier => UpdateBezier(ref projectile, ref target, ref position, ref runtimeState, Tick.deltaTime),
                ProjectileTrajectoryType.Parabolic => UpdateParabolic(ref projectile, ref target, ref position, ref runtimeState, Tick.deltaTime),
                ProjectileTrajectoryType.Sinusoidal => UpdateSinusoidal(ref projectile, ref target, ref position, ref runtimeState, Tick.deltaTime),
                ProjectileTrajectoryType.Spiral => UpdateSpiral(ref projectile, ref target, ref position, ref runtimeState, Tick.deltaTime),
                _ => UpdateTracking(ref projectile, ref target, ref position, Tick.deltaTime)
            };

            if (!projectile.effectEntity.IsNull)
            {
                EffectHelper.SetPosition(projectile.effectEntity, position.x, position.y, position.z);
            }

            if (arrived && decision != ProjectileTravelDecision.SuppressArrivalThisTick)
            {
                runtimeState.phase = ProjectileLifecyclePhase.ArriveRequested;
                arriveRequests.Add(effectEntity);
            }
        });

        ProjectileFlowHelper.ApplyRequests(arriveRequests, expireRequests);
    }

    private static void NormalizeProjectileDefaults(ref ProjectileData projectile)
    {
        if (projectile.arrivalThreshold <= 0f)
            projectile.arrivalThreshold = 30f;

        if (projectile.trajectoryType == default)
            projectile.trajectoryType = ProjectileTrajectoryType.Tracking;
    }

    private static void InitializeProjectileRuntime(ref ProjectileData projectile, ref EffectTargetInfo target,
        ref Position position, ref ProjectileRuntimeState runtimeState)
    {
        if (projectile.trajectoryType == ProjectileTrajectoryType.Linear)
        {
            var dx = target.targetX - position.x;
            var dy = target.targetY - position.y;
            var dist = MathF.Sqrt(dx * dx + dy * dy);
            if (dist > float.Epsilon)
            {
                runtimeState.dirX = dx / dist;
                runtimeState.dirY = dy / dist;
            }
        }
    }

    private static bool UpdateTracking(ref ProjectileData projectile, ref EffectTargetInfo target, ref Position position,
        float deltaTime)
    {
        var tx = target.targetX;
        var ty = target.targetY;
        if (!target.targetUnit.IsNull && target.targetUnit.TryGetComponent<Position>(out var targetPos))
        {
            tx = targetPos.x;
            ty = targetPos.y;
            target.targetX = tx;
            target.targetY = ty;
        }

        var dx = tx - position.x;
        var dy = ty - position.y;
        var dist = MathF.Sqrt(dx * dx + dy * dy);
        if (dist <= projectile.arrivalThreshold)
            return true;

        var move = MathF.Min(projectile.speed * deltaTime, dist);
        position.x += dx / dist * move;
        position.y += dy / dist * move;
        return false;
    }

    private static bool UpdateLinear(ref ProjectileData projectile, ref Position position,
        ref ProjectileRuntimeState runtimeState, float deltaTime)
    {
        var maxDistance = projectile.maxDistance > 0f ? projectile.maxDistance : float.MaxValue;
        var remaining = MathF.Max(0f, maxDistance - runtimeState.traveled);
        if (remaining <= 0f)
            return true;

        var move = MathF.Min(projectile.speed * deltaTime, remaining);
        position.x += runtimeState.dirX * move;
        position.y += runtimeState.dirY * move;
        runtimeState.traveled += move;

        return runtimeState.traveled >= maxDistance;
    }

    private static bool UpdateBezier(ref ProjectileData projectile, ref EffectTargetInfo target,
        ref Position position, ref ProjectileRuntimeState runtimeState, float deltaTime)
    {
        return UpdateCurve(projectile.speed, target.targetX, target.targetY, ref position, ref runtimeState,
            CurveKind.Bezier, deltaTime);
    }

    private static bool UpdateParabolic(ref ProjectileData projectile, ref EffectTargetInfo target,
        ref Position position, ref ProjectileRuntimeState runtimeState, float deltaTime)
    {
        return UpdateCurve(projectile.speed, target.targetX, target.targetY, ref position, ref runtimeState,
            CurveKind.Parabolic, deltaTime);
    }

    private static bool UpdateSinusoidal(ref ProjectileData projectile, ref EffectTargetInfo target,
        ref Position position, ref ProjectileRuntimeState runtimeState, float deltaTime)
    {
        return UpdateCurve(projectile.speed, target.targetX, target.targetY, ref position, ref runtimeState,
            CurveKind.Sinusoidal, deltaTime);
    }

    private static bool UpdateSpiral(ref ProjectileData projectile, ref EffectTargetInfo target,
        ref Position position, ref ProjectileRuntimeState runtimeState, float deltaTime)
    {
        return UpdateCurve(projectile.speed, target.targetX, target.targetY, ref position, ref runtimeState,
            CurveKind.Spiral, deltaTime);
    }

    private enum CurveKind
    {
        Bezier,
        Parabolic,
        Sinusoidal,
        Spiral
    }

    private static bool UpdateCurve(float speed, float targetX, float targetY,
        ref Position position, ref ProjectileRuntimeState runtimeState, CurveKind kind, float deltaTime)
    {
        var start = runtimeState.normalizedProgress <= float.Epsilon
            ? new Vector3(position.x, position.y, position.z)
            : new Vector3(position.x, position.y, position.z);
        var end = new Vector3(targetX, targetY, position.z);
        var totalDist = MathF.Max(Vector3.Distance(start, end), 1f);
        runtimeState.normalizedProgress = MathF.Min(1f, runtimeState.normalizedProgress + speed * deltaTime / totalDist);
        var t = runtimeState.normalizedProgress;

        var linear = Vector3.Lerp(start, end, t);
        var direction = Vector3.Normalize(end - start);
        if (float.IsNaN(direction.X) || float.IsNaN(direction.Y))
            direction = Vector3.UnitX;
        var perpendicular = new Vector3(-direction.Y, direction.X, 0f);
        var offset = Vector3.Zero;

        switch (kind)
        {
            case CurveKind.Bezier:
                if (runtimeState.controlPoint1 == default && runtimeState.controlPoint2 == default)
                {
                    var mid = (start + end) * 0.5f;
                    var arc = totalDist * 0.3f;
                    runtimeState.controlPoint1 = Vector3.Lerp(start, mid, 0.5f) + perpendicular * arc;
                    runtimeState.controlPoint2 = Vector3.Lerp(mid, end, 0.5f) + perpendicular * arc;
                }

                var oneMinusT = 1f - t;
                linear = oneMinusT * oneMinusT * oneMinusT * start +
                         3f * oneMinusT * oneMinusT * t * runtimeState.controlPoint1 +
                         3f * oneMinusT * t * t * runtimeState.controlPoint2 +
                         t * t * t * end;
                break;
            case CurveKind.Parabolic:
                offset = new Vector3(0f, 0f, 4f * totalDist * 0.4f * t * (1f - t));
                break;
            case CurveKind.Sinusoidal:
                offset = perpendicular * (MathF.Sin(t * MathF.PI * 3f) * totalDist * 0.15f);
                break;
            case CurveKind.Spiral:
                var radius = totalDist * 0.2f * (1f - t);
                var angle = t * MathF.PI * 6f;
                offset = perpendicular * (MathF.Cos(angle) * radius) + new Vector3(0f, 0f, MathF.Sin(angle) * radius);
                break;
        }

        var final = linear + offset;
        position.x = final.X;
        position.y = final.Y;
        position.z = final.Z;
        return t >= 1f;
    }
}

[SystemRegister(SystemKind.Interval, 102)]
public class ProjectileLifecycleApplySystem : QuerySystem<ProjectileRuntimeState, EffectSource, EffectTargetInfo, Position>
{
    public ProjectileLifecycleApplySystem()
    {
        Filter.AnyTags(Tags.Get<EffectPending>());
    }

    protected override void OnUpdate()
    {
        var toArrive = new List<Entity>();
        var toExpire = new List<Entity>();

        Query.ForEachEntity((ref ProjectileRuntimeState runtimeState, ref EffectSource source,
            ref EffectTargetInfo target, ref Position pos, Entity effectEntity) =>
        {
            if (effectEntity.Tags.Has<ProjectileArriveRequest>())
                toArrive.Add(effectEntity);

            if (effectEntity.Tags.Has<ProjectileExpireRequest>())
                toExpire.Add(effectEntity);
        });

        foreach (var effectEntity in toArrive)
        {
            if (!effectEntity.TryGetComponent<ProjectileRuntimeState>(out var runtimeState) ||
                !effectEntity.TryGetComponent<EffectSource>(out var source) ||
                !effectEntity.TryGetComponent<EffectTargetInfo>(out var target) ||
                !effectEntity.TryGetComponent<Position>(out var pos))
            {
                continue;
            }

            runtimeState.phase = ProjectileLifecyclePhase.Arrived;
            effectEntity.AddComponent(runtimeState);
            effectEntity.RemoveTag<ProjectileArriveRequest>();
            if (!effectEntity.Tags.Has<ProjectileArrived>())
                effectEntity.AddTag<ProjectileArrived>();

            ProjectileFlowHelper.DestroyProjectileVisual(effectEntity);
            ProjectileHookBridge.DispatchArriveHooks(effectEntity, ref source, ref target, ref pos, ref runtimeState);
            effectEntity.AddComponent(runtimeState);
        }

        foreach (var effectEntity in toExpire)
        {
            if (effectEntity.TryGetComponent<ProjectileRuntimeState>(out var runtimeState))
            {
                runtimeState.phase = ProjectileLifecyclePhase.Expired;
                effectEntity.AddComponent(runtimeState);
            }

            effectEntity.RemoveTag<ProjectileExpireRequest>();
            ProjectileFlowHelper.DestroyProjectileVisual(effectEntity);
            effectEntity.AddTag<EffectExpired>();
        }
    }
}

[SystemRegister(SystemKind.Interval, 110)]
public class AreaSearchSystem : QuerySystem<AreaSearchData, EffectSource, EffectTargetInfo>
{
    public AreaSearchSystem()
    {
        Filter.AnyTags(Tags.Get<EffectPending>());
    }

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref AreaSearchData area, ref EffectSource source,
            ref EffectTargetInfo target, Entity effectEntity) =>
        {
            if (ProjectileFlowHelper.HasPendingProjectile(effectEntity))
                return;

            var radius = AbilityHelper.GetRadius(source.ability);
            var centerX = area.centerX == 0f && area.centerY == 0f ? target.targetX : area.centerX;
            var centerY = area.centerX == 0f && area.centerY == 0f ? target.targetY : area.centerY;

            var targets = GroupHelper.FindInCircle(
                source.caster,
                centerX,
                centerY,
                radius,
                area.filter,
                area.customFilterId,
                area.maxTargets);

            foreach (var targetUnit in targets)
            {
                AbilityEffectHelper.CreateChildEffect(effectEntity, targetUnit);
            }

            effectEntity.AddTag<EffectCompleted>();
        });
    }
}

[SystemRegister(SystemKind.Interval, 120)]
public class DamageEffectSystem : QuerySystem<DamageEffectData, EffectSource, EffectTargetInfo>
{
    public DamageEffectSystem()
    {
        Filter.AnyTags(Tags.Get<EffectPending>());
    }

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref DamageEffectData damageData, ref EffectSource source,
            ref EffectTargetInfo target, Entity effectEntity) =>
        {
            if (!EffectSettlementHelper.CanSettle(effectEntity))
                return;

            if (target.targetUnit.IsNull)
            {
                EffectSettlementHelper.MarkSettlementDone(effectEntity, typeof(DamageEffectData));
                return;
            }

            var amount = damageData.damageFunc != null
                ? damageData.damageFunc(source.caster, source.ability, target.targetUnit, damageData)
                : AbilityHelper.GetDamageAmount(source.ability);

            Game.Store.CreateEntity(new DamageRequest
            {
                source = source.caster,
                target = target.targetUnit,
                damage = new DamageBase
                {
                    damage = amount,
                    damageType = damageData.damageType,
                    damageSrc = damageData.damageSrc,
                    source = source.caster,
                    target = target.targetUnit
                }
            });

            EffectSettlementHelper.MarkSettlementDone(effectEntity, typeof(DamageEffectData));
        });
    }
}

[SystemRegister(SystemKind.Interval, 121)]
public class HealEffectSystem : QuerySystem<HealEffectData, EffectSource, EffectTargetInfo>
{
    public HealEffectSystem()
    {
        Filter.AnyTags(Tags.Get<EffectPending>());
    }

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref HealEffectData heal, ref EffectSource source,
            ref EffectTargetInfo target, Entity effectEntity) =>
        {
            if (!EffectSettlementHelper.CanSettle(effectEntity))
                return;

            if (target.targetUnit.IsNull)
            {
                EffectSettlementHelper.MarkSettlementDone(effectEntity, typeof(HealEffectData));
                return;
            }

            var amount = heal.healFunc != null
                ? heal.healFunc(source.caster, source.ability, target.targetUnit, heal)
                : heal.amount > 0f
                    ? heal.amount
                    : AbilityHelper.GetHealAmount(source.ability);

            Game.Store.CreateEntity(new HealRequest
            {
                source = source.caster,
                target = target.targetUnit,
                amount = amount
            });

            EffectSettlementHelper.MarkSettlementDone(effectEntity, typeof(HealEffectData));
        });
    }
}

[SystemRegister(SystemKind.Interval, 122)]
public class BuffEffectSystem : QuerySystem<ApplyBuffData, EffectSource, EffectTargetInfo>
{
    public BuffEffectSystem()
    {
        Filter.AnyTags(Tags.Get<EffectPending>());
    }

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref ApplyBuffData buffData, ref EffectSource source,
            ref EffectTargetInfo target, Entity effectEntity) =>
        {
            if (!EffectSettlementHelper.CanSettle(effectEntity))
                return;

            if (target.targetUnit.IsNull)
            {
                EffectSettlementHelper.MarkSettlementDone(effectEntity, typeof(ApplyBuffData));
                return;
            }

            Game.Store.CreateEntity(new BuffApplyRequest
            {
                source = source.caster,
                target = target.targetUnit,
                buffId = buffData.buffId,
                attrTypeId = buffData.attrTypeId,
                modifyType = buffData.modifyType,
                value = buffData.value,
                duration = buffData.duration,
                refreshBehavior = buffData.refreshBehavior
            });

            EffectSettlementHelper.MarkSettlementDone(effectEntity, typeof(ApplyBuffData));
        });
    }
}

[SystemRegister(SystemKind.Interval, 125)]
public class DamageResolveSystem : QuerySystem<DamageRequest>
{
    protected override void OnUpdate()
    {
        var resolved = new List<Entity>();

        Query.ForEachEntity((ref DamageRequest request, Entity requestEntity) =>
        {
            if (request.target.IsNull)
            {
                resolved.Add(requestEntity);
                return;
            }

            var finalDamage = MathF.Max(0f, request.damage.damage);
            var remaining = AttributeHelper.ModifyCurrent(request.target, AttributeHelper.Health, -finalDamage);

            Game.Store.CreateEntity(new DamageEvent
            {
                source = request.source,
                target = request.target,
                damage = request.damage,
                finalDamage = finalDamage,
                remainingHealth = remaining
            });

            if (remaining <= 0f)
                UnitHelper.KillUnit(request.target);

            resolved.Add(requestEntity);
        });

        foreach (var entity in resolved)
            entity.DeleteEntity();
    }
}

[SystemRegister(SystemKind.Interval, 126)]
public class HealResolveSystem : QuerySystem<HealRequest>
{
    protected override void OnUpdate()
    {
        var resolved = new List<Entity>();

        Query.ForEachEntity((ref HealRequest request, Entity requestEntity) =>
        {
            if (request.target.IsNull)
            {
                resolved.Add(requestEntity);
                return;
            }

            var finalHeal = MathF.Max(0f, request.amount);
            var remaining = AttributeHelper.ModifyCurrent(request.target, AttributeHelper.Health, finalHeal);

            Game.Store.CreateEntity(new HealEvent
            {
                source = request.source,
                target = request.target,
                baseHeal = request.amount,
                finalHeal = finalHeal,
                remainingHealth = remaining
            });

            resolved.Add(requestEntity);
        });

        foreach (var entity in resolved)
            entity.DeleteEntity();
    }
}

[SystemRegister(SystemKind.Interval, 127)]
public class BuffApplyResolveSystem : QuerySystem<BuffApplyRequest>
{
    protected override void OnUpdate()
    {
        var resolved = new List<Entity>();

        Query.ForEachEntity((ref BuffApplyRequest request, Entity requestEntity) =>
        {
            if (request.target.IsNull)
            {
                resolved.Add(requestEntity);
                return;
            }

            var buff = BuffHelper.AddTimedBuff(
                Game.Store,
                request.target,
                request.source,
                request.buffId,
                request.attrTypeId,
                request.modifyType,
                request.value,
                request.duration,
                request.refreshBehavior);

            Game.Store.CreateEntity(new BuffAppliedEvent
            {
                source = request.source,
                target = request.target,
                buff = buff,
                buffId = request.buffId
            });

            resolved.Add(requestEntity);
        });

        foreach (var entity in resolved)
            entity.DeleteEntity();
    }
}

[SystemRegister(SystemKind.Interval, 130)]
public class EffectLifecycleSystem : QuerySystem<EffectSource>
{
    public EffectLifecycleSystem()
    {
        Filter.AnyTags(Tags.Get<EffectPending>());
    }

    protected override void OnUpdate()
    {
        var toDelete = new List<Entity>();

        Query.ForEachEntity((ref EffectSource source, Entity effectEntity) =>
        {
            if (effectEntity.Tags.Has<EffectExpired>() || effectEntity.Tags.Has<EffectCompleted>())
                toDelete.Add(effectEntity);
        });

        foreach (var entity in toDelete)
            entity.DeleteEntity();
    }
}

internal static class EffectSettlementHelper
{
    public static bool CanSettle(Entity effectEntity)
    {
        if (ProjectileFlowHelper.HasPendingProjectile(effectEntity))
            return false;

        return !effectEntity.HasComponent<AreaSearchData>();
    }

    public static void MarkSettlementDone(Entity effectEntity, Type settlementType)
    {
        if (settlementType == typeof(DamageEffectData))
            effectEntity.RemoveComponent<DamageEffectData>();
        else if (settlementType == typeof(HealEffectData))
            effectEntity.RemoveComponent<HealEffectData>();
        else if (settlementType == typeof(ApplyBuffData))
            effectEntity.RemoveComponent<ApplyBuffData>();

        if (!effectEntity.HasComponent<DamageEffectData>() &&
            !effectEntity.HasComponent<HealEffectData>() &&
            !effectEntity.HasComponent<ApplyBuffData>())
        {
            effectEntity.AddTag<EffectCompleted>();
        }
    }
}

internal static class ProjectileFlowHelper
{
    public static bool ShouldProcess(ProjectileRuntimeState state)
    {
        return state.phase is ProjectileLifecyclePhase.PendingStart or ProjectileLifecyclePhase.InFlight;
    }

    public static bool HasPendingProjectile(Entity effectEntity)
    {
        return effectEntity.HasComponent<ProjectileData>() && !effectEntity.Tags.Has<ProjectileArrived>();
    }

    public static void ApplyRequests(List<Entity> arriveRequests, List<Entity> expireRequests)
    {
        foreach (var effectEntity in arriveRequests)
        {
            if (!effectEntity.Tags.Has<ProjectileArriveRequest>())
                effectEntity.AddTag<ProjectileArriveRequest>();
        }

        foreach (var effectEntity in expireRequests)
        {
            if (!effectEntity.Tags.Has<ProjectileExpireRequest>())
                effectEntity.AddTag<ProjectileExpireRequest>();
        }
    }

    public static void DestroyProjectileVisual(Entity effectEntity)
    {
        if (effectEntity.TryGetComponent<ProjectileData>(out var projectile) && !projectile.effectEntity.IsNull)
            EffectHelper.Destroy(projectile.effectEntity, hideFirst: true);
    }
}

internal static class ProjectileHookBridge
{
    public static void DispatchStartHooks(Entity effectEntity, ref ProjectileData projectile,
        ref EffectSource source, ref EffectTargetInfo target, ref Position position,
        ref ProjectileRuntimeState runtimeState)
    {
        if (!TryResolveTemplate(source.ability, out var template))
            return;

        if (template is AbilityTemplateBase templateBase)
            templateBase.OnProjectileStart(effectEntity, ref source, ref target, ref position, ref runtimeState);

        if (template is IProjectileOnStart legacyStart)
        {
            var legacyProjectile = CreateLegacyProjectile(source, target, position, projectile.speed);
            legacyStart.ProjectileOnStart(ref legacyProjectile, ref position, effectEntity);
            ApplyLegacyProjectile(ref legacyProjectile, ref target, ref projectile.speed);
        }

        if (template is IProjectileHooksV2 hooksV2)
            hooksV2.OnStart(effectEntity, ref source, ref target, ref position, ref runtimeState);
    }

    public static ProjectileTravelDecision DispatchTravelHooks(Entity effectEntity, ref ProjectileData projectile,
        ref EffectSource source, ref EffectTargetInfo target, ref Position position,
        ref ProjectileRuntimeState runtimeState)
    {
        if (!TryResolveTemplate(source.ability, out var template))
            return ProjectileTravelDecision.Continue;

        var decision = ProjectileTravelDecision.Continue;

        if (template is AbilityTemplateBase templateBase)
        {
            decision = MergeDecision(decision,
                templateBase.OnProjectileTravel(effectEntity, ref source, ref target, ref position, ref runtimeState));
        }

        if (template is IProjectileOnTravel legacyTravel)
        {
            var legacyProjectile = CreateLegacyProjectile(source, target, position, projectile.speed);
            var allowArrive = legacyTravel.ProjectileOnTravel(ref legacyProjectile, ref position, effectEntity);
            ApplyLegacyProjectile(ref legacyProjectile, ref target, ref projectile.speed);
            if (!allowArrive)
                decision = MergeDecision(decision, ProjectileTravelDecision.SuppressArrivalThisTick);
        }

        if (template is IProjectileHooksV2 hooksV2)
        {
            decision = MergeDecision(decision,
                hooksV2.OnTravel(effectEntity, ref source, ref target, ref position, ref runtimeState));
        }

        return decision;
    }

    public static void DispatchArriveHooks(Entity effectEntity, ref EffectSource source,
        ref EffectTargetInfo target, ref Position position, ref ProjectileRuntimeState runtimeState)
    {
        if (!TryResolveTemplate(source.ability, out var template))
            return;

        var speed = effectEntity.TryGetComponent<ProjectileData>(out var projectile) ? projectile.speed : 0f;

        if (template is AbilityTemplateBase templateBase)
            templateBase.OnProjectileArrive(effectEntity, ref source, ref target, ref position, ref runtimeState);

        if (template is IProjectileOnArrive legacyArrive)
        {
            var legacyProjectile = CreateLegacyProjectile(source, target, position, speed);
            legacyArrive.ProjectileOnArrive(ref legacyProjectile, ref position, effectEntity);
        }

        if (template is IProjectileHooksV2 hooksV2)
            hooksV2.OnArrive(effectEntity, ref source, ref target, ref position, ref runtimeState);
    }

    private static ProjectileTravelDecision MergeDecision(ProjectileTravelDecision current, ProjectileTravelDecision incoming)
    {
        if (current == ProjectileTravelDecision.RequestExpire || incoming == ProjectileTravelDecision.RequestExpire)
            return ProjectileTravelDecision.RequestExpire;

        if (current == ProjectileTravelDecision.SuppressArrivalThisTick ||
            incoming == ProjectileTravelDecision.SuppressArrivalThisTick)
            return ProjectileTravelDecision.SuppressArrivalThisTick;

        return ProjectileTravelDecision.Continue;
    }

    private static ProjectileBase CreateLegacyProjectile(EffectSource source, EffectTargetInfo target,
        Position position, float speed)
    {
        return new ProjectileBase
        {
            TargetEntity = target.targetUnit.IsNull ? null : target.targetUnit,
            SourceAbility = source.ability,
            SourceEntity = source.caster,
            targetX = target.targetX,
            targetY = target.targetY,
            targetZ = position.z,
            speed = speed,
            height = position.z,
            startX = position.x,
            startY = position.y
        };
    }

    private static void ApplyLegacyProjectile(ref ProjectileBase projectile, ref EffectTargetInfo target, ref float speed)
    {
        target.targetX = projectile.targetX;
        target.targetY = projectile.targetY;
        speed = projectile.speed;
    }

    private static bool TryResolveTemplate(Entity abilityEntity, out IAbilityTemplate template)
    {
        template = null!;
        if (!abilityEntity.TryGetComponent(out AbilityBase abilityBase))
            return false;

        return AbilityTemplate.TryGet(abilityBase.templateName, out template);
    }
}

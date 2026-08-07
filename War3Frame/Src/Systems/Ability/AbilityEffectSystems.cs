using System.Numerics;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;
using War3Frame.Components.AbilityEffectExtend;
using War3Frame.Helpers;
using War3Frame.Src.Components;
using War3Frame.Systems;
using War3Frame.TemplateInit;

namespace War3Frame;

/// <summary>
/// 弹道推进系统。
/// 只推进运行时位置与弹道阶段，到达/过期通过请求标记交给后续系统处理。
/// </summary>
[SystemRegister(SystemKind.Interval, 100)]
public class ProjectileSystem : QuerySystem<ProjectileData, EffectSource, EffectTargetInfo, Position, ProjectileRuntimeState>
{
    private readonly List<Entity> _arriveRequests = new();
    private readonly List<Entity> _expireRequests = new();

    public ProjectileSystem()
    {
        Filter.AnyTags(Tags.Get<EffectPending>());
    }

    protected override void OnUpdate()
    {
        _arriveRequests.Clear();
        _expireRequests.Clear();

        Query.ForEachEntity((ref ProjectileData projectile, ref EffectSource source,
            ref EffectTargetInfo target, ref Position position,
            ref ProjectileRuntimeState runtimeState, Entity effectEntity) =>
        {
            if (!ProjectileFlowHelper.ShouldProcess(runtimeState))
                return;

            NormalizeProjectileDefaults(ref projectile, ref source, ref target, effectEntity);

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
                _expireRequests.Add(effectEntity);
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
                _arriveRequests.Add(effectEntity);
            }
        });

        ProjectileFlowHelper.ApplyRequests(_arriveRequests, _expireRequests);
    }

    private static void NormalizeProjectileDefaults(ref ProjectileData projectile, ref EffectSource source,
        ref EffectTargetInfo target, Entity effectEntity)
    {
        // spec 数值优先，旧 float 字段和 AbilityHelper stat 作为兼容回退。
        var legacySpeed = projectile.speed;
        var sourceAbility = source.ability;
        projectile.speed = EffectFormulaRegistry.Resolve(
            source.caster,
            source.ability,
            target.targetUnit,
            effectEntity,
            projectile.speedValue,
            () => legacySpeed > 0f
                ? legacySpeed
                : AbilityHelper.GetFinalValue(sourceAbility, AbilityHelper.ProjectileSpeed));

        var fallbackArrivalThreshold = projectile.arrivalThreshold;
        projectile.arrivalThreshold = EffectFormulaRegistry.Resolve(
            source.caster,
            source.ability,
            target.targetUnit,
            effectEntity,
            projectile.arrivalThresholdValue,
            fallbackArrivalThreshold);

        var fallbackMaxDistance = projectile.maxDistance;
        projectile.maxDistance = EffectFormulaRegistry.Resolve(
            source.caster,
            source.ability,
            target.targetUnit,
            effectEntity,
            projectile.maxDistanceValue,
            fallbackMaxDistance);

        var fallbackHitRadius = projectile.hitRadius;
        projectile.hitRadius = EffectFormulaRegistry.Resolve(
            source.caster,
            source.ability,
            target.targetUnit,
            effectEntity,
            projectile.hitRadiusValue,
            fallbackHitRadius);

        if (projectile.arrivalThreshold <= 0f)
            projectile.arrivalThreshold = 30f;

        if (projectile.trajectoryType == default)
            projectile.trajectoryType = ProjectileTrajectoryType.Tracking;
    }

    private static void InitializeProjectileRuntime(ref ProjectileData projectile, ref EffectTargetInfo target,
        ref Position position, ref ProjectileRuntimeState runtimeState)
    {
        // Linear 轨迹只在启动时确定方向，后续按该方向飞行直到最大距离。
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
        // 曲线类轨迹使用统一进度推进，避免每种曲线重复生命周期逻辑。
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

/// <summary>
/// 弹道生命周期应用系统。
/// 将 ProjectileSystem 产生的请求标记转换为 Arrived / Expired 状态，并调用模板 hook。
/// </summary>
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
            if (effectEntity.TryGetComponent<ProjectileData>(out var projectile) && projectile.arriveEffect != null)
                AbilityEffectHelper.CreateArriveEffect(effectEntity, projectile.arriveEffect, pos);
            if (!EffectSettlementHelper.HasSettlementPayload(effectEntity))
                effectEntity.AddTag<EffectCompleted>();
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

/// <summary>
/// 视觉特效步骤系统。
/// 只把技能效果链中的视觉描述转换为 ECS 特效实体或销毁请求，原生调用仍由 EffectNativeSystem 执行。
/// </summary>
[SystemRegister(SystemKind.Interval, 109)]
public class EffectVisualSystem : QuerySystem<EffectVisualData, EffectSource, EffectTargetInfo>
{
    public EffectVisualSystem()
    {
        Filter.AnyTags(Tags.Get<EffectPending>());
    }

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref EffectVisualData visual, ref EffectSource source,
            ref EffectTargetInfo target, Entity effectEntity) =>
        {
            if (ProjectileFlowHelper.HasPendingProjectile(effectEntity))
                return;

            var current = GetCurrentVisualStep(visual);
            if (effectEntity.HasComponent<AreaSearchData>() && NeedsResolvedTarget(current.kind) && target.targetUnit.IsNull)
                return;

            if (current.kind == EffectVisualKind.RemoveByKey)
            {
                RemoveLinkedEffects(source.caster, current.key);
                CompleteCurrentVisual(effectEntity, visual);
                return;
            }

            var duration = EffectFormulaRegistry.Resolve(
                source.caster,
                source.ability,
                target.targetUnit,
                effectEntity,
                current.duration,
                current.fallbackDuration);
            var visualEntity = CreateVisualEntity(current, source, target, duration);
            if (visualEntity.HasValue && !string.IsNullOrEmpty(current.key))
            {
                visualEntity.Value.AddComponent(new EffectVisualLink
                {
                    owner = source.caster,
                    key = current.key
                });
            }

            CompleteCurrentVisual(effectEntity, visual);
        });
    }

    /// <summary>
    /// 取得当前应执行的视觉步骤；兼容单步 payload 与多步队列两种形态。
    /// </summary>
    private static EffectVisualStepSpec GetCurrentVisualStep(EffectVisualData visual)
    {
        if (visual.steps != null && visual.nextIndex >= 0 && visual.nextIndex < visual.steps.Count)
            return visual.steps[visual.nextIndex];

        return new EffectVisualStepSpec
        {
            kind = visual.kind,
            model = visual.model,
            key = visual.key,
            attachPoint = visual.attachPoint,
            duration = visual.durationValue,
            fallbackDuration = visual.duration,
            hasPoint = visual.hasPoint,
            x = visual.x,
            y = visual.y,
            z = visual.z
        };
    }

    /// <summary>
    /// 标记当前视觉步骤完成；队列未结束时推进索引，结束时完成视觉 payload。
    /// </summary>
    private static void CompleteCurrentVisual(Entity effectEntity, EffectVisualData visual)
    {
        if (visual.steps == null)
        {
            EffectSettlementHelper.MarkSettlementDone(effectEntity, typeof(EffectVisualData));
            return;
        }

        visual.nextIndex++;
        if (visual.nextIndex >= visual.steps.Count)
            EffectSettlementHelper.MarkSettlementDone(effectEntity, typeof(EffectVisualData));
        else
            effectEntity.AddComponent(visual);
    }

    /// <summary>
    /// 判断视觉步骤是否必须等待区域/线形搜索生成具体目标后才能执行。
    /// </summary>
    private static bool NeedsResolvedTarget(EffectVisualKind kind)
    {
        return kind is EffectVisualKind.AttachTarget or EffectVisualKind.AttachEachTarget;
    }

    /// <summary>
    /// 根据视觉步骤创建 ECS 特效实体；不直接执行任何 War3 原生调用。
    /// </summary>
    private static Entity? CreateVisualEntity(EffectVisualStepSpec visual, EffectSource source,
        EffectTargetInfo target, float duration)
    {
        switch (visual.kind)
        {
            case EffectVisualKind.Point:
                var x = visual.hasPoint ? visual.x : target.targetX;
                var y = visual.hasPoint ? visual.y : target.targetY;
                var z = visual.hasPoint ? visual.z : 0f;
                return EffectHelper.CreatePosition(visual.model, x, y, z, duration);
            case EffectVisualKind.TargetPoint:
                return EffectHelper.CreatePosition(visual.model, target.targetX, target.targetY, 0f, duration);
            case EffectVisualKind.AttachCaster:
            case EffectVisualKind.AttachOwner:
                return source.caster.IsNull
                    ? null
                    : EffectHelper.CreateAttached(source.caster, visual.model, visual.attachPoint, duration);
            case EffectVisualKind.AttachTarget:
            case EffectVisualKind.AttachEachTarget:
                return target.targetUnit.IsNull
                    ? null
                    : EffectHelper.CreateAttached(target.targetUnit, visual.model, visual.attachPoint, duration);
            default:
                return null;
        }
    }

    /// <summary>
    /// 按 owner 与 key 请求销毁长期视觉特效，避免同名 key 跨单位误删。
    /// </summary>
    private static void RemoveLinkedEffects(Entity owner, string? key)
    {
        if (owner.IsNull || string.IsNullOrEmpty(key))
            return;

        var query = Game.Store.Query<EffectVisualLink>();
        query.ForEachEntity((ref EffectVisualLink link, Entity visualEntity) =>
        {
            if (link.owner == owner && link.key == key)
                EffectHelper.Destroy(visualEntity, hideFirst: true);
        });
    }
}

/// <summary>
/// 区域搜索系统。
/// 只负责选出目标并生成子 effect；具体伤害、治疗和 Buff 仍由结算系统处理。
/// </summary>
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

            var sourceAbility = source.ability;
            var radius = EffectFormulaRegistry.Resolve(
                source.caster,
                source.ability,
                target.targetUnit,
                effectEntity,
                area.radiusValue,
                () => AbilityHelper.GetRadius(sourceAbility));
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

/// <summary>
/// 线形搜索系统。
/// 复用 GroupHelper.FindInLine 生成子 effect，并把火焰等接触语义转为显式区域反应请求。
/// </summary>
[SystemRegister(SystemKind.Interval, 111)]
public class LineSearchSystem : QuerySystem<LineSearchData, EffectSource, EffectTargetInfo>
{
    public LineSearchSystem()
    {
        Filter.AnyTags(Tags.Get<EffectPending>());
    }

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref LineSearchData line, ref EffectSource source,
            ref EffectTargetInfo target, Entity effectEntity) =>
        {
            if (ProjectileFlowHelper.HasPendingProjectile(effectEntity))
                return;

            if (!source.caster.TryGetComponent<Position>(out var casterPos))
            {
                effectEntity.AddTag<EffectCompleted>();
                return;
            }

            var legacyRange = line.range;
            var sourceAbility = source.ability;
            var range = EffectFormulaRegistry.Resolve(
                source.caster,
                source.ability,
                target.targetUnit,
                effectEntity,
                line.rangeValue,
                () => legacyRange > 0f ? legacyRange : AbilityHelper.GetCastRange(sourceAbility));
            var width = EffectFormulaRegistry.Resolve(
                source.caster,
                source.ability,
                target.targetUnit,
                effectEntity,
                line.widthValue,
                line.width);

            var end = ResolveLineEnd(casterPos.x, casterPos.y, target.targetX, target.targetY, range);
            var targets = GroupHelper.FindInLine(
                source.caster,
                casterPos.x,
                casterPos.y,
                end.x,
                end.y,
                width,
                line.filter,
                line.customFilterId,
                line.maxTargets);

            foreach (var targetUnit in targets)
            {
                AbilityEffectHelper.CreateChildEffect(effectEntity, targetUnit);
            }

            if (line.reactionTag != GroundAreaTag.None)
                GroundAreaQueryHelper.EmitLineContactRequests(source.caster, effectEntity, casterPos.x, casterPos.y, end.x, end.y, width, line.reactionTag);

            effectEntity.AddTag<EffectCompleted>();
        });
    }

    private static (float x, float y) ResolveLineEnd(float startX, float startY, float targetX, float targetY, float range)
    {
        var dx = targetX - startX;
        var dy = targetY - startY;
        var dist = MathF.Sqrt(dx * dx + dy * dy);
        if (dist <= float.Epsilon)
            return (startX + range, startY);

        return (startX + dx / dist * range, startY + dy / dist * range);
    }
}

/// <summary>
/// 地面区域创建系统。
/// 把一次性效果 payload 转换成独立 ECS 区域实体，区域位置来自目标点。
/// </summary>
[SystemRegister(SystemKind.Interval, 112)]
public class GroundAreaCreateSystem : QuerySystem<GroundAreaCreateData, EffectSource, EffectTargetInfo>
{
    private readonly List<(GroundAreaCreateData create, EffectSource source, EffectTargetInfo target, Entity effect)> _pending = new();

    public GroundAreaCreateSystem()
    {
        Filter.AnyTags(Tags.Get<EffectPending>());
    }

    protected override void OnUpdate()
    {
        _pending.Clear();
        Query.ForEachEntity((ref GroundAreaCreateData create, ref EffectSource source,
            ref EffectTargetInfo target, Entity effectEntity) =>
        {
            if (!EffectSettlementHelper.CanSettle(effectEntity))
                return;
            _pending.Add((create, source, target, effectEntity));
        });

        foreach (var pending in _pending)
            CreateArea(pending.create, pending.source, pending.target, pending.effect);
    }

    /// <summary>
    /// 在查询结束后创建区域并传播来源，避免查询期间执行结构变更。
    /// </summary>
    private static void CreateArea(GroundAreaCreateData create, EffectSource source,
        EffectTargetInfo target, Entity effectEntity)
    {
        if (effectEntity.IsNull)
            return;

        var sourceAbility = source.ability;
        var legacyRadius = create.radius;
        var radius = EffectFormulaRegistry.Resolve(
            source.caster,
            source.ability,
            target.targetUnit,
            effectEntity,
            create.radiusValue,
            () => legacyRadius > 0f ? legacyRadius : AbilityHelper.GetRadius(sourceAbility));
        var duration = EffectFormulaRegistry.Resolve(
            source.caster,
            source.ability,
            target.targetUnit,
            effectEntity,
            create.durationValue,
            create.duration);

        var area = effectEntity.Store.CreateEntity(
            new GroundAreaData { tags = create.tags, radius = radius, radiusValue = create.radiusValue },
            new GroundAreaSource { caster = source.caster, ability = source.ability, sourceEffect = effectEntity },
            new GroundAreaLifetime { duration = duration, remaining = duration },
            new Position { x = target.targetX, y = target.targetY, z = 0f });

        if (effectEntity.TryGetComponent<ItemEffectOrigin>(out var itemOrigin))
            area.AddComponent(itemOrigin);
        if (create.buff.enabled)
            area.AddComponent(create.buff);
        if (create.periodicDamage.enabled)
            area.AddComponent(create.periodicDamage);
        if (create.reaction.enabled)
            area.AddComponent(create.reaction);

        EffectSettlementHelper.MarkSettlementDone(effectEntity, typeof(GroundAreaCreateData));
    }
}

/// <summary>
/// 伤害效果转请求系统。
/// 这里只产生 DamageRequest，不直接扣血；实际属性修改在 DamageResolveSystem。
/// </summary>
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

            var sourceAbility = source.ability;
            // delegate 是高级自定义覆盖；普通技能走 EffectValueSpec 的 formulaId/statId。
            var amount = damageData.damageFunc != null
                ? damageData.damageFunc(source.caster, source.ability, target.targetUnit, damageData)
                : EffectFormulaRegistry.Resolve(
                    source.caster,
                    source.ability,
                    target.targetUnit,
                    effectEntity,
                    damageData.value,
                    () => AbilityHelper.GetDamageAmount(sourceAbility));

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

/// <summary>
/// 治疗效果转请求系统。
/// 这里只产生 HealRequest，实际生命值修改在 HealResolveSystem。
/// </summary>
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

            var legacyHealAmount = heal.amount;
            var sourceAbility = source.ability;
            var amount = heal.healFunc != null
                ? heal.healFunc(source.caster, source.ability, target.targetUnit, heal)
                : EffectFormulaRegistry.Resolve(
                    source.caster,
                    source.ability,
                    target.targetUnit,
                    effectEntity,
                    heal.value,
                    () => legacyHealAmount > 0f
                        ? legacyHealAmount
                        : AbilityHelper.GetHealAmount(sourceAbility));

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

/// <summary>
/// Buff 效果转请求系统。
/// 这里只表达“要施加 Buff”的意图，实际 Buff 实体和属性修改由 resolver/helper 完成。
/// </summary>
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

            var fallbackDuration = buffData.duration;
            var duration = EffectFormulaRegistry.Resolve(
                source.caster,
                source.ability,
                target.targetUnit,
                effectEntity,
                buffData.durationValue,
                fallbackDuration);

            var fallbackValue = buffData.value;
            var value = EffectFormulaRegistry.Resolve(
                source.caster,
                source.ability,
                target.targetUnit,
                effectEntity,
                buffData.modifyValue,
                fallbackValue);

            Game.Store.CreateEntity(new BuffApplyRequest
            {
                source = source.caster,
                target = target.targetUnit,
                buffId = buffData.buffId,
                attrTypeId = buffData.attrTypeId,
                modifyType = buffData.modifyType,
                value = value,
                duration = duration,
                refreshBehavior = buffData.refreshBehavior
            });

            EffectSettlementHelper.MarkSettlementDone(effectEntity, typeof(ApplyBuffData));
        });
    }
}

/// <summary>
/// 伤害结算系统。
/// 这里是扣减生命值、发出 DamageEvent、触发死亡语义的统一结算点。
/// </summary>
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

/// <summary>
/// 治疗结算系统。
/// 统一修改 Health，并发出 HealEvent 供 UI、日志或后续系统监听。
/// </summary>
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

/// <summary>
/// Buff 应用结算系统。
/// 消费 BuffApplyRequest，并通过 BuffHelper 创建或刷新 Buff。
/// </summary>
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

/// <summary>
/// 地面区域生命周期系统。
/// 到期时删除区域实体，并清理由该区域创建的 Buff。
/// </summary>
[SystemRegister(SystemKind.Interval, 128)]
public class GroundAreaLifetimeSystem : QuerySystem<GroundAreaLifetime>
{
    protected override void OnUpdate()
    {
        var expired = new List<Entity>();

        Query.ForEachEntity((ref GroundAreaLifetime lifetime, Entity areaEntity) =>
        {
            lifetime.remaining -= Tick.deltaTime;
            if (lifetime.remaining <= 0f)
                expired.Add(areaEntity);
        });

        foreach (var areaEntity in expired)
        {
            GroundAreaQueryHelper.DeleteAreaBuffs(areaEntity);
            areaEntity.DeleteEntity();
        }
    }
}

/// <summary>
/// 地面区域 Buff 系统。
/// 根据区域范围应用永久 Buff，单位离开或区域消失时移除区域拥有的 Buff。
/// </summary>
[SystemRegister(SystemKind.Interval, 128)]
public class GroundAreaBuffSystem : QuerySystem<GroundAreaData, GroundAreaSource, GroundAreaBuffData, Position>
{
    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref GroundAreaData area, ref GroundAreaSource source,
            ref GroundAreaBuffData buffData, ref Position position, Entity areaEntity) =>
        {
            if (!buffData.enabled)
                return;

            var affected = GroundAreaQueryHelper.GetLinkedUnits(areaEntity);
            var inRange = new HashSet<int>();
            var targets = GroupHelper.FindInCircle(source.caster, position.x, position.y, area.radius, TargetFilter.EnemyAlive);
            foreach (var target in targets)
            {
                inRange.Add(target.Id);
                if (affected.Contains(target.Id))
                    continue;

                var value = EffectFormulaRegistry.Resolve(
                    source.caster,
                    source.ability,
                    target,
                    areaEntity,
                    buffData.value,
                    buffData.fallbackValue);
                var buff = BuffHelper.AddPermanentBuff(
                    Game.Store,
                    target,
                    areaEntity,
                    buffData.buffId,
                    buffData.attrTypeId,
                    buffData.modifyType,
                    value);
                if (!buff.IsNull)
                    buff.AddComponent(new GroundAreaBuffLink(areaEntity));
            }

            GroundAreaQueryHelper.DeleteAreaBuffsNotIn(areaEntity, inRange);
        });
    }
}

/// <summary>
/// 地面区域周期伤害系统。
/// 按 tick 对范围内目标发 DamageRequest，保持真实扣血在 DamageResolveSystem。
/// </summary>
[SystemRegister(SystemKind.Interval, 129)]
public class GroundAreaPeriodicDamageSystem : QuerySystem<GroundAreaData, GroundAreaSource, GroundAreaPeriodicDamageData, Position>
{
    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref GroundAreaData area, ref GroundAreaSource source,
            ref GroundAreaPeriodicDamageData damage, ref Position position, Entity areaEntity) =>
        {
            if (!damage.enabled)
                return;

            damage.timeSinceTick += Tick.deltaTime;
            var interval = damage.tickInterval > 0f ? damage.tickInterval : 1f;
            if (damage.timeSinceTick < interval)
                return;

            damage.timeSinceTick = 0f;
            var targets = GroupHelper.FindInCircle(
                source.caster,
                position.x,
                position.y,
                area.radius,
                damage.filter,
                damage.customFilterId);

            foreach (var target in targets)
            {
                var amount = EffectFormulaRegistry.Resolve(
                    source.caster,
                    source.ability,
                    target,
                    areaEntity,
                    damage.damageValue,
                    damage.fallbackDamage);
                Game.Store.CreateEntity(new DamageRequest
                {
                    source = source.caster,
                    target = target,
                    damage = new DamageBase
                    {
                        damage = amount,
                        damageType = damage.damageType,
                        damageSrc = damage.damageSrc,
                        source = source.caster,
                        target = target
                    }
                });
            }
        });
    }
}

/// <summary>
/// 地面区域反应系统。
/// 消费显式反应请求，将油污等区域替换成燃烧地面。
/// </summary>
[SystemRegister(SystemKind.Interval, 129)]
public class GroundAreaReactionSystem : QuerySystem<GroundAreaReactionRequest>
{
    private readonly List<(Entity entity, GroundAreaReactionRequest request)> _pending = new();

    protected override void OnUpdate()
    {
        _pending.Clear();
        Query.ForEachEntity((ref GroundAreaReactionRequest request, Entity requestEntity) =>
        {
            _pending.Add((requestEntity, request));
        });

        foreach (var pending in _pending)
        {
            try
            {
                Process(pending.entity, pending.request);
            }
            finally
            {
                if (!pending.entity.IsNull)
                    pending.entity.DeleteEntity();
            }
        }
    }

    /// <summary>
    /// 在请求查询结束后替换区域并保留物品来源。
    /// </summary>
    private static void Process(Entity requestEntity, GroundAreaReactionRequest request)
    {
        var areaEntity = request.groundArea;
        if (areaEntity.IsNull
            || !ReferenceEquals(requestEntity.Store, areaEntity.Store)
            || !areaEntity.TryGetComponent<GroundAreaReactionData>(out var reaction) ||
            !areaEntity.TryGetComponent<GroundAreaData>(out var area) ||
            !areaEntity.TryGetComponent<GroundAreaSource>(out var source) ||
            !areaEntity.TryGetComponent<Position>(out var position))
        {
            return;
        }

        if (!reaction.enabled || (reaction.triggerTag & request.incomingTag) == GroundAreaTag.None)
            return;

        var duration = EffectFormulaRegistry.Resolve(
            source.caster,
            source.ability,
            default,
            areaEntity,
            reaction.resultDuration,
            reaction.fallbackDuration);

        var hasItemOrigin = areaEntity.TryGetComponent<ItemEffectOrigin>(out var itemOrigin);
        var store = areaEntity.Store;

        GroundAreaQueryHelper.DeleteAreaBuffs(areaEntity);
        areaEntity.DeleteEntity();

        var burning = store.CreateEntity(
            new GroundAreaData { tags = reaction.resultTags, radius = area.radius, radiusValue = area.radiusValue },
            source,
            new GroundAreaLifetime { duration = duration, remaining = duration },
            position);
        if (hasItemOrigin)
            burning.AddComponent(itemOrigin);
        if (reaction.resultPeriodicDamage.enabled)
            burning.AddComponent(reaction.resultPeriodicDamage);
    }
}

/// <summary>
/// 技能效果实体清理系统。
/// 只清理完成或过期的运行时 effect entity，不处理业务结算。
/// </summary>
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
    /// <summary>
    /// 判断当前 effect 是否可以结算。
    /// 弹道未到达、区域搜索未展开时，伤害/治疗/Buff 必须等待。
    /// </summary>
    public static bool CanSettle(Entity effectEntity)
    {
        if (ProjectileFlowHelper.HasPendingProjectile(effectEntity))
            return false;

        return !effectEntity.HasComponent<AreaSearchData>();
    }

    /// <summary>
    /// 移除已完成的结算 payload；当所有 payload 都完成后标记 effect 完成。
    /// </summary>
    public static void MarkSettlementDone(Entity effectEntity, Type settlementType)
    {
        if (settlementType == typeof(DamageEffectData))
            effectEntity.RemoveComponent<DamageEffectData>();
        else if (settlementType == typeof(HealEffectData))
            effectEntity.RemoveComponent<HealEffectData>();
        else if (settlementType == typeof(ApplyBuffData))
            effectEntity.RemoveComponent<ApplyBuffData>();
        else if (settlementType == typeof(GroundAreaCreateData))
            effectEntity.RemoveComponent<GroundAreaCreateData>();
        else if (settlementType == typeof(EffectVisualData))
            effectEntity.RemoveComponent<EffectVisualData>();

        if (!HasSettlementPayload(effectEntity))
        {
            effectEntity.AddTag<EffectCompleted>();
        }
    }

    public static bool HasSettlementPayload(Entity effectEntity)
    {
        return effectEntity.HasComponent<DamageEffectData>() ||
               effectEntity.HasComponent<HealEffectData>() ||
               effectEntity.HasComponent<ApplyBuffData>() ||
               effectEntity.HasComponent<GroundAreaCreateData>() ||
               effectEntity.HasComponent<EffectVisualData>() ||
               effectEntity.HasComponent<AreaSearchData>() ||
               effectEntity.HasComponent<LineSearchData>();
    }
}

internal static class GroundAreaQueryHelper
{
    /// <summary>按线段接触检测地面区域，并为命中的区域发反应请求。</summary>
    public static void EmitLineContactRequests(Entity source, Entity effectEntity, float startX, float startY,
        float endX, float endY, float width, GroundAreaTag incomingTag)
    {
        var query = Game.Store.Query<GroundAreaData, Position>();
        query.ForEachEntity((ref GroundAreaData area, ref Position position, Entity areaEntity) =>
        {
            if (!LineIntersectsCircle(startX, startY, endX, endY, position.x, position.y, area.radius + width * 0.5f))
                return;

            Game.Store.CreateEntity(new GroundAreaReactionRequest
            {
                source = source,
                groundArea = areaEntity,
                incomingTag = incomingTag
            });
        });
    }

    public static HashSet<int> GetLinkedUnits(Entity areaEntity)
    {
        var units = new HashSet<int>();
        foreach (var link in areaEntity.GetIncomingLinks<GroundAreaBuffLink>())
        {
            var buff = link.Entity;
            if (!buff.TryGetComponent<ModifyTarget>(out var target) || target.target.IsNull)
                continue;

            if (!target.target.TryGetComponent<AttrOwner>(out var owner) || owner.owner.IsNull)
                continue;

            units.Add(owner.owner.Id);
        }

        return units;
    }

    public static void DeleteAreaBuffsNotIn(Entity areaEntity, HashSet<int> activeUnitIds)
    {
        foreach (var buff in CollectAreaBuffs(areaEntity))
        {
            if (!TryGetBuffOwner(buff, out var owner) || !activeUnitIds.Contains(owner.Id))
                DeleteBuff(buff);
        }
    }

    public static void DeleteAreaBuffs(Entity areaEntity)
    {
        foreach (var buff in CollectAreaBuffs(areaEntity))
            DeleteBuff(buff);
    }

    private static List<Entity> CollectAreaBuffs(Entity areaEntity)
    {
        var buffs = new List<Entity>();
        foreach (var link in areaEntity.GetIncomingLinks<GroundAreaBuffLink>())
            buffs.Add(link.Entity);
        return buffs;
    }

    private static bool TryGetBuffOwner(Entity buff, out Entity owner)
    {
        owner = default;
        if (!buff.TryGetComponent<ModifyTarget>(out var target) || target.target.IsNull)
            return false;

        if (!target.target.TryGetComponent<AttrOwner>(out var attrOwner))
            return false;

        owner = attrOwner.owner;
        return !owner.IsNull;
    }

    private static void DeleteBuff(Entity buff)
    {
        if (buff.TryGetComponent<ModifyTarget>(out var target) && !target.target.IsNull)
            target.target.AddTag<AttrDirty>();
        buff.DeleteEntity();
    }

    private static bool LineIntersectsCircle(float startX, float startY, float endX, float endY,
        float centerX, float centerY, float radius)
    {
        var dx = endX - startX;
        var dy = endY - startY;
        var lenSq = dx * dx + dy * dy;
        if (lenSq <= float.Epsilon)
            return DistanceSq(startX, startY, centerX, centerY) <= radius * radius;

        var t = ((centerX - startX) * dx + (centerY - startY) * dy) / lenSq;
        t = Math.Clamp(t, 0f, 1f);
        var nearestX = startX + dx * t;
        var nearestY = startY + dy * t;
        return DistanceSq(nearestX, nearestY, centerX, centerY) <= radius * radius;
    }

    private static float DistanceSq(float ax, float ay, float bx, float by)
    {
        var dx = ax - bx;
        var dy = ay - by;
        return dx * dx + dy * dy;
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

    /// <summary>
    /// 将弹道系统收集的到达/过期实体批量打标，避免查询遍历中直接执行复杂副作用。
    /// </summary>
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

    /// <summary>
    /// 销毁弹道视觉实体；视觉效果仍走 EffectHelper / Native 系统分层。
    /// </summary>
    public static void DestroyProjectileVisual(Entity effectEntity)
    {
        if (effectEntity.TryGetComponent<ProjectileData>(out var projectile) && !projectile.effectEntity.IsNull)
            EffectHelper.Destroy(projectile.effectEntity, hideFirst: true);
    }
}

internal static class ProjectileHookBridge
{
    /// <summary>
    /// 兼容新旧 projectile hook：模板基类、旧接口和 V2 hook 会按顺序被调用。
    /// </summary>
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

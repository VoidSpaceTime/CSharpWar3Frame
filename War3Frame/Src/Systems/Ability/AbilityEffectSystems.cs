using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components.AbilityEffectExtend;
using War3Frame.Helpers;
using War3Frame.Systems;
using War3Frame.TemplateInit;

namespace War3Frame;

// ============================================================================
// 技能效果处理系统
// 按照处理顺序排列：弹道推进/生命周期 → 范围搜索 → 伤害 → 治疗 → Buff 施加 → 清理
// ============================================================================

/// <summary>
/// 范围搜索系统 - 处理 AOE 效果
/// 在指定区域内搜索目标，为每个目标创建子效果 Entity。
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
        var toDelete = new List<Entity>();

        Query.ForEachEntity((ref AreaSearchData area, ref EffectSource source,
            ref EffectTargetInfo target, Entity effectEntity) =>
        {
            if (ProjectileFlowHelper.HasPendingProjectile(effectEntity))
                return;

            var radius = AbilityHelper.GetRadius(source.ability);

            var targets = FindTargetsInArea(
                source.caster, area.centerX, area.centerY,
                radius, area.filter, area.customFilterId, area.maxTargets);

            foreach (var targetUnit in targets)
            {
                AbilityEffectHelper.CreateChildEffect(effectEntity, targetUnit);
            }

            toDelete.Add(effectEntity);
        });

        foreach (var entity in toDelete)
        {
            entity.DeleteEntity();
        }
    }

    private List<Entity> FindTargetsInArea(Entity caster, float x, float y,
        float radius, TargetFilter filter, string? customFilterId, int maxTargets)
    {
        var results = new List<Entity>();
        float radiusSq = radius * radius;

        // TODO: 替换为你的空间查询或单位遍历逻辑
        // 示例：遍历所有带 Position 的单位
        // foreach (var (pos, unit) in allUnits)
        // {
        //     float dx = pos.x - x;
        //     float dy = pos.y - y;
        //     if (dx * dx + dy * dy > radiusSq) continue;
        //
        //     if (!TargetFilterRegistry.PassFilter(filter, customFilterId, caster, unit))
        //         continue;
        //
        //     results.Add(unit);
        //     if (maxTargets > 0 && results.Count >= maxTargets) break;
        // }

        return results;
    }
}

/// <summary>
/// 追踪/指向型弹道系统。
/// </summary>
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

        Query.ForEachEntity((ref ProjectileData projectile, ref EffectSource source, ref EffectTargetInfo target,
            ref Position pos, ref ProjectileRuntimeState runtimeState, Entity effectEntity) =>
        {
            if (!ProjectileFlowHelper.ShouldProcess(runtimeState))
                return;

            if (runtimeState.phase == ProjectileLifecyclePhase.PendingStart)
            {
                ProjectileHookBridge.DispatchStartHooks(effectEntity, ref projectile, ref source, ref target, ref pos,
                    ref runtimeState);
                runtimeState.phase = ProjectileLifecyclePhase.InFlight;
            }

            var decision = ProjectileHookBridge.DispatchTravelHooks(effectEntity, ref projectile, ref source,
                ref target, ref pos, ref runtimeState);
            if (decision == ProjectileTravelDecision.RequestExpire)
            {
                runtimeState.phase = ProjectileLifecyclePhase.ExpireRequested;
                expireRequests.Add(effectEntity);
                return;
            }

            float tx = target.targetX;
            float ty = target.targetY;
            if (!target.targetUnit.IsNull &&
                target.targetUnit.TryGetComponent<Position>(out var targetPos))
            {
                tx = targetPos.x;
                ty = targetPos.y;
                target.targetX = tx;
                target.targetY = ty;
            }

            float dx = tx - pos.x;
            float dy = ty - pos.y;
            float dist = MathF.Sqrt(dx * dx + dy * dy);

            if (dist <= projectile.arrivalThreshold)
            {
                if (decision != ProjectileTravelDecision.SuppressArrivalThisTick)
                {
                    runtimeState.phase = ProjectileLifecyclePhase.ArriveRequested;
                    arriveRequests.Add(effectEntity);
                }

                return;
            }

            float move = projectile.speed * Tick.deltaTime;
            pos.x += dx / dist * move;
            pos.y += dy / dist * move;

            if (!projectile.effectEntity.IsNull)
            {
                EffectHelper.SetPosition(projectile.effectEntity, pos.x, pos.y, pos.z);
            }
        });

        ProjectileFlowHelper.ApplyRequests(arriveRequests, expireRequests);
    }
}

/// <summary>
/// 方向型线性弹道系统。
/// </summary>
[SystemRegister(SystemKind.Interval, 101)]
public class ProjectileLinearSystem : QuerySystem<ProjectileLinearData, EffectSource, EffectTargetInfo, Position, ProjectileRuntimeState>
{
    public ProjectileLinearSystem()
    {
        Filter.AnyTags(Tags.Get<EffectPending>());
    }

    protected override void OnUpdate()
    {
        var arriveRequests = new List<Entity>();
        var expireRequests = new List<Entity>();

        Query.ForEachEntity((ref ProjectileLinearData projectile, ref EffectSource source, ref EffectTargetInfo target,
            ref Position pos, ref ProjectileRuntimeState runtimeState, Entity effectEntity) =>
        {
            if (!ProjectileFlowHelper.ShouldProcess(runtimeState))
                return;

            if (runtimeState.phase == ProjectileLifecyclePhase.PendingStart)
            {
                ProjectileHookBridge.DispatchStartHooks(effectEntity, ref projectile, ref source, ref target, ref pos,
                    ref runtimeState);
                runtimeState.phase = ProjectileLifecyclePhase.InFlight;
            }

            var decision = ProjectileHookBridge.DispatchTravelHooks(effectEntity, ref projectile, ref source,
                ref target, ref pos, ref runtimeState);
            if (decision == ProjectileTravelDecision.RequestExpire)
            {
                runtimeState.phase = ProjectileLifecyclePhase.ExpireRequested;
                expireRequests.Add(effectEntity);
                return;
            }

            float remainingDistance = MathF.Max(0f, projectile.maxDistance - projectile.traveled);
            if (remainingDistance <= 0f)
            {
                if (decision != ProjectileTravelDecision.SuppressArrivalThisTick)
                {
                    runtimeState.phase = ProjectileLifecyclePhase.ArriveRequested;
                    arriveRequests.Add(effectEntity);
                }

                return;
            }

            float move = MathF.Min(projectile.speed * Tick.deltaTime, remainingDistance);
            pos.x += projectile.dirX * move;
            pos.y += projectile.dirY * move;
            projectile.traveled += move;

            if (!projectile.effectEntity.IsNull)
            {
                EffectHelper.SetPosition(projectile.effectEntity, pos.x, pos.y, pos.z);
            }

            if (projectile.traveled >= projectile.maxDistance &&
                decision != ProjectileTravelDecision.SuppressArrivalThisTick)
            {
                runtimeState.phase = ProjectileLifecyclePhase.ArriveRequested;
                arriveRequests.Add(effectEntity);
            }
        });

        ProjectileFlowHelper.ApplyRequests(arriveRequests, expireRequests);
    }
}

/// <summary>
/// 弹道生命周期应用系统。
/// 统一消费到达/过期请求，避免在 movement query 内直接做结构变更。
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
            {
                toArrive.Add(effectEntity);
            }

            if (effectEntity.Tags.Has<ProjectileExpireRequest>())
            {
                toExpire.Add(effectEntity);
            }
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
            {
                effectEntity.AddTag<ProjectileArrived>();
            }

            ProjectileFlowHelper.DestroyProjectileVisual(effectEntity);
            ProjectileHookBridge.DispatchArriveHooks(effectEntity, ref source, ref target, ref pos, ref runtimeState);
            effectEntity.AddComponent(runtimeState);
        }

        foreach (var effectEntity in toExpire)
        {
            if (!effectEntity.TryGetComponent<ProjectileRuntimeState>(out var runtimeState))
            {
                continue;
            }

            runtimeState.phase = ProjectileLifecyclePhase.Expired;
            effectEntity.AddComponent(runtimeState);
            effectEntity.RemoveTag<ProjectileExpireRequest>();
            ProjectileFlowHelper.DestroyProjectileVisual(effectEntity);
            effectEntity.DeleteEntity();
        }
    }
}

/// <summary>
/// 伤害效果处理系统 - 对目标造成伤害
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
        var toDelete = new List<Entity>();

        Query.ForEachEntity((ref DamageEffectData dmg, ref EffectSource source,
            ref EffectTargetInfo target, Entity effectEntity) =>
        {
            if (ProjectileFlowHelper.HasPendingProjectile(effectEntity))
                return;

            if (effectEntity.HasComponent<AreaSearchData>())
                return;

            if (target.targetUnit.IsNull) return;

            float finalDamage = dmg.damageFunc(source.caster, source.ability, target.targetUnit, dmg);
            float remaining = AttributeHelper.ModifyCurrent(
                target.targetUnit, AttributeHelper.Health, -finalDamage);

            if (remaining <= 0)
            {
                // TODO: 调用 UnitHelper.Kill(target.targetUnit, source.caster);
            }

            if (!effectEntity.HasComponent<HealEffectData>() &&
                !effectEntity.HasComponent<ApplyBuffData>())
            {
                toDelete.Add(effectEntity);
            }
        });

        foreach (var entity in toDelete)
        {
            entity.DeleteEntity();
        }
    }
}

/// <summary>
/// 治疗效果处理系统 - 回复目标生命值
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
        var toDelete = new List<Entity>();

        Query.ForEachEntity((ref HealEffectData heal, ref EffectSource source,
            ref EffectTargetInfo target, Entity effectEntity) =>
        {
            if (ProjectileFlowHelper.HasPendingProjectile(effectEntity))
                return;

            if (effectEntity.HasComponent<AreaSearchData>())
                return;

            if (target.targetUnit.IsNull) return;

            float finalHeal = heal.healFunc(source.caster, source.ability, target.targetUnit, heal);

            AttributeHelper.ModifyCurrent(
                target.targetUnit, AttributeHelper.Health, finalHeal);

            if (!effectEntity.HasComponent<DamageEffectData>() &&
                !effectEntity.HasComponent<ApplyBuffData>())
            {
                toDelete.Add(effectEntity);
            }
        });

        foreach (var entity in toDelete)
        {
            entity.DeleteEntity();
        }
    }
}

/// <summary>
/// Buff 施加效果系统 - 给目标添加 Buff
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
        var toDelete = new List<Entity>();

        Query.ForEachEntity((ref ApplyBuffData buffData, ref EffectSource source,
            ref EffectTargetInfo target, Entity effectEntity) =>
        {
            if (ProjectileFlowHelper.HasPendingProjectile(effectEntity))
                return;

            if (effectEntity.HasComponent<AreaSearchData>())
                return;

            if (target.targetUnit.IsNull) return;

            BuffHelper.AddTimedBuff(
                Game.Store,
                target.targetUnit,
                source.caster,
                buffData.buffId,
                buffData.attrTypeId,
                buffData.modifyType,
                buffData.value,
                buffData.duration,
                buffData.refreshBehavior
            );

            toDelete.Add(effectEntity);
        });

        foreach (var entity in toDelete)
        {
            entity.DeleteEntity();
        }
    }
}

/// <summary>
/// 效果清理系统 - 清理所有已处理完毕的效果 Entity。
/// </summary>
[SystemRegister(SystemKind.Interval, 130)]
public class EffectCleanupSystem : QuerySystem<EffectSource>
{
    public EffectCleanupSystem()
    {
        Filter.AnyTags(Tags.Get<EffectPending>());
    }

    protected override void OnUpdate()
    {
        var toDelete = new List<Entity>();

        Query.ForEachEntity((ref EffectSource source, Entity effectEntity) =>
        {
            if (ProjectileFlowHelper.HasPendingProjectile(effectEntity))
                return;

            if (effectEntity.HasComponent<AreaSearchData>())
                return;

            bool hasUnprocessed =
                effectEntity.HasComponent<DamageEffectData>() ||
                effectEntity.HasComponent<HealEffectData>() ||
                effectEntity.HasComponent<ApplyBuffData>();

            if (!hasUnprocessed)
            {
                toDelete.Add(effectEntity);
            }
        });

        foreach (var entity in toDelete)
        {
            entity.DeleteEntity();
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
        bool hasProjectile = effectEntity.HasComponent<ProjectileData>() ||
                             effectEntity.HasComponent<ProjectileLinearData>();
        return hasProjectile && !effectEntity.Tags.Has<ProjectileArrived>();
    }

    public static void ApplyRequests(List<Entity> arriveRequests, List<Entity> expireRequests)
    {
        foreach (var effectEntity in arriveRequests)
        {
            if (!effectEntity.Tags.Has<ProjectileArriveRequest>())
            {
                effectEntity.AddTag<ProjectileArriveRequest>();
            }
        }

        foreach (var effectEntity in expireRequests)
        {
            if (!effectEntity.Tags.Has<ProjectileExpireRequest>())
            {
                effectEntity.AddTag<ProjectileExpireRequest>();
            }
        }
    }

    public static void DestroyProjectileVisual(Entity effectEntity)
    {
        if (effectEntity.TryGetComponent<ProjectileData>(out var projectile) && !projectile.effectEntity.IsNull)
        {
            EffectHelper.Destroy(projectile.effectEntity, hideFirst: true);
        }

        if (effectEntity.TryGetComponent<ProjectileLinearData>(out var linear) && !linear.effectEntity.IsNull)
        {
            EffectHelper.Destroy(linear.effectEntity, hideFirst: true);
        }
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

        if (template is IProjectileOnStart legacyStart)
        {
            var legacyProjectile = CreateLegacyProjectile(source, target, position, projectile.speed);
            legacyStart.ProjectileOnStart(ref legacyProjectile, ref position, effectEntity);
            ApplyLegacyProjectile(ref legacyProjectile, ref target, ref projectile.speed);
        }

        if (template is IProjectileHooksV2 hooksV2)
        {
            hooksV2.OnStart(effectEntity, ref source, ref target, ref position, ref runtimeState);
        }
    }

    public static void DispatchStartHooks(Entity effectEntity, ref ProjectileLinearData projectile,
        ref EffectSource source, ref EffectTargetInfo target, ref Position position,
        ref ProjectileRuntimeState runtimeState)
    {
        if (!TryResolveTemplate(source.ability, out var template))
            return;

        if (template is IProjectileOnStart legacyStart)
        {
            var legacyProjectile = CreateLegacyProjectile(source, target, position, projectile.speed);
            legacyStart.ProjectileOnStart(ref legacyProjectile, ref position, effectEntity);
            ApplyLegacyProjectile(ref legacyProjectile, ref target, ref projectile.speed);
        }

        if (template is IProjectileHooksV2 hooksV2)
        {
            hooksV2.OnStart(effectEntity, ref source, ref target, ref position, ref runtimeState);
        }
    }

    public static ProjectileTravelDecision DispatchTravelHooks(Entity effectEntity, ref ProjectileData projectile,
        ref EffectSource source, ref EffectTargetInfo target, ref Position position,
        ref ProjectileRuntimeState runtimeState)
    {
        return DispatchTravelHooksCore(effectEntity, ref source, ref target, ref position, ref runtimeState,
            ref projectile.speed);
    }

    public static ProjectileTravelDecision DispatchTravelHooks(Entity effectEntity, ref ProjectileLinearData projectile,
        ref EffectSource source, ref EffectTargetInfo target, ref Position position,
        ref ProjectileRuntimeState runtimeState)
    {
        return DispatchTravelHooksCore(effectEntity, ref source, ref target, ref position, ref runtimeState,
            ref projectile.speed);
    }

    public static void DispatchArriveHooks(Entity effectEntity, ref EffectSource source,
        ref EffectTargetInfo target, ref Position position, ref ProjectileRuntimeState runtimeState)
    {
        if (!TryResolveTemplate(source.ability, out var template))
            return;

        float speed = 0f;
        if (effectEntity.TryGetComponent<ProjectileData>(out var projectile))
        {
            speed = projectile.speed;
        }
        else if (effectEntity.TryGetComponent<ProjectileLinearData>(out var linear))
        {
            speed = linear.speed;
        }

        if (template is IProjectileOnArrive legacyArrive)
        {
            var legacyProjectile = CreateLegacyProjectile(source, target, position, speed);
            legacyArrive.ProjectileOnArrive(ref legacyProjectile, ref position, effectEntity);
        }

        if (template is IProjectileHooksV2 hooksV2)
        {
            hooksV2.OnArrive(effectEntity, ref source, ref target, ref position, ref runtimeState);
        }
    }

    private static ProjectileTravelDecision DispatchTravelHooksCore(Entity effectEntity,
        ref EffectSource source, ref EffectTargetInfo target, ref Position position,
        ref ProjectileRuntimeState runtimeState, ref float speed)
    {
        if (!TryResolveTemplate(source.ability, out var template))
            return ProjectileTravelDecision.Continue;

        var decision = ProjectileTravelDecision.Continue;

        if (template is IProjectileOnTravel legacyTravel)
        {
            var legacyProjectile = CreateLegacyProjectile(source, target, position, speed);
            var allowArrive = legacyTravel.ProjectileOnTravel(ref legacyProjectile, ref position, effectEntity);
            ApplyLegacyProjectile(ref legacyProjectile, ref target, ref speed);
            if (!allowArrive)
            {
                decision = MergeDecision(decision, ProjectileTravelDecision.SuppressArrivalThisTick);
            }
        }

        if (template is IProjectileHooksV2 hooksV2)
        {
            decision = MergeDecision(decision,
                hooksV2.OnTravel(effectEntity, ref source, ref target, ref position, ref runtimeState));
        }

        return decision;
    }

    private static ProjectileTravelDecision MergeDecision(
        ProjectileTravelDecision current,
        ProjectileTravelDecision incoming)
    {
        if (current == ProjectileTravelDecision.RequestExpire || incoming == ProjectileTravelDecision.RequestExpire)
            return ProjectileTravelDecision.RequestExpire;

        if (current == ProjectileTravelDecision.SuppressArrivalThisTick ||
            incoming == ProjectileTravelDecision.SuppressArrivalThisTick)
            return ProjectileTravelDecision.SuppressArrivalThisTick;

        return ProjectileTravelDecision.Continue;
    }

    private static ProjectileBase CreateLegacyProjectile(
        EffectSource source,
        EffectTargetInfo target,
        Position position,
        float speed)
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

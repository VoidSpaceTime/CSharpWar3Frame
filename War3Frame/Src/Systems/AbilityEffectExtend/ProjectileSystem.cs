using System.Numerics;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components.AbilityEffectExtend;
using War3Frame.TemplateInit;

namespace War3Frame.Systems.AbilityEffectExtend;

public class ProjectileSystem : QuerySystem<ProjectileBase, Position, ProjectileRuntimeState>, ITimedSystem
{
    private const float ArrivalThreshold = 50f;

    public float Interval { get; } = 0.05f;

    public ProjectileSystem()
    {
        // Filter.AnyTags(Tags.Get<ProjectileOnTravel>());
    }

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref projectile, ref position, ref runtimeState, entity) =>
        {
            var step = projectile.speed / 1 / Interval;
            bool flag = true;

            if (projectile.SourceAbility.TryGetComponent(out AbilityBase abilityBase) &&
                AbilityTemplate.TryGet(abilityBase.Name, out var template))
            {
                if (entity.Tags.Has<ProjectileOnStart>() && template is IProjectileOnStart onStart)
                {
                    onStart.ProjectileOnStart(ref projectile, ref position, entity);
                    entity.RemoveTag<ProjectileOnStart>();
                }

                if (entity.Tags.Has<ProjectileOnTravel>() && template is IProjectileOnTravel onTravel)
                {
                    flag = onTravel.ProjectileOnTravel(ref projectile, ref position, entity);
                    entity.RemoveTag<ProjectileOnTravel>();
                }

                if (entity.Tags.Has<ProjectileOnArrive>() && template is IProjectileOnArrive onArrive)
                {
                    entity.RemoveTag<ProjectileOnArrive>();
                    onArrive.ProjectileOnArrive(ref projectile, ref position, entity);
                    entity.DeleteEntity();
                    return;
                }
            }

            runtimeState.elapsedTime += Interval;

            bool moved = UpdateTrajectory(ref projectile, ref position, ref runtimeState, step, out float dist);

            if (dist < ArrivalThreshold && flag)
            {
                entity.RemoveTag<ProjectileOnTravel>();
                entity.AddTag<ProjectileOnArrive>();
                return;
            }

            if (moved)
            {
                entity.AddTag<ProjectilePositionDirty>();
            }

            entity.AddTag<ProjectileOnTravel>();
        });
    }

    private bool UpdateTrajectory(
        ref ProjectileBase projectile,
        ref Position position,
        ref ProjectileRuntimeState runtimeState,
        float step,
        out float dist)
    {
        return projectile.trajectoryType switch
        {
            TrajectoryType.Linear => UpdateLinearTrajectory(ref projectile, ref position, step, out dist),
            TrajectoryType.Tracking => UpdateTrackingTrajectory(ref projectile, ref position, step, out dist),
            TrajectoryType.Bezier => UpdateBezierTrajectory(ref projectile, ref position, ref runtimeState, step, out dist),
            TrajectoryType.Parabolic => UpdateParabolicTrajectory(ref projectile, ref position, ref runtimeState, step, out dist),
            TrajectoryType.Sinusoidal => UpdateSinusoidalTrajectory(ref projectile, ref position, ref runtimeState, step, out dist),
            TrajectoryType.Spiral => UpdateSpiralTrajectory(ref projectile, ref position, ref runtimeState, step, out dist),
            _ => UpdateLinearTrajectory(ref projectile, ref position, step, out dist)
        };
    }

    private bool UpdateLinearTrajectory(
        ref ProjectileBase projectile,
        ref Position position,
        float step,
        out float dist)
    {
        var current = new Vector3(position.x, position.y, position.z);
        var target = new Vector3(projectile.targetX, projectile.targetY, projectile.height);
        var delta = target - current;
        dist = delta.Length();
        if (dist < ArrivalThreshold)
        {
            return false;
        }

        float dx = projectile.targetX - position.x;
        float dy = projectile.targetY - position.y;
        float planarDist = MathF.Sqrt(dx * dx + dy * dy);
        if (planarDist <= float.Epsilon)
        {
            return false;
        }

        float move = MathF.Min(step, planarDist);
        position.x += dx / planarDist * move;
        position.y += dy / planarDist * move;
        return true;
    }

    private bool UpdateTrackingTrajectory(
        ref ProjectileBase projectile,
        ref Position position,
        float step,
        out float dist)
    {
        if (projectile.TargetEntity is { } targetEntity &&
            targetEntity.TryGetComponent(out Position targetPos))
        {
            projectile.targetX = targetPos.x;
            projectile.targetY = targetPos.y;
        }

        return UpdateLinearTrajectory(ref projectile, ref position, step, out dist);
    }

    private bool UpdateBezierTrajectory(
        ref ProjectileBase projectile,
        ref Position position,
        ref ProjectileRuntimeState runtimeState,
        float step,
        out float dist)
    {
        var start = new Vector3(projectile.startX, projectile.startY, projectile.height);
        var end = new Vector3(projectile.targetX, projectile.targetY, projectile.height);

        if (runtimeState.elapsedTime <= float.Epsilon)
        {
            var mid = (start + end) * 0.5f;
            var perpendicular = new Vector3(-(end.Y - start.Y), end.X - start.X, 0);
            perpendicular = Vector3.Normalize(perpendicular);
            float arcHeight = Vector3.Distance(start, end) * 0.3f;
            runtimeState.controlPoint1 = start + (mid - start) * 0.5f + perpendicular * arcHeight * 0.5f;
            runtimeState.controlPoint2 = mid + (end - mid) * 0.5f + perpendicular * arcHeight * 0.5f;
        }

        float totalDist = Vector3.Distance(start, end);
        float progressIncrement = step / totalDist;
        runtimeState.normalizedProgress = MathF.Min(1.0f, runtimeState.normalizedProgress + progressIncrement);

        float t = runtimeState.normalizedProgress;
        float oneMinusT = 1 - t;
        Vector3 newPos = oneMinusT * oneMinusT * oneMinusT * start +
                        3 * oneMinusT * oneMinusT * t * runtimeState.controlPoint1 +
                        3 * oneMinusT * t * t * runtimeState.controlPoint2 +
                        t * t * t * end;

        position.x = newPos.X;
        position.y = newPos.Y;
        position.z = newPos.Z;

        dist = Vector3.Distance(newPos, end);
        return true;
    }

    private bool UpdateParabolicTrajectory(
        ref ProjectileBase projectile,
        ref Position position,
        ref ProjectileRuntimeState runtimeState,
        float step,
        out float dist)
    {
        var start = new Vector3(projectile.startX, projectile.startY, projectile.height);
        var end = new Vector3(projectile.targetX, projectile.targetY, projectile.height);

        float totalDist = Vector3.Distance(start, end);
        float progressIncrement = step / totalDist;
        runtimeState.normalizedProgress = MathF.Min(1.0f, runtimeState.normalizedProgress + progressIncrement);

        float t = runtimeState.normalizedProgress;
        Vector3 linearPos = Vector3.Lerp(start, end, t);

        float arcHeight = totalDist * 0.4f;
        float heightOffset = 4 * arcHeight * t * (1 - t);

        position.x = linearPos.X;
        position.y = linearPos.Y;
        position.z = linearPos.Z + heightOffset;

        dist = Vector3.Distance(linearPos, end);
        return true;
    }

    private bool UpdateSinusoidalTrajectory(
        ref ProjectileBase projectile,
        ref Position position,
        ref ProjectileRuntimeState runtimeState,
        float step,
        out float dist)
    {
        var start = new Vector3(projectile.startX, projectile.startY, projectile.height);
        var end = new Vector3(projectile.targetX, projectile.targetY, projectile.height);

        float totalDist = Vector3.Distance(start, end);
        float progressIncrement = step / totalDist;
        runtimeState.normalizedProgress = MathF.Min(1.0f, runtimeState.normalizedProgress + progressIncrement);

        float t = runtimeState.normalizedProgress;
        Vector3 linearPos = Vector3.Lerp(start, end, t);

        var direction = Vector3.Normalize(end - start);
        var perpendicular = new Vector3(-direction.Y, direction.X, 0);

        float amplitude = totalDist * 0.15f;
        float frequency = 3.0f;
        float sineOffset = MathF.Sin(t * MathF.PI * frequency) * amplitude;

        Vector3 finalPos = linearPos + perpendicular * sineOffset;
        position.x = finalPos.X;
        position.y = finalPos.Y;
        position.z = finalPos.Z;

        dist = Vector3.Distance(linearPos, end);
        return true;
    }

    private bool UpdateSpiralTrajectory(
        ref ProjectileBase projectile,
        ref Position position,
        ref ProjectileRuntimeState runtimeState,
        float step,
        out float dist)
    {
        var start = new Vector3(projectile.startX, projectile.startY, projectile.height);
        var end = new Vector3(projectile.targetX, projectile.targetY, projectile.height);

        float totalDist = Vector3.Distance(start, end);
        float progressIncrement = step / totalDist;
        runtimeState.normalizedProgress = MathF.Min(1.0f, runtimeState.normalizedProgress + progressIncrement);

        float t = runtimeState.normalizedProgress;
        Vector3 linearPos = Vector3.Lerp(start, end, t);

        var direction = Vector3.Normalize(end - start);
        var perpendicular = new Vector3(-direction.Y, direction.X, 0);

        float radius = totalDist * 0.2f * (1 - t);
        float angle = t * MathF.PI * 6;
        float offsetX = MathF.Cos(angle) * radius;
        float offsetY = MathF.Sin(angle) * radius;

        Vector3 finalPos = linearPos + perpendicular * offsetX + new Vector3(0, 0, offsetY);
        position.x = finalPos.X;
        position.y = finalPos.Y;
        position.z = finalPos.Z;

        dist = Vector3.Distance(linearPos, end);
        return true;
    }
}

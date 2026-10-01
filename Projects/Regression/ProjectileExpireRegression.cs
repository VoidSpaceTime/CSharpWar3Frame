using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Src.Components;

namespace War3Frame.Regression;

/// <summary>验证弹道超距、超时、到达优先与未启用时不过期。</summary>
internal static class ProjectileExpireRegression
{
    /// <summary>注册弹道过期回归场景。</summary>
    internal static void Register(List<RegressionCase> tests)
    {
        tests.Add(new("runtime", "ProjectileExpireOutOfRange", ProjectileExpireOutOfRange));
        tests.Add(new("runtime", "ProjectileExpireTimeout", ProjectileExpireTimeout));
        tests.Add(new("runtime", "ProjectileExpireArrivalWins", ProjectileExpireArrivalWins));
        tests.Add(new("runtime", "ProjectileExpireDisabled", ProjectileExpireDisabled));
    }

    private static void ProjectileExpireOutOfRange()
    {
        var store = new EntityStore();
        var caster = store.CreateEntity();
        var projectile = Linear(store, caster, maxDistance: 150f, speed: 100f);
        var root = Flight(store);

        root.Update(new UpdateTick(1f, 1f));
        Check.That(projectile.GetComponent<ProjectileRuntimeState>().phase == ProjectileLifecyclePhase.InFlight,
            "the first flight frame does not expire");
        Check.That(store.Query<ProjectileExpiredEvent>().Count == 0, "the first flight frame emits no expire fact");

        root.Update(new UpdateTick(1f, 2f));
        Check.That(projectile.GetComponent<ProjectileRuntimeState>().phase == ProjectileLifecyclePhase.Expired,
            "a linear projectile expires when traveled reaches max distance");
        Check.That(projectile.Tags.Has<EffectExpired>() && !projectile.Tags.Has<ProjectileArrived>(),
            "expiry is terminal and is not arrival");
        Check.That(!projectile.HasComponent<ProjectileExpireRequest>() && !projectile.HasComponent<ProjectileArriveRequest>(),
            "the expire request is consumed and no arrive request is written");
        Check.That(store.Query<DamageRequest>().Count == 0, "expiry does not create damage");
        var expired = Single<ProjectileExpiredEvent>(store).GetComponent<ProjectileExpiredEvent>();
        Check.That(expired.projectile == projectile && expired.source == caster, "one expire fact points at the projectile");
    }

    private static void ProjectileExpireTimeout()
    {
        var store = new EntityStore();
        var caster = store.CreateEntity();
        var projectile = Linear(store, caster, maxDistance: 0f, speed: 10f);
        projectile.AddComponent(new ProjectileLifetime { maxLifetime = 0.5f });
        var root = Flight(store);

        root.Update(new UpdateTick(0.2f, 0.2f));
        Check.That(projectile.GetComponent<ProjectileRuntimeState>().phase == ProjectileLifecyclePhase.InFlight,
            "the start frame does not count toward lifetime");

        root.Update(new UpdateTick(0.5f, 0.7f));
        Check.That(projectile.GetComponent<ProjectileRuntimeState>().phase == ProjectileLifecyclePhase.Expired
            && projectile.Tags.Has<EffectExpired>(),
            "lifetime expiry is terminal");
        Check.That(!projectile.Tags.Has<ProjectileArrived>() && store.Query<DamageRequest>().Count == 0,
            "timeout does not arrive or deal damage");
        Check.That(store.Query<ProjectileExpiredEvent>().Count == 1, "timeout emits one expire fact");
    }

    private static void ProjectileExpireArrivalWins()
    {
        var store = new EntityStore();
        var caster = store.CreateEntity();
        var projectile = Tracking(store, caster, distance: 40f, speed: 100f, threshold: 50f, maxDistance: 40f);
        projectile.AddComponent(new ProjectileLifetime { maxLifetime = 0.01f });
        var root = Flight(store);

        root.Update(new UpdateTick(1f, 1f));
        Check.That(projectile.Tags.Has<ProjectileArrived>()
            && projectile.GetComponent<ProjectileRuntimeState>().phase == ProjectileLifecyclePhase.Arrived,
            "same-frame arrival wins");
        Check.That(!projectile.Tags.Has<EffectExpired>() && store.Query<ProjectileExpiredEvent>().Count == 0,
            "arrival does not emit an expire fact");
    }

    private static void ProjectileExpireDisabled()
    {
        var store = new EntityStore();
        var caster = store.CreateEntity();
        var projectile = Linear(store, caster, maxDistance: 0f, speed: 100f);
        var root = Flight(store);

        root.Update(new UpdateTick(1f, 1f));
        root.Update(new UpdateTick(1f, 2f));
        Check.That(projectile.GetComponent<ProjectileRuntimeState>().phase == ProjectileLifecyclePhase.InFlight,
            "a non-positive max distance does not expire");
        Check.That(!projectile.HasComponent<ProjectileExpireRequest>() && store.Query<ProjectileExpiredEvent>().Count == 0,
            "disabled limits write no expire intent");
    }

    private static SystemRoot Flight(EntityStore store)
    {
        var root = new SystemRoot(store);
        root.Add(new ProjectileSystem());
        root.Add(new ProjectileLifecycleApplySystem());
        return root;
    }

    private static Entity Linear(EntityStore store, Entity caster, float maxDistance, float speed)
    {
        var projectile = store.CreateEntity(
            new EffectSource { caster = caster },
            new EffectTargetInfo { targetX = 1000, targetY = 0 },
            new Position(0, 0, 0),
            new ProjectileData
            {
                trajectoryType = ProjectileTrajectoryType.Linear,
                speed = speed,
                maxDistance = maxDistance,
                arrivalThreshold = 30f
            },
            new ProjectileRuntimeState { phase = ProjectileLifecyclePhase.PendingStart });
        projectile.AddTag<EffectPendingTag>();
        return projectile;
    }

    private static Entity Tracking(EntityStore store, Entity caster, float distance, float speed, float threshold,
        float maxDistance)
    {
        var projectile = store.CreateEntity(
            new EffectSource { caster = caster },
            new EffectTargetInfo { targetX = distance, targetY = 0 },
            new Position(0, 0, 0),
            new ProjectileData
            {
                trajectoryType = ProjectileTrajectoryType.Tracking,
                speed = speed,
                maxDistance = maxDistance,
                arrivalThreshold = threshold
            },
            new ProjectileRuntimeState { phase = ProjectileLifecyclePhase.PendingStart });
        projectile.AddTag<EffectPendingTag>();
        return projectile;
    }

    private static Entity Single<T>(EntityStore store) where T : struct, IComponent
    {
        Entity found = default;
        var count = 0;
        store.Query<T>().ForEachEntity((ref T _, Entity entity) =>
        {
            found = entity;
            count++;
        });
        Check.That(count == 1, "expected exactly one entity");
        return found;
    }
}

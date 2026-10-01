using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Helpers;
using War3Frame.Src.Components;
using War3Frame.Src.Systems;

namespace War3Frame.Regression;

/// <summary>验证远程命中、链跳、无后续目标与过期放弃结算。</summary>
internal static class RangedAttackRegression
{
    /// <summary>注册远程与闪电链攻击回归场景。</summary>
    internal static void Register(List<RegressionCase> tests)
    {
        tests.Add(new("runtime", "RangedAttackHit", RangedAttackHit));
        tests.Add(new("runtime", "ChainAttackJump", ChainAttackJump));
        tests.Add(new("runtime", "ChainAttackNoTarget", ChainAttackNoTarget));
        tests.Add(new("runtime", "RangedAttackExpireLink", RangedAttackExpireLink));
        tests.Add(new("runtime", "MeleeAttackDirect", MeleeAttackDirect));
    }

    private static void RangedAttackHit()
    {
        var store = new EntityStore();
        var attacker = Unit(store, 0, 0, AttackType.Ranged, damage: 40, range: 800);
        var target = Unit(store, 100, 0, AttackType.Melee, damage: 0, range: 0);
        var attack = AttackRoot(store);
        var flight = new SystemRoot(store);
        flight.Add(new ProjectileSystem());
        flight.Add(new ProjectileLifecycleApplySystem());
        flight.Add(new AttackProjectileImpactSystem());

        Attack(store, attacker, target);
        attack.Update(new UpdateTick(.01f, .01f));

        Check.That(store.Query<UnitAttackedRequest>().Count == 0, "attack request is consumed when the projectile is created");
        Check.That(store.Query<DamageRequest>().Count == 0, "in-flight ranged attack does not deal damage");
        var projectile = Single<AttackProjectileImpact>(store);
        Check.That(projectile.GetComponent<ProjectileRuntimeState>().phase == ProjectileLifecyclePhase.PendingStart,
            "creation tick does not settle the projectile");
        Check.Near(projectile.GetComponent<AttackProjectileImpact>().damage, 40, "impact stores the attack snapshot");
        Check.Near(projectile.GetComponent<ProjectileData>().maxDistance, 800, "max distance is the attack range");

        flight.Update(new UpdateTick(.01f, .02f));
        Check.That(projectile.GetComponent<ProjectileRuntimeState>().phase == ProjectileLifecyclePhase.InFlight
            && store.Query<DamageRequest>().Count == 0,
            "in-flight projectile still has no damage");

        for (var step = 0; step < 8 && store.Query<DamageRequest>().Count == 0; step++)
            flight.Update(new UpdateTick(.05f, .07f + step * .05f));

        var request = Single<DamageRequest>(store);
        var damage = request.GetComponent<DamageRequest>();
        Check.That(damage.damage.damageSrc == DamageSrc.Ranged && damage.target == target,
            "arrival creates one ranged damage request");
        Check.Near(damage.damage.damage, 40, "arrival damage equals the creation snapshot");
        Check.That(!projectile.HasComponent<AttackProjectileImpact>(), "arrival consumes the impact payload");
    }

    private static void ChainAttackJump()
    {
        var store = new EntityStore();
        var attacker = EnemyPair(store, out var first, out var second);
        AttributeHelper.GetOrCreateAttr(attacker, AttributeHelper.AttackDamage, 100);
        AttributeHelper.GetOrCreateAttr(attacker, AttributeHelper.AttackRange, 200);
        AttackHelper.SetAttackType(attacker, AttackType.Chain);
        var root = AttackRoot(store);

        Attack(store, attacker, first);
        root.Update(new UpdateTick(.01f, .01f));

        var damages = Damages(store);
        Check.That(damages.Count == 2, "chain hits the primary and one jump");
        Check.That(damages[0].target == first && damages[1].target == second, "jumps follow nearest enemy order");
        Check.Near(damages[0].damage.damage, 100, "first jump uses full attack damage");
        Check.Near(damages[1].damage.damage, 85, "second jump decays by 0.85");
        Check.That(damages.All(item => item.damage.damageSrc == DamageSrc.Ranged), "chain damage stays ranged");
        Check.That(store.Query<ProjectileData>().Count == 0, "chain does not create a projectile");
    }

    private static void ChainAttackNoTarget()
    {
        var store = new EntityStore();
        var attacker = Unit(store, 0, 0, AttackType.Chain, damage: 50, range: 100);
        var target = Enemy(store, attacker, 0, 0);
        store.CreateEntity(new Position(1000, 0, 0), new UnitLifeState { isAlive = true });
        var root = AttackRoot(store);

        Attack(store, attacker, target);
        root.Update(new UpdateTick(.01f, .01f));

        var damages = Damages(store);
        Check.That(damages.Count == 1 && damages[0].target == target, "chain stops after the only reachable target");
        Check.That(damages.All(item => !item.target.IsNull), "chain never emits an empty target");
    }

    private static void RangedAttackExpireLink()
    {
        var store = new EntityStore();
        var attacker = Unit(store, 0, 0, AttackType.Ranged, damage: 30, range: 50);
        var target = Unit(store, 1000, 0, AttackType.Melee, damage: 0, range: 0);
        store.CreateEntity(
            new EffectSource { caster = attacker },
            new EffectTargetInfo { targetUnit = target, targetX = 1000, targetY = 0 },
            new Position(0, 0, 0),
            new ProjectileData
            {
                trajectoryType = ProjectileTrajectoryType.Tracking,
                speedValue = EffectValueSpec.Constant(900),
                speed = 900,
                maxDistance = 50
            },
            new ProjectileRuntimeState { phase = ProjectileLifecyclePhase.InFlight },
            new AttackProjectileImpact { attacker = attacker, target = target, damage = 30, token = 1 });
        var projectile = Single<AttackProjectileImpact>(store);
        projectile.AddTag<EffectPendingTag>();
        projectile.AddComponent(new ProjectileExpireRequest());
        var expire = new SystemRoot(store);
        expire.Add(new ProjectileLifecycleApplySystem());
        expire.Update(new UpdateTick(.01f, .01f));
        Check.That(projectile.Tags.Has<EffectExpired>() && !projectile.HasComponent<ProjectileExpireRequest>(),
            "existing expire branch reaches the terminal tag");

        var root = new SystemRoot(store);
        root.Add(new AttackProjectileImpactSystem());
        root.Update(new UpdateTick(.01f, .02f));
        Check.That(store.Query<DamageRequest>().Count == 0, "expired attack projectile does not deal damage");
        Check.That(!projectile.HasComponent<AttackProjectileImpact>(), "expiry removes the impact payload");
        Check.That(projectile.Tags.Has<EffectExpired>(), "terminal expire tag stays on the projectile");
    }

    private static void MeleeAttackDirect()
    {
        var store = new EntityStore();
        var attacker = Unit(store, 0, 0, AttackType.Melee, damage: 12, range: 0);
        var target = Unit(store, 10, 0, AttackType.Melee, damage: 0, range: 0);
        var root = AttackRoot(store);

        Attack(store, attacker, target);
        root.Update(new UpdateTick(.01f, .01f));

        var damage = Single<DamageRequest>(store).GetComponent<DamageRequest>();
        Check.That(damage.damage.damageSrc == DamageSrc.Melee && damage.target == target, "default melee still resolves directly");
        Check.That(store.Query<ProjectileData>().Count == 0, "melee does not create a projectile");
    }

    private static SystemRoot AttackRoot(EntityStore store)
    {
        var root = new SystemRoot(store);
        root.Add(new SpatialGridSystem());
        root.Add(new AttackSimulationSystem());
        return root;
    }

    private static Entity Unit(EntityStore store, float x, float y, AttackType type, float damage, float range)
    {
        var unit = store.CreateEntity(new Position(x, y, 0), new UnitLifeState { isAlive = true });
        if (type != AttackType.Melee)
            AttackHelper.SetAttackType(unit, type);
        if (damage > 0f)
            AttributeHelper.GetOrCreateAttr(unit, AttributeHelper.AttackDamage, damage);
        if (range > 0f)
            AttributeHelper.GetOrCreateAttr(unit, AttributeHelper.AttackRange, range);
        return unit;
    }

    private static Entity EnemyPair(EntityStore store, out Entity first, out Entity second)
    {
        var attackerPlayer = Player(store, 0);
        var enemyPlayer = Player(store, 1);
        PlayerNative[] players = [attackerPlayer, enemyPlayer];
        PlayerHelper.InitializePlayers(ref players);
        PlayerHelper.SetAlliance(attackerPlayer, enemyPlayer, false);

        var attacker = Owned(store, attackerPlayer, 0, 0);
        first = Owned(store, enemyPlayer, 10, 0);
        second = Owned(store, enemyPlayer, 40, 0);
        return attacker;
    }

    private static Entity Enemy(EntityStore store, Entity attacker, float x, float y)
    {
        var attackerPlayer = Player(store, 0);
        var enemyPlayer = Player(store, 1);
        PlayerNative[] players = [attackerPlayer, enemyPlayer];
        PlayerHelper.InitializePlayers(ref players);
        PlayerHelper.SetAlliance(attackerPlayer, enemyPlayer, false);
        attacker.AddComponent(new UnitOwner { player = attackerPlayer.getentity });
        return Owned(store, enemyPlayer, x, y);
    }

    private static PlayerNative Player(EntityStore store, int index)
    {
        var entity = store.CreateEntity();
        var player = new PlayerNative { index = index, getentity = entity };
        entity.AddComponent(player);
        return player;
    }

    private static Entity Owned(EntityStore store, PlayerNative player, float x, float y)
        => store.CreateEntity(new Position(x, y, 0), new UnitOwner { player = player.getentity },
            new UnitLifeState { isAlive = true, lifePhase = UnitLifecyclePhase.Alive });

    private static void Attack(EntityStore store, Entity attacker, Entity target)
        => store.CreateEntity(new UnitAttackedRequest { attacker = attacker, target = target });

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

    private static List<DamageRequest> Damages(EntityStore store)
    {
        var list = new List<DamageRequest>();
        store.Query<DamageRequest>().ForEachEntity((ref DamageRequest request, Entity _) => list.Add(request));
        return list;
    }
}

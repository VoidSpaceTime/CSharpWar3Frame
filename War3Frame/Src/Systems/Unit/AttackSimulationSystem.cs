using System.Collections.Generic;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame;
using War3Frame.Helpers;
using War3Frame.Src.Components;
using War3Frame.Systems;

namespace War3Frame.Src.Systems;

/// <summary>
/// 模拟攻击系统：消费 UnitAttackedRequest，按攻击形态分流结算。
/// 近战直接生成 DamageRequest；远程创建跟踪弹道并在到达后结算；闪电链同帧选跳并衰减。
/// 执行顺序在 DamageResolveSystem(125) 之前，保证同帧请求可被结算。
/// </summary>
[SystemRegister(SystemKind.Immediate, 124)]
public class AttackSimulationSystem : QuerySystem<UnitAttackedRequest>
{
    /// <summary>攻击弹道速度。本变更不新增弹速属性。</summary>
    private const float AttackProjectileSpeed = 900f;

    /// <summary>攻击弹道到达阈值，与弹道系统的缺省阈值一致。</summary>
    private const float AttackArrivalThreshold = 30f;

    /// <summary>闪电链缺省跳数（含第一跳）。</summary>
    private const int DefaultChainJumps = 4;

    /// <summary>闪电链缺省每跳衰减系数。</summary>
    private const float DefaultChainDecay = 0.85f;

    private readonly List<Entity> _resolved = new();
    private readonly List<RangedLaunch> _ranged = new();
    private readonly HashSet<Entity> _chainHit = new();

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref UnitAttackedRequest request, Entity requestEntity) =>
        {
            if (request.attacker.IsNull || request.target.IsNull)
            {
                _resolved.Add(requestEntity);
                return;
            }

            var attackType = AttackHelper.GetAttackType(request.attacker);
            switch (attackType)
            {
                case AttackType.Melee:
                    // 近战：命中即结算伤害（伤害 = AttackDamage finalValue）
                    var damage = GetAttackDamage(request.attacker);
                    if (damage > 0f)
                        CreateDamage(request.attacker, request.target, damage, DamageSrc.Melee);
                    break;

                case AttackType.Ranged:
                    // 创建弹道会改变 archetype，不能在本查询循环内执行。
                    _ranged.Add(new RangedLaunch(request, requestEntity.Id));
                    break;

                case AttackType.Chain:
                    ResolveChain(request);
                    break;
            }

            _resolved.Add(requestEntity);
        });

        foreach (var launch in _ranged)
            CreateRangedProjectile(launch.Request, launch.Token);
        _ranged.Clear();

        foreach (var entity in _resolved)
            entity.DeleteEntity();
        _resolved.Clear();
    }

    /// <summary>循环外再创建的远程攻击。token 是请求实体 id 快照。</summary>
    private readonly record struct RangedLaunch(UnitAttackedRequest Request, int Token);

    /// <summary>创建跟踪攻击弹道；缺位置则不建弹道、不结算。</summary>
    private static void CreateRangedProjectile(UnitAttackedRequest request, int token)
    {
        if (!request.attacker.TryGetComponent<Position>(out var origin)
            || !request.target.TryGetComponent<Position>(out var targetPos))
            return;

        var damage = GetAttackDamage(request.attacker);
        var range = AttributeHelper.GetFinalValue(request.attacker, AttributeHelper.AttackRange);
        var maxDistance = range > 0f ? range : 0f;
        var store = request.attacker.Store ?? Game.Store;
        store.CreateEntity(
            new EffectSource { caster = request.attacker, ability = default },
            new EffectTargetInfo
            {
                targetUnit = request.target,
                targetX = targetPos.x,
                targetY = targetPos.y
            },
            new Position(origin.x, origin.y, origin.z),
            new ProjectileData
            {
                trajectoryType = ProjectileTrajectoryType.Tracking,
                speed = AttackProjectileSpeed,
                arrivalThreshold = AttackArrivalThreshold,
                maxDistance = maxDistance
            },
            new ProjectileRuntimeState { phase = ProjectileLifecyclePhase.PendingStart },
            new AttackProjectileImpact
            {
                attacker = request.attacker,
                target = request.target,
                damage = damage,
                token = token
            }).AddTag<EffectPendingTag>();
    }

    /// <summary>以主目标为第一跳，在射程内按最近敌对单位继续跳并衰减。</summary>
    private void ResolveChain(UnitAttackedRequest request)
    {
        var attacker = request.attacker;
        var baseDamage = GetAttackDamage(attacker);
        var config = attacker.TryGetComponent<AttackChainConfig>(out var chain)
            ? chain
            : new AttackChainConfig { maxJumps = DefaultChainJumps, decay = DefaultChainDecay };
        var maxJumps = config.maxJumps < 1 ? 1 : config.maxJumps;
        var decay = config.decay < 0f ? 0f : config.decay;
        var range = AttributeHelper.GetFinalValue(attacker, AttributeHelper.AttackRange);

        _chainHit.Clear();
        var current = request.target;
        for (var jump = 0; jump < maxJumps; jump++)
        {
            if (current.IsNull)
                break;

            _chainHit.Add(current);
            var damage = baseDamage * MathF.Pow(decay, jump);
            if (damage <= 0f)
                break;

            CreateDamage(attacker, current, damage, DamageSrc.Ranged);
            if (jump + 1 >= maxJumps || range <= 0f || !current.TryGetComponent<Position>(out var pos))
                break;

            current = FindNextChainTarget(attacker, pos.x, pos.y, range);
        }

        _chainHit.Clear();
    }

    /// <summary>在圆心射程内选取尚未命中、距离最近的敌对单位。</summary>
    private Entity FindNextChainTarget(Entity attacker, float x, float y, float radius)
    {
        var candidates = GroupHelper.FindInCircle(attacker, x, y, radius, TargetFilter.Enemy);
        Entity nearest = default;
        var nearestDistSq = float.MaxValue;
        foreach (var candidate in candidates)
        {
            if (candidate.IsNull || candidate == attacker || _chainHit.Contains(candidate))
                continue;
            if (!candidate.TryGetComponent<Position>(out var pos))
                continue;

            var dx = pos.x - x;
            var dy = pos.y - y;
            var distSq = dx * dx + dy * dy;
            if (distSq < nearestDistSq)
            {
                nearestDistSq = distSq;
                nearest = candidate;
            }
        }

        return nearest;
    }

    /// <summary>创建一条物理伤害请求，不在此处扣血。</summary>
    private static void CreateDamage(Entity source, Entity target, float damage, DamageSrc damageSrc)
    {
        var store = source.Store ?? Game.Store;
        store.CreateEntity(new DamageRequest
        {
            source = source,
            target = target,
            damage = new DamageBase
            {
                damage = damage,
                damageType = DamageType.Physical,
                damageSrc = damageSrc,
                source = source,
                target = target
            }
        });
    }

    /// <summary>读取单位攻击力（AttackDamage finalValue，ECS 优先；无属性视为 0）。</summary>
    private static float GetAttackDamage(Entity unit)
    {
        return AttributeHelper.GetFinalValue(unit, AttributeHelper.AttackDamage);
    }
}

/// <summary>
/// 攻击弹道命中结算：到达后按快照发伤害，过期只丢弃快照。
/// </summary>
[SystemRegister(SystemKind.Interval, 123)]
public class AttackProjectileImpactSystem : QuerySystem<AttackProjectileImpact, ProjectileRuntimeState>
{
    private readonly List<Entity> _arrived = new();
    private readonly List<Entity> _expired = new();

    protected override void OnUpdate()
    {
        _arrived.Clear();
        _expired.Clear();
        Query.ForEachEntity((ref AttackProjectileImpact impact, ref ProjectileRuntimeState runtime, Entity effect) =>
        {
            if (effect.Tags.Has<EffectExpired>())
                _expired.Add(effect);
            else if (effect.Tags.Has<ProjectileArrived>())
                _arrived.Add(effect);
        });

        foreach (var effect in _expired)
            RemoveImpact(effect);

        foreach (var effect in _arrived)
        {
            if (!effect.TryGetComponent<AttackProjectileImpact>(out var impact))
                continue;

            if (impact.damage > 0f && !impact.target.IsNull)
            {
                var store = effect.Store ?? Game.Store;
                store.CreateEntity(new DamageRequest
                {
                    source = impact.attacker,
                    target = impact.target,
                    damage = new DamageBase
                    {
                        damage = impact.damage,
                        damageType = DamageType.Physical,
                        damageSrc = DamageSrc.Ranged,
                        source = impact.attacker,
                        target = impact.target
                    }
                });
            }

            RemoveImpact(effect);
        }
    }

    /// <summary>移除攻击结算快照，不删除弹道实体。</summary>
    private static void RemoveImpact(Entity effect)
    {
        if (!effect.IsNull && effect.HasComponent<AttackProjectileImpact>())
            effect.RemoveComponent<AttackProjectileImpact>();
    }
}

using Friflo.Engine.ECS;
using War3Frame.Helpers;

namespace War3Frame;

/// <summary>
/// 技能效果实例化助手。
/// 负责把 ability 上的效果配置展开为运行时 effect entity，不直接结算效果。
/// </summary>
public static class AbilityEffectHelper
{
    // 仅用于跨系统追踪本次效果实例，不承载业务语义。
    private static int _nextEffectId = 1;

    /// <summary>
    /// 创建一次技能效果实例。
    /// ability 持有长期配置；这里只生成带 EffectPending 的临时 effect entity。
    /// </summary>
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

        // 兼容旧路径：直接挂在 ability 上的效果 payload 会被复制到运行时 effect。
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

        if (ability.TryGetComponent<LineSearchData>(out var line))
            effectEntity.AddComponent(line);

        if (ability.TryGetComponent<GroundAreaCreateData>(out var groundAreaCreate))
            effectEntity.AddComponent(groundAreaCreate);

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

        // 新路径：把配置友好的 EffectSpec 展开成现有 EffectSystem 可消费的组件。
        if (AbilityHelper.TryGetEffectSpec(ability, out var spec))
            ApplyEffectSpec(effectEntity, spec);

        return effectEntity;
    }

    /// <summary>
    /// 为区域搜索命中的单个目标创建子效果。
    /// 子效果复用父效果来源与 payload，只替换目标。
    /// </summary>
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

        if (parentEffect.TryGetComponent<EffectVisualData>(out var visual))
            childEntity.AddComponent(visual);

        if (parentEffect.TryGetComponent<GroundAreaCreateData>(out var groundAreaCreate))
            childEntity.AddComponent(groundAreaCreate);

        return childEntity;
    }

    /// <summary>
    /// 触发技能生命周期行为效果，供施法、挂载和移除工作流复用。
    /// </summary>
    public static bool TriggerBehaviorEffect(Entity owner, Entity ability, AbilityBehaviorTrigger trigger,
        Entity targetUnit = default, float targetX = 0f, float targetY = 0f)
    {
        if (!TryGetBehaviorEffect(ability, trigger, out var effect))
            return false;

        var previous = GetCurrentEffectSpec(ability, out var hadPrevious);
        AbilityHelper.SetEffectSpec(ability, effect);
        CreateEffectEntity(owner, ability, targetUnit, targetX, targetY);
        RestoreEffectSpec(ability, previous, hadPrevious);
        return true;
    }

    /// <summary>
    /// 为弹道到达事件创建独立效果链实例，避免到达后效果污染弹道本体生命周期。
    /// </summary>
    public static Entity CreateArriveEffect(Entity projectileEffect, EffectSpec arriveEffect, Position position)
    {
        var source = projectileEffect.GetComponent<EffectSource>();
        var target = projectileEffect.GetComponent<EffectTargetInfo>();

        var arriveEntity = Game.Store.CreateEntity(
            source,
            new EffectTargetInfo
            {
                targetUnit = target.targetUnit,
                targetX = position.x,
                targetY = position.y
            },
            new AbilityEffectContext
            {
                caster = source.caster,
                ability = source.ability,
                sourceEffect = projectileEffect,
                targetUnit = target.targetUnit,
                targetX = position.x,
                targetY = position.y,
                effectId = _nextEffectId++
            });
        arriveEntity.AddTag<EffectPending>();
        arriveEntity.AddComponent(position);
        ApplyEffectSpec(arriveEntity, arriveEffect);
        return arriveEntity;
    }

    /// <summary>
    /// 将数据化效果链转换为现有 ECS 组件。
    /// 这里只做数据转换，不执行伤害、治疗、Buff 或 native 调用。
    /// </summary>
    private static void ApplyEffectSpec(Entity effectEntity, EffectSpec spec)
    {
        foreach (var step in spec.steps)
        {
            switch (step.kind)
            {
                case EffectStepKind.Damage:
                    effectEntity.AddComponent(new DamageEffectData
                    {
                        value = step.damage.value,
                        damageType = step.damage.damageType,
                        damageSrc = step.damage.damageSrc
                    });
                    break;
                case EffectStepKind.Heal:
                    effectEntity.AddComponent(new HealEffectData
                    {
                        value = step.heal.value,
                        valueTypeId = step.heal.valueTypeId,
                        amount = step.heal.amount
                    });
                    break;
                case EffectStepKind.Buff:
                    effectEntity.AddComponent(new ApplyBuffData
                    {
                        buffId = step.buff.buffId,
                        durationValue = step.buff.duration,
                        attrTypeId = step.buff.attrTypeId,
                        modifyType = step.buff.modifyType,
                        modifyValue = step.buff.value,
                        refreshBehavior = step.buff.refreshBehavior
                    });
                    break;
                case EffectStepKind.AreaSearch:
                    effectEntity.AddComponent(new AreaSearchData
                    {
                        centerX = step.areaSearch.centerX,
                        centerY = step.areaSearch.centerY,
                        radiusValue = step.areaSearch.radius,
                        maxTargets = step.areaSearch.maxTargets,
                        filter = step.areaSearch.filter,
                        customFilterId = step.areaSearch.customFilterId
                    });
                    break;
                case EffectStepKind.LineSearch:
                    effectEntity.AddComponent(new LineSearchData
                    {
                        rangeValue = step.lineSearch.range,
                        range = step.lineSearch.fallbackRange,
                        widthValue = step.lineSearch.width,
                        width = step.lineSearch.fallbackWidth,
                        maxTargets = step.lineSearch.maxTargets,
                        filter = step.lineSearch.filter,
                        customFilterId = step.lineSearch.customFilterId,
                        reactionTag = step.lineSearch.reactionTag
                    });
                    break;
                case EffectStepKind.GroundAreaCreate:
                    effectEntity.AddComponent(new GroundAreaCreateData
                    {
                        tags = step.groundAreaCreate.tags,
                        radiusValue = step.groundAreaCreate.radius,
                        radius = step.groundAreaCreate.fallbackRadius,
                        durationValue = step.groundAreaCreate.duration,
                        duration = step.groundAreaCreate.fallbackDuration,
                        buff = step.groundAreaCreate.buff,
                        periodicDamage = step.groundAreaCreate.periodicDamage,
                        reaction = step.groundAreaCreate.reaction
                    });
                    break;
                case EffectStepKind.EffectVisual:
                    AddEffectVisual(effectEntity, step.effectVisual);
                    break;
                case EffectStepKind.Projectile:
                    var projectile = new ProjectileData
                    {
                        trajectoryType = step.projectile.trajectoryType,
                        model = step.projectile.model,
                        speedValue = step.projectile.speed,
                        effectEntity = step.projectile.effectEntity,
                        arrivalThresholdValue = step.projectile.arrivalThreshold,
                        maxDistanceValue = step.projectile.maxDistance,
                        hitRadiusValue = step.projectile.hitRadius,
                        hitFilter = step.projectile.hitFilter,
                        canHitSameTarget = step.projectile.canHitSameTarget,
                        arriveEffect = step.projectile.arriveEffect
                    };

                    effectEntity.AddComponent(projectile);
                    EnsureProjectilePosition(effectEntity, effectEntity.GetComponent<EffectSource>().caster);
                    EnsureProjectileRuntimeState(effectEntity);
                    break;
            }
        }
    }

    /// <summary>
    /// 为弹道 effect 补充初始位置，供 ProjectileSystem 推进。
    /// </summary>
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

    /// <summary>
    /// 为弹道 effect 补充运行时阶段状态。
    /// </summary>
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

    /// <summary>
    /// 追加视觉步骤；多个视觉步骤会保留顺序并由 EffectVisualSystem 逐个消费。
    /// </summary>
    private static void AddEffectVisual(Entity effectEntity, EffectVisualStepSpec visual)
    {
        if (effectEntity.TryGetComponent<EffectVisualData>(out var data) && data.steps != null)
        {
            data.steps.Add(visual);
            effectEntity.AddComponent(data);
            return;
        }

        if (effectEntity.TryGetComponent<EffectVisualData>(out var existing))
        {
            existing.steps = new List<EffectVisualStepSpec>
            {
                ToStep(existing),
                visual
            };
            existing.nextIndex = 0;
            effectEntity.AddComponent(existing);
            return;
        }

        effectEntity.AddComponent(new EffectVisualData
        {
            kind = visual.kind,
            model = visual.model,
            key = visual.key,
            attachPoint = visual.attachPoint,
            durationValue = visual.duration,
            duration = visual.fallbackDuration,
            hasPoint = visual.hasPoint,
            x = visual.x,
            y = visual.y,
            z = visual.z
        });
    }

    /// <summary>
    /// 将单步运行时视觉 payload 转回队列步骤，便于升级为多步视觉队列。
    /// </summary>
    private static EffectVisualStepSpec ToStep(EffectVisualData data)
    {
        return new EffectVisualStepSpec
        {
            kind = data.kind,
            model = data.model,
            key = data.key,
            attachPoint = data.attachPoint,
            duration = data.durationValue,
            fallbackDuration = data.duration,
            hasPoint = data.hasPoint,
            x = data.x,
            y = data.y,
            z = data.z
        };
    }

    /// <summary>
    /// 从技能行为配置中查找指定生命周期触发点的效果链。
    /// </summary>
    private static bool TryGetBehaviorEffect(Entity ability, AbilityBehaviorTrigger trigger, out EffectSpec effect)
    {
        if (ability.TryGetComponent<AbilityBehaviorData>(out var data) && data.behaviors != null)
        {
            foreach (var behavior in data.behaviors)
            {
                if (behavior.trigger == trigger && behavior.effect != null)
                {
                    effect = behavior.effect;
                    return true;
                }
            }
        }

        effect = null!;
        return false;
    }

    /// <summary>
    /// 暂存技能当前默认效果链，供临时触发生命周期行为后恢复。
    /// </summary>
    private static EffectSpec? GetCurrentEffectSpec(Entity ability, out bool exists)
    {
        if (AbilityHelper.TryGetEffectSpec(ability, out var spec))
        {
            exists = true;
            return spec;
        }

        exists = false;
        return null;
    }

    /// <summary>
    /// 恢复生命周期行为触发前的默认效果链，避免临时 spec 泄漏到后续施法。
    /// </summary>
    private static void RestoreEffectSpec(Entity ability, EffectSpec? previous, bool hadPrevious)
    {
        if (hadPrevious && previous != null)
            AbilityHelper.SetEffectSpec(ability, previous);
        else
            ability.RemoveComponent<EffectSpecData>();
    }
}

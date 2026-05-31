using Friflo.Engine.ECS;
using War3Frame.Components;
using War3Frame.Src.Components;

namespace War3Frame.Helpers;

/// <summary>
/// 技能效果 Builder 的新命名入口；迁移期委托给 EffectSpecBuilder。
/// </summary>
public sealed class AbilityEffectSpecBuilder
{
    private readonly EffectSpecBuilder _inner;

    private AbilityEffectSpecBuilder(EffectSpecBuilder inner)
    {
        _inner = inner;
    }

    /// <summary>
    /// 创建一个按顺序执行的技能效果链。
    /// </summary>
    public static AbilityEffectSpecBuilder Chain()
    {
        return new AbilityEffectSpecBuilder(EffectSpecBuilder.Chain());
    }

    /// <summary>
    /// 构建可挂到技能行为或物品使用上的效果规格。
    /// </summary>
    public AbilityEffectSpec Build()
    {
        return new AbilityEffectSpec(_inner.Build());
    }

    /// <summary>
    /// 添加伤害结算步骤，实际扣血仍由伤害系统处理。
    /// </summary>
    public AbilityEffectSpecBuilder Damage(AbilityValue value, DamageType damageType = DamageType.Magical,
        DamageSrc damageSrc = DamageSrc.Skill)
    {
        _inner.Damage(value.EffectValue, damageType, damageSrc);
        return this;
    }

    /// <summary>
    /// 添加治疗结算步骤，实际回血仍由治疗系统处理。
    /// </summary>
    public AbilityEffectSpecBuilder Heal(AbilityValue value, int valueTypeId = 0, float amount = 0f)
    {
        _inner.Heal(value.EffectValue, valueTypeId, amount);
        return this;
    }

    /// <summary>
    /// 添加 Buff 应用步骤，属性修改通过 Buff/属性系统落地。
    /// </summary>
    public AbilityEffectSpecBuilder Buff(string buffId, AbilityValue duration, int attrTypeId, ModifyType modifyType,
        AbilityValue value, BuffRefreshBehavior refreshBehavior = BuffRefreshBehavior.RefreshDuration)
    {
        _inner.Buff(buffId, duration.EffectValue, attrTypeId, modifyType, value.EffectValue, refreshBehavior);
        return this;
    }

    /// <summary>
    /// 添加圆形区域搜索步骤，用搜索结果改变后续效果目标集。
    /// </summary>
    public AbilityEffectSpecBuilder Area(TargetFilter filter, int maxTargets = 0, AbilityValue radius = default,
        string? customFilterId = null, float centerX = 0f, float centerY = 0f)
    {
        _inner.Area(filter, maxTargets, radius.EffectValue, customFilterId, centerX, centerY);
        return this;
    }

    /// <summary>
    /// 添加线形搜索步骤，可用于穿刺、喷火等沿线命中效果。
    /// </summary>
    public AbilityEffectSpecBuilder Line(TargetFilter filter, AbilityValue range = default, float fallbackRange = 0f,
        AbilityValue width = default, float fallbackWidth = 0f, int maxTargets = 0,
        string? customFilterId = null, GroundAreaTag reactionTag = GroundAreaTag.None)
    {
        _inner.Line(filter, range.EffectValue, fallbackRange, width.EffectValue, fallbackWidth, maxTargets,
            customFilterId, reactionTag);
        return this;
    }

    /// <summary>
    /// 添加地面区域创建步骤，只写 ECS 区域语义，不直接创建 War3 原生表现。
    /// </summary>
    public AbilityEffectSpecBuilder GroundArea(GroundAreaTag tags, AbilityValue radius = default,
        float fallbackRadius = 0f, AbilityValue duration = default, float fallbackDuration = 0f,
        GroundAreaBuffData buff = default, GroundAreaPeriodicDamageData periodicDamage = default,
        GroundAreaReactionData reaction = default)
    {
        _inner.GroundArea(tags, radius.EffectValue, fallbackRadius, duration.EffectValue, fallbackDuration,
            buff, periodicDamage, reaction);
        return this;
    }

    /// <summary>
    /// 添加弹道步骤，移动、命中与特效表现由后续系统推进。
    /// </summary>
    public AbilityEffectSpecBuilder Projectile(string model, AbilityValue speed,
        ProjectileTrajectoryType trajectoryType = ProjectileTrajectoryType.Tracking,
        AbilityValue arrivalThreshold = default, AbilityValue maxDistance = default,
        AbilityValue hitRadius = default, TargetFilter hitFilter = TargetFilter.None,
        bool canHitSameTarget = false, Entity effectEntity = default,
        Func<AbilityEffectSpecBuilder, AbilityEffectSpecBuilder>? onArrive = null)
    {
        Func<EffectSpecBuilder, EffectSpecBuilder>? innerArrive = null;
        if (onArrive != null)
        {
            innerArrive = builder =>
            {
                var wrapped = new AbilityEffectSpecBuilder(builder);
                onArrive(wrapped);
                return builder;
            };
        }

        _inner.Projectile(model, speed.EffectValue, trajectoryType, arrivalThreshold.EffectValue,
            maxDistance.EffectValue, hitRadius.EffectValue, hitFilter, canHitSameTarget, effectEntity, innerArrive);
        return this;
    }

    /// <summary>
    /// 添加视觉特效步骤，只写 ECS 视觉意图，由 Native 特效系统执行原生表现。
    /// </summary>
    public AbilityEffectSpecBuilder Effect(EffectVisualKind kind, string model, string? key = null,
        EffectAttachType attachPoint = EffectAttachType.Origin, AbilityValue duration = default,
        float fallbackDuration = -1f, bool hasPoint = false, float x = 0f, float y = 0f, float z = 0f)
    {
        _inner.Effect(kind, model, key, attachPoint, duration.EffectValue, fallbackDuration, hasPoint, x, y, z);
        return this;
    }

    /// <summary>
    /// 按 key 请求移除该来源此前创建的长期视觉特效。
    /// </summary>
    public AbilityEffectSpecBuilder RemoveEffectByKey(string key)
    {
        _inner.RemoveEffectByKey(key);
        return this;
    }

    /// <summary>
    /// 为最近一个 Projectile 步骤追加到达后执行的效果链。
    /// </summary>
    public AbilityEffectSpecBuilder OnProjectileArrive(Func<AbilityEffectSpecBuilder, AbilityEffectSpecBuilder> configure)
    {
        _inner.OnProjectileArrive(builder =>
        {
            var wrapped = new AbilityEffectSpecBuilder(builder);
            configure(wrapped);
            return builder;
        });
        return this;
    }
}

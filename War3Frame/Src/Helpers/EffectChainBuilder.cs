using Friflo.Engine.ECS;
using War3Frame.Components;
using War3Frame.Src.Components;

namespace War3Frame.Helpers;

/// <summary>
/// 效果链 Builder，面向技能、物品等 authoring 入口按顺序描述效果步骤。
/// </summary>
public sealed class EffectChainBuilder
{
    // Builder 面向配置/编辑器友好的效果链描述；普通技能尽量使用 statId + formulaId + 参数表。
    private readonly EffectSpec _spec = new();

    private EffectChainBuilder()
    {
    }

    /// <summary>
    /// 创建一个空效果链，后续按调用顺序追加 step。
    /// </summary>
    public static EffectChainBuilder Chain()
    {
        // 从一个空链开始按顺序追加 step，执行系统会按 steps 顺序解释效果。
        return new EffectChainBuilder();
    }

    /// <summary>
    /// 构建底层效果规格数据；构建完成后调用方应按只读配置使用。
    /// </summary>
    public EffectSpec Build()
    {
        // 返回同一个 spec 实例，调用方构建完成后应视为只读配置使用。
        return _spec;
    }

    public EffectChainBuilder Damage(EffectValueSpec value, DamageType damageType = DamageType.Magical,
        DamageSrc damageSrc = DamageSrc.Skill)
    {
        // 低层入口允许直接传 EffectValueSpec，保留给已有数据或高级自定义场景。
        _spec.steps.Add(EffectStepSpec.Damage(new DamageEffectStepSpec
        {
            value = value,
            damageType = damageType,
            damageSrc = damageSrc
        }));
        return this;
    }

    /// <summary>
    /// 添加作者友好数值写法的伤害结算步骤，实际扣血仍由伤害系统处理。
    /// </summary>
    public EffectChainBuilder Damage(AbilityValue value, DamageType damageType = DamageType.Magical,
        DamageSrc damageSrc = DamageSrc.Skill)
    {
        return Damage(value.EffectValue, damageType, damageSrc);
    }

    public EffectChainBuilder Damage(string formulaId, int statId, DamageType damageType = DamageType.Magical,
        DamageSrc damageSrc = DamageSrc.Skill, Dictionary<string, float>? parameters = null)
    {
        // 推荐入口：用公式 ID + statId + 参数表表达数值，方便编辑器序列化和调参。
        return Damage(EffectValueSpec.Stat(statId, formulaId, parameters), damageType, damageSrc);
    }

    public EffectChainBuilder Heal(EffectValueSpec value, int valueTypeId = 0, float amount = 0f)
    {
        // heal step 既保留旧 amount 字段，也优先承载新的 EffectValueSpec。
        _spec.steps.Add(EffectStepSpec.Heal(new HealEffectStepSpec
        {
            value = value,
            valueTypeId = valueTypeId,
            amount = amount
        }));
        return this;
    }

    /// <summary>
    /// 添加作者友好数值写法的治疗结算步骤，实际回血仍由治疗系统处理。
    /// </summary>
    public EffectChainBuilder Heal(AbilityValue value, int valueTypeId = 0, float amount = 0f)
    {
        return Heal(value.EffectValue, valueTypeId, amount);
    }

    public EffectChainBuilder Heal(string formulaId, int statId, Dictionary<string, float>? parameters = null)
    {
        // 推荐入口：治疗数值同样通过公式注册表解释，避免普通技能直接写 delegate。
        return Heal(EffectValueSpec.Stat(statId, formulaId, parameters), statId);
    }

    public EffectChainBuilder Buff(string buffId, EffectValueSpec duration, int attrTypeId, ModifyType modifyType,
        EffectValueSpec value, BuffRefreshBehavior refreshBehavior = BuffRefreshBehavior.RefreshDuration)
    {
        // buff step 只描述长期属性贡献，真正挂载/刷新由效果执行系统处理。
        _spec.steps.Add(EffectStepSpec.Buff(new BuffEffectStepSpec
        {
            buffId = buffId,
            duration = duration,
            attrTypeId = attrTypeId,
            modifyType = modifyType,
            value = value,
            refreshBehavior = refreshBehavior
        }));
        return this;
    }

    /// <summary>
    /// 添加作者友好数值写法的 Buff 应用步骤，属性修改通过 Buff/属性系统落地。
    /// </summary>
    public EffectChainBuilder Buff(string buffId, AbilityValue duration, int attrTypeId, ModifyType modifyType,
        AbilityValue value, BuffRefreshBehavior refreshBehavior = BuffRefreshBehavior.RefreshDuration)
    {
        return Buff(buffId, duration.EffectValue, attrTypeId, modifyType, value.EffectValue, refreshBehavior);
    }

    public EffectChainBuilder Area(TargetFilter filter, int maxTargets = 0, EffectValueSpec radius = default,
        string? customFilterId = null, float centerX = 0f, float centerY = 0f)
    {
        // area step 负责改变后续目标集，filter 优先用枚举/customFilterId 这类可配置数据。
        _spec.steps.Add(EffectStepSpec.AreaSearch(new AreaSearchEffectStepSpec
        {
            centerX = centerX,
            centerY = centerY,
            radius = radius,
            maxTargets = maxTargets,
            filter = filter,
            customFilterId = customFilterId
        }));
        return this;
    }

    public EffectChainBuilder Line(TargetFilter filter, EffectValueSpec range = default, float fallbackRange = 0f,
        EffectValueSpec width = default, float fallbackWidth = 0f, int maxTargets = 0,
        string? customFilterId = null, GroundAreaTag reactionTag = GroundAreaTag.None)
    {
        // line step 负责改变后续目标集，也可标记火焰等接触型地面反应。
        _spec.steps.Add(EffectStepSpec.LineSearch(new LineSearchEffectStepSpec
        {
            range = range,
            fallbackRange = fallbackRange,
            width = width,
            fallbackWidth = fallbackWidth,
            maxTargets = maxTargets,
            filter = filter,
            customFilterId = customFilterId,
            reactionTag = reactionTag
        }));
        return this;
    }

    public EffectChainBuilder GroundArea(GroundAreaTag tags, EffectValueSpec radius = default,
        float fallbackRadius = 0f, EffectValueSpec duration = default, float fallbackDuration = 0f,
        GroundAreaBuffData buff = default, GroundAreaPeriodicDamageData periodicDamage = default,
        GroundAreaReactionData reaction = default)
    {
        // ground area step 只创建 ECS 区域语义，视觉和原生副作用不在这里处理。
        _spec.steps.Add(EffectStepSpec.GroundAreaCreate(new GroundAreaCreateEffectStepSpec
        {
            tags = tags,
            radius = radius,
            fallbackRadius = fallbackRadius,
            duration = duration,
            fallbackDuration = fallbackDuration,
            buff = buff,
            periodicDamage = periodicDamage,
            reaction = reaction
        }));
        return this;
    }

    /// <summary>
    /// 添加弹道步骤；可选到达链会在弹道到达后作为独立 effect 执行。
    /// </summary>
    public EffectChainBuilder Projectile(string model, EffectValueSpec speed,
        ProjectileTrajectoryType trajectoryType = ProjectileTrajectoryType.Tracking,
        EffectValueSpec arrivalThreshold = default, EffectValueSpec maxDistance = default,
        EffectValueSpec hitRadius = default, TargetFilter hitFilter = TargetFilter.None,
        bool canHitSameTarget = false, Entity effectEntity = default,
        Func<EffectChainBuilder, EffectChainBuilder>? onArrive = null)
    {
        var arriveEffect = onArrive?.Invoke(Chain()).Build();
        // projectile step 只描述投射物规格；运动、命中和特效同步由后续系统推进。
        _spec.steps.Add(EffectStepSpec.Projectile(new ProjectileEffectStepSpec
        {
            trajectoryType = trajectoryType,
            model = model,
            speed = speed,
            effectEntity = effectEntity,
            arrivalThreshold = arrivalThreshold,
            maxDistance = maxDistance,
            hitRadius = hitRadius,
            hitFilter = hitFilter,
            canHitSameTarget = canHitSameTarget,
            arriveEffect = arriveEffect
        }));
        return this;
    }

    /// <summary>
    /// 添加作者友好数值写法的弹道步骤，可选到达链会在弹道到达后独立执行。
    /// </summary>
    public EffectChainBuilder Projectile(string model, AbilityValue speed,
        ProjectileTrajectoryType trajectoryType = ProjectileTrajectoryType.Tracking,
        AbilityValue arrivalThreshold = default, AbilityValue maxDistance = default,
        AbilityValue hitRadius = default, TargetFilter hitFilter = TargetFilter.None,
        bool canHitSameTarget = false, Entity effectEntity = default,
        Func<EffectChainBuilder, EffectChainBuilder>? onArrive = null)
    {
        return Projectile(model, speed.EffectValue, trajectoryType, arrivalThreshold.EffectValue,
            maxDistance.EffectValue, hitRadius.EffectValue, hitFilter, canHitSameTarget, effectEntity, onArrive);
    }

    /// <summary>
    /// 添加视觉特效步骤，只描述创建或附着意图，具体原生表现由 Native 系统同步。
    /// </summary>
    public EffectChainBuilder Effect(EffectVisualKind kind, string model, string? key = null,
        EffectAttachType attachPoint = EffectAttachType.Origin, EffectValueSpec duration = default,
        float fallbackDuration = -1f, bool hasPoint = false, float x = 0f, float y = 0f, float z = 0f)
    {
        _spec.steps.Add(EffectStepSpec.EffectVisual(new EffectVisualStepSpec
        {
            kind = kind,
            model = model,
            key = key,
            attachPoint = attachPoint,
            duration = duration,
            fallbackDuration = fallbackDuration,
            hasPoint = hasPoint,
            x = x,
            y = y,
            z = z
        }));
        return this;
    }

    /// <summary>
    /// 添加按 key 清理长期视觉特效的步骤，作用域由运行时 owner 限定。
    /// </summary>
    public EffectChainBuilder RemoveEffectByKey(string key)
    {
        _spec.steps.Add(EffectStepSpec.EffectVisual(new EffectVisualStepSpec
        {
            kind = EffectVisualKind.RemoveByKey,
            key = key
        }));
        return this;
    }

    /// <summary>
    /// 为最近追加的弹道步骤配置到达后的嵌套效果链。
    /// </summary>
    public EffectChainBuilder OnProjectileArrive(Func<EffectChainBuilder, EffectChainBuilder> configure)
    {
        if (_spec.steps.Count == 0 || _spec.steps[^1].kind != EffectStepKind.Projectile)
            throw new InvalidOperationException("OnProjectileArrive must follow Projectile.");

        var last = _spec.steps[^1];
        last.projectile.arriveEffect = configure(Chain()).Build();
        _spec.steps[^1] = last;
        return this;
    }
}

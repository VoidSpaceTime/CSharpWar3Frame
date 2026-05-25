using Friflo.Engine.ECS;
using War3Frame.Components;
using War3Frame.Src.Components;

namespace War3Frame.Helpers;

public sealed class EffectSpecBuilder
{
    // Builder 面向配置/编辑器友好的效果链描述；普通技能尽量使用 statId + formulaId + 参数表。
    private readonly EffectSpec _spec = new();

    private EffectSpecBuilder()
    {
    }

    public static EffectSpecBuilder Chain()
    {
        // 从一个空链开始按顺序追加 step，执行系统会按 steps 顺序解释效果。
        return new EffectSpecBuilder();
    }

    public EffectSpec Build()
    {
        // 返回同一个 spec 实例，调用方构建完成后应视为只读配置使用。
        return _spec;
    }

    public EffectSpecBuilder Damage(EffectValueSpec value, DamageType damageType = DamageType.Magical,
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

    public EffectSpecBuilder Damage(string formulaId, int statId, DamageType damageType = DamageType.Magical,
        DamageSrc damageSrc = DamageSrc.Skill, Dictionary<string, float>? parameters = null)
    {
        // 推荐入口：用公式 ID + statId + 参数表表达数值，方便编辑器序列化和调参。
        return Damage(EffectValueSpec.Stat(statId, formulaId, parameters), damageType, damageSrc);
    }

    public EffectSpecBuilder Heal(EffectValueSpec value, int valueTypeId = 0, float amount = 0f)
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

    public EffectSpecBuilder Heal(string formulaId, int statId, Dictionary<string, float>? parameters = null)
    {
        // 推荐入口：治疗数值同样通过公式注册表解释，避免普通技能直接写 delegate。
        return Heal(EffectValueSpec.Stat(statId, formulaId, parameters), statId);
    }

    public EffectSpecBuilder Buff(string buffId, EffectValueSpec duration, int attrTypeId, ModifyType modifyType,
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

    public EffectSpecBuilder Area(TargetFilter filter, int maxTargets = 0, EffectValueSpec radius = default,
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

    public EffectSpecBuilder Line(TargetFilter filter, EffectValueSpec range = default, float fallbackRange = 0f,
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

    public EffectSpecBuilder GroundArea(GroundAreaTag tags, EffectValueSpec radius = default,
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

    public EffectSpecBuilder Projectile(string model, EffectValueSpec speed,
        ProjectileTrajectoryType trajectoryType = ProjectileTrajectoryType.Tracking,
        EffectValueSpec arrivalThreshold = default, EffectValueSpec maxDistance = default,
        EffectValueSpec hitRadius = default, TargetFilter hitFilter = TargetFilter.None,
        bool canHitSameTarget = false, Entity effectEntity = default)
    {
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
            canHitSameTarget = canHitSameTarget
        }));
        return this;
    }
}

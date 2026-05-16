using Friflo.Engine.ECS;
using War3Frame;
using War3Frame.Components;
using War3Frame.Helpers;
using War3Frame.Src.Components;
using War3Frame.TemplateInit;

namespace War3Frame.Scripts.Template;

/// <summary>
/// 示例技能模板：火焰冲击。
/// </summary>
[AbilityTemplate("fire_blast")]
public class FireBlastTemplate : IAbilityTemplate
{
    public void Configure(Entity ability, int level)
    {
        ability.AddComponent(new AbilityBase
        {
            templateName = "fire_blast",
            level = level,
            Name = "火焰冲击",
            Description = "对目标区域造成伤害。",
            state = AbilityState.Ready,
            targetType = AbilityTargetType.Point
        });

        AbilityHelper.SetBaseValue(ability, AbilityHelper.ManaCost, 80);
        AbilityHelper.SetBaseValue(ability, AbilityHelper.CooldownDuration, 6f);
        AbilityHelper.SetBaseValue(ability, AbilityHelper.Range, 700f);
        AbilityHelper.SetBaseValue(ability, AbilityHelper.Radius, 180f);
        AbilityHelper.SetBaseValue(ability, AbilityHelper.DamageAmount, 100f);
        AbilityHelper.SetEffectSpec(ability, EffectSpecBuilder
            .Chain()
            .Area(TargetFilter.EnemyAlive)
            .Damage(EffectValueSpec.Stat(AbilityHelper.DamageAmount), DamageType.Magical, DamageSrc.Skill)
            .Build());
    }
}

/// <summary>
/// 示例技能模板：熔岩球。
/// 当前按现有能力效果结构，先表达“飞行命中后范围伤害”的主阶段。
/// 后续如果补了周期伤害/到达后生成子效果语义，再扩展成熔岩地面持续灼烧。
/// </summary>
[AbilityTemplate("lava_ball")]
public class LavaBallTemplate : IAbilityTemplate
{
    public void Configure(Entity ability, int level)
    {
        ability.AddComponent(new AbilityBase
        {
            templateName = "lava_ball",
            level = level,
            Name = "熔岩球",
            Description = "向目标区域发射熔岩球，命中后造成范围伤害。",
            state = AbilityState.Ready,
            targetType = AbilityTargetType.Point
        });

        // 技能参数层
        AbilityHelper.SetBaseValue(ability, AbilityHelper.ManaCost, 100);
        AbilityHelper.SetBaseValue(ability, AbilityHelper.CooldownDuration, 10f);
        AbilityHelper.SetBaseValue(ability, AbilityHelper.Range, 800f);
        AbilityHelper.SetBaseValue(ability, AbilityHelper.Radius, 220f);

        // 熔岩球飞行阶段
        ability.AddComponent(new ProjectileData
        {
            model = "Abilities\\Weapons\\FireBallMissile\\FireBallMissile.mdl",
            speed = 650f, // 迁移期兼容：当前运行时仍直接读取该字段
            arrivalThreshold = 35f // 迁移期兼容：当前运行时仍直接读取该字段
        });

        // 落点范围搜索
        ability.AddComponent(new AreaSearchData
        {
            filter = TargetFilter.EnemyAlive,
            maxTargets = 0
        });

        // 落点范围伤害
        ability.AddComponent(new DamageEffectData
        {
            damageFunc = (caster, entity, target, damage) => 100f,
            damageType = DamageType.Magical,
            damageSrc = DamageSrc.Skill
        });
    }
}

/// <summary>
/// 示例技能模板：坚韧天赋。
/// 展示“挂载技能也可以通过统一属性贡献层为单位提供长期加成”的最新用法。
/// </summary>
[AbilityTemplate("talent_vitality")]
public class TalentVitalityTemplate : IAbilityTemplate
{
    public void Configure(Entity ability, int level)
    {
        ability.AddComponent(new AbilityBase
        {
            templateName = "talent_vitality",
            level = level,
            Name = "坚韧天赋",
            Description = "挂载后提高单位生命上限。",
            state = AbilityState.Ready,
            targetType = AbilityTargetType.None
        });

        ability.AddComponent(new AttributeContributionEntry
        {
            attrTypeId = AttributeHelper.Health,
            modifyType = ModifyType.Flat,
            value = 120 + 80 * level,
            priority = 0
        });
    }
}

/// <summary>
/// 示例技能模板：治疗波。
/// 展示治疗效果与伤害效果一样，也走委托公式主路径。
/// </summary>
[AbilityTemplate("healing_wave")]
public class HealingWaveTemplate : IAbilityTemplate
{
    public void Configure(Entity ability, int level)
    {
        ability.AddComponent(new AbilityBase
        {
            templateName = "healing_wave",
            level = level,
            Name = "治疗波",
            Description = "治疗目标单位。",
            state = AbilityState.Ready,
            targetType = AbilityTargetType.Unit
        });

        AbilityHelper.SetBaseValue(ability, AbilityHelper.ManaCost, 60);
        AbilityHelper.SetBaseValue(ability, AbilityHelper.CooldownDuration, 6f);
        AbilityHelper.SetBaseValue(ability, AbilityHelper.Range, 500f);
        AbilityHelper.SetBaseValue(ability, AbilityHelper.HealAmount, 100 + 50 * level);
        AbilityHelper.SetEffectSpec(ability, EffectSpecBuilder
            .Chain()
            .Heal(EffectValueSpec.Stat(AbilityHelper.HealAmount), AbilityHelper.HealAmount)
            .Build());
    }
}

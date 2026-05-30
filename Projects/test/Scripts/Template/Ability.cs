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
        AbilitySpecBuilder
            .Create("fire_blast")
            .Name("火焰冲击")
            .Description("对目标区域造成伤害。")
            .TargetType(AbilityTargetType.Point)
            .BaseValue(AbilityHelper.ManaCost, 80)
            .BaseValue(AbilityHelper.CooldownDuration, 6f)
            .BaseValue(AbilityHelper.Range, 700f)
            .BaseValue(AbilityHelper.Radius, 180f)
            .BaseValue(AbilityHelper.DamageAmount, LevelValue.PerLevel(100f, 25f))
            .Experience(ExperienceCurve.LevelTable(100f, 250f, 500f), maxLevel: 4)
            .OnCast(e => e
                .Area(TargetFilter.EnemyAlive)
                .Damage(AbilityValue.AbilityStat(AbilityHelper.DamageAmount), DamageType.Magical, DamageSrc.Skill))
            .BuildTo(ability, level);
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

/// <summary>
/// 示例技能模板：冰霜新星。
/// 展示一个范围技能同时产生伤害与控制类 Buff 请求。
/// </summary>
[AbilityTemplate("frost_nova")]
public class FrostNovaTemplate : IAbilityTemplate
{
    public void Configure(Entity ability, int level)
    {
        ability.AddComponent(new AbilityBase
        {
            templateName = "frost_nova",
            level = level,
            Name = "冰霜新星",
            Description = "冻结目标区域，对敌人造成伤害并施加定身效果。",
            state = AbilityState.Ready,
            targetType = AbilityTargetType.Point
        });

        AbilityHelper.SetBaseValue(ability, AbilityHelper.ManaCost, 90);
        AbilityHelper.SetBaseValue(ability, AbilityHelper.CooldownDuration, 8f);
        AbilityHelper.SetBaseValue(ability, AbilityHelper.Range, 650f);
        AbilityHelper.SetBaseValue(ability, AbilityHelper.Radius, 200f);
        AbilityHelper.SetBaseValue(ability, AbilityHelper.DamageAmount, 70 + 25 * level);

        AbilityHelper.SetEffectSpec(ability, EffectSpecBuilder
            .Chain()
            .Area(TargetFilter.EnemyAlive, radius: EffectValueSpec.Stat(AbilityHelper.Radius))
            .Damage(EffectValueSpec.Stat(AbilityHelper.DamageAmount), DamageType.Magical, DamageSrc.Skill)
            .Buff(
                "frost_nova_root",
                EffectValueSpec.Constant(1.2f + 0.2f * level),
                AttributeHelper.Root,
                ModifyType.Flat,
                EffectValueSpec.Constant(1f),
                BuffRefreshBehavior.RefreshDuration)
            .Build());
    }
}

/// <summary>
/// 示例技能模板：奥术飞弹。
/// 展示单体追踪弹道命中后结算伤害。
/// </summary>
[AbilityTemplate("arcane_missile")]
public class ArcaneMissileTemplate : IAbilityTemplate
{
    public void Configure(Entity ability, int level)
    {
        ability.AddComponent(new AbilityBase
        {
            templateName = "arcane_missile",
            level = level,
            Name = "奥术飞弹",
            Description = "发射追踪飞弹，命中目标后造成魔法伤害。",
            state = AbilityState.Ready,
            targetType = AbilityTargetType.Unit
        });

        AbilityHelper.SetBaseValue(ability, AbilityHelper.ManaCost, 45);
        AbilityHelper.SetBaseValue(ability, AbilityHelper.CooldownDuration, 3.5f);
        AbilityHelper.SetBaseValue(ability, AbilityHelper.Range, 750f);
        AbilityHelper.SetBaseValue(ability, AbilityHelper.DamageAmount, 55 + 35 * level);
        AbilityHelper.SetBaseValue(ability, AbilityHelper.ProjectileSpeed, 900f);
        AbilityHelper.SetBaseValue(ability, AbilityHelper.ArrivalThreshold, 30f);

        AbilityHelper.SetEffectSpec(ability, EffectSpecBuilder
            .Chain()
            .Projectile(
                "Abilities\\Weapons\\AvengerMissile\\AvengerMissile.mdl",
                EffectValueSpec.Stat(AbilityHelper.ProjectileSpeed),
                arrivalThreshold: EffectValueSpec.Stat(AbilityHelper.ArrivalThreshold),
                hitFilter: TargetFilter.EnemyAlive)
            .Damage(EffectValueSpec.Stat(AbilityHelper.DamageAmount), DamageType.Magical, DamageSrc.Skill)
            .Build());
    }
}

/// <summary>
/// 示例技能模板：战吼。
/// 展示以目标点为中心的范围增益 Buff，当前目标筛选依赖 TargetFilterRegistry 的阵营实现。
/// </summary>
[AbilityTemplate("battle_shout")]
public class BattleShoutTemplate : IAbilityTemplate
{
    public void Configure(Entity ability, int level)
    {
        ability.AddComponent(new AbilityBase
        {
            templateName = "battle_shout",
            level = level,
            Name = "战吼",
            Description = "鼓舞目标区域内单位，临时提高攻击力。",
            state = AbilityState.Ready,
            targetType = AbilityTargetType.Point
        });

        AbilityHelper.SetBaseValue(ability, AbilityHelper.ManaCost, 50);
        AbilityHelper.SetBaseValue(ability, AbilityHelper.CooldownDuration, 12f);
        AbilityHelper.SetBaseValue(ability, AbilityHelper.Range, 450f);
        AbilityHelper.SetBaseValue(ability, AbilityHelper.Radius, 350f);

        AbilityHelper.SetEffectSpec(ability, EffectSpecBuilder
            .Chain()
            .Area(TargetFilter.AllAliveIncludeSelf, radius: EffectValueSpec.Stat(AbilityHelper.Radius))
            .Buff(
                "battle_shout_damage",
                EffectValueSpec.Constant(8f),
                AttributeHelper.Damage,
                ModifyType.Flat,
                EffectValueSpec.Constant(15f + 5f * level),
                BuffRefreshBehavior.RefreshDuration)
            .Build());
    }
}

/// <summary>
/// 示例技能模板：战术专注。
/// 展示非槽位/被动技能通过属性贡献层提高法力恢复。
/// </summary>
[AbilityTemplate("talent_mana_focus")]
public class TalentManaFocusTemplate : IAbilityTemplate
{
    public void Configure(Entity ability, int level)
    {
        ability.AddComponent(new AbilityBase
        {
            templateName = "talent_mana_focus",
            level = level,
            Name = "战术专注",
            Description = "挂载后提高单位法力恢复。",
            state = AbilityState.Ready,
            targetType = AbilityTargetType.None
        });

        ability.AddComponent(new AttributeContributionEntry
        {
            attrTypeId = AttributeHelper.ManaRegen,
            modifyType = ModifyType.Flat,
            value = 0.5f + 0.25f * level,
            priority = 0
        });
    }
}

/// <summary>
/// 示例技能模板：流星术。
/// 展示延迟弹道到达后再展开区域伤害。
/// </summary>
[AbilityTemplate("meteor_strike")]
public class MeteorStrikeTemplate : IAbilityTemplate
{
    public void Configure(Entity ability, int level)
    {
        ability.AddComponent(new AbilityBase
        {
            templateName = "meteor_strike",
            level = level,
            Name = "流星术",
            Description = "召唤流星轰击目标区域，命中后造成大范围伤害。",
            state = AbilityState.Ready,
            targetType = AbilityTargetType.Point
        });

        AbilityHelper.SetBaseValue(ability, AbilityHelper.ManaCost, 130);
        AbilityHelper.SetBaseValue(ability, AbilityHelper.CooldownDuration, 15f);
        AbilityHelper.SetBaseValue(ability, AbilityHelper.Range, 900f);
        AbilityHelper.SetBaseValue(ability, AbilityHelper.Radius, 260f);
        AbilityHelper.SetBaseValue(ability, AbilityHelper.DamageAmount, 160 + 55 * level);
        AbilityHelper.SetBaseValue(ability, AbilityHelper.ProjectileSpeed, 500f);
        AbilityHelper.SetBaseValue(ability, AbilityHelper.ArrivalThreshold, 45f);

        AbilityHelper.SetEffectSpec(ability, EffectSpecBuilder
            .Chain()
            .Projectile(
                "Abilities\\Weapons\\InfernalMeteor\\InfernalMeteor.mdl",
                EffectValueSpec.Stat(AbilityHelper.ProjectileSpeed),
                ProjectileTrajectoryType.Parabolic,
                EffectValueSpec.Stat(AbilityHelper.ArrivalThreshold),
                hitFilter: TargetFilter.EnemyAlive)
            .Area(TargetFilter.EnemyAlive, radius: EffectValueSpec.Stat(AbilityHelper.Radius))
            .Damage(EffectValueSpec.Stat(AbilityHelper.DamageAmount), DamageType.Magical, DamageSrc.Skill)
            .Build());
    }
}

/// <summary>
/// 示例技能模板：凝固汽油。
/// 展示技能只配置地面油污区域，减速和点燃反应由 ECS 系统执行。
/// </summary>
[AbilityTemplate("napalm_oil")]
public class NapalmOilTemplate : IAbilityTemplate
{
    public void Configure(Entity ability, int level)
    {
        ability.AddComponent(new AbilityBase
        {
            templateName = "napalm_oil",
            level = level,
            Name = "凝固汽油",
            Description = "在目标地点生成油污区域，范围内敌人被减速，遇火后转为燃烧地面。",
            state = AbilityState.Ready,
            targetType = AbilityTargetType.Point
        });

        AbilityHelper.SetBaseValue(ability, AbilityHelper.ManaCost, 70);
        AbilityHelper.SetBaseValue(ability, AbilityHelper.CooldownDuration, 9f);
        AbilityHelper.SetBaseValue(ability, AbilityHelper.Range, 650f);
        AbilityHelper.SetBaseValue(ability, AbilityHelper.Radius, 220f);
        AbilityHelper.SetBaseValue(ability, AbilityHelper.DamageAmount, 10f);

        var burningDamage = new GroundAreaPeriodicDamageData
        {
            enabled = true,
            damageValue = EffectValueSpec.Stat(AbilityHelper.DamageAmount),
            fallbackDamage = 10f,
            tickInterval = 1f,
            damageType = DamageType.Magical,
            damageSrc = DamageSrc.Skill,
            filter = TargetFilter.EnemyAlive
        };

        AbilityHelper.SetEffectSpec(ability, EffectSpecBuilder
            .Chain()
            .GroundArea(
                GroundAreaTag.Oil,
                EffectValueSpec.Stat(AbilityHelper.Radius),
                duration: EffectValueSpec.Constant(10f),
                buff: new GroundAreaBuffData
                {
                    enabled = true,
                    buffId = "napalm_oil_slow",
                    attrTypeId = AttributeHelper.MoveSpeed,
                    modifyType = ModifyType.Flat,
                    value = EffectValueSpec.Constant(-20f),
                    fallbackValue = -20f
                },
                reaction: new GroundAreaReactionData
                {
                    enabled = true,
                    triggerTag = GroundAreaTag.Fire,
                    resultTags = GroundAreaTag.Burning,
                    resultDuration = EffectValueSpec.Constant(5f),
                    fallbackDuration = 5f,
                    resultPeriodicDamage = burningDamage
                })
            .Build());
    }
}

/// <summary>
/// 示例技能模板：喷火。
/// 展示线形搜索造成伤害，并用 Fire 标签显式触发油污反应。
/// </summary>
[AbilityTemplate("flamethrower")]
public class FlamethrowerTemplate : IAbilityTemplate
{
    public void Configure(Entity ability, int level)
    {
        ability.AddComponent(new AbilityBase
        {
            templateName = "flamethrower",
            level = level,
            Name = "喷火",
            Description = "向目标方向喷出火焰，对线形范围内敌人造成伤害，并点燃油污。",
            state = AbilityState.Ready,
            targetType = AbilityTargetType.Point
        });

        AbilityHelper.SetBaseValue(ability, AbilityHelper.ManaCost, 85);
        AbilityHelper.SetBaseValue(ability, AbilityHelper.CooldownDuration, 7f);
        AbilityHelper.SetBaseValue(ability, AbilityHelper.Range, 600f);
        AbilityHelper.SetBaseValue(ability, AbilityHelper.DamageAmount, 60f + 25f * level);

        AbilityHelper.SetEffectSpec(ability, EffectSpecBuilder
            .Chain()
            .Line(
                TargetFilter.EnemyAlive,
                EffectValueSpec.Stat(AbilityHelper.Range),
                width: EffectValueSpec.Constant(140f),
                fallbackWidth: 140f,
                reactionTag: GroundAreaTag.Fire)
            .Damage(EffectValueSpec.Stat(AbilityHelper.DamageAmount), DamageType.Magical, DamageSrc.Skill)
            .Build());
    }
}

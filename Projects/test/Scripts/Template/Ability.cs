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
            .OnEffect(e => e
                .Area(TargetFilter.EnemyAlive, radius: AbilityValue.AbilityStat(AbilityHelper.Radius))
                .Damage(AbilityValue.AbilityStat(AbilityHelper.DamageAmount), DamageType.Magical, DamageSrc.Skill))
            .BuildTo(ability, level);
    }
}

/// <summary>
/// 示例技能模板：熔岩球。
/// 展示弹道命中后展开范围搜索与伤害，Projectile 作为技能效果链的一步表达。
/// </summary>
[AbilityTemplate("lava_ball")]
public class LavaBallTemplate : IAbilityTemplate
{
    public void Configure(Entity ability, int level)
    {
        AbilitySpecBuilder
            .Create("lava_ball")
            .Name("熔岩球")
            .Description("向目标区域发射熔岩球，命中后造成范围伤害。")
            .TargetType(AbilityTargetType.Point)
            .BaseValue(AbilityHelper.ManaCost, 100)
            .BaseValue(AbilityHelper.CooldownDuration, 10f)
            .BaseValue(AbilityHelper.Range, 800f)
            .BaseValue(AbilityHelper.Radius, 220f)
            .BaseValue(AbilityHelper.DamageAmount, LevelValue.PerLevel(100f, 35f))
            .BaseValue(AbilityHelper.ProjectileSpeed, 650f)
            .BaseValue(AbilityHelper.ArrivalThreshold, 35f)
            .OnEffect(e => e
                .Projectile(
                    "Abilities\\Weapons\\FireBallMissile\\FireBallMissile.mdl",
                    AbilityValue.AbilityStat(AbilityHelper.ProjectileSpeed),
                    arrivalThreshold: AbilityValue.AbilityStat(AbilityHelper.ArrivalThreshold),
                    hitFilter: TargetFilter.EnemyAlive)
                .Area(TargetFilter.EnemyAlive, radius: AbilityValue.AbilityStat(AbilityHelper.Radius))
                .Damage(AbilityValue.AbilityStat(AbilityHelper.DamageAmount), DamageType.Magical, DamageSrc.Skill))
            .BuildTo(ability, level);
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
/// 展示治疗效果与伤害效果一样，也走统一技能效果链。
/// </summary>
[AbilityTemplate("healing_wave")]
public class HealingWaveTemplate : IAbilityTemplate
{
    public void Configure(Entity ability, int level)
    {
        AbilitySpecBuilder
            .Create("healing_wave")
            .Name("治疗波")
            .Description("治疗目标单位。")
            .TargetType(AbilityTargetType.Unit)
            .BaseValue(AbilityHelper.ManaCost, 60)
            .BaseValue(AbilityHelper.CooldownDuration, 6f)
            .BaseValue(AbilityHelper.Range, 500f)
            .BaseValue(AbilityHelper.HealAmount, LevelValue.PerLevel(100f, 50f))
            .OnEffect(e => e.Heal(AbilityValue.AbilityStat(AbilityHelper.HealAmount), AbilityHelper.HealAmount))
            .BuildTo(ability, level);
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
        AbilitySpecBuilder
            .Create("frost_nova")
            .Name("冰霜新星")
            .Description("冻结目标区域，对敌人造成伤害并施加定身效果。")
            .TargetType(AbilityTargetType.Point)
            .BaseValue(AbilityHelper.ManaCost, 90)
            .BaseValue(AbilityHelper.CooldownDuration, 8f)
            .BaseValue(AbilityHelper.Range, 650f)
            .BaseValue(AbilityHelper.Radius, 200f)
            .BaseValue(AbilityHelper.DamageAmount, LevelValue.PerLevel(70f, 25f))
            .OnEffect(e => e
                .Area(TargetFilter.EnemyAlive, radius: AbilityValue.AbilityStat(AbilityHelper.Radius))
                .Damage(AbilityValue.AbilityStat(AbilityHelper.DamageAmount), DamageType.Magical, DamageSrc.Skill)
                .Buff(
                    "frost_nova_root",
                    AbilityValue.Constant(1.2f + 0.2f * level),
                    AttributeHelper.Root,
                    ModifyType.Flat,
                    AbilityValue.Constant(1f),
                    BuffRefreshBehavior.RefreshDuration))
            .BuildTo(ability, level);
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
        AbilitySpecBuilder
            .Create("arcane_missile")
            .Name("奥术飞弹")
            .Description("发射追踪飞弹，命中目标后造成魔法伤害。")
            .TargetType(AbilityTargetType.Unit)
            .BaseValue(AbilityHelper.ManaCost, 45)
            .BaseValue(AbilityHelper.CooldownDuration, 3.5f)
            .BaseValue(AbilityHelper.Range, 750f)
            .BaseValue(AbilityHelper.DamageAmount, LevelValue.PerLevel(55f, 35f))
            .BaseValue(AbilityHelper.ProjectileSpeed, 900f)
            .BaseValue(AbilityHelper.ArrivalThreshold, 30f)
            .OnEffect(e => e
                .Projectile(
                    "Abilities\\Weapons\\AvengerMissile\\AvengerMissile.mdl",
                    AbilityValue.AbilityStat(AbilityHelper.ProjectileSpeed),
                    arrivalThreshold: AbilityValue.AbilityStat(AbilityHelper.ArrivalThreshold),
                    hitFilter: TargetFilter.EnemyAlive)
                .Damage(AbilityValue.AbilityStat(AbilityHelper.DamageAmount), DamageType.Magical, DamageSrc.Skill))
            .BuildTo(ability, level);
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
        AbilitySpecBuilder
            .Create("battle_shout")
            .Name("战吼")
            .Description("鼓舞目标区域内单位，临时提高攻击力。")
            .TargetType(AbilityTargetType.Point)
            .BaseValue(AbilityHelper.ManaCost, 50)
            .BaseValue(AbilityHelper.CooldownDuration, 12f)
            .BaseValue(AbilityHelper.Range, 450f)
            .BaseValue(AbilityHelper.Radius, 350f)
            .OnEffect(e => e
                .Area(TargetFilter.AllAliveIncludeSelf, radius: AbilityValue.AbilityStat(AbilityHelper.Radius))
                .Buff(
                    "battle_shout_damage",
                    AbilityValue.Constant(8f),
                    AttributeHelper.Damage,
                    ModifyType.Flat,
                    AbilityValue.Constant(15f + 5f * level),
                    BuffRefreshBehavior.RefreshDuration))
            .BuildTo(ability, level);
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
        AbilitySpecBuilder
            .Create("meteor_strike")
            .Name("流星术")
            .Description("召唤流星轰击目标区域，命中后造成大范围伤害。")
            .TargetType(AbilityTargetType.Point)
            .BaseValue(AbilityHelper.ManaCost, 130)
            .BaseValue(AbilityHelper.CooldownDuration, 15f)
            .BaseValue(AbilityHelper.Range, 900f)
            .BaseValue(AbilityHelper.Radius, 260f)
            .BaseValue(AbilityHelper.DamageAmount, LevelValue.PerLevel(160f, 55f))
            .BaseValue(AbilityHelper.ProjectileSpeed, 500f)
            .BaseValue(AbilityHelper.ArrivalThreshold, 45f)
            .OnEffect(e => e
                .Projectile(
                    "Abilities\\Weapons\\InfernalMeteor\\InfernalMeteor.mdl",
                    AbilityValue.AbilityStat(AbilityHelper.ProjectileSpeed),
                    ProjectileTrajectoryType.Parabolic,
                    AbilityValue.AbilityStat(AbilityHelper.ArrivalThreshold),
                    hitFilter: TargetFilter.EnemyAlive)
                .Area(TargetFilter.EnemyAlive, radius: AbilityValue.AbilityStat(AbilityHelper.Radius))
                .Damage(AbilityValue.AbilityStat(AbilityHelper.DamageAmount), DamageType.Magical, DamageSrc.Skill))
            .BuildTo(ability, level);
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

        AbilitySpecBuilder
            .Create("napalm_oil")
            .Name("凝固汽油")
            .Description("在目标地点生成油污区域，范围内敌人被减速，遇火后转为燃烧地面。")
            .TargetType(AbilityTargetType.Point)
            .BaseValue(AbilityHelper.ManaCost, 70)
            .BaseValue(AbilityHelper.CooldownDuration, 9f)
            .BaseValue(AbilityHelper.Range, 650f)
            .BaseValue(AbilityHelper.Radius, 220f)
            .BaseValue(AbilityHelper.DamageAmount, 10f)
            .OnEffect(e => e.GroundArea(
                GroundAreaTag.Oil,
                AbilityValue.AbilityStat(AbilityHelper.Radius),
                duration: AbilityValue.Constant(10f),
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
                }))
            .BuildTo(ability, level);
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
        AbilitySpecBuilder
            .Create("flamethrower")
            .Name("喷火")
            .Description("向目标方向喷出火焰，对线形范围内敌人造成伤害，并点燃油污。")
            .TargetType(AbilityTargetType.Point)
            .BaseValue(AbilityHelper.ManaCost, 85)
            .BaseValue(AbilityHelper.CooldownDuration, 7f)
            .BaseValue(AbilityHelper.Range, 600f)
            .BaseValue(AbilityHelper.DamageAmount, LevelValue.PerLevel(60f, 25f))
            .OnEffect(e => e
                .Line(
                    TargetFilter.EnemyAlive,
                    AbilityValue.AbilityStat(AbilityHelper.Range),
                    width: AbilityValue.Constant(140f),
                    fallbackWidth: 140f,
                    reactionTag: GroundAreaTag.Fire)
                .Damage(AbilityValue.AbilityStat(AbilityHelper.DamageAmount), DamageType.Magical, DamageSrc.Skill))
            .BuildTo(ability, level);
    }
}

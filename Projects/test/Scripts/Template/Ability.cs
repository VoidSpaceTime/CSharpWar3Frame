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
        AbilityHelper.SetBaseValue(ability, AbilityHelper.DamageAmount, 60 + 20 * level);

        ability.AddComponent(new AreaSearchData
        {
            filter = TargetFilter.EnemyAlive,
            maxTargets = 0
        });

        ability.AddComponent(new DamageEffectData
        {
            amount = 60 + 20 * level,
            damageType = DamageType.Magical,
            damageSrc = DamageSrc.Skill
        });
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
        AbilityHelper.SetBaseValue(ability, AbilityHelper.DamageAmount, 90 + 30 * level);
        AbilityHelper.SetBaseValue(ability, AbilityHelper.ProjectileSpeed, 650f);
        AbilityHelper.SetBaseValue(ability, AbilityHelper.ArrivalThreshold, 35f);

        // 熔岩球飞行阶段
        ability.AddComponent(new ProjectileData
        {
            model = "Abilities\\Weapons\\FireBallMissile\\FireBallMissile.mdl",
            speed = 650f,               // 迁移期兼容：当前运行时仍直接读取该字段
            arrivalThreshold = 35f      // 迁移期兼容：当前运行时仍直接读取该字段
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
            amount = 90 + 30 * level,   // 迁移期兼容：后续建议交给统一伤害公式/结算层
            damageType = DamageType.Magical,
            damageSrc = DamageSrc.Skill
        });
    }
}

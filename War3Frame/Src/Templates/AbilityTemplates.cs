using Friflo.Engine.ECS;
using War3Frame.Helpers;
using War3Frame.Src.Components;
using War3Frame.TemplateInit;

namespace War3Frame.Templates;

// ============================================================================
// 技能模板示例
// 每个模板只需要实现 IAbilityTemplate.Configure，添加技能需要的效果组件
// Source Generator 会自动注册到 AbilityTemplate 工厂中
// ============================================================================

/// <summary>
/// 箭矢射击 - 发射一根箭矢，对沿途敌方单位造成一次伤害
/// 使用 LinearProjectileData 实现线性弹道
/// </summary>
[AbilityTemplate("arrow_shot")]
public class ArrowShotTemplate : IAbilityTemplate
{
    public void Configure(Entity ability, int level)
    {
        // 技能基础信息（覆盖默认值）
        var abilityBase = ability.GetComponent<AbilityBase>();
        abilityBase.level = level;
        abilityBase.targetType = AbilityTargetType.Point;  // 指向性技能
        ability.AddComponent(abilityBase);

        Helpers.AbilityHelper.SetBaseValue(ability, Helpers.AbilityHelper.Range, 800f);
        Helpers.AbilityHelper.SetBaseValue(ability, Helpers.AbilityHelper.CooldownDuration, 8f);
        Helpers.AbilityHelper.SetBaseValue(ability, Helpers.AbilityHelper.ManaCost, 75f);
        Helpers.AbilityHelper.SetBaseValue(ability, Helpers.AbilityHelper.ProjectileSpeed, 900f);
        Helpers.AbilityHelper.SetBaseValue(ability, Helpers.AbilityHelper.ProjectileDistance, 800f);
        Helpers.AbilityHelper.SetBaseValue(ability, Helpers.AbilityHelper.HitWidth, 80f);
        Helpers.AbilityHelper.SetBaseValue(ability, Helpers.AbilityHelper.DamageAmount, 50 + 25 * level);

        // 线性弹道（朝目标方向飞行，沿途命中敌人）
        ability.AddComponent(new LinearProjectileData
        {
            model = "Abilities\\Weapons\\SearingArrow\\SearingArrowMissile.mdl",
            speed = 900f,
            maxDistance = 800f,
            hitRadius = 80f,
            hitFilter = TargetFilter.EnemyAlive,
            canHitSameTarget = true    // 每个目标只命中一次
        });

        // 伤害效果（对命中的每个目标造成伤害）
        ability.AddComponent(new DamageEffectData
        {
            amount = 50 + 25 * level,
            damageType = DamageType.Magical,
            damageSrc = DamageSrc.Skill
        });
    }
}

/// <summary>
/// 火球术 - 对目标单位发射追踪火球，命中后造成范围伤害
/// 使用 ProjectileData（追踪弹道）+ AreaSearchData（爆炸范围）
/// </summary>
[AbilityTemplate("fireball")]
public class FireballTemplate : IAbilityTemplate
{
    public void Configure(Entity ability, int level)
    {
        var abilityBase = ability.GetComponent<AbilityBase>();
        abilityBase.level = level;
        abilityBase.targetType = AbilityTargetType.Unit;   // 单位目标
        ability.AddComponent(abilityBase);

        Helpers.AbilityHelper.SetBaseValue(ability, Helpers.AbilityHelper.Range, 600f);
        Helpers.AbilityHelper.SetBaseValue(ability, Helpers.AbilityHelper.CooldownDuration, 10f);
        Helpers.AbilityHelper.SetBaseValue(ability, Helpers.AbilityHelper.ManaCost, 120f);
        Helpers.AbilityHelper.SetBaseValue(ability, Helpers.AbilityHelper.ProjectileSpeed, 700f);
        Helpers.AbilityHelper.SetBaseValue(ability, Helpers.AbilityHelper.ArrivalThreshold, 30f);
        Helpers.AbilityHelper.SetBaseValue(ability, Helpers.AbilityHelper.Radius, 200f);
        Helpers.AbilityHelper.SetBaseValue(ability, Helpers.AbilityHelper.MaxTargets, 0f);
        Helpers.AbilityHelper.SetBaseValue(ability, Helpers.AbilityHelper.DamageAmount, 75 + 25 * level);

        // 追踪弹道 → 飞到目标身上
        ability.AddComponent(new ProjectileData
        {
            model = "Abilities\\Weapons\\FireBallMissile\\FireBallMissile.mdl",
            speed = 700f,
            arrivalThreshold = 30f
        });

        // 到达后进行范围搜索
        ability.AddComponent(new AreaSearchData
        {
            radius = 200f,
            filter = TargetFilter.EnemyAlive,
            maxTargets = 0  // 无限制
        });

        // 对搜索到的每个目标造成伤害
        ability.AddComponent(new DamageEffectData
        {
            amount = 75 + 25 * level,
            damageType = DamageType.Magical,
            damageSrc = DamageSrc.Skill
        });
    }
}

/// <summary>
/// 治疗波 - 治疗目标友方单位
/// 最简单的技能模板示例
/// </summary>
[AbilityTemplate("heal")]
public class HealTemplate : IAbilityTemplate
{
    public void Configure(Entity ability, int level)
    {
        var abilityBase = ability.GetComponent<AbilityBase>();
        abilityBase.level = level;
        abilityBase.targetType = AbilityTargetType.Unit;
        ability.AddComponent(abilityBase);

        Helpers.AbilityHelper.SetBaseValue(ability, Helpers.AbilityHelper.Range, 500f);
        Helpers.AbilityHelper.SetBaseValue(ability, Helpers.AbilityHelper.CooldownDuration, 6f);
        Helpers.AbilityHelper.SetBaseValue(ability, Helpers.AbilityHelper.ManaCost, 60f);
        Helpers.AbilityHelper.SetBaseValue(ability, Helpers.AbilityHelper.HealAmount, 100 + 50 * level);

        // 直接治疗，无弹道
        ability.AddComponent(new HealEffectData
        {
            amount = 100 + 50 * level
        });
    }
}

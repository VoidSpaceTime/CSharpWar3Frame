using Friflo.Engine.ECS;
using War3Frame.Components;
using War3Frame.Helpers;
using War3Frame.Src.Components;
using War3Frame.TemplateInit;

namespace War3Frame.Scripts.Template;

/// <summary>
/// 示例物品模板：力量护符。
/// 装备后提高生命，使用后治疗自身。
/// </summary>
[ItemTemplate("amulet_of_vigor")]
public class AmuletOfVigorTemplate : IItemTemplate
{
    public void Configure(Entity item)
    {
        item.AddComponent(new ItemBase
        {
            templateName = "amulet_of_vigor",
            name = "力量护符",
            stackCount = 1,
            maxStack = 1,
            isUsable = true,
            isConsumable = false,
            isInstantiate = true
        });

        item.AddComponent(new AttributeContributionEntry
        {
            attrTypeId = AttributeHelper.Health,
            modifyType = ModifyType.Flat,
            value = 150,
            priority = 0
        });

        item.AddComponent(new HealEffectData
        {
            amount = 120
        });
    }
}

/// <summary>
/// 示例物品模板：火球卷轴。
/// 使用时向目标区域释放一颗火球并造成范围伤害。
/// </summary>
[ItemTemplate("scroll_fireball")]
public class ScrollFireballTemplate : IItemTemplate
{
    public void Configure(Entity item)
    {
        // 物品基础信息
        item.AddComponent(new ItemBase
        {
            templateName = "scroll_fireball",
            name = "火球卷轴",
            stackCount = 1,
            maxStack = 10,
            isUsable = true,
            isConsumable = true,
            isInstantiate = true
        });
        // 这里建议后续单独补一个 ItemUseConfig / ItemUseData
        // 先用最小示例表达“这个物品可向目标点释放效果”
        item.AddComponent(new AbilityBase
        {
            templateName = "scroll_fireball_cast",
            level = 1,
            Name = "火球卷轴-释放",
            Description = "向目标区域释放火球。",
            state = AbilityState.Ready,
            targetType = AbilityTargetType.Point
        });
        // 使用参数：如果你后面决定 item-use 也统一走能力参数层，可以这样挂
        AbilityHelper.SetBaseValue(item, AbilityHelper.Range, 700f);
        AbilityHelper.SetBaseValue(item, AbilityHelper.Radius, 180f);
        AbilityHelper.SetBaseValue(item, AbilityHelper.DamageAmount, 120f);
        // 火球飞到目标点
        item.AddComponent(new ProjectileData
        {
            model = "Abilities\\Weapons\\FireBallMissile\\FireBallMissile.mdl",
            speed = 700f,
            arrivalThreshold = 30f
        });
        // 到点后做范围搜索
        item.AddComponent(new AreaSearchData
        {
            filter = TargetFilter.EnemyAlive,
            maxTargets = 0
        });
        // 范围伤害
        item.AddComponent(new DamageEffectData
        {
            amount = 120f,
            damageType = DamageType.Magical,
            damageSrc = DamageSrc.Skill
        });
    }
}
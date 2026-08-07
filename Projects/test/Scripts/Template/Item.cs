using Friflo.Engine.ECS;
using War3Frame.Components;
using War3Frame.Helpers;
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
        ItemSpecBuilder
            .Create("amulet_of_vigor")
            .Name("力量护符")
            .Stack(max: 1)
            .Usable(consumable: false)
            .Attr(AttributeHelper.Health, ModifyType.Flat, 150)
            .UseAbility("amulet_of_vigor_cast")
            .BuildTo(item);
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
        ItemSpecBuilder
            .Create("scroll_fireball")
            .Name("火球卷轴")
            .Stack(max: 10)
            .Usable(consumable: true)
            .UseAbility("scroll_fireball_cast")
            .BuildTo(item);
    }
}

/// <summary>
/// Inline 即时 Effect 编译与运行验证物品。
/// </summary>
[ItemTemplate("inline_healing_charm")]
public sealed class InlineHealingCharmTemplate : IItemTemplate
{
    /// <summary>
    /// 使用即时 Effect 重载配置物品模板。
    /// </summary>
    public void Configure(Entity item)
    {
        ItemSpecBuilder.Create("inline_healing_charm")
            .Name("内联治疗护符")
            .UseAbility(effect => effect.Heal(AbilityValue.Constant(120f)))
            .BuildTo(item);
    }
}

/// <summary>
/// Inline 完整 Ability 编译与运行验证物品。
/// </summary>
[ItemTemplate("inline_point_scroll")]
public sealed class InlinePointScrollTemplate : IItemTemplate
{
    /// <summary>
    /// 使用完整 AbilitySpec 重载配置物品模板。
    /// </summary>
    public void Configure(Entity item)
    {
        ItemSpecBuilder.Create("inline_point_scroll")
            .Name("内联点目标卷轴")
            .UseAbility(ability => ability
                .TargetType(AbilityTargetType.Point)
                .BaseValue(AbilityHelper.Range, 99999f)
                .BaseValue(AbilityHelper.CooldownDuration, 0f)
                .OnEffect(effect => effect.Heal(AbilityValue.Constant(2f))))
            .BuildTo(item);
    }
}

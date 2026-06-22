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
            .UseEffect(e => e.Heal(AbilityValue.Constant(120f)))
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

using Friflo.Engine.ECS;
using War3Frame.Helpers;
using War3Frame.TemplateInit;

namespace War3Frame.Scripts.Template;

/// <summary>
/// 示例单位模板：步兵。
/// </summary>
[UnitTemplate("footman")]
public partial class FootmanTemplate : IUnitTemplate
{
    public void Configure(Entity e)
    {
        UnitSpecBuilder
            .Create("footman")
            .Name("步兵")
            .Attr(AttributeHelper.Health, 420)
            .Attr(AttributeHelper.Mana, 0)
            .Attr(AttributeHelper.AttackDamage, 24)
            .BuildTo(e);
    }
}

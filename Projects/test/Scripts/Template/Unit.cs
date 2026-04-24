using Friflo.Engine.ECS;
using War3Frame.Components;
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
        e.AddComponent(new UnitBase
        {
            templateName = "footman",
            name = "步兵"
        });

        AttributeHelper.CreateAttr(e, AttributeHelper.Health, 420);
        AttributeHelper.CreateAttr(e, AttributeHelper.Mana, 0);
        AttributeHelper.CreateAttr(e, AttributeHelper.Damage, 24);
    }
}



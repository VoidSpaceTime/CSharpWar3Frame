using Friflo.Engine.ECS;
using War3Frame.Components.Attribute;
using War3Frame.TemplateInit;

namespace War3Frame.Templates;

/// <summary>
/// Example: Footman unit template
/// </summary>
[UnitTemplate("footman")]
public class FootmanTemplate : IUnitTemplate
{
    public void Configure(Entity entity)
    {
        var store = entity.Store;

        // Add health pool
        entity.AddComponent(new HealthAttr { current = 420 });

        // Create attribute entities
        UnitAttrHelper.CreateAttr(entity, AttributeHelper.Health, 420);
        UnitAttrHelper.CreateAttr(entity, AttributeHelper.HealthRegen, 0.25f);
        UnitAttrHelper.CreateAttr(entity, AttributeHelper.Damage, 12);
    }
}

/// <summary>
/// Example: Knight unit template
/// </summary>
[UnitTemplate("knight")]
public class KnightTemplate : IUnitTemplate
{
    public void Configure(Entity entity)
    {
        var store = entity.Store;

        entity.AddComponent(new HealthAttr { current = 800 });

        UnitAttrHelper.CreateAttr(entity, AttributeHelper.Health, 800);
        UnitAttrHelper.CreateAttr(entity, AttributeHelper.HealthRegen, 0.5f);
        UnitAttrHelper.CreateAttr(entity, AttributeHelper.Damage, 28);
    }
}
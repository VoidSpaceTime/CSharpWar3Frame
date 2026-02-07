using Friflo.Engine.ECS;

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

        // Create attribute entities
        // Health with current value
        var health = UnitAttrHelper.CreateAttr(entity, AttributeHelper.Health, 420);

        UnitAttrHelper.CreateAttr(entity, AttributeHelper.HealthRegen, 0.25f);
        UnitAttrHelper.CreateAttr(entity, AttributeHelper.Damage, 12);
        
        // Mana example (if needed)
        // var mana = UnitAttrHelper.CreateAttr(entity, AttributeHelper.Mana, 200);
        // mana.AddComponent(new AttrCurrentValue { value = 200 });
        // UnitAttrHelper.CreateAttr(entity, AttributeHelper.ManaRegen, 1.0f);
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

        var health = UnitAttrHelper.CreateAttr(entity, AttributeHelper.Health, 800);

        UnitAttrHelper.CreateAttr(entity, AttributeHelper.HealthRegen, 0.5f);
        UnitAttrHelper.CreateAttr(entity, AttributeHelper.Damage, 28);
    }
}
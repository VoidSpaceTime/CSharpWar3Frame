using Friflo.Engine.ECS;
using War3Frame.Components;

namespace War3Frame.Helpers;

/// <summary>
/// 单位模板 Builder，负责把单位静态配置写入现有模板实体。
/// </summary>
public sealed class UnitSpecBuilder
{
    private readonly UnitSpec _spec = new();

    private UnitSpecBuilder(string templateName)
    {
        _spec.templateName = templateName;
        _spec.name = templateName;
    }

    public static UnitSpecBuilder Create(string templateName)
    {
        return new UnitSpecBuilder(templateName);
    }

    public UnitSpecBuilder Name(string name)
    {
        _spec.name = name;
        return this;
    }

    public UnitSpecBuilder Attr(int attrTypeId, float baseValue)
    {
        _spec.attributes.Add(new UnitAttributeSpec(attrTypeId, baseValue));
        return this;
    }

    public UnitSpecBuilder ItemSlots(int maxSlots)
    {
        _spec.itemSlotCount = maxSlots;
        return this;
    }

    public UnitSpecBuilder AbilitySlots(int maxSlots)
    {
        _spec.abilitySlotCount = maxSlots;
        return this;
    }

    public UnitSpecBuilder Ability(string abilityTemplateName)
    {
        _spec.abilityTemplateNames.Add(abilityTemplateName);
        return this;
    }

    public UnitSpec Build()
    {
        return _spec;
    }

    public Entity BuildTo(Entity unit)
    {
        Apply(unit, _spec);
        return unit;
    }

    private static void Apply(Entity unit, UnitSpec spec)
    {
        unit.AddComponent(new UnitBase
        {
            templateName = spec.templateName,
            name = spec.name
        });

        foreach (var attribute in spec.attributes)
            AttributeHelper.CreateAttr(unit, attribute.attrTypeId, attribute.baseValue);

        if (spec.itemSlotCount.HasValue)
        {
            unit.AddComponent(new ItemSlotContainer
            {
                maxSlots = spec.itemSlotCount.Value,
                currentCount = 0
            });
        }

        if (spec.abilitySlotCount.HasValue)
            unit.AddComponent(AbilitySlotContainer.WithSlots(spec.abilitySlotCount.Value));

        if (spec.abilityTemplateNames.Count > 0)
        {
            unit.AddComponent(new UnitAbilityTemplateData
            {
                abilityTemplateNames = new List<string>(spec.abilityTemplateNames)
            });
        }

        unit.AddComponent(new UnitSpecData { spec = spec });
    }
}

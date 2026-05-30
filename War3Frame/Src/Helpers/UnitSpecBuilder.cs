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

    /// <summary>
    /// 创建指定模板名的单位模板 Builder。
    /// </summary>
    public static UnitSpecBuilder Create(string templateName)
    {
        return new UnitSpecBuilder(templateName);
    }

    /// <summary>
    /// 设置单位显示名称。
    /// </summary>
    public UnitSpecBuilder Name(string name)
    {
        _spec.name = name;
        return this;
    }

    /// <summary>
    /// 设置单位固定基础属性。
    /// </summary>
    public UnitSpecBuilder Attr(int attrTypeId, float baseValue)
    {
        return Attr(attrTypeId, LevelValue.Fixed(baseValue));
    }

    /// <summary>
    /// 设置单位按等级解析的基础属性。
    /// </summary>
    public UnitSpecBuilder Attr(int attrTypeId, LevelValue baseValue)
    {
        _spec.attributes.Add(new UnitAttributeSpec(attrTypeId, baseValue));
        return this;
    }

    /// <summary>
    /// 设置单位经验曲线和最高等级。
    /// </summary>
    public UnitSpecBuilder Experience(ExperienceCurve curve, int maxLevel = 0, float currentExp = 0f)
    {
        _spec.experience = new ExperienceData
        {
            currentExp = currentExp,
            totalExp = currentExp,
            maxLevel = maxLevel,
            curve = curve
        };
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

        if (!unit.TryGetComponent<UnitLevel>(out var level))
        {
            level = new UnitLevel { level = 1 };
            unit.AddComponent(level);
        }

        foreach (var attribute in spec.attributes)
            AttributeHelper.CreateAttr(unit, attribute.attrTypeId, attribute.baseValue.Resolve(level.level));

        if (spec.experience.HasValue)
            unit.AddComponent(spec.experience.Value);

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

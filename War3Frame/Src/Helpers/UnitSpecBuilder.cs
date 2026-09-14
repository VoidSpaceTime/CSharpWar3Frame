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
    /// <summary>
    /// 设置单位攻击形态（缺省 Melee 近战）。
    /// </summary>
    public UnitSpecBuilder AttackType(AttackType type)
    {
        _spec.attackType = type;
        return this;
    }

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

    /// <summary>
    /// 设置击杀本单位后发给击杀者的经验（按本单位等级解析）。
    /// </summary>
    public UnitSpecBuilder ExpReward(LevelValue reward)
    {
        _spec.expReward = reward;
        return this;
    }

    /// <summary>
    /// 启用单位技能点：每升 1 级发放 perLevel 点，可选 initial 初始点。
    /// perLevel &gt; 0 才会在构建时挂点池。
    /// </summary>
    public UnitSpecBuilder SkillPoints(int perLevel, int initial = 0)
    {
        _spec.skillPointsPerLevel = Math.Max(0, perLevel);
        _spec.initialSkillPoints = Math.Max(0, initial);
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

        // 仅非近战时挂 AttackTypeState（无组件即 Melee，近战单位零组件开销）。
        if (spec.attackType != War3Frame.AttackType.Melee)
        {
            unit.AddComponent(new AttackTypeState { value = spec.attackType });
        }

        if (!unit.TryGetComponent<UnitLevel>(out var level))
        {
            level = new UnitLevel { level = 1 };
            unit.AddComponent(level);
        }

        foreach (var attribute in spec.attributes)
            AttributeHelper.CreateAttr(unit, attribute.attrTypeId, attribute.baseValue.Resolve(level.level));

        if (spec.experience.HasValue)
            unit.AddComponent(spec.experience.Value);

        if (spec.expReward.Resolve(1) > 0f || spec.expReward.kind != LevelValueKind.Fixed)
        {
            unit.AddComponent(new UnitKillRewardData { expReward = spec.expReward });
        }

        if (spec.skillPointsPerLevel > 0)
        {
            unit.AddComponent(new SkillPointPool
            {
                unspent = spec.initialSkillPoints,
                earned = spec.initialSkillPoints,
                perLevel = spec.skillPointsPerLevel
            });
        }

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

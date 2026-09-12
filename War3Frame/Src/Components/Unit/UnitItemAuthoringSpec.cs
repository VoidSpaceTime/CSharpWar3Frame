using Friflo.Engine.ECS;
using War3Frame.Components;

namespace War3Frame;

/// <summary>
/// 单位模板中的基础属性配置。
/// </summary>
public readonly struct UnitAttributeSpec
{
    public readonly int attrTypeId;
    public readonly LevelValue baseValue;

    public UnitAttributeSpec(int attrTypeId, LevelValue baseValue)
    {
        this.attrTypeId = attrTypeId;
        this.baseValue = baseValue;
    }
}

/// <summary>
/// 单位模板规格，只描述单位是什么，不负责创建运行时单位。
/// </summary>
public sealed class UnitSpec
{
public string templateName = string.Empty;
public string name = string.Empty;
public AttackType attackType = AttackType.Melee;
public TargetFilter targetTraits = TargetFilter.Normal;
public readonly List<UnitAttributeSpec> attributes = new();
    public readonly List<string> abilityTemplateNames = new();
    public int? itemSlotCount;
    public int? abilitySlotCount;
    public ExperienceData? experience;

    /// <summary>击杀本单位后发给击杀者的经验，按本单位自身等级解析。</summary>
    public LevelValue expReward = LevelValue.Fixed(0f);

    /// <summary>每升 1 级发放的技能点数；0 表示不启用发点。</summary>
    public int skillPointsPerLevel;

    /// <summary>创建时写入点池的初始技能点（计入 unspent 与 earned）。</summary>
    public int initialSkillPoints;
}

/// <summary>
/// 挂在单位实体上的模板 authoring 数据。
/// </summary>
public struct UnitSpecData : IComponent
{
    public UnitSpec spec;
}

/// <summary>
/// 击杀奖励运行时数据，挂在单位上，死亡时由击杀奖励入口读取。
/// </summary>
public struct UnitKillRewardData : IComponent
{
    public LevelValue expReward;
}

/// <summary>
/// 单位模板中的技能引用配置。
/// </summary>
public struct UnitAbilityTemplateData : IComponent
{
    public List<string> abilityTemplateNames;
}

/// <summary>
/// 物品模板中的属性贡献配置。
/// </summary>
public readonly struct ItemAttributeContributionSpec
{
    public readonly int attrTypeId;
    public readonly ModifyType modifyType;
    public readonly LevelValue value;
    public readonly int priority;

    public ItemAttributeContributionSpec(int attrTypeId, ModifyType modifyType, LevelValue value, int priority)
    {
        this.attrTypeId = attrTypeId;
        this.modifyType = modifyType;
        this.value = value;
        this.priority = priority;
    }
}

/// <summary>
/// 物品模板中的多条属性贡献配置。
/// </summary>
public struct ItemAttributeContributionListData : IComponent
{
    public List<ItemAttributeContributionSpec> attributes;
}

/// <summary>
/// 物品模板规格，只描述物品是什么，不负责装备、卸下或丢弃。
/// </summary>
public sealed class ItemSpec
{
    public string templateName = string.Empty;
    public string name = string.Empty;
    public int stackCount = 1;
    public int maxStack = 1;
    public bool isUsable;
    public bool isConsumable;
    public bool isInstantiate = true;
    public readonly List<ItemAttributeContributionSpec> attributes = new();
    public string? useAbilityTemplateName;
    public ExperienceData? experience;
}

/// <summary>
/// 挂在物品实体上的模板 authoring 数据。
/// </summary>
public struct ItemSpecData : IComponent
{
    public ItemSpec spec;
}

/// <summary>
/// 物品使用时引用的技能模板。
/// </summary>
public struct ItemUseAbilityData : IComponent
{
    public string abilityTemplateName;
}

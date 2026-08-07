using Friflo.Engine.ECS;

namespace War3Frame.TemplateInit;

public interface IAbilityTemplate
{
    public void Configure(Entity entity, int level);
}

public abstract class AbilityTemplateBase : IAbilityTemplate
{
    public abstract void Configure(Entity entity, int level);

    public virtual void OnProjectileStart(
        Entity effectEntity,
        ref EffectSource source,
        ref EffectTargetInfo target,
        ref Position position,
        ref ProjectileRuntimeState runtimeState)
    {
    }

    public virtual ProjectileTravelDecision OnProjectileTravel(
        Entity effectEntity,
        ref EffectSource source,
        ref EffectTargetInfo target,
        ref Position position,
        ref ProjectileRuntimeState runtimeState)
    {
        return ProjectileTravelDecision.Continue;
    }

    public virtual void OnProjectileArrive(
        Entity effectEntity,
        ref EffectSource source,
        ref EffectTargetInfo target,
        ref Position position,
        ref ProjectileRuntimeState runtimeState)
    {
    }
}

/// <summary>
/// 标记一个类为技能模板
/// Source Generator 会自动发现并注册这些类
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class AbilityTemplateAttribute : Attribute
{
    public string Name { get; }
    public AbilityTemplateAttribute(string name) => Name = name;
}

/// <summary>
/// 技能模板注册表和工厂
/// </summary>
public static partial class AbilityTemplate
{
    internal const string InlineItemAbilityPrefix = "__item_inline__:";
    internal const int MaxInlineItemAbilityOwnerLength = 256;

    private static readonly SortedDictionary<string, IAbilityTemplate> _templates = new(StringComparer.Ordinal);
    private static bool _initialized = false;

    /// <summary>
    /// 初始化技能模板系统（在游戏启动时调用）
    /// </summary>
    public static void Initialize()
    {
        if (_initialized) return;

        // 调用 Source Generator 自动生成的注册方法。
        RegisterGenerated();
        _initialized = true;
    }

    /// <summary>
    /// 手动注册技能模板
    /// </summary>
    public static void Register(string name, IAbilityTemplate template)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(template);
        if (name.StartsWith(InlineItemAbilityPrefix, StringComparison.Ordinal))
            throw new ArgumentException($"技能模板名称前缀 '{InlineItemAbilityPrefix}' 由框架保留", nameof(name));

        _templates[name] = template;
    }

    /// <summary>
    /// 根据物品模板逻辑名称生成确定性的内部 Ability template name。
    /// </summary>
    internal static string GetInlineItemAbilityName(string itemTemplateName)
    {
        return InlineItemAbilityPrefix + NormalizeInlineOwner(itemTemplateName);
    }

    /// <summary>
    /// 注册物品私有 AbilitySpec；同名重复注册按 first-wins 幂等返回已有模板。
    /// </summary>
    internal static string RegisterInlineItemAbility(string itemTemplateName, AbilitySpec spec)
    {
        ArgumentNullException.ThrowIfNull(spec);

        var owner = NormalizeInlineOwner(itemTemplateName);
        var templateName = InlineItemAbilityPrefix + owner;
        if (!string.Equals(spec.templateName, templateName, StringComparison.Ordinal))
            throw new InvalidOperationException($"Inline AbilitySpec 必须使用内部模板名称 '{templateName}'");

        if (_templates.TryGetValue(templateName, out var existing))
        {
            if (existing is not InlineItemAbilityTemplate)
                throw new InvalidOperationException($"内部技能模板名称 '{templateName}' 已被其他模板占用");

            return templateName;
        }

        _templates.Add(templateName, new InlineItemAbilityTemplate(owner, spec));
        return templateName;
    }

    /// <summary>
    /// 获取技能模板
    /// </summary>
    public static IAbilityTemplate? Get(string templateName)
    {
        return _templates.GetValueOrDefault(templateName);
    }

    public static bool TryGet(string templateName, out IAbilityTemplate template)
    {
        return _templates.TryGetValue(templateName, out template!);
    }


    /// <summary>
    /// 将模板应用到技能 Entity 上
    /// </summary>
    public static Entity Apply(string templateName, Entity targetEntity, int level = 1)
    {
        if (!_templates.TryGetValue(templateName, out var template))
            throw new ArgumentException($"技能模板 '{templateName}' 未找到");

        template.Configure(targetEntity, level);
        return targetEntity;
    }

    /// <summary>
    /// 检查模板是否存在
    /// </summary>
    public static bool HasTemplate(string name)
    {
        return _templates.ContainsKey(name);
    }

    /// <summary>
    /// 获取所有已注册的模板名称
    /// </summary>
    public static IEnumerable<string> GetAllTemplateNames()
    {
        // 返回快照，避免调用方遍历期间注册新模板导致集合被修改。
        return _templates.Keys.ToArray();
    }

    private static string NormalizeInlineOwner(string itemTemplateName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(itemTemplateName);
        var owner = itemTemplateName.Trim();
        if (owner.Length > MaxInlineItemAbilityOwnerLength)
        {
            throw new ArgumentException(
                $"物品模板 owner 长度不能超过 {MaxInlineItemAbilityOwnerLength}", nameof(itemTemplateName));
        }

        return owner;
    }

    /// <summary>
    /// Source Generator 自动生成的注册方法
    /// </summary>
    static partial void RegisterGenerated();
}

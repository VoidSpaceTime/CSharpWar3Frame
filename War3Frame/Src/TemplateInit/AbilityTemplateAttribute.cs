using Friflo.Engine.ECS;

namespace War3Frame.TemplateInit;

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
    private static readonly SortedDictionary<string, ITemplate> _templates = new();
    private static bool _initialized = false;

    /// <summary>
    /// 初始化技能模板系统（在游戏启动时调用）
    /// </summary>
    public static void Initialize()
    {
        if (_initialized) return;
        _initialized = true;

        // 调用 Source Generator 自动生成的注册方法
        RegisterGenerated();
    }

    /// <summary>
    /// 手动注册技能模板
    /// </summary>
    public static void Register(string name, ITemplate template)
    {
        _templates[name] = template;
    }

    /// <summary>
    /// 获取技能模板
    /// </summary>
    public static ITemplate? Get(string templateName)
    {
        return _templates.GetValueOrDefault(templateName);
    }

    /// <summary>
    /// 将模板应用到技能 Entity 上
    /// </summary>
    public static Entity Apply(string templateName, Entity targetEntity, int level = 1)
    {
        var entity = Game.Store.CreateEntity();
        if (!_templates.TryGetValue(templateName, out var template))
            throw new ArgumentException($"技能模板 '{templateName}' 未找到");

        template.Configure(entity);
        return entity;
    }

    /// <summary>
    /// 检查模板是否存在
    /// </summary>
    public static bool HasTemplate(string name) => _templates.ContainsKey(name);

    /// <summary>
    /// 获取所有已注册的模板名称
    /// </summary>
    public static IEnumerable<string> GetAllTemplateNames() => _templates.Keys;

    /// <summary>
    /// Source Generator 自动生成的注册方法
    /// </summary>
    static partial void RegisterGenerated();
}
using Friflo.Engine.ECS;

namespace War3Frame.TemplateInit;

/// <summary>
/// 标记一个类为物品模板
/// Source Generator 会自动发现并注册这些类
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ItemTemplateAttribute : Attribute
{
    public string Name { get; }
    public ItemTemplateAttribute(string name) => Name = name;
}

/// <summary>
/// 物品模板接口 - 所有物品模板必须实现此接口
/// </summary>
public interface IItemTemplate
{
    /// <summary>
    /// 配置物品 Entity 的组件
    /// 在这里添加物品的属性加成、主动技能、被动效果等
    /// </summary>
    /// <param name="item">物品 Entity</param>
    void Configure(Entity item);
}

/// <summary>
/// 物品模板注册表和工厂
/// </summary>
public static partial class ItemTemplate
{
    private static readonly SortedDictionary<string, IItemTemplate> _templates = new();
    private static bool _initialized = false;

    /// <summary>
    /// 初始化物品模板系统（在游戏启动时调用）
    /// </summary>
    public static void Initialize()
    {
        if (_initialized) return;
        _initialized = true;
        RegisterGenerated();
    }

    /// <summary>
    /// 手动注册物品模板
    /// </summary>
    public static void Register(string name, IItemTemplate template)
    {
        _templates[name] = template;
    }

    /// <summary>
    /// 获取物品模板
    /// </summary>
    public static IItemTemplate? Get(string templateName)
    {
        return _templates.GetValueOrDefault(templateName);
    }

    /// <summary>
    /// 将模板应用到物品 Entity 上
    /// </summary>
    public static void Apply(string templateName, Entity item)
    {
        if (!_templates.TryGetValue(templateName, out var template))
            throw new ArgumentException($"物品模板 '{templateName}' 未找到");
        template.Configure(item);
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

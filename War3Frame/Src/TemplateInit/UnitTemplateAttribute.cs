using Friflo.Engine.ECS;

namespace War3Frame.TemplateInit;

/// <summary>
/// Attribute to mark a class as a unit template
/// Source Generator will auto-discover and register these
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UnitTemplateAttribute : Attribute
{
    public string Name { get; }
    public UnitTemplateAttribute(string name) => Name = name;
}

/// <summary>
/// Interface for unit template configuration
/// </summary>
public interface IUnitTemplate
{
    /// <summary>
    /// Configure the unit entity with components
    /// </summary>
    void Configure(Entity entity);
}

/// <summary>
/// Unit template registry and factory
/// </summary>
public static partial class UnitTemplate
{
    private static readonly SortedDictionary<string, IUnitTemplate> _templates = new();
    private static bool _initialized = false;

    /// <summary>
    /// Initialize the template system (call once at game start)
    /// </summary>
    public static void Initialize()
    {
        if (_initialized) return;
        _initialized = true;

        // Call auto-generated registration method
        RegisterGenerated();
    }

    /// <summary>
    /// Register a template (can also be called manually)
    /// </summary>
    public static void Register(string name, IUnitTemplate template)
    {
        _templates[name] = template;
    }

    /// <summary>
    /// Create a unit from template (Entity only, no native unit)
    /// </summary>
    public static Entity Create(string templateName)
    {
        if (!_templates.TryGetValue(templateName, out var template))
        {
            throw new ArgumentException($"Unit template '{templateName}' not found");
        }

        var entity = Game.Store.CreateEntity();
        template.Configure(entity);
        return entity;
    }

    /// <summary>
    /// Create a unit from template with native War3 unit
    /// </summary>
    public static Entity Create(string templateName, JPlayer player, float x, float y, float facing = 270)
    {
        if (!_templates.TryGetValue(templateName, out var template))
        {
            throw new ArgumentException($"Unit template '{templateName}' not found");
        }

        // 创建原生 War3 单位（使用通用单位类型，后续可以扩展）
        var junit = JassApi.CreateUnit(player, JassApi.C2I("hfoo"), x, y, facing);
        HandleHelper.HandleAdd(junit);

        // 创建 Entity + 基础组件
        var entity = Game.Store.CreateEntity(
            new UnitNative { unit = junit },
            new UnitState { isAlive = true },
            new Position { x = x, y = y }
        );

        // 应用模板配置
        template.Configure(entity);
        return entity;
    }

    /// <summary>
    /// Try to create a unit from template
    /// </summary>
    public static bool TryCreate(string templateName, out Entity entity)
    {
        if (_templates.TryGetValue(templateName, out var template))
        {
            entity = Game.Store.CreateEntity();
            template.Configure(entity);
            return true;
        }

        entity = default;
        return false;
    }

    /// <summary>
    /// Check if a template exists
    /// </summary>
    public static bool HasTemplate(string name) => _templates.ContainsKey(name);

    /// <summary>
    /// Get all registered template names
    /// </summary>
    public static IEnumerable<string> GetAllTemplateNames() => _templates.Keys;

    /// <summary>
    /// Auto-generated registration method (implemented by Source Generator)
    /// </summary>
    static partial void RegisterGenerated();
}
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace War3Frame.Generator;

// ╔═══════════════════════════════════════════════════════════════════════════╗
// ║                    Source Generator 学习指南                               ║
// ╠═══════════════════════════════════════════════════════════════════════════╣
// ║  Source Generator 是 C# 编译器的扩展，在编译时运行                          ║
// ║  它可以分析源代码并生成额外的 C# 代码                                        ║
// ║                                                                           ║
// ║  工作流程:                                                                 ║
// ║  1. 编译器加载 Generator                                                   ║
// ║  2. Generator 扫描源代码，查找特定模式（如 Attribute）                       ║
// ║  3. Generator 根据发现的内容生成新代码                                       ║
// ║  4. 生成的代码加入编译                                                      ║
// ╚═══════════════════════════════════════════════════════════════════════════╝

/// <summary>
/// UnitTemplate + AbilityTemplate 源生成器
/// 
/// 功能: 自动发现所有标记了 [UnitTemplate("xxx")] 和 [AbilityTemplate("xxx")] 的类，
///      并生成注册代码，避免手动维护模板列表
/// </summary>
[Generator]
public class UnitTemplateGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // ════════════════ UnitTemplate 管道 ════════════════

        var unitTemplates = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "War3Frame.TemplateInit.UnitTemplateAttribute",
                predicate: static (node, _) => node is ClassDeclarationSyntax,
                transform: static (ctx, _) => GetTemplateInfo(ctx, "War3Frame.TemplateInit.UnitTemplateAttribute"))
            .Where(static info => info is not null)
            .Select(static (info, _) => info!.Value);

        var collectedUnitTemplates = unitTemplates.Collect();
        context.RegisterSourceOutput(collectedUnitTemplates,
            (ctx, templates) => GenerateRegistrationCode(ctx, templates,
                "UnitTemplate.g.cs", "UnitTemplate", "IUnitTemplate"));

        // ════════════════ AbilityTemplate 管道 ════════════════

        var abilityTemplates = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "War3Frame.TemplateInit.AbilityTemplateAttribute",
                predicate: static (node, _) => node is ClassDeclarationSyntax,
                transform: static (ctx, _) => GetTemplateInfo(ctx, "War3Frame.TemplateInit.AbilityTemplateAttribute"))
            .Where(static info => info is not null)
            .Select(static (info, _) => info!.Value);

        var collectedAbilityTemplates = abilityTemplates.Collect();
        context.RegisterSourceOutput(collectedAbilityTemplates,
            (ctx, templates) => GenerateRegistrationCode(ctx, templates,
                "AbilityTemplate.g.cs", "AbilityTemplate", "IAbilityTemplate"));

        // ════════════════ ItemTemplate 管道 ════════════════

        var itemTemplates = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "War3Frame.TemplateInit.ItemTemplateAttribute",
                predicate: static (node, _) => node is ClassDeclarationSyntax,
                transform: static (ctx, _) => GetTemplateInfo(ctx, "War3Frame.TemplateInit.ItemTemplateAttribute"))
            .Where(static info => info is not null)
            .Select(static (info, _) => info!.Value);

        var collectedItemTemplates = itemTemplates.Collect();
        context.RegisterSourceOutput(collectedItemTemplates,
            (ctx, templates) => GenerateRegistrationCode(ctx, templates,
                "ItemTemplate.g.cs", "ItemTemplate", "IItemTemplate"));
    }

    /// <summary>
    /// 从语法上下文中提取模板信息（通用方法，支持 UnitTemplate 和 AbilityTemplate）
    /// </summary>
    private static TemplateInfo? GetTemplateInfo(GeneratorAttributeSyntaxContext context, string attributeFullName)
    {
        if (context.TargetSymbol is not INamedTypeSymbol classSymbol)
            return null;

        var attributeData = context.Attributes.FirstOrDefault(a =>
            a.AttributeClass?.ToDisplayString() == attributeFullName);

        if (attributeData is null)
            return null;

        var templateName = attributeData.ConstructorArguments.FirstOrDefault().Value?.ToString();
        if (string.IsNullOrEmpty(templateName))
            return null;

        return new TemplateInfo
        {
            ClassName = classSymbol.ToDisplayString(),
            TemplateName = templateName!,
            Namespace = classSymbol.ContainingNamespace.ToDisplayString()
        };
    }

    /// <summary>
    /// 生成注册代码（通用方法）
    /// </summary>
    private static void GenerateRegistrationCode(
        SourceProductionContext context,
        ImmutableArray<TemplateInfo> templates,
        string fileName,
        string registryClassName,
        string interfaceName)
    {
        if (templates.Length == 0)
            return;

        var sb = new StringBuilder();

        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("#nullable enable");
        sb.AppendLine();
        sb.AppendLine("using Friflo.Engine.ECS;");
        sb.AppendLine();

        var namespaces = templates
            .Select(t => t.Namespace)
            .Distinct()
            .OrderBy(n => n);

        foreach (var ns in namespaces)
        {
            if (!string.IsNullOrEmpty(ns) && ns != "global")
            {
                sb.AppendLine($"using {ns};");
            }
        }

        sb.AppendLine();
        sb.AppendLine("namespace War3Frame.TemplateInit;");
        sb.AppendLine();
        sb.AppendLine($"public static partial class {registryClassName}");
        sb.AppendLine("{");
        sb.AppendLine("    /// <summary>");
        sb.AppendLine($"    /// Auto-generated method to register all {registryClassName.ToLower()}s");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    static partial void RegisterGenerated()");
        sb.AppendLine("    {");

        foreach (var template in templates.OrderBy(t => t.TemplateName))
        {
            sb.AppendLine($"        Register(\"{template.TemplateName}\", new {template.ClassName}());");
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");

        context.AddSource(fileName, SourceText.From(sb.ToString(), Encoding.UTF8));
    }

    /// <summary>
    /// 模板信息结构体
    /// </summary>
    private struct TemplateInfo
    {
        public string ClassName;
        public string TemplateName;
        public string Namespace;
    }
}

// ╔═══════════════════════════════════════════════════════════════════════════╗
// ║                              生成结果示例                                  ║
// ╠═══════════════════════════════════════════════════════════════════════════╣
// ║  当项目中有以下模板类:                                                     ║
// ║                                                                           ║
// ║    [UnitTemplate("footman")]                                              ║
// ║    public class FootmanTemplate : IUnitTemplate { ... }                   ║
// ║                                                                           ║
// ║    [UnitTemplate("knight")]                                               ║
// ║    public class KnightTemplate : IUnitTemplate { ... }                    ║
// ║                                                                           ║
// ║  生成的代码 (UnitTemplate.g.cs):                                          ║
// ║                                                                           ║
// ║    // <auto-generated/>                                                   ║
// ║    #nullable enable                                                       ║
// ║                                                                           ║
// ║    using Friflo.Engine.ECS;                                               ║
// ║    using War3Frame.Templates;                                             ║
// ║                                                                           ║
// ║    namespace War3Frame.TemplateInit;                                      ║
// ║                                                                           ║
// ║    public static partial class UnitTemplate                               ║
// ║    {                                                                      ║
// ║        static partial void RegisterGenerated()                            ║
// ║        {                                                                  ║
// ║            Register("footman", new FootmanTemplate());                    ║
// ║            Register("knight", new KnightTemplate());                      ║
// ║        }                                                                  ║
// ║    }                                                                      ║
// ╚═══════════════════════════════════════════════════════════════════════════╝

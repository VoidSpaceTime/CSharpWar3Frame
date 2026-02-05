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
/// UnitTemplate 源生成器
/// 
/// 功能: 自动发现所有标记了 [UnitTemplate("xxx")] 的类，
///      并生成注册代码，避免手动维护模板列表
/// 
/// 输入示例:
///   [UnitTemplate("footman")]
///   public class FootmanTemplate : IUnitTemplate { ... }
/// 
/// 输出示例 (自动生成):
///   static partial void RegisterGenerated()
///   {
///       Register("footman", new FootmanTemplate());
///   }
/// </summary>
[Generator]  // 标记这是一个 Source Generator，编译器会自动加载
public class UnitTemplateGenerator : IIncrementalGenerator
{
    // ══════════════════════════════════════════════════════════════════════
    // IIncrementalGenerator 是增量式 Generator 接口（.NET 6+ 推荐）
    // 相比旧版 ISourceGenerator，它支持增量编译，性能更好
    // 只有当相关代码变化时才会重新生成，而不是每次都全量生成
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// 初始化方法 - Generator 的入口点
    /// 在这里设置"管道"：定义要查找什么，找到后如何处理
    /// </summary>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // ┌─────────────────────────────────────────────────────────────────┐
        // │ 第一步: 定义数据源 - 查找所有带 [UnitTemplate] 的类              │
        // └─────────────────────────────────────────────────────────────────┘

        var templateClasses = context.SyntaxProvider
            // ForAttributeWithMetadataName: 高效查找带特定 Attribute 的节点
            // 这是 Roslyn 提供的优化方法，比手动遍历语法树快得多
            .ForAttributeWithMetadataName(
                // 要查找的 Attribute 完整名称（包含命名空间）
                "War3Frame.TemplateInit.UnitTemplateAttribute",

                // predicate: 语法过滤器 - 只关心 class 声明
                // static 关键字让 lambda 不捕获外部变量，提高性能
                predicate: static (node, _) => node is ClassDeclarationSyntax,

                // transform: 转换函数 - 将找到的节点转换为我们需要的信息
                transform: static (ctx, _) => GetTemplateInfo(ctx))

            // 过滤掉转换失败的（返回 null 的）
            .Where(static info => info is not null)

            // 解包 Nullable，因为我们已经过滤掉了 null
            .Select(static (info, _) => info!.Value);

        // ┌─────────────────────────────────────────────────────────────────┐
        // │ 第二步: 收集所有结果到一个数组                                   │
        // └─────────────────────────────────────────────────────────────────┘

        // Collect(): 将流式数据收集到 ImmutableArray
        // 这样我们可以在一次生成调用中处理所有模板
        var collectedTemplates = templateClasses.Collect();

        // ┌─────────────────────────────────────────────────────────────────┐
        // │ 第三步: 注册输出 - 当数据准备好时，调用生成方法                   │
        // └─────────────────────────────────────────────────────────────────┘

        // RegisterSourceOutput: 注册生成代码的回调
        // 当 collectedTemplates 变化时，会调用 GenerateRegistrationCode
        context.RegisterSourceOutput(collectedTemplates, GenerateRegistrationCode);
    }

    /// <summary>
    /// 从语法上下文中提取模板信息
    /// 
    /// 这个方法解析每一个带 [UnitTemplate] 的类，提取:
    /// - 类的完整名称 (如 "War3Frame.Templates.FootmanTemplate")
    /// - 模板名称 (如 "footman")
    /// - 命名空间 (用于生成 using 语句)
    /// </summary>
    private static TemplateInfo? GetTemplateInfo(GeneratorAttributeSyntaxContext context)
    {
        // context.TargetSymbol: 被 Attribute 标记的符号（这里是类）
        // INamedTypeSymbol: 代表一个命名类型（class, struct, interface 等）
        if (context.TargetSymbol is not INamedTypeSymbol classSymbol)
            return null;

        // ══════════════════════════════════════════════════════════════════
        // 从 Attribute 中提取参数
        // ══════════════════════════════════════════════════════════════════

        // context.Attributes: 该符号上的所有 Attribute 数据
        // 我们查找 UnitTemplateAttribute
        var attributeData = context.Attributes.FirstOrDefault(a =>
            a.AttributeClass?.ToDisplayString() == "War3Frame.TemplateInit.UnitTemplateAttribute");

        if (attributeData is null)
            return null;

        // ══════════════════════════════════════════════════════════════════
        // 获取构造函数参数
        // [UnitTemplate("footman")] 中的 "footman"
        // ══════════════════════════════════════════════════════════════════

        // ConstructorArguments: Attribute 构造函数的参数列表
        // 第一个参数就是模板名称
        var templateName = attributeData.ConstructorArguments.FirstOrDefault().Value?.ToString();
        if (string.IsNullOrEmpty(templateName))
            return null;

        // 返回提取的信息
        return new TemplateInfo
        {
            // ToDisplayString(): 获取符号的完整限定名
            // 如 "War3Frame.Templates.FootmanTemplate"
            ClassName = classSymbol.ToDisplayString(),
            TemplateName = templateName!,
            // ContainingNamespace: 类所在的命名空间
            Namespace = classSymbol.ContainingNamespace.ToDisplayString()
        };
    }

    /// <summary>
    /// 生成注册代码
    /// 
    /// 这个方法接收所有发现的模板信息，生成 C# 源代码
    /// 生成的代码会自动加入编译
    /// </summary>
    private static void GenerateRegistrationCode(
        SourceProductionContext context,      // 用于添加生成的源代码
        ImmutableArray<TemplateInfo> templates)  // 收集到的所有模板信息
    {
        // 没有模板就不生成
        if (templates.Length == 0)
            return;

        // ══════════════════════════════════════════════════════════════════
        // 使用 StringBuilder 构建代码字符串
        // ══════════════════════════════════════════════════════════════════

        var sb = new StringBuilder();

        // 生成文件头
        sb.AppendLine("// <auto-generated/>");  // 标记这是自动生成的代码
        sb.AppendLine("#nullable enable");       // 启用 nullable
        sb.AppendLine();
        sb.AppendLine("using Friflo.Engine.ECS;");
        sb.AppendLine();

        // ══════════════════════════════════════════════════════════════════
        // 生成必要的 using 语句
        // 收集所有模板类所在的命名空间
        // ══════════════════════════════════════════════════════════════════

        var namespaces = templates
            .Select(t => t.Namespace)    // 取出命名空间
            .Distinct()                   // 去重
            .OrderBy(n => n);            // 排序（输出稳定）

        foreach (var ns in namespaces)
        {
            // 排除全局命名空间
            if (!string.IsNullOrEmpty(ns) && ns != "global")
            {
                sb.AppendLine($"using {ns};");
            }
        }

        // ══════════════════════════════════════════════════════════════════
        // 生成 partial class 和注册方法
        // ══════════════════════════════════════════════════════════════════

        sb.AppendLine();
        sb.AppendLine("namespace War3Frame.TemplateInit;");
        sb.AppendLine();
        // partial class: 与手写的 UnitTemplate 类合并
        sb.AppendLine("public static partial class UnitTemplate");
        sb.AppendLine("{");
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Auto-generated method to register all unit templates");
        sb.AppendLine("    /// </summary>");
        // partial method: 实现在 UnitTemplateAttribute.cs 中声明的分部方法
        sb.AppendLine("    static partial void RegisterGenerated()");
        sb.AppendLine("    {");

        // ══════════════════════════════════════════════════════════════════
        // 生成每个模板的注册语句
        // ══════════════════════════════════════════════════════════════════

        // OrderBy: 按模板名排序，确保生成顺序稳定（方便 diff）
        foreach (var template in templates.OrderBy(t => t.TemplateName))
        {
            // 生成: Register("footman", new War3Frame.Templates.FootmanTemplate());
            sb.AppendLine($"        Register(\"{template.TemplateName}\", new {template.ClassName}());");
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");

        // ══════════════════════════════════════════════════════════════════
        // 将生成的代码添加到编译
        // ══════════════════════════════════════════════════════════════════

        // AddSource: 添加生成的源文件
        // 参数1: 文件名（会显示在 IDE 的生成文件夹中）
        // 参数2: 源代码内容
        context.AddSource("UnitTemplate.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }

    // ══════════════════════════════════════════════════════════════════════
    // 数据结构：存储从源代码中提取的模板信息
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// 模板信息结构体
    /// </summary>
    private struct TemplateInfo
    {
        /// <summary>类的完整名称，如 "War3Frame.Templates.FootmanTemplate"</summary>
        public string ClassName;

        /// <summary>模板名称，如 "footman"</summary>
        public string TemplateName;

        /// <summary>类所在的命名空间，如 "War3Frame.Templates"</summary>
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

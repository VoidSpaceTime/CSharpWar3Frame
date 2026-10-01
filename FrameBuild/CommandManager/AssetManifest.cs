using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using War3Frame;

namespace War3FrameBuild.CommandManager;

public partial class CommandManager
{
    internal string GeneratedAssetsTargetsPath => Path.Combine(WorkingBuildPath, "generated", "assets.targets");

    /// <summary>把资源清单转换到构建目录，通过 MSBuild 编译输入替换原清单；源码保持不变。</summary>
    public void SupplementAssetsPackPath(string[] targetPaths)
    {
        var trees = targetPaths.OrderBy(p => p, StringComparer.Ordinal).Select(p =>
            CSharpSyntaxTree.ParseText(File.ReadAllText(p), path: Path.GetFullPath(p))).ToArray();
        var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(Path.PathSeparator)
            .Append(typeof(Assets).Assembly.Location).Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(p => MetadataReference.CreateFromFile(p));
        var compilation = CSharpCompilation.Create("AssetManifest", trees.Append(CSharpSyntaxTree.ParseText(
            "global using System; global using System.Collections.Generic; global using System.Linq;")), references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var outputs = new List<(string Source, string Generated, string Content)>();
        foreach (var tree in trees)
        {
            var rewriter = new AssetManifestRewriter(this, compilation.GetSemanticModel(tree));
            var rewritten = rewriter.Visit(tree.GetRoot())!;
            if (rewritten.ContainsDiagnostics) throw new InvalidDataException("资源转换生成了无效语法: " + tree.FilePath);
            var relative = Path.GetRelativePath(PwdProject, tree.FilePath);
            var generated = ResourcePath(Path.Combine(WorkingBuildPath, "generated"), relative);
            outputs.Add((relative, generated, rewritten.NormalizeWhitespace().ToFullString()));
        }
        var items = new XElement("ItemGroup");
        foreach (var output in outputs)
        {
            WriteGeneratedFile(output.Generated, output.Content);
            items.Add(new XElement("Compile", new XAttribute("Remove", EscapeMsbuild(output.Source))));
            items.Add(new XElement("Compile", new XAttribute("Include", "$(MSBuildThisFileDirectory)" +
                EscapeMsbuild(Path.GetRelativePath(Path.GetDirectoryName(GeneratedAssetsTargetsPath)!, output.Generated)))));
        }
        var target = new XElement("Target", new XAttribute("Name", "UseWar3AssetManifests"),
            new XAttribute("BeforeTargets", "CoreCompile"),
            new XAttribute("Condition", "'$(MSBuildProjectFullPath)' == '" + EscapeMsbuild(Path.GetFullPath(Path.Combine(PwdProject, ProjectName + ".csproj"))) + "'"), items);
        WriteGeneratedFile(GeneratedAssetsTargetsPath, new XDocument(new XElement("Project", target)).ToString());
    }

    private static string EscapeMsbuild(string value) => value.Replace("%", "%25").Replace("$", "%24")
        .Replace("@", "%40").Replace(";", "%3B").Replace("'", "%27").Replace("*", "%2A").Replace("?", "%3F");

    private static void WriteGeneratedFile(string path, string content)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        if (File.Exists(path) && File.ReadAllText(path) == content) return;
        var temporary = path + ".tmp";
        File.WriteAllText(temporary, content);
        File.Move(temporary, path, true);
    }

    private sealed class AssetManifestRewriter(CommandManager manager, SemanticModel semantic) : CSharpSyntaxRewriter
    {
        public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (semantic.GetOperation(node) is not IInvocationOperation call || call.TargetMethod.ContainingType.ToDisplayString() != "War3Frame.Assets")
                return base.VisitInvocationExpression(node);
            var explicitArgs = call.Arguments.Where(a => a.ArgumentKind != ArgumentKind.DefaultValue)
                .ToDictionary(a => a.Parameter!.Name, a => ((ArgumentSyntax)a.Syntax).Expression, StringComparer.Ordinal);
            var method = call.TargetMethod.Name;
            var derived = new Dictionary<string, ExpressionSyntax>(StringComparer.Ordinal);
            string Constant(string name)
            {
                var value = semantic.GetConstantValue(explicitArgs[name]);
                if (!value.HasValue || value.Value is not string text)
                    throw new InvalidDataException($"{node.SyntaxTree.FilePath}: {method}.{name} 必须是编译期字符串常量");
                return text;
            }
            bool Needs(string name) => !explicitArgs.TryGetValue(name, out var expression)
                || semantic.GetConstantValue(expression) is { HasValue: true, Value: null or "" };
            switch (method)
            {
                case "AddImage": case "AddModel": case "AddSounds": case "AddVCM": case "AddV3D":
                    var kind = method switch { "AddImage" => "image", "AddModel" => "model", "AddSounds" => "bgm", "AddVCM" => "vcm", _ => "v3d" };
                    var asset = manager.AnalysisFile(kind, Constant("path"), manager.IsSkip);
                    if (!asset.status) throw new FileNotFoundException("资源不存在", Constant("path"));
                    if (Needs("pickPath")) derived["pickPath"] = StringLiteral(method == "AddModel" ? Path.ChangeExtension(asset.pickPath, ".mdl") : asset.pickPath);
                    if (kind is "bgm" or "vcm" or "v3d" && !explicitArgs.ContainsKey("duration"))
                        derived["duration"] = SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                            SyntaxFactory.Literal(asset.isWar3 ? manager.GetWar3AuidoDuration(asset.pickPath) : manager.GetAudioFileDuration(asset.sourcePath)));
                    break;
                case "AddVWP":
                    if (Needs("vwpSound")) derived["vwpSound"] = manager.BuildVoicePack(Constant("folder"));
                    break;
                case "AddFont": manager.InstallFont(Constant("file")); break;
                case "AddTerrain": manager.InstallTerrain(Constant("dir"), Path.Combine(manager.WorkingBuildPath, "resource")); break;
                case "AddSelection": manager.InstallSelection(Constant("dir")); break;
                case "AddLoading": manager.InstallLoading(Constant("fileName")); break;
                case "AddPreview": manager.InstallPreview(Constant("fileName")); break;
                case "AddUIKit": manager.InstallUIKit(Constant("dir")); break;
                default: return base.VisitInvocationExpression(node);
            }
            // 按符号绑定参数，但保留源码求值顺序；命名参数表达式也可能带副作用。
            var args = new List<ArgumentSyntax>();
            foreach (var argument in call.Arguments.Where(a => a.ArgumentKind != ArgumentKind.DefaultValue).OrderBy(a => a.Syntax.SpanStart))
            {
                var name = argument.Parameter!.Name;
                var expression = derived.Remove(name, out var replacement) ? replacement : explicitArgs[name];
                args.Add(SyntaxFactory.Argument(expression).WithNameColon(SyntaxFactory.NameColon(SyntaxFactory.IdentifierName(name))));
            }
            // 只追加省略的派生参数，其他默认值仍交给 C#。
            foreach (var parameter in call.TargetMethod.Parameters)
                if (derived.TryGetValue(parameter.Name, out var expression))
                    args.Add(SyntaxFactory.Argument(expression).WithNameColon(SyntaxFactory.NameColon(SyntaxFactory.IdentifierName(parameter.Name))));
            return node.WithArgumentList(SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(args)));
        }
    }

    private static LiteralExpressionSyntax StringLiteral(string value)
        => SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(value));
}

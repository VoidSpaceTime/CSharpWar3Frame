using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using War3Frame.Generator;
using War3Frame.TemplateInit;

namespace War3Frame.Regression;

internal static class GeneratorRegression
{
    internal static void Register(List<RegressionCase> tests)
    {
        tests.Add(new("generator", "consumer-types-literals-idempotence", Consumer));
        tests.Add(new("generator", "empty-consumer", Empty));
        tests.Add(new("generator", "source-diagnostics", Diagnostics));
        tests.Add(new("generator", "real-project-templates", RealTemplates));
        tests.Add(new("generator", "framework-global-system-order", FrameworkSystems));
    }

    private static (Compilation Compilation, GeneratorDriverRunResult Result) Generate(params string[] sources)
    {
        var compilation = CompilationTest.Create(sources);
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            [new UnitTemplateGenerator().AsSourceGenerator(), new SystemGenerator().AsSourceGenerator()],
            parseOptions: CompilationTest.ParseOptions);
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var output, out _);
        return (output, driver.GetRunResult());
    }

    private static System.Reflection.Assembly Initialize(Compilation compilation)
    {
        var assembly = CompilationTest.Emit(compilation);
        var initialize = assembly.GetType("War3Frame.Generated.ProjectTemplateRegistration")!
            .GetMethod("Initialize", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!;
        initialize.Invoke(null, null);
        initialize.Invoke(null, null);
        return assembly;
    }

    private static void Consumer()
    {
        const string source = """
            using Friflo.Engine.ECS;
            using War3Frame.TemplateInit;
            [UnitTemplate("regression:quote\"slash\\newline\n")]
            public class GlobalTemplate : IUnitTemplate { public void Configure(Entity entity) {} }
            namespace One {
                public class Outer { [ItemTemplate("regression:nested")] public class Same : IItemTemplate { public void Configure(Entity entity) {} } }
            }
            namespace Two {
                [AbilityTemplate("regression:ability")]
                public class Same : IAbilityTemplate {
                    public static int Constructed;
                    public Same() { Constructed++; }
                    public void Configure(Entity entity, int level) {}
                }
            }
            """;
        var (output, result) = Generate(source);
        Check.That(!result.Diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error), "valid consumer diagnostics");
        var assembly = Initialize(output);
        Check.That(UnitTemplate.HasTemplate("regression:quote\"slash\\newline\n") && ItemTemplate.HasTemplate("regression:nested")
            && AbilityTemplate.HasTemplate("regression:ability"), "three template registries receive exact names");
        Check.That((int)assembly.GetType("Two.Same")!.GetField("Constructed")!.GetValue(null)! == 1, "initialize is idempotent");
        var (_, repeated) = Generate(source);
        Check.That(result.GeneratedTrees.Select(t => t.ToString()).SequenceEqual(repeated.GeneratedTrees.Select(t => t.ToString())), "deterministic output");
    }

    private static void Empty() => Initialize(Generate("public class Empty {}").Compilation);

    private static void FrameworkSystems()
    {
        // Attribute 与 Game 在被编译程序集内，走框架生成路径；实际系统基类来自 Friflo。
        var sources = new[]
        {
            "global using System;",
            CompilationTest.Read("War3Frame/Src/Systems/SystemRegisterAttribute.cs"),
            """
            using Friflo.Engine.ECS.Systems;
            using War3Frame.Systems;
            namespace War3Frame {
                public static partial class Game {
                    private static readonly RecordingRoot Root = new();
                    static partial void RegisterGeneratedSystems();
                    public static string[] Register() { RegisterGeneratedSystems(); return Root.Calls.ToArray(); }
                }
                public class RecordingRoot {
                    public readonly System.Collections.Generic.List<string> Calls = new();
                    public void Add(BaseSystem system) => Calls.Add(system.GetType().FullName + ":interval");
                    public void Add(BaseSystem system, float delay) => Calls.Add(system.GetType().FullName + (delay == 0 ? ":immediate" : ":delayed"));
                }
            }
            [SystemRegister(SystemKind.Immediate, 0)]
            public class GlobalSystem : QuerySystem<War3Frame.Duration> { protected override void OnUpdate() {} }
            namespace Beta {
                [SystemRegister(SystemKind.Interval, 4)]
                public class Same : QuerySystem<War3Frame.Duration> { protected override void OnUpdate() {} }
            }
            namespace Alpha {
                public class Outer {
                    [SystemRegister(SystemKind.Interval, 4)]
                    public class Same : QuerySystem<War3Frame.Duration> { protected override void OnUpdate() {} }
                }
            }
            """
        };
        var (output, result) = Generate(sources);
        Check.That(!result.Diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error), "valid framework systems accepted");
        var assembly = CompilationTest.Emit(output);
        var order = (string[])assembly.GetType("War3Frame.Game")!.GetMethod("Register")!.Invoke(null, null)!;
        Check.That(order.SequenceEqual(new[] { "GlobalSystem:immediate", "Alpha.Outer+Same:interval", "Beta.Same:interval" }),
            "global/nested systems compile and register by order then ordinal qualified name");
        var (_, invalid) = Generate(sources.Append("""
            [War3Frame.Systems.SystemRegister(War3Frame.Systems.SystemKind.Interval)]
            public class InvalidSystem {}
            """).ToArray());
        Check.That(invalid.Diagnostics.Any(d => d.Id == "WFGEN003" && d.Location.IsInSource), "invalid framework system diagnosed at source");
    }

    private static void Diagnostics()
    {
        var (_, result) = Generate("""
            using Friflo.Engine.ECS;
            using Friflo.Engine.ECS.Systems;
            using War3Frame.TemplateInit;
            using War3Frame.Systems;
            [UnitTemplate("duplicate")] public class A : IUnitTemplate { public void Configure(Entity e) {} }
            [UnitTemplate("duplicate")] public class B : IUnitTemplate { public void Configure(Entity e) {} }
            [UnitTemplate("abstract")] public abstract class C : IUnitTemplate { public abstract void Configure(Entity e); }
            [UnitTemplate("generic")] public class D<T> : IUnitTemplate { public void Configure(Entity e) {} }
            [UnitTemplate("ctor")] public class E : IUnitTemplate { private E() {} public void Configure(Entity e) {} }
            [ItemTemplate("interface")] public class F {}
            [SystemRegister(SystemKind.Interval)] public class ExternalSystem : QuerySystem<War3Frame.Duration> { protected override void OnUpdate() {} }
            """);
        Check.That(result.Diagnostics.Count(d => d.Id == "WFGEN001") == 4, "invalid type inputs diagnosed");
        Check.That(result.Diagnostics.Count(d => d.Id == "WFGEN002") == 2, "both duplicate declarations diagnosed");
        Check.That(result.Diagnostics.Any(d => d.Id == "WFGEN004" && d.Location.IsInSource), "external systems explicitly rejected at source");
    }

    private static void RealTemplates()
    {
        var sources = Directory.GetFiles(Path.Combine(CompilationTest.Repository, "Projects/test"), "*.cs", SearchOption.AllDirectories)
            .Where(p => !p.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar))
            .Select(File.ReadAllText).Where(s => s.Contains("[UnitTemplate(") || s.Contains("[AbilityTemplate(") || s.Contains("[ItemTemplate("));
        var (output, result) = Generate(sources.Prepend("global using System; global using System.Collections.Generic; global using System.Linq;").ToArray());
        var expected = result.GeneratedTrees.Sum(t => t.ToString().Split(".Register(").Length - 1);
        Check.That(expected >= 20, "real project templates discovered");
        Initialize(output);
    }
}

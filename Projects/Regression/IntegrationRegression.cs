using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text.Json;
using FastMDX;
using War3FrameBuild.CommandManager;
using War3FrameBuild.Execution;
using War3FrameBuild.Extension;

namespace War3Frame.Regression;

internal static class IntegrationRegression
{
    internal static void Register(List<RegressionCase> tests)
    {
        tests.Add(new("integration", "cli-failure-exit-codes", Cli));
        tests.Add(new("integration", "scaffold-jit-publish", Scaffold));
        tests.Add(new("integration", "test-jit-publish", TestPublish));
        tests.Add(new("integration", "model-format-input-and-exit", ModelFormat));
    }

    private static async Task<ProcessResult> Dotnet(params string[] args)
        => await new ProcessRunner().RunAsync(new ProcessRequest(BuildRegression.Dotnet, args, Timeout: TimeSpan.FromMinutes(3)));

    private static async Task Cli()
    {
        using var fixture = new BuildRegression.Fixture();
        var destination = Path.Combine(fixture.Root, "cli");
        DirectoryExtensions.CopyDir(Path.Combine(CompilationTest.Repository, "CSharpWar3Frame/bin/Release/net10.0"), destination);
        var config = fixture.Config;
        BuildRegression.Fixture.Write(Path.Combine(destination, "appsettings.yml"), JsonSerializer.Serialize(new
        { war3 = config.War3, we = config.We, pwd = config.Pwd, w3x2lni = config.W3x2lni, assets = config.Assets }));
        var executable = Path.Combine(destination, "CSharpWar3FrameConsole.dll");
        foreach (var args in new[] { new[] { "multi", "0" }, new[] { "run", "missing", "--noTest" }, new[] { "new", "sample" } })
        {
            var result = await Dotnet(new[] { executable }.Concat(args).ToArray());
            Check.That(result.ExitCode == 1 && !result.Canceled, "CLI nonzero for " + string.Join(' ', args) + ": " + result.Output + result.Error);
        }
    }

    private static async Task Scaffold()
    {
        using var fixture = new BuildRegression.Fixture();
        var exclude = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "bin", "obj", ".git", ".temp", ".build" };
        foreach (var directory in new[] { "War3Frame", "War3Frame.Generator", "Projects/demo" })
            DirectoryExtensions.CopyDir(Path.Combine(CompilationTest.Repository, directory), Path.Combine(fixture.Root, directory), exclude);
        // 构建缓存不应从模板复制进新项目。
        BuildRegression.Fixture.Write(Path.Combine(fixture.Root, "Projects/demo/obj/stale"), "stale");
        var manager = new CommandManager(fixture.Config, "created");
        Check.That(manager.New(), "new command creates scaffold");
        Check.That(!Directory.Exists(Path.Combine(manager.PwdProject, "obj")), "scaffold excludes prior build cache");
        var output = Path.Combine(fixture.Root, "published");
        var result = await Dotnet("publish", Path.Combine(manager.PwdProject, "created.csproj"), "-c", "Release", "-r", "win-x86", "--self-contained", "false", "-p:PublishAot=false", "-o", output, "-v", "quiet");
        Check.That(result.Success, "created project publishes: " + result.Output + result.Error);
        VerifyPayload(output);
    }

    private static async Task TestPublish()
    {
        using var fixture = new BuildRegression.Fixture();
        var output = Path.Combine(fixture.Root, "test-published");
        var result = await Dotnet("publish", Path.Combine(CompilationTest.Repository, "Projects/test/test.csproj"), "-c", "Release", "-r", "win-x86", "--self-contained", "false", "-p:PublishAot=false", "-o", output, "-v", "quiet");
        Check.That(result.Success, "test JIT publish: " + result.Output + result.Error);
        VerifyPayload(output);
    }

    private static void VerifyPayload(string output)
    {
        foreach (var file in new[] { "project.dll", "project.deps.json", "project.runtimeconfig.json", "War3Frame.dll", "Friflo.Engine.ECS.dll" })
            Check.That(File.Exists(Path.Combine(output, file)), "payload file " + file);
        using var stream = File.OpenRead(Path.Combine(output, "project.dll"));
        using var pe = new PEReader(stream);
        var metadata = pe.GetMetadataReader();
        var entry = metadata.TypeDefinitions.Select(metadata.GetTypeDefinition)
            .Single(t => metadata.GetString(t.Namespace) == "War3Frame" && metadata.GetString(t.Name) == "Bootstrap");
        var method = entry.GetMethods().Select(metadata.GetMethodDefinition)
            .Single(m => metadata.GetString(m.Name) == "BridgeMain");
        Check.That(method.Attributes.HasFlag(MethodAttributes.Public) && method.Attributes.HasFlag(MethodAttributes.Static), "bridge entry exists without invoking native functions");
        Check.That(metadata.TypeDefinitions.Select(metadata.GetTypeDefinition)
            .Any(t => metadata.GetString(t.Namespace) == "War3Frame.Generated" && metadata.GetString(t.Name) == "ProjectTemplateRegistration"), "project registrar is emitted in actual payload");
    }

    private static async Task ModelFormat()
    {
        using var fixture = new BuildRegression.Fixture();
        var project = Path.Combine(CompilationTest.Repository, "ModelFormat/ModelFormat.csproj");
        var build = await Dotnet("build", project, "-c", "Release", "-v", "quiet");
        Check.That(build.Success, "ModelFormat build: " + build.Output + build.Error);
        var executable = Path.Combine(CompilationTest.Repository, "ModelFormat/bin/Release/net10.0-windows/win-x64/ModelFormat.dll");
        var source = Path.Combine(fixture.Root, "input-models");
        Directory.CreateDirectory(source);
        var file = Path.Combine(source, "example.mdx");
        new MDX().SaveTo(file);
        var before = File.ReadAllBytes(file);
        var result = await Dotnet(executable, source);
        Check.That(result.Success && File.Exists(Path.Combine(source + "_Format", "example", "example.mdx")), "formatter honors provided input and exits: " + result.Output + result.Error);
        Check.That(File.ReadAllBytes(file).SequenceEqual(before), "formatter leaves original model unchanged");
        Check.That((await Dotnet(executable)).ExitCode == 1, "missing argument is a failure");
        Check.That((await Dotnet(executable, Path.GetPathRoot(source)!)).ExitCode == 1, "filesystem root has no sibling output and is rejected before conversion");
        BuildRegression.Fixture.Write(Path.Combine(source, "broken.mdx"), "invalid model");
        Check.That((await Dotnet(executable, source)).ExitCode == 1, "conversion failure reaches exit code");
    }
}

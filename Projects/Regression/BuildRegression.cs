using System.Diagnostics;
using System.Reflection;
using System.Xml.Linq;
using FastMDX;
using War3FrameBuild;
using War3FrameBuild.CommandManager;
using War3FrameBuild.Execution;
using War3FrameBuild.Extension;

namespace War3Frame.Regression;

internal static class BuildRegression
{
    internal static string Dotnet => Environment.GetEnvironmentVariable("DOTNET_HOST_PATH")
        ?? (Path.GetFileNameWithoutExtension(Environment.ProcessPath) == "dotnet" ? Environment.ProcessPath! : "dotnet");

    internal static void Register(List<RegressionCase> tests)
    {
        tests.Add(new("build", "process-output-exit-timeout", ProcessExecution));
        tests.Add(new("build", "pipeline-failure-and-artifact-boundaries", Pipeline));
        tests.Add(new("build", "file-set-and-we-markers", SyncFiles));
        tests.Add(new("build", "config-and-bounded-multi", ConfigurationAndMulti));
        tests.Add(new("build", "asset-parameters-and-sdk-input", AssetCompilation));
        tests.Add(new("build", "loading-validation-and-ini", Loading));
        tests.Add(new("build", "buffer-capacity-and-model-roundtrip", Buffer));
        tests.Add(new("build", "artifact-promotion-rollback", PromotionRollback));
    }

    private static void PromotionRollback()
    {
        using var fixture = new Fixture();
        var destination = Path.Combine(fixture.Root, "published");
        Fixture.Write(Path.Combine(destination, "project.dll"), "old module");
        using var transaction = new BuildWorkspace(destination, false);
        Fixture.Write(Path.Combine(transaction.WorkingDirectory, "project.dll"), "new module");
        var map = Path.Combine(fixture.Root, "staged.w3x");
        Fixture.Write(map, "new map");
        var invalidDestination = Path.Combine(fixture.Root, "destination-is-directory");
        Directory.CreateDirectory(invalidDestination);
        try { transaction.Commit(map, invalidDestination); throw new Exception("invalid map destination accepted"); }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException) { }
        Check.That(File.ReadAllText(Path.Combine(destination, "project.dll")) == "old module", "map publish failure rolls modules back");
        Check.That(File.Exists(map), "failed file promotion retains staged map until owner cleanup");
    }

    private static async Task ProcessExecution()
    {
        var runner = new ProcessRunner();
        ProcessRequest Child(string name, double seconds = 10) => new(Dotnet, [typeof(Program).Assembly.Location, "--child", name], Timeout: TimeSpan.FromSeconds(seconds));
        var flood = await runner.RunAsync(Child("flood"));
        Check.That(flood.Success && flood.Output.EndsWith("OUT-END") && flood.Error.EndsWith("ERR-END"), "both redirected pipes drained");
        Check.That(flood.Output.Length == ProcessRunner.MaxOutputChars && flood.Error.Length == ProcessRunner.MaxOutputChars, "bounded diagnostic tails");
        var failed = await runner.RunAsync(Child("failure"));
        Check.That(!failed.Success && failed.ExitCode == 17 && failed.Error.Contains("controlled failure"), "nonzero exit preserved");
        var watch = Stopwatch.StartNew();
        var timeout = await runner.RunAsync(Child("wait", .3));
        Check.That(timeout.Canceled && !timeout.Success && watch.Elapsed < TimeSpan.FromSeconds(6), "timeout terminates owned child");
        using var cancel = new CancellationTokenSource(300);
        Check.That((await runner.RunAsync(Child("wait"), cancel.Token)).Canceled, "caller cancellation propagated");
        var missing = await runner.RunAsync(new ProcessRequest(Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".exe"), []));
        Check.That(!missing.Success && !missing.Canceled && missing.ExitCode == null, "start failure distinguished from cancellation");
    }

    private static async Task Pipeline()
    {
        foreach (var failure in new[] { "prepare", "publish", "artifact", "pack", "none" })
        {
            using var fixture = new Fixture();
            var manager = fixture.Manager;
            var map = Path.Combine(fixture.Config.War3, "Maps", "Test", "sample.w3x");
            Fixture.Write(map, "old valid map");
            var oldModule = Path.Combine(manager.BuildDstPath, "map", "project.dll");
            Fixture.Write(oldModule, "old module");
            var plugin = Path.Combine(fixture.Config.War3, "version.dll");
            Fixture.Write(plugin, "existing plugin");
            var calls = new List<ProcessRequest>();
            manager.ProcessRunner = new FakeRunner(request =>
            {
                calls.Add(request);
                if (request.Arguments[0] == "publish")
                {
                    if (failure == "publish") return new(7, "", "publish failed");
                    if (failure != "artifact")
                        foreach (var name in new[] { "project.dll", "project.deps.json", "project.runtimeconfig.json" })
                            Fixture.Write(Path.Combine(request.Arguments[request.Arguments.ToList().IndexOf("-o") + 1], name), "published");
                    return new(0, "", "");
                }
                Fixture.Write(request.Arguments[2], "new packed map");
                return new(failure == "pack" ? 9 : 0, "", "");
            });
            if (failure == "prepare") File.Delete(Path.Combine(manager.Template, "callback"));
            var success = await manager.Run(false, true);
            Check.That(success == (failure == "none"), "pipeline result for " + failure);
            var expectedCalls = failure == "prepare" ? 0 : failure is "publish" or "artifact" ? 1 : 2;
            Check.That(calls.Count == expectedCalls, "later stages stopped for " + failure);
            Check.That(File.ReadAllText(map) == (success ? "new packed map" : "old valid map"), "final map preserved until pack succeeds");
            Check.That(File.ReadAllText(plugin) == "existing plugin", "noTest does not remove game configuration");
            Check.That(File.ReadAllText(oldModule) == (success ? "published" : "old module"), "failed build preserves previous external JIT module");
            if (success)
            {
                var callback = File.ReadAllText(Path.Combine(manager.BuildDstPath, "map", "callback"));
                Check.That(callback.Contains(Path.GetFullPath(Path.Combine(manager.BuildDstPath, "map")).Replace("\\", "\\\\")), "callback still points to stable build directory");
            }
        }
    }

    private static void SyncFiles()
    {
        using var fixture = new Fixture();
        var source = Path.Combine(fixture.Manager.PwdProject, "w3x", "table");
        Fixture.Write(Path.Combine(source, "unit.ini"), "old");
        Fixture.Write(Path.Combine(source, "delete.ini"), "delete");
        foreach (var overlap in new[] { source, Path.GetDirectoryName(source)!, Path.Combine(source, "nested") })
        {
            try { DirectoryExtensions.CopyDir(source, overlap); throw new Exception("overlapping copy accepted"); }
            catch (ArgumentException) { }
            try { DirectoryExtensions.SyncDirectory(source, overlap); throw new Exception("overlapping sync accepted"); }
            catch (ArgumentException) { }
        }
        fixture.Manager.ProjectResourceToTemp();
        var before = Directory.GetLastWriteTimeUtc(source);
        Fixture.Write(Path.Combine(source, "unit.ini"), "modified");
        File.Delete(Path.Combine(source, "delete.ini"));
        Directory.SetLastWriteTimeUtc(source, before);
        fixture.Manager.ProjectResourceToTemp();
        var target = Path.Combine(fixture.Manager.TempProjectBuildPath, "table");
        Check.That(File.ReadAllText(Path.Combine(target, "unit.ini")) == "modified" && !File.Exists(Path.Combine(target, "delete.ini")), "file content and deletion synced despite unchanged directory timestamp");
        var marker = Path.Combine(fixture.Manager.TempProjectBuildPath, ".we");
        Fixture.Write(marker, "pending");
        fixture.Manager.ProcessRunner = new FakeRunner(_ => new(1, "", "unpack failed"));
        try { fixture.Manager.SyncW3xFile(false); throw new Exception("failed unpack accepted"); }
        catch (InvalidOperationException) { }
        Check.That(File.Exists(marker), "failed unpack retains marker");
        fixture.Manager.ProcessRunner = new FakeRunner(_ => new(0, "", ""));
        fixture.Manager.SyncW3xFile(false);
        Check.That(!File.Exists(marker), "successful unpack and back-sync consumes marker");
        Fixture.Write(marker, "pending");
        Directory.Delete(Path.Combine(fixture.Manager.TempProjectBuildPath, "resource"), true);
        try { fixture.Manager.SyncW3xFile(false); throw new Exception("failed back-sync accepted"); }
        catch (DirectoryNotFoundException) { }
        Check.That(File.Exists(marker), "failed back-sync retains marker");
    }

    private static async Task ConfigurationAndMulti()
    {
        using var fixture = new Fixture();
        Check.That(ApplicationBuilderExtensions.ValidateConfig(fixture.Config, out _), "complete config accepted");
        File.Move(Path.Combine(fixture.Config.We, "WE.exe"), Path.Combine(fixture.Config.We, "KKWE.exe"));
        Check.That(ApplicationBuilderExtensions.ValidateConfig(fixture.Config, out _), "KKWE alternative accepted");
        var launcher = Path.Combine(fixture.Config.We, "bin", "YDWEConfig.exe");
        File.Delete(launcher);
        Check.That(!ApplicationBuilderExtensions.ValidateConfig(fixture.Config, out _), "missing launcher rejected");
        Fixture.Write(launcher, "launcher");
        var calls = 0;
        fixture.Manager.LaunchInterval = TimeSpan.Zero;
        fixture.Manager.ProcessRunner = new FakeRunner(_ => { calls++; return new(0, "", ""); });
        Check.That(!await fixture.Manager.LaunchAdditionalAsync(0) && calls == 0, "invalid count never launches");
        Check.That(await fixture.Manager.LaunchAdditionalAsync(3) && calls == 3, "exactly three extra launch attempts");
        fixture.Manager.ProcessRunner = new FakeRunner(_ => { calls++; return new(1, "", "fail"); });
        Check.That(!await fixture.Manager.LaunchAdditionalAsync(5) && calls == 4, "first launch failure stops batch");
    }

    private static async Task AssetCompilation()
    {
        using var fixture = new Fixture();
        fixture.Manager.War3SoundsYaml = new CommandManager.War3Sounds
        { sounds = [new CommandManager.SoundItem { path = "Sound\\Interface\\Error.wav", duration = 616 }] };
        var source = Path.Combine(fixture.Manager.PwdProject, "Assets", "Manifest.cs");
        const string content = """
            using War3Frame;
            public static class SampleManifest {
                private static int order;
                private static int Volume() => ++order;
                private static string Alias() => "ordered-" + ++order;
                public static Assets Create() {
                    var a = new Assets();
                    a.AddModel("units\\Human\\Footman\\Footman.mdx");
                    a.AddV3D(path: "Sound\\Interface\\Error.wav", alias: null);
                    a.AddVCM(volume: 41, alias: "named", path: "Sound\\Interface\\Error.wav");
                    a.AddVCM(volume: Volume(), alias: Alias(), path: "Sound\\Interface\\Error.wav");
                    return a;
                }
            }
            """;
        Fixture.Write(source, content);
        fixture.Manager.SupplementAssetsPackPath([source]);
        var generated = Path.Combine(fixture.Manager.BuildDstPath, "generated", "Assets", "Manifest.cs");
        var transformed = File.ReadAllText(generated);
        fixture.Manager.SupplementAssetsPackPath([source]);
        Check.That(File.ReadAllText(source) == content && File.ReadAllText(generated) == transformed, "source immutable, repeated output stable");
        // 也验证已转换参数再次输入时不会丢失 volume、重复追加 pickPath 或 duration。
        Fixture.Write(source, transformed);
        fixture.Manager.SupplementAssetsPackPath([source]);
        Check.That(File.ReadAllText(generated) == transformed, "conversion idempotent");
        Fixture.Write(source, content);
        var result = await new ProcessRunner().RunAsync(new ProcessRequest(Dotnet,
            ["build", Path.Combine(fixture.Manager.PwdProject, "sample.csproj"), "-c", "Release", "-v", "quiet",
             "-p:CustomBeforeDirectoryBuildTargets=" + fixture.Manager.GeneratedAssetsTargetsPath], Timeout: TimeSpan.FromMinutes(2)));
        Check.That(result.Success, "real MSBuild compile-input replacement: " + result.Output + result.Error);
        var dll = Path.Combine(fixture.Manager.PwdProject, "bin", "Release", "net10.0", "project.dll");
        var assembly = Assembly.Load(File.ReadAllBytes(dll));
        var assets = (Assets)assembly.GetType("SampleManifest")!.GetMethod("Create")!.Invoke(null, null)!;
        Check.That(assets.Models.Values.Single().pickPath!.EndsWith(".mdl"), "generated model path used by compiler");
        Check.That(assets.V3dSounds.Values.Single() is { volume: 127, duration: 616 }, "omitted numeric volume keeps API default");
        Check.That(assets.VcmSounds["named"] is { volume: 41, duration: 616 }, "named volume retained");
        Check.That(assets.VcmSounds["ordered-2"].volume == 1, "named arguments retain source evaluation order");
        Check.That(File.ReadAllText(source) == content, "SDK build leaves source unchanged");
    }

    private static void Loading()
    {
        using var fixture = new Fixture();
        var manager = fixture.Manager;
        var ini = Path.Combine(manager.BuildDstPath, "table", "w3i.ini");
        Fixture.Write(ini, "[载入图]\n路径=old\n");
        var directory = Path.Combine(fixture.Config.Assets, "war3MapLoading", "parts");
        Fixture.Write(Path.Combine(directory, "pic.tga"), "image");
        var destination = Path.Combine(manager.BuildDstPath, "resource", "Framework", "LoadingScreen.mdx");
        Fixture.Write(destination, "old model");
        try { manager.InstallLoading("parts"); throw new Exception("incomplete loading accepted"); }
        catch (FileNotFoundException) { }
        Check.That(File.ReadAllText(destination) == "old model" && File.ReadAllText(ini).Contains("old"), "validate all loading inputs before writing");
        Fixture.Write(Path.Combine(directory, "bc.tga"), "image");
        Fixture.Write(Path.Combine(directory, "bg.tga"), "image");
        Fixture.Write(Path.Combine(manager.Template, "lni", "assets", "LoadingScreenDir.mdx"), "model");
        manager.InstallLoading("parts");
        Check.That(File.ReadAllText(ini).Contains("Framework\\LoadingScreen.mdx"), "loading path saved to target INI");
    }

    private static unsafe void Buffer()
    {
        var stream = new DataStream(1);
        for (var i = 0; i < 10000; i++) stream.WriteStruct(i);
        Check.That(stream.Offset == 40000 && stream.Size >= stream.Offset && stream.Size <= 65536, "capacity doubles within bounds");
        Check.That(((int*)stream.Pointer)[9999] == 9999 && ((int*)stream.Pointer)[0] == 0, "realloc retains bytes");
        stream.Dispose(); stream.Dispose();
        Check.That(stream.Size == 0 && stream.Pointer == null, "idempotent release resets accounting");
        try { stream.WriteStruct(1); throw new Exception("disposed write accepted"); } catch (ObjectDisposedException) { }
        using var zero = new DataStream(0);
        zero.WriteStruct(23);
        Check.That(zero.Offset == 4 && zero.Size >= 4, "zero initial capacity grows");
        var model = new MDX();
        using var memory = new MemoryStream();
        model.SaveTo(memory); memory.Position = 0;
        _ = new MDX(memory);
        using var truncated = new MemoryStream(memory.ToArray()[..^1]);
        try { _ = new MDX(truncated, (uint)memory.Length); throw new Exception("truncated model accepted"); }
        catch (EndOfStreamException) { }
    }

    internal sealed class FakeRunner(Func<ProcessRequest, ProcessResult> run) : IProcessRunner
    {
        public Task<ProcessResult> RunAsync(ProcessRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult(run(request));
    }

    internal sealed class Fixture : IDisposable
    {
        internal readonly string Root = Path.Combine(Path.GetTempPath(), "War3Frame-regression-" + Guid.NewGuid().ToString("N"));
        internal ConfigPath Config { get; }
        internal CommandManager Manager { get; }
        internal Fixture()
        {
            Config = new ConfigPath { Pwd = Root, War3 = Path.Combine(Root, "game"), We = Path.Combine(Root, "we"), W3x2lni = Path.Combine(Root, "w2l"), Assets = Path.Combine(Root, "assets") };
            foreach (var directory in new[] { Config.Pwd, Config.War3, Config.We, Config.W3x2lni, Config.Assets }) Directory.CreateDirectory(directory);
            Manager = new CommandManager(Config, "sample", BuildModeEnum.Build);
            foreach (var file in new[] { Path.Combine(Config.We, "WE.exe"), Path.Combine(Config.We, "bin", "YDWEConfig.exe"), Path.Combine(Config.War3, "war3.exe"), Path.Combine(Config.W3x2lni, "w2l.exe") }) Write(file, "fixture");
            foreach (var directory in new[] { "map", "table", "resource" }) Directory.CreateDirectory(Path.Combine(Manager.PwdProject, "w3x", directory));
            foreach (var directory in new[] { "w3x2lni", "resource", "assets/UI" }) Directory.CreateDirectory(Path.Combine(Manager.Template, "lni", directory));
            Write(Path.Combine(Manager.Template, "callback"), "string ModulePath = \"old\"\nstring ModuleName = \"old\"\nbool IsNative = false\n");
            Write(Path.Combine(Manager.Template, "lni", "x.w3x"), "lni marker");
            foreach (var file in new[] { "BridgeToJIT.dll", "BridgeToJIT.runtimeconfig.json", "BridgeToJIT.deps.json", "Ijwhost.dll" }) Write(Path.Combine(Manager.Template, file), "bridge");
            var project = new XElement("Project", new XAttribute("Sdk", "Microsoft.NET.Sdk"),
                new XElement("PropertyGroup", new XElement("TargetFramework", "net10.0"), new XElement("AssemblyName", "project")),
                new XElement("ItemGroup", new XElement("Reference", new XAttribute("Include", "War3Frame"), new XElement("HintPath", typeof(Assets).Assembly.Location))));
            Write(Path.Combine(Manager.PwdProject, "sample.csproj"), project.ToString());
        }
        internal static void Write(string path, string content) { Directory.CreateDirectory(Path.GetDirectoryName(path)!); File.WriteAllText(path, content); }
        public void Dispose()
        {
            var full = Path.GetFullPath(Root);
            Check.That(full.StartsWith(Path.GetFullPath(Path.GetTempPath()), StringComparison.OrdinalIgnoreCase)
                && Path.GetFileName(full).StartsWith("War3Frame-regression-"), "cleanup remains within owned temp fixture");
            if (Directory.Exists(full)) Directory.Delete(full, true);
        }
    }
}

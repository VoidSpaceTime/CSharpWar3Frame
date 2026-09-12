using System.Diagnostics;
using System.Text.RegularExpressions;
using Serilog;
using War3FrameBuild.Execution;
using War3FrameBuild.Extension;

namespace War3FrameBuild.CommandManager;

public partial class CommandManager
{
    private static readonly Regex ModulePathRegex = new(@"string ModulePath = .*", RegexOptions.Compiled);
    private static readonly Regex ModuleNameRegex = new(@"string ModuleName = .*", RegexOptions.Compiled);
    private static readonly Regex IsNativeRegex = new(@"bool IsNative = .*", RegexOptions.Compiled);

    internal async Task<bool> ExecuteAsync(ProcessRequest request, CancellationToken cancellationToken = default)
    {
        var result = await ProcessRunner.RunAsync(request, cancellationToken).ConfigureAwait(false);
        if (!string.IsNullOrWhiteSpace(result.Output)) Log.Debug("{Output}", result.Output);
        if (!result.Success) Log.Error("{Tool} 失败，exit={ExitCode}: {Error}", request.FileName, result.ExitCode, result.Error);
        else if (!string.IsNullOrWhiteSpace(result.Error)) Log.Warning("{Error}", result.Error);
        return result.Success;
    }

    private bool StartW3XToLni(string[] args)
        => ExecuteAsync(new ProcessRequest(Path.Combine(Config.W3x2lni, "w2l.exe"), args)).GetAwaiter().GetResult();

    internal async Task<bool> PackMapAsync(string source, string destination, string mode, CancellationToken cancellationToken = default)
    {
        var staging = await CreatePackedMapAsync(source, destination, mode, cancellationToken).ConfigureAwait(false);
        if (staging == null) return false;
        try { File.Move(staging, destination, overwrite: true); return true; }
        finally { if (File.Exists(staging)) File.Delete(staging); }
    }

    private async Task<string?> CreatePackedMapAsync(string source, string destination, string mode, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
        // 最终地图同目录暂存；工具失败或未生成产物时，既有地图保持可用。
        var staging = Path.Combine(Path.GetDirectoryName(destination)!, Path.GetFileNameWithoutExtension(destination) + "." + Guid.NewGuid().ToString("N") + ".w3x");
        var ready = false;
        try
        {
            if (!await ExecuteAsync(new ProcessRequest(Path.Combine(Config.W3x2lni, "w2l.exe"),
                    [mode, source, staging, "-ydwe", Config.We]), cancellationToken).ConfigureAwait(false)) return null;
            if (!File.Exists(staging) || new FileInfo(staging).Length == 0)
            {
                Log.Error("打包工具未生成有效地图: {Path}", staging);
                return null;
            }
            ready = true;
            return staging;
        }
        finally { if (!ready && File.Exists(staging)) File.Delete(staging); }
    }

    private bool PrepareMap(bool isCache)
    {
        if (string.IsNullOrWhiteSpace(ProjectName) || !File.Exists(Path.Combine(PwdProject, ProjectName + ".csproj")))
            throw new FileNotFoundException("项目文件不存在", PwdProject);
        SyncW3xFile(isCache);
        if (!isCache)
        {
            Directory.CreateDirectory(WorkingBuildPath);
            foreach (var folder in new[] { "map", "table", "resource" })
                DirectoryExtensions.SyncDirectory(Path.Combine(TempProjectBuildPath, folder), Path.Combine(WorkingBuildPath, folder));
            DirectoryExtensions.CopyDir(Path.Combine(TempProjectBuildPath, "w3x2lni"), Path.Combine(WorkingBuildPath, "w3x2lni"));
            File.Copy(Path.Combine(TempProjectBuildPath, ".w3x"), Path.Combine(WorkingBuildPath, ".w3x"), true);
            DirectoryExtensions.CopyDir(Path.Combine(Template, "lni", "assets", "UI"), Path.Combine(WorkingBuildPath, "map", "UI"));
        }
        // Callback 依赖绝对构建目录，缓存构建也必须核对当前模式和路径。
        if (!ProcessCallback()) return false;
        var assets = Path.Combine(PwdProject, "Assets");
        SupplementAssetsPackPath(Directory.Exists(assets) ? Directory.GetFiles(assets, "*.cs", SearchOption.AllDirectories) : []);
        return true;
    }

    private bool ProcessCallback()
    {
        var mapDir = Path.Combine(WorkingBuildPath, "map");
        var source = Path.Combine(PwdProject, "w3x", "map", "callback");
        if (!File.Exists(source)) source = Path.Combine(Template, "callback");
        if (!File.Exists(source)) { Log.Error("Callback文件丢失"); return false; }
        var content = File.ReadAllText(source);
        if (!ModulePathRegex.IsMatch(content) || !ModuleNameRegex.IsMatch(content) || !IsNativeRegex.IsMatch(content))
            throw new InvalidDataException("Callback 缺少模块路径、名称或模式声明");
        var escapedPath = Path.GetFullPath(Path.Combine(BuildDstPath, "map")).Replace("\\", "\\\\");
        content = ModulePathRegex.Replace(content, _ => "string ModulePath = \"" + escapedPath + "\"");
        content = ModuleNameRegex.Replace(content, _ => "string ModuleName = \"" + (BuildMode == BuildModeEnum.Release ? "project.dll" : "BridgeToJIT.dll") + "\"");
        content = IsNativeRegex.Replace(content, _ => "bool IsNative = " + (BuildMode == BuildModeEnum.Release ? "true" : "false"));
        Directory.CreateDirectory(mapDir);
        File.WriteAllText(Path.Combine(mapDir, "callback"), content);
        return true;
    }

    private async Task<bool> PublishProject(CancellationToken cancellationToken)
    {
        var mapDir = Path.Combine(WorkingBuildPath, "map");
        var isNative = BuildMode == BuildModeEnum.Release;
        var arguments = new List<string> { "publish", Path.Combine(PwdProject, ProjectName + ".csproj"), "-c", "Release", "-r", "win-x86", "-o", mapDir };
        arguments.Add("-p:CustomBeforeDirectoryBuildTargets=" + GeneratedAssetsTargetsPath);
        if (isNative) arguments.AddRange(["--self-contained", "true", "-p:PublishAot=true", "-p:DebugType=None", "-p:DebugSymbols=false"]);
        else arguments.AddRange(["--self-contained", "false", "-p:PublishAot=false"]);
        // 删除旧 payload，防止工具成功退出但未输出时把旧 DLL 判成新产物。
        foreach (var name in new[] { "project.dll", "project.deps.json", "project.runtimeconfig.json" })
        {
            var path = Path.Combine(mapDir, name);
            if (File.Exists(path)) File.Delete(path);
        }
        var dotnet = Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") ?? "dotnet";
        if (!await ExecuteAsync(new ProcessRequest(dotnet, arguments, Config.Pwd, TimeSpan.FromMinutes(15)), cancellationToken).ConfigureAwait(false)) return false;
        var required = isNative ? new[] { "project.dll" } : new[] { "project.dll", "project.deps.json", "project.runtimeconfig.json" };
        foreach (var file in required)
            if (!File.Exists(Path.Combine(mapDir, file)) || new FileInfo(Path.Combine(mapDir, file)).Length == 0)
            { Log.Error("Publish 产物缺失: {File}", file); return false; }
        if (!isNative)
        {
            var bridgeBuildDir = Path.Combine(Config.Pwd, "BridgeToJIT", ".build", "Debug");
            var bridgeFiles = new[] { "BridgeToJIT.dll", "BridgeToJIT.runtimeconfig.json", "BridgeToJIT.deps.json", "Ijwhost.dll" };
            var source = bridgeFiles.All(f => File.Exists(Path.Combine(bridgeBuildDir, f))) ? bridgeBuildDir : Template;
            if (bridgeFiles.Any(f => !File.Exists(Path.Combine(source, f))))
            { Log.Error("完整 BridgeToJIT 产物未找到: {Path}", source); return false; }
            foreach (var file in bridgeFiles) File.Copy(Path.Combine(source, file), Path.Combine(mapDir, file), true);
        }
        return true;
    }

    /// <summary>额外启动指定数量的客户端，每次启动只执行一次；不会结束已有客户端。</summary>
    public async Task<bool> LaunchAdditionalAsync(int count, string? map = null, CancellationToken cancellationToken = default)
    {
        if (count <= 0) { Log.Error("多开数量必须大于 0"); return false; }
        var launcher = Path.Combine(Config.We, "bin", "YDWEConfig.exe");
        var game = Path.Combine(Config.War3, "war3.exe");
        if (!File.Exists(launcher) || !File.Exists(game) || map != null && !File.Exists(map))
        { Log.Error("启动器、游戏或地图文件不存在"); return false; }
        for (var i = 0; i < count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var args = new List<string> { "-launchwar3", game };
            if (map != null) args.AddRange(["-loadfile", map]);
            if (!await ExecuteAsync(new ProcessRequest(launcher, args, Config.We, TimeSpan.FromSeconds(30)), cancellationToken).ConfigureAwait(false)) return false;
            if (i + 1 < count) await Task.Delay(LaunchInterval, cancellationToken).ConfigureAwait(false);
        }
        return true;
    }

    public async Task<bool> Run(bool isCache, bool noTest, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            // 校验工作目录位于配置的临时根内，再创建独立工作区。
            ResourcePath(Temp, Path.GetRelativePath(Temp, BuildDstPath));
            using var workspace = new BuildWorkspace(BuildDstPath, isCache);
            _workingBuildPath = workspace.WorkingDirectory;
            if (!PrepareMap(isCache)) return false;
            if (!await PublishProject(cancellationToken).ConfigureAwait(false)) return false;
            var map = Path.Combine(Config.War3, "Maps", "Test", ProjectName + ".w3x");
            var stagedMap = await CreatePackedMapAsync(WorkingBuildPath, map, BuildMode == BuildModeEnum.Release ? "slk" : "obj", cancellationToken).ConfigureAwait(false);
            if (stagedMap == null) return false;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                workspace.Commit(stagedMap, map);
            }
            finally { if (File.Exists(stagedMap)) File.Delete(stagedMap); }
            return noTest || await LaunchAdditionalAsync(1, map, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "构建/运行失败: {Message}", exception.Message);
            return false;
        }
        finally { _workingBuildPath = null; }
    }
}

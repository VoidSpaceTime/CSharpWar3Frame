using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Serilog;
using War3FrameBuild.Extension;

namespace War3FrameBuild.CommandManager;

public partial class CommandManager
{
    // 预编译正则表达式以提高性能
    private static readonly Regex ModulePathRegex = new(@"string ModulePath = .*", RegexOptions.Compiled);
    private static readonly Regex ModuleNameRegex = new(@"string ModuleName = .*", RegexOptions.Compiled);

    /// <summary>
    ///     运行war3 进行测试
    /// </summary>
    /// <param name="w3xFire"></param>
    /// <param name="qty"></param>
    private bool RunTest(string w3xFire, int qty)
    {
        Log.Information("启动魔兽争霸III");
        var psi = new ProcessStartInfo
        {
            FileName = Path.Combine(Config.We, "bin", "YDWEConfig.exe"),
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        psi.ArgumentList.Add("-launchwar3");
        psi.ArgumentList.Add("-loadfile");
        psi.ArgumentList.Add(w3xFire);

        // 检查必要文件是否存在
        if (!File.Exists(Path.Combine(Config.We, "bin", "YDWEConfig.exe")))
        {
            Log.Error("YDWEConfig.exe 不存在，请检查 YDWE 安装路径");
            return false;
        }

        if (!File.Exists(w3xFire) && !Directory.Exists(w3xFire))
        {
            Log.Error($"地图文件不存在: {w3xFire}");
            return false;
        }

        try
        {
            using var war3Psi = Process.Start(psi);
            war3Psi?.WaitForExit();
        }
        catch
        {
            Log.Warning("war3 启动失败,正在尝试重启");
        }

        // 精确（不区分大小写）
        var war3Count = Process.GetProcesses()
            .Count(p => string.Equals(p.ProcessName, "war3", StringComparison.OrdinalIgnoreCase));
        if (war3Count > 0)
        {
            Log.Information("检测到魔兽争霸III已运行");
            if (BuildMode is BuildModeEnum.Test or BuildModeEnum.Build)
            {
                // 热更  暂未完成
                //Hot();
            }

            return true;
        }

        if (qty > 3)
        {
            Log.Error("启动魔兽争霸III失败，请检查环境");
            return false;
        }

        Log.Warning($"未检测到魔兽争霸III运行，等待1秒后重试启动（第{qty}次尝试）");
        return RunTest(w3xFire, qty + 1);
    }

    /// <summary>
    ///     通过w2l 打包地图
    /// </summary>
    /// <param name="modeLni"></param>
    /// <param name="dstW3xFire"></param>
    private bool PackupMap(string modeLni, string dstW3xFire)
    {
        if (File.Exists(dstW3xFire))
            File.Delete(dstW3xFire);

        // 打包地图
        Log.Verbose("开始打包地图");
        var startTime = DateTime.Now;
        var w2lProc = CreateW2lProcessInfo();
        w2lProc.ArgumentList.Add(modeLni);
        w2lProc.ArgumentList.Add(BuildDstPath);
        w2lProc.ArgumentList.Add(dstW3xFire);

        using var w2l = new Process { StartInfo = w2lProc, EnableRaisingEvents = true };

        if (!w2l.Start())
        {
            Log.Error("w2l 进程启动失败");
            return false;
        }

        w2l.WaitForExit();
        if (w2l.ExitCode != 0)
        {
            var errorOutput = w2l.StandardError.ReadToEnd();
            Log.Error($"打包地图失败，退出码: {w2l.ExitCode}");
            if (!string.IsNullOrWhiteSpace(errorOutput))
                Log.Error($"w2l 打包错误: {errorOutput}");

            return false;
        }

        if (!File.Exists(dstW3xFire))
        {
            Log.Error($"打包地图失败，目标文件未生成: {dstW3xFire}");
            return false;
        }

        Log.Debug($"打包地图，路径：{dstW3xFire}");
        Log.Verbose($"打包地图，耗时：{(DateTime.Now - startTime).TotalSeconds.ToString()}");
        return true;
    }

    /// <summary>
    ///     创建 w2l 进程启动信息
    /// </summary>
    private ProcessStartInfo CreateW2lProcessInfo()
    {
        return new ProcessStartInfo
        {
            FileName = Path.Combine(Config.W3x2lni, "w2l.exe"),
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
    }

    /// <summary>
    ///     处理 callback 文件内容替换
    /// </summary>
    /// <param name="sourceFile">源文件路径</param>
    /// <param name="destFile">目标文件路径（可与源文件相同）</param>
    /// <param name="modulePath">模块路径</param>
    /// <param name="dllName">DLL 名称</param>
    private void ProcessCallbackFile(string sourceFile, string destFile, string modulePath, string dllName)
    {
        var content = File.ReadAllText(sourceFile);
        var escapedPath = modulePath.Replace("\\", "/").Replace("/", "\\\\");

        content = ModulePathRegex.Replace(content, $"string ModulePath = \"{escapedPath}\"");
        content = ModuleNameRegex.Replace(content, $"string ModuleName = \"{dllName}.dll\"");

        File.WriteAllText(destFile, content);
    }

    /// <summary>
    ///     清理构建目录中的特定文件和文件夹
    /// </summary>
    private void CleanBuildDirectory()
    {
        var weFile = Path.Combine(BuildDstPath, ".we");
        var mapDir = Path.Combine(BuildDstPath, "map");
        var tableDir = Path.Combine(BuildDstPath, "table");

        if (File.Exists(weFile)) File.Delete(weFile);
        if (Directory.Exists(mapDir)) Directory.Delete(mapDir, true);
        if (Directory.Exists(tableDir)) Directory.Delete(tableDir, true);
    }

    /// <summary>
    ///     构建地图
    /// </summary>
    /// <param name="isCache"></param>
    /// <param name="noTest"></param>
    /// <returns></returns>
    private async Task<bool> BuildMap(bool isCache, bool noTest)
    {
        var temProjectDir = Path.Combine(Temp, ProjectName);
        var temProjectW3xFile = Path.Combine(Temp, $"{ProjectName}.w3x");
        var buoyFile = Path.Combine(temProjectDir, ".we");

        // 同步地图文件
        if (!await SyncMapFilesAsync(temProjectW3xFile, buoyFile, isCache))
            return false;

        if (!isCache)
        {
            // 准备构建目录
            PrepareDirectory();

            // 复制项目文件
            var mapDir = Path.Combine(BuildDstPath, "map");
            DirectoryExtensions.CopyDir(temProjectDir, BuildDstPath);
            DirectoryExtensions.CopyDir(Path.Combine(Template, "lni", "assets", "UI"), Path.Combine(mapDir, "UI"));

            // 处理 callback 文件
            if (!ProcessCallback(mapDir))
                return false;

            Log.Verbose($"构建地图完毕：{BuildMode}");
        }

        // 处理资源文件
        var startTime = DateTime.Now;
        var assetFiles = Directory.GetFiles(Path.Combine(PwdProject, "Assets"))
            .Where(p => Path.GetExtension(p).Equals(".cs", StringComparison.OrdinalIgnoreCase))
            .ToArray();
        SupplementAssetsPackPath(assetFiles);
        Log.Verbose($"资源及代码处理完成，耗时：{(DateTime.Now - startTime).TotalSeconds:F2}s");

        return true;
    }

    /// <summary>
    ///     同步地图文件（检测 WE 保存状态）
    /// </summary>
    private async Task<bool> SyncMapFilesAsync(string mapFile, string buoyFile, bool isCache)
    {
        var mapWriteTime = File.GetLastWriteTime(mapFile);
        var buoyWriteTime = File.GetLastWriteTime(buoyFile);

        if (mapWriteTime > buoyWriteTime)
        {
            // 地图文件较新，需要拆包同步
            var w2lProc = CreateW2lProcessInfo();
            w2lProc.ArgumentList.Add("lni");
            w2lProc.ArgumentList.Add(mapFile);

            using var w2l = new Process { StartInfo = w2lProc, EnableRaisingEvents = true };
            if (!w2l.Start())
            {
                Log.Error("w2l 进程启动失败");
                return false;
            }

            await w2l.WaitForExitAsync();
            if (w2l.ExitCode != 0)
            {
                var errorOutput = await w2l.StandardError.ReadToEndAsync();
                Log.Warning($"w2l 执行警告 (ExitCode={w2l.ExitCode}): {errorOutput}");
            }

            File.Delete(buoyFile);
            File.Copy(Path.Combine(Template, "lni", "x.we"), buoyFile);
            Backup();
            Log.Information("同步完毕[检测到有新的地图保存行为，以'WE'为主版本]");
        }
        else if (!isCache)
        {
            Pickup();
            Log.Information("同步完毕[检测到没有新的地图保存行为，以'project'为主版本]");
        }

        return true;
    }

    /// <summary>
    ///     准备构建目录
    /// </summary>
    private void PrepareDirectory()
    {
        if (BuildMode is BuildModeEnum.Release)
        {
            Log.Debug("准备发布打包");
            if (Directory.Exists(BuildDstPath))
                Directory.Delete(BuildDstPath, true);
        }
        else
        {
            // 非 release 模式采用增量覆盖
            CleanBuildDirectory();
        }
    }

    /// <summary>
    ///     处理 callback 文件
    /// </summary>
    private bool ProcessCallback(string mapDir)
    {
        var callbackInBuild = Path.Combine(mapDir, "callback");
        var callbackInTemplate = Path.Combine(Template, "callback");
        var dllName = BuildMode is BuildModeEnum.Release ? ProjectName : $"{ProjectName}NE";

        string sourceFile;
        if (File.Exists(callbackInBuild))
        {
            sourceFile = callbackInBuild;
        }
        else if (File.Exists(callbackInTemplate))
        {
            sourceFile = callbackInTemplate;
        }
        else
        {
            Log.Error("CallBack 文件丢失");
            return false;
        }

        ProcessCallbackFile(sourceFile, callbackInBuild, mapDir, dllName);
        return true;
    }

    /// <summary>
    ///     脚本文件打包成dll
    /// </summary>
    /// <param name="isNative"></param>
    /// <param name="projectsPath"></param>
    /// <param name="publishDir"></param>
    /// <returns></returns>
    private async Task<bool> PublishProject(bool isNative, string projectsPath, string publishDir)
    {
        /*// 目前只会 AOT 打包
        isNative = true;*/
        // -p:PublishTrimmed=false -p:DebugType=None -p:DebugSymbols=false -p:PublishSingleFile=true --self-contained true
        var aotCommand = isNative ? " -p:PublishAot=true -p:DebugType=None -p:DebugSymbols=false " : "";
        var command = @$"publish {projectsPath} -c Release -r win-x86  {aotCommand}  -o {publishDir}";

        var psi = new ProcessStartInfo("dotnet", command)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        Log.Debug($"准备执行 dotnet publish，输出目录: {publishDir}");

        using var proc = new Process { StartInfo = psi, EnableRaisingEvents = true };
        // 异步读取输出，避免子进程因输出缓冲区满而阻塞
        var stdoutSb = new StringBuilder();
        var stderrSb = new StringBuilder();
        try
        {
            if (!proc.Start())
            {
                Log.Error("dotnet publish 启动失败");
                return false;
            }

            // 异步读取输出，避免子进程因输出缓冲区满而阻塞
            proc.OutputDataReceived += (s, e) =>
            {
                if (e.Data != null)
                {
                    stdoutSb.AppendLine(e.Data);
                    Log.Debug(e.Data);
                }
            };
            proc.ErrorDataReceived += (s, e) =>
            {
                if (e.Data != null)
                {
                    stderrSb.AppendLine(e.Data);
                    Log.Warning(e.Data);
                }
            };
            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();
        }
        catch (Exception ex)
        {
            Log.Error($"dotnet publish 启动异常: {ex.Message}");
            return false;
        }

        // 等待发布进程完成
        await proc.WaitForExitAsync();
        // 读取收集到的输出
        var stderr = stderrSb.ToString();
        if (proc.ExitCode != 0)
        {
            Log.Error($"dotnet publish 失败，退出码: {proc.ExitCode}");
            if (!string.IsNullOrEmpty(stderr))
                Log.Error($"dotnet publish 错误: {stderr}");

            return false;
        }

        if (!string.IsNullOrEmpty(stderr))
            Log.Error($"dotnet publish 错误: {stderr}");

        Log.Information("dotnet publish 完成");

        // DNNE: 复制 native DLL 到输出目录
        if (!isNative)
        {
            // 获取项目目录和项目名
            var projectDir = Path.GetDirectoryName(projectsPath)!;
            var projectName = Path.GetFileNameWithoutExtension(projectsPath);

            // DNNE 生成的 native DLL 路径
            var dnneNativeDll = Path.Combine(projectDir, "obj", "Release", "net10.0", "win-x86", "dnne", "bin",
                $"{projectName}NE.dll");

            if (File.Exists(dnneNativeDll))
            {
                var destDll = Path.Combine(publishDir, $"{projectName}NE.dll");
                File.Copy(dnneNativeDll, destDll, true);
                Log.Information($"DNNE native DLL 已复制: {destDll}");
            }
            else
            {
                Log.Warning($"DNNE native DLL 未找到: {dnneNativeDll}");
            }
        }

        return true;
    }

    private void DeleteOtherConfig()
    {
        if (File.Exists(Path.Combine(Config.War3, "fwht.txt")))
            File.Delete(Path.Combine(Config.War3, "fwht.txt"));
        if (File.Exists(Path.Combine(Config.War3, "fwhc.txt")))
            File.Delete(Path.Combine(Config.War3, "fwhc.txt"));
        if (File.Exists(Path.Combine(Config.War3, "dz_w3_plugin.dll")))
            File.Delete(Path.Combine(Config.War3, "dz_w3_plugin.dll"));
        if (File.Exists(Path.Combine(Config.War3, "version.dll")))
            File.Delete(Path.Combine(Config.War3, "version.dll"));
    }

    public async Task<bool> Run(bool isCache, bool noTest)
    {
        var startTime = DateTime.Now;
        var dstW3xFire = Path.Combine(Config.War3, "Maps", "Test", $"{ProjectName}.w3x");
        var testLoadPath = dstW3xFire;
        // 确保目录存在（CreateDirectory 会自动创建所有缺失的父目录）
        Directory.CreateDirectory(Path.Combine(Config.War3, "Maps", "Test"));

        var modeLni = "slk";
        if (BuildMode is BuildModeEnum.Test) modeLni = "obj";

        var projectsPath = Path.Combine(Projects, ProjectName, $"{ProjectName}.csproj");
        var pubilshDir = Path.Combine(BuildDstPath, "map");

        // 打包dll->
        // await PublishProject(BuildMode is BuildModeEnum.Release, projectsPath, pubilshDir);
        var buildTask = BuildMap(isCache, noTest);
        Task<bool> publishTask;
        if (BuildMode is BuildModeEnum.Release)
        {
            publishTask = PublishProject(true, projectsPath, pubilshDir);
        }
        else
        {
            Log.Debug("非 Release 模式，跳过 dotnet publish");
            publishTask = Task.FromResult(true);
        }

        var results = await Task.WhenAll(buildTask, publishTask);
        if (!results.All(static result => result))
        {
            Log.Error("构建流程中止：资源构建或发布步骤失败");
            return false;
        }

        // 确保前面所有异步步骤均已完成后再进行打包
        if (BuildMode is BuildModeEnum.Test)
        {
            testLoadPath = BuildDstPath;
        }
        else
        {
            if (!PackupMap(modeLni, dstW3xFire))
                return false;
        }

        DeleteOtherConfig();


        if (noTest) return true;

        Log.Information("即将准备地图测试");
        // 精确（不区分大小写）
        var war3Count = Process.GetProcesses()
            .Count(p => string.Equals(p.ProcessName, "war3", StringComparison.OrdinalIgnoreCase));
        if (war3Count > 0) Log.Warning(">>> 请先关闭当前war3!!! <<<");

        if (!RunTest(testLoadPath, 0))
            return false;

        Log.Information($"本次执行时间: {(DateTime.Now - startTime).TotalSeconds}");
        return true;
    }
}

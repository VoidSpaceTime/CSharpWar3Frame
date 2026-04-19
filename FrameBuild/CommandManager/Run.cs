using Serilog;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using War3FrameBuild.Extension;
using static War3Frame.Assets;

namespace War3FrameBuild.CommandManager
{
    public partial class CommandManager
    {
        /// <summary>
        /// 运行war3 进行测试
        /// </summary>
        /// <param name="w3xFire"></param>
        /// <param name="qty"></param>
        private void RunTest(string w3xFire, int qty)
        {
            Log.Information("启动魔兽争霸III");
            var psi = new ProcessStartInfo
            {
                FileName = Path.Combine(Config.We, "bin", "YDWEConfig.exe"),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            psi.ArgumentList.Add("-launchwar3");
            psi.ArgumentList.Add(Path.Combine(Config.War3, "war3.exe"));
            psi.ArgumentList.Add("-loadfile");
            psi.ArgumentList.Add(w3xFire);

            // 检查必要文件是否存在
            if (!File.Exists(Path.Combine(Config.We, "bin", "YDWEConfig.exe")))
            {
                Log.Error("YDWEConfig.exe 不存在，请检查 YDWE 安装路径");
                return;
            }

            if (!File.Exists(w3xFire))
            {
                Log.Error($"地图文件不存在: {w3xFire}");
                return;
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
            }
            else
            {
                if (qty > 3)
                {
                    Log.Error("启动魔兽争霸III失败，请检查环境");
                    return;
                }

                Log.Warning($"未检测到魔兽争霸III运行，等待1秒后重试启动（第{qty}次尝试）");
                RunTest(w3xFire, qty + 1);
            }
        }

        /// <summary>
        /// 通过w2l 打包地图
        /// </summary>
        /// <param name="modeLni"></param>
        /// <param name="dstW3xFire"></param>
        private void PackupMap(string modeLni, string dstW3xFire)
        {
            if (File.Exists(dstW3xFire))
                File.Delete(dstW3xFire);

            // 打包地图
            Log.Verbose("开始打包地图");
            var startTime = DateTime.Now;
            StartW3XToLni(new[] { modeLni, BuildDstPath, dstW3xFire, "-ydwe", Config.We });

            Log.Debug($"打包地图，路径：{dstW3xFire}");
            Log.Verbose($"打包地图，耗时：{(DateTime.Now - startTime).TotalSeconds.ToString()}");
        }

        // 预编译正则表达式以提高性能
        private static readonly Regex ModulePathRegex = new(@"string ModulePath = .*", RegexOptions.Compiled);
        private static readonly Regex ModuleNameRegex = new(@"string ModuleName = .*", RegexOptions.Compiled);
        private static readonly Regex IsNativeRegex = new(@"bool IsNative = .*", RegexOptions.Compiled);

        /// <summary>
        /// 创建 w2l 进程启动信息
        /// </summary>
        private ProcessStartInfo CreateW2lProcessInfo() => new()
        {
            FileName = Path.Combine(Config.W3x2lni, "w2l.exe"),
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };

        private bool StartW3XToLni(string[] args)
        {
            // 地图文件较新，需要拆包同步
            var w2lProc = CreateW2lProcessInfo();
            foreach (var arg in args)
            {
                w2lProc.ArgumentList.Add(arg);
            }

            using var w2l = new Process { StartInfo = w2lProc, EnableRaisingEvents = true };
            if (!w2l.Start())
            {
                Log.Error("w2l 进程启动失败");
                return false;
            }

            w2l.WaitForExit();
            /*if (w2l.ExitCode != 0)
            {
                var errorOutput = w2l.StandardError.ReadToEndAsync().Result;
                Log.Warning($"w2l 执行警告 (ExitCode={w2l.ExitCode}): {errorOutput}");
            }*/

            return true;
        }


        /// <summary>
        /// 清理构建目录中的特定文件和文件夹
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
        /// 构建地图
        /// </summary>
        /// <param name="isCache"></param>
        /// <param name="noTest"></param>
        /// <returns></returns>
        private async Task<bool> BuildMap(bool isCache, bool noTest)
        {
            // 同步地图文件
            SyncW3xFile(isCache);

            if (!isCache)
            {
                // 准备构建目录
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

                // 复制项目文件
                DirectoryExtensions.CopyDir(TempProjectBuildPath, BuildDstPath);
                DirectoryExtensions.CopyDir(Path.Combine(Template, "lni", "assets", "UI"),
                    Path.Combine(BuildDstPath, "map", "UI"));

                // 处理 callback 文件
                if (!ProcessCallback())
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
        /// 处理 callback 文件
        /// </summary>
        private bool ProcessCallback()
        {
            var mapDir = Path.Combine(BuildDstPath, "map");
            var callbackInBuild = Path.Combine(mapDir, "callback");
            var dllName = BuildMode is BuildModeEnum.Release ? "project.dll" : $"BridgeToJIT.dll";
            string sourceFile = Path.Combine(PwdProject, "w3x", "map", "callback");
            if (File.Exists(callbackInBuild))
            {
                File.Delete(callbackInBuild);
            }

            if (!File.Exists(sourceFile))
            {
                sourceFile = Path.Combine(Template, "callback");
                if (!File.Exists(sourceFile))
                {
                    Log.Error("CallBack文件丢失");
                    return false;
                }
            }


            var content = File.ReadAllText(sourceFile);
            var escapedPath = mapDir.Replace("\\", "/").Replace("/", "\\\\");

            content = ModulePathRegex.Replace(content, $"string ModulePath = \"{escapedPath}\"");
            content = ModuleNameRegex.Replace(content, $"string ModuleName = \"{dllName}\"");
            content = IsNativeRegex.Replace(content, $"bool IsNative = {(BuildMode is BuildModeEnum.Release).ToString()}");

            File.WriteAllText(callbackInBuild, content);
            return true;
        }


        /// <summary>
        /// 脚本文件打包成dll
        /// </summary>
        /// <param name="isNative"></param>
        /// <param name="projectsPath"></param>
        /// <param name="publishDir"></param>
        /// <returns></returns>
        private async Task PublishProject()
        {
            var projectsPath = Path.Combine(PwdProject, $"{ProjectName}.csproj");
            var publishMapDir = Path.Combine(BuildDstPath, "map");
            var isNative = BuildMode == BuildModeEnum.Release;
            var projectName = Path.GetFileNameWithoutExtension(projectsPath);
            /*// 目前只会 AOT 打包
            isNative = true;*/
            var psi = new ProcessStartInfo("dotnet")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            psi.ArgumentList.Add("publish");
            psi.ArgumentList.Add(projectsPath);
            psi.ArgumentList.Add("-c");
            psi.ArgumentList.Add("Release");
            psi.ArgumentList.Add("-r");
            psi.ArgumentList.Add("win-x86");


            if (isNative)
            {
                psi.ArgumentList.Add("--self-contained");
                psi.ArgumentList.Add("true");
                psi.ArgumentList.Add("-p:PublishAot=true");
                psi.ArgumentList.Add("-p:DebugType=None");
                psi.ArgumentList.Add("-p:DebugSymbols=false");
            }

            psi.ArgumentList.Add("-o");
            psi.ArgumentList.Add(publishMapDir);

            Log.Debug($"准备执行 dotnet publish，输出目录: {publishMapDir}");

            using var proc = new Process() { StartInfo = psi, EnableRaisingEvents = true };
            // 异步读取输出，避免子进程因输出缓冲区满而阻塞
            var stdoutSb = new StringBuilder();
            var stderrSb = new StringBuilder();
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

            try
            {
                if (!proc.Start())
                {
                    Log.Error("dotnet publish 启动失败");
                    return;
                }

                proc.BeginOutputReadLine();
                proc.BeginErrorReadLine();


                await proc.WaitForExitAsync();
            }
            catch (Exception ex)
            {
                Log.Error($"dotnet publish 启动异常: {ex.Message}");
                return;
            }

            // 读取收集到的输出
            var stderr = stderrSb.ToString();
            if (proc.ExitCode != 0)
            {
                Log.Error($"dotnet publish 失败，ExitCode={proc.ExitCode}");
                if (stdoutSb.Length > 0)
                {
                    Log.Error($"dotnet publish 输出: {stdoutSb}");
                }

                if (!string.IsNullOrWhiteSpace(stderr))
                {
                    Log.Error($"dotnet publish 错误: {stderr}");
                }

                return;
            }

            if (!string.IsNullOrWhiteSpace(stderr))
            {
                Log.Warning($"dotnet publish 警告: {stderr}");
            }

            Log.Information("dotnet publish 完成");

            if (!isNative)
            {
                var bridgeBuildDir = Path.Combine(Config.Pwd, "BridgeToJIT", ".build", "Debug");
                var bridgeSourceDir = File.Exists(Path.Combine(bridgeBuildDir, "BridgeToJIT.dll"))
                    ? bridgeBuildDir
                    : Template;
                var bridgeFiles = new[]
                {
                    "BridgeToJIT.dll",
                    "BridgeToJIT.runtimeconfig.json",
                    "BridgeToJIT.deps.json",
                    "Ijwhost.dll"
                };
                var copiedBridgeFile = false;

                foreach (var bridgeFile in bridgeFiles)
                {
                    var sourcePath = Path.Combine(bridgeSourceDir, bridgeFile);
                    if (!File.Exists(sourcePath))
                    {
                        continue;
                    }

                    var destPath = Path.Combine(publishMapDir, bridgeFile);
                    File.Copy(sourcePath, destPath, true);
                    Log.Information($"Bridge文件已复制: {destPath}");
                    copiedBridgeFile = true;
                }

                if (!copiedBridgeFile)
                {
                    Log.Warning($"BridgeToJIT 构建产物未找到: {bridgeSourceDir}");
                    return;
                }
            }

            return;
        }

        void DeleteOtherConfig()
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
            // 确保目录存在（CreateDirectory 会自动创建所有缺失的父目录）
            Directory.CreateDirectory(Path.Combine(Config.War3, "Maps", "Test"));

            var modeLni = "slk";
            if (BuildMode is BuildModeEnum.Test or BuildModeEnum.Build)
            {
                modeLni = "obj";
            }

            var tasks = new List<Task>();


            // 打包dll->
            tasks.Add(BuildMap(isCache, noTest));
            tasks.Add(PublishProject());

            await Task.WhenAll(tasks);
            /*BuildMap(isCache, noTest).Wait();
            PublishProject(BuildMode is BuildModeEnum.Release, projectsPath, pubilshDir).Wait();*/

            // 确保前面所有异步步骤均已完成后再进行打包
            PackupMap(modeLni, dstW3xFire);

            DeleteOtherConfig();

            if (noTest)
            {
                return true;
            }

            Log.Information("即将准备地图测试");
            // 精确（不区分大小写）
            var war3Count = Process.GetProcesses()
                .Count(p => string.Equals(p.ProcessName, "war3", StringComparison.OrdinalIgnoreCase));
            if (war3Count > 0)
            {
                Log.Warning(">>> 请先关闭当前war3!!! <<<");
            }

            RunTest(dstW3xFire, 0);
            Log.Information($"本次执行时间: {(DateTime.Now - startTime).TotalSeconds}");
            return true;
        }
    }
}

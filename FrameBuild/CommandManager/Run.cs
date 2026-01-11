using Serilog;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using War3FrameBuild.Extension;

namespace War3FrameBuild.CommandManager
{
    public partial class CommandManager
    {
        private void RunTest(string w3xFire, int qty)
        {
            Log.Information("启动魔兽争霸III");
            var psi = new ProcessStartInfo
            {
                FileName = Path.Combine(Config.We, "bin", "WEConfig.exe"),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            psi.ArgumentList.Add("-launchwar3");
            psi.ArgumentList.Add("-loadfile");
            psi.ArgumentList.Add(w3xFire);

            var war3Psi = Process.Start(psi);
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
                Task.Delay(1000).Wait();
                RunTest(w3xFire, qty + 1);
            }
        }
        public bool Run(bool isCache, bool isSemi)
        {
            var BuildDstPath = Path.Combine(Temp, BuildMode.ToString(), ProjectName);
            var dstW3xFire = Path.Combine(BuildDstPath, "pack.w3x");
            var modeLni = "slk";
            if (BuildMode is BuildModeEnum.Test)
            {
                modeLni = "obj";
            }
            var temProjectDir = Path.Combine(Temp, ProjectName); //缓存项目目录
            var temProjectW3xFire = Path.Combine(Temp, ProjectName + ".w3x"); //缓存w3x路径
            var buoyFire = Path.Combine(Temp, ProjectName, ".we"); //缓存we路径
            var mtW = File.GetLastWriteTime(temProjectW3xFire);
            var mtB = File.GetLastWriteTime(buoyFire);
            // 如果地图文件比we打开时新（说明有额外保存过）把保存后的文件拆包并同步
            if (mtW > mtB)
            {
                var w2lProc = new ProcessStartInfo
                {
                    FileName = Path.Combine(Config.W3x2lni, "w2l.exe"),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };
                w2lProc.ArgumentList.Add("lni");
                w2lProc.ArgumentList.Add(temProjectW3xFire);


                using var w2l = new Process { StartInfo = w2lProc, EnableRaisingEvents = true };

                if (!w2l.Start())
                {
                    Log.Error($"w2l执行失败: {w2l.StandardError.ReadToEnd()}");
                }
                File.Delete(buoyFire);
                File.Copy(Path.Combine(Template, "lni", "x.we"), buoyFire);
                Backup();
                Log.Information("同步完毕[检测到有新的地图保存行为，以‘WE’为主版本]");
            }
            else
            {
                Pickup();
                Log.Information("同步完毕[检测到没有新的地图保存行为，以‘project’为主版本]");
            }
            if (BuildMode == BuildModeEnum.Release)
            {
                Log.Debug("准备发布打包");
                Directory.Delete(BuildDstPath, true);
            }
            else
            {
                // 非release|dist采用非必要更替性覆盖（多余的资源文件将存留在.tmp中继续使用，除非有更新的同名文件覆盖它）
                File.Delete(Path.Combine(BuildDstPath, ".we"));
                Directory.Delete(Path.Combine(BuildDstPath, "map"), true);
                File.Delete(Path.Combine(BuildDstPath, "table"));
            }
            DirectoryExtensions.CopyDir(temProjectDir, BuildDstPath);

            // 需要增加对callback 的处理
            // 调试模式下, 不进行打包, 发布模式调整为AOT编译dll
            var callBackFile = Path.Combine(BuildDstPath, "map", "callback");
            if (File.Exists(callBackFile))
            {
                var content = File.ReadAllText(callBackFile);
                var patternPath = "string ModulePath = .*";
                var replacementPath = $"string ModulePath = \"{Path.Combine(Temp, ProjectName)}\"";
                var res = Regex.Replace(content, patternPath, replacementPath);
                File.WriteAllText(callBackFile, res);
            }
            else
            {
                Log.Error("CallBack 文件丢失");
                return false;
            }



            Log.Verbose("构建地图完毕：" + BuildMode.ToString());

            // 调整代码，以支持war3
            //War3map(); 暂未实现
            var startTime = DateTime.Now;
            Log.Verbose($"资源及代码处理完成，耗时：{(DateTime.Now - startTime).TotalSeconds.ToString()}");

            // 打包地图
            Log.Verbose("开始打包地图");
            startTime = DateTime.Now;
            var w2lProc2 = new ProcessStartInfo
            {
                FileName = Path.Combine(Config.W3x2lni, "w2l.exe"),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            w2lProc2.ArgumentList.Add(modeLni);
            w2lProc2.ArgumentList.Add(BuildDstPath);
            w2lProc2.ArgumentList.Add(dstW3xFire);

            using var w2l2 = new Process { StartInfo = w2lProc2, EnableRaisingEvents = true };

            if (!w2l2.Start())
            {
                Log.Error($"w2l执行失败: {w2l2.StandardError.ReadToEnd()}");
            }


            File.Delete(Path.Combine(Config.War3, "fwht.txt"));
            File.Delete(Path.Combine(Config.War3, "fwhc.txt"));
            File.Delete(Path.Combine(Config.War3, "dz_w3_plugin.dll"));
            File.Delete(Path.Combine(Config.War3, "version.dll"));
            var mtPath = Path.Combine(Config.War3, "Maps", "Test");
            if (Directory.Exists(mtPath) is false)
            {
                Directory.CreateDirectory(mtPath);
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
            return true;
        }
    }

}

using Serilog;
using System.Diagnostics;
using System.Text.RegularExpressions;
using War3FrameBuild.Extension;

namespace War3FrameBuild.CommandManager
{
    public partial class CommandManager
    {
        public async Task WE()
        {
            var weExe = new string[] { "KKWE.exe", "WE.exe" }.Where(p=>File.Exists(Path.Combine(Config.We, p))).FirstOrDefault();
           
            if (ProjectName == string.Empty)
            {
                var psi = new ProcessStartInfo
                {
                    FileName = weExe,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };
                var weProcess = Process.Start(psi);
                return;
            }
            if (Directory.Exists(Path.Combine(Projects, ProjectName)) is false)
            {
                Log.Error("项目不存在，请先使用 new 命令创建项目");
                return;
            }
            var weClient = new string[] { "worldedit.exe", "worldeditydwe.exe", "worldeditkkwe.exe" };
            var war3Count = 0;
            weClient.Select(procName =>
            {
                war3Count += Process.GetProcesses()
                             .Count(p => string.Equals(p.ProcessName, procName, StringComparison.OrdinalIgnoreCase));
                return procName;
            }).ToArray();
            if (war3Count > 0)
            {
                Log.Warning("提示：检测到已有WE开启中，如果你是重复调用了we命令，请保留一个进行修改!");
            }
            // 定义临时目录和文件路径
            var tempW3xDir = Path.Combine(Temp, ProjectName);
            var tempW3xFile = Path.Combine(tempW3xDir, ".w3x");
            var tempWEFlie = Path.Combine(tempW3xDir, ".we");
            // 检查上一次we的修改数据是否未保存
            var timeW3x = File.GetLastWriteTime(tempW3xFile); // 获取w3x文件修改时间
            var timeWE = File.GetLastWriteTime(tempWEFlie); // 获取标记文件修改时间
            var terrain = "";
            // 如果地图文件比we打开时新（说明有额外保存过）把保存后的文件拆包并同步
            if (timeW3x > timeWE)
            {
                var w2lProc = new ProcessStartInfo
                {
                    FileName = Path.Combine(Config.W3x2lni, "w2l.exe"),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };
                w2lProc.ArgumentList.Add("lni");
                w2lProc.ArgumentList.Add(tempW3xFile);
                Process.Start(w2lProc);
                Backup();  // 以编辑器为主版本
                Log.Information("同步完毕[检测到之前有过使用we命令进行地图保存行为，正在进行同步备份]");
            }
            // 删除旧的标记文件
            File.Delete(tempWEFlie);

            // 打包文件
            Pickup();

            // 加载项目地形贴图
            var projectAssetsPath = Path.Combine(Projects, ProjectName, "assets");
            if (Directory.Exists(projectAssetsPath))
            {
                Log.Information($"尝试加载项目 {ProjectName} 中的terrain资源");
                string allText = "";
                foreach (var item in Directory.EnumerateFiles(projectAssetsPath))
                {
                    var text = await File.ReadAllTextAsync(item);
                    allText.Replace("\r\n", "\n");
                    allText.Replace("\r", "\n");
                    allText = Regex.Replace(allText, @"//(.*)", string.Empty, RegexOptions.Multiline);
                    allText = Regex.Replace(allText, @"/\*.*?\*/", string.Empty, RegexOptions.Singleline);

                    allText += text;

                }
                var process = allText.Split("\n")
                    .Where(t => t.TrimStart().StartsWith("AssetsList.AddTerrain"))
                    .Select(e =>
                    {
                        var m = Regex.Matches(e, @"""[^""]*""");
                        return m[0].Value ?? "";
                    });
                if (process.Count() > 1)
                {
                    Log.Error("地形贴图冲突[调用过" + process.First() + "的贴图，确保项目只引用过一次的地形贴图]");
                }
                terrain = process.First();

                if (terrain == "")
                {
                    Log.Error($"未找到项目{ProjectName}中引用了terrain资源");
                }
                else
                {
                    var terrainDir = Path.Combine(Config.Assets, "war3mapTerrain", terrain);
                    if (Directory.Exists(terrainDir))
                    {

                        var cliff = Path.Combine(terrainDir, "Cliff");
                        var terrainArt = Path.Combine(terrainDir, "TerrainArt");
                        if (!Directory.Exists(terrainArt) || !Directory.Exists(cliff))
                        {
                            Log.Error($"地形贴图：{terrain} 地形数据错误");
                        }
                        DirectoryExtensions.CopyDir(cliff, Path.Combine(tempW3xDir, "resource", "ReplaceableTextures", "Cliff"));
                        DirectoryExtensions.CopyDir(terrainArt, Path.Combine(tempW3xDir, "resource", "TerrainArt"));
                    }
                    else
                    {
                        Log.Error($"地形贴图：{terrain} 资源不存在，请检查Assets/war3mapTerrain目录");
                    }
                }
            }
            // 打包地图文件
            var w2lProcess = new ProcessStartInfo
            {
                FileName = Path.Combine(Config.W3x2lni, "w2l.exe"),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            w2lProcess.ArgumentList.Add("obj");
            w2lProcess.ArgumentList.Add(tempW3xDir);
            w2lProcess.ArgumentList.Add(tempW3xFile);
            using var w2l = new Process { StartInfo = w2lProcess, EnableRaisingEvents = true };
            if (!w2l.Start())
            {
                Log.Error($"w2l执行失败: {w2l.StandardError.ReadToEnd()}");
                return;
            }
            // 创建标记文件
            File.Copy(Path.Combine(Template, "lni", "x.we"), tempWEFlie);
            var weProc = new ProcessStartInfo(Path.Combine(Config.We, "we.exe"), ["-loadfile", tempW3xFile])
            {
                FileName = Path.Combine(Config.We, "we.exe"),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            using var we = new Process { StartInfo = w2lProcess, EnableRaisingEvents = true };
            if (!we.Start())
            {
                Log.Error($"we启动失败: {we.StandardError.ReadToEnd()}");
                return;
            }
            Log.Verbose("WE编辑器启动成功");
        }
    }
}

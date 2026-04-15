using Serilog;
using System.Diagnostics;
using System.Text.RegularExpressions;
using War3FrameBuild.Extension;

namespace War3FrameBuild.CommandManager
{
    public partial class CommandManager
    {
        public void WE()
        {
            var weExe = new string[] { "KKWE.exe", "WE.exe" }.Where(p => File.Exists(Path.Combine(Config.We, p)))
                .FirstOrDefault();

            if (ProjectName == string.Empty)
            {
                var psi = new ProcessStartInfo
                {
                    FileName = weExe,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };
                using var weProcess = Process.Start(psi);
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
            Path.Combine(Temp, ProjectName);
            var weMarkFile = Path.Combine(TempProjectBuildPath, ".we");

            // 同步资源we文件
            SyncW3xFile(false);
            var terrain = "";


            // 加载项目地形贴图
            var projectAssetsPath = Path.Combine(Projects, ProjectName, "assets");
            if (Directory.Exists(projectAssetsPath))
            {
                Log.Information($"尝试加载项目 {ProjectName} 中的terrain资源");
                var allText = new List<string>();
                foreach (var item in Directory.EnumerateFiles(projectAssetsPath))
                {
                    var text = File.ReadAllText(item);
                    text = text.Replace("\r\n", "\n");
                    text = text.Replace("\r", "\n");
                    text = Regex.Replace(text, @"//(.*)", string.Empty, RegexOptions.Multiline);
                    text = Regex.Replace(text, @"/\*.*?\*/", string.Empty, RegexOptions.Singleline);


                    allText.AddRange(text.Split("\n").Where(t => t.Contains("AssetsList.AddTerrain")));
                }

                var process = allText
                    .Select(e =>
                    {
                        var m = Regex.Matches(e, @"""[^""]*""");
                        return m[0].Value ?? "";
                    });
                if (process.Count() > 1)
                {
                    Log.Error("地形贴图冲突[调用过" + process.First() + "的贴图，确保项目只引用过一次的地形贴图]");
                }

                if (process.Count() < 1)
                {
                    Log.Error($"未找到项目{ProjectName}中引用了terrain资源");
                }
                else
                {
                    terrain = process.First();

                    var terrainDir = Path.Combine(Config.Assets, "war3mapTerrain", terrain);
                    if (Directory.Exists(terrainDir))
                    {
                        var cliff = Path.Combine(terrainDir, "Cliff");
                        var terrainArt = Path.Combine(terrainDir, "TerrainArt");
                        if (!Directory.Exists(terrainArt) || !Directory.Exists(cliff))
                        {
                            Log.Error($"地形贴图：{terrain} 地形数据错误");
                        }

                        DirectoryExtensions.CopyDir(cliff,
                            Path.Combine(TempProjectBuildPath, "resource", "ReplaceableTextures", "Cliff"));
                        DirectoryExtensions.CopyDir(terrainArt,
                            Path.Combine(TempProjectBuildPath, "resource", "TerrainArt"));
                    }
                    else
                    {
                        Log.Error($"地形贴图：{terrain} 资源不存在，请检查Assets/war3mapTerrain目录");
                    }
                }
            }

            var distFile = Path.Combine(Temp, $"{ProjectName}.w3x");

            // 打包地图文件
            PackWeW3xFile();
            // 创建标记文件
            var wePath = new string[] { "we.exe", "kkwe.exe" }.Where(p => File.Exists(Path.Combine(Config.We, p)))
                .FirstOrDefault("");
            if (wePath is not "")
            {
                var weProc = new ProcessStartInfo
                {
                    FileName = Path.Combine(Config.We, wePath),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };
                weProc.ArgumentList.Add("-loadfile");
                weProc.ArgumentList.Add(distFile);
                Task.Delay(500).Wait();
                using var we = new Process { StartInfo = weProc, EnableRaisingEvents = true };
                if (!we.Start())
                {
                    Log.Error($"we启动失败: {we.StandardError.ReadToEnd()}");
                    return;
                }

                File.Copy(Path.Combine(Template, "lni", "x.we"), weMarkFile);

                Log.Verbose("WE编辑器启动成功");
            }
            else
            {
                throw new Exception("WE编辑器不存在，请检查配置文件中的We路径是否正确");
            }
        }
    }
}
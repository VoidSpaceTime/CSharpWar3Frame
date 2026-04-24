using System.Diagnostics;
using Serilog;
using War3FrameBuild.Extension;

namespace War3FrameBuild.CommandManager;

public partial class CommandManager
{
    public void SyncW3xFile(bool isCache)
    {
        var tempW3xDir = Path.Combine(TempProjectBuildPath);
        var weMarkFile = Path.Combine(TempProjectBuildPath, ".we");
        var w3xMarkFile = Path.Combine(tempW3xDir, ".w3x");

        if (File.Exists(weMarkFile))
        {
            UnpackWeW3xFile();
            TempResourceToProject();
            // ProjectResourceToTemp();
            Log.Information("同步完毕[检测到有新的地图保存行为，以'WE'为主版本]");
        }
        else if (!isCache)
        {
            ProjectResourceToTemp();
            Log.Information("同步完毕[检测到没有新的地图保存行为，以'project'为主版本]");
        }
    }

    /// <summary>
    /// 打包资源文件给we使用
    /// </summary>
    public void PackWeW3xFile()
    {
        var w2lProc = CreateW2lProcessInfo();
        w2lProc.ArgumentList.Add("obj");
        w2lProc.ArgumentList.Concat(["adfa", "dfadfa"]);
        w2lProc.ArgumentList.Add(TempProjectBuildPath);
        w2lProc.ArgumentList.Add(Path.Combine(Temp, ProjectName + ".w3x"));
        using var w2l = new Process { StartInfo = w2lProc, EnableRaisingEvents = true };
        if (!w2l.Start())
        {
            Log.Error("w2l 进程启动失败");
            return;
        }

        w2l.WaitForExitAsync();
        if (w2l.ExitCode != 0)
        {
            var errorOutput = w2l.StandardError.ReadToEndAsync();
            Log.Warning($"w2l 打包失败,警告 (ExitCode={w2l.ExitCode}): {errorOutput}");
        }
    }

    /// <summary>
    /// 解压资源文件给we使用
    /// </summary>
    public void UnpackWeW3xFile()
    {
        var tempW3xDir = Path.Combine(TempProjectBuildPath);
        var weMarkFile = Path.Combine(TempProjectBuildPath, ".we");
        var w3xMarkFile = Path.Combine(tempW3xDir, ".w3x");
        if (File.Exists(weMarkFile))
        {
            // 地图文件较新，需要拆包同步
            StartW3XToLni(new[] { "lni", Path.Combine(Temp, ProjectName + ".w3x") });

            File.Delete(weMarkFile);
        }
        // File.Copy(Path.Combine(Template, "lni", "x.we"), weMarkFile);
    }

    public void ProjectResourceToTemp()
    {
        var w3xDir = Path.Combine(PwdProject, "w3x");
        // 复制project的w3x文件夹
        if (Directory.Exists(TempProjectBuildPath) is false)
        {
            DirectoryExtensions.CopyDir(Path.Combine(Template, "lni", "w3x2lni"),
                Path.Combine(TempProjectBuildPath, "w3x2lni"));
            File.Copy(Path.Combine(Template, "lni", "x.w3x"), Path.Combine(TempProjectBuildPath, ".w3x"));
        }

        // 删除map文件夹并重新复制
        if (Directory.Exists(Path.Combine(TempProjectBuildPath, "map")))
            Directory.Delete(Path.Combine(TempProjectBuildPath, "map"), true);
        DirectoryExtensions.CopyDir(Path.Combine(w3xDir, "map"), Path.Combine(TempProjectBuildPath, "map"));

        var w3xTableDir = Path.Combine(w3xDir, "table");
        var tempTableDir = Path.Combine(TempProjectBuildPath, "table");
        // 复制project的table文件夹
        if (File.GetLastWriteTime(w3xTableDir) > File.GetLastWriteTime(tempTableDir))
        {
            Directory.Delete(tempTableDir, true);
            DirectoryExtensions.CopyDir(Path.Combine(w3xDir, "table"), Path.Combine(tempTableDir, "table"));
        }

        if (Directory.Exists(Path.Combine(TempProjectBuildPath, "resource")))
        {
            Directory.Delete(Path.Combine(TempProjectBuildPath, "resource"), true);
        }

        DirectoryExtensions.CopyDir(Path.Combine(Template, "lni", "resource"),
            Path.Combine(TempProjectBuildPath, "resource"));
        // 资源判定
        if (Directory.Exists(Path.Combine(PwdProject, "w3x", "resource")))
        {
            Directory.Delete(Path.Combine(TempProjectBuildPath, "resource"), true);
        }

        if (Directory.Exists(Path.Combine(w3xDir, "resource")))
            DirectoryExtensions.CopyDir(Path.Combine(w3xDir, "resource"),
                Path.Combine(TempProjectBuildPath, "resource"));

        // map
        var war3mapMap = Path.Combine(PwdProject, "w3x", "war3mapMap.blp");
        // 小地图判定
        if (File.GetLastWriteTime(war3mapMap) >
            File.GetLastWriteTime(Path.Combine(TempProjectBuildPath, "resource", "war3mapMap.blp")))
        {
            if (File.Exists(Path.Combine(TempProjectBuildPath, "resource", "war3mapMap.blp")))
                File.Delete(Path.Combine(TempProjectBuildPath, "resource", "war3mapMap.blp"));

            File.Copy(war3mapMap, Path.Combine(TempProjectBuildPath, "resource", "war3mapMap.blp"));
        }
    }

    public void TempResourceToProject()
    {
        var w3xDir = Path.Combine(PwdProject, "w3x", "map");
        if (Directory.GetLastWriteTime(Path.Combine(TempProjectBuildPath, "map")) > Directory.GetLastWriteTime(w3xDir))
        {
            Directory.Delete(w3xDir, true);
            DirectoryExtensions.CopyDir(Path.Combine(TempProjectBuildPath, "map"), w3xDir);
            Log.Information("备份完成[.tmp(地图备份)->w3x/map]");
        }

        var war3mapMap = Path.Combine(PwdProject, "w3x", "war3mapMap.blp");
        if (File.Exists(war3mapMap) is false)
        {
            File.Copy(Path.Combine(Template, "w3x", "war3mapMap.blp"), war3mapMap);
            Log.Information("修正备份[lni(war3mapMap)->w3x / war3mapMap]");
        }

        if (File.GetLastWriteTime(Path.Combine(TempProjectBuildPath, "resource", "war3mapMap.blp")) >
            File.GetLastWriteTime(war3mapMap))
        {
            File.Delete(war3mapMap);
            File.Copy(Path.Combine(TempProjectBuildPath, "resource", "war3mapMap.blp"), war3mapMap);
            Log.Information("更新同步[.tmp(war3mapMap)->w3x/war3mapMap]");
        }

        var tableDir = Path.Combine(Projects, ProjectName, "w3x", "table");
        if (Directory.GetLastWriteTime(Path.Combine(TempProjectBuildPath, "table")) >
            Directory.GetLastWriteTime(tableDir))
        {
            Directory.Delete(tableDir, true);
            DirectoryExtensions.CopyDir(Path.Combine(TempProjectBuildPath, "table"), tableDir);
            Log.Information("同步完成[.tmp(原生物编)->w3x/table]");
        }
    }
}
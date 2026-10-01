using Serilog;
using War3FrameBuild.Extension;

namespace War3FrameBuild.CommandManager;

public partial class CommandManager
{
    public void SyncW3xFile(bool isCache)
    {
        var marker = Path.Combine(TempProjectBuildPath, ".we");
        if (File.Exists(marker))
        {
            UnpackWeW3xFile();
            TempResourceToProject();
            // 解包和所有回同步都成功后才消费标记，失败可恢复重试。
            File.Delete(marker);
            Log.Information("WE 地图修改已同步到项目");
        }
        else if (!isCache)
            ProjectResourceToTemp();
    }

    public void PackWeW3xFile()
    {
        if (!PackMapAsync(TempProjectBuildPath, Path.Combine(Temp, ProjectName + ".w3x"), "obj").GetAwaiter().GetResult())
            throw new InvalidOperationException("WE 地图打包失败");
    }

    public void UnpackWeW3xFile()
    {
        if (!File.Exists(Path.Combine(TempProjectBuildPath, ".we"))) return;
        if (!StartW3XToLni(["lni", Path.Combine(Temp, ProjectName + ".w3x"), TempProjectBuildPath]))
            throw new InvalidOperationException("WE 地图解包失败，保留 .we 标记");
    }

    public void ProjectResourceToTemp()
    {
        var source = Path.Combine(PwdProject, "w3x");
        // 验证必需源目录，避免缺失输入时清理有效 staging。
        foreach (var folder in new[] { "map", "table" })
            if (!Directory.Exists(Path.Combine(source, folder))) throw new DirectoryNotFoundException(Path.Combine(source, folder));
        Directory.CreateDirectory(TempProjectBuildPath);
        DirectoryExtensions.CopyDir(Path.Combine(Template, "lni", "w3x2lni"), Path.Combine(TempProjectBuildPath, "w3x2lni"));
        File.Copy(Path.Combine(Template, "lni", "x.w3x"), Path.Combine(TempProjectBuildPath, ".w3x"), true);
        foreach (var folder in new[] { "map", "table" })
            DirectoryExtensions.SyncDirectory(Path.Combine(source, folder), Path.Combine(TempProjectBuildPath, folder));
        var resources = Directory.Exists(Path.Combine(source, "resource")) ? Path.Combine(source, "resource") : Path.Combine(Template, "lni", "resource");
        DirectoryExtensions.SyncDirectory(resources, Path.Combine(TempProjectBuildPath, "resource"));
        var minimap = Path.Combine(source, "war3mapMap.blp");
        if (File.Exists(minimap)) File.Copy(minimap, Path.Combine(TempProjectBuildPath, "resource", "war3mapMap.blp"), true);
    }

    public void TempResourceToProject()
    {
        var destination = Path.Combine(PwdProject, "w3x");
        foreach (var folder in new[] { "map", "table", "resource" })
            if (!Directory.Exists(Path.Combine(TempProjectBuildPath, folder))) throw new DirectoryNotFoundException(Path.Combine(TempProjectBuildPath, folder));
        foreach (var folder in new[] { "map", "table", "resource" })
            DirectoryExtensions.SyncDirectory(Path.Combine(TempProjectBuildPath, folder), Path.Combine(destination, folder));
        var minimap = Path.Combine(TempProjectBuildPath, "resource", "war3mapMap.blp");
        if (File.Exists(minimap)) File.Copy(minimap, Path.Combine(destination, "war3mapMap.blp"), true);
    }
}

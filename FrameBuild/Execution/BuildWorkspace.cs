using Serilog;
using War3FrameBuild.Extension;

namespace War3FrameBuild.Execution;

/// <summary>隔离构建副作用；最终路径固定，地图发布失败时恢复原模块目录。</summary>
internal sealed class BuildWorkspace : IDisposable
{
    private readonly string _destination;
    private readonly string _backup;
    internal string WorkingDirectory { get; }

    internal BuildWorkspace(string destination, bool copyPrevious)
    {
        _destination = Path.GetFullPath(destination);
        var parent = Path.GetDirectoryName(_destination) ?? throw new ArgumentException("不能用文件系统根作为构建目录");
        Directory.CreateDirectory(parent);
        var suffix = Guid.NewGuid().ToString("N");
        WorkingDirectory = _destination + ".staging-" + suffix;
        _backup = _destination + ".previous-" + suffix;
        Directory.CreateDirectory(WorkingDirectory);
        try
        {
            if (copyPrevious)
                DirectoryExtensions.CopyDir(_destination, WorkingDirectory);
        }
        catch
        {
            Directory.Delete(WorkingDirectory, true);
            throw;
        }
    }

    internal void Commit(string stagedMap, string destinationMap)
    {
        var hadPrevious = Directory.Exists(_destination);
        if (hadPrevious) Directory.Move(_destination, _backup);
        var installed = false;
        try
        {
            Directory.Move(WorkingDirectory, _destination);
            installed = true;
            File.Move(stagedMap, destinationMap, true);
        }
        catch
        {
            if (installed) Directory.Move(_destination, WorkingDirectory);
            if (hadPrevious) Directory.Move(_backup, _destination);
            throw;
        }
        if (hadPrevious)
        {
            try { Directory.Delete(_backup, true); }
            catch (IOException exception) { Log.Warning(exception, "构建已发布，旧模块目录稍后可清理: {Path}", _backup); }
            catch (UnauthorizedAccessException exception) { Log.Warning(exception, "构建已发布，旧模块目录稍后可清理: {Path}", _backup); }
        }
    }

    public void Dispose()
    {
        // 两个待清理位置均由本对象创建的唯一名称持有，绝不清理最终发布目录。
        if (Directory.Exists(WorkingDirectory)) Directory.Delete(WorkingDirectory, true);
    }
}

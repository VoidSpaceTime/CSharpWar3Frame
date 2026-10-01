namespace War3FrameBuild.Extension;

public static class DirectoryExtensions
{
    /// <summary>按完整文件集同步新增、修改和删除；目录时间戳不能代表内容版本。</summary>
    public static void SyncDirectory(string sourceDir, string targetDir)
    {
        var source = Path.TrimEndingDirectorySeparator(Path.GetFullPath(sourceDir));
        var target = Path.TrimEndingDirectorySeparator(Path.GetFullPath(targetDir));
        if (!Directory.Exists(source)) throw new DirectoryNotFoundException(source);
        if (IsWithin(source, target) || IsWithin(target, source))
            throw new ArgumentException("同步目录不得重叠");
        var files = Directory.GetFiles(source, "*", SearchOption.AllDirectories)
            .ToDictionary(p => Path.GetRelativePath(source, p), StringComparer.OrdinalIgnoreCase);
        Directory.CreateDirectory(target);
        foreach (var (relative, file) in files)
        {
            var destination = Path.Combine(target, relative);
            Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
            File.Copy(file, destination, true);
        }
        // 源文件全部复制成功后才清理目标多余文件。
        foreach (var file in Directory.GetFiles(target, "*", SearchOption.AllDirectories))
            if (!files.ContainsKey(Path.GetRelativePath(target, file))) File.Delete(file);
        foreach (var directory in Directory.GetDirectories(target, "*", SearchOption.AllDirectories).OrderByDescending(p => p.Length))
            if (!Directory.EnumerateFileSystemEntries(directory).Any()) Directory.Delete(directory);
    }

    public static void CopyDir(string sourceDir, string targetDir, ISet<string>? excludedDirectories = null)
    {
        if (string.IsNullOrEmpty(sourceDir))
            throw new ArgumentException("sourceDir is null or empty", nameof(sourceDir));
        if (string.IsNullOrEmpty(targetDir))
            throw new ArgumentException("targetDir is null or empty", nameof(targetDir));

        // normalize full paths
        var sourceFull = Path.TrimEndingDirectorySeparator(Path.GetFullPath(sourceDir));
        var targetFull = Path.TrimEndingDirectorySeparator(Path.GetFullPath(targetDir));

        // 任一方向重叠都会造成递归复制或源文件覆盖，必须在写入前拒绝。
        if (IsWithin(sourceFull, targetFull) || IsWithin(targetFull, sourceFull))
            throw new ArgumentException("复制目录不得重叠");

        // ensure source exists
        if (!Directory.Exists(sourceFull))
            throw new DirectoryNotFoundException($"Source directory not found: {sourceFull}");

        // create target if missing
        if (!Directory.Exists(targetFull))
            Directory.CreateDirectory(targetFull);

        // enumerate entries in source
        var entries = Directory.GetFileSystemEntries(sourceFull);
        foreach (var entry in entries)
        {
            var entryFull = Path.GetFullPath(entry);

            var name = Path.GetFileName(entryFull);
            var destPath = Path.Combine(targetFull, name);

            if (Directory.Exists(entryFull))
            {
                if (excludedDirectories?.Contains(name) != true)
                    CopyDir(entryFull, destPath, excludedDirectories);
            }
            else
                File.Copy(entryFull, destPath, true);
        }
    }
    private static bool IsWithin(string path, string directory)
        => string.Equals(path, directory, StringComparison.OrdinalIgnoreCase)
           || path.StartsWith(Path.EndsInDirectorySeparator(directory) ? directory : directory + Path.DirectorySeparatorChar,
               StringComparison.OrdinalIgnoreCase);
    // helper: 在起始目录向上查找相对文件（返回找到的完整路径或 null）

    public static string FindFileUpwards(string startDir, int maxLevels)
    {
        var dir = new DirectoryInfo(startDir);
        for (var i = 0; i <= maxLevels && dir != null; i++)
        {
            var candidate = dir.FullName;
            dir = dir.Parent;
        }

        if (dir is null) return string.Empty;
        return dir.FullName;
    }
}

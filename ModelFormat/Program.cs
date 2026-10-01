using FastMDX;

if (args.Length != 1 || string.IsNullOrWhiteSpace(args[0]) || !Directory.Exists(args[0]))
{
    Console.Error.WriteLine("用法: ModelFormat <现有模型目录>");
    return 1;
}

var sourceFolder = Path.TrimEndingDirectorySeparator(Path.GetFullPath(args[0]));
if (Directory.GetParent(sourceFolder) == null)
{
    Console.Error.WriteLine("模型目录不能是文件系统根目录，需要在其同级创建输出目录。");
    return 1;
}
var targetFolder = sourceFolder + "_Format";
Directory.CreateDirectory(targetFolder);
var files = Directory.GetFiles(sourceFolder, "*", SearchOption.AllDirectories)
    .Where(p => Path.GetExtension(p).Equals(".mdx", StringComparison.OrdinalIgnoreCase) || Path.GetExtension(p).Equals(".mdl", StringComparison.OrdinalIgnoreCase))
    .OrderBy(p => p, StringComparer.Ordinal).ToArray();
var count = 0;
foreach (var file in files)
{
    try
    {
        // 保留源相对目录，避免不同子目录的同名模型互相覆盖。
        var relative = Path.GetRelativePath(sourceFolder, file);
        var output = Path.Combine(targetFolder, Path.GetDirectoryName(relative)!, Path.GetFileNameWithoutExtension(file));
        Directory.CreateDirectory(output);
        var model = new MDX(file);
        for (var i = 0; i < (model.Textures?.Length ?? 0); i++)
        {
            var texture = model.Textures[i];
            if (texture.ReplaceableId != 0 || string.IsNullOrEmpty(texture.Name)) continue;
            var candidate = Path.Combine(Path.GetDirectoryName(file)!, texture.Name);
            if (!File.Exists(candidate))
            {
                // 兼容 war3mapModel 的公共资源根；普通目录无需具备该布局。
                for (var directory = Directory.GetParent(file); directory != null; directory = directory.Parent)
                    if (directory.Name.Equals("war3mapModel", StringComparison.OrdinalIgnoreCase) && directory.Parent != null)
                    { candidate = Path.Combine(directory.Parent.FullName, texture.Name); break; }
            }
            if (!File.Exists(candidate)) continue; // 游戏内置贴图保持原路径。
            var name = "texture" + i + Path.GetExtension(candidate).ToLowerInvariant();
            File.Copy(candidate, Path.Combine(output, name), true);
            texture.Name = name;
            model.Textures[i] = texture; // Texture 是值类型，写回数组后模型才引用新路径。
        }
        model.SaveTo(Path.Combine(output, Path.GetFileName(file)));
        count++;
    }
    catch (Exception exception)
    {
        Console.Error.WriteLine($"模型处理失败 {file}: {exception.Message}");
    }
}
Console.WriteLine($"完成模型格式化，成功: {count}，失败: {files.Length - count}");
return count == files.Length ? 0 : 1;

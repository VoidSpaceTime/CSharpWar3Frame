using FastMDX;
using IniParser;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NAudio.Wave;
using Serilog;
using War3FrameBuild.Extension;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace War3FrameBuild.CommandManager;

public partial class CommandManager
{
    public readonly Dictionary<string, (string Path, string Name, List<string> Extention)> AssetsTypes = new()
    {
        ["image"] = ("war3mapImage", "Image", [".tga", ".blp"]),
        ["model"] = ("war3mapModel", "Model", [".mdl", ".mdx"]),
        ["bgm"] = ("war3mapBgm", "Bgm", [".mp3", ".wav"]),
        ["vcm"] = ("war3mapVoice", "Vcm", [".mp3", ".wav"]),
        ["v3d"] = ("war3mapVoice", "V3d", [".mp3", ".wav"]),
        ["vwp"] = ("war3mapVwp", "Vwp", [".yaml"]),
        ["vwp-voice"] = ("war3mapVoice", "Vwp", [".mp3", ".wav"])
    };

    internal static string ResourcePath(string root, string relative)
    {
        var fullRoot = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        var full = Path.GetFullPath(Path.Combine(fullRoot, relative));
        if (!full.StartsWith(fullRoot, StringComparison.OrdinalIgnoreCase)) throw new ArgumentException("资源路径超出目标目录: " + relative);
        return full;
    }

    private static void CopyRequired(string source, string destination)
    {
        if (!File.Exists(source)) throw new FileNotFoundException("资源文件不存在", source);
        Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
        File.Copy(source, destination, true);
    }

    public (bool status, bool isWar3, string pickPath, string sourcePath) AnalysisFile(string kind, string path, bool skipDetect)
    {
        if (!AssetsTypes.TryGetValue(kind, out var support)) throw new ArgumentException("未知资源类型", nameof(kind));
        var relative = path.Replace('/', '\\');
        if (Path.GetExtension(relative).Length == 0) relative = Path.ChangeExtension(relative, support.Extention[0]);
        var source = ResourcePath(Path.Combine(Config.Assets, support.Path), relative);
        if (File.Exists(source))
        {
            var pick = Path.Combine(support.Path, relative);
            if (!skipDetect)
            {
                var destination = ResourcePath(Path.Combine(WorkingBuildPath, "resource"), pick);
                CopyRequired(source, destination);
                if (Path.GetExtension(source).ToLowerInvariant() is ".mdl" or ".mdx")
                    CopyTexturesToResourceSync(source, Path.GetDirectoryName(destination)!);
            }
            return (true, false, pick, source);
        }
        var resourceRelative = relative.StartsWith("resource\\", StringComparison.OrdinalIgnoreCase) ? relative[9..] : relative;
        source = ResourcePath(Path.Combine(PwdProject, "w3x", "resource"), resourceRelative);
        if (File.Exists(source))
        {
            if (!skipDetect) CopyRequired(source, ResourcePath(Path.Combine(WorkingBuildPath, "resource"), resourceRelative));
            return (true, false, resourceRelative, source);
        }
        var nativeRoots = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        { "units", "buildings", "abilities", "doodads", "replaceabletextures", "ui", "glues", "textures", "sound", "environment", "terrainart", "splats", "scripts", "objects", "sharedmodels" };
        return nativeRoots.Contains(relative.Split('\\')[0]) ? (true, true, relative, "") : (false, false, "", source);
    }

    internal void InstallTerrain(string name, string destination)
    {
        if (name.Length == 0) return;
        var source = ResourcePath(Path.Combine(Config.Assets, "war3mapTerrain"), name);
        foreach (var folder in new[] { "Cliff", "TerrainArt" })
            if (!Directory.Exists(Path.Combine(source, folder))) throw new DirectoryNotFoundException(Path.Combine(source, folder));
        DirectoryExtensions.CopyDir(Path.Combine(source, "Cliff"), Path.Combine(destination, "ReplaceableTextures", "Cliff"));
        DirectoryExtensions.CopyDir(Path.Combine(source, "TerrainArt"), Path.Combine(destination, "TerrainArt"));
    }

    private void InstallSelection(string name)
    {
        var source = ResourcePath(Path.Combine(Config.Assets, "war3mapSelection"), name);
        if (!Directory.Exists(source)) source = Path.Combine(Template, "lni", "assets", "Selection");
        DirectoryExtensions.CopyDir(source, Path.Combine(WorkingBuildPath, "resource", "ReplaceableTextures", "Selection"));
    }

    private void InstallFont(string name)
    {
        var source = name == "default" ? Path.Combine(Template, "lni", "assets", "ARHei2月液晶.ttf")
            : ResourcePath(Path.Combine(Config.Assets, "war3mapFont"), Path.HasExtension(name) ? name : name + ".ttf");
        CopyRequired(source, Path.Combine(WorkingBuildPath, "map", "fonts.ttf"));
    }

    internal void InstallLoading(string name)
    {
        var root = Path.Combine(Config.Assets, "war3MapLoading");
        var directory = ResourcePath(root, name);
        var parts = new List<(string Source, string Destination)>();
        if (Directory.Exists(directory))
        {
            foreach (var part in new[] { "pic", "bc", "bg" })
                parts.Add((Path.Combine(directory, part + ".tga"), "LoadingScreen" + part + ".tga"));
            parts.Add((Path.Combine(Template, "lni", "assets", "LoadingScreenDir.mdx"), "LoadingScreen.mdx"));
        }
        else
        {
            parts.Add((ResourcePath(root, Path.HasExtension(name) ? name : name + ".tga"), "LoadingScreen.tga"));
            parts.Add((Path.Combine(Template, "lni", "assets", "LoadingScreenFile.mdx"), "LoadingScreen.mdx"));
        }
        var iniPath = Path.Combine(WorkingBuildPath, "table", "w3i.ini");
        if (!File.Exists(iniPath)) throw new FileNotFoundException("载入图需要 w3i.ini", iniPath);
        foreach (var part in parts) if (!File.Exists(part.Source)) throw new FileNotFoundException("载入图组件不完整", part.Source);
        var parser = new FileIniDataParser();
        var data = parser.ReadFile(iniPath);
        foreach (var part in parts) CopyRequired(part.Source, Path.Combine(WorkingBuildPath, "resource", "Framework", part.Destination));
        data["载入图"]["路径"] = "Framework\\LoadingScreen.mdx";
        parser.WriteFile(iniPath, data);
    }

    private void InstallPreview(string name)
    {
        if (name.Length == 0) return;
        CopyRequired(ResourcePath(Path.Combine(Config.Assets, "war3mapPreview"), Path.HasExtension(name) ? name : name + ".tga"),
            Path.Combine(WorkingBuildPath, "resource", "war3mapPreview.tga"));
    }

    private void InstallUIKit(string name)
    {
        var directory = ResourcePath(Path.Combine(Config.Assets, "war3mapUI"), name);
        CopyRequired(Path.Combine(directory, "main.fdf"), ResourcePath(Path.Combine(WorkingBuildPath, "map", "UI"), name + ".fdf"));
        var assets = Path.Combine(directory, "assets");
        if (Directory.Exists(assets)) DirectoryExtensions.CopyDir(assets, ResourcePath(Path.Combine(WorkingBuildPath, "resource"), name));
    }

    private ExpressionSyntax BuildVoicePack(string folder)
    {
        var asset = AnalysisFile("vwp", folder, IsSkip);
        if (!asset.status || !File.Exists(asset.sourcePath)) throw new FileNotFoundException("声音包配置不存在", folder);
        var configuration = new DeserializerBuilder().Build().Deserialize<Dictionary<string, List<string>>>(File.ReadAllText(asset.sourcePath));
        if (configuration == null) throw new InvalidDataException("声音包配置为空");
        var entries = configuration.OrderBy(k => k.Key, StringComparer.Ordinal).Select(k =>
            "{ " + Microsoft.CodeAnalysis.CSharp.SymbolDisplay.FormatLiteral(k.Key, true) + ", new global::System.Collections.Generic.List<(string pickPath, int duration)> { "
            + string.Join(", ", k.Value.Select(p => "(" + Microsoft.CodeAnalysis.CSharp.SymbolDisplay.FormatLiteral(p, true) + ", " + GetWar3AuidoDuration(p) + ")")) + " } }");
        return SyntaxFactory.ParseExpression("new global::War3Frame.Assets.VwpSound(" + Microsoft.CodeAnalysis.CSharp.SymbolDisplay.FormatLiteral(folder, true)
            + ", new global::System.Collections.Generic.Dictionary<string, global::System.Collections.Generic.List<(string pickPath, int duration)>> { " + string.Join(", ", entries) + " })");
    }

    public int GetAudioFileDuration(string file)
    {
        if (!File.Exists(file)) throw new FileNotFoundException("音频文件不存在", file);
        using WaveStream reader = Path.GetExtension(file).ToLowerInvariant() switch
        { ".wav" => new WaveFileReader(file), ".mp3" => new Mp3FileReader(file), _ => throw new InvalidDataException("不支持的音频格式: " + file) };
        return (int)reader.TotalTime.TotalMilliseconds;
    }

    public int GetWar3AuidoDuration(string path)
    {
        if (War3SoundsYaml == null)
        {
            var yaml = Path.Combine(Template, "war3sounds.yaml");
            if (!File.Exists(yaml)) throw new FileNotFoundException("原生音频时长配置不存在", yaml);
            War3SoundsYaml = new DeserializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).Build().Deserialize<War3Sounds>(File.ReadAllText(yaml));
        }
        var sound = War3SoundsYaml?.sounds?.FirstOrDefault(s => string.Equals(s.path, path, StringComparison.OrdinalIgnoreCase));
        return sound?.duration ?? throw new InvalidDataException("未配置原生音频时长: " + path);
    }

    public static bool CopyTexturesToResourceSync(string modelFile, string targetModelFolder)
    {
        if (!File.Exists(modelFile) || !Directory.Exists(targetModelFolder)) return false;
        var model = new MDX(modelFile);
        foreach (var texture in model.Textures ?? [])
        {
            if (texture.ReplaceableId != 0 || string.IsNullOrEmpty(texture.Name)) continue;
            var source = ResourcePath(Path.GetDirectoryName(modelFile)!, texture.Name);
            if (File.Exists(source)) CopyRequired(source, ResourcePath(targetModelFolder, texture.Name));
        }
        return true;
    }
}

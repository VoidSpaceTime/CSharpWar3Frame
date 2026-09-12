using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Serilog;

namespace War3FrameBuild.CommandManager;

public partial class CommandManager
{
    public bool WE()
    {
        var executable = new[] { "KKWE.exe", "WE.exe" }.Select(name => Path.Combine(Config.We, name)).FirstOrDefault(File.Exists);
        if (executable == null) throw new FileNotFoundException("WE 编辑器不存在", Config.We);
        string? map = null;
        if (!string.IsNullOrEmpty(ProjectName))
        {
            if (!Directory.Exists(PwdProject)) { Log.Error("项目不存在: {Project}", ProjectName); return false; }
            SyncW3xFile(false);
            var assets = Path.Combine(PwdProject, "Assets");
            if (Directory.Exists(assets))
            {
                var terrains = Directory.GetFiles(assets, "*.cs", SearchOption.AllDirectories)
                    .SelectMany(p => CSharpSyntaxTree.ParseText(File.ReadAllText(p)).GetRoot().DescendantNodes().OfType<InvocationExpressionSyntax>())
                    .Where(n => n.Expression is MemberAccessExpressionSyntax member && member.Name.Identifier.Text == "AddTerrain")
                    .Select(n => n.ArgumentList.Arguments.FirstOrDefault()?.Expression as LiteralExpressionSyntax)
                    .Where(n => n?.Token.Value is string).Select(n => n!.Token.ValueText).Where(s => s.Length > 0).Distinct().ToArray();
                if (terrains.Length > 1) throw new InvalidDataException("项目只能引用一套地形贴图");
                if (terrains.Length == 1) InstallTerrain(terrains[0], Path.Combine(TempProjectBuildPath, "resource"));
            }
            PackWeW3xFile();
            map = Path.Combine(Temp, ProjectName + ".w3x");
        }
        // 编辑器为长生命周期 UI：不重定向无人读取的输出，也不等待其关闭。
        var start = new ProcessStartInfo(executable) { UseShellExecute = false, WorkingDirectory = Config.We };
        if (map != null) { start.ArgumentList.Add("-loadfile"); start.ArgumentList.Add(map); }
        using var process = Process.Start(start);
        if (process == null) return false;
        if (map != null) File.WriteAllText(Path.Combine(TempProjectBuildPath, ".we"), "");
        Log.Information("WE 编辑器已启动");
        return true;
    }
}

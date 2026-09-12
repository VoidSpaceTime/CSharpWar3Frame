using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace War3Frame.Regression;

internal static class CompilationTest
{
    internal static readonly string Repository = FindRepository();
    internal static readonly CSharpParseOptions ParseOptions = new(LanguageVersion.Preview);
    internal static CSharpCompilation Create(params string[] sources)
    {
        var paths = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(Path.PathSeparator)
            .Concat(Directory.GetFiles(AppContext.BaseDirectory, "*.dll"))
            .Distinct(StringComparer.OrdinalIgnoreCase);
        return CSharpCompilation.Create("RegressionGenerated" + Guid.NewGuid().ToString("N"),
            sources.Select(s => CSharpSyntaxTree.ParseText(s, ParseOptions)),
            paths.Select(p => MetadataReference.CreateFromFile(p)),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true));
    }

    internal static Assembly Emit(Compilation compilation)
    {
        using var stream = new MemoryStream();
        var result = compilation.Emit(stream);
        Check.That(result.Success, string.Join(Environment.NewLine, result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error)));
        return Assembly.Load(stream.ToArray());
    }

    internal static string Read(string path) => File.ReadAllText(Path.Combine(Repository, path));
    private static string FindRepository()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory != null; directory = directory.Parent)
            if (File.Exists(Path.Combine(directory.FullName, "CSharpWar3Frame.slnx"))) return directory.FullName;
        throw new DirectoryNotFoundException("Regression tests must run from a repository build.");
    }
}

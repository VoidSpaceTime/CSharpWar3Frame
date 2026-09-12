using System.Reflection;
using War3Frame;

namespace War3Frame.Regression;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        var selected = args.Length == 0 ? null : args[0];
        var tests = new List<RegressionCase>();
        foreach (var type in typeof(Program).Assembly.GetTypes()
                     .Where(t => t.Namespace == "War3Frame.Scripts.Process")
                     .OrderBy(t => t.Name, StringComparer.Ordinal))
        {
            var initialize = type.GetMethod("Initialize", [typeof(JPlayer)]);
            if (initialize == null) continue;
            tests.Add(new RegressionCase("existing", type.Name, () =>
            {
                initialize.Invoke(null, [default(JPlayer)]);
                return Task.CompletedTask;
            }));
        }
        RegisterTests(tests);
        var failures = 0;
        var executed = 0;
        foreach (var test in tests.Where(t => selected == null || t.Group == selected))
        {
            executed++;
            try
            {
                await test.Run();
                Console.WriteLine($"PASS {test.Group}/{test.Name}");
            }
            catch (Exception exception)
            {
                failures++;
                var error = exception is TargetInvocationException { InnerException: not null } wrapped
                    ? wrapped.InnerException : exception;
                Console.Error.WriteLine($"FAIL {test.Group}/{test.Name}: {error}");
            }
        }
        Console.WriteLine($"Executed {executed}; passed {executed - failures}; failed {failures}.");
        return executed == 0 || failures != 0 ? 1 : 0;
    }

    private static void RegisterTests(List<RegressionCase> tests)
    {
        RuntimeRegression.Register(tests);
        DomainRegression.Register(tests);
        GeneratorRegression.Register(tests);
        NativeProjectionRegression.Register(tests);
    }
}

internal sealed record RegressionCase(string Group, string Name, Func<Task> Run)
{
    internal RegressionCase(string group, string name, Action run)
        : this(group, name, () => { run(); return Task.CompletedTask; }) { }
}

internal static class Check
{
    internal static void That(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    internal static void Near(double actual, double expected, string message, double tolerance = 0.0001)
        => That(Math.Abs(actual - expected) <= tolerance, $"{message}: expected {expected}, got {actual}");
}

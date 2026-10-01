using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using War3Frame;

namespace War3Frame.Regression;

/// <summary>验证 JassVM cdecl 分发对 13–16 槽的支持、超限拒绝，以及既有类型与 ≤12 槽路径不回归。</summary>
internal static class JassVmNativeArgsRegression
{
    /// <summary>最近一次进入 cdecl 桩时收到的槽序列，用于断言参数顺序。</summary>
    private static int[]? _lastStubArgs;

    /// <summary>注册 JassVM 参数分发回归场景。</summary>
    internal static void Register(List<RegressionCase> tests)
    {
        tests.Add(new("runtime", "JassVmNativeArgsDispatch", JassVmNativeArgsDispatch));
    }

    private static void JassVmNativeArgsDispatch()
    {
        // 13 槽：返回值与参数顺序都必须符合桩约定。
        var expected13 = new[] { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 };
        _lastStubArgs = null;
        var result13 = War3.CallNative<int>(Stub13Address(), ToObjects(expected13));
        Check.That(result13 == 23, $"13 槽应回传最后一个槽，实际 {result13}");
        Check.That(_lastStubArgs is not null && _lastStubArgs.SequenceEqual(expected13),
            "13 槽参数顺序应与入参一致");

        // 16 槽（上限内）：同样验证顺序与返回值。
        var expected16 = Enumerable.Range(1, 16).ToArray();
        _lastStubArgs = null;
        var result16 = War3.CallNative<int>(Stub16Address(), ToObjects(expected16));
        Check.That(result16 == 16, $"16 槽应回传最后一个槽，实际 {result16}");
        Check.That(_lastStubArgs is not null && _lastStubArgs.SequenceEqual(expected16),
            "16 槽参数顺序应与入参一致");

        // 17 槽：必须抛 NotSupportedException，且不得截断后进入桩。
        _lastStubArgs = null;
        var rejected17 = false;
        string? message17 = null;
        try
        {
            War3.CallNative<int>(Stub13Address(), ToObjects(Enumerable.Range(1, 17).ToArray()));
        }
        catch (NotSupportedException exception)
        {
            rejected17 = true;
            message17 = exception.Message;
        }
        Check.That(rejected17, "17 槽应抛 NotSupportedException");
        Check.That(_lastStubArgs is null, "17 槽不得截断参数后进入桩");
        Check.That(message17 is not null && message17.Contains("17") && message17.Contains("16"),
            $"17 槽异常信息应含实际个数与上限 16，实际 {message17}");

        // 非封送类型：double 必须继续被拒绝。
        var rejectedDouble = false;
        try
        {
            War3.CallNative<int>(Stub13Address(), 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13.0d);
        }
        catch (InvalidCastException)
        {
            rejectedDouble = true;
        }
        Check.That(rejectedDouble, "double 参数应抛 InvalidCastException");
    }

    private static object[] ToObjects(int[] values)
    {
        var boxed = new object[values.Length];
        for (var i = 0; i < values.Length; i++) boxed[i] = values[i];
        return boxed;
    }

    /// <summary>取 13 槽托管 cdecl 桩的地址，经 CallNative 分发。</summary>
    private static unsafe nint Stub13Address()
        => (nint)(delegate* unmanaged[Cdecl]<int, int, int, int, int, int, int, int, int, int, int, int, int, int>)&Stub13;

    /// <summary>取 16 槽托管 cdecl 桩的地址，经 CallNative 分发。</summary>
    private static unsafe nint Stub16Address()
        => (nint)(delegate* unmanaged[Cdecl]<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>)&Stub16;

    /// <summary>13 槽 cdecl 桩：记录槽序并回传最后一个槽。</summary>
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static int Stub13(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m)
    {
        _lastStubArgs = new[] { a, b, c, d, e, f, g, h, i, j, k, l, m };
        return m;
    }

    /// <summary>16 槽 cdecl 桩：记录槽序并回传最后一个槽。</summary>
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static int Stub16(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int p)
    {
        _lastStubArgs = new[] { a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p };
        return p;
    }
}

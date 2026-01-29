using System;
using System.Runtime.InteropServices;

const string DllPath = @"G:\CSharp\CSharpWar3Frame\Projects\test\obj\Release\net10.0\win-x86\dnne\bin\testNE.dll";

Console.WriteLine($"测试加载: {DllPath}");
Console.WriteLine($"当前进程架构: {(IntPtr.Size == 4 ? "32-bit" : "64-bit")}");

try
{
    IntPtr handle = NativeLibrary.Load(DllPath);
    Console.WriteLine($"DLL 加载成功: 0x{handle:X}");

    // 尝试找函数
    foreach (var name in new[] { "main", "_main", "_main@0", "MainJIT", "_MainJIT@0" })
    {
        if (NativeLibrary.TryGetExport(handle, name, out IntPtr ptr))
            Console.WriteLine($"✓ 找到: {name} @ 0x{ptr:X}");
        else
            Console.WriteLine($"✗ 未找到: {name}");
    }

    // 调用函数
    if (NativeLibrary.TryGetExport(handle, "_main@0", out IntPtr funcPtr))
    {
        Console.WriteLine("\n调用 _main@0...");
        var func = Marshal.GetDelegateForFunctionPointer<MainDelegate>(funcPtr);
        int result = func();
        Console.WriteLine($"返回值: {result}");
    }

    NativeLibrary.Free(handle);
    Console.WriteLine("\nDLL 已卸载");
}
catch (Exception ex)
{
    Console.WriteLine($"错误: {ex.Message}");
    Console.WriteLine(ex.ToString());
}

Console.WriteLine("\n按任意键退出...");
Console.ReadKey();

[UnmanagedFunctionPointer(CallingConvention.StdCall)]
delegate int MainDelegate();

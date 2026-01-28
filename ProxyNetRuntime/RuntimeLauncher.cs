using Serilog;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ProxyNetRuntime
{
    /// <summary>
    /// .NET 运行时启动器 - War3 调用的入口点
    /// </summary>
    public static unsafe class RuntimeLauncher
    {
        private static bool _runtimeLoaded = false;
        private static nint _mainFunctionPtr = 0;

        // 默认的 JIT DLL 配置
        private const string DEFAULT_JIT_DLL_NAME = "GameLogic.dll";
        private const string DEFAULT_ENTRY_TYPE = "War3Frame.Game";
        private const string DEFAULT_ENTRY_METHOD = "MainJIT";

        /// <summary>
        /// War3 调用的主入口点 (AOT 导出)
        /// </summary>
        [UnmanagedCallersOnly(EntryPoint = "main")]
        public static int Main()
        {

            // AOT 兼容的 Serilog 配置
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File(
                    path: "logs/proxynetruntime-.log",
                    rollingInterval: Serilog.RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    flushToDiskInterval: TimeSpan.FromSeconds(1)  // 每秒刷新到磁盘
                )
                .CreateLogger();
            Log.Information("[ProxyNetRuntime] Logger initialized.");

            try
            {
                // 获取当前 DLL 所在目录
                var currentDir = AppContext.BaseDirectory;
                var jitDllPath = Path.Combine(currentDir, "JITPlugins", DEFAULT_JIT_DLL_NAME);
                var runtimeConfigPath = Path.Combine(currentDir, "JITPlugins", Path.GetFileNameWithoutExtension(DEFAULT_JIT_DLL_NAME) + ".runtimeconfig.json");

                Log.Debug($"[ProxyNetRuntime] BaseDirectory: {currentDir}");
                Log.Debug($"[ProxyNetRuntime] JIT DLL Path: {jitDllPath}");
                Log.Debug($"[ProxyNetRuntime] RuntimeConfig Path: {runtimeConfigPath}");

                // 检查文件是否存在
                if (!File.Exists(jitDllPath))
                {
                    Log.Error($"[ProxyNetRuntime] JIT DLL not found: {jitDllPath}");
                    return FallbackToAot();
                }
                Log.Debug("[ProxyNetRuntime] JIT DLL found.");

                if (!File.Exists(runtimeConfigPath))
                {
                    Log.Error($"[ProxyNetRuntime] runtimeconfig.json not found: {runtimeConfigPath}");
                    return -1;
                }
                Log.Debug("[ProxyNetRuntime] RuntimeConfig found.");

                // 初始化 hostfxr
                Log.Debug("[ProxyNetRuntime] Initializing hostfxr...");
                HostFxr.Initialize(runtimeConfigPath);
                Log.Debug("[ProxyNetRuntime] hostfxr initialized.");

                // 加载 JIT DLL 并获取入口函数
                Log.Debug($"[ProxyNetRuntime] Loading {DEFAULT_ENTRY_TYPE}.{DEFAULT_ENTRY_METHOD}...");
                // 使用 UNMANAGEDCALLERSONLY_METHOD 表示方法标记了 [UnmanagedCallersOnly]
                _mainFunctionPtr = HostFxr.LoadAssemblyAndGetFunctionPointer(
                    jitDllPath,
                    DEFAULT_ENTRY_TYPE,
                    DEFAULT_ENTRY_METHOD,
                    HostFxr.UNMANAGEDCALLERSONLY_METHOD);

                if (_mainFunctionPtr == 0)
                {
                    Log.Error("[ProxyNetRuntime] Failed to get function pointer");
                    return -1;
                }

                _runtimeLoaded = true;
                Log.Information("[ProxyNetRuntime] JIT DLL loaded successfully, calling entry point...");

                // 调用 JIT DLL 的入口函数
                var mainFunc = (delegate* unmanaged[Stdcall]<int>)_mainFunctionPtr;
                return mainFunc();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[ProxyNetRuntime] Exception occurred");
                return -1;
            }
            finally
            {
                Log.Debug("[ProxyNetRuntime] Exiting...");
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        /// 使用自定义配置初始化运行时
        /// </summary>
        [UnmanagedCallersOnly(EntryPoint = "init_runtime")]
        public static int InitRuntime(nint runtimeConfigPathPtr)
        {
            try
            {
                var runtimeConfigPath = Marshal.PtrToStringUni(runtimeConfigPathPtr);
                if (string.IsNullOrEmpty(runtimeConfigPath))
                    return -1;

                HostFxr.Initialize(runtimeConfigPath);
                _runtimeLoaded = true;
                return 0;
            }
            catch (Exception ex)
            {
                //Log.Debug($"[ProxyNetRuntime] InitRuntime failed: {ex.Message}");
                return -1;
            }
        }

        /// <summary>
        /// 加载指定的托管程序集并调用指定方法
        /// </summary>
        [UnmanagedCallersOnly(EntryPoint = "load_and_run")]
        public static int LoadAndRun(nint assemblyPathPtr, nint typeNamePtr, nint methodNamePtr)
        {
            try
            {
                if (!_runtimeLoaded)
                {
                    //Log.Debug("[ProxyNetRuntime] Runtime not initialized");
                    return -1;
                }

                var assemblyPath = Marshal.PtrToStringUni(assemblyPathPtr);
                var typeName = Marshal.PtrToStringUni(typeNamePtr);
                var methodName = Marshal.PtrToStringUni(methodNamePtr);

                if (string.IsNullOrEmpty(assemblyPath) ||
                    string.IsNullOrEmpty(typeName) ||
                    string.IsNullOrEmpty(methodName))
                    return -1;

                var fnPtr = HostFxr.LoadAssemblyAndGetFunctionPointer(
                    assemblyPath, typeName, methodName,
                    "ProxyNetRuntime.MainDelegate, ProxyNetRuntime");

                var func = (delegate* unmanaged[Stdcall]<int>)fnPtr;
                return func();
            }
            catch (Exception ex)
            {
                //Log.Debug($"[ProxyNetRuntime] LoadAndRun failed: {ex.Message}");
                return -1;
            }
        }

        /// <summary>
        /// 关闭运行时
        /// </summary>
        [UnmanagedCallersOnly(EntryPoint = "shutdown")]
        public static void Shutdown()
        {
            try
            {
                HostFxr.Close();
                _runtimeLoaded = false;
                //Log.Debug("[ProxyNetRuntime] Runtime shutdown complete");
            }
            catch (Exception ex)
            {
                //Log.Debug($"[ProxyNetRuntime] Shutdown failed: {ex.Message}");
            }
        }

        /// <summary>
        /// 当 JIT DLL 不存在时的回退处理
        /// </summary>
        private static int FallbackToAot()
        {
            // 这里可以添加 AOT 模式的回退逻辑
            // 比如直接调用 War3Frame.Game.MainAOT() (如果链接了的话)
            //Log.Debug("[ProxyNetRuntime] No fallback available in pure launcher mode");
            return -1;
        }
    }

    /// <summary>
    /// JIT DLL 入口方法的委托类型
    /// </summary>
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate int MainDelegate();
}

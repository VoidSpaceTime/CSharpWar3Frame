using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Serilog;

namespace ProxyNetRuntime
{
    /// <summary>
    /// hostfxr API 绑定 - 用于在 AOT 进程中启动 .NET CLR 运行时
    /// </summary>
    public static unsafe class HostFxr
    {
        // int hostfxr_initialize_for_runtime_config(runtime_config_path, parameters, host_context_handle)
        private static delegate* unmanaged[Stdcall]<nint, nint, nint*, int> _hostfxr_initialize_for_runtime_config;
        // int hostfxr_get_runtime_delegate(host_context_handle, type, delegate)
        private static delegate* unmanaged[Stdcall]<nint, int, nint*, int> _hostfxr_get_runtime_delegate;
        private static delegate* unmanaged[Stdcall]<nint, int> _hostfxr_close;

        // 运行时委托类型
        private const int HDT_LOAD_ASSEMBLY_AND_GET_FUNCTION_POINTER = 5;

        // 特殊值：表示方法使用 [UnmanagedCallersOnly] 属性
        public const string UNMANAGEDCALLERSONLY_METHOD = "\u0001"; // 使用特殊字符串表示 -1

        private static nint _hostfxrHandle;
        private static nint _contextHandle;
        private static bool _initialized;

        /// <summary>
        /// 初始化 hostfxr 并加载 .NET 运行时
        /// </summary>
        /// <param name="runtimeConfigPath">runtimeconfig.json 文件路径</param>
        public static void Initialize(string runtimeConfigPath)
        {
            if (_initialized) return;

            // 1. 加载 hostfxr.dll
            var hostfxrPath = GetHostFxrPath();
            _hostfxrHandle = NativeLibrary.Load(hostfxrPath);

            if (_hostfxrHandle == 0)
                throw new Exception($"Failed to load hostfxr from: {hostfxrPath}");

            // 2. 获取 hostfxr 函数指针
            _hostfxr_initialize_for_runtime_config = (delegate* unmanaged[Stdcall]<nint, nint, nint*, int>)
                NativeLibrary.GetExport(_hostfxrHandle, "hostfxr_initialize_for_runtime_config");

            _hostfxr_get_runtime_delegate = (delegate* unmanaged[Stdcall]<nint, int, nint*, int>)
                NativeLibrary.GetExport(_hostfxrHandle, "hostfxr_get_runtime_delegate");

            _hostfxr_close = (delegate* unmanaged[Stdcall]<nint, int>)
                NativeLibrary.GetExport(_hostfxrHandle, "hostfxr_close");

            // 3. 初始化运行时
            // 确保使用绝对路径
            var absoluteConfigPath = Path.GetFullPath(runtimeConfigPath);
            Log.Debug($"[ProxyNetRuntime] RuntimeConfig absolute path: {absoluteConfigPath}");

            // 验证文件存在且可读
            if (!File.Exists(absoluteConfigPath))
                throw new Exception($"RuntimeConfig not found: {absoluteConfigPath}");

            var configContent = File.ReadAllText(absoluteConfigPath);
            Log.Debug($"[ProxyNetRuntime] RuntimeConfig content length: {configContent.Length}");
            Log.Debug($"[ProxyNetRuntime] RuntimeConfig content: {configContent}");

            nint contextHandle;
            fixed (char* configPath = absoluteConfigPath)
            {
                // 参数顺序: runtime_config_path, parameters (null), host_context_handle
                int result = _hostfxr_initialize_for_runtime_config(
                    (nint)configPath, 0, &contextHandle);

                // hostfxr 返回码说明:
                // 0x00000000 = Success
                // 0x00000001 = Success_HostAlreadyInitialized  
                // 0x00000002 = Success_DifferentRuntimeProperties
                // 0x80008081 = InvalidArgFailure（通常是 runtimeconfig.json 格式问题）
                // 0x80008091 = FrameworkMissingFailure（找不到指定的 .NET 版本）
                // 0x80008096 = CoreHostIncompatibleConfig（配置不兼容）

                if (result < 0) // 负数（高位为1）表示错误
                {
                    string errorMsg = result switch
                    {
                        unchecked((int)0x80008081) => "InvalidArgFailure - runtimeconfig.json 格式错误或参数无效",
                        unchecked((int)0x80008091) => "FrameworkMissingFailure - 找不到指定的 .NET Runtime 版本，请检查 runtimeconfig.json 中的版本号",
                        unchecked((int)0x80008096) => "CoreHostIncompatibleConfig - 运行时配置不兼容",
                        unchecked((int)0x80008082) => "CoreClrResolveFailure - 无法找到 coreclr.dll",
                        unchecked((int)0x80008083) => "CoreClrBindFailure - 无法绑定到 coreclr.dll",
                        _ => $"未知错误"
                    };
                    throw new Exception($"hostfxr_initialize_for_runtime_config failed: 0x{result:X} ({errorMsg})");
                }

                Log.Debug($"[ProxyNetRuntime] hostfxr init result: 0x{result:X}");
            }
            _contextHandle = contextHandle;

            _initialized = true;
            Log.Information("[ProxyNetRuntime] .NET Runtime initialized successfully");
        }

        /// <summary>
        /// 加载托管程序集并获取指定方法的函数指针
        /// </summary>
        /// <param name="assemblyPath">程序集路径 (.dll)</param>
        /// <param name="typeName">完整类型名 (命名空间.类名)</param>
        /// <param name="methodName">方法名</param>
        /// <param name="delegateTypeName">委托类型名（可选，null 表示默认签名）</param>
        /// <returns>函数指针</returns>
        public static nint LoadAssemblyAndGetFunctionPointer(
            string assemblyPath,
            string typeName,
            string methodName,
            string? delegateTypeName = null)
        {
            if (!_initialized)
                throw new InvalidOperationException("Runtime not initialized. Call Initialize() first.");

            // 获取 load_assembly_and_get_function_pointer 委托
            nint loadAssemblyDelegate;
            // 参数: host_context_handle, delegate_type, delegate_out
            int result = _hostfxr_get_runtime_delegate(
                _contextHandle,
                HDT_LOAD_ASSEMBLY_AND_GET_FUNCTION_POINTER,
                &loadAssemblyDelegate);

            if (result != 0)
                throw new Exception($"hostfxr_get_runtime_delegate failed: 0x{result:X}");

            Log.Debug("[ProxyNetRuntime] Got load_assembly_and_get_function_pointer delegate");

            // 确保使用绝对路径
            var absoluteAssemblyPath = Path.GetFullPath(assemblyPath);
            Log.Debug($"[ProxyNetRuntime] Loading assembly: {absoluteAssemblyPath}");
            Log.Debug($"[ProxyNetRuntime] Type: {typeName}");
            Log.Debug($"[ProxyNetRuntime] Method: {methodName}");
            Log.Debug($"[ProxyNetRuntime] DelegateType: {delegateTypeName ?? "(null - using default signature)"}");

            if (!File.Exists(absoluteAssemblyPath))
            {
                throw new Exception($"Assembly file not found: {absoluteAssemblyPath}");
            }

            // 调用委托加载程序集
            var loadAssembly = (delegate* unmanaged[Stdcall]<nint, nint, nint, nint, nint, nint*, int>)loadAssemblyDelegate;

            nint fnPtr;
            fixed (char* assembly = absoluteAssemblyPath)
            fixed (char* type = typeName)
            fixed (char* method = methodName)
            {
                // 处理委托类型参数:
                // - null 或 空: 使用默认签名 (IntPtr args, int sizeBytes)
                // - UNMANAGEDCALLERSONLY_METHOD: 传入 -1，表示方法标记了 [UnmanagedCallersOnly]
                // - 其他: 作为程序集限定的委托类型名
                nint delegateTypeArg;
                if (string.IsNullOrEmpty(delegateTypeName))
                {
                    delegateTypeArg = 0; // 默认签名
                }
                else if (delegateTypeName == UNMANAGEDCALLERSONLY_METHOD)
                {
                    delegateTypeArg = -1; // UnmanagedCallersOnly
                }
                else
                {
                    // 这种情况下委托类型必须在目标程序集中定义
                    fixed (char* delegateType = delegateTypeName)
                    {
                        delegateTypeArg = (nint)delegateType;
                    }
                }

                result = loadAssembly(
                    (nint)assembly,
                    (nint)type,
                    (nint)method,
                    delegateTypeArg,
                    0,
                    &fnPtr);
            }

            if (result != 0)
            {
                string errorMsg = result switch
                {
                    unchecked((int)0x80070002) => "FILE_NOT_FOUND - 程序集或类型不存在",
                    unchecked((int)0x80131522) => "TypeLoadException - 找不到指定的类型",
                    unchecked((int)0x80131523) => "MissingMethodException - 找不到指定的方法",
                    _ => "未知错误"
                };
                throw new Exception($"load_assembly_and_get_function_pointer failed: 0x{result:X} ({errorMsg})");
            }

            return fnPtr;
        }

        /// <summary>
        /// 关闭运行时
        /// </summary>
        public static void Close()
        {
            if (_contextHandle != 0)
            {
                _hostfxr_close(_contextHandle);
                _contextHandle = 0;
            }
            if (_hostfxrHandle != 0)
            {
                NativeLibrary.Free(_hostfxrHandle);
                _hostfxrHandle = 0;
            }
            _initialized = false;
        }

        /// <summary>
        /// 查找 hostfxr.dll 路径
        /// </summary>
        private static string GetHostFxrPath()
        {
            // 优先从环境变量获取
            var dotnetRoot = Environment.GetEnvironmentVariable("DOTNET_ROOT");

            if (string.IsNullOrEmpty(dotnetRoot))
            {
                // 根据当前进程架构选择正确的路径
                // 32 位进程需要使用 x86 的 .NET
                if (IntPtr.Size == 4) // 32 位进程
                {
                    // 尝试 DOTNET_ROOT(x86) 环境变量
                    dotnetRoot = Environment.GetEnvironmentVariable("DOTNET_ROOT(x86)");
                    if (string.IsNullOrEmpty(dotnetRoot))
                    {
                        // 使用 x86 默认路径
                        dotnetRoot = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                            "dotnet");
                    }
                }
                else // 64 位进程
                {
                    dotnetRoot = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                        "dotnet");
                }
            }

            Log.Debug($"[ProxyNetRuntime] DOTNET_ROOT: {dotnetRoot}");
            Log.Debug($"[ProxyNetRuntime] Process is {(IntPtr.Size == 4 ? "32-bit" : "64-bit")}");

            var hostFxrDir = Path.Combine(dotnetRoot, "host", "fxr");

            if (!Directory.Exists(hostFxrDir))
                throw new Exception($"hostfxr directory not found: {hostFxrDir}");

            // 获取最新版本（使用语义版本排序，而非字符串排序）
            var versions = Directory.GetDirectories(hostFxrDir);
            if (versions.Length == 0)
                throw new Exception("No hostfxr version found");

            // 按版本号排序（修复 "9.0" vs "10.0" 的字符串排序问题）
            var latestVersion = versions
                .Select(v => new { Path = v, Version = TryParseVersion(Path.GetFileName(v)) })
                .Where(x => x.Version != null)
                .OrderByDescending(x => x.Version)
                .FirstOrDefault()?.Path;

            if (latestVersion == null)
                throw new Exception("No valid hostfxr version found");

            var hostfxrPath = Path.Combine(latestVersion, "hostfxr.dll");

            if (!File.Exists(hostfxrPath))
                throw new Exception($"hostfxr.dll not found: {hostfxrPath}");

            Log.Debug($"[ProxyNetRuntime] Using hostfxr: {hostfxrPath}");
            return hostfxrPath;
        }

        /// <summary>
        /// 尝试解析版本号字符串
        /// </summary>
        private static Version? TryParseVersion(string versionString)
        {
            // 处理预览版本号，如 "10.0.0-preview.1"
            var parts = versionString.Split('-')[0];
            return Version.TryParse(parts, out var version) ? version : null;
        }
    }
}

using System.Runtime.InteropServices;

namespace War3Frame;

public partial class War3
{
    public enum TypeVersion
    {
        None = 0,
        V24E = 6387,
        V27A = 52240
    }

    private static readonly Lazy<TypeVersion> _Version = new(() =>
    {
        unsafe
        {
            var module = GetModuleHandleW("game.dll");
            var moduleSize = GetModuleSize(module);
            if (moduleSize == 0) return TypeVersion.None;

            var pattern = "Warcraft III (build ";
            var pBase = (byte*)module;
            var end = pBase + moduleSize - pattern.Length;

            fixed (char* patternPtr = pattern)
            {
                for (var p = pBase; p < end; p += 4)
                {
                    var match = true;
                    for (var i = 0; i < pattern.Length; i++)
                        if (p[i] != patternPtr[i])
                        {
                            match = false;
                            break;
                        }

                    if (match)
                    {
                        var versionPtr = p + pattern.Length;
                        var result = 0;
                        while (*versionPtr >= '0' && *versionPtr <= '9')
                        {
                            result = result * 10 + (*versionPtr - '0');
                            versionPtr++;
                        }

                        if ((TypeVersion)result == TypeVersion.V24E) return TypeVersion.V24E;
                        if ((TypeVersion)result == TypeVersion.V27A) return TypeVersion.V27A;
                        return TypeVersion.None;
                    }
                }
            }

            return TypeVersion.None;
        }
    });

    private static readonly Lazy<nint> JassEnvAddress = new(() => SelectVersion(new Select(V27A: 0xBE3740)));

    private static readonly Lazy<nint> CGameUIAddress = new(() => SelectVersion(new Select(V27A: 0xBE6350)));

    [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf16)]
    private static partial nint GetModuleHandleW(string lpModuleName);

    private static uint GetModuleSize(nint moduleHandle)
    {
        unsafe
        {
            if (moduleHandle == 0) return 0;

            var pDosHeader = (ushort*)moduleHandle;
            if (*pDosHeader != 0x5A4D)
                return 0;

            var pBase = (byte*)moduleHandle;
            var e_lfanew = *(int*)(pBase + 0x3C);

            var pNtHeader = (uint*)(pBase + e_lfanew);
            if (*pNtHeader != 0x00004550)
                return 0;

            var pOptionalHeader = pBase + e_lfanew + 0x18;
            var sizeOfImage = *(uint*)(pOptionalHeader + 0x38);

            return sizeOfImage;
        }
    }

    public static TypeVersion GetVersion()
    {
        return _Version.Value;
    }

    private static nint SelectVersion(Select data)
    {
        var moduleHandle = GetModuleHandleW("game.dll");
        if (moduleHandle == 0)
        {
            // 宿主模块不可用时直接安全失败，避免伪造 game.dll + 0 地址。
            return 0;
        }

        var version = GetVersion();
        var offset = version switch
        {
            TypeVersion.V24E => data.V24E,
            TypeVersion.V27A => data.V27A,
            _ => 0
        };

        if (offset == 0)
        {
            // 当前版本没有有效偏移时直接安全失败，后续调用方会按 0 地址处理。
            return 0;
        }

        return moduleHandle + offset;
    }

    // private static readonly nint JassEnvAddress = SelectVersion(new(V27A: 0xBE3740));
    private static nint GetJassEnvAddress()
    {
        return JassEnvAddress.Value;
    }

    private static nint GetCGameUIAddress()
    {
        return CGameUIAddress.Value;
    }

    private struct Select(int V24E = 0, int V27A = 0)
    {
        public readonly int V24E = V24E;
        public readonly int V27A = V27A;
    }
}

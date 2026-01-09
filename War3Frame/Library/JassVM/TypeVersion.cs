using System.Runtime.InteropServices;

namespace War3Frame
{
    public partial class War3
    {
        [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf16)]
        private static partial nint GetModuleHandleW(string lpModuleName);

        private static uint GetModuleSize(nint moduleHandle)
        {
            unsafe
            {
                if (moduleHandle == 0) return 0;

                ushort* pDosHeader = (ushort*)moduleHandle;
                if (*pDosHeader != 0x5A4D)
                    return 0;

                byte* pBase = (byte*)moduleHandle;
                int e_lfanew = *(int*)(pBase + 0x3C);

                uint* pNtHeader = (uint*)(pBase + e_lfanew);
                if (*pNtHeader != 0x00004550)
                    return 0;

                byte* pOptionalHeader = pBase + e_lfanew + 0x18;
                uint sizeOfImage = *(uint*)(pOptionalHeader + 0x38);

                return sizeOfImage;
            }
        }

        public enum TypeVersion
        {
            None = 0,
            V24E = 6387,
            V27A = 52240,
        }

        private static readonly Lazy<TypeVersion> _Version = new(() =>
        {
            unsafe
            {
                nint module = GetModuleHandleW("game.dll");
                uint moduleSize = GetModuleSize(module);
                if (moduleSize == 0) return TypeVersion.None;

                string pattern = "Warcraft III (build ";
                byte* pBase = (byte*)module;
                byte* end = pBase + moduleSize - pattern.Length;

                fixed (char* patternPtr = pattern)
                {
                    for (byte* p = pBase; p < end; p += 4)
                    {
                        bool match = true;
                        for (int i = 0; i < pattern.Length; i++)
                        {
                            if (p[i] != patternPtr[i])
                            {
                                match = false;
                                break;
                            }
                        }

                        if (match)
                        {
                            byte* versionPtr = p + pattern.Length;
                            int result = 0;
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
        public static TypeVersion GetVersion() => _Version.Value;

        private struct Select(int V24E = 0, int V27A = 0)
        {
            public int V24E = V24E;
            public int V27A = V27A;
        }

        private static nint SelectVersion(Select data)
        {
            nint moduleHandle = GetModuleHandleW("game.dll");
            int offset = GetVersion() switch
            {
                TypeVersion.V24E => data.V24E,
                TypeVersion.V27A => data.V27A,
                _ => 0,
            };
            return moduleHandle + offset;
        }

        private static readonly Lazy<nint> JassEnvAddress = new(() => SelectVersion(new(V27A: 0xBE3740)));
        // private static readonly nint JassEnvAddress = SelectVersion(new(V27A: 0xBE3740));
        private static nint GetJassEnvAddress() => JassEnvAddress.Value;

        private static readonly Lazy<nint> CGameUIAddress = new(() => SelectVersion(new(V27A: 0xBE6350)));
        private static nint GetCGameUIAddress() => CGameUIAddress.Value;
    }
}

using System.Runtime.InteropServices;

namespace War3Frame
{
    public partial class War3
    {
        internal class JassVM(nint realJassVM)
        {
            readonly nint RealJassVM = realJassVM;

            public static bool operator true(JassVM jassVM) => jassVM != null && jassVM.RealJassVM != 0;
            public static bool operator false(JassVM jassVM) => jassVM == null || jassVM.RealJassVM == 0;
            public static bool operator !(JassVM jassVM) => jassVM == null || jassVM.RealJassVM == 0;

            private static readonly Lazy<nint> CreateJassStringPtr = new(() => SelectVersion(new(V27A: 0x7F1540)));
            public uint CreateJassString(string str)
            {
                unsafe
                {
                    byte[] utf8Bytes = GetUtf8BytesWithNull(str);
                    fixed (byte* strPtr = utf8Bytes)
                    {
                        var func = (delegate* unmanaged[Thiscall]<nint, byte*, uint>)CreateJassStringPtr.Value;
                        return func(RealJassVM, strPtr);
                    }
                }
            }

            private static readonly Lazy<nint> DestroyJassStringPtr = new(() => SelectVersion(new(V27A: 0x7EC9B0)));
            public void DestroyJassString(uint i)
            {
                unsafe
                {
                    var func = (delegate* unmanaged[Thiscall]<nint, uint, void>)DestroyJassStringPtr.Value;
                    func(RealJassVM, i);
                }
            }

            public string GetJassString(uint i)
            {
                unsafe
                {
                    nint addr = *(nint*)(RealJassVM + 0x2874);
                    if (i >= *(uint*)(addr + 0x04))
                        return "";

                    addr = *(nint*)(addr + 0x08);
                    addr += (nint)i << 4;

                    addr = *(nint*)(addr + 0x08);
                    if (addr == 0)
                        return "";
                    addr = *(nint*)(addr + 0x1C);

                    return Marshal.PtrToStringUTF8(addr) ?? "";
                }
            }

            private static readonly Lazy<nint> CreateOpcodeIdPtr = new(() => SelectVersion(new(V27A: 0x7ECE50)));
            public uint CreateOpcodeId(nint opCode)
            {
                unsafe
                {
                    var func = (delegate* unmanaged[Thiscall]<nint, nint, uint>)CreateOpcodeIdPtr.Value;
                    return func(RealJassVM, opCode);
                }
            }

            public uint GetJassHashedId(string str)
            {
                unsafe
                {
                    uint hash = GetHashKey(str);
                    nint addr = *(nint*)(RealJassVM + 0x2858);
                    addr = *(nint*)(addr + 0x08);

                    uint hashMask = *(uint*)(addr + 0x34);
                    uint offset = (hashMask & hash) * 12 + 8;

                    addr = *(nint*)(addr + 0x2C);
                    addr = *(nint*)(addr + offset);

                    while (addr.ToInt32() > 0)
                    {
                        if (hash == *(uint*)(addr))
                        {
                            byte* namePtr = *(byte**)(addr + 0x14);
                            if (namePtr != null)
                            {
                                byte[] utf8Bytes = GetUtf8BytesWithNull(str);
                                fixed (byte* strPtr = utf8Bytes)
                                {
                                    if (CStringCompare(strPtr, namePtr))
                                    {
                                        return *(uint*)(addr + 0x18);
                                    }
                                }
                            }
                        }
                        addr = *(nint*)(addr + 0x08);
                    }
                    return 0;
                }
            }

            public string GetJassHashedStr(uint id)
            {
                unsafe
                {
                    nint addr = *(nint*)(RealJassVM + 0x2858);
                    addr = *(nint*)(addr + 0x08);

                    if (id >= *(uint*)(addr))
                        return "";

                    addr = *(nint*)(addr + 0x08);
                    addr = *(nint*)(addr + 0x04 * (int)id);
                    addr = *(nint*)(addr + 0x14);

                    return Marshal.PtrToStringUTF8(addr) ?? "";
                }
            }

            public nint GetJassRegVariable(byte i)
            {
                return RealJassVM + 0x50 + i * 0x28;
            }

            public (int type, uint value) GetJassRegVariableValue(byte i)
            {
                unsafe
                {
                    nint t = GetJassRegVariable(i);
                    return (*(int*)(t + 0x1C), *(uint*)(t + 0x20));
                }
            }

            private void SetHandleReference(nint handle, bool sub)
            {
                unsafe
                {
                    nint handleContext = *(nint*)(RealJassVM + 0x28A4);
                    if (handleContext == 0)
                        return;

                    var func = (delegate* unmanaged[Fastcall]<nint, int, nint, void>)*(nint*)(RealJassVM + 0x28A0);
                    func(handle, sub ? 1 : 0, handleContext);
                }
            }

            public void AddHandleReference(nint handle)
            {
                SetHandleReference(handle, false);
            }

            public void SubHandleReference(nint handle)
            {
                SetHandleReference(handle, true);
            }

            public uint GetHandleReference(nint handle)
            {
                unsafe
                {
                    int h = (int)handle - 0x100000;
                    if (h <= 0)
                        return 0;

                    nint handleContext = *(nint*)(RealJassVM + 0x28A4);
                    if (handleContext == 0)
                        return 0;

                    nint arrayPtr = *(nint*)handleContext;
                    if (arrayPtr == 0)
                        return 0;

                    int size = *(int*)(arrayPtr + 0x198);
                    if (h > size)
                        return 0;

                    nint objectsPtr = *(nint*)(arrayPtr + 0x19C);
                    if (objectsPtr == 0)
                        return 0;

                    nint objectPtr = objectsPtr + h * 12;
                    return *(uint*)objectPtr;
                }
            }
        }

        private static JassVM GetJassVM(int i = 1)
        {
            unsafe
            {
                nint addr = *(nint*)GetJassEnvAddress();
                addr = *(nint*)(addr + 0x14);
                addr = *(nint*)(addr + 0x90);
                return new JassVM(*(nint*)(addr + 0x04 * i));
            }
        }

        private static readonly Lazy<JassVM> MainJassVM = new(() => GetJassVM(1));
        private static JassVM GetMainJassVM()
        {
            return MainJassVM.Value;
        }

        private static JassVM GetCurrentJassVM()
        {
            unsafe
            {
                nint addr = *(nint*)GetJassEnvAddress();
                addr = *(nint*)(addr + 0x14);
                int i = *(int*)(addr + 0x14);
                if (i > 0)
                {
                    addr = *(nint*)(addr + 0x0C);
                    nint realJassVM = *(nint*)(addr + 0x04 * (i - 1));
                    return new JassVM(realJassVM);
                }
                return new JassVM(0);
            }
        }
    }
}

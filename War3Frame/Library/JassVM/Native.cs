using System;
using System.Runtime.InteropServices;

namespace War3Frame
{
    public partial class War3
    {
        private const int JassIntegerType = 4;

        private static void ReportNativeSafetyIssue(string message)
        {
            Console.WriteLine($"[War3Frame.Native] {message}");
        }

        private static nint GetNativeTable()
        {
            unsafe
            {
                nint addr = *(nint*)GetJassEnvAddress();
                addr = *(nint*)(addr + 0x14);
                return *(nint*)(addr + 0x24);
            }
        }

        private static nint GetNativeTable(string name)
        {
            unsafe
            {
                byte[] utf8 = GetUtf8BytesWithNull(name);

                uint hash = GetHashKey(name);
                nint addr = *(nint*)GetJassEnvAddress();
                addr = *(nint*)(addr + 0x14);

                uint hashMask = *(uint*)(addr + 0x3C);
                uint offset = (hashMask & hash) * 12 + 8;

                addr = *(nint*)(addr + 0x34);
                addr = *(nint*)(addr + offset);

                while (addr.ToInt32() > 0)
                {
                    if (hash == *(uint*)(addr + 0x04))
                    {
                        byte* namePtr = *(byte**)(addr + 0x18);
                        if (namePtr != null)
                        {
                            fixed (byte* utf8Ptr = utf8)
                            {
                                if (CStringCompare(utf8Ptr, namePtr))
                                {
                                    return addr;
                                }
                            }
                        }
                    }
                    addr = *(nint*)(addr + 0x0C);
                }
                return 0;
            }
        }

        public static nint GetNativeFunction(string name)
        {
            unsafe
            {
                nint addr = GetNativeTable(name);
                if (addr != 0)
                {
                    return *(nint*)(addr + 0x1C);
                }
                return 0;
            }
        }

        private static void SetNativeFunction(string name, nint func)
        {
            unsafe
            {
                nint addr = GetNativeTable(name);
                if (addr != 0)
                {
                    *(nint*)(addr + 0x1C) = func;
                }
            }
        }

        private delegate void NativeFunctionCallback(string name, nint func);
        private static unsafe uint ForEachNativeFunction(NativeFunctionCallback callback)
        {
            uint count = 0;
            nint firstTable = GetNativeTable();
            if (firstTable == 0)
                return 0;
            nint nextTable = firstTable;
            while (nextTable.ToInt32() > 0)
            {
                nint namePtr = *(nint*)(nextTable + 0x18);
                nint func = *(nint*)(nextTable + 0x1C);
                string name = Marshal.PtrToStringUTF8(namePtr) ?? "";
                callback(name, func);
                count++;
                nextTable = *(nint*)(nextTable + 0x14);
            }
            return count;
        }

        private static nint CreateNativeString(string str)
        {
            unsafe
            {
                nint addr = Marshal.AllocHGlobal(0x40);
                if (addr == 0)
                    return 0;

                nint cstr = Marshal.StringToCoTaskMemUTF8(str);
                *(nint*)(addr + 0x08) = addr;
                *(nint*)(addr + 0x1C) = cstr;

                return addr;
            }
        }

        private static void DestoryNativeString(nint address)
        {
            unsafe
            {
                if (address == 0) return;
                nint cstr = *(nint*)(address + 0x1C);
                Marshal.FreeCoTaskMem(cstr);
                Marshal.FreeHGlobal(address);
            }
        }

        private static Lazy<uint> NativeCodeId = new(() =>
        {
            var vm = GetMainJassVM();
            if (!vm)
            {
                ReportNativeSafetyIssue("主 JassVM 不可用，无法初始化 NativeCodeId。");
                return 0;
            }

            SetNativeFunction("IsNoDefeatCheat", Marshal.GetFunctionPointerForDelegate<Action>(NativeCodeCallback));
            return vm.GetJassHashedId("IsNoDefeatCheat");
        });
        private static List<Action> NativeCodeDelegates = [];
        private static int _namedCodeIndex = 0;

        /// <summary>
        /// 用于标识此 Action 需要注册为命名 Jass 函数（供 Dz ByCode 系列 API 使用）。
        /// 在 DzApi 的 ByCode 方法内部自动包装，用户侧无需感知。
        /// </summary>
        public record DzAction(Action Action);

        private static void NativeCodeCallback()
        {
            if (!TryGetCurrentJassVM(out var vm))
            {
                ReportNativeSafetyIssue("当前 JassVM 不可用，已跳过 native callback 分发。");
                return;
            }

            if (!vm.TryGetJassRegVariableValue(0, out var type, out var value))
            {
                ReportNativeSafetyIssue("无法读取 callback 寄存器值，已跳过 native callback 分发。");
                return;
            }

            if (type != JassIntegerType)
            {
                ReportNativeSafetyIssue($"callback 寄存器类型无效: {type}");
                return;
            }

            if (value >= (uint)NativeCodeDelegates.Count)
            {
                ReportNativeSafetyIssue($"callback 索引越界: {value}");
                return;
            }

            var callback = NativeCodeDelegates[(int)value];
            callback();
        }

        private static unsafe nint BuildOpcodes(nint idx)
        {
            int id = (int)NativeCodeId.Value;
            if (id == 0) return 0;

            const int TYPE_VARIABLE_INTEGER = 4;
            const int TYPE_OPCODE_MOVRLITERAL = 12;
            const int TYPE_OPCODE_CALLNATIVE = 21;
            const int TYPE_OPCODE_RETURN = 39;

            nint opcodes = Marshal.AllocHGlobal(0x08 * 3);
            byte* op = (byte*)opcodes;
            {
                op[0] = 0;
                op[1] = TYPE_VARIABLE_INTEGER;
                op[2] = 0;
                op[3] = TYPE_OPCODE_MOVRLITERAL;
                *(int*)(op + 4) = (int)idx;
                op += 8;
            }
            {
                op[0] = 0;
                op[1] = 0;
                op[2] = 0;
                op[3] = TYPE_OPCODE_CALLNATIVE;
                *(int*)(op + 4) = id;
                op += 8;
            }
            {
                op[0] = 0;
                op[1] = 0;
                op[2] = 0;
                op[3] = TYPE_OPCODE_RETURN;
                *(int*)(op + 4) = 0;
            }
            return opcodes;
        }

        private static uint CreateNativeCode(nint idx)
        {
            unsafe
            {
                nint opcodes = BuildOpcodes(idx);
                if (opcodes == 0) return 0;
                var vm = GetMainJassVM();
                if (!vm)
                {
                    ReportNativeSafetyIssue("主 JassVM 不可用，无法创建匿名 native code。");
                    return 0;
                }

                return vm.CreateOpcodeId(opcodes);
            }
        }

        private static uint CreateNativeCode(Action func)
        {
            NativeCodeDelegates.Add(func);
            return CreateNativeCode(NativeCodeDelegates.Count - 1);
        }

        /// <summary>
        /// 为 Dz ByCode 系列函数创建具名 Jass 函数，通过 CreateJassFunction 注册，返回 HashId。
        /// 普通 JASS code 类型用 CreateNativeCode，Dz ByCode 用此方法。
        /// </summary>
        private static uint CreateNamedNativeCode(Action func)
        {
            NativeCodeDelegates.Add(func);
            var idx = NativeCodeDelegates.Count - 1;
            unsafe
            {
                nint opcodes = BuildOpcodes(idx);
                if (opcodes == 0) return 0;
                var name = $"__cs_{_namedCodeIndex++}";
                var vm = GetMainJassVM();
                if (!vm)
                {
                    ReportNativeSafetyIssue("主 JassVM 不可用，无法创建具名 native code。");
                    return 0;
                }

                return vm.CreateJassFunction(name, opcodes);
            }
        }

        private static void EnsureNativeFunctionAvailable(nint func)
        {
            if (func == 0)
            {
                throw new InvalidOperationException("原生函数地址无效，已停止调用。");
            }
        }

        public static void CallNative(nint func, params object[] args)
        {
            EnsureNativeFunctionAvailable(func);
            unsafe
            {
                List<nint> jstr = [];
                int[] values = new int[args.Length];
                float[] floats = new float[args.Length];
                fixed (float* floatPtr = floats)
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        switch (args[i])
                        {
                            case int v:
                                values[i] = v;
                                break;
                            case nint v:
                                values[i] = Convert.ToInt32(v);
                                break;
                            case float v:
                                floats[i] = v;
                                values[i] = (int)(floatPtr + i);
                                break;
                            case bool v:
                                values[i] = v ? 1 : 0;
                                break;
                            case string v:
                                nint s = CreateNativeString(v);
                                jstr.Add(s);
                                values[i] = (int)s;
                                break;
                            case Action v:
                                values[i] = (int)CreateNativeCode(v);
                                break;
                            case DzAction v:
                                values[i] = (int)CreateNamedNativeCode(v.Action);
                                break;
                            default:
                                throw new InvalidCastException($"不支持将类型 {args[i].GetType().Name} 转换为 Int32");
                        }
                    }
                    CallCdeclFunction(func, values);
                    foreach (var s in jstr)
                    {
                        DestoryNativeString(s);
                    }
                }
            }
        }
        public static T CallNative<T>(nint func, params object[] args)
        {
            EnsureNativeFunctionAvailable(func);
            unsafe
            {
                List<nint> jstr = [];
                int[] values = new int[args.Length];
                float[] floats = new float[args.Length];
                int result;
                fixed (float* floatPtr = floats)
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        switch (args[i])
                        {
                            case int v:
                                values[i] = v;
                                break;
                            case nint v :
                                values[i] = Convert.ToInt32(v);
                                break;
                            case float v:
                                floats[i] = v;
                                values[i] = (int)(floatPtr + i);
                                break;
                            case bool v:
                                values[i] = v ? 1 : 0;
                                break;
                            case string v:
                                nint s = CreateNativeString(v);
                                jstr.Add(s);
                                values[i] = (int)s;
                                break;
                            case Action v:
                                values[i] = (int)CreateNativeCode(v);
                                break;
                            case DzAction v:
                                values[i] = (int)CreateNamedNativeCode(v.Action);
                                break;
                            default:
                                throw new InvalidCastException($"不支持将类型 {args[i].GetType().Name} 转换为 Int32");
                        }
                    }

                    result = CallCdeclFunction(func, values);

                    foreach (var s in jstr)
                    {
                        DestoryNativeString(s);
                    }
                }

                if (typeof(T) == typeof(void))
                {
                    return (T)(object)result;
                }
                else if (typeof(T) == typeof(int))
                {
                    return (T)(object)result;
                }
                else if (typeof(T) == typeof(float))
                {
                    float floatValue = BitConverter.ToSingle(BitConverter.GetBytes(result), 0);
                    return (T)(object)floatValue;
                }
                else if (typeof(T) == typeof(bool))
                {
                    return (T)(object)(result != 0);
                }
                else if (typeof(T) == typeof(string))
                {
                    if (result == 0)
                        return (T)(object)"";

                    var vm = GetMainJassVM();
                    string strValue = vm.GetJassString((uint)result);
                    vm.DestroyJassString((uint)result);
                    return (T)(object)strValue;
                }
                else
                {
                    throw new InvalidCastException($"不支持将返回类型 {typeof(T).Name} 转换为 T");
                }
            }
        }

        private static unsafe int CallCdeclFunction(nint func, int[] args)
        {
            EnsureNativeFunctionAvailable(func);
            switch (args.Length)
            {
                case 0:
                    {
                        var fn = (delegate* unmanaged[Cdecl]<int>)(func);
                        return fn();
                    }
                case 1:
                    {
                        var fn = (delegate* unmanaged[Cdecl]<int, int>)(func);
                        return fn(args[0]);
                    }
                case 2:
                    {
                        var fn = (delegate* unmanaged[Cdecl]<int, int, int>)(func);
                        return fn(args[0], args[1]);
                    }
                case 3:
                    {
                        var fn = (delegate* unmanaged[Cdecl]<int, int, int, int>)(func);
                        return fn(args[0], args[1], args[2]);
                    }
                case 4:
                    {
                        var fn = (delegate* unmanaged[Cdecl]<int, int, int, int, int>)(func);
                        return fn(args[0], args[1], args[2], args[3]);
                    }
                case 5:
                    {
                        var fn = (delegate* unmanaged[Cdecl]<int, int, int, int, int, int>)(func);
                        return fn(args[0], args[1], args[2], args[3], args[4]);
                    }
                case 6:
                    {
                        var fn = (delegate* unmanaged[Cdecl]<int, int, int, int, int, int, int>)(func);
                        return fn(args[0], args[1], args[2], args[3], args[4], args[5]);
                    }
                case 7:
                    {
                        var fn = (delegate* unmanaged[Cdecl]<int, int, int, int, int, int, int, int>)(func);
                        return fn(args[0], args[1], args[2], args[3], args[4], args[5], args[6]);
                    }
                case 8:
                    {
                        var fn = (delegate* unmanaged[Cdecl]<int, int, int, int, int, int, int, int, int>)(func);
                        return fn(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]);
                    }
                case 9:
                    {
                        var fn = (delegate* unmanaged[Cdecl]<int, int, int, int, int, int, int, int, int, int>)(func);
                        return fn(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8]);
                    }
                case 10:
                    {
                        var fn = (delegate* unmanaged[Cdecl]<int, int, int, int, int, int, int, int, int, int, int>)(func);
                        return fn(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9]);
                    }
                case 11:
                    {
                        var fn = (delegate* unmanaged[Cdecl]<int, int, int, int, int, int, int, int, int, int, int, int>)(func);
                        return fn(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10]);
                    }
                case 12:
                    {
                        var fn = (delegate* unmanaged[Cdecl]<int, int, int, int, int, int, int, int, int, int, int, int, int>)(func);
                        return fn(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11]);
                    }
                default:
                    throw new NotSupportedException($"暂时不支持 {args.Length} 个参数的调用");
            }
        }
    }
}

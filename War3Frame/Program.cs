using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Text;

namespace War3Frame
{
    public class Program
    {
        public static int Main(bool IsAot)
        {





            return 0;
        }

        // C++/CLR 入口
        // 调试插件: Microsoft Child Process Debugging Power Tool 2022+
        // 调试=>其他调试目标=>添加war3.exe[托管 (.NET Core, .NET 5+)]
        public static int MainCLR()
        {
            War3.EnableConsole();

            string dllPath = Path.Combine(AppContext.BaseDirectory, "demo.dll");
            var asm = AssemblyLoadContext.Default.LoadFromAssemblyPath(dllPath);
            Type type = asm.GetType("War3Frame.Game");
            MethodInfo staticM = type.GetMethod("StaticFunc");
            object r = staticM.Invoke(null, new object[] { 123 });


            return Main(false);
        }

        // Native AOT 入口
        [UnmanagedCallersOnly(EntryPoint = "main")]
        public static int MainAOT()
        {
            return Main(true);
        }
    }
}

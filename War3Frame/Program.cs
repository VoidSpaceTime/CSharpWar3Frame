using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace War3Frame
{
    public class Program
    {
        public static int Main(bool IsAot)
        {
            War3.EnableConsole();







            return 0;
        }

        // C++/CLR 入口
        // 调试插件: Microsoft Child Process Debugging Power Tool 2022+
        // 调试=>其他调试目标=>添加war3.exe[托管 (.NET Core, .NET 5+)]
        public static int MainCLR()
        {
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

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using War3Frame.Library.Api;

namespace War3Frame
{
    public static class Game
    {
        // Native AOT 入口
        [UnmanagedCallersOnly(EntryPoint = "main")]
        public static int MainAOT()
        {


            War3.EnableConsole();
            Console.WriteLine("Hello World!");

/*            var func = War3.GetNativeFunction("CreateTimer");
            var t = War3.CallNative<int>(func);
            Console.WriteLine($"timer = {t}");
            var i = 0;
            func = War3.GetNativeFunction("TimerStart");
            //var p = JassApi.Player(0);
            //JassApi.CreateUnitByName(p, "hmpr", 0f, 0f, 0f);
  
            War3.CallNative<int>(func, t, 1.0f, true, () =>
            {
                Console.WriteLine($"{t} {i++}");
            });*/

            return 0;
        }
    }
}

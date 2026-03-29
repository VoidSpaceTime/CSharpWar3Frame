using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using System.Numerics;
using System.Runtime.InteropServices;

namespace War3Frame
{
    public static partial class Game
    {
        public static float TimeSpan = 0f;
        public const float TICK_RATE = 0.01f; // War3 中心计时器频率

        // Native AOT 入口
        //[UnmanagedCallersOnly(EntryPoint = "main")]
        public static int MainAOT()
        {
            /* AOTInit 注册 Component
             *
             * ECSInit 创建root entityStore
             * War3Init  初始化原生信息,
             * 其他框架初始化
             * 地图内容初始化
             */

            Main(true);

            return 0;
        }

        // 使用 Cdecl 调用约定，导出名为 "main"（不带装饰符）
        [UnmanagedCallersOnly(EntryPoint = "main",
            CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        public static int MainJIT()
        {
            Main(false);
            return 0;
        }

        public static void Main(bool isAot)
        {
            War3.EnableConsole("war3Debug");
            Console.WriteLine("Hello World! isAot: " + isAot.ToString());
            if (isAot)
            {
                var aot = new NativeAOT();
                aot.CreateSchema();
            }


            // 中心计时器
            var func = War3.GetNativeFunction("CreateTimer");
            var t = War3.CallNative<int>(func);
            Console.WriteLine($"timer = {t}");
            func = War3.GetNativeFunction("TimerStart");

            var p0 = JassApi.Player(0);
            Console.WriteLine($"玩家 = {p0.Handle}");
            Console.WriteLine($"hfoo = {JassApi.C2I("hfoo")}");

            var unit = JassApi.CreateUnit(p0, JassApi.C2I("hfoo"), 0, 0, 270);
            // var unit2 = JassApi.CreateUnit(p0, JassApi.C2I("hfoo"), 0, 0, 270);
            Console.WriteLine($"创建单位 = {unit.Handle}");
            

            var tgr = JassApi.CreateTrigger();
            HandleHelper.HandleAdd(tgr);
            JassApi.TriggerRegisterPlayerUnitEvent(tgr, JassApi.Player(0),
                JassApi.ConvertPlayerUnitEvent(Blizzard.EVENT_PLAYER_UNIT_SELECTED), null);
            JassApi.TriggerAddAction(tgr, () =>
            {
                var triggerUnit = JassApi.GetTriggerUnit();
                var triggerPlayer = JassApi.GetTriggerPlayer();
                Console.WriteLine("选择单位");
            });

            
            War3.CallNative<int>(func, t, TICK_RATE, true, () =>
            {
                TimeSpan += TICK_RATE;
            });
        }

     
    }
}
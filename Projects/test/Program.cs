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
        [UnmanagedCallersOnly(EntryPoint = "main", CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
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
                aot.RegisterComponent<Position>();
                aot.RegisterComponent<Velocity>();
                aot.CreateSchema();
            }
            var world = new EntityStore();

            // 使用 TimedSystemRoot
            var root = new TimedSystemRoot(world);
            root.SetMonitorPerf(true);

            // 添加 System 并指定间隔：MoveSystem 每 1 秒更新一次
            root.Add(new MoveSystem(), 1.0f);

            // 中心计时器
            var func = War3.GetNativeFunction("CreateTimer");
            var t = War3.CallNative<int>(func);
            Console.WriteLine($"timer = {t}");
            func = War3.GetNativeFunction("TimerStart");

            var p0 = JassApi.Player(0);
            Console.WriteLine($"玩家 = {p0.Handle}");
            Console.WriteLine($"hfoo = {JassApi.C2I("hfoo")}");

            var unit = JassApi.CreateUnit(p0, JassApi.C2I("hfoo"), 0, 0, 270);
            Console.WriteLine($"创建单位 = {unit.Handle}");

            var entity = world.CreateEntity(new Position(0, 0, 0), new Velocity { value = new Vector3(0, 0, 0), unit = unit });

            War3.CallNative<int>(func, t, TICK_RATE, true, () =>
            {
                root.Update(new(TICK_RATE, TimeSpan));
                TimeSpan += TICK_RATE;
            });
        }
        public struct Velocity : IComponent { public Vector3 value; public JUnit unit; }

        // 使用原生 QuerySystem，频率由 TimedSystemRoot 控制
        class MoveSystem : QuerySystem<Position>
        {
            protected override void OnUpdate()
            {
                Query.ForEachEntity((ref Position position, Entity entity) =>
                {
                    var unit = entity.GetComponent<Velocity>().unit;
                    var ux = JassApi.GetUnitX(unit);
                    var uy = JassApi.GetUnitY(unit);
                    position.value = new Vector3(ux, uy, 0);
                    Console.WriteLine($"更新单位位置: x: {ux}, y: {uy}");
                });
            }
        }
    }
}



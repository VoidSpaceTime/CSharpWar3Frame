using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace War3Frame;

public static class Bootstrap
{
    public const float TICK_RATE = 0.01f; // War3 中心计时器频率
    public static float TimeSpan;


    // Native AOT 入口
    [UnmanagedCallersOnly(EntryPoint = "main")]
    public static int AotMain()
    {
        Main(true);
        ;
        return 0;
    }

    // C++/CLI bridge 调用的托管入口。
    public static int BridgeMain()
    {
        Main(false);
        return 0;
    }

    public static void Main(bool isAot)
    {
        War3.EnableConsole();
        Console.WriteLine("Hello World! isAot: " + isAot);

        var world = new EntityStore();

        // 使用 TimedSystemRoot
        var root = new TimedSystemRoot(world);

        root.SetMonitorPerf(true);

        // 添加 System 并指定间隔：MoveSystem 每 1 秒更新一次
        root.Add(new MoveSystem(), 1.0f);


        var func = War3.GetNativeFunction("CreateTimer");
        var p0 = JassApi.Player(0);
        Console.WriteLine($"玩家 = {p0.Handle}");
        Console.WriteLine($"hfoo = {JassApi.C2I("hfoo")}");

        var p1 = JassApi.Player(1);
        var unit = JassApi.CreateUnit(p0, JassApi.C2I("hfoo"), 0, 0, 270);
        var unit1 = JassApi.CreateUnit(p1, JassApi.C2I("hfoo"), 0, 0, 270);

        Console.WriteLine($"创建单位 = {unit.Handle}");
        RegisterPlayerChart();
        RedisterPlayerUnitAttack();

        void RegisterPlayerChart()
        {
            Console.WriteLine(" 注册玩家对话");
            var trigger = JassApi.CreateTrigger();
            var condition = JassApi.Condition(() =>
            {
                var message = JassApi.GetEventPlayerChatString();
                JassApi.DisplayTimedTextToPlayer(p0, 0, 0, 180, "玩家消息:" + message);
                Console.WriteLine("玩家消息:" + message);
            });
            JassApi.TriggerAddCondition(trigger, condition);
            JassApi.TriggerRegisterPlayerChatEvent(trigger, p0, "", false);
        }

        void RedisterPlayerUnitAttack()
        {
            Console.WriteLine(" 注册单位攻击");
            var trigger = JassApi.CreateTrigger();
            var condition = JassApi.Condition(() =>
            {
                var attacker = JassApi.GetAttacker();
                var target = JassApi.GetTriggerUnit();
                JassApi.DisplayTimedTextToPlayer(p0, 0, 0, 180, "触发攻击事件:");
                Console.WriteLine("触发攻击事件:");
            });
            JassApi.TriggerAddCondition(trigger, condition);
            JassApi.TriggerRegisterPlayerUnitEvent(trigger, p0, Blizzard.EVENT_PLAYER_UNIT_ATTACKED, null);
        }
    }

    public struct Velocity : IComponent
    {
        public Vector3 value;
        public JUnit unit;
    }

    // 使用原生 QuerySystem，频率由 TimedSystemRoot 控制
    private class MoveSystem : QuerySystem<Position>
    {
        protected override void OnUpdate()
        {
            Query.ForEachEntity((ref position, entity) =>
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
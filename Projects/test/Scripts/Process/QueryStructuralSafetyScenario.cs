using System;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame;
using War3Frame.Components;
using War3Frame.Src.Systems;
using War3Frame.Src.Systems.Time;
using War3Frame.Systems.Time;
using War3Frame.Systems.Unit;

namespace War3Frame.Scripts.Process;

/// <summary>
/// 本地 ECS 验证 Query 结构安全与玩家初始化修复（fix-query-loop-structural-mutation + fix-player-init-self-assign）。
/// 每个用例都构造匹配实体以**真正执行循环体**——旧实现会在此抛 StructuralChangeException。
/// 说明：这些场景不依赖 War3 原生句柄，可在独立宿主进程中运行。
/// </summary>
public static class QueryStructuralSafetyScenario
{
    private const string ScenarioName = "QueryStructuralSafetyScenario";

    /// <summary>运行不依赖 War3 Native 句柄的同步 ECS 验证。</summary>
    public static void Initialize(JPlayer player)
    {
        _ = player;
        Run();
    }

    /// <summary>同步验证已在 Initialize 完成，运行时无需操作。</summary>
    public static void Update()
    {
    }

    private static void Run()
    {
        CheckDurationExpiry();
        CheckLifecycleTransition();
        CheckTimerTaskExpiry();
        CheckBuffDuration();
        CheckPlayerInit();

        Console.WriteLine($"{ScenarioName}: PASS");
    }

    /// <summary>DurationSystem：递减并打 DurationExpired（旧实现在循环内 AddTag/AddComponent 会抛）。</summary>
    private static void CheckDurationExpiry()
    {
        var store = new EntityStore();
        var root = new TimedSystemRoot(store);
        root.Add(new DurationSystem(), 0f);

        store.CreateEntity(new Duration { remaining = 0.01f, total = 0.01f });
        root.Update(new UpdateTick(1.0f, 0f));

        var entity = FirstEntity<Duration>(store);
        Require(entity.Tags.Has<DurationExpired>(), "duration/expired");
        Require(entity.GetComponent<Duration>().remaining == 0f, "duration/remainingZero");
    }

    /// <summary>UnitLifecycleTransitionSystem：Death 推进到 Corpse（旧实现循环内 AddComponent 会抛）。</summary>
    private static void CheckLifecycleTransition()
    {
        var store = new EntityStore();
        var root = new TimedSystemRoot(store);
        root.Add(new UnitLifecycleTransitionSystem(), 0f);

        var unit = store.CreateEntity(new UnitLifeState { isAlive = false, lifePhase = UnitLifecyclePhase.Death });
        root.Update(new UpdateTick(0.01f, 0f));

        Require(unit.GetComponent<UnitLifeState>().lifePhase == UnitLifecyclePhase.Corpse, "lifecycle/deathToCorpse");
    }

    /// <summary>TimerTaskSystem：Once 到期打 TimerExpired 并移除 TimerTask（旧实现循环内结构变更会抛）。</summary>
    private static void CheckTimerTaskExpiry()
    {
        var store = new EntityStore();
        var root = new TimedSystemRoot(store);
        root.Add(new TimerTaskSystem(), 0f);

        var owner = store.CreateEntity(new UnitLifeState { isAlive = true, lifePhase = UnitLifecyclePhase.Alive });
        var timer = store.CreateEntity(new TimerTask
        {
            mode = TimerTaskMode.Once,
            interval = 0.01f,
            remaining = 0.01f,
            paused = false,
            owner = owner,
            kind = TimerTaskKind.BuffExpire,
            triggerCount = 0,
            maxTriggerCount = 1
        });

        root.Update(new UpdateTick(1.0f, 0f));

        Require(timer.Tags.Has<TimerExpired>(), "timer/expired");
        Require(timer.Tags.Has<BuffExpired>(), "timer/buffExpired");
        Require(!timer.HasComponent<TimerTask>(), "timer/removed");
    }

    /// <summary>BuffDurationSystem：DurationExpired 存在时打 BuffExpired（旧实现循环内 AddTag 会抛）。</summary>
    private static void CheckBuffDuration()
    {
        var store = new EntityStore();
        var root = new TimedSystemRoot(store);
        root.Add(new BuffDurationSystem(), 0f);

        var buff = store.CreateEntity(
            new Buff { buffId = "test", kind = BuffKind.Attribute },
            new BuffBehavior(),
            new Duration { remaining = 0f, total = 1f });
        buff.AddTag<DurationExpired>();

        root.Update(new UpdateTick(0.01f, 0f));

        Require(buff.Tags.Has<BuffExpired>(), "buff/expired");
    }

    /// <summary>PlayerHelper.InitializePlayers：玩家镜像非空、联盟矩阵默认敌对（旧实现自赋值导致恒空）。</summary>
    private static void CheckPlayerInit()
    {
        var store = new EntityStore();
        var players = new PlayerNative[16];
        for (var i = 0; i < players.Length; i++)
        {
            var entity = store.CreateEntity();
            players[i] = new PlayerNative
            {
                index = i,
                name = $"p{i}",
                color = i,
                getentity = entity
            };
        }

        PlayerHelper.InitializePlayers(ref players);

        Require(PlayerHelper.Players.Length == 16, "player/count");
        Require(PlayerHelper.GetRelation(players[0], players[1]) == PlayerTeamState.Enemy, "player/defaultEnemy");
        Require(PlayerHelper.GetRelation(players[0], players[0]) == PlayerTeamState.Allie, "player/selfAllie");
    }

    private static Entity FirstEntity<T>(EntityStore store) where T : struct, IComponent
    {
        var result = default(Entity);
        store.Query<T>().ForEachEntity((ref T _, Entity entity) => result = entity);
        return result;
    }

    private static void Require(bool condition, string invariantContext)
    {
        if (!condition)
            throw new InvalidOperationException($"{ScenarioName}: {invariantContext}");
    }
}

using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame;
using War3Frame.Src.Systems.Time;
using War3Frame.Systems.Native;
using War3Frame.Systems.Unit;

namespace War3Frame.Regression;

/// <summary>验证单位复活倒计时、单位池复用与生命周期原生句柄配对。</summary>
internal static class UnitRebornPoolRegression
{
    /// <summary>注册复活与单位池回归场景。</summary>
    internal static void Register(List<RegressionCase> tests)
    {
        tests.Add(new("runtime", "复活倒计时", RebornCountdown));
        tests.Add(new("runtime", "池重用", PoolReuse));
        tests.Add(new("runtime", "句柄配对", HandlePairing));
    }

    /// <summary>正数 rebornTime 经 RebornPending 回到 Alive；零值仍被终态删除；复活不补发死亡。</summary>
    private static void RebornCountdown()
    {
        var store = new EntityStore();
        var unit = store.CreateEntity(new UnitLifeState
        {
            isAlive = true,
            lifePhase = UnitLifecyclePhase.Alive,
            rebornTime = 1f
        });
        Check.That(UnitHelper.KillUnit(unit), "a live unit must enter death");
        Check.That(store.Query<UnitDiedEvent>().Count == 1, "death broadcasts exactly one fact");

        var root = LifecycleRoot(store, includeTimer: true);
        root.Update(new UpdateTick(0.1f, 0.1f));
        Check.That(unit.GetComponent<UnitLifeState>().lifePhase == UnitLifecyclePhase.Corpse,
            "death advances to corpse while the corpse timer is pending");

        root.Update(new UpdateTick(3.1f, 3.2f));
        Check.That(unit.GetComponent<UnitLifeState>().lifePhase == UnitLifecyclePhase.RebornPending,
            "positive reborn time waits instead of removing");
        Check.That(!unit.IsNull, "the reborn wait keeps the entity");

        root.Update(new UpdateTick(1.1f, 4.3f));
        var state = unit.GetComponent<UnitLifeState>();
        Check.That(state.lifePhase == UnitLifecyclePhase.Alive && state.isAlive,
            "the reborn wait returns the same entity to alive");
        Check.That(state.rebornTime == 0f, "the reborn time is consumed");
        Check.That(!unit.IsNull, "reborn does not delete the entity");
        Check.That(!unit.HasComponent<UnitRebornNativeRequest>(),
            "a handle-less host still consumes and drops the reborn request");
        Check.That(store.Query<UnitDiedEvent>().Count == 1, "reborn must not broadcast a second death fact");

        var controlStore = new EntityStore();
        var doomed = controlStore.CreateEntity(new UnitLifeState
        {
            isAlive = true,
            lifePhase = UnitLifecyclePhase.Alive
        });
        Check.That(UnitHelper.KillUnit(doomed), "the control unit must enter death");
        var control = LifecycleRoot(controlStore, includeTimer: true);
        control.Update(new UpdateTick(0.1f, 0.1f));
        control.Update(new UpdateTick(3.1f, 3.2f));
        Check.That(!doomed.IsNull, "a zero-reborn corpse passes through remove before disposal");
        control.Update(new UpdateTick(0.1f, 3.3f));
        Check.That(doomed.IsNull, "zero reborn time disposes the entity");
    }

    /// <summary>入池保留实体，出池复用同一 Entity.Id，放弃后经唯一销毁路径删除，全程无死亡事件。</summary>
    private static void PoolReuse()
    {
        var store = new EntityStore();
        var unit = store.CreateEntity(new UnitLifeState
        {
            isAlive = true,
            lifePhase = UnitLifecyclePhase.Alive
        });
        var id = unit.Id;

        Check.That(UnitHelper.RequestPool(unit), "an alive unit can enter the pool");
        var state = unit.GetComponent<UnitLifeState>();
        Check.That(state.lifePhase == UnitLifecyclePhase.Pooled && !state.isAlive, "a pooled unit is suspended");
        Check.That(!unit.IsNull && unit.Id == id, "pooling keeps the same entity");
        Check.That(store.Query<UnitDiedEvent>().Count == 0, "pooling does not broadcast a death fact");
        Check.That(unit.HasComponent<UnitRebornNativeRequest>()
            && !unit.GetComponent<UnitRebornNativeRequest>().show, "pooling writes a hide request");

        var root = LifecycleRoot(store, includeTimer: false);
        root.Update(new UpdateTick(0.01f, 0.01f));
        Check.That(!unit.HasComponent<UnitRebornNativeRequest>(),
            "a handle-less host consumes the hide request");

        Check.That(UnitHelper.AcquirePooled(unit), "a pooled unit can be reused");
        state = unit.GetComponent<UnitLifeState>();
        Check.That(state.lifePhase == UnitLifecyclePhase.Alive && state.isAlive, "reuse returns the unit to alive");
        Check.That(unit.Id == id, "reuse keeps the same entity id");
        Check.That(unit.HasComponent<UnitRebornNativeRequest>()
            && unit.GetComponent<UnitRebornNativeRequest>().show, "reuse writes a show request");
        root.Update(new UpdateTick(0.01f, 0.02f));
        Check.That(!unit.HasComponent<UnitRebornNativeRequest>(),
            "the reuse request is consumed without a handle");

        var waiting = store.CreateEntity(new UnitLifeState
        {
            isAlive = false,
            lifePhase = UnitLifecyclePhase.RebornPending,
            rebornTime = 2f
        });
        waiting.AddComponent(new TimerTask
        {
            mode = TimerTaskMode.Once,
            interval = 2f,
            remaining = 2f,
            owner = waiting,
            kind = TimerTaskKind.RebornWait,
            triggerCount = 0,
            maxTriggerCount = 1
        });
        Check.That(UnitHelper.RequestPool(waiting), "a reborn-waiting unit can enter the pool");
        Check.That(waiting.GetComponent<UnitLifeState>().lifePhase == UnitLifecyclePhase.Pooled,
            "the waiting unit becomes pooled");
        Check.That(!waiting.HasComponent<TimerTask>(), "entering the pool cancels the reborn timer");

        Check.That(UnitHelper.RequestPool(unit), "the reused unit can be pooled again");
        Check.That(UnitHelper.DiscardPooled(unit), "a pooled unit can be discarded");
        Check.That(unit.GetComponent<UnitLifeState>().lifePhase == UnitLifecyclePhase.Remove,
            "discard enters the removal phase");
        root.Update(new UpdateTick(0.01f, 0.03f));
        root.Update(new UpdateTick(0.01f, 0.04f));
        Check.That(unit.IsNull, "discard is finalized by the lifecycle dispose path");
        Check.That(store.Query<UnitDiedEvent>().Count == 0, "the pool lifecycle never broadcasts a death fact");
    }

    /// <summary>源码断言：复活/出池路径不登记或注销句柄，销毁只保留 UnitRemoveNativeSystem 单点。</summary>
    private static void HandlePairing()
    {
        var reborn = CompilationTest.Read("War3Frame/Src/Systems/Native/UnitRebornNativeSystem.cs");
        Check.That(reborn.Contains("SetUnitState") && reborn.Contains("ShowUnit"),
            "reborn native work restores life and visibility");
        foreach (var forbidden in new[] { "HandleAdd", "HandleRemove", "CreateUnit", "RemoveUnit", "ReviveHero" })
            Check.That(!reborn.Contains(forbidden, StringComparison.Ordinal),
                "reborn native work must not reference " + forbidden);

        var create = CompilationTest.Read("War3Frame/Src/Systems/Native/UnitCreateNativeSystem.cs");
        Check.That(Count(create, "HandleAdd") == 1 && Count(create, "JassApi.CreateUnit") == 1,
            "creation registers the handle exactly once beside creation");
        Check.That(Count(create, "HandleRemove") == 0 && Count(create, "JassApi.RemoveUnit") == 0,
            "creation neither unregisters nor removes");

        var remove = CompilationTest.Read("War3Frame/Src/Systems/Native/UnitRemoveNativeSystem.cs");
        Check.That(Count(remove, "HandleRemove") == 1 && Count(remove, "JassApi.RemoveUnit") == 1,
            "removal unregisters and removes exactly once");
        Check.That(Count(remove, "HandleAdd") == 0 && Count(remove, "JassApi.CreateUnit") == 0,
            "removal never registers or creates");
        var unregisterIndex = remove.IndexOf("NativeEntityIndex.Unregister", StringComparison.Ordinal);
        var handleRemoveIndex = remove.IndexOf("HandleHelper.HandleRemove", StringComparison.Ordinal);
        var removeUnitIndex = remove.IndexOf("JassApi.RemoveUnit", StringComparison.Ordinal);
        Check.That(unregisterIndex >= 0 && unregisterIndex < handleRemoveIndex && handleRemoveIndex < removeUnitIndex,
            "unregister runs before HandleRemove which runs before removal");

        var helper = CompilationTest.Read("War3Frame/Src/Helpers/UnitHelper.cs");
        Check.That(!helper.Contains("JassApi", StringComparison.Ordinal)
            && !helper.Contains("HandleHelper", StringComparison.Ordinal),
            "lifecycle helpers carry no native calls");
    }

    /// <summary>生命周期系统根（执行顺序即插入顺序）：可选计时器 + Remove/Transition/Reborn/Dispose。</summary>
    private static SystemRoot LifecycleRoot(EntityStore store, bool includeTimer)
    {
        var root = new SystemRoot(store);
        if (includeTimer)
            root.Add(new TimerTaskSystem());
        root.Add(new UnitRemoveNativeSystem());
        root.Add(new UnitLifecycleTransitionSystem());
        root.Add(new UnitRebornNativeSystem());
        root.Add(new UnitLifecycleDisposeSystem());
        return root;
    }

    /// <summary>统计子串出现次数。</summary>
    private static int Count(string text, string token)
    {
        var count = 0;
        for (var index = text.IndexOf(token, StringComparison.Ordinal); index >= 0;
             index = text.IndexOf(token, index + token.Length, StringComparison.Ordinal))
            count++;
        return count;
    }
}

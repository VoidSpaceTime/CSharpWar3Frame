using System;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame;
using War3Frame.Components;
using War3Frame.Systems.Native;
using War3Frame.Systems.Unit;

namespace War3Frame.Scripts.Process;

/// <summary>
/// 本地验证单位生命周期销毁顺序（方案 A）与终态清理：
/// Death→Corpse、ClearCorpse→Remove、Remove→(原生移除)→Disposing→ECS 删除；
/// 且终态清理会解绑并回收单位身上的物品。
/// 说明：单位不带 UnitNative，原生 KillUnit/RemoveUnit 分支跳过，可在无 War3 句柄的本地宿主运行。
/// </summary>
public static class UnitLifecycleValidationScenario
{
    private const string ScenarioName = "UnitLifecycleValidationScenario";

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
        CheckPhaseProgression();
        CheckItemRecycledOnDispose();

        Console.WriteLine($"{ScenarioName}: PASS");
    }

    /// <summary>阶段推进：Death→Corpse→…→Remove→Disposing→删除。</summary>
    private static void CheckPhaseProgression()
    {
        var store = new EntityStore();
        var root = CreateRoot(store);

        var unit = store.CreateEntity(new UnitLifeState { isAlive = false, lifePhase = UnitLifecyclePhase.Death });

        root.Update(new UpdateTick(0f, 0f));
        Require(unit.GetComponent<UnitLifeState>().lifePhase == UnitLifecyclePhase.Corpse, "phase/deathToCorpse");

        // 模拟尸体计时到期：Corpse→ClearCorpse 由 TimerTask 触发，这里直接置位。
        var state = unit.GetComponent<UnitLifeState>();
        state.lifePhase = UnitLifecyclePhase.ClearCorpse;
        unit.AddComponent(state);

        root.Update(new UpdateTick(0f, 0f));
        Require(unit.GetComponent<UnitLifeState>().lifePhase == UnitLifecyclePhase.Remove, "phase/clearCorpseToRemove");

        // Remove → (无 UnitNative 时)Disposing → 清理删除
        root.Update(new UpdateTick(0f, 0f));
        Require(unit.IsNull, "phase/removed");
    }

    /// <summary>终态清理：单位销毁时解绑并回收其物品。</summary>
    private static void CheckItemRecycledOnDispose()
    {
        var store = new EntityStore();
        var root = CreateRoot(store);

        var unit = store.CreateEntity(new UnitLifeState { isAlive = false, lifePhase = UnitLifecyclePhase.Remove });
        var item = store.CreateEntity(new ItemBase { templateName = "t", name = "t" });
        item.AddComponent(new ItemOwner(unit));
        item.AddTag<ItemEquippedTag>();
        item.AddComponent(new ItemSlotIndex { index = 0 });

        root.Update(new UpdateTick(0f, 0f));

        Require(unit.IsNull, "item/unitRemoved");
        Require(!item.IsNull, "item/itemStillAlive");
        Require(!item.HasComponent<ItemOwner>(), "item/ownerDetached");
        Require(item.Tags.Has<ItemDestroyPendingTag>(), "item/markedForRecycle");
    }

    /// <summary>生命周期三系统：order 10 / 20 / 30（此处按插入顺序驱动）。</summary>
    private static TimedSystemRoot CreateRoot(EntityStore store)
    {
        var root = new TimedSystemRoot(store);
        root.Add(new UnitRemoveNativeSystem(), 0f);
        root.Add(new UnitLifecycleTransitionSystem(), 0f);
        root.Add(new UnitLifecycleDisposeSystem(), 0f);
        return root;
    }

    private static void Require(bool condition, string invariantContext)
    {
        if (!condition)
            throw new InvalidOperationException($"{ScenarioName}: {invariantContext}");
    }
}

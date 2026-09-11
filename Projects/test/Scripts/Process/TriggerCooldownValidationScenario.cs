using System;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;
using War3Frame.Helpers;
using War3Frame.Src.Components;
using War3Frame.Src.Systems;

namespace War3Frame.Scripts.Process;

/// <summary>
/// 本地验证触发器 Cooldown 策略：冷却期内拦截、冷却结束后可再次触发。
/// 说明：TriggerCooldownSystem 仅对 ref 字段递减，不依赖 War3 原生句柄。
/// </summary>
public static class TriggerCooldownValidationScenario
{
    private const string ScenarioName = "TriggerCooldownValidationScenario";

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
        var store = new EntityStore();
        var root = new TimedSystemRoot(store);
        root.Add(new TriggerCooldownSystem(), 0f);
        root.Add(new TriggerSystem(), 0f);
        root.Add(new EventCleanupSystem(), 0f);

        var unit = store.CreateEntity();

        // 规则：伤害 > 100 → 治疗 1，冷却 1.0s
        TriggerHelper.Register(store, b => b
            .OnEvent<DamageEvent>()
            .When(TriggerConditions.DamageGreater(100f))
            .Cooldown(1.0f)
            .Then(TriggerActions.Heal(1f)));

        // 第一次触发
        FireDamage(store, unit, 150f);
        root.Update(new UpdateTick(0f, 0f));
        Require(CountHeal(store) == 1, "cooldown/firstTrigger");

        // 冷却中：第二次不触发
        FireDamage(store, unit, 150f);
        root.Update(new UpdateTick(0f, 0f));
        Require(CountHeal(store) == 1, "cooldown/blockedDuringCooldown");

        // 推进冷却超过 1s
        root.Update(new UpdateTick(1.1f, 0f));

        // 冷却结束：可再次触发
        FireDamage(store, unit, 150f);
        root.Update(new UpdateTick(0f, 0f));
        Require(CountHeal(store) == 2, "cooldown/triggerAfterCooldown");

        Console.WriteLine($"{ScenarioName}: PASS");
    }

    /// <summary>手动创建伤害事件实体（模拟结算系统产出）。</summary>
    private static void FireDamage(EntityStore store, Entity target, float amount)
    {
        store.CreateEntity(new DamageEvent
        {
            source = target,
            target = target,
            damage = new DamageBase
            {
                damage = amount,
                damageType = DamageType.Magical,
                damageSrc = DamageSrc.Skill,
                source = target,
                target = target
            },
            finalDamage = amount,
            remainingHealth = 0f
        }).AddComponent(new TriggerEventMarker
        {
            eventTypeId = EventTypeRegistry.Get<DamageEvent>()
        });
    }

    private static int CountHeal(EntityStore store) => store.Query<HealRequest>().Count;

    private static void Require(bool condition, string invariantContext)
    {
        if (!condition)
            throw new InvalidOperationException($"{ScenarioName}: {invariantContext}");
    }
}

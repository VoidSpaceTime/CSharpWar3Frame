using System;
using System.Collections.Generic;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;
using War3Frame.Helpers;
using War3Frame.Src.Components;
using War3Frame.Src.Systems;

namespace War3Frame.Scripts.Process;

/// <summary>
/// 以本地 ECS Store 同步验证触发器体系：
/// 规则匹配（DamageGreater/SourceIs 条件）、策略收敛（Count/Once）、动作生成 Request、
/// All/Any/not 条件组合、事件清理（EventCleanupSystem）。
/// 说明：结算系统（125/126/127）内部使用 Game.Store 创建事件，本地场景手动创建事件实体
/// 模拟结算产出，聚焦验证 TriggerSystem 与注册表逻辑。
/// </summary>
public static class TriggerValidationScenario
{
    private const string ScenarioName = "TriggerValidationScenario";

    /// <summary>运行不依赖 War3 Native 句柄的同步 ECS 验证。</summary>
    public static void Initialize(JPlayer player)
    {
        _ = player;
        RunValidation();
    }

    /// <summary>同步验证已在 Initialize 中完成，运行时更新无需操作。</summary>
    public static void Update()
    {
    }

    private static void RunValidation()
    {
        var store = new EntityStore();
        var root = CreateSystemRoot(store);

        var unitA = store.CreateEntity();
        var unitB = store.CreateEntity();
        CreateAttr(store, unitA, AttributeHelper.Health, 1000f);

        // ---- 规则注册 ----
        // r1: 伤害 > 100 → 追加伤害 50（Count 3）
        var ruleCount = TriggerHelper.Register(store, b => b
            .OnEvent<DamageEvent>()
            .When(TriggerConditions.DamageGreater(100f))
            .Count(3)
            .Then(TriggerActions.Damage(50f)));

        // r2: 伤害 > 500 → 治疗 200（Once，触发后删除）
        TriggerHelper.Register(store, b => b
            .OnEvent<DamageEvent>()
            .When(TriggerConditions.DamageGreater(500f))
            .Once()
            .Then(TriggerActions.Heal(200f)));

        // r3: Any 组合——伤害 > 1000 或 来源为 unitB
        TriggerHelper.Register(store, b => b
            .OnEvent<DamageEvent>()
            .Combine(ConditionCombine.Any)
            .When(TriggerConditions.DamageGreater(1000f))
            .When(TriggerConditions.SourceIs(unitB))
            .Count(1)
            .Then(TriggerActions.Heal(50f)));

        // r4: All + Not 组合——伤害 > 1000 且 来源不是 unitB
        TriggerHelper.Register(store, b => b
            .OnEvent<DamageEvent>()
            .When(TriggerConditions.DamageGreater(1000f))
            .When(TriggerConditions.Not(TriggerConditions.SourceIs(unitB)))
            .Count(1)
            .Then(TriggerActions.Heal(30f)));

        // r5: 无条件 → BuffApply（验证动作参数三通道）
        TriggerHelper.Register(store, b => b
            .OnEvent<DamageEvent>()
            .Count(1)
            .Then(TriggerActions.BuffApply("test_buff", AttributeHelper.Health, ModifyType.Flat, 50f, 5f)));

        // ---- Phase 1：Count 策略 + Damage 动作 ----
        FireDamage(store, unitA, unitA, 150f);
        root.Update(new UpdateTick(0f, 0f));
        // 150 > 100 → r1 命中 → 追加 DamageRequest(50)；r2/r3/r4/r5 不命中
        Require(CountComponent<DamageRequest>(store) == 1, "phase1/appendedDamage");
        Require(CountComponent<HealRequest>(store) == 0, "phase1/noHeal");
        Require(CountComponent<TriggerEventMarker>(store) == 0, "phase1/cleaned");

        FireDamage(store, unitA, unitA, 150f);
        root.Update(new UpdateTick(0f, 0f));
        // 第二次 150：r1 第 2 次 → 追加请求 +1
        Require(CountComponent<DamageRequest>(store) == 2, "phase1/secondTrigger");

        FireDamage(store, unitA, unitA, 150f);
        root.Update(new UpdateTick(0f, 0f));
        Require(CountComponent<DamageRequest>(store) == 3, "phase1/thirdTrigger");

        // 第 4 次：Count 3 已满 → 不再触发
        FireDamage(store, unitA, unitA, 150f);
        root.Update(new UpdateTick(0f, 0f));
        Require(CountComponent<DamageRequest>(store) == 3, "phase1/countCap");

        // 规则实体仍在（Count 不删）
        Require(!ruleCount.IsNull && ruleCount.HasComponent<TriggerSpec>(), "phase1/ruleAlive");

        // ---- Phase 2：Once 策略 + Heal 动作 + 规则删除 ----
        FireDamage(store, unitA, unitA, 600f);
        root.Update(new UpdateTick(0f, 0f));
        // 600 > 500 → r2 命中 → HealRequest(200)；r1 已满不触发；r3 Any：600>1000 否但 source=unitA 非 B → 不命中；r4：600>1000 否 → 不命中；r5 已触发 1 次（Count 1 满）
        Require(CountComponent<HealRequest>(store) == 1, "phase2/healOnce");
        // Once 规则实体已删除
        Require(CountOnceRules(store) == 0, "phase2/onceRemoved");

        FireDamage(store, unitA, unitA, 600f);
        root.Update(new UpdateTick(0f, 0f));
        Require(CountComponent<HealRequest>(store) == 1, "phase2/noRepeatAfterOnce");

        // ---- Phase 3：Any / All+Not 条件组合 ----
        // r3 Any：source=unitB 且伤害 50 → SourceIs 真 → 命中
        FireDamage(store, unitB, unitA, 50f);
        root.Update(new UpdateTick(0f, 0f));
        Require(CountComponent<HealRequest>(store) == 2, "phase3/anySourceIs");

        // r3 Count 1 已满；r4：伤害 1500 > 1000 且 source=unitA（非 B）→ 命中
        FireDamage(store, unitA, unitA, 1500f);
        root.Update(new UpdateTick(0f, 0f));
        Require(CountComponent<HealRequest>(store) == 3, "phase3/allNotHit");

        // r4 已满：source=unitB 伤害 1500 → All 中 Not(SourceIs(B)) 为假 → 不命中（且 r4 已删不了，仍存在但条件假）
        FireDamage(store, unitB, unitA, 1500f);
        root.Update(new UpdateTick(0f, 0f));
        Require(CountComponent<HealRequest>(store) == 3, "phase3/notBlocks");

        // ---- Phase 4：事件清理（每帧后事件实体为 0）----
        // 上面的每次 Update 已断言清理；此处显式验证计数
        FireDamage(store, unitA, unitA, 10f);
        root.Update(new UpdateTick(0f, 0f));
        Require(CountComponent<TriggerEventMarker>(store) == 0, "phase4/cleanup");

        // ---- Phase 5：BuffApply 动作（参数三通道）----
        FireDamage(store, unitA, unitA, 10f);
        root.Update(new UpdateTick(0f, 0f));
        // r5 无条件 → 命中（第 1 次）→ BuffApplyRequest(buffId=test_buff, attr=Health, Flat, 50, 5s)
        Require(CountComponent<BuffApplyRequest>(store) == 1, "phase5/buffApply");
        var buffRequest = FirstComponent<BuffApplyRequest>(store);
        Require(buffRequest.buffId == "test_buff" && buffRequest.attrTypeId == AttributeHelper.Health
                && buffRequest.modifyType == ModifyType.Flat && buffRequest.value == 50f && buffRequest.duration == 5f,
            "phase5/buffParams");

        Console.WriteLine($"{ScenarioName}: PASS");
    }

    /// <summary>按数据流顺序创建本地系统（结算系统因 Game.Store 依赖不注册，事件手动创建）。</summary>
    private static TimedSystemRoot CreateSystemRoot(EntityStore store)
    {
        var root = new TimedSystemRoot(store);
        root.Add(new AttrCalculationSystem(), 0f);
        root.Add(new TriggerSystem(), 0f);
        root.Add(new EventCleanupSystem(), 0f);
        return root;
    }

    /// <summary>手动创建伤害事件实体（模拟结算系统产出）。</summary>
    private static void FireDamage(EntityStore store, Entity source, Entity target, float amount)
    {
        store.CreateEntity(new DamageEvent
        {
            source = source,
            target = target,
            damage = new DamageBase
            {
                damage = amount,
                damageType = DamageType.Magical,
                damageSrc = DamageSrc.Skill,
                source = source,
                target = target,
            },
            finalDamage = amount,
            remainingHealth = 0f,
        }).AddComponent(new TriggerEventMarker
        {
            eventTypeId = EventTypeRegistry.Get<DamageEvent>()
        });
    }

    /// <summary>本地建属性实体（绕开 AttributeHelper 的 Game.Store 依赖）。</summary>
    private static Entity CreateAttr(EntityStore store, Entity unit, int typeId, float baseValue)
    {
        var attr = store.CreateEntity(
            new AttrTypeId { typeId = typeId },
            new AttrValue { baseValue = baseValue, finalValue = baseValue, current = baseValue },
            new AttrOwner(unit));
        unit.AddRelation(new HasAttr(attr, typeId));
        return attr;
    }

    private static int CountComponent<T>(EntityStore store) where T : struct, IComponent
    {
        return store.Query<T>().Count;
    }

    private static T FirstComponent<T>(EntityStore store) where T : struct, IComponent
    {
        var result = default(T);
        var found = false;
        store.Query<T>().ForEachEntity((ref T component, Entity _) =>
        {
            if (found)
                return;
            result = component;
            found = true;
        });
        Require(found, "firstComponent: 未找到组件");
        return result;
    }

    private static int CountOnceRules(EntityStore store)
    {
        var count = 0;
        store.Query<TriggerSpec>().ForEachEntity((ref TriggerSpec spec, Entity _) =>
        {
            if (spec.policy.kind == TriggerPolicyKind.Once)
                count++;
        });
        return count;
    }

    private static void Require(bool condition, string invariantContext)
    {
        if (!condition)
            throw new InvalidOperationException($"{ScenarioName}: {invariantContext}");
    }
}
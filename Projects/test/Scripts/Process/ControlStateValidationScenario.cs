using System;
using System.Collections.Generic;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;
using War3Frame.Src.Systems;

namespace War3Frame.Scripts.Process;

/// <summary>
/// 以本地 ECS Store 同步验证控制状态叠加态（方案 B）：
/// 多来源叠加基于属性 finalValue 聚合；0↔正 跳变恰好一次；
/// 免疫压制按有效值判定。
/// 说明：AttributeHelper/ModifyHelper 内部硬编码 Game.Store（游戏内正常），
/// 本地场景直接用 store 创建属性/修改器实体，绕开全局 Store 依赖。
/// </summary>
public static class ControlStateValidationScenario
{
    private const string ScenarioName = "ControlStateValidationScenario";
    private static readonly List<Entity> _modifiers = new();

    /// <summary>
    /// 运行不依赖 War3 Native 句柄的同步 ECS 验证。
    /// </summary>
    public static void Initialize(JPlayer player)
    {
        _ = player;
        RunValidation();
    }

    /// <summary>
    /// 同步验证已在 Initialize 中完成，运行时更新无需操作。
    /// </summary>
    public static void Update()
    {
        // 保留入口以匹配测试客户端时钟。
    }

    private static void RunValidation()
    {
        var store = new EntityStore();
        var root = CreateSystemRoot(store);

        // 单位 + 眩晕/免疫属性（基础值 0，施加走修改器）
        var unit = store.CreateEntity();
        var stunAttr = CreateAttr(store, unit, AttributeHelper.Stun, 0f);
        var immunityAttr = CreateAttr(store, unit, AttributeHelper.StunImmunity, 0f);

        var sourceA = store.CreateEntity();
        var sourceB = store.CreateEntity();
        var sourceC = store.CreateEntity();
        var sourceD = store.CreateEntity();

        // ---- Phase 1：双来源叠加，恰好一次 entered ----
        AddModifier(store, stunAttr, sourceA, ModifyType.Flat, 1f);
        AddModifier(store, stunAttr, sourceB, ModifyType.Flat, 1f);
        root.Update(new UpdateTick(0f, 0f));

        Require(ControlHelper.GetEffectiveValue(unit, AttributeHelper.Stun) == 2f,
            "phase1/finalValue: 双来源叠加后有效值应为 2");
        Require(CountEvents(store, entered: true) == 1,
            "phase1/enter: 预期恰好 1 次进入事件");
        Require(CountRequests(store, entered: true) == 1,
            "phase1/request: 预期恰好 1 个进入请求");

        var enterEvent = FirstEvent(store, entered: true);
        Require(enterEvent.unit == unit && enterEvent.controlType == ControlType.Stun,
            "phase1/eventFields: 进入事件字段（unit/Stun）不正确");

        // ---- Phase 2：移除一个来源仍眩晕，无新跳变 ----
        RemoveModifier(sourceA, stunAttr);
        root.Update(new UpdateTick(0f, 0f));

        Require(ControlHelper.GetEffectiveValue(unit, AttributeHelper.Stun) == 1f,
            "phase2/finalValue: 移除一个来源后有效值应为 1");
        Require(CountEvents(store, entered: true) == 1 && CountEvents(store, entered: false) == 0,
            "phase2/silent: 仍处于眩晕不应产生新事件");

        // ---- Phase 3：移除全部来源，恰好一次 exited ----
        RemoveModifier(sourceB, stunAttr);
        root.Update(new UpdateTick(0f, 0f));

        Require(ControlHelper.GetEffectiveValue(unit, AttributeHelper.Stun) == 0f,
            "phase3/finalValue: 全部移除后有效值应为 0");
        Require(CountEvents(store, entered: false) == 1,
            "phase3/exit: 预期恰好 1 次解除事件");
        Require(CountRequests(store, entered: false) == 1,
            "phase3/request: 预期恰好 1 个解除请求");

        // ---- Phase 4：免疫压制按有效值判定 ----
        AddModifier(store, stunAttr, sourceC, ModifyType.Flat, 1f);
        root.Update(new UpdateTick(0f, 0f));
        Require(CountEvents(store, entered: true) == 2,
            "phase4/reEnter: 重新施加后应再次进入");

        // 免疫属性实体在 Phase 1 已创建（immunityAttr），这里直接挂修改器
        AddModifier(store, immunityAttr, sourceD, ModifyType.Flat, 1f);
        root.Update(new UpdateTick(0f, 0f));
        Require(ControlHelper.GetEffectiveValue(unit, AttributeHelper.Stun) == 0f,
            "phase4/immunity: 免疫应压制有效值");
        Require(CountEvents(store, entered: false) == 2,
            "phase4/immunityExit: 免疫生效应视为解除");

        RemoveModifier(sourceD, immunityAttr);
        root.Update(new UpdateTick(0f, 0f));
        Require(CountEvents(store, entered: true) == 3,
            "phase4/immunityRemove: 免疫移除且仍有来源应再次进入");

        // ---- Phase 5：属性整体移除后快照清理 + 残留控制补发解除 ----
        RemoveAllAttrs(store, unit);
        root.Update(new UpdateTick(0f, 0f));
        Require(!unit.HasComponent<ControlStateSnapshot>(),
            "phase5/snapshot: 无控制/免疫属性后快照应被清理");
        Require(CountEvents(store, entered: false) == 3 && CountRequests(store, entered: false) == 3,
            "phase5/release: 属性整体移除时残留眩晕应补发解除（避免原生永久暂停）");

        Console.WriteLine($"{ScenarioName}: PASS");
    }

    /// <summary>
    /// 按验证数据流顺序创建本地系统，强制每次更新立即执行。
    /// 不注册 Native 系统（本地无 JUnit 句柄，Native 副作用不在本地验证）。
    /// </summary>
    private static TimedSystemRoot CreateSystemRoot(EntityStore store)
    {
        var root = new TimedSystemRoot(store);
        root.Add(new AttrCalculationSystem(), 0f);
        root.Add(new ControlStateTransitionSystem(), 0f);
        return root;
    }

    /// <summary>本地建属性实体（对应 AttributeHelper.CreateAttr，绕开 Game.Store）。</summary>
    private static Entity CreateAttr(EntityStore store, Entity unit, int typeId, float baseValue)
    {
        var attr = store.CreateEntity(
            new AttrTypeId { typeId = typeId },
            new AttrValue { baseValue = baseValue, finalValue = baseValue, current = baseValue },
            new AttrOwner(unit));
        unit.AddRelation(new HasAttr(attr, typeId));
        return attr;
    }

    /// <summary>本地建 Flat 修改器并打 AttrDirty（对应 ModifyHelper.AddModifier，绕开 Game.Store）。</summary>
    private static void AddModifier(EntityStore store, Entity attr, Entity source, ModifyType type, float value)
    {
        var mod = store.CreateEntity(
            new ModifyValue { modifyType = type, value = value },
            new ModifyTarget(attr),
            new ModifySource(source));
        _modifiers.Add(mod);
        attr.AddTag<AttrDirty>();
    }

    /// <summary>按 source 移除修改器（场景内记录列表，避免 GetIncomingLinks 泛型二义）。</summary>
    private static void RemoveModifier(Entity source, Entity attr)
    {
        for (var i = _modifiers.Count - 1; i >= 0; i--)
        {
            if (_modifiers[i].IsNull)
                continue;

            if (_modifiers[i].TryGetComponent<ModifySource>(out var modSource) && modSource.source == source)
            {
                _modifiers[i].DeleteEntity();
                _modifiers.RemoveAt(i);
            }
        }

        attr.AddTag<AttrDirty>();
    }

    /// <summary>删除单位全部属性与修改器（对应 AttributeHelper.RemoveAllAttrs 的本地版）。</summary>
    private static void RemoveAllAttrs(EntityStore store, Entity unit)
    {
        foreach (var mod in _modifiers)
        {
            if (!mod.IsNull)
                mod.DeleteEntity();
        }

        _modifiers.Clear();

        var attrs = new List<Entity>();
        foreach (ref var rel in unit.GetRelations<HasAttr>())
            attrs.Add(rel.attrEntity);
        foreach (var attr in attrs)
            attr.DeleteEntity();
    }

    private static int CountEvents(EntityStore store, bool entered)
    {
        var count = 0;
        store.Query<ControlStateChangedEvent>().ForEachEntity((ref ControlStateChangedEvent evt, Entity _) =>
        {
            if (evt.entered == entered)
                count++;
        });
        return count;
    }

    private static int CountRequests(EntityStore store, bool entered)
    {
        var count = 0;
        store.Query<ControlStateNativeRequest>().ForEachEntity((ref ControlStateNativeRequest req, Entity _) =>
        {
            if (req.entered == entered)
                count++;
        });
        return count;
    }

    private static ControlStateChangedEvent FirstEvent(EntityStore store, bool entered)
    {
        var result = default(ControlStateChangedEvent);
        var found = false;
        store.Query<ControlStateChangedEvent>().ForEachEntity((ref ControlStateChangedEvent evt, Entity _) =>
        {
            if (found || evt.entered != entered)
                return;
            result = evt;
            found = true;
        });
        Require(found, "firstEvent: 未找到事件");
        return result;
    }

    private static void Require(bool condition, string invariantContext)
    {
        if (!condition)
            throw new InvalidOperationException($"{ScenarioName}: {invariantContext}");
    }
}
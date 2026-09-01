using System;
using System.Collections.Generic;
using Friflo.Engine.ECS;
using War3Frame;

namespace War3Frame.Src.Components;

/// <summary>
/// 事件类型注册表：为每种事件组件分配稳定 typeId，供 TriggerEventMarker 使用。
/// 静态注册（同 EffectFormulaRegistry 形态）；新事件类型 Register 后无需改动 TriggerSystem。
/// </summary>
public static class EventTypeRegistry
{
    private static readonly Dictionary<int, string> _types = new();
    private static readonly Dictionary<Type, int> _idsByType = new();
    private static int _nextId = 1;

    static EventTypeRegistry()
    {
        RegisterBuiltIn();
    }

    /// <summary>注册事件组件类型，返回稳定 typeId（重复注册返回已有 id）。</summary>
    public static int Register<T>() where T : struct, IComponent
    {
        var type = typeof(T);
        if (_idsByType.TryGetValue(type, out var existing))
            return existing;

        var id = _nextId++;
        _idsByType.Add(type, id);
        _types.Add(id, type.Name);
        return id;
    }

    /// <summary>获取事件组件类型的 typeId（未注册返回 0）。</summary>
    public static int Get<T>() where T : struct, IComponent
    {
        return _idsByType.TryGetValue(typeof(T), out var id) ? id : 0;
    }

    /// <summary>内置事件登记：结算事件与控制状态事件。</summary>
    private static void RegisterBuiltIn()
    {
        Register<DamageEvent>();
        Register<HealEvent>();
        Register<BuffAppliedEvent>();
        Register<ControlStateChangedEvent>();
    }
}

/// <summary>
/// 事件标记：挂事件实体，TriggerSystem / EventCleanupSystem 单查询发现全部事件。
/// </summary>
public struct TriggerEventMarker : IComponent
{
    public int eventTypeId;
}

/// <summary>条件组合模式（单根：整组 All 或 Any，叶子可 not）。</summary>
public enum ConditionCombine
{
    All,
    Any,
}

/// <summary>条件（叶子）：注册表键 + not 标志 + 三通道参数。</summary>
public struct TriggerCondition
{
    /// <summary>TriggerConditionRegistry 键（0 = 恒真）。</summary>
    public int conditionId;

    /// <summary>取反标志。</summary>
    public bool not;

    /// <summary>数值参数（如伤害阈值）。</summary>
    public float[] paramF;

    /// <summary>字符串参数（如 buffId）。</summary>
    public string[] paramS;

    /// <summary>实体参数（如目标单位）。</summary>
    public Entity[] paramE;
}

/// <summary>动作：注册表键 + 三通道参数。</summary>
public struct TriggerAction
{
    /// <summary>TriggerActionRegistry 键。</summary>
    public int actionId;

    public float[] paramF;
    public string[] paramS;
    public Entity[] paramE;
}

/// <summary>触发策略类型。</summary>
public enum TriggerPolicyKind
{
    /// <summary>一次性：触发一次后规则实体删除。</summary>
    Once,

    /// <summary>冷却：触发后冷却 cooldown 秒。</summary>
    Cooldown,

    /// <summary>次数：最多触发 maxCount 次。</summary>
    Count,
}

/// <summary>触发策略配置。</summary>
public struct TriggerPolicy
{
    public TriggerPolicyKind kind;
    public float cooldown;
    public int maxCount;
}

/// <summary>
/// 触发器规则配置组件（挂独立触发器实体）。
/// 规则 = 匹配事件类型 + 条件（单根组合）+ 策略 + 动作。
/// </summary>
public struct TriggerSpec : IComponent
{
    /// <summary>匹配的事件类型（0 = 匹配全部事件）。</summary>
    public int eventTypeId;

    /// <summary>条件组合模式。</summary>
    public ConditionCombine combine;

    /// <summary>条件列表（空 = 无条件）。</summary>
    public TriggerCondition[] conditions;

    /// <summary>触发策略。</summary>
    public TriggerPolicy policy;

    /// <summary>动作列表（命中后依次执行）。</summary>
    public TriggerAction[] actions;
}

/// <summary>触发器规则状态组件（挂触发器实体，由 TriggerSystem 推进）。</summary>
public struct TriggerRuntime : IComponent
{
    /// <summary>冷却剩余秒（Cooldown 策略）。</summary>
    public float cooldownRemain;

    /// <summary>已触发次数（Count/Once 策略）。</summary>
    public int triggerCount;
}

/// <summary>条件/动作上下文：携带 store 与事件/规则实体引用。</summary>
public readonly struct TriggerContext
{
    public readonly EntityStore Store;
    public readonly Entity EventEntity;
    public readonly Entity TriggerEntity;

    public TriggerContext(EntityStore store, Entity eventEntity, Entity triggerEntity)
    {
        Store = store;
        EventEntity = eventEntity;
        TriggerEntity = triggerEntity;
    }
}
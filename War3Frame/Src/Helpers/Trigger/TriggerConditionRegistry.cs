using System.Collections.Generic;
using Friflo.Engine.ECS;
using War3Frame.Src.Components;

namespace War3Frame.Helpers;

/// <summary>触发器条件处理委托：返回条件是否满足（禁止非确定性源）。</summary>
public delegate bool TriggerConditionHandler(TriggerContext ctx, TriggerCondition condition);

/// <summary>
/// 触发器条件注册表（同 EffectFormulaRegistry 形态）。
/// 内置 id：1=DamageGreater、2=TargetIs、3=SourceIs；自定义条件 Register 从 100 起分配。
/// 注册表内禁止使用 Random/DateTime 等非确定性源，保证锁步一致。
/// </summary>
public static class TriggerConditionRegistry
{
    private static readonly SortedDictionary<int, TriggerConditionHandler> _handlers = new();
    private static readonly SortedDictionary<int, string> _names = new();
    private static int _nextId = 100;

    static TriggerConditionRegistry()
    {
        _handlers.Add(1, DamageGreater);
        _names.Add(1, nameof(DamageGreater));
        _handlers.Add(2, TargetIs);
        _names.Add(2, nameof(TargetIs));
        _handlers.Add(3, SourceIs);
        _names.Add(3, nameof(SourceIs));
    }

    /// <summary>注册自定义条件，返回分配的条件 id。</summary>
    public static int Register(TriggerConditionHandler handler)
    {
        var id = _nextId++;
        _handlers.Add(id, handler);
        _names.Add(id, handler.Method.Name);
        return id;
    }

    /// <summary>按 id 获取条件处理（0 = 恒真，未注册返回 false）。</summary>
    public static bool TryGet(int id, out TriggerConditionHandler handler)
    {
        return _handlers.TryGetValue(id, out handler);
    }

    /// <summary>按 id 获取条件名（调试/日志可观测性；未注册返回 #id）。</summary>
    public static string GetName(int id)
    {
        return _names.TryGetValue(id, out var name) ? name : $"#{id}";
    }

    /// <summary>内置：事件为伤害事件且 finalDamage 大于阈值。</summary>
    private static bool DamageGreater(TriggerContext ctx, TriggerCondition c)
    {
        return ctx.EventEntity.TryGetComponent<DamageEvent>(out var evt)
               && evt.finalDamage > GetF(c.paramF, 0);
    }

    /// <summary>内置：事件目标等于指定单位。</summary>
    private static bool TargetIs(TriggerContext ctx, TriggerCondition c)
    {
        return GetE(c.paramE, 0) == GetEventTarget(ctx);
    }

    /// <summary>内置：事件来源等于指定单位。</summary>
    private static bool SourceIs(TriggerContext ctx, TriggerCondition c)
    {
        return GetE(c.paramE, 0) == GetEventSource(ctx);
    }

    /// <summary>从事件实体提取目标单位（按事件类型读取对应组件字段）。</summary>
    internal static Entity GetEventTarget(TriggerContext ctx)
    {
        var typeId = ctx.EventEntity.GetComponent<TriggerEventMarker>().eventTypeId;
        if (typeId == EventTypeRegistry.Get<DamageEvent>())
            return ctx.EventEntity.GetComponent<DamageEvent>().target;
        if (typeId == EventTypeRegistry.Get<HealEvent>())
            return ctx.EventEntity.GetComponent<HealEvent>().target;
        if (typeId == EventTypeRegistry.Get<BuffAppliedEvent>())
            return ctx.EventEntity.GetComponent<BuffAppliedEvent>().target;
        if (typeId == EventTypeRegistry.Get<ControlStateChangedEvent>())
            return ctx.EventEntity.GetComponent<ControlStateChangedEvent>().unit;
        return default;
    }

    /// <summary>从事件实体提取来源单位（控制事件无来源，返回 default）。</summary>
    internal static Entity GetEventSource(TriggerContext ctx)
    {
        var typeId = ctx.EventEntity.GetComponent<TriggerEventMarker>().eventTypeId;
        if (typeId == EventTypeRegistry.Get<DamageEvent>())
            return ctx.EventEntity.GetComponent<DamageEvent>().source;
        if (typeId == EventTypeRegistry.Get<HealEvent>())
            return ctx.EventEntity.GetComponent<HealEvent>().source;
        if (typeId == EventTypeRegistry.Get<BuffAppliedEvent>())
            return ctx.EventEntity.GetComponent<BuffAppliedEvent>().source;
        return default;
    }

    private static float GetF(float[]? arr, int idx) => arr != null && idx < arr.Length ? arr[idx] : 0f;
    private static Entity GetE(Entity[]? arr, int idx) => arr != null && idx < arr.Length ? arr[idx] : default;
}
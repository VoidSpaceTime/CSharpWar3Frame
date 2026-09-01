using System.Collections.Generic;
using Friflo.Engine.ECS;
using War3Frame.Components;
using War3Frame.Src.Components;

namespace War3Frame.Helpers;

/// <summary>触发器动作处理委托：只允许生成已有 Request，禁止调用 War3 原生 API。</summary>
public delegate void TriggerActionHandler(TriggerContext ctx, TriggerAction action);

/// <summary>
/// 触发器动作注册表（同 EffectFormulaRegistry 形态）。
/// 内置 id：1=Damage、2=Heal、3=BuffApply；自定义动作 Register 从 100 起分配。
/// 动作只创建/附加 Request 组件，由现有 Resolve 系统消费；禁止 War3 原生调用。
/// </summary>
public static class TriggerActionRegistry
{
    private static readonly SortedDictionary<int, TriggerActionHandler> _handlers = new();
    private static int _nextId = 100;

    static TriggerActionRegistry()
    {
        _handlers.Add(1, Damage);
        _handlers.Add(2, Heal);
        _handlers.Add(3, BuffApply);
    }

    /// <summary>注册自定义动作，返回分配的动作 id。</summary>
    public static int Register(TriggerActionHandler handler)
    {
        var id = _nextId++;
        _handlers.Add(id, handler);
        return id;
    }

    /// <summary>按 id 获取动作处理。</summary>
    public static bool TryGet(int id, out TriggerActionHandler handler)
    {
        return _handlers.TryGetValue(id, out handler);
    }

    /// <summary>内置：追加伤害（paramF[0]=amount），创建 DamageRequest 独立实体。</summary>
    private static void Damage(TriggerContext ctx, TriggerAction a)
    {
        var source = TriggerConditionRegistry.GetEventSource(ctx);
        var target = TriggerConditionRegistry.GetEventTarget(ctx);
        if (target.IsNull)
            return;

        ctx.Store.CreateEntity(new DamageRequest
        {
            source = source,
            target = target,
            damage = new DamageBase
            {
                damage = GetF(a.paramF, 0),
                damageType = DamageType.Magical,
                damageSrc = DamageSrc.Skill,
                source = source,
                target = target,
            }
        });
    }

    /// <summary>内置：治疗（paramF[0]=amount），创建 HealRequest 独立实体。</summary>
    private static void Heal(TriggerContext ctx, TriggerAction a)
    {
        var source = TriggerConditionRegistry.GetEventSource(ctx);
        var target = TriggerConditionRegistry.GetEventTarget(ctx);
        if (target.IsNull)
            return;

        ctx.Store.CreateEntity(new HealRequest
        {
            source = source,
            target = target,
            amount = GetF(a.paramF, 0)
        });
    }

    /// <summary>
    /// 内置：施加 Buff（paramS[0]=buffId、paramF[0]=attrTypeId、paramF[1]=modifyType、
    /// paramF[2]=value、paramF[3]=duration），创建 BuffApplyRequest 独立实体。
    /// </summary>
    private static void BuffApply(TriggerContext ctx, TriggerAction a)
    {
        var source = TriggerConditionRegistry.GetEventSource(ctx);
        var target = TriggerConditionRegistry.GetEventTarget(ctx);
        if (target.IsNull || string.IsNullOrEmpty(GetS(a.paramS, 0)))
            return;

        ctx.Store.CreateEntity(new BuffApplyRequest
        {
            source = source,
            target = target,
            buffId = GetS(a.paramS, 0),
            attrTypeId = (int)GetF(a.paramF, 0),
            modifyType = (ModifyType)(int)GetF(a.paramF, 1),
            value = GetF(a.paramF, 2),
            duration = GetF(a.paramF, 3),
            refreshBehavior = default
        });
    }

    private static float GetF(float[]? arr, int idx) => arr != null && idx < arr.Length ? arr[idx] : 0f;
    private static string GetS(string[]? arr, int idx) => arr != null && idx < arr.Length ? arr[idx] ?? string.Empty : string.Empty;
}
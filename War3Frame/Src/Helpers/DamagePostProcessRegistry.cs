using System;
using System.Collections.Generic;
using Friflo.Engine.ECS;
using War3Frame.Src.Components;

namespace War3Frame.Helpers;

/// <summary>
/// 伤害落地后处理钩子委托（吸血/反射/溅射/格挡等扩展位）。
/// 语义约束：只允许读 ctx、只发新的 Request（吸血→HealRequest、反射→对 source 的 DamageRequest），
/// 禁止调用 War3 原生 API（分层约束）。护盾吸收是唯一例外，走 Apply 前列表，见注册表说明。
/// </summary>
public delegate void DamagePostProcessHandler(ref DamageContext ctx, EntityStore store);

/// <summary>
/// 伤害后处理注册表，分两段：扣血前吸收（仅内置护盾）与伤害事件后处理（吸血/反射/外部扩展）。
/// 注册表是吸血、反射、护盾的唯一扩展点；伤害主流程不得内联这三类业务分支。
/// 外部 <see cref="Register"/> 只能进入 Event 后列表，无法插入护盾吸收之前，避免双重吸收。
/// </summary>
public static class DamagePostProcessRegistry
{
    private static readonly List<DamagePostProcessHandler> _absorbHandlers = new();
    private static readonly List<DamagePostProcessHandler> _postHandlers = new();

    static DamagePostProcessRegistry()
    {
        // 内置护盾吸收：唯一允许在扣血前写 ctx.absorbed / ctx.finalDamage 的处理器。
        _absorbHandlers.Add(AbsorbDamageShield);
        // 内置吸血与反射：仅读 ctx，按注册顺序在伤害事件之后执行。
        _postHandlers.Add(ApplyLifesteal);
        _postHandlers.Add(ApplyDamageReflect);
    }

    /// <summary>注册落地后钩子；外部处理器只能进入 Event 后列表。</summary>
    public static void Register(DamagePostProcessHandler handler)
    {
        _postHandlers.Add(handler);
    }

    /// <summary>扣血前吸收：执行内置护盾处理器，允许改 ctx.absorbed 与 ctx.finalDamage。</summary>
    public static void RunAbsorb(ref DamageContext ctx, EntityStore store)
    {
        for (var i = 0; i < _absorbHandlers.Count; i++)
            _absorbHandlers[i](ref ctx, store);
    }

    /// <summary>伤害事件后处理：执行吸血、反射及外部扩展；调用方需按 skipPostProcess / 免疫自行跳过。</summary>
    public static void RunPost(ref DamageContext ctx, EntityStore store)
    {
        for (var i = 0; i < _postHandlers.Count; i++)
            _postHandlers[i](ref ctx, store);
    }

    /// <summary>护盾吸收：按受击者 DamageShield.current 吸收待扣伤害，回写剩余护盾与实际扣血。</summary>
    private static void AbsorbDamageShield(ref DamageContext ctx, EntityStore store)
    {
        _ = store;
        if (ctx.isImmune || ctx.finalDamage <= 0f)
            return;

        var current = AttributeHelper.GetCurrent(ctx.target, AttributeHelper.DamageShield);
        if (current <= 0f)
            return;

        var absorbed = MathF.Min(current, ctx.finalDamage);
        AttributeHelper.SetCurrent(ctx.target, AttributeHelper.DamageShield, current - absorbed);
        ctx.absorbed += absorbed;
        ctx.finalDamage -= absorbed;
    }

    /// <summary>吸血：按攻击者 Lifesteal 最终值（夹紧 0..1）对实际扣血发一条自我治疗请求。</summary>
    private static void ApplyLifesteal(ref DamageContext ctx, EntityStore store)
    {
        if (ctx.isImmune || ctx.finalDamage <= 0f || ctx.source.IsNull)
            return;

        var ratio = Math.Clamp(AttributeHelper.GetFinalValue(ctx.source, AttributeHelper.Lifesteal), 0f, 1f);
        if (ratio <= 0f)
            return;

        store.CreateEntity(new HealRequest
        {
            source = ctx.source,
            target = ctx.source,
            amount = ctx.finalDamage * ratio
        });
    }

    /// <summary>反射：按受击者 DamageReflect 最终值（夹紧 0..1）对攻击者发真实伤害请求，并跳过其后续处理。</summary>
    private static void ApplyDamageReflect(ref DamageContext ctx, EntityStore store)
    {
        if (ctx.isImmune || ctx.finalDamage <= 0f || ctx.source.IsNull || ctx.target.IsNull)
            return;

        var ratio = Math.Clamp(AttributeHelper.GetFinalValue(ctx.target, AttributeHelper.DamageReflect), 0f, 1f);
        if (ratio <= 0f)
            return;

        store.CreateEntity(new DamageRequest
        {
            source = ctx.target,
            target = ctx.source,
            skipPostProcess = true,
            damage = new DamageBase
            {
                damage = ctx.finalDamage * ratio,
                damageType = DamageType.Real,
                damageSrc = ctx.damageSrc,
                source = ctx.target,
                target = ctx.source
            }
        });
    }
}

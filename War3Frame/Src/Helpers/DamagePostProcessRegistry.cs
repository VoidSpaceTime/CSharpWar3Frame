using System.Collections.Generic;
using Friflo.Engine.ECS;
using War3Frame.Src.Components;

namespace War3Frame.Helpers;

/// <summary>
/// 伤害落地后处理钩子委托（吸血/反射/溅射/格挡等扩展位）。
/// 语义约束：只允许读 ctx 并发新的 Request（吸血→HealRequest、反射→对 source 的 DamageRequest），
/// 禁止直接修改 ctx.finalDamage、禁止调用 War3 原生 API（分层约束）。
/// </summary>
public delegate void DamagePostProcessHandler(ref DamageContext ctx, EntityStore store);

/// <summary>
/// 伤害落地后钩子注册表。
/// 本提案不注册任何内置实现；由后续专项提案（吸血/护盾/反射等）注册扩展。
/// </summary>
public static class DamagePostProcessRegistry
{
    private static readonly List<DamagePostProcessHandler> _handlers = new();

    /// <summary>注册落地后钩子；按注册顺序在每次伤害结算后执行。</summary>
    public static void Register(DamagePostProcessHandler handler)
    {
        _handlers.Add(handler);
    }

    /// <summary>执行全部已注册钩子（当前无内置实现，列表为空时零开销）。</summary>
    public static void Run(ref DamageContext ctx, EntityStore store)
    {
        if (_handlers.Count == 0)
            return;

        for (var i = 0; i < _handlers.Count; i++)
            _handlers[i](ref ctx, store);
    }
}

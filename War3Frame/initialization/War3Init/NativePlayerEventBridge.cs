using System.Collections.Generic;
using Friflo.Engine.ECS;
using War3Frame;
using War3Frame.Helpers;
using War3Frame.Src.Components;

namespace War3Frame.Initialization;

/// <summary>
/// 原生玩家单位事件桥：把 War3 玩家单位事件（EVENT_PLAYER_UNIT_*）桥接为 ECS 事件实体。
/// 注册方式：C# 循环 16 玩家调用 TriggerRegisterPlayerUnitEvent（等价 AnyUnitEventBJ，但不封装 BJ），
/// 回调闭包已捕获所属玩家，无需 GetTriggerPlayer 反查。
/// 回调只做桥接翻译（native handle → ECS entity 反查 → 建带 TriggerEventMarker 的事件实体），
/// 不执行业务逻辑；业务由触发器体系（TriggerSpec）与攻击模拟等消费方处理。
/// </summary>
public static class NativePlayerEventBridge
{
    // 保活：原生 trigger 无 ECS 引用，防止被 GC 回收导致事件失效。
    private static readonly List<JTrigger> _triggers = new();

    /// <summary>注册全部玩家单位事件桥（须在玩家创建后调用一次）。</summary>
    public static void Initialize()
    {
        foreach (var player in PlayerHelper.Players)
        {
            if (player.player.Handle == System.IntPtr.Zero)
                continue;
            RegisterPlayerUnitAttacked(player.player);
        }
    }

    /// <summary>注册"任意单位被攻击"事件桥（EVENT_PLAYER_UNIT_ATTACKED）。</summary>
    private static void RegisterPlayerUnitAttacked(JPlayer player)
    {
        var trigger = JassApi.CreateTrigger();
        HandleHelper.HandleAdd(trigger);

        var condition = JassApi.Condition(() =>
        {
            var attackerNative = JassApi.GetAttacker();
            var targetNative = JassApi.GetTriggerUnit();
            if (!NativeEntityIndex.TryGetByNative(attackerNative, out var attacker))
                return;
            if (!NativeEntityIndex.TryGetByNative(targetNative, out var target))
                return;

            // 模拟攻击请求：AttackSimulationSystem 唯一消费（近战直发伤害 / 远程走投射物）。
            Game.Store.CreateEntity(new UnitAttackedRequest
            {
                attacker = attacker,
                target = target
            });

            // 对外事实广播：TriggerSpec 只读匹配，EventCleanupSystem 统一清理。
            Game.Store.CreateEntity(
                new UnitAttackedEvent
                {
                    attacker = attacker,
                    target = target
                },
                new TriggerEventMarker
                {
                    eventTypeId = EventTypeRegistry.Get<UnitAttackedEvent>()
                });
        });

        JassApi.TriggerAddCondition(trigger, condition);
        JassApi.TriggerRegisterPlayerUnitEvent(trigger, player,
            Blizzard.EVENT_PLAYER_UNIT_ATTACKED, null);
        _triggers.Add(trigger);
    }
}

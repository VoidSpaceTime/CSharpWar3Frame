using System.Collections.Generic;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace War3Frame.Systems;

/// <summary>
/// 击杀奖励系统：消费 UnitDiedEvent，为击杀者派发经验请求。
/// 仅 source 非空（击杀死亡）且死者 expReward &gt; 0 时派发；非击杀死亡/无奖励不派发。
/// 事件实体留给 EventCleanupSystem（order 132）统一清理，本系统不删除，允许多监听。
/// </summary>
[SystemRegister(SystemKind.Interval, 126)]
public class KillRewardSystem : QuerySystem<UnitDiedEvent>
{
    // Friflo 约束：Query 迭代内禁止结构变更，先收集快照，循环外统一派发。
    private readonly List<(UnitDiedEvent evt, Entity eventEntity)> _pending = new();

    protected override void OnUpdate()
    {
        _pending.Clear();
        Query.ForEachEntity((ref UnitDiedEvent evt, Entity eventEntity) =>
        {
            _pending.Add((evt, eventEntity));
        });

        foreach (var (evt, eventEntity) in _pending)
        {
            TryDispatchKillReward(evt);
        }
    }

    /// <summary>读取死者 expReward，向击杀者创建经验请求；不满足条件时静默跳过。</summary>
    private static void TryDispatchKillReward(in UnitDiedEvent evt)
    {
        if (evt.source.IsNull || evt.unit.IsNull)
            return;

        if (!evt.unit.TryGetComponent<UnitKillRewardData>(out var reward))
            return;

        var victimLevel = evt.unit.TryGetComponent<UnitLevel>(out var unitLevel)
            ? Math.Max(1, unitLevel.level)
            : 1;
        var amount = reward.expReward.Resolve(victimLevel);
        if (amount <= 0f)
            return;

        evt.unit.Store.CreateEntity(new ExperienceGainRequest
        {
            target = evt.source,
            amount = amount,
            multiplier = 1f,
            source = evt.source,
            sourceType = "kill"
        });
    }
}

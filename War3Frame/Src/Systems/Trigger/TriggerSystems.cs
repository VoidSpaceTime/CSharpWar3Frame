using System.Collections.Generic;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using War3Frame.Components;
using War3Frame.Helpers;
using War3Frame.Src.Components;
using War3Frame.Systems;

namespace War3Frame.Src.Systems;

/// <summary>
/// 触发器匹配系统。
/// 扫描事件实体（TriggerEventMarker）与规则实体（TriggerSpec），按 eventTypeId 索引匹配：
/// 条件判定（单根 All/Any + 叶子 not）→ 策略消耗（Once/Cooldown/Count，写 TriggerRuntime）
/// → 动作注册表生成已有 XxxRequest。
/// </summary>
/// <remarks>
/// order 131：严格晚于事件创建（46/125/126/127/129）与 GroundArea/EffectLifecycle（128-130）；
/// 事件"滞后一拍"语义（129 产出的 Request 下一轮才变事件）符合同步确定性。
/// 事件实体由 EventCleanupSystem（132）清理，本系统只读。
/// </remarks>
[SystemRegister(SystemKind.Interval, 131)]
public class TriggerSystem : QuerySystem<TriggerEventMarker>
{
    /// <summary>规则索引：eventTypeId → 规则实体列表（每 tick 重建，规则数小）。</summary>
    private readonly Dictionary<int, List<Entity>> _rulesByType = new();

    /// <summary>通用规则（eventTypeId=0，匹配全部事件）。</summary>
    private readonly List<Entity> _universalRules = new();

    /// <summary>策略 Once 命中的规则实体（迭代外统一删除）。</summary>
    private readonly List<Entity> _expiredRules = new();

    /// <summary>本 tick 的事件实体（收集后统一匹配，避免嵌套查询迭代）。</summary>
    private readonly List<Entity> _events = new();

    /// <summary>绑定的 EntityStore（首次从事件实体缓存，本地验证场景可独立驱动）。</summary>
    private EntityStore? _store;

    protected override void OnUpdate()
    {
        _rulesByType.Clear();
        _universalRules.Clear();
        _expiredRules.Clear();
        _events.Clear();

        // 1. 事件收集（同时缓存 store）。
        Query.ForEachEntity((ref TriggerEventMarker marker, Entity eventEntity) =>
        {
            _store ??= eventEntity.Store;
            _events.Add(eventEntity);
        });

        // 2. 规则索引。
        if (_store != null)
        {
            _store.Query<TriggerSpec>().ForEachEntity((ref TriggerSpec spec, Entity ruleEntity) =>
            {
                if (spec.eventTypeId == 0)
                {
                    _universalRules.Add(ruleEntity);
                }
                else
                {
                    if (!_rulesByType.TryGetValue(spec.eventTypeId, out var list))
                    {
                        list = new List<Entity>();
                        _rulesByType.Add(spec.eventTypeId, list);
                    }

                    list.Add(ruleEntity);
                }
            });
        }

        // 3. 事件扫描与匹配。
        foreach (var eventEntity in _events)
        {
            var marker = eventEntity.GetComponent<TriggerEventMarker>();
            EvaluateRules(eventEntity, marker.eventTypeId, _universalRules);
            if (_rulesByType.TryGetValue(marker.eventTypeId, out var typedRules))
                EvaluateRules(eventEntity, marker.eventTypeId, typedRules);
        }

        // 4. 一次性规则实体统一删除（避免迭代中结构变更）。
        foreach (var ruleEntity in _expiredRules)
        {
            if (!ruleEntity.IsNull)
                ruleEntity.DeleteEntity();
        }
    }

    /// <summary>对一组规则执行匹配：条件判定 → 策略消耗 → 动作执行。</summary>
    private void EvaluateRules(Entity eventEntity, int eventTypeId, List<Entity> rules)
    {
        foreach (var ruleEntity in rules)
        {
            if (!ruleEntity.TryGetComponent<TriggerSpec>(out var spec))
                continue;

            if (!ruleEntity.TryGetComponent<TriggerRuntime>(out var runtime))
            {
                runtime = new TriggerRuntime();
                ruleEntity.AddComponent(runtime);
            }

            if (!CanTrigger(spec.policy, ref runtime))
                continue;

            if (!EvaluateConditions(spec, eventEntity, ruleEntity))
                continue;

            ExecuteActions(spec, eventEntity, ruleEntity);
            ConsumePolicy(spec.policy, ref runtime, ruleEntity);

            ruleEntity.AddComponent(runtime);
        }
    }

    /// <summary>策略门禁：冷却未到/次数已满则不触发。</summary>
    private static bool CanTrigger(TriggerPolicy policy, ref TriggerRuntime runtime)
    {
        if (runtime.cooldownRemain > 0f)
            return false;

        return policy.kind switch
        {
            TriggerPolicyKind.Count => runtime.triggerCount < policy.maxCount,
            _ => true,
        };
    }

    /// <summary>条件判定：combine=All 全真 / Any 任一真；叶子 not 取反；空条件恒真。</summary>
    private static bool EvaluateConditions(TriggerSpec spec, Entity eventEntity, Entity ruleEntity)
    {
        if (spec.conditions == null || spec.conditions.Length == 0)
            return true;

        var ctx = new TriggerContext(eventEntity.Store, eventEntity, ruleEntity);
        var matched = false;
        foreach (var condition in spec.conditions)
        {
            var result = EvaluateCondition(ctx, condition);
            if (spec.combine == ConditionCombine.All && !result)
                return false;
            if (spec.combine == ConditionCombine.Any && result)
                matched = true;
        }

        return spec.combine == ConditionCombine.All || matched;
    }

    /// <summary>单个条件评估：0=恒真，其余走注册表。</summary>
    private static bool EvaluateCondition(TriggerContext ctx, TriggerCondition condition)
    {
        if (condition.conditionId == 0)
            return !condition.not;

        var result = TriggerConditionRegistry.TryGet(condition.conditionId, out var handler)
                     && handler(ctx, condition);
        return condition.not ? !result : result;
    }

    /// <summary>执行动作：注册表生成 Request；禁止 War3 原生调用。</summary>
    private static void ExecuteActions(TriggerSpec spec, Entity eventEntity, Entity ruleEntity)
    {
        if (spec.actions == null || spec.actions.Length == 0)
            return;

        var ctx = new TriggerContext(eventEntity.Store, eventEntity, ruleEntity);
        foreach (var action in spec.actions)
        {
            if (TriggerActionRegistry.TryGet(action.actionId, out var handler))
                handler(ctx, action);
        }
    }

    /// <summary>策略消耗：Cooldown 记冷却、Count 计数、Once 标记删除。</summary>
    private void ConsumePolicy(TriggerPolicy policy, ref TriggerRuntime runtime, Entity ruleEntity)
    {
        switch (policy.kind)
        {
            case TriggerPolicyKind.Once:
                _expiredRules.Add(ruleEntity);
                break;
            case TriggerPolicyKind.Cooldown:
                runtime.cooldownRemain = policy.cooldown;
                break;
            case TriggerPolicyKind.Count:
                runtime.triggerCount++;
                break;
        }
    }
}

/// <summary>
/// 事件实体清理系统。
/// 删除带 TriggerEventMarker 的事件实体（消费窗口 = 1 tick：131 已消费，132 清理），
/// 解决全仓事件实体永不清理的泄漏问题。TriggerSystem 只读事件实体，本系统负责生命周期。
/// </summary>
/// <remarks>
/// order 132：严格晚于 TriggerSystem 与全部事件监听系统；新增事件监听系统必须 order &lt; 132，否则在清理后读不到事件实体。
/// </remarks>
[SystemRegister(SystemKind.Interval, 132)]
public class EventCleanupSystem : QuerySystem<TriggerEventMarker>
{
    private readonly List<Entity> _expired = new();

    protected override void OnUpdate()
    {
        _expired.Clear();
        Query.ForEachEntity((ref TriggerEventMarker marker, Entity eventEntity) =>
        {
            _expired.Add(eventEntity);
        });

        foreach (var entity in _expired)
        {
            if (!entity.IsNull)
                entity.DeleteEntity();
        }
    }
}
using System.Collections.Generic;
using Friflo.Engine.ECS;
using War3Frame.Components;
using War3Frame.Src.Components;

namespace War3Frame.Helpers;

/// <summary>触发器条件工厂（内置条件常量入口）。</summary>
public static class TriggerConditions
{
    /// <summary>伤害事件 finalDamage 大于阈值。</summary>
    public static TriggerCondition DamageGreater(float threshold)
        => new() { conditionId = 1, paramF = new[] { threshold } };

    /// <summary>事件目标等于指定单位。</summary>
    public static TriggerCondition TargetIs(Entity unit)
        => new() { conditionId = 2, paramE = new[] { unit } };

    /// <summary>事件来源等于指定单位。</summary>
    public static TriggerCondition SourceIs(Entity unit)
        => new() { conditionId = 3, paramE = new[] { unit } };

    /// <summary>取反一个条件。</summary>
    public static TriggerCondition Not(TriggerCondition condition)
    {
        condition.not = !condition.not;
        return condition;
    }
}

/// <summary>触发器动作工厂（内置动作常量入口）。</summary>
public static class TriggerActions
{
    /// <summary>追加伤害（Magical/Skill 来源）。</summary>
    public static TriggerAction Damage(float amount)
        => new() { actionId = 1, paramF = new[] { amount } };

    /// <summary>治疗。</summary>
    public static TriggerAction Heal(float amount)
        => new() { actionId = 2, paramF = new[] { amount } };

    /// <summary>施加 Buff（buffId + 属性修改参数）。</summary>
    public static TriggerAction BuffApply(string buffId, int attrTypeId, ModifyType modifyType, float value, float duration)
        => new()
        {
            actionId = 3,
            paramS = new[] { buffId },
            paramF = new[] { (float)attrTypeId, (float)modifyType, value, duration },
        };
}

/// <summary>
/// 触发器规则链式配置器：OnEvent / When / Once / Cooldown / Count / Then → Build。
/// 写 ECS 意图（TriggerSpec 数据），同 EffectChainBuilder 族；类型安全入口 OnEvent&lt;T&gt; 编译期绑定事件组件。
/// </summary>
public class TriggerSpecBuilder
{
    private int _eventTypeId;
    private ConditionCombine _combine = ConditionCombine.All;
    private readonly List<TriggerCondition> _conditions = new();
    private TriggerPolicy _policy = new() { kind = TriggerPolicyKind.Count, maxCount = int.MaxValue };
    private readonly List<TriggerAction> _actions = new();

    /// <summary>绑定事件类型（未登记时自动登记）。</summary>
    public TriggerSpecBuilder OnEvent<T>() where T : struct, IComponent
    {
        var typeId = EventTypeRegistry.Get<T>();
        _eventTypeId = typeId != 0 ? typeId : EventTypeRegistry.Register<T>();
        return this;
    }

    /// <summary>匹配全部事件类型。</summary>
    public TriggerSpecBuilder OnAnyEvent()
    {
        _eventTypeId = 0;
        return this;
    }

    /// <summary>设置条件组合模式（默认 All）。</summary>
    public TriggerSpecBuilder Combine(ConditionCombine combine)
    {
        _combine = combine;
        return this;
    }

    /// <summary>添加条件（可多次；配合 Combine 表达 All/Any 组合）。</summary>
    public TriggerSpecBuilder When(TriggerCondition condition)
    {
        _conditions.Add(condition);
        return this;
    }

    /// <summary>一次性：触发一次后规则实体删除。</summary>
    public TriggerSpecBuilder Once()
    {
        _policy = new TriggerPolicy { kind = TriggerPolicyKind.Once };
        return this;
    }

    /// <summary>冷却：触发后冷却指定秒数。</summary>
    public TriggerSpecBuilder Cooldown(float seconds)
    {
        _policy = new TriggerPolicy { kind = TriggerPolicyKind.Cooldown, cooldown = seconds };
        return this;
    }

    /// <summary>次数：最多触发指定次数。</summary>
    public TriggerSpecBuilder Count(int maxCount)
    {
        _policy = new TriggerPolicy { kind = TriggerPolicyKind.Count, maxCount = maxCount };
        return this;
    }

    /// <summary>添加动作（命中后依次执行）。</summary>
    public TriggerSpecBuilder Then(TriggerAction action)
    {
        _actions.Add(action);
        return this;
    }

    /// <summary>构建 TriggerSpec 数据。</summary>
    public TriggerSpec Build()
    {
        return new TriggerSpec
        {
            eventTypeId = _eventTypeId,
            combine = _combine,
            conditions = _conditions.Count > 0 ? _conditions.ToArray() : null,
            policy = _policy,
            actions = _actions.Count > 0 ? _actions.ToArray() : null,
        };
    }
}
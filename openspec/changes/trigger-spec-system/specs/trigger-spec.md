# Capability: TriggerRule（触发器规则）

## 概述

跨领域规则系统：事件实体 + 规则实体 + 动作注册表。规则 = 匹配事件类型 + 条件（单根组合）+ 策略（一次性/冷却/次数）+ 动作（生成已有 Request）。

## Requirements

| ID | 需求 | 验证 |
|---|---|---|
| TRG-1 | 事件实体创建处挂 `TriggerEventMarker`（eventTypeId），TriggerSystem 单查询发现 | 4 处创建点核对 |
| TRG-2 | 触发器规则为独立实体，挂 `TriggerSpec`（eventTypeId/conditions/policy/actions）+ `TriggerRuntime`（冷却/次数） | 组件定义 |
| TRG-3 | 条件为单根组合：`combine` All/Any + 叶子 `not`；条件经 `TriggerConditionRegistry` 扩展 | 场景条件断言 |
| TRG-4 | 策略：Once（触发后删除规则实体）/ Cooldown（冷却秒）/ Count（次数上限）；状态写 `TriggerRuntime` | 场景策略收敛断言 |
| TRG-5 | 动作经 `TriggerActionRegistry` 生成已有 Request；CastRequest 挂单位主体；禁止调用 War3 原生 API | 代码审查 + 场景 |
| TRG-6 | `TriggerSystem` order 131（晚于事件创建与 GroundArea/EffectLifecycle） | order 常量核对 |
| TRG-7 | `EventCleanupSystem` order 132：删除消费窗口（1 tick）后的事件实体 | 场景事件清理断言 |
| TRG-8 | 事件类型注册表：新事件类型登记后无需改 TriggerSystem | ControlStateChangedEvent 登记 |
| TRG-9 | 确定性：条件/动作禁止非确定性源（Random/DateTime） | 代码审查 |
| TRG-10 | `TriggerSpecBuilder`/`TriggerHelper`：链式类型安全入口（OnEvent<T> 编译期绑定） | 场景使用 |
| TRG-11 | 与 `AbilityBehaviorTrigger`/`TimerTask`/`EffectChainBuilder` 边界：不重叠、不修改 | 代码审查 |
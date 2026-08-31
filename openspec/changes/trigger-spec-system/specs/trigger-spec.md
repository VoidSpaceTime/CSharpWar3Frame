# Capability Spec：TriggerRule（触发器规则）

## 能力描述

触发器规则能力提供跨领域的事件-规则-动作管线：领域系统生成的事实事件（`XxxEvent` 独立实体）可被声明式规则匹配，规则由事件匹配器、条件组合、触发策略与动作组成；命中后动作通过注册表生成已有 `XxxRequest` 交由现有 ResolveSystem 执行。规则不直接调用 War3 原生 API。

## Requirement

| ID | 需求 | 验证方式 |
|---|---|---|
| TR-01 | 事件实体可被标记为可触发（`TriggerEventTag`），标记不改变事件实体语义与清理纪律 | 代码审查 + 场景验证：事件实体不被 TriggerSystem 删除 |
| TR-02 | 规则以 `TriggerSpec` 组件挂独立触发器实体，包含事件类型、条件、策略、动作 | 代码审查：字段完整性 |
| TR-03 | 事件类型通过注册表注册（int 键 + 组件类型映射），内置 Damage/Heal/BuffApplied | 场景验证：三类事件均能匹配 |
| TR-04 | 条件支持 All/Any/Not 三种组合语义，复杂条件经 `TriggerConditionRegistry` 扩展 | 场景验证：多条件组合判定正确 |
| TR-05 | 策略支持 Once/Cooldown/Count，运行态存 `TriggerRuntime`（冷却剩余/已触发次数），达上限规则失效 | 场景验证：Once 只触发一次；Cooldown 窗口内不重复；Count 达上限失效 |
| TR-06 | 同事件多规则按 priority 降序执行 | 代码审查 + 场景：两个规则注册后顺序稳定 |
| TR-07 | 动作经 `TriggerActionRegistry` 生成已有 `XxxRequest`（damage/buff/heal/cast），触发器回调路径零原生调用 | grep 审查：`Trigger/` 目录无 `JassApi`/`DzApi` 引用 |
| TR-08 | 规则提供链式 Builder（`TriggerSpecBuilder`）与快捷注册入口（`TriggerHelper.Register/Unregister`） | 代码审查 + 场景编译通过 |
| TR-09 | TriggerSystem 注册顺序位于事件结算之后、生命周期清理之前 | 代码审查：`SystemRegister(Interval, 128)` |
| TR-10 | 规则按事件类型分组索引，事件到达只遍历同组规则 | 代码审查：索引结构存在 |

## 边界

- 不覆盖技能内部生命周期触发（`AbilityBehaviorTrigger` 域）。
- 不把 Friflo 原生结构变化事件作为触发器输入（AGENTS.md #26）。
- 单位私有规则（owner 字段）不在首期范围。
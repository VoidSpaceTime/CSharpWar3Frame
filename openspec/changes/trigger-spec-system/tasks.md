# 任务：触发器体系 TriggerSpec

> 说明：本清单用于实施跟踪；最终完成状态以 git 提交与代码现状为准（AGENTS.md #1）。

## 1. 组件（War3Frame/Src/Components/Trigger/）

- [ ] `EventTypeRegistry.cs`：typeId ↔ 组件类型静态注册表；内置登记 DamageEvent/HealEvent/BuffAppliedEvent/ControlStateChangedEvent
- [ ] `TriggerEventMarker.cs`：事件标记组件（eventTypeId）
- [ ] `TriggerSpec.cs`：TriggerSpec/TriggerCondition/TriggerAction/TriggerPolicy/TriggerRuntime/ConditionCombine

## 2. 注册表（War3Frame/Src/Helpers/Trigger/）

- [ ] `TriggerConditionRegistry.cs`：conditionId → handler；内置 AlwaysTrue/DamageGreater/TargetIs/SourceIs
- [ ] `TriggerActionRegistry.cs`：actionId → handler；内置 Damage/Heal/BuffApply/Cast（只生成 Request）
- [ ] `TriggerContext.cs`：条件/动作上下文结构

## 3. 系统（War3Frame/Src/Systems/Trigger/）

- [ ] `TriggerSystem.cs`（Interval 131）：规则索引（按 eventTypeId 分组）→ 事件扫描 → 条件判定 → 策略消耗 → 动作执行；结构变更收集后统一处理
- [ ] `EventCleanupSystem.cs`（Interval 132）：删除带 TriggerEventMarker 的事件实体（消费窗口 1 tick）

## 4. 事件创建点挂载（4 处）

- [ ] `AbilityEffectSystems.cs`：DamageEvent 创建处挂 TriggerEventMarker
- [ ] `AbilityEffectSystems.cs`：HealEvent 创建处挂 TriggerEventMarker
- [ ] `AbilityEffectSystems.cs`：BuffAppliedEvent 创建处挂 TriggerEventMarker
- [ ] `ControlStateTransitionSystem.cs`：ControlStateChangedEvent 创建处挂 TriggerEventMarker（含清理补发路径）

## 5. Builder/Helper（War3Frame/Src/Helpers/Trigger/）

- [ ] `TriggerSpecBuilder.cs`：OnEvent<T>()/When()/Once()/Cooldown()/Count()/Then() → Build()
- [ ] `TriggerHelper.cs`：Register(store, 配置) → 创建触发器实体（TriggerSpec + TriggerRuntime）

## 6. 验证

- [ ] `Projects/test/Scripts/Process/TriggerValidationScenario.cs`：伤害阈值规则（Damage 动作）+ BuffApply 规则 + Once/Count 策略收敛 + All/Any/not 条件 + 事件清理
- [ ] `dotnet build War3Frame` + `dotnet build Projects/test` 0 错误
- [ ] 本地 runner 场景 PASS（临时项目，验证后清理）
- [ ] 真实 War3 客户端验证（非阻塞，记录原因）

## 7. 文档

- [ ] proposal.md/design.md/specs/trigger-spec.md 与实现一致
- [ ] summary.md 实施总结
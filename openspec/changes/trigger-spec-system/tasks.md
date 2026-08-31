# 任务清单：触发器体系 TriggerSpec

状态约定：`[ ]` 待办 / `[x]` 完成。任务按实现顺序排列，验证任务内联标注。

## P0：组件与注册表

- [ ] 新增 `War3Frame/Src/Components/Trigger/TriggerSpec.cs`：`TriggerEventTag`(ITag)、`TriggerSpec`、`TriggerCondition`、`TriggerPolicy`、`TriggerAction`、`TriggerRuntime`、`TriggerConditionKind`、`TriggerPolicyKind` 枚举
- [ ] 新增 `War3Frame/Src/Helpers/TriggerEventType.cs`：静态注册表（Register/TryResolve）+ 事件类型↔组件类型映射（内置 Damage/Heal/BuffApplied）
- [ ] 新增 `War3Frame/Src/Helpers/TriggerConditionRegistry.cs`：`TriggerConditionFunc` 委托 + Register/TryResolve + 内置条件（unit.is / attr.threshold / damage.min / odds / 存在性）
- [ ] 新增 `War3Frame/Src/Helpers/TriggerActionRegistry.cs`：`TriggerActionFunc` 委托 + Register/TryResolve + 内置动作（request.damage / request.buff / request.heal / request.cast，全部只生成 Request）
- [ ] 验证：`dotnet build War3Frame/War3Frame.csproj` 0 错误

## P1：系统与事件挂载

- [ ] 新增 `War3Frame/Src/Systems/Trigger/TriggerSystem.cs`：`[SystemRegister(SystemKind.Interval, 128)]`——冷却递减 → 事件快照 → eventTypeId 分组匹配 → 条件判定 → 策略消耗 → 动作执行
- [ ] 触发器实体索引：按 eventTypeId 分组（Register/Unregister 维护），事件到达只遍历同组规则
- [ ] 事件实体挂载 `TriggerEventTag`：`AbilityEffectSystems.cs` 中 `DamageResolveSystem`（DamageEvent 创建处）
- [ ] 事件实体挂载 `TriggerEventTag`：`HealResolveSystem`（HealEvent 创建处）
- [ ] 事件实体挂载 `TriggerEventTag`：`BuffApplyResolveSystem`（BuffAppliedEvent 创建处）
- [ ] 验证：`dotnet build War3Frame/War3Frame.csproj` 0 错误；确认 TriggerSystem order 128 位于结算(125-127)之后、清理(130)之前

## P2：Helper / Builder

- [ ] 新增 `War3Frame/Src/Helpers/TriggerSpecBuilder.cs`：链式 API（OnEvent/When/Once/Cooldown/Count/Then/Build）
- [ ] 新增 `War3Frame/Src/Helpers/TriggerHelper.cs`：`Register(store, spec)` 创建触发器实体、`Unregister(entity)`
- [ ] 验证：`dotnet build War3Frame/War3Frame.csproj` 0 错误

## P3：验证场景

- [ ] 新增 `Projects/test/Scripts/Process/TriggerValidationScenario.cs`：规则 A（Damage + damage.min ≥ 50 → request.buff）、规则 B（Once）、规则 C（Cooldown 1.0）
- [ ] 场景内 `Require` 断言：条件过滤命中/未命中、Once 只触发一次、冷却窗口内不重复触发、动作生成 Request 且无原生调用
- [ ] 验证：`dotnet build Projects/test/` 0 错误

## P4：实施后验证

- [ ] 全仓 grep 确认触发器回调路径无 `JassApi`/`DzApi` 直接调用
- [ ] `TriggerSystem` 注册顺序与事件清理纪律核对（事件实体不由 TriggerSystem 删除）
- [ ] 按 full 级 `R2 Targeted` 复盘：技术准确性 + 性能/生命周期视角
# 实施总结：触发器体系 TriggerSpec

**状态**：已实施
**等级**：full（ECS 规则实体 + 动作注册表 + 事件清理交付）
**实施日期**：2026-08-31

## 实际改动范围

- **新增** `War3Frame/Src/Components/Trigger/TriggerComponents.cs`：`EventTypeRegistry`（typeId ↔ 组件类型，内置登记 Damage/Heal/BuffApplied/ControlStateChanged）、`TriggerEventMarker`（事件标记）、`TriggerSpec`/`TriggerCondition`/`TriggerAction`/`TriggerPolicy`/`TriggerRuntime`/`ConditionCombine`/`TriggerContext`。
- **新增** `War3Frame/Src/Helpers/Trigger/TriggerConditionRegistry.cs`：内置条件 DamageGreater(1)/TargetIs(2)/SourceIs(3)，自定义 Register 从 100 起；事件 target/source 按类型提取。
- **新增** `War3Frame/Src/Helpers/Trigger/TriggerActionRegistry.cs`：内置动作 Damage(1)/Heal(2)/BuffApply(3)，只生成已有 Request；Cast 未内置（扩展示例，design 已注明）。
- **新增** `War3Frame/Src/Helpers/Trigger/TriggerSpecBuilder.cs` + `TriggerHelper.cs`：链式 OnEvent<T>/When/Once/Cooldown/Count/Then + Register 入口。
- **新增** `War3Frame/Src/Systems/Trigger/TriggerSystems.cs`：`TriggerSystem`（Interval 131，规则索引 + 条件判定 + 策略消耗 + 动作执行）+ `EventCleanupSystem`（Interval 132，事件实体消费窗口清理）。
- **小改**（事件创建点挂 TriggerEventMarker，共 5 处）：`AbilityEffectSystems.cs`（DamageEvent/HealEvent/BuffAppliedEvent）、`ControlStateTransitionSystem.cs`（跳变 + 清理补发两处）。
- **新增** `Projects/test/Scripts/Process/TriggerValidationScenario.cs`：本地同步验证（手动创建事件实体模拟结算产出，绕开结算系统的 Game.Store 依赖）。

## 验证结果

- `dotnet build War3Frame`：0 错误；`dotnet build Projects/test`：0 错误。
- 本地同步场景（临时 runner，已清理）`TriggerValidationScenario: PASS`：
  - Phase 1：Count 3 策略收敛（第 4 次不触发），Damage 动作生成追加请求 ✓
  - Phase 2：Once 策略触发后规则实体删除、不再重复，Heal 动作 ✓
  - Phase 3：Any 组合（SourceIs 命中）、All+Not 组合（来源排除）✓
  - Phase 4：事件清理（每帧后事件实体为 0，EventCleanupSystem 生效）✓
  - Phase 5：BuffApply 动作三通道参数（buffId/attrTypeId/modifyType/value/duration）✓

## 设计偏离与后续事项

- **Cast 动作未内置**（design 修订为扩展示例）：动作需 ability 实体经 paramE 传入，首期场景未覆盖；需要时按注册表扩展。
- **EventCleanupSystem 落地改变了全仓事件生命周期**：DamageEvent 等此前永不清除，现在消费窗口 1 tick；若未来有系统需要跨帧监听事件（如时间窗口），扩展 TriggerEventMarker 增加 bornTick（design 已预留）。
- **真实 War3 客户端验证未执行**（本地无句柄环境）：TriggerSystem 在真实结算链（125/126/127 产出事件）下的行为建议后续测试客户端验证，非阻塞。
- 确定性约束（禁止 Random/DateTime 于注册表）已写入代码注释与 design。
- grok-4.6 验证修订全部落实：order 131/132、TriggerEventMarker 替代违规命名、参数三通道、扁平单根条件组合、动作挂主体语义。
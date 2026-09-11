# 任务清单

## 1. 审核

- [ ] 1.1 用户审核并批准 `proposal.md` / `design.md` / `tasks.md` / `spec.md`。
- [ ] 1.2 确认提案等级为 `full`，实施后审查强度为 `R2 Targeted`。
- [ ] 1.3 确认本 change 只接线 `AttrCalculationSystem` 与 `AbilityStatCalculationSystem`，不处理 `SpatialGridSystem` / `AbilitySlotSystem`。

## 2. AttrCalculationSystem 接线

- [ ] 2.1 为 `AttrCalculationSystem` 增加 `[SystemRegister(SystemKind.Interval, 45)]`。
- [ ] 2.2 将 `OnUpdate` 改为"查询内收集 dirty 实体 + 查询后统一 `RemoveTag<AttrDirty>`"模式。
- [ ] 2.3 确认 struct 字段修改通过 `AddComponent` 回写，循环内无结构变更。

## 3. AbilityStatCalculationSystem 接线

- [ ] 3.1 为 `AbilityStatCalculationSystem` 增加 `[SystemRegister(SystemKind.Interval, 30)]`。
- [ ] 3.2 将 `OnUpdate` 改为"查询内收集 dirty 实体 + 查询后统一 `RemoveTag<AbilityStatDirty>`"模式。
- [ ] 3.3 确认 struct 字段修改通过 `AddComponent` 回写，循环内无结构变更。

## 4. 验证

- [ ] 4.1 `dotnet build War3Frame/War3Frame.csproj`。
- [ ] 4.2 `dotnet build Projects/test/test.csproj`。
- [ ] 4.3 静态检查两个系统带 `[SystemRegister]`，循环体内无 `AddTag`/`RemoveTag`/`DeleteEntity`。
- [ ] 4.4 运行时验证（`Projects/test`）：属性贡献加成生效、Buff 过期数值回落、技能数值随等级变化、原生血蓝同步使用最新 `finalValue`。
- [ ] 4.5 确认 `SpatialGridSystem` / `AbilitySlotSystem` 保持未注册（本 change 非目标）。
- [ ] 4.6 总结本 change 的改动范围、验证结果、剩余手测项。

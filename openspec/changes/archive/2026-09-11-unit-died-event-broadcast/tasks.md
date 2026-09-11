# 任务：单位死亡事件广播（UnitDiedEvent）+ 击杀奖励迁移

> change：`unit-died-event-broadcast`（full）。全部任务在提案批准后执行。

## Phase 1：事件与广播

- [x] T1.1 `LevelExperience.cs` 或邻近：新增 `UnitDiedEvent`（unit/source）。→ 落于 `Components/Unit/UnitLifecycle.cs`。
- [x] T1.2 `EventTypeRegistry.RegisterBuiltIn` 登记 `UnitDiedEvent`。
- [x] T1.3 `UnitHelper.KillUnit`：签名升级 `KillUnit(Entity unit, Entity source = default)`；Alive→Death 转移时创建 `UnitDiedEvent` + `TriggerEventMarker`。
- [x] T1.4 `UnitHelper.RemoveUnit` / `CleanupFinalizeEntityDispose`：补充注释明确"移除 ≠ 死亡，不广播 UnitDiedEvent；删除单位请用 RemoveUnit，勿用 KillUnit 当删除手段"。
- [x] 验证：Alive→Death 恰一条事件；已死/无生命周期不创建；RemoveUnit 不创建事件。

## Phase 2：击杀奖励迁移

- [x] T2.1 新增 `Systems/Ability/KillRewardSystem.cs`（或邻近目录，order 126，Interval）：消费 `UnitDiedEvent`，source 非空 + 死者 expReward>0 时创建 `ExperienceGainRequest(target: source)`；不删事件；快照收集模式。→ 落于 `Systems/KillRewardSystem.cs`。
- [x] T2.2 `DamageResolveSystem` 死亡分支：改为 `KillUnit(request.target, request.source)`，移除 `KillRewardHelper.TryDispatch`。
- [x] T2.3 删除 `KillRewardHelper.cs`。
- [x] 验证：击杀配 expReward 野怪 → 恰一条 `UnitDiedEvent` + 一条 `ExperienceGainRequest`；同帧超杀不重复。

## Phase 3：验证迁移

- [x] T3.1 `SkillPointUpgradeScenario` Phase 1：改为真实 `KillUnit` + 驱动 `KillRewardSystem` 后断言请求，移除直接 `TryDispatch`。→ 拆为 4 条独立验证流（A 击杀闭环 / B 加点 / C 超杀 / D 非击杀无奖励）。
- [x] T3.2 Phase 6：同帧超杀断言"事件恰 1 条 + 请求恰 1 条"；本地系统树补 `KillRewardSystem`。
- [x] T3.3 补断言：source 为空（非击杀死亡）事件照发、奖励不派发；无 expReward 死者事件照发、奖励不派发。
- [x] T3.4 场景注册顺序/入口不变（沿用 `SkillPointUpgradeScenario.Initialize`）。

## Phase 4：验证与总结

- [x] T4.1 `dotnet build War3Frame/War3Frame.csproj` 0 error（含删除 helper 后无残留引用）。
- [x] T4.2 `dotnet build Projects/test/test.csproj` 0 error。
- [x] T4.3 场景 PASS（临时无 native runner 驱动真实系统）。
- [x] T4.4 写 `summary.md`（含 R2 覆盖记录）。

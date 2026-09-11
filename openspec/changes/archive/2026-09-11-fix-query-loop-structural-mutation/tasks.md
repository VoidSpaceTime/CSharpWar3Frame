# 任务清单：消除 Query 循环内结构变更

对应提案：`proposal.md`（`fix-query-loop-structural-mutation`，`full`）
状态：`待审核`（未批准前不实施）

## 0. 验证先行（TDD）

- [ ] 新增 `Projects/test/Scripts/Process/QueryStructuralSafetyScenario.cs`
  - [ ] 覆盖 `DurationSystem` 递减（RED：当前抛 `StructuralChangeException`）
  - [ ] 覆盖 `UnitLifecycleTransitionSystem` Death→Corpse
  - [ ] 覆盖 `TimerTaskSystem` 到期
  - [ ] 每修一个系统 → 对应场景 GREEN
- [ ] 注册到 `Projects/test/Program.cs`

## 1. 逐文件审计与修复

> 规则：R1 删冗余写回 / R2 副本改 ref 或移出 / R3 收集后应用 Tag·组件增删 / R4 删除收集后循环外。
> 每文件：标出循环内四类禁止操作 → 修复 → 编译 → 场景验证。

- [ ] `Systems/Time/DurationSystem.cs`（R1）
- [ ] `Systems/Time/TimerTaskSystem.cs`（R1 + R3）
- [ ] `Systems/Unit/UnitLifecycleTransitionSystem.cs`（R1）
- [ ] `Systems/Unit/UnitLifecycleDisposeSystem.cs`（R3/R4，配合 `fix-unit-lifecycle-and-finalize-cleanup`）
- [ ] `Systems/Unit/MoveSystem.cs`
- [ ] `Systems/Unit/ManaSystem.cs` / `Systems/Unit/HealthSystem.cs`（复核，仅 ref 写则无需改）
- [ ] `Systems/Native/UnitNativeSystem.cs`（R2）
- [ ] `Systems/Native/PlayerNativeSystem.cs`（R3）
- [ ] `Systems/Native/UnitCreateNativeSystem.cs`
- [ ] `Systems/Native/UnitMoveNativeSystem.cs`
- [ ] `Systems/Native/UnitRemoveNativeSystem.cs`（复核）
- [ ] `Systems/Native/EffectNativeSystem.cs`
- [ ] `Systems/Native/ItemCreateNativeSystem.cs`（配合 stub 决策）
- [ ] `Systems/Native/War3NativeBootstrap.cs`
- [ ] `Systems/Attribute/AttrCalculationSystem.cs`（已正确，复核）
- [ ] `Systems/Ability/AbilityStatCalculationSystem.cs`（R3）
- [ ] `Systems/Ability/AbilityCooldownSystem.cs`
- [ ] `Systems/Ability/AbilitySlotSystem.cs`
- [ ] `Systems/Ability/AbilityUpgradeWorkflowSystem.cs`
- [ ] `Systems/Ability/CastingSystem.cs`（已正确，复核）
- [ ] `Systems/Ability/AbilityEffectSystems.cs`
- [ ] `Systems/BuffSystem.cs`（部分正确，复核循环内）
- [ ] `Systems/AuraSystem.cs`
- [ ] `Systems/EffectRuntimeSystem.cs`
- [ ] `Systems/ControlStateTransitionSystem.cs`
- [ ] `Systems/Item/ItemSystem.cs`
- [ ] `Systems/Item/ItemUseSystem.cs`
- [ ] `Systems/LevelExperienceSystem.cs`
- [ ] `Systems/Trigger/TriggerSystems.cs`（复核）
- [ ] `Systems/Unit/AttackSimulationSystem.cs`（复核，已收集）

## 2. 静态复核

- [ ] 全仓确认 `ForEachEntity` 回调内无 `AddComponent`/`AddTag`/`RemoveTag`/`RemoveComponent`
- [ ] 确认收集列表按 `entity.Id` 升序应用（确定性）
- [ ] 确认收集列表应用前做 `IsNull` 校验

## 3. 验证

- [ ] `dotnet build War3Frame/War3Frame.csproj` 0 error
- [ ] `dotnet build Projects/test/test.csproj` 0 error
- [ ] 新场景全部 PASS（宿主 runner 逐进程执行）
- [ ] 既有全部本地场景重跑 PASS（回归）
- [ ] `lsp_diagnostics` 对改动文件无新增 error

## 4. 收尾

- [ ] 按 `R2 Targeted` 记录证据与 verdict
- [ ] `summary.md`
- [ ] `proposal.md` 状态更新

## 5. 依赖与协作

- [ ] 与 `fix-unit-lifecycle-and-finalize-cleanup` 协调 `UnitLifecycleDisposeSystem` / `UnitRemoveNativeSystem` 的改动，避免重复或交叉。

# 总结：消除 Query 循环内结构变更

对应提案：`proposal.md`（`fix-query-loop-structural-mutation`，`full`）
状态：`已实施`
日期：2026-09-11

## 1. 实际改动范围

按统一修法（R1 删冗余写回 / R2 副本改 ref 或收集 / R3 收集后应用 / R4 删除收集）改造以下系统，使 Query 回调内不再出现 `AddComponent`/`AddTag`/`RemoveTag`/`RemoveComponent`：

| 文件 | 处理 |
|---|---|
| `Systems/Time/DurationSystem.cs` | 收集到期实体，循环外 `AddTag<DurationExpired>`；删除冗余 `AddComponent(duration)` |
| `Systems/Time/TimerTaskSystem.cs` | 收集到期/打标/移除/owner 状态更新，循环外应用 |
| `Systems/Unit/UnitLifecycleTransitionSystem.cs` | 删除冗余 `AddComponent(state)`（ref 已持久化） |
| `Systems/Unit/UnitLifecycleDisposeSystem.cs` | 收集 Remove 实体，循环外调用清理 helper |
| `Systems/Unit/MoveSystem.cs` | 收集推进决策，循环外执行；（`MoveToTaskSystem` 同理） |
| `Systems/BuffSystem.cs` | `BuffDurationSystem` 收集后打 `BuffExpired` |
| `Systems/AuraSystem.cs` | 收集到期光环，循环外更新；嵌套查询只收集待新增单位，循环外建 buff |
| `Systems/EffectRuntimeSystem.cs` | 收集位置写回，循环外 `AddComponent` |
| `Systems/LevelExperienceSystem.cs` | `Unit/ItemLevelStatRebuildSystem` 收集后重算 |
| `Systems/Ability/AbilitySlotSystem.cs` | 挂载/移除工作流改为收集后处理 |
| `Systems/Ability/AbilityStatCalculationSystem.cs` | 收集后清 `AbilityStatDirty` |
| `Systems/Ability/AbilityEffectSystems.cs` | Projectile / EffectVisual / AreaSearch / LineSearch / Damage / Heal / BuffEffect / BuffApplyResolve / GroundAreaBuff 全部改为收集后处理 |
| `Systems/Item/ItemSystem.cs` | 4 个属性贡献系统改为收集后处理 |
| `Systems/Native/UnitNativeSystem.cs` | Position/快照写回收集后应用 |
| `Systems/Native/UnitMoveNativeSystem.cs` | 收集后移除请求 |
| `Systems/Native/UnitCreateNativeSystem.cs` | 收集后写回 `UnitNative` / 移除请求 |
| `Systems/Native/EffectNativeSystem.cs` | 收集 ECS 写回，原生副作用仍循环内执行 |
| `Systems/Native/PlayerNativeSystem.cs` | 收集玩家实体，循环外同步并清 `PlayerDirty` |

**核实为已安全、未改动**：`ControlStateTransitionSystem`、`AbilityCooldownSystem`、`CastingSystem`（4 系统）、`KillRewardSystem`、`AttackSimulationSystem`、`ItemUseSystem`、`AbilityUpgradeWorkflowSystem`、`GroundAreaCreateSystem`、`GroundAreaReactionSystem`、`EffectLifecycleSystem`、`DamageResolveSystem`、`HealResolveSystem`、`GroundAreaLifetimeSystem`、`ProjectileLifecycleApplySystem`、`AttrCalculationSystem`、`HealthSystem`、`ManaSystem`、`SpatialGridSystem`、`TriggerSystems`、`UnitControlNativeSystem`、`UnitRemoveNativeSystem`（其结构变更本就在循环外，或仅 `DeleteEntity`/`CreateEntity`）。

### 引擎契约（实测基线，Friflo.Engine.ECS 3.6.0）

| 循环内操作 | 结果 |
|---|---|
| `ref` 字段原地写 / `CreateEntity(...)` / `DeleteEntity()` | ✅ 允许 |
| `AddComponent`（新或已存在）/ `AddTag` / `RemoveTag` / `RemoveComponent` | ❌ 抛 `StructuralChangeException` |

## 2. 验证

- `dotnet build War3Frame` + `Projects/test` → 0 error。
- 新增 `Projects/test/Scripts/Process/QueryStructuralSafetyScenario.cs`，**构造匹配实体驱动循环体**，覆盖：
  - `DurationSystem` 递减 + `DurationExpired`；
  - `UnitLifecycleTransitionSystem` Death→Corpse；
  - `TimerTaskSystem` 到期 `TimerExpired`/`BuffExpired` + 移除；
  - `BuffDurationSystem` `BuffExpired`。
- 宿主 runner 逐进程执行：新场景 + 既有 7 个场景全部 `PASS`（无回归）。
- 静态扫描：`Systems/` 下 71 个 `ForEachEntity` 循环，**循环内结构变更 0 处**。

### RED→GREEN 证据

- RED（修改前，真实系统）：`UnitLifecycleTransitionSystem(Death)`、`DurationSystem`、`UnitLifecycleDisposeSystem` 用临时宿主直接抛 `StructuralChangeException`。
- GREEN（修改后）：同场景在 `QueryStructuralSafetyScenario` 中通过。

## 3. R2 Targeted 视角与 verdict

- **技术正确性**：修法遵循"收集后循环外应用"，顺序按既有语义保留；`ref` 写回已具备持久语义，删除冗余 `AddComponent` 不改变行为。verdict：通过。
- **兼容性/回归**：两项目 0 error；既有 7 场景全 PASS。verdict：通过。
- **确定性/结构约束**：收集列表按原遍历序处理；删除仅在循环外。verdict：通过。

## 4. 遗留与后续

- 本变更只修"结构安全"，不改生命周期阶段语义与终态清理完整性——见 `fix-unit-lifecycle-and-finalize-cleanup`（批二）。
- `remove-dead-code-and-unread-fields` 覆盖的占位/死代码未在本变更处理。

# 总结：单位生命周期销毁顺序修正 + 终态清理补全

对应提案：`proposal.md`（`fix-unit-lifecycle-and-finalize-cleanup`，`full`）
状态：`已实施`（方案 A）
日期：2026-09-11

## 1. 实际改动

### 阶段机（方案 A）

- `Components/Unit/UnitLifecycle.cs`：新增 `UnitLifecyclePhase.Disposing`（原生已移除、ECS 待销毁）。
- `Systems/Native/UnitRemoveNativeSystem.cs`：显式 `order 10`；`Death→KillUnit`；`Remove→RemoveUnit` 并置 `Disposing`。改为收集后循环外处理（遵守结构安全）。
  - 句柄配对顺序修正：`NativeEntityIndex.Unregister` → `HandleHelper.HandleRemove` → `JassApi.RemoveUnit`（销毁前注销，符合 `AGENTS.md`）。
- `Systems/Unit/UnitLifecycleTransitionSystem.cs`：显式 `order 20`（`Death→Corpse`、`ClearCorpse→Remove`）。
- `Systems/Unit/UnitLifecycleDisposeSystem.cs`：显式 `order 30`；仅在 `Disposing` 阶段调用终态清理并删除实体。

执行顺序契约：`UnitRemoveNativeSystem(10) < UnitLifecycleTransitionSystem(20) < UnitLifecycleDisposeSystem(30)`，且均晚于默认 `order=1` 的其它 Immediate 系统。

### 终态清理补全 —— `UnitHelper.CleanupFinalizeEntityDispose`

原：`RemoveTag<TimerExpired>` → `RemoveAllAttrs` → `RemoveAllAbilities` → `DeleteEntity`。
现：
1. `RemoveTag<TimerExpired>`
2. `AuraHelper.RemoveAllAuras(entity)` —— 删除光环实体及其 aura buff
3. `DetachAndRecycleItems(entity)` —— 对每件 `ItemOwner` 物品 `Detach(dropToGround:false)` 解绑，再置 `ItemDestroyPendingTag` 交 `ItemCompanionDeferredDeleteSystem`(131) 回收
4. `AbilitySlotHelper.RemoveAllAbilities(entity)` —— 已扩展为覆盖无槽位伴生技能
5. `AttributeHelper.RemoveAllAttrs(entity)`
6. `entity.DeleteEntity()`

- `AbilitySlotHelper.RemoveAllAbilities`：带 `AbilitySlotIndex` 者走 `RemoveAbilityFromSlot`，无槽位者直接 `AbilityHelper.RemoveAbility`，消除悬挂 `AbilityOwner`。
- `UnitHelper.RemoveUnit`：保留为“非死亡直接移除”入口（不广播 `UnitDiedEvent`），其 `Remove` 阶段现由上述顺序正确处理。

## 2. 验证

| 项 | 结果 |
|---|---|
| `dotnet build` War3Frame / test / CSharpWar3FrameConsole / FrameBuild / demo | 0 error |
| 新增 `UnitLifecycleValidationScenario` | PASS |
| 既有 9 个场景回归 | 全部 PASS |

`UnitLifecycleValidationScenario` 覆盖：
- 阶段推进 `Death→Corpse`、`ClearCorpse→Remove`、`Remove→(原生移除)→Disposing→删除`（无 `UnitNative` 时跳过原生分支）；
- 终态清理：单位销毁后物品 `ItemOwner` 已解绑且带 `ItemDestroyPendingTag`（进入回收管线）。

## 3. R2 Targeted verdict

- **顺序正确性**：三系统显式 order + `Disposing` 阶段保证“原生移除先于 ECS 删除；KillUnit 先于 Death→Corpse”。verdict：通过。
- **清理完整性**：光环/物品/无槽伴生技能/属性全部回收；场景断言无悬挂 `ItemOwner`。verdict：通过。
- **兼容性**：全项目 0 error，10 场景全 PASS。verdict：通过。

## 4. 遗留

- 物品**死亡路径不掉落**（按已定决策）；需掉落时应在销毁前显式调用 `ItemHelper.DropToGround`。
- `RebornPending` / `Pooled` 仍为 `待实现`，不在本变更范围。

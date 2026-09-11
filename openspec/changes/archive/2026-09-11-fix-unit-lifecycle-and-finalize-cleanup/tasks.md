# 任务清单：单位生命周期销毁顺序修正 + 终态清理补全

对应提案：`proposal.md`（`fix-unit-lifecycle-and-finalize-cleanup`，`full`）
状态：`已实施`（方案 A；编译 + 场景验证通过）

## 0. 决策点（已定）

- [x] `UnitHelper.RemoveUnit`：**保留并启用**（非死亡直接移除入口）
- [x] 单位销毁时物品策略：**安全解绑 + 回收**（`Detach(dropToGround:false)` 后回收物品实体；死亡路径不掉落）
- [x] 销毁顺序实现方案：**方案 A**（显式 order 10/20/30 + `Disposing` 阶段）

## 1. 阶段与顺序

- [ ] `Components/Unit/UnitLifecycle.cs`：新增 `UnitLifecyclePhase.Disposing`
- [ ] `Systems/Native/UnitRemoveNativeSystem.cs`：显式 `order 10`
  - [ ] `Death→KillUnit`
  - [ ] `Remove→RemoveUnit+HandleRemove→置 Disposing`；无 `UnitNative` 也置 `Disposing`
- [ ] `Systems/Unit/UnitLifecycleTransitionSystem.cs`：显式 `order 20`
  - [ ] `Death→Corpse`、`ClearCorpse→Remove`（保持）
  - [ ] 移除循环内 `AddComponent(state)`（改 ref 写，见 P0-2 变更）
- [ ] `Systems/Unit/UnitLifecycleDisposeSystem.cs`：显式 `order 30`
  - [ ] 仅 `Disposing→CleanupFinalizeEntityDispose`
  - [ ] 收集后循环外执行（见 P0-2 变更）

## 2. 终态清理补全

- [ ] `Helpers/UnitHelper.cs` `CleanupFinalizeEntityDispose`：
  - [ ] 追加 `AuraHelper.RemoveAllAuras(entity)`
  - [ ] 追加物品解绑 / 回收
  - [ ] `RemoveAllAbilities` 覆盖无 slot 的 companion（按 `AbilityOwner` 清理全部）
  - [ ] 保持既有顺序：外围状态 → 子实体 → 本体删除
- [ ] 复核清理路径无悬挂 `ModifyTarget` / `ItemOwner` / `AbilityOwner` / `AuraBuffLink`

## 3. 死 API

- [ ] 按决策点处理 `UnitHelper.RemoveUnit`

## 4. 验证

- [ ] 编译 `War3Frame` + `Projects/test` 0 error
- [ ] 新增生命周期场景（宿主 runner）：
  - [ ] Alive→Death：phase 序列 + KillUnit 消费
  - [ ] 直删 Remove：RemoveUnit 先于删除、Disposing 被消费
  - [ ] 死亡终态：aura/item/companion 全回收、无悬挂关系
- [ ] 既有场景全量回归 PASS

## 5. 收尾

- [ ] `R2 Targeted` 证据与 verdict
- [ ] `summary.md` + 状态更新

## 6. 协作

- [ ] 与 `fix-query-loop-structural-mutation` 共享 `UnitLifecycleDisposeSystem` / `UnitRemoveNativeSystem` 改动，统一在一次或紧邻提交完成。

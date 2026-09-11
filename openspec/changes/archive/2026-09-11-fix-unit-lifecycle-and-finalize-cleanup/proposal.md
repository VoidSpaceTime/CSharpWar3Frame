# 提案：单位生命周期销毁顺序修正 + 终态清理补全

## 元信息

- **状态**：已实施
- **等级**：`full`
- **变更 ID**：`fix-unit-lifecycle-and-finalize-cleanup`
- **日期**：2026-09-11
- **请求来源**：仓库疏漏扫描（ULW 只读审计）
- **默认实施后审查强度**：`R2 Targeted`
- **命中的审查升级触发器**：改核心业务流程 / 跨系统状态协作（生命周期 ↔ 原生 / 属性 / 技能 / 物品 / 光环）
- **完整 `review-work` 授权来源**：无

## 1. 背景 / Why

### 问题 1：销毁顺序错误 → 原生 KillUnit 成死分支、直删路径泄漏句柄

`SystemGenerator.cs:141-142` 的注册顺序为 `OrderBy(Order).ThenBy(ClassName, Ordinal)`，`Order` 缺省为 `1`。三个 `SystemKind.Immediate` 生命周期系统都没有显式 order，按类名排序后实际执行顺序为：

```
UnitLifecycleDisposeSystem  <  UnitLifecycleTransitionSystem  <  UnitRemoveNativeSystem
```

后果：
- **Death 路径**：`UnitLifecycleTransitionSystem` 先把 `Death→Corpse`，随后 `UnitRemoveNativeSystem` 已看不到 `Death` → **`JassApi.KillUnit` 永不执行**（`UnitRemoveNativeSystem.cs:19-26` 成死分支）。原生单位不会真正“死亡”，只是尸体计时后直接 `RemoveUnit`。
- **直删路径**（`UnitHelper.RemoveUnit` 直接把 phase 置 `Remove`）：下一 tick `Dispose` 先删实体，`UnitRemoveNativeSystem` 查不到 → **`RemoveUnit` + `HandleHelper.HandleRemove` 永不执行**（原生单位与句柄泄漏、`NativeEntityIndex` 残留）。
  - 注：`UnitHelper.RemoveUnit` 当前零调用点（死 API），但一旦使用即命中。
- 违反 `AGENTS.md`「同一对象类型唯一销毁执行点」：现在两个系统（Dispose 与 RemoveNative）抢同一 `Remove` 阶段。

### 问题 2：终态清理不完整 → 悬挂关系 + 孤儿实体

`UnitHelper.cs:207 CleanupFinalizeEntityDispose` 仅执行 `RemoveTag<TimerExpired>` → `RemoveAllAttrs` → `RemoveAllAbilities` → `DeleteEntity`，未处理：

- **Aura 实体**（`ModifyTarget` 指向该单位）→ 孤儿实体。
- **物品**（`ItemOwner` 指向该单位）→ 悬挂关系。
- **item companion 技能**（无 `AbilitySlotIndex`，被 `RemoveAllAbilities` 跳过）→ 悬挂 `AbilityOwner`。

物品处置策略（已定）：**安全解绑 + 回收**。
- *解绑* = 对每件物品调用 `ItemLifecycleOperations.Detach(item, dropToGround: false)`：移除 `ItemOwner`/`ItemSlotIndex`、递减单位 `ItemSlotContainer.currentCount`、`ItemCompanionAbilityHelper.UnbindOwner` 解绑伴生技能、`RemoveModifiersFromSource` 撤销属性贡献。
- *回收* = 在解绑基础上删除物品实体（走受控路径：置 `ItemDestroyPendingTag`，由 `ItemCompanionDeferredDeleteSystem`(order 131) 待 companion 的 Cast/Effect/GroundArea 引用释放后删除），避免留下无主物品实体。
- 对比：*掉落* = `Detach(item, dropToGround: true, x, y)`，物品保留为地面物品（`ItemGroundTag`）。本变更**不采用掉落**。

### 关联

`CleanupFinalizeEntityDispose` 在 `Dispose` 的查询循环内被调用，其 `RemoveTag`/`DeleteEntity` 属循环内结构变更，会抛异常——由 `fix-query-loop-structural-mutation` 一并处理；本变更负责**顺序语义与清理完整性**，两者需协调同一批文件。

## 2. 目标 / What

- 为三个生命周期系统设定**显式 order**，保证：`原生移除` 在 `ECS 销毁` 之前、且 `Death` 阶段的 `KillUnit` 能被执行。
- 引入**原生移除完成的确认阶段**，消除“Dispose 抢跑”的竞态，落成唯一销毁执行点。
- 补齐终态清理：光环、物品（安全解绑 + 回收）、物品伴生技能。
- **保留并启用** `UnitHelper.RemoveUnit`，作为“非死亡直接移除”入口（与 `KillUnit` 死亡语义区分）。

## 3. 非目标

- 不改属性/技能/Buff/光环各自的内部模型。
- 不实现 `UnitLifecyclePhase.RebornPending` / `Pooled`（`待实现` 状态另议）。
- 不顺手做死代码清理（见 `remove-dead-code-and-unread-fields`）。

## 4. 影响范围

- `Systems/Unit/UnitLifecycleTransitionSystem.cs`、`UnitLifecycleDisposeSystem.cs`
- `Systems/Native/UnitRemoveNativeSystem.cs`
- `Components/Unit/UnitLifecycle.cs`（如需新增 `Disposing` 阶段）
- `Helpers/UnitHelper.cs`（`CleanupFinalizeEntityDispose`、`RemoveUnit`）
- `Helpers/AbilitySlotHelper.cs`、`Helpers/AuraHelper.cs`、`Helpers/Item*`（补清理入口）
- `Projects/test/`：新增生命周期场景

## 5. 全局影响分析

- **`War3Frame/`**：受影响（核心）。
- **`War3Frame.Generator/`**：不改生成逻辑；仅依赖既有 order 语义。
- **`FrameBuild/`**、**`CSharpWar3Frame/`**：不受影响。
- **`Projects/`**：新增验证场景。

## 6. 设计要点（详见 `design.md`）

1. **销毁顺序修复（方案 A，已定）**：显式 order + `Disposing` 阶段。
   - `UnitRemoveNativeSystem` order 10：`Death→KillUnit`；`Remove→RemoveUnit+HandleRemove`，随后置 `phase=Disposing`（无 `UnitNative` 亦置）。
   - `UnitLifecycleTransitionSystem` order 20：`Death→Corpse`；`ClearCorpse→Remove`。
   - `UnitLifecycleDisposeSystem` order 30：仅 `Disposing→CleanupFinalizeEntityDispose`。
   - 备选方案 B（合并单一 Finalize 系统）见 `design.md` §3.2。
2. **终态清理补全**：`CleanupFinalizeEntityDispose` 增加 `RemoveAllAuras(entity)` + 物品 `Detach` + 回收 + 覆盖无 slot 的伴生技能。
3. **`RemoveUnit`（已定）**：保留并启用，作为“非死亡直接移除”入口。

## 7. 风险与回滚

- **风险**：阶段机改动影响死亡/移除全流程；`Disposing` 阶段若未被 Dispose 消费会卡住（需覆盖测试）。
- **回滚**：还原 order 与阶段处理即可；无持久化影响。

## 8. 验证计划

1. 编译 0 error。
2. 新增场景（宿主 runner 逐进程执行）：
   - `Alive→Death`：断言 `KillUnit` 分支被消费（可用 `UnitNative` 桩验证，或在 ECS 侧断言 phase 依次 `Death→Corpse`）。
   - `Remove` 直删：断言 `RemoveUnit`+`HandleRemove` 先于实体删除执行（顺序探针）。
   - 死亡终态：断言 aura/item/companion 全部回收、无悬挂关系。
3. 既有场景全量回归。
4. `R2 Targeted` 记录证据与 verdict；`summary.md`。

## 9. 拆分任务

见 `tasks.md`。

## 10. 工件

- `proposal.md`、`design.md`、`tasks.md`
- `specs/unit-lifecycle-destroy/spec.md`

## 11. 相关文档

- `AGENTS.md`（原生句柄引用配对 / 生命周期清理职责）
- `War3Frame/Src/Systems/Unit/UnitLifecycleTransitionSystem.cs`、`Systems/Native/UnitRemoveNativeSystem.cs`
- `openspec/changes/archive/2026-08-13-clarify-lifecycle-cleanup-responsibilities/`

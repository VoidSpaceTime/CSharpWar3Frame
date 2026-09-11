## Context

当前仓库已经拥有生命周期数据模型，但缺少生命周期架构所有权的一致收敛。

- `UnitLifeState` 与 `UnitLifecyclePhase` 定义了持久生命周期状态。
- `UnitHelper` 既写 lifecycle phase，又创建 timer，又触发 immediate flush，甚至可能直接删除实体。
- `TimerTaskSystem` 既推进计时，又参与尸体阶段推进，并可能影响后续 lifecycle flow。
- `UnitNativeRemoveSystem` 既执行 native death/remove，又推进 lifecycle state。
- `CorpseCleanupSystem` 仍然通过 `CorpseExpired` 参与 terminal cleanup。
- `UnitTerminalCleanupHelper` 已经抽出终态收尾步骤，但其入口仍然分散。

这导致项目虽然已有持久状态容器，却仍处于 mixed-ownership lifecycle model：phase、timer、tag、dirty flag 与 helper 都在参与定义主路径。

A 方案把这条链严格收敛成以下结构：

1. `UnitLifeState`：唯一生命周期真相层
2. `UnitLifecycleTransitionSystem`：唯一 phase progression owner
3. `UnitLifecycleNativeEffectSystem`：唯一 lifecycle-native side-effect owner
4. `UnitLifecycleDisposeSystem`：唯一 terminal ECS cleanup owner

## Goals / Non-Goals

**Goals:**
- 让 `UnitLifeState.lifePhase` 成为 unit lifecycle 的唯一持久真相源。
- 让所有 primary lifecycle phase progression 只由 `UnitLifecycleTransitionSystem` 拥有。
- 让 native kill/remove 等生命周期副作用只由 `UnitLifecycleNativeEffectSystem` 拥有。
- 让属性清理、技能清理、tag 清理与 `DeleteEntity()` 只由 `UnitLifecycleDisposeSystem` 拥有。
- 让 `TimerTaskSystem` 只推进尸体阶段到 `ClearCorpse`，不拥有终态删除语义。
- 让 `UnitHelper` 缩减为写入意图与 flush immediate 的薄入口。
- 为 future reborn/pooling/cleanup-policy 扩展保留清晰架构面。

**Non-Goals:**
- 本提案不直接实现 reborn / pooling 细节。
- 本提案不直接重构 `Health/Mana` 的轮询 compare 同步模型。
- 本提案不要求一次性删除所有 transient tag，仅要求它们不再充当 primary lifecycle truth。
- 本提案不进入运行时代码实现。
- 本提案不保留旧的 mixed-ownership lifecycle model 作为长期结构。

## Decisions

### 1. `UnitLifeState` is the sole lifecycle truth
**Decision:** `UnitLifeState.lifePhase` MUST 成为 unit death、corpse retention、clear-corpse progression 与 terminal removal 的唯一持久真相源。

**Rationale:** 仓库已经有持久生命周期组件，缺少的是架构层对 competing lifecycle owners 的收敛。

### 2. `UnitLifecycleTransitionSystem` owns phase progression
**Decision:** 所有主路径上的 lifecycle transitions MUST 集中在 `UnitLifecycleTransitionSystem` 中。

至少包括：
- `Alive -> Death`
- `Death -> Corpse`
- `Corpse -> ClearCorpse`
- `ClearCorpse -> Remove`

**Rationale:** 当前缺陷不是缺 phase，而是 transition authority 分散。

### 3. `UnitLifecycleNativeEffectSystem` owns lifecycle-native side effects
**Decision:** `JassApi.KillUnit(...)`、`JassApi.RemoveUnit(...)`、`HandleHelper.HandleRemove(...)` 等生命周期相关 native side effects MUST 由 `UnitLifecycleNativeEffectSystem` 作为 lifecycle phase 的结果统一执行。

**Rationale:** native effects 是 lifecycle state 的 consequence，而不是 lifecycle truth。

### 4. `UnitLifecycleDisposeSystem` owns terminal ECS cleanup
**Decision:** 属性清理、技能清理、transient tag cleanup 与 entity disposal MUST 只由 `UnitLifecycleDisposeSystem` 拥有。

**Rationale:** terminal cleanup 目前可从多个入口到达，必须收敛到单一 owner 以保证可验证性与幂等性。

### 5. `TimerTaskSystem` only advances corpse retention toward `ClearCorpse`
**Decision:** `TimerTaskSystem` 对 unit corpse-retention 的职责仅限于时间推进与推进 lifecycle toward `ClearCorpse`。它 MUST NOT 拥有 native remove 或 ECS terminal disposal 的语义。

**Rationale:** 时间推进与业务/副作用收尾必须分离。

### 6. `UnitHelper` only sets lifecycle intent and flushes
**Decision:** `UnitHelper.KillUnit(...)` 与 `UnitHelper.RemoveUnit(...)` 成为薄入口：只写入 lifecycle phase intent，并在需要时调用 `Game.FlushImmediateSystems()`。它们 SHALL NOT 保留 patchwork cleanup orchestration 责任。

**Rationale:** helper 应该是 API convenience layer，而不是生命周期架构 owner。

### 7. Legacy lifecycle path leaves the primary architecture
**Decision:** 下列机制 SHALL NOT 保留为 primary lifecycle owners：
- `CorpseCleanupSystem`
- `CorpseExpired`
- `Death` dirty flag
- `Remove` dirty flag

迁移期 MAY 存在 compatibility shim，但必须在 tasks 中有明确退出条件。

**Rationale:** 新架构不能在命名上统一、在实现上继续 patchwork。

## Risks / Trade-offs

- [风险] 新旧生命周期路径在迁移期重叠，可能造成重复 native side effects 或重复 entity disposal。  
  [缓解] 明确 canonical owner 顺序，先建立新 owner，再移除旧 owner，并用场景验证控制双路径重叠时间。

- [风险] 现有 demo/test 可能依赖旧的执行时序。  
  [缓解] 在实现前定义场景级验证：death flow、corpse retention、clear-corpse、direct remove、terminal cleanup 顺序。

- [风险] compatibility shim 长期残留。  
  [缓解] 在 tasks 中列出 shim 退出条件与清理步骤。

- [风险] direct remove 与 corpse cleanup 重新被便利实现耦合。  
  [缓解] 语义继续分离，但共用单一 dispose owner。

## Migration Plan

1. 审核通过本次 OpenSpec 架构提案。
2. 先定义基于场景的 red-first 验证，覆盖 death、corpse retention、clear-corpse、direct remove 与 terminal cleanup。
3. 引入 `UnitLifecycleTransitionSystem`，建立唯一 phase progression owner。
4. 引入 `UnitLifecycleNativeEffectSystem`，让 native effects 跟随 lifecycle phase。
5. 引入 `UnitLifecycleDisposeSystem`，让 ECS terminal cleanup 只有一个 owner。
6. 将 `TimerTaskSystem` 收缩为只推进到 `ClearCorpse`。
7. 将 `UnitHelper` 收缩为只写 phase intent 与 flush。
8. 在验证通过后，将 `CorpseCleanupSystem` / `CorpseExpired` / `Death/Remove` dirty flags 退出 primary lifecycle path。
9. 对 `Projects/demo` 与 `Projects/test` 做运行时场景验证。

## Open Questions

- `CorpseExpired` 在迁移期是否保留为内部兼容信号，还是直接完全转为 phase-driven corpse expiry。
- `Death/Remove` dirty flags 是否完全删除，还是仅保留在主路径外作为兼容桥接。
- `Death -> Corpse` 的推进是否需要与 native effect completion 通过显式 transient signal 协调，还是可以在同一 immediate flush 内完成。

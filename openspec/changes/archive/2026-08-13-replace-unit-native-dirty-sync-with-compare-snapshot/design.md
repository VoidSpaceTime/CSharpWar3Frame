## Context

当前仓库中的 native sync 已经自然分裂成两类：

- `Health/Mana`：ECS 属性为真相，`UnitNativeSystem` 将数值写回原生
- `Position`：native 为观测源，`UnitNativeSystem` 从原生读回 ECS

但 `Health/Mana` 仍然依赖 `UnitNativeDirtyFlags`：

- `HealthSystem` / `ManaSystem` / `AbilityCostHelper` 在属性写入后调用 `UnitNativeDirtyHelper.Mark(...)`
- `UnitNativeSystem` 再消费 dirty flag，调用 `SetUnitState(...)`

这意味着连续数值同步仍然依赖“每个写点都记得打脏”的纪律。与之相对，A 方案生命周期架构已经把动作语义从 dirty flag 中剥离出去：

- `UnitLifeState` 负责 truth
- `UnitLifecycleTransitionSystem` 负责 phase progression
- `UnitNativeRemoveSystem` 负责 lifecycle-native side effects
- `UnitLifecycleDisposeSystem` 负责 terminal ECS cleanup

在这个背景下，`UnitNativeDirtyFlags` 已经不再适合作为运行时统一同步概念。更一致的设计是：

- `Health/Mana` 通过 compare-sync + snapshot baseline 统一同步
- `Position` 保留为 native-observe path
- `Death/Remove` 继续由生命周期系统拥有

## Goals / Non-Goals

**Goals:**
- 让 `UnitNativeDirtyFlags` 退出运行时主架构。
- 让 `Health/Mana` 成为 compare-sync 字段。
- 引入最小化 snapshot 组件，仅存储 compare baseline 所需字段。
- 保持 `Position` 作为独立 native-observe path。
- 保证 `Death/Remove` 不会通过 sync flags 或 snapshot 重新承载动作语义。
- 明确 cross-project impact 边界。

**Non-Goals:**
- 本提案不重构 A 方案生命周期架构本身。
- 本提案不将 `Position` 折叠为 compare-sync 字段。
- 本提案不把 snapshot 演化为通用事件总线或生命周期状态容器。
- 本提案不进入运行时代码实现。

## Decisions

### 1. `UnitNativeDirtyFlags` leaves the runtime architecture
**Decision:** `UnitNativeDirtyFlags` SHALL NOT 保留为 runtime architecture 的主同步机制。

**Rationale:** 在生命周期动作已经转移到 A 方案后，dirty flags 只剩连续属性同步用途，继续保留会把动作语义与连续字段语义混在一个过时抽象中。

### 2. `Health/Mana` use compare-sync with snapshot baseline
**Decision:** `Health/Mana` MUST 通过 `UnitNativeSystem` 中的 compare-sync 进行同步：比较当前 ECS 属性值与上次已同步的 snapshot baseline，只有差异存在时才写回原生。

**Rationale:** compare-sync 可以把同步责任收回统一的 native sync layer，避免每个写点都手动打脏。

### 3. Snapshot state is minimal and sync-local
**Decision:** snapshot 组件 MUST 只存储 compare-sync 所需的最小 baseline，例如 `Health/Mana` 的 current/max 与初始化状态；它 SHALL NOT 成为生命周期 truth，也 SHALL NOT 承担动作语义。

**Rationale:** snapshot 的目的是减少无意义原生调用，不是引入第二套业务真相。

### 4. `Position` remains a separate native-observe path
**Decision:** `Position` SHALL 保持在独立的 native-observe path 中，不折叠到 compare-sync ownership 内。

**Rationale:** `Position` 的当前方向是 native -> ECS 观测同步，不同于 `Health/Mana` 的 ECS -> native 下发同步。

### 5. `Death/Remove` remain lifecycle-driven
**Decision:** `Death/Remove` SHALL 继续由 A 方案 lifecycle architecture 拥有，而 SHALL NOT 通过 dirty flags 或 snapshot 回流到同步字段模型。

**Rationale:** 动作语义与连续字段同步属于不同责任域，重新混合会破坏已批准的 A 方案边界。

## Risks / Trade-offs

- [风险] compare-sync 与 dirty-sync 迁移期重叠，可能出现重复写原生。  
  [缓解] 先引入 snapshot，再移除 dirty 写点，最后删除类型定义。

- [风险] snapshot baseline 定义不清会导致浮点抖动或重复同步。  
  [缓解] 在设计与测试里明确初始化策略、比较语义与容差策略。

- [风险] 开发者误把 snapshot 用成生命周期或事件信号。  
  [缓解] 在 spec 中明确 snapshot 只是 sync-local baseline。

- [风险] `Position` 被错误并入 compare-sync，导致 authority model 混乱。  
  [缓解] 在设计中明确 `Position` 为独立 native-observe path。

## Migration Plan

1. 审核通过本次 OpenSpec 提案。
2. 先定义 red-first 测试，覆盖 `Health/Mana` compare-sync、snapshot 初始化、`Position` observe path 与生命周期非回归场景。
3. 引入最小 snapshot 组件与 compare helpers。
4. 将 `Health` 从 dirty-sync 迁移到 compare-sync。
5. 将 `Mana` 从 dirty-sync 迁移到 compare-sync。
6. 保持或隔离 `Position` 观察同步路径。
7. 移除 `HealthSystem` / `ManaSystem` / `AbilityCostHelper` 等写点中的 dirty 标记。
8. 在验证通过后，删除 `UnitNativeDirtyFlags` / `UnitNativeDirtyHelper` / `UnitNativeDirty` 类型定义与剩余依赖。
9. 重新验证 `Projects/*` 的运行时行为。

## Open Questions

- `Position` 是否未来应进一步拆成专门的 native observe system，而不是继续与 `Health/Mana` 共享同一个 `UnitNativeSystem`。
- snapshot 组件是否只保留 `Health/Mana` 的 baseline，还是未来扩展为更多连续同步字段的最小集合。
- float compare 的容差策略如何定义，才能避免 regen 场景下的同步抖动。

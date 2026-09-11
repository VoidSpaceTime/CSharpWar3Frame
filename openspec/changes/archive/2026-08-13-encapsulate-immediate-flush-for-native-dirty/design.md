## Context

当前仓库已经把一部分 native 生命周期操作放入 `ImmediateRoot`，但业务代码里仍然直接调用 `Game.ImmediateRoot.Update(default(UpdateTick))`。这会让 `UpdateTick` 和调度细节泄漏到 helper 层，也不利于后续统一管理 immediate 刷新行为。与此同时，`UnitNativeDirtyHelper` 已经是原生脏位的集中入口，适合作为“普通标脏”和“标脏后立即落地”之间的语义分界点。

## Goals / Non-Goals

**Goals:**
- 封装 immediate flush，避免业务代码直接调用 `ImmediateRoot.Update(...)`。
- 为立即型 native dirty 提供统一入口。
- 保持普通 interval dirty 标记语义不变。

**Non-Goals:**
- 不把所有 `Mark(...)` 都改成立即刷新。
- 不改变现有 interval/native system 的职责边界。
- 不重构创建、死亡或属性同步逻辑本身。

## Decisions

### 1. 在 `Game` 上新增 `FlushImmediateSystems()`
**Decision:** 统一由 `Game.FlushImmediateSystems()` 封装 `ImmediateRoot.Update(default)`。

**Rationale:** 业务层无需再理解 `UpdateTick`，同时 future-proof，后续如果 immediate flush 需要额外逻辑，只需改一处。

### 2. 在 `UnitNativeDirtyHelper` 上新增 `MarkImmediate(...)`
**Decision:** 新增立即版 helper，内部按“先 Mark，再 FlushImmediateSystems”执行。

**Rationale:** 让立即型生命周期逻辑通过统一入口表达语义，而不是在每个调用点手写两行。

### 3. 普通 `Mark(...)` 语义保持不变
**Decision:** `Mark(...)` 继续只负责写 dirty flags，不触发 immediate update。

**Rationale:** 避免把 interval 同步与 immediate 生命周期事件重新耦合在一起。

## Risks / Trade-offs

- [风险] 调用方误把普通同步也切到 `MarkImmediate(...)` → [缓解] 保持命名明确，仅在立即型生命周期路径使用。
- [风险] immediate flush 行为集中后被误以为总是安全可重入 → [缓解] 将封装范围限定为现有简单调用，不引入额外调度语义。

## Migration Plan

1. 在 `Game` 中新增 immediate flush 封装。
2. 在 `UnitNativeDirtyHelper` 中新增立即版标脏入口。
3. 将现有裸调用替换为封装调用。

## Open Questions

- 是否还需要为创建请求链路也统一使用同类 immediate 封装；本次先只覆盖 native dirty 与现有业务层调用点。

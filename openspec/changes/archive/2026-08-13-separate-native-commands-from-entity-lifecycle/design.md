## Context

当前 War3Frame 的单位死亡流程已经包含：

- entity 层的 `UnitState.lifePhase`
- native 命令层的 `UnitNativeDirtyFlags.Death/Remove/Reborn`
- immediate system 处理原生命令
- timed system 处理尸体保留与延迟清理

但这套实现还没有把职责边界彻底写死，导致 `Death` 脏位容易被理解成“死亡状态的一部分”，而不是“立即执行一次 native death 动作的命令”。这会给未来单位池、复活、尸体保留策略扩展带来歧义。

## Goals / Non-Goals

**Goals:**
- 明确生命周期状态只由 entity 组件管理。
- 明确 native dirty flags 只表示立即型原生命令。
- 让死亡、删除、复活都变成“状态 + 命令”协作，而不是由 dirty flags 承担长期含义。
- 为单位池保留“死了但不删 native”的扩展空间。

**Non-Goals:**
- 不在本次提案中直接落地完整单位池。
- 不引入基于 callback 的原生命令分发。
- 不改变现有 AOT 友好约束。

## Decisions

### 1. UnitLifecycle is the sole source of truth for long-lived unit state
**Decision:** `UnitState.lifePhase` 及相关 entity 生命周期组件，是单位长期状态的唯一真相源。

**Rationale:** 长期状态需要可持续读取、可延迟推进、可与 timer 协作，适合放在组件中，而不是一次性命令脏位中。

### 2. UnitNativeDirtyFlags represent native commands only
**Decision:** `UnitNativeDirtyFlags` 只表示立即型 native 命令，例如：

- `Death`：立刻让 native 进入死亡动作/死亡状态
- `Remove`：立刻从 native 世界移除
- `Reborn`：立刻恢复 native 生存状态

它们不再承担 entity 生命周期状态语义。

**Rationale:** 这能让 dirty flags 退回到“native command bus”角色，避免长期语义混杂。

### 3. Death flow is modeled as lifecycle progression plus native command consumption
**Decision:** 单位死亡流程应被表达为：

1. entity 生命周期推进到 `Dying`
2. 发出 `Death` native command
3. immediate system 消费 `Death`，执行 native kill
4. entity 生命周期推进到 `Corpse`
5. timer 到期后再由生命周期逻辑决定删除、回池或待复活

**Rationale:** 这使“死亡”和“删除 native”自然解耦，为对象池和复活留出空间。

### 4. Pooling compatibility is achieved by varying native command policy, not lifecycle meaning
**Decision:** 后续若引入单位池，改变的是 corpse cleanup 之后的 native 处理策略，而不是改变生命周期模型本身。

**Rationale:** 同一套生命周期状态机可以支持“销毁路线”和“回池路线”，差别只在 native command policy。

## Risks / Trade-offs

- [风险] 初期代码量略增，因为状态推进和命令执行要分层 → [缓解] 长远可维护性和池化兼容性明显提升。
- [风险] 需要重新检查 Death/Remove/Reborn 的系统边界 → [缓解] 本提案明确给出职责规则，便于后续逐步对齐。

## Migration Plan

1. 明确 `UnitLifecycle` 与 `UnitNativeDirtyFlags` 的职责文档。
2. 对齐 `KillUnit`、`RemoveUnit`、`Reborn` 等入口逻辑，使其先推进状态，再发 native 命令。
3. 对齐 immediate native systems，只消费命令、不定义长期状态。
4. 对齐 corpse cleanup，使其根据生命周期阶段和策略决定 delete 或 pool。

## Open Questions

- 是否需要将 `Remove` 也从 tag 改为统一的命令组件/状态机入口；本次先不强制统一到单一实现形式。

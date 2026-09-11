## Context

当前仓库中与单位死亡和最终清理相关的链路，已经形成了一个隐含的三段式流程：

- `UnitHelper.KillUnit(...)` 触发立即型 `Death` native dirty，并挂载 `TimerTask(kind = CorpseCleanup)`
- `UnitNativeRemoveSystem` 消费 `Death` 命令，执行 `JassApi.KillUnit(...)`，并将 `UnitState.lifePhase` 推进为 `Corpse`
- `TimerTaskSystem` 在 `CorpseCleanup` 到期时打上 `CorpseExpired`
- `CorpseCleanupSystem` 消费 `CorpseExpired`，执行最终 native remove、属性/技能清理与 entity 删除

这说明当前实现已经自然地区分了“立即死亡动作”“尸体保留期”“最终销毁”三件事，但职责边界还没有在规格层写死，因此仍存在以下风险：

- `UnitNativeSystem` 容易被继续扩张为不仅做 native 同步，还承担更多 entity 生命周期职责
- `Remove` 与 `CorpseCleanup` 都可能走到“删除 native + 删除 entity”，但它们属于不同语义阶段，当前尚未被规格明确区分
- `CorpseExpired` 是一个瞬时到期信号，但若未来被直接并入 `UnitLifecyclePhase`，会让长期状态与瞬时事件语义混合

## Goals / Non-Goals

**Goals:**
- 定义生命周期阶段、瞬时到期信号、立即型 native 动作、最终清理之间的边界。
- 明确 `Death`、`CorpseCleanup`、`Remove` 的职责与语义差异。
- 明确 `UnitNativeSystem` 的目标边界应偏向 native 同步，而不是生命周期清理拥有者。
- 保留 `CorpseExpired` 作为瞬时到期信号，避免长期状态枚举与瞬时触发语义混合。
- 为后续复活、对象池、尸体交互等扩展保留清晰架构面。

**Non-Goals:**
- 不在本次提案中直接修改 `UnitNativeSystem`、`UnitNativeRemoveSystem`、`CorpseCleanupSystem` 或 `TimerTaskSystem` 的实现。
- 不强制本轮将所有删除路径统一到一个具体代码结构中。
- 不在本次提案中决定所有 API 命名、系统命名或 helper 重构细节。
- 不把所有生命周期概念折叠成单一枚举状态机。

## Decisions

### 1. Native command systems do not own lifecycle cleanup completion
**Decision:** 消费 `Death` / `Remove` 等立即型 native 命令的系统，只负责执行相应的 native 副作用与必要的即时状态推进，不拥有完整生命周期清理完成语义。

**Rationale:** 立即型 native 命令属于“此刻要对 War3 原生世界做什么”；而生命周期清理完成属于“这个实体在架构上是否已经完成从尸体阶段进入最终销毁”的问题。两者时间尺度与责任范围不同。

**Alternatives considered:**
- 让 native 命令系统直接拥有完整清理责任：实现上看似简单，但会把即时 native 副作用与长期 cleanup policy 重新耦合。

### 2. Death, CorpseCleanup, and Remove are distinct semantic stages
**Decision:**
- `Death`：表示单位进入死亡流并执行立即型 native death 动作
- `CorpseCleanup`：表示尸体保留期结束后的 cleanup policy 阶段
- `Remove`：表示终态删除动作，语义上是 terminal removal，而不是 death 的别名

**Rationale:** 当前代码虽然在不同系统里实现了这三件事，但如果规格不明确，未来实现很容易把它们重新混成“只要会删掉 native 就算同一回事”。

### 3. CorpseExpired remains a transient expiration signal
**Decision:** `CorpseExpired` 继续作为瞬时到期信号存在，不直接并入 `UnitLifecyclePhase`。

**Rationale:** `UnitLifecyclePhase` 表达的是稳定生命周期阶段，`CorpseExpired` 表达的是“尸体保留时间已到”的瞬时触发条件。把两者折叠，会让 phase 同时承担长期状态与瞬时事件语义。

**Alternatives considered:**
- 将 `CorpseExpired` 直接改成新的 lifecycle phase：会让 phase 语义膨胀，并丢失“到期事件”这一瞬时概念。
- 只保留 `TimerExpired + TimerTaskKind`：长期可以考虑，但本次先不强制替换当前显式信号模式。

### 4. Cleanup policy owns post-expiry final disposal
**Decision:** 尸体到期后的最终销毁由 cleanup policy / cleanup system 拥有，包括 native remove、属性/技能清理与 entity disposal progression，而不是由 death/native command 系统隐式完成。

**Rationale:** 这与当前 `CorpseCleanupSystem` 的角色一致，也更容易支持后续“尸体到期后删除 native”与“尸体到期后回池”这类策略变化。

### 5. UnitNativeSystem should converge toward native synchronization responsibilities
**Decision:** 对 `UnitNativeSystem` 的未来约束，应是收敛为 native synchronization 相关职责，而不是继续扩展生命周期清理责任。

**Rationale:** 当前仓库已经通过其他 OpenSpec 变更确立了“长期状态在组件、瞬时事件在 tag/native command、时间推进与业务副作用分离”的方向；`UnitNativeSystem` 应延续这一方向。

## Risks / Trade-offs

- [风险] 规格只定义边界，不直接规定代码结构，短期内仍可能保留部分重复 cleanup 实现 → [缓解] 在后续实现提案中把 Remove 路径与 CorpseCleanup 路径分别对齐。
- [风险] `CorpseExpired` 继续保留为独立 tag，会让标签数量看起来增加 → [缓解] 用清晰语义换取阶段/事件分离；未来若要收敛，可在不破坏语义的前提下再讨论 `TimerExpired + kind` 组合。
- [风险] 若未来引入复活或对象池，阶段数量继续增长 → [缓解] 先固定“阶段 vs 事件”的边界，再讨论具体扩展字段与阶段数量。

## Migration Plan

1. 先审核并确认本次提案中的职责边界定义。
2. 若审核通过，再创建或扩展后续实现级提案，明确哪些系统只保留 native 同步、哪些系统负责 cleanup policy。
3. 实施前补充验证方案，覆盖 death path、corpse expiry path、cleanup path 与 direct remove path。
4. 审核通过前，不修改任何运行时代码。

## Open Questions

- `Remove` 路径未来是否需要抽象成与 `CorpseCleanup` 共享的 terminal disposal policy；本次先只定义语义边界，不锁死实现结构。
- `UnitNativeSystem` 中的位置回写是否应视为 native synchronization 的一部分，还是应拆出独立 snapshot system；本次先只约束方向，不锁死实现手法。

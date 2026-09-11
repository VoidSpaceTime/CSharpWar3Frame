## Context

当前代码已经证明仓库里存在一个正确但尚未收口的方向：

- ECS 发布移动意图
- native 执行实际移动命令
- 到达范围内后再执行下一步动作（例如施法）

`CastingSystem` 中的 `MoveToCastSystem` 已经是这个方向的缩影，但它仍然把移动 ownership 部分嵌在施法流程里，导致：

- 移动语义被误解为施法前置逻辑
- 玩家覆盖命令、Stop/Hold、中断语义没有上升为通用 move concept
- 预设任务、AI、交互等其它调用方难以复用同一套控制框架

因此，最优结构不是继续扩展 `MoveToCastSystem`，而是正式引入独立 move command architecture：

1. command publishing
2. native execution
3. execution monitoring
4. outcome emission
5. continuation handling by higher-level callers

## Goals / Non-Goals

**Goals:**
- 让 move 成为独立的 command/execution/monitoring subsystem。
- 让施法、任务、AI 等只成为 move 的调用方。
- 定义 `MoveCommand`、`MoveExecutionState`、`MoveOutcome`、`MoveContinuation` 的职责。
- 定义玩家覆盖命令、Stop、Hold、控制效果中断等语义为 move 层的一等概念。
- 明确 ECS 只负责发布和监控，native 负责实际发 Warcraft 命令与被动观测。

**Non-Goals:**
- 本提案不实现 pathfinding。
- 本提案不让 ECS 逐帧手动推进单位真实位置。
- 本提案不要求施法系统失去“移动后施法”能力，而是要求它改为使用独立 move subsystem。
- 本提案不在提案阶段实施运行时代码修改。

## Decisions

### 1. Move is an independent subsystem, not a casting submodule
**Decision:** 移动系统 MUST 被建模为独立 subsystem；施法、任务、AI、交互 SHALL 只是调用它的上层流程。

**Rationale:** move 的职责是命令发布、执行监控和结果归类，不是“为了施法服务的位移步骤”。

### 2. ECS publishes and monitors; native executes
**Decision:** ECS MUST 负责发布 `MoveCommand`、维护 execution state、判定 arrival/outcome；native 层 MUST 负责实际发 Warcraft move/stop/hold/target orders 并暴露可观测结果。

**Rationale:** 这与当前 repo 中 entity truth / native execution separation 的整体方向一致。

### 3. Arrival is an ECS-level semantic based on distance thresholds
**Decision:** “到达” MUST 通过 ECS 持有的目标点与 arrivalDistance 阈值判定，而 SHALL NOT 依赖 native-only magical completion semantics。

**Rationale:** arrival 必须可配置、可验证，并服务于不同上层调用方。

### 4. Override and interruption are first-class move outcomes
**Decision:** 玩家新命令、Stop、Hold、控制效果中断、目标失效等 MUST 作为一等 move outcomes 被显式建模，而不是隐式异常路径。

**Rationale:** Warcraft 的原生命令覆盖是主路径，不是边角条件。

### 5. Continuations belong to callers, not to move ownership
**Decision:** move system MUST emit outcomes；“到达后施法/交互/执行任务” 这类 continuation SHALL 由调用方工作流解释，而 SHALL NOT 由 move subsystem 直接 hard-code。

**Rationale:** 这保证 move 可以被 casting、preset tasks、AI、interaction 统一复用。

## Proposed Runtime Shape

### Canonical ECS data
- `MoveCommand`
- `MoveExecutionState`
- `MoveOutcome`
- `MoveContinuation`

### Canonical responsibilities
- 发布命令
- 执行 native order
- 监控执行过程
- 发出到达/取消/覆盖/中断/失败结果
- 上层工作流处理 continuation

## Outcome Taxonomy

- `Arrived`
- `Cancelled`
- `Overridden`
- `Interrupted`
- `Failed`

## Risks / Trade-offs

- [风险] 迁移期 `MoveToCastSystem` 与新 move subsystem 并存，导致重复所有权。  
  [缓解] 先建立 canonical move data model，再逐个将 casting/task/AI 接入新 outcome model。

- [风险] 玩家覆盖命令的 native 观测不完整。  
  [缓解] 先用“当前命令与 reason/token 不匹配”做基础判定，再逐步细化 stop/hold/order ID 映射。

- [风险] continuation 语义被错误塞回 move system。  
  [缓解] 在 spec 中明确 move 只发结果，不直接拥有施法/任务/AI 后续动作。

## Migration Plan

1. 审核通过本次 OpenSpec 提案。
2. 先定义 red-first tests，覆盖 arrival、override、stop/hold、control interrupt、move-then-cast 与 move-then-task 场景。
3. 引入 canonical move command / execution / outcome / continuation 数据模型。
4. 将当前施法前移动迁移为 move caller，而不是 move owner。
5. 将预设任务与 AI 路径接入同一 move outcome model。
6. 在验证通过后清理旧 `MoveToCast`-centric ownership。

## Open Questions

- 原生命令覆盖的最可靠观测方式是 order id、unit current order，还是更高层 native wrapper 事件。
- `MoveContinuation` 是否应该只是轻量 data carrier，还是由更通用 action/workflow subsystem 接管。
- 到达判定是否需要支持 2D/3D/路径长度差异化模式。

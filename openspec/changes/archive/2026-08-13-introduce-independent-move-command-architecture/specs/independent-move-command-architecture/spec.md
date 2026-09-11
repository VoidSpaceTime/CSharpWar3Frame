## ADDED Requirements

### Requirement: Move MUST be an independent subsystem
move MUST 被建模为独立 subsystem，而 SHALL NOT 作为 casting 的内嵌子模块存在。

#### Scenario: Casting requires pre-move
- **WHEN** 某次施法在距离不足时需要先移动
- **THEN** casting MUST invoke the move subsystem as a caller
- **AND** move ownership SHALL remain independent from casting ownership

### Requirement: ECS MUST publish and monitor move commands
ECS MUST 负责发布 move commands、维护 execution state、判定 outcomes；native execution layers MUST 只负责执行 Warcraft 命令与暴露观测结果。

#### Scenario: A move command is issued
- **WHEN** 某个单位被要求移动到目标点
- **THEN** ECS MUST store the command and execution semantics
- **AND** native execution MUST be treated as execution-only

### Requirement: Arrival MUST be determined through ECS-owned thresholds
到达判定 MUST 基于 ECS-owned target coordinates 与 arrivalDistance 阈值完成。

#### Scenario: Unit gets within arrival distance
- **WHEN** 单位进入目标点的 arrivalDistance 范围内
- **THEN** ECS MUST classify the command as arrived
- **AND** higher-level callers MAY continue their workflows based on that outcome

### Requirement: Move override and interruption MUST be explicit outcomes
玩家新命令、Stop、Hold、控制效果中断、目标失效等 MUST 作为显式 move outcomes 建模。

#### Scenario: Player issues a different command while ECS move is active
- **WHEN** 当前 move command 被玩家其他命令覆盖
- **THEN** move subsystem MUST emit an explicit override-related outcome

#### Scenario: Unit is controlled during move execution
- **WHEN** 单位在执行 move command 时被控制效果打断
- **THEN** move subsystem MUST emit an explicit interruption-related outcome

### Requirement: Move subsystem MUST emit outcomes rather than owning continuations
move subsystem MUST 发出 arrival/cancel/override/interruption/failure outcomes；到达后施法、到达后执行任务等 continuation SHALL 由调用方处理。

#### Scenario: Move-then-cast workflow reaches destination
- **WHEN** 某个 move-then-cast workflow 的移动阶段到达
- **THEN** move subsystem MUST emit arrival outcome
- **AND** casting workflow MUST decide how to continue

### Requirement: Canonical move data model MUST exist
仓库 MUST 为 move architecture 提供 canonical ECS data model，至少覆盖 command、execution state、outcome 与 continuation。

#### Scenario: Multiple higher-level systems reuse move
- **WHEN** casting、preset task 与 AI 都要复用移动能力
- **THEN** they MUST share the canonical move data model rather than each inventing private move semantics

### Requirement: Proposal and design MUST include cross-project impact analysis
任何 move architecture proposal / design MUST 明确说明对 `War3Frame`、`War3Frame.Generator`、`FrameBuild`、`CSharpWar3Frame` 与 `Projects/*` 的影响或非影响结论。

#### Scenario: A move architecture change is proposed
- **WHEN** 新的 move architecture proposal 被创建
- **THEN** proposal 与 design MUST document cross-project impact boundaries
- **AND** they MUST explain why the change is runtime-local or why other projects are affected

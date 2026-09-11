## ADDED Requirements

### Requirement: Unit native sync registry MUST represent projection rules
unit native sync registry/list MUST 表达 projection rules，而 SHALL NOT 仅表达 `attrTypeId -> nativeState` 的狭义映射。

#### Scenario: A synced field requires a specialized native API
- **WHEN** 某个同步字段不能通过通用 `SetUnitState(...)` 写入原生
- **THEN** registry/list MUST still be able to represent that projection rule without forcing handwritten system branches

### Requirement: Per-unit baselines MUST remain separate from projection rules
per-unit baseline state MUST 继续由 snapshot 持有，而 SHALL NOT 被写入 registry/list。

#### Scenario: Two units have different baseline values for the same projection
- **WHEN** 不同单位对同一投影项拥有不同 baseline
- **THEN** those baselines MUST remain isolated in per-unit runtime state

### Requirement: Projection apply logic SHOULD be explicit and centralized
projection 规则 SHOULD 通过显式且集中化的 apply 逻辑表达，而不是继续在系统中分散硬编码。

#### Scenario: Adding a new projection type
- **WHEN** 新增一个 unit native projection
- **THEN** the preferred extension point SHOULD be the projection registry/rule definition rather than another handwritten branch in the main system body

### Requirement: Compare-sync semantics MUST remain unchanged after widening the registry
registry 升级为 projection rules 后，compare-sync 的核心语义 MUST 保持不变。

#### Scenario: ECS values did not meaningfully change
- **WHEN** baseline 与当前 ECS 值差异不显著
- **THEN** native projection SHALL NOT execute redundantly

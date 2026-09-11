## ADDED Requirements

### Requirement: Unit native sync registry MUST only contain static sync declarations
unit native sync registry/list MUST 只保存静态同步规则，而 SHALL NOT 保存 per-unit 运行时 baseline。

#### Scenario: Multiple units share the same sync declarations
- **WHEN** 多个单位使用同一套原生同步规则
- **THEN** registry/list MAY be shared globally
- **AND** runtime snapshot state SHALL remain per-unit

### Requirement: Per-unit snapshot MUST remain the owner of runtime baselines
每个单位的原生同步 baseline MUST 由 per-unit snapshot 持有，而 SHALL NOT 写入全局 registry/list。

#### Scenario: Two units have different last synced health values
- **WHEN** 两个单位具有不同的 last synced values
- **THEN** those baselines MUST remain isolated in per-unit runtime state

### Requirement: UnitNativeSystem SHOULD iterate sync declarations instead of handwritten branches
`UnitNativeSystem` SHOULD 基于静态同步声明列表进行遍历，而 SHALL NOT 继续依赖固定的手写分支作为唯一扩展方式。

#### Scenario: A new native-synced attribute is introduced
- **WHEN** 新增一个需要同步到原生的连续属性
- **THEN** the preferred extension point SHOULD be the sync registry/list rather than additional handwritten system branches and snapshot fields

### Requirement: Compare-sync semantics MUST remain unchanged
registry-driven 改造后，compare-sync 语义 MUST 保持一致：仅当当前值与 baseline 存在有效差异时同步原生。

#### Scenario: Current/final values did not change meaningfully
- **WHEN** 当前属性值与 baseline 的差异未超过有效阈值
- **THEN** native sync SHALL NOT execute redundantly

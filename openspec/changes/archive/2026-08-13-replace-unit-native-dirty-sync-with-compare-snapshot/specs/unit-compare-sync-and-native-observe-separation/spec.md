## ADDED Requirements

### Requirement: Unit native runtime sync MUST NOT be owned by `UnitNativeDirtyFlags`
`UnitNativeDirtyFlags` SHALL NOT 保留为 runtime architecture 的主同步机制。

#### Scenario: Health or mana changes
- **WHEN** 某个单位的 `Health` 或 `Mana` 在 ECS 中发生变化
- **THEN** 运行时同步 MUST NOT 依赖 dirty-flag marking/clearing 作为主路径

### Requirement: Health MUST use compare-sync with snapshot baseline
`Health` MUST 通过 compare-sync 同步到 native，基于 ECS 当前值与 snapshot baseline 的比较结果决定是否调用 `SetUnitState(...)`。

#### Scenario: Health changes after regen or direct mutation
- **WHEN** 某个单位的 `Health` 在 ECS 中被修改
- **THEN** native life MUST 在当前值与 snapshot baseline 不一致时被更新
- **AND** successful sync MUST update the snapshot baseline

### Requirement: Mana MUST use compare-sync with snapshot baseline
`Mana` MUST 通过 compare-sync 同步到 native，基于 ECS 当前值与 snapshot baseline 的比较结果决定是否调用 `SetUnitState(...)`。

#### Scenario: Mana changes after regen or direct mutation
- **WHEN** 某个单位的 `Mana` 在 ECS 中被修改
- **THEN** native mana MUST 在当前值与 snapshot baseline 不一致时被更新
- **AND** successful sync MUST update the snapshot baseline

### Requirement: Snapshot state MUST be minimal and sync-local
snapshot 组件 MUST 只包含 compare-sync 所需的最小 baseline 字段，并 SHALL NOT 成为生命周期真相或动作语义载体。

#### Scenario: Sync baseline is stored
- **WHEN** compare-sync 需要保存已同步状态
- **THEN** snapshot MUST 仅存储最小化同步 baseline
- **AND** it SHALL NOT carry lifecycle truth or action intent

### Requirement: Position MUST remain a separate native-observe path
`Position` MUST 保持在独立的 native-observe path 中，而 SHALL NOT 被折叠进 `Health/Mana` compare-sync ownership。

#### Scenario: Unit position is observed from native
- **WHEN** 系统需要同步单位位置
- **THEN** position readback MUST remain separate from compare-sync ownership

### Requirement: Death and Remove MUST remain lifecycle-driven
`Death` 与 `Remove` MUST 继续由 A 方案生命周期架构拥有，而 SHALL NOT 通过 sync flags 或 snapshot 被重新承载。

#### Scenario: A unit dies or is directly removed
- **WHEN** 某个单位进入 death flow 或 direct remove flow
- **THEN** lifecycle semantics MUST remain governed by `UnitLifeState` and canonical lifecycle systems
- **AND** sync flags or snapshot SHALL NOT reintroduce lifecycle action ownership

### Requirement: Proposal and design MUST include cross-project impact analysis
任何 native sync architecture proposal / design MUST 明确说明对 `War3Frame`、`War3Frame.Generator`、`FrameBuild`、`CSharpWar3Frame` 与 `Projects/*` 的影响或非影响结论。

#### Scenario: A native sync architecture change is proposed
- **WHEN** 新的 native sync 架构提案被创建
- **THEN** proposal 与 design MUST document cross-project impact boundaries
- **AND** they MUST explain why the change is runtime-local or why other projects are affected

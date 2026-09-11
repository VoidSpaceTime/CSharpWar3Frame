## ADDED Requirements

### Requirement: `UnitLifeState` MUST be the sole lifecycle truth
`UnitLifeState.lifePhase` MUST 成为 unit death、corpse retention、clear-corpse progression 与 terminal removal 的唯一持久真相源。Helper-local behavior、timer tags 与 dirty flags SHALL NOT 保留为 primary lifecycle truth。

#### Scenario: A unit enters death flow
- **WHEN** 某个单位被请求进入死亡流程
- **THEN** 主生命周期路径 MUST 通过 `UnitLifeState.lifePhase` 表达
- **AND** 系统 SHALL NOT 依赖 helper-local orchestration 或 `Death` dirty flags 作为生命周期真相源

### Requirement: Lifecycle phase progression MUST be owned by `UnitLifecycleTransitionSystem`
所有主路径上的 lifecycle phase progression MUST 由 `UnitLifecycleTransitionSystem` 集中拥有。

#### Scenario: Corpse retention expires
- **WHEN** 尸体保留时间到期
- **THEN** 生命周期 MUST 通过 `UnitLifecycleTransitionSystem` 推进 toward `ClearCorpse`
- **AND** 该推进 SHALL NOT 由 corpse-specific cleanup system 作为主所有者完成

### Requirement: Lifecycle-native side effects MUST be owned by `UnitLifecycleNativeEffectSystem`
`JassApi.KillUnit(...)`、`JassApi.RemoveUnit(...)`、`HandleHelper.HandleRemove(...)` 等生命周期相关 native side effects MUST 由 `UnitLifecycleNativeEffectSystem` 统一执行。

#### Scenario: A lifecycle phase requires native death behavior
- **WHEN** 某个生命周期阶段要求执行 native death
- **THEN** `UnitLifecycleNativeEffectSystem` MUST 执行该 native death side effect
- **AND** native side effect SHALL NOT 自身成为 lifecycle truth source

#### Scenario: A lifecycle phase requires native remove behavior
- **WHEN** 某个生命周期阶段要求执行 native remove
- **THEN** `UnitLifecycleNativeEffectSystem` MUST 执行该 native remove side effect
- **AND** 生命周期 completion MUST 仍由 lifecycle systems govern

### Requirement: Terminal ECS cleanup MUST be owned by `UnitLifecycleDisposeSystem`
属性清理、技能清理、transient tag cleanup 与 final entity disposal MUST 只由 `UnitLifecycleDisposeSystem` 拥有。

#### Scenario: A unit reaches terminal removal
- **WHEN** 某个单位进入 terminal lifecycle removal
- **THEN** `UnitLifecycleDisposeSystem` MUST 执行最终 ECS cleanup 与 entity disposal
- **AND** helper 或 corpse-specific cleanup path SHALL NOT 再作为 terminal ECS cleanup co-owner

### Requirement: `TimerTaskSystem` MUST only advance lifecycle toward `ClearCorpse`
`TimerTaskSystem` 对 unit corpse-retention 的职责 MUST 限于时间推进与推动生命周期 toward `ClearCorpse`。它 SHALL NOT 拥有 terminal native remove 或 terminal ECS disposal 语义。

#### Scenario: A corpse-retention timer expires
- **WHEN** 某个单位的 corpse-retention timer 到期
- **THEN** `TimerTaskSystem` MUST only advance lifecycle toward `ClearCorpse`
- **AND** it SHALL NOT 执行 terminal native removal
- **AND** it SHALL NOT 执行 terminal ECS disposal

### Requirement: `UnitHelper` MUST only set lifecycle phase intent and flush
`UnitHelper` 的 lifecycle entrypoints MUST 被限制为写入 lifecycle phase intent，并在需要时触发 immediate flush。它们 SHALL NOT 保留 patchwork lifecycle orchestration 责任。

#### Scenario: Helper requests unit death
- **WHEN** 调用方通过 `UnitHelper` 请求单位死亡
- **THEN** helper MUST only write lifecycle phase intent and flush
- **AND** 随后的生命周期路径 MUST 由 canonical lifecycle systems 完成

#### Scenario: Helper requests direct unit removal
- **WHEN** 调用方通过 `UnitHelper` 请求单位直接移除
- **THEN** helper MUST only write lifecycle phase intent and flush
- **AND** native removal 与 ECS disposal MUST 由 canonical lifecycle systems 完成

### Requirement: `CorpseCleanupSystem` and `CorpseExpired` SHALL NOT remain in the primary lifecycle path
实现后的 A 方案 SHALL NOT 依赖 `CorpseCleanupSystem` 与 `CorpseExpired` 作为 primary corpse-finalization route。

#### Scenario: Corpse retention completes
- **WHEN** 尸体保留阶段完成
- **THEN** lifecycle path MUST continue through canonical lifecycle ownership
- **AND** it SHALL NOT require `CorpseCleanupSystem` to remain the primary owner of corpse-finalization semantics

### Requirement: `Death` and `Remove` dirty flags SHALL NOT drive the primary lifecycle path
`Death` 与 `Remove` dirty flags SHALL NOT 保留为 primary lifecycle-driving mechanism。

#### Scenario: A unit dies or is directly removed
- **WHEN** 某个单位进入 death flow 或 direct remove flow
- **THEN** 生命周期 MUST 由 `UnitLifeState` 与 canonical lifecycle systems govern
- **AND** dirty flags SHALL NOT define primary progression semantics

### Requirement: Lifecycle architecture proposals MUST include cross-project impact analysis
任何 lifecycle architecture proposal / design MUST 明确说明对 `War3Frame`、`War3Frame.Generator`、`FrameBuild`、`CSharpWar3Frame` 与 `Projects/*` 的影响或非影响结论。

#### Scenario: A lifecycle architecture change is proposed
- **WHEN** 新的生命周期架构提案被创建
- **THEN** proposal 与 design MUST document cross-project impact boundaries
- **AND** they MUST explain why the change is runtime-local or why other projects are affected

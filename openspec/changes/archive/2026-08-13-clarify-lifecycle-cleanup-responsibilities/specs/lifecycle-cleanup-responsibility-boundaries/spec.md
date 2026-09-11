## ADDED Requirements

### Requirement: Native command systems MUST NOT own lifecycle cleanup completion
消费 `Death`、`Remove` 等立即型 native 命令的系统 MUST 只执行对应的 native 副作用与必要的即时状态推进，而 SHALL NOT 单独承担生命周期清理完成的全部语义。

#### Scenario: Death command is consumed
- **WHEN** 某个单位进入死亡流程并触发立即型 `Death` native 命令
- **THEN** 系统 MAY 执行 native death 动作并推进生命周期到尸体阶段，但 MUST NOT 将该次 native 命令消费视为尸体清理已完成

### Requirement: Death MUST be distinct from corpse cleanup
`Death` 与 `CorpseCleanup` MUST 被视为两个不同阶段。`Death` 表示进入死亡流并执行立即死亡动作；`CorpseCleanup` 表示尸体保留期后的 cleanup policy 阶段。

#### Scenario: Unit dies but corpse should remain temporarily
- **WHEN** 单位执行死亡动作后仍需保留尸体一段时间
- **THEN** 系统 MUST 允许单位进入尸体阶段，并在后续通过单独的 cleanup 流程处理最终销毁，而不是将 death 与 cleanup 视为同一动作

### Requirement: Remove MUST be treated as terminal removal semantics
`Remove` MUST 被视为 terminal removal 语义，而不是 `Death` 或 `CorpseCleanup` 的同义替代。

#### Scenario: Unit is directly removed
- **WHEN** 某个单位走直接移除路径
- **THEN** 系统 MUST 将该操作视为终态删除，而不是将其隐式解释为尸体阶段的一部分

### Requirement: Corpse cleanup MUST own post-expiry final disposal
尸体到期后的最终销毁 MUST 由 cleanup policy / cleanup system 拥有，其职责包括到期后需要执行的最终 native remove、entity 清理与处置推进。

#### Scenario: Corpse retention expires
- **WHEN** 某个单位处于尸体阶段且保留时间到期
- **THEN** 系统 MUST 通过独立 cleanup 流程处理最终销毁，而不是要求 death/native command 系统隐式完成全部收尾

### Requirement: CorpseExpired MUST remain a transient expiration signal
`CorpseExpired` MUST 被视为瞬时到期信号，而不是稳定生命周期阶段；`UnitLifecyclePhase` SHALL NOT 直接承担“尸体已到期”这一瞬时语义。

#### Scenario: Corpse cleanup timer reaches zero
- **WHEN** 尸体保留计时到期
- **THEN** 系统 MUST 发出或消费一个瞬时到期信号以驱动 cleanup，而不是要求通过持久 lifecycle phase 单独表达“刚刚到期”的状态

### Requirement: Lifecycle cleanup proposals MUST include cross-project impact analysis
后续任何涉及生命周期清理边界的提案与设计，都 MUST 从架构师视角说明对 `War3Frame`、`War3Frame.Generator`、`FrameBuild`、`CSharpWar3Frame` 与 `Projects/*` 的影响或不影响结论。

#### Scenario: Runtime lifecycle cleanup change is proposed
- **WHEN** 某个变更涉及运行时中的死亡、尸体保留、最终清理或移除职责调整
- **THEN** proposal 与 design MUST 明确说明为何只影响运行时层，或为何会波及其他项目

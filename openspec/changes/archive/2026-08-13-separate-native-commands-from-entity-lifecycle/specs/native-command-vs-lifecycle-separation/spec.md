## ADDED Requirements

### Requirement: Entity lifecycle state MUST be the sole source of truth for long-lived unit state
单位的长期状态（如存活、垂死、尸体、待复活、池中）MUST 由 entity 生命周期组件表达，SHALL NOT 由 native dirty flags 充当唯一真相源。

#### Scenario: Unit is in corpse-retention phase
- **WHEN** 单位已经原生死亡但尸体保留时间尚未结束
- **THEN** 系统 MUST 能通过 entity 生命周期状态判断该单位处于 corpse 阶段，而不是依赖 `Death` dirty flag 持续存在

### Requirement: UnitNativeDirtyFlags MUST represent immediate native commands only
`UnitNativeDirtyFlags` MUST 仅表示立即型 native 命令，不得承载长期生命周期语义。

#### Scenario: Death command is consumed
- **WHEN** immediate system 已经消费 `UnitNativeDirtyFlags.Death`
- **THEN** 该脏位 MUST 被清除，而单位后续是否仍处于尸体、回池、待复活等阶段 MUST 由生命周期组件继续表达

### Requirement: Death flow MUST separate native death from native removal
死亡流程 MUST 将“执行 native death”与“执行 native remove/handle cleanup”视为两个不同阶段。

#### Scenario: Unit dies but corpse should remain
- **WHEN** 单位死亡后仍需保留尸体阶段
- **THEN** 系统 MUST 先执行 native death，再在后续阶段依据生命周期/时间策略决定是否执行 native remove 与 handle cleanup

### Requirement: Pool-ready designs MUST vary native command policy, not lifecycle semantics
若未来引入单位池，系统 MUST 通过调整 native 命令策略决定“回池还是删除 native”，而不是让生命周期状态机本身语义漂移。

#### Scenario: Corpse cleanup ends under pooling strategy
- **WHEN** 单位尸体阶段结束，且当前策略为对象池复用
- **THEN** 系统 MAY 不执行 native remove，而改为进入池相关 native 处理；但生命周期推进规则 MUST 保持一致且可解释

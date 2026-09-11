## ADDED Requirements

### Requirement: Lifecycle state MUST be component-based
像单位生死、尸体、待复活、池中等长期生命周期状态 MUST 通过组件持久表达，而不是仅依赖 tag 或布尔值作为唯一真相源。

#### Scenario: Unit enters a non-alive lifecycle phase
- **WHEN** 某个单位从存活态进入死亡、尸体、待复活或池化阶段
- **THEN** 系统 MUST 能通过组件状态明确表达当前阶段，并允许后续系统读取该阶段信息

### Requirement: Tags SHALL represent transient events, not persistent lifecycle truth
tag SHOULD 仅用于表达瞬时请求或事件，例如死亡请求、到期通知、回收请求，而 SHALL NOT 单独承担长期生命周期状态真相。

#### Scenario: Delayed cleanup after immediate death
- **WHEN** 单位立即死亡但后续还存在尸体保留期或延迟清理阶段
- **THEN** 系统 MUST 用组件记录长期阶段，而不是仅靠一个一次性 tag 表达整个生命周期

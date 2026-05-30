## ADDED Requirements

### Requirement: 技能生效入口必须命名为 OnEffect

技能 authoring API MUST 使用 `OnEffect` 表达释放前摇完成后的真正生效点。`OnCast` MUST NOT 作为长期公共命名表示生效点。

#### Scenario: 瞬发技能配置生效效果

- **WHEN** 模板使用 `AbilitySpecBuilder.OnEffect(...)`
- **THEN** builder MUST 记录一个 `OnEffect` 行为触发
- **AND** 构建后的 ability MUST 能通过该行为触发对应效果链

### Requirement: 释放请求阶段不得扣除资源

施法请求和开始前摇阶段 MUST 只做资源可用性预检查，不得实际扣除资源。

#### Scenario: 前摇期间被打断

- **WHEN** 单位开始释放带 `CastPoint` 的技能
- **AND** 在 `OnEffect` 之前被打断
- **THEN** 系统 MUST 清理施法状态
- **AND** MUST NOT 执行资源返还逻辑
- **AND** 资源不应因开始施法被扣除

### Requirement: OnEffect 条件通过后扣除资源并触发效果

前摇完成时，系统 MUST 重新检查资源与目标条件。条件通过后 MUST 扣除资源，再触发 `OnEffect` 效果链。

#### Scenario: 前摇完成但资源不足

- **WHEN** 技能前摇完成
- **AND** 施法者资源不足
- **THEN** 系统 MUST NOT 触发 `OnEffect`
- **AND** MUST NOT 扣除资源

#### Scenario: 前摇完成且条件满足

- **WHEN** 技能前摇完成
- **AND** 施法者资源和目标条件满足
- **THEN** 系统 MUST 扣除资源
- **AND** MUST 触发 `OnEffect` 效果链

### Requirement: Channel 必须有独立 tick 触发边界

持续吟唱技能 MUST 通过 `OnChannelTick` 表达周期生效点，而不是复用 `OnEffect` 或每帧无节制触发。

#### Scenario: 持续吟唱每跳触发

- **WHEN** 技能进入 `Channeling`
- **AND** tick 计时达到 `channelTickInterval`
- **THEN** 系统 MUST 触发一次 `OnChannelTick` 条件检查
- **AND** 条件通过后才触发 tick 效果链

### Requirement: 后摇必须独立于生效点

技能后摇 MUST 表达为生效后的生命周期阶段，不得阻止已通过 `OnEffect` 的效果结算。

#### Scenario: 技能生效后进入后摇

- **WHEN** `OnEffect` 已成功触发
- **AND** 技能配置了 `Backswing`
- **THEN** 单位 MUST 进入后摇阶段
- **AND** 后摇结束后技能 MUST 进入冷却或完成状态

### Requirement: 技能生命周期不得直接执行 War3 原生副作用

施法生命周期系统 MUST 只推进 ECS 状态、检查条件、扣除资源和创建效果意图，不得直接调用 War3 原生 API。

#### Scenario: OnEffect 触发 Projectile

- **WHEN** `OnEffect` 效果链包含 Projectile step
- **THEN** 生命周期系统 MUST 创建效果意图
- **AND** Projectile 移动、命中和原生表现 MUST 由后续效果/Native 系统处理

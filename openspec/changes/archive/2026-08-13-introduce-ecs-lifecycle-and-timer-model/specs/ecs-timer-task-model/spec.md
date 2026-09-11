## ADDED Requirements

### Requirement: Timer task MUST support once and loop modes
统一计时任务模型 MUST 同时支持单次延时与循环延时两种模式。

#### Scenario: Schedule a one-shot cleanup
- **WHEN** 某个实体需要在固定延时后执行一次清理行为
- **THEN** 系统 MUST 能以单次模式表达该任务，并在首次触发后结束

#### Scenario: Schedule a periodic effect
- **WHEN** 某个实体需要每隔固定周期反复触发一次效果
- **THEN** 系统 MUST 能以循环模式表达该任务，并在多次触发间持续保留该 timer

### Requirement: Timer task MUST include owner, interval, remaining, paused, and trigger limits
统一计时任务模型 MUST 至少包含以下字段语义：owner、interval、remaining、paused、triggerCount、maxTriggerCount。

#### Scenario: Owner entity is destroyed before timer expires
- **WHEN** 某个计时任务的 owner 已经失效或被销毁
- **THEN** 系统 MUST 能根据 owner 生命周期自动清理或终止该 timer，避免悬空任务继续运行

#### Scenario: Loop timer has a maximum trigger count
- **WHEN** 某个循环 timer 的触发次数达到 `maxTriggerCount`
- **THEN** 系统 MUST 在最后一次触发后结束该 timer，而不是无限继续执行

### Requirement: Timer system MUST separate time progression from business side effects
时间推进系统 MUST 只负责推进时间和判定到期；到期后的业务副作用 MUST 由对应领域系统负责，而不是由 timer 系统直接执行任意业务逻辑。

#### Scenario: Corpse retention reaches zero
- **WHEN** 某个尸体保留 timer 到期
- **THEN** 时间系统 MUST 产生命令式到期信号，而尸体清理 system MUST 负责实际的句柄解绑与实体清理副作用

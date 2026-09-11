## ADDED Requirements

### Requirement: Instant and periodic damage MUST be modeled separately
单次伤害与周期伤害 MUST 作为不同 effect semantics 被显式建模。

#### Scenario: A projectile deals damage once on hit
- **WHEN** 某个弹道效果只应命中一次
- **THEN** it MUST be representable as instant damage semantics

#### Scenario: A moving wave damages units every 0.1s
- **WHEN** 某个移动中的效果需要按固定间隔重复造成伤害
- **THEN** it MUST be representable as periodic damage semantics

### Requirement: Hit policy SHOULD be explicit
命中去重/限频规则 SHOULD 通过独立语义表达，而 SHALL NOT 隐式散落在伤害逻辑里。

#### Scenario: A wave can hit each unit only once
- **WHEN** 某个技能要求每个目标只受一次伤害
- **THEN** the hit-once rule SHOULD be explicitly modeled

#### Scenario: A wave can damage the same unit repeatedly with cooldown
- **WHEN** 某个技能允许重复命中但有最小间隔
- **THEN** that per-target cooldown SHOULD be explicitly modeled

### Requirement: Area and trajectory semantics SHALL remain separate from damage semantics
区域搜索与轨迹语义 SHALL 保持与伤害语义分层。

#### Scenario: A fixed area deals damage over time
- **WHEN** 某个区域持续 effect 每秒造成伤害
- **THEN** area semantics and periodic damage semantics SHALL both be representable without collapsing into one monolithic damage component

### Requirement: Effect systems and damage settlement MUST remain separated
effect systems MUST 负责决定何时发出伤害；damage system MUST 负责最终伤害结算。

#### Scenario: Target has special buff that increases incoming damage by 30%
- **WHEN** 某次 effect 触发伤害请求
- **THEN** the final adjusted damage MUST be determined by the damage settlement layer rather than the effect trigger layer alone

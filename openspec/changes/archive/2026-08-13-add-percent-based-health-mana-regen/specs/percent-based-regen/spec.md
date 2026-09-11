## ADDED Requirements

### Requirement: Health regeneration MUST include percent-based regen
生命回复系统 MUST 同时计算固定回复 `HealthRegen` 与百分比回复 `HealthRegenPercent`。百分比回复 MUST 基于生命属性的 `finalValue` 计算，并与固定回复线性叠加后应用到当前生命值。

#### Scenario: Unit has both flat and percent health regen
- **WHEN** 某个单位同时拥有 `HealthRegen` 和 `HealthRegenPercent`
- **THEN** 生命系统 MUST 按 `flat + maxHealth * percent` 计算每秒回复量，并在每个 tick 中累加到当前生命值

### Requirement: Mana regeneration MUST include percent-based regen
魔法回复系统 MUST 同时计算固定回复 `ManaRegen` 与百分比回复 `ManaRegenPercent`。百分比回复 MUST 基于魔法属性的 `finalValue` 计算，并与固定回复线性叠加后应用到当前魔法值。

#### Scenario: Unit has both flat and percent mana regen
- **WHEN** 某个单位同时拥有 `ManaRegen` 和 `ManaRegenPercent`
- **THEN** 魔法系统 MUST 按 `flat + maxMana * percent` 计算每秒回复量，并在每个 tick 中累加到当前魔法值

### Requirement: Regen output MUST remain clamped to final value
无论固定回复还是百分比回复如何组合，生命与魔法当前值在应用回复后 MUST 被限制在对应属性的 `finalValue` 范围内。

#### Scenario: Regen would exceed current maximum
- **WHEN** 某个单位在一个 tick 内的生命或魔法回复会超过其 `finalValue`
- **THEN** 系统 MUST 将当前值限制到 `finalValue`，而不是允许超出上限

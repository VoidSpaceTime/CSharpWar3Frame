## ADDED Requirements

### Requirement: Kill reward is authored on the victim and dispatched at death

The system SHALL read the killed unit's reward configuration to give experience to the killer when a unit dies. Units without that configuration SHALL grant nothing.

#### Scenario: Kill grants authored experience to killer

- **GIVEN** a victim unit authored with `ExpReward(Fixed(100))`
- **AND** a killer unit that has `ExperienceData`
- **WHEN** the victim's health reaches zero through a `DamageRequest`
- **THEN** an `ExperienceGainRequest` SHALL be created with target = killer and amount = `100`

#### Scenario: Victim without reward grants nothing

- **GIVEN** a victim with no `ExpReward` authoring (default `0`)
- **WHEN** the victim dies
- **THEN** no `ExperienceGainRequest` SHALL be created

#### Scenario: Killer without experience data is ignored

- **GIVEN** a killer that has no `ExperienceData`
- **WHEN** an experience reward request targets it
- **THEN** the experience system SHALL silently ignore the request

### Requirement: Unit skill point pool is opt-in

The system SHALL attach a skill-point pool only to units whose authoring enables it.

#### Scenario: Unconfigured unit gains no pool

- **GIVEN** a unit with no `SkillPoints` authoring
- **WHEN** the unit is built
- **THEN** it SHALL NOT have `SkillPointPool`

#### Scenario: Configured unit receives a pool

- **GIVEN** `UnitSpecBuilder.SkillPoints(perLevel: 1, initial: 0)`
- **WHEN** the unit is built
- **THEN** it SHALL have `SkillPointPool` with `perLevel = 1`, `unspent = 0`, `earned = 0`

### Requirement: Unit level-up grants skill points synchronously

When a unit with `SkillPointPool` increases `UnitLevel`, the experience system SHALL grant `perLevel` unspent points for each level gained and create one `UnitLeveledEvent`.

#### Scenario: Single level grant

- **GIVEN** a unit with `SkillPointPool.perLevel = 1` and `unspent = 0` at level `1`
- **WHEN** experience raises the unit to level `2`
- **THEN** `unspent` SHALL become `1` and `earned` SHALL increase by `1`

#### Scenario: Multi-level settlement sums once

- **GIVEN** a unit with `perLevel = 1` at level `1`
- **WHEN** one experience settlement raises the unit to level `3`
- **THEN** `unspent` SHALL become `2`
- **AND** exactly one `UnitLeveledEvent` SHALL be created with `fromLevel = 1`, `toLevel = 3`

#### Scenario: Unit without pool still broadcasts level event

- **GIVEN** a unit without `SkillPointPool`
- **WHEN** the unit levels up
- **THEN** it SHALL NOT receive points
- **AND** a `UnitLeveledEvent` SHALL still be created for listeners

### Requirement: Experience system does not grant through a separate event-consuming system

Skill-point granting SHALL happen synchronously inside the experience settlement; it SHALL NOT depend on another system consuming `UnitLeveledEvent` to add points.

#### Scenario: No intermediate grant system

- **GIVEN** a leveled unit with `SkillPointPool`
- **WHEN** the experience settlement finishes
- **THEN** `unspent` SHALL already be updated in the same settlement

### Requirement: Ability max level gates skill-point upgrade and marks growth mode

An ability with `AbilitySpec.maxLevel > 0` is a skill-point-growth ability. `maxLevel <= 0` SHALL reject skill-point spend and keep the proficiency-experience path.

#### Scenario: MaxLevel authoring

- **GIVEN** `AbilitySpecBuilder.MaxLevel(3)`
- **WHEN** the spec is applied
- **THEN** `AbilitySpec.maxLevel` SHALL be `3`

#### Scenario: Default max level rejects spend

- **GIVEN** an ability whose spec never set `MaxLevel`
- **WHEN** an `AbilityUpgradeRequest` targets it
- **THEN** the request SHALL fail
- **AND** skill points and ability level SHALL stay unchanged

### Requirement: Proficiency experience does not drive skill-point-growth abilities

The experience system SHALL NOT auto-level an ability whose `AbilitySpec.maxLevel > 0`; experience on such abilities SHALL accumulate without changing `AbilityBase.level`.

#### Scenario: Growth-mode ability ignores experience level-up

- **GIVEN** an ability with `AbilitySpec.maxLevel = 3` and `ExperienceData`
- **WHEN** an experience request would otherwise raise its level
- **THEN** `AbilityBase.level` SHALL remain unchanged
- **AND** `ExperienceData.currentExp` SHALL still accumulate

### Requirement: Upgrade spends points and raises slot ability level

The system SHALL consume `AbilityUpgradeRequest` to spend unspent points and increase a slot-mounted owned ability's level, then mark the ability `LevelStatDirty`.

#### Scenario: Successful one-level upgrade

- **GIVEN** a unit with `unspent = 1` owning a slot ability at level `1` with `maxLevel = 3`
- **WHEN** `AbilityUpgradeRequest` with `levels = 1` is consumed
- **THEN** ability level SHALL become `2`, `unspent` SHALL become `0`
- **AND** the ability SHALL have `LevelStatDirty`
- **AND** an `AbilityUpgradedEvent` SHALL be created with `fromLevel = 1`, `toLevel = 2`, `pointsSpent = 1`

#### Scenario: Insufficient points

- **GIVEN** `unspent = 0`
- **WHEN** an upgrade request is consumed
- **THEN** ability level and `unspent` SHALL remain unchanged
- **AND** no `AbilityUpgradedEvent` SHALL be created

#### Scenario: Already at max level

- **GIVEN** ability level `3` and `maxLevel = 3`
- **WHEN** an upgrade request is consumed
- **THEN** ability level and `unspent` SHALL remain unchanged

### Requirement: Non-slot abilities cannot be skill-point upgraded

Item-granted companion abilities and other non-slot mounts SHALL be rejected by upgrade requests.

#### Scenario: Item companion rejected

- **GIVEN** an ability with `AbilityMountType.ItemGranted`
- **WHEN** an `AbilityUpgradeRequest` targets it
- **THEN** the request SHALL fail and the companion level SHALL remain unchanged

### Requirement: Level and skill-point facts are Event entities

`UnitLeveledEvent` and `AbilityUpgradedEvent` SHALL be independent event entities with `TriggerEventMarker`, registered in `EventTypeRegistry`, and cleaned by `EventCleanupSystem`.

#### Scenario: Events are marked for cleanup

- **GIVEN** a unit levels up or an ability is upgraded by skill points
- **WHEN** the corresponding event entity is created
- **THEN** it SHALL include `TriggerEventMarker`
- **AND** listeners with order `>= 132` SHALL NOT be required to observe it

### Requirement: No War3 native calls for skill-point flow

The skill-point pool, kill reward, level events, and upgrade workflow SHALL NOT call War3 native APIs such as `UnitModifySkillPoints` or `IncUnitAbilityLevel`.

#### Scenario: ECS is the only source of truth

- **GIVEN** the full flow from kill reward to ability upgrade
- **WHEN** any step executes
- **THEN** only ECS components, requests, and event entities SHALL be mutated

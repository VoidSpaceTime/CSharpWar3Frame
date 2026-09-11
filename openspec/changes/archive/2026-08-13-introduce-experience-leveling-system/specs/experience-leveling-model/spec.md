## ADDED Requirements

### Requirement: Experience curve model

The system SHALL provide a data-driven experience curve model for Unit, Ability, and Item leveling.

#### Scenario: Fixed step curve

- **GIVEN** an experience curve `FixedStep(100)`
- **WHEN** the current level is any positive level
- **THEN** the required experience for next level SHALL be `100`

#### Scenario: Linear curve

- **GIVEN** an experience curve `Linear(100, 50)`
- **WHEN** the current level is `2`
- **THEN** the required experience for next level SHALL be `150`

#### Scenario: Level table curve

- **GIVEN** an experience curve `LevelTable(100, 180, 300)`
- **WHEN** the current level is `3`
- **THEN** the required experience for next level SHALL be `300`

### Requirement: Experience gain request

The system SHALL use an experience gain request to add experience to a target entity instead of requiring source systems to mutate experience data directly.

#### Scenario: Multiplied experience gain

- **GIVEN** an `ExperienceGainRequest` with `amount = 100` and `multiplier = 1.5`
- **WHEN** the experience system consumes the request
- **THEN** the target SHALL gain `150` experience

### Requirement: Upgrade triggers level stat dirty

When experience gain causes a level increase, the system SHALL update the target level and add `LevelStatDirty` to the target entity.

#### Scenario: Unit levels up

- **GIVEN** a unit with experience data and current level `1`
- **AND** a curve requiring `100` experience for next level
- **WHEN** the unit gains `100` experience
- **THEN** the unit level SHALL become `2`
- **AND** the unit SHALL receive `LevelStatDirty`

### Requirement: Experience system responsibility boundary

The experience system SHALL NOT calculate level-derived attributes, ability values, item contributions, Buff composition, equipment aggregation, or War3 native synchronization.

#### Scenario: Level up does not directly rebuild attributes

- **GIVEN** an entity gains enough experience to level up
- **WHEN** the experience system processes the gain
- **THEN** it SHALL only update level and add `LevelStatDirty`
- **AND** it SHALL NOT directly modify final attack, health, mana cost, damage amount, or item contribution totals

### Requirement: Unit Ability Item applicability

The experience model SHALL be applicable to Unit, Ability, and Item entities.

#### Scenario: Ability gains proficiency experience

- **GIVEN** an ability entity with `ExperienceData`
- **WHEN** it gains enough experience to level up
- **THEN** its ability level SHALL increase
- **AND** it SHALL receive `LevelStatDirty`

#### Scenario: Item gains growth experience

- **GIVEN** an item entity with `ExperienceData`
- **WHEN** it gains enough experience to level up
- **THEN** its item level SHALL increase
- **AND** it SHALL receive `LevelStatDirty`

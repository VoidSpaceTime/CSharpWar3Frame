# Capability: UnitDiedEvent broadcast and kill reward as event consumer

## ADDED Requirements

### Requirement: Death broadcasts one UnitDiedEvent at the lethal transition

The death entry `KillUnit` SHALL create exactly one `UnitDiedEvent` event entity when and only when a unit transitions Alive → Death. Later damage or kill attempts on an already dead unit SHALL NOT create further events.

#### Scenario: Lethal hit creates one event

- **GIVEN** a unit in `Alive` state
- **WHEN** `KillUnit(unit, source)` performs the Alive → Death transition
- **THEN** exactly one `UnitDiedEvent` SHALL be created with `unit` = the dead unit and `source` = the killer

#### Scenario: Same-frame overkill does not duplicate events

- **GIVEN** a unit whose health reaches zero from the first of two lethal damage requests in one frame
- **WHEN** both damage requests are resolved
- **THEN** exactly one `UnitDiedEvent` SHALL exist
- **AND** exactly one kill reward request SHALL be dispatched

#### Scenario: Non-kill death still broadcasts with null source

- **GIVEN** a unit entering death without a killer (source is default/empty)
- **WHEN** `KillUnit` transitions it to Death
- **THEN** a `UnitDiedEvent` SHALL be created with `source` empty

### Requirement: Died events are marked event entities

`UnitDiedEvent` SHALL be an independent event entity carrying `TriggerEventMarker`, registered in `EventTypeRegistry`, and cleaned by `EventCleanupSystem` at order 132. Consumers MUST have order below 132 to observe it.

#### Scenario: Event lifecycle contract

- **GIVEN** a unit dies
- **WHEN** the death event entity is created
- **THEN** it SHALL include `TriggerEventMarker`
- **AND** `EventCleanupSystem` SHALL remove it at order 132

### Requirement: Kill reward consumes the death event, not the damage branch

The kill reward logic SHALL live in a dedicated system consuming `UnitDiedEvent`, and SHALL NOT be inlined in the damage resolution death branch.

#### Scenario: Reward follows the event

- **GIVEN** a victim authored with `expReward` killed by a source
- **WHEN** `KillRewardSystem` consumes the death event
- **THEN** an `ExperienceGainRequest` SHALL be created with target = the killer
- **AND** the `DamageResolveSystem` death branch SHALL NOT dispatch rewards directly

#### Scenario: Reward system does not delete the event

- **GIVEN** a death event consumed by `KillRewardSystem`
- **WHEN** the system dispatches the reward request
- **THEN** the `UnitDiedEvent` entity SHALL remain for other listeners until `EventCleanupSystem` runs

### Requirement: Reward only for configured kill deaths

Kill reward SHALL be dispatched only when the death event has a non-empty source AND the dead unit carries reward data resolving to a positive amount.

#### Scenario: Null-source death grants nothing

- **GIVEN** a dead unit with `expReward` and an empty `source`
- **WHEN** `KillRewardSystem` consumes the event
- **THEN** no `ExperienceGainRequest` SHALL be created

#### Scenario: Victim without reward grants nothing

- **GIVEN** a dead unit without `UnitKillRewardData`
- **WHEN** `KillRewardSystem` consumes the event
- **THEN** no `ExperienceGainRequest` SHALL be created

### Requirement: Removal is not death

Unit removal paths (`RemoveUnit`, terminal cleanup) SHALL NOT create `UnitDiedEvent`; only the death entry `KillUnit` broadcasts death. Callers SHALL use `RemoveUnit` to delete a unit without death side effects instead of a hypothetical "silent KillUnit".

#### Scenario: Remove does not broadcast death

- **GIVEN** a unit that must be removed without death semantics
- **WHEN** `RemoveUnit` or terminal cleanup is invoked
- **THEN** no `UnitDiedEvent` SHALL be created

#### Scenario: Optional source for KillUnit

- **GIVEN** `KillUnit(unit)` invoked without a killer
- **WHEN** the unit transitions Alive → Death
- **THEN** a `UnitDiedEvent` SHALL be created with `source` empty
- **AND** kill reward SHALL NOT be dispatched

### Requirement: No War3 native calls in the death event flow

Death broadcasting and reward consumption SHALL NOT call War3 native APIs; only ECS components, event entities, and request entities SHALL be mutated.

#### Scenario: ECS is the only source of truth

- **GIVEN** a death and reward dispatch
- **WHEN** any step executes
- **THEN** no War3 native API SHALL be invoked

# unit-reborn-and-pool Specification

## Purpose
单位复活等待（`RebornPending` + `rebornTime`）与单位池复用（`Pooled`）语义：定义生命周期阶段与原生移除/销毁系统的互斥边界、句柄保留和复活显隐/回满规则，并排除 JASS 原生池与物品池路径。

## Requirements

### Requirement: Positive rebornTime waits instead of removing the unit

When a unit's corpse cleanup completes and `UnitLifeState.rebornTime` is greater than zero, the lifecycle transition SHALL move that unit from `ClearCorpse` to `RebornPending` and SHALL NOT move it to `Remove`. The ECS entity and any existing `UnitNative` handle reference SHALL remain. Units with `rebornTime` less than or equal to zero SHALL keep the current `ClearCorpse` to `Remove` path.

#### Scenario: Reborn countdown

- **WHEN** a unit with `rebornTime` greater than zero finishes corpse time and the reborn wait then elapses
- **THEN** the same entity SHALL be `Alive` and `isAlive` SHALL be true
- **AND** the entity SHALL NOT have been deleted
- **AND** no second `UnitDiedEvent` SHALL be created

#### Scenario: Zero rebornTime still disposes

- **WHEN** a unit with `rebornTime` less than or equal to zero reaches `ClearCorpse`
- **THEN** the unit SHALL enter `Remove` and then `Disposing`
- **AND** terminal cleanup SHALL delete the entity

### Requirement: Reborn native work reuses the existing handle

The native reborn execution SHALL run only from `UnitRebornNativeRequest` inside a Native system. If the unit has `UnitNative`, that system SHALL restore native life and show the unit. It SHALL NOT call `CreateUnit`, `RemoveUnit`, `HandleAdd`, `HandleRemove`, or `ReviveHero`.

#### Scenario: Handle pairing on reborn

- **WHEN** a `RebornPending` unit becomes `Alive`
- **THEN** the original `HandleAdd` from unit creation SHALL remain the matching reference
- **AND** `HandleRemove` SHALL NOT run for that transition

#### Scenario: No native handle in local validation

- **WHEN** a reborn request is consumed and the unit has no `UnitNative`
- **THEN** the request SHALL be removed
- **AND** no War3 native API SHALL be called

### Requirement: Pooled units are suspended entities, not a JASS type pool

`RequestPool` SHALL suspend an `Alive` or `RebornPending` unit into `Pooled` without broadcasting `UnitDiedEvent` and without `RemoveUnit`. `AcquirePooled` SHALL return that same entity to `Alive` while keeping the same `UnitNative`. `DiscardPooled` SHALL be the only pool exit that enters `Remove`. This capability SHALL NOT call `CreateUnitPool`, `PlaceRandomUnit`, `CreateItemPool`, or `PlaceRandomItem`.

#### Scenario: Pool reuse

- **WHEN** `RequestPool` then `AcquirePooled` run on a unit
- **THEN** the entity id SHALL be unchanged
- **AND** the phase SHALL be `Alive`
- **AND** no `UnitDiedEvent` SHALL be created by pooling

#### Scenario: Pool discard uses the single destroy point

- **WHEN** `DiscardPooled` runs on a `Pooled` unit
- **THEN** the unit SHALL enter `Remove`
- **AND** handle unregistration SHALL happen only in `UnitRemoveNativeSystem` immediately before `RemoveUnit`

### Requirement: Remove and dispose ignore reborn and pool phases

`UnitRemoveNativeSystem` SHALL NOT kill, remove, or unregister handles for `RebornPending` or `Pooled`. `UnitLifecycleDisposeSystem` SHALL delete an entity only in `Disposing`.

#### Scenario: Waiting unit is not disposed

- **WHEN** a unit is `RebornPending` or `Pooled` and the lifecycle systems update
- **THEN** the entity SHALL still exist
- **AND** its phase SHALL NOT become `Disposing`

### Requirement: Item pools stay out of this capability

`JItemPool` and `CreateItemPool` SHALL remain unused by unit reborn and unit pooling. Item-pool behavior is not part of this capability.

#### Scenario: Item pool excluded

- **WHEN** this capability's reborn or pool path runs
- **THEN** no item-pool native API SHALL be called

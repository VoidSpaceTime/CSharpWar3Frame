## ADDED Requirements

### Requirement: Friflo relationship type selection

The project SHALL distinguish `IRelation<TKey>`, `ILinkComponent`, and `ILinkRelation` by semantic intent.

#### Scenario: Entity to entity single link

- **GIVEN** one source entity links to one target entity for a long-lived binding
- **WHEN** the relationship is modeled with Friflo
- **THEN** the design SHALL prefer `ILinkComponent`

#### Scenario: Entity to multiple entity links

- **GIVEN** one source entity links to multiple target entities for a long-lived binding
- **WHEN** the relationship is modeled with Friflo
- **THEN** the design SHALL prefer `ILinkRelation`

#### Scenario: Entity to non-entity key relation

- **GIVEN** one entity stores multiple same-type entries keyed by enum, int, string, or another non-entity key
- **WHEN** the relation is modeled with Friflo
- **THEN** the design SHALL use `IRelation<TKey>` rather than entity link APIs

### Requirement: Do not migrate transient payloads by default

Transient commands, requests, and calculation payloads SHALL NOT be migrated to Friflo link/relation APIs unless a follow-up proposal proves they are long-lived semantic bindings.

#### Scenario: Damage payload

- **GIVEN** a component only carries source and target for one damage or settlement operation
- **WHEN** evaluating Friflo relation migration
- **THEN** it SHALL remain a normal component by default

### Requirement: Preserve slot and order semantics

Relationships with slot, order, index, or priority semantics SHALL preserve that metadata during any migration.

#### Scenario: Ability slot binding

- **GIVEN** an ability is attached to a unit slot
- **WHEN** the owner binding is represented as a Friflo link
- **THEN** the slot index SHALL remain explicitly represented

### Requirement: Native layer isolation

Friflo relation migration SHALL NOT move War3 native side effects into business workflow systems.

#### Scenario: Effect attachment

- **GIVEN** an effect attachment relationship is migrated to a Friflo link
- **WHEN** the relationship is applied
- **THEN** War3 native visual attachment SHALL still be executed only by Native / Execution layer systems

### Requirement: Link update correctness

`ILinkComponent` target updates SHALL use `AddComponent(new Link { target = ... })` so Friflo indexes remain correct.

#### Scenario: Updating owner link

- **GIVEN** an entity already has a link component
- **WHEN** the target entity changes
- **THEN** code SHALL re-add the component rather than mutate the target field by ref

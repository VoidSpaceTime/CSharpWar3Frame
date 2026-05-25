## ADDED Requirements

### Requirement: Ground area effects MUST be represented as ECS entities

Ground area effects such as oil pools, burning ground, poison clouds, or similar persistent zones MUST be represented as ECS entities with ECS-owned position, radius, lifetime, source, and semantic tags.

#### Scenario: Oil area persists after ability impact

- **WHEN** `凝固汽油` reaches its target point
- **THEN** an ECS ground area entity tagged as oil MUST exist at that position
- **AND** the area MUST remain interpretable from ECS components until its lifetime expires

### Requirement: Ground area effects MUST use existing `Position` as location truth

Ground area entities MUST use the existing `Position` component as their location truth and SHALL NOT introduce a separate ground-area position model.

#### Scenario: Area query runs

- **WHEN** a ground area system queries units inside an area
- **THEN** the query center MUST be derived from the area entity's `Position`

### Requirement: Ground area Buffs MUST be applied and removed by systems

Ground area Buff behavior MUST be implemented by systems that apply Buffs to units inside the area and remove area-owned Buffs when units leave or the area expires.

#### Scenario: Unit enters oil area

- **WHEN** a ground unit is inside an active oil area
- **THEN** it MUST receive the configured slow Buff
- **AND** the Buff MUST be owned or linked to the source ground area

#### Scenario: Unit leaves oil area

- **WHEN** a unit no longer satisfies the oil area's range/filter condition
- **THEN** the slow Buff created by that oil area MUST be removed or expired

### Requirement: Periodic ground area damage MUST emit `DamageRequest`

Periodic area damage MUST generate `DamageRequest` entities on tick and SHALL NOT directly modify target health in the ground area system.

#### Scenario: Burning ground ticks

- **WHEN** a burning ground area is active
- **THEN** it MUST emit configured `DamageRequest` instances at the configured interval
- **AND** each request MUST target units that satisfy the area's filter at that tick

### Requirement: Ground area reactions MUST be explicit ECS-driven interactions

Interactions such as fire igniting oil MUST be represented as explicit ECS-driven reactions or requests, not hidden inside template methods or helper-local state.

#### Scenario: Fire contacts oil

- **WHEN** a fire-tagged effect contacts an oil-tagged ground area
- **THEN** a reaction MUST transform, expire, or replace the oil area according to ECS-visible reaction data

#### Scenario: Oil becomes burning ground

- **WHEN** an oil area is ignited
- **THEN** a burning ground area MUST exist with configured duration, radius, tick interval, and damage amount

### Requirement: Shape search MUST support line or cone effects

The effect model MUST support line or cone search semantics by reusing existing spatial query helpers such as `GroupHelper.FindInLine` or `GroupHelper.FindInCone`.

#### Scenario: Flamethrower hits units in a line

- **WHEN** `喷火` is cast toward a target point
- **THEN** units inside the configured line width and range MUST be selected by ECS search logic
- **AND** the resulting damage MUST be represented through `DamageRequest`

### Requirement: Ability templates MUST remain data configuration only

Ability templates for ground area interactions MUST configure data and SHALL NOT directly query ground areas, apply damage, manipulate Buffs, or call War3 native APIs.

#### Scenario: Napalm template is configured

- **WHEN** the `凝固汽油` template is applied to an ability entity
- **THEN** it MUST only attach data components or `EffectSpec`-like descriptions needed by runtime systems

#### Scenario: Flamethrower template is configured

- **WHEN** the `喷火` template is applied to an ability entity
- **THEN** it MUST only configure line damage and fire/oil reaction intent data

### Requirement: Native execution MUST remain separate from ground area semantics

Ground area semantics MUST remain in ECS. Native effects or visuals MAY represent oil, fire, or burning areas, but SHALL NOT own their gameplay state.

#### Scenario: Ground area visual is missing

- **WHEN** a visual effect for an oil or burning area fails to spawn
- **THEN** the ECS ground area gameplay semantics MUST remain valid

### Requirement: Proposal and design MUST document cross-project impact

Changes to the ground area effect model MUST document impact or non-impact for `War3Frame`, `War3Frame.Generator`, `FrameBuild`, `CSharpWar3Frame`, and `Projects/*`.

#### Scenario: Ground area model is proposed

- **WHEN** a ground area model proposal is reviewed
- **THEN** it MUST include explicit cross-project impact boundaries

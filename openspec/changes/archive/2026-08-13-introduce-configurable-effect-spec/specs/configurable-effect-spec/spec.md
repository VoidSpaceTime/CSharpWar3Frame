# Configurable Effect Spec

## ADDED Requirements

### Requirement: Data-authored effect chains

The ability effect layer SHALL support an editor/configuration-friendly data model that describes ordered effect chains without requiring C# delegates for ordinary skill authoring.

#### Scenario: Ordinary damage skill uses data spec

- **GIVEN** an ability entity with an effect spec containing a damage step
- **AND** the damage step references a `statId` and a built-in formula id
- **WHEN** the ability creates an effect entity
- **THEN** the runtime shall settle damage using the referenced ability stat and formula
- **AND** no delegate formula shall be required

#### Scenario: Builder emits data spec

- **GIVEN** C# authoring code uses the effect spec builder
- **WHEN** the builder produces a chain
- **THEN** the output shall be the same data model used by configuration/editor input
- **AND** the builder shall not add behavior that cannot be represented in the data spec

### Requirement: Formula references

The effect spec SHALL express ordinary numeric formulas through `formulaId`, optional `statId`, and a parameter table.

#### Scenario: Formula id with parameters resolves a value

- **GIVEN** a damage or heal effect step with `formulaId`, `statId`, and parameter values
- **WHEN** the effect settles
- **THEN** the formula registry shall resolve the formula id
- **AND** the formula shall receive caster, ability, target, context, stat reference, and parameters
- **AND** the resolved value shall be used by the settlement request

#### Scenario: Missing formula is predictable

- **GIVEN** an effect step references an unknown `formulaId`
- **WHEN** the effect settles
- **THEN** the runtime shall fail predictably or use only an explicitly documented fallback
- **AND** silent zero-value settlement shall not be the default behavior

### Requirement: Delegate formulas remain advanced overrides

The runtime SHALL preserve existing delegate formula support for advanced customization while making data formulas the ordinary path.

#### Scenario: Delegate override takes precedence

- **GIVEN** a damage or heal runtime effect has both a delegate formula and a data formula reference
- **WHEN** the effect settles
- **THEN** the delegate formula shall take precedence
- **AND** the data formula shall remain available as the ordinary non-delegate path

### Requirement: Compatibility with existing effect components

The new spec layer SHALL be additive and preserve existing direct component-authored effects.

#### Scenario: Existing direct damage component still works

- **GIVEN** an ability entity authored with `DamageEffectData` and no effect spec
- **WHEN** the ability creates and settles an effect
- **THEN** the existing direct component path shall still produce damage using existing fallback behavior

#### Scenario: Existing projectile and area components still work

- **GIVEN** an ability entity authored with existing `ProjectileData` or `AreaSearchData`
- **WHEN** the ability creates an effect
- **THEN** the current projectile and area search behavior shall remain compatible

### Requirement: Native separation is preserved

The configurable effect spec SHALL NOT introduce new direct War3 native calls in business workflow or semantic effect authoring layers.

#### Scenario: Spec-authored visual/projectile effect

- **GIVEN** an effect spec describes projectile or visual effect data
- **WHEN** the runtime executes the effect
- **THEN** War3 native side effects shall continue to be handled by existing native/execution pathways
- **AND** the spec/builder layer shall remain an ECS intent/data authoring layer

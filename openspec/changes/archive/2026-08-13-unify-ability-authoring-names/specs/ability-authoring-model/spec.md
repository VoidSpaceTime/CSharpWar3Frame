## ADDED Requirements

### Requirement: Ability authoring MUST use three named layers

技能 authoring 模型 MUST 明确区分 `AbilitySpec`、`AbilityBehavior` 和 `AbilityEffect` 三层职责。

#### Scenario: Active ability is authored

- **WHEN** a normal active ability is configured
- **THEN** top-level identity and stats MUST belong to `AbilitySpec`
- **AND** cast trigger and targeting flow MUST belong to `AbilityBehavior`
- **AND** damage/heal/buff/ground-area settlement MUST belong to `AbilityEffect`

### Requirement: AbilityBehavior MUST own trigger and flow semantics

`AbilityBehavior` MUST describe when a skill runs, how it repeats, how targets are searched/selected, and when it invokes effects.

#### Scenario: Passive ability starts on grant

- **WHEN** an ability is granted to a unit
- **THEN** an `OnGranted` behavior MAY create or activate runtime behavior state
- **AND** the behavior MUST be owned by ECS-visible data rather than helper-local state or template-local closure

#### Scenario: Periodic behavior runs

- **WHEN** a behavior declares `Repeat(interval)`
- **THEN** its interval MUST be resolved through the unified value system
- **AND** the repeated execution MUST be stoppable when the owner dies or the ability is removed

### Requirement: AbilityEffect MUST remain the settlement effect model

`AbilityEffect` MUST represent actual effects such as damage, healing, Buff application, projectile payloads, target search payloads, and ground area creation.

#### Scenario: Behavior invokes an effect

- **WHEN** `AbilityBehavior` reaches a `Do` or `OnArrive` step
- **THEN** it MUST invoke an `AbilityEffect` description or create equivalent ECS effect/request data
- **AND** it MUST NOT directly modify health, attributes, Buff truth, or War3 native state

### Requirement: AbilityValue MUST support ability stats and unit attributes

The authoring model MUST provide a unified value representation for constants, ability stats, owner/caster/target attributes, and formulas.

#### Scenario: Damage scales from caster attack

- **WHEN** a skill configures damage as `CasterAttr(AttackDamage, scale: 1.1)`
- **THEN** value resolution MUST read the caster attribute through ECS attribute truth
- **AND** the damage effect MUST still emit `DamageRequest` rather than directly mutating health

#### Scenario: Heal scales from target max health

- **WHEN** a skill configures heal as `TargetAttr(HealthMax, scale: 0.01)`
- **THEN** value resolution MUST read the target attribute through ECS attribute truth
- **AND** the heal effect MUST still emit `HealRequest`

### Requirement: Existing EffectSpec users MUST have a migration path

The migration from `EffectSpec` naming to `AbilityEffectSpec` naming MUST preserve a clear path for existing templates.

#### Scenario: Existing active templates build

- **WHEN** old templates still use `EffectSpecBuilder`
- **THEN** the migration strategy MUST keep them buildable during the first implementation slice
- **AND** new examples SHOULD prefer the new `AbilityEffectSpecBuilder` naming

### Requirement: Target selection MUST be separate from effect settlement

Target searching, filtering, and sorting MUST be represented as behavior or target selector semantics rather than hidden inside damage/heal effects.

#### Scenario: Healing Bird selects wounded ally

- **WHEN** `治疗之鸟` searches within owner attack range
- **THEN** target selection MUST be expressible as circle search + ally/alive/missing-health filter + lowest health percent metric
- **AND** the final heal MUST remain an `AbilityEffect` invoked after projectile arrival

### Requirement: Native execution MUST remain separate

Ability authoring, behavior, and effect settlement MUST NOT directly call War3 native APIs.

#### Scenario: Behavior spawns projectile or visual intent

- **WHEN** a behavior or effect needs projectile or visual representation
- **THEN** it MUST create ECS intent/request data
- **AND** Native/Execution systems MAY consume that data to perform War3 native side effects

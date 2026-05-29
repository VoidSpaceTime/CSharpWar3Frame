## ADDED Requirements

### Requirement: Unit authoring MUST use UnitSpecBuilder for template configuration

单位模板 authoring SHOULD use `UnitSpecBuilder` to describe unit identity, base attributes, item slots, and ability references.

#### Scenario: Unit template is configured

- **WHEN** an `IUnitTemplate.Configure(Entity entity)` implementation defines a normal unit template
- **THEN** it SHOULD be able to call `UnitSpecBuilder.Create(templateName).BuildTo(entity)`
- **AND** the builder MUST write template data to the provided entity rather than creating a runtime unit entity
- **AND** base attributes MUST be expressed through a clear authoring API rather than scattered direct component writes

#### Scenario: Unit has abilities

- **WHEN** a unit template declares ability template ids
- **THEN** the declaration MUST remain authoring data or invoke existing approved ability attachment paths
- **AND** `UnitSpecBuilder` MUST NOT become the owner of runtime casting, cooldown, or behavior state

### Requirement: Item authoring MUST use ItemSpecBuilder for template configuration

物品模板 authoring SHOULD use `ItemSpecBuilder` to describe item identity, stack behavior, usability, equipment attributes, and use behavior references.

#### Scenario: Equipment item is configured

- **WHEN** an `IItemTemplate.Configure(Entity item)` implementation defines an equipment item
- **THEN** it SHOULD be able to configure `ItemBase` semantics and attribute contributions through `ItemSpecBuilder`
- **AND** attribute contribution data MUST remain ECS-visible data consumed by existing attribute systems

#### Scenario: Usable item is configured

- **WHEN** an item has an active use behavior
- **THEN** the recommended authoring path SHOULD reference an ability template through `UseAbility(...)`
- **AND** complex target selection, projectile, damage, heal, buff, or ground-area flow SHOULD reuse `AbilitySpec` / `AbilityBehavior` / `AbilityEffect` rather than duplicating a separate item DSL

### Requirement: SpecBuilder MUST NOT replace runtime helpers

`UnitSpecBuilder` and `ItemSpecBuilder` MUST remain template authoring tools and MUST NOT replace runtime helper entry points.

#### Scenario: Unit is created at runtime

- **WHEN** game logic needs to spawn a unit in the world
- **THEN** it MUST continue to use `UnitHelper.CreateUnit(...)` or another approved runtime creation path
- **AND** `UnitHelper` MAY call template creation internally
- **AND** `UnitSpecBuilder` MUST NOT directly own position, lifecycle phase transition, or native creation request flow

#### Scenario: Item state changes at runtime

- **WHEN** an item is equipped, unequipped, stored, or dropped
- **THEN** runtime code MUST continue to use `ItemHelper` or approved systems
- **AND** `ItemSpecBuilder` MUST NOT execute inventory state transitions

### Requirement: Runtime workflow and native execution MUST remain separate

Unit/Item authoring builders MUST NOT directly execute War3 native calls or own long-running runtime semantics.

#### Scenario: Builder applies a template

- **WHEN** `BuildTo(entity)` is called from a template
- **THEN** it MAY write components, tags, and authoring data needed by systems
- **AND** it MUST NOT call `JassApi`, `KKApi`, `YDApi`, `DzApi`, or other War3 native APIs
- **AND** it MUST NOT start lifecycle, movement, equipment, or item-use workflows directly

### Requirement: Existing templates MUST have a migration path

Existing direct component template authoring MUST remain buildable during the first migration slice.

#### Scenario: Old template still writes components directly

- **WHEN** a template has not yet migrated to `UnitSpecBuilder` or `ItemSpecBuilder`
- **THEN** it SHOULD continue to build during the first implementation slice
- **AND** new examples SHOULD prefer the new SpecBuilder style

### Requirement: Example templates MUST demonstrate the intended boundary

Representative examples MUST show the difference between template authoring and runtime helper usage.

#### Scenario: Test templates are migrated

- **WHEN** `Projects/test/Scripts/Template/Unit.cs` and `Projects/test/Scripts/Template/Item.cs` are migrated
- **THEN** unit/item template configuration SHOULD use `UnitSpecBuilder` / `ItemSpecBuilder`
- **AND** examples MUST NOT show builders spawning runtime units or equipping/dropping runtime items

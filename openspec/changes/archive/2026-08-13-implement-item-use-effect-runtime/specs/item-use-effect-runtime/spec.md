## ADDED Requirements

### Requirement: Item UseEffect MUST use a thin request-driven handoff
`ItemSpecBuilder.UseEffect(...)` authored data MUST be executable through an ECS `ItemUseRequest`。ItemUse SHALL only validate the request、create an item-origin root Effect from a runtime EffectSpec snapshot、and remove the request entity。

#### Scenario: Gameplay requests a self-target item effect
- **WHEN** gameplay calls `ItemHelper.RequestUse(user, item)`
- **THEN** a request entity MUST be created
- **AND** the ItemUse system MUST attempt an item-origin EffectSpec handoff

### Requirement: Item use requests MUST contain only identity and target intent
`ItemUseRequest` MUST contain user and item。Target intent MUST be represented separately as `None`、`Unit` or `Point`。The capability SHALL NOT require a request token、receipt or network identity。

#### Scenario: Request is created without UI
- **WHEN** gameplay creates a request directly through ItemHelper
- **THEN** no Inventory UI or synchronized ingress MUST be required

### Requirement: ItemUse MUST revalidate basic item state
At processing time the system MUST require live user/item entities、`ItemBase`、matching `ItemOwner`、inventory or equipped state、`isUsable=true` and a non-null UseEffect-only behavior。It MUST NOT read or mutate consumable stack semantics。

#### Scenario: Item owner changes before processing
- **WHEN** item owner no longer equals request user
- **THEN** the request MUST be removed
- **AND** no root Effect or item mutation SHALL occur

### Requirement: Target intent MUST normalize explicitly
`None` MUST normalize to user plus the user's current Position；`Unit` MUST require a live target with Position；`Point` MUST require finite bounded coordinates。Missing required unit/position context MUST end the request without creating a root Effect。

#### Scenario: Self-target Effect is requested
- **WHEN** a valid `None` target request is processed
- **THEN** root targetUnit MUST equal user
- **AND** point context MUST equal the user's processing-time Position

### Requirement: Item-origin Effects MUST NOT impersonate abilities
Root Effects MUST carry `ItemEffectOrigin { item, user }`。EffectSource ability MUST remain empty，and the item entity SHALL NOT be written into an ability field。

#### Scenario: Item-origin formula reads owner attribute
- **WHEN** EffectSpec uses supported `owner.attr.final`
- **THEN** formula resolution MUST map owner to the ItemEffectOrigin user

### Requirement: Item-origin EffectSpec execution MUST be bounded and detached
Before root Effect creation，the system MUST recursively validate active EffectValueSpec fields and create a deep runtime snapshot。Traversal MUST reject cycles、excessive depth/steps/parameters、unset values and unsupported source-ability formulas。The snapshot MUST detach nested specs、lists、dictionaries、formula parameters and GroundArea data from authoring objects。

#### Scenario: Nested spec contains unsupported formula
- **WHEN** a nested arrive or GroundArea value requires ability stat context
- **THEN** the request MUST be removed without creating a root Effect

### Requirement: Item-origin context MUST propagate through derived Effects
ItemEffectOrigin and detached context MUST propagate to child、arrive、GroundArea and reaction-derived entities。

#### Scenario: GroundArea outlives request processing
- **WHEN** a valid GroundArea root Effect creates a derived area
- **THEN** the area MUST retain item/user origin independent of the deleted request entity

### Requirement: ItemUse MUST NOT own consumption or outcomes
ItemUse SHALL NOT decrement stack、change owner/slot/tags、create ItemRemoveRequest、delete the item、write committed/rejected outcome、maintain replay state or rate-limit requests。Every request is an independent fire-and-forget execution intent。

#### Scenario: Consumable item executes UseEffect
- **WHEN** a valid item with `isConsumable=true` creates a root Effect
- **THEN** ItemBase stack MUST remain unchanged
- **AND** no removal state or outcome component SHALL be created

#### Scenario: Two identical requests are created
- **WHEN** two valid request entities are processed
- **THEN** each request MAY create its own root Effect
- **AND** no idempotency guarantee SHALL be implied

### Requirement: ItemUseAbility runtime MUST remain unsupported
ItemUse SHALL NOT create or invoke `CastRequest`。Items with Ability-only or ambiguous UseAbility/UseEffect runtime data MUST not execute either behavior。Builder validation MAY reject simultaneous authoring eagerly。

#### Scenario: Item contains both use behaviors
- **WHEN** ItemUse processes ambiguous behavior data
- **THEN** the request MUST be removed without creating Effect or CastRequest

### Requirement: Invalid requests MUST be silently consumed
Success and failure MUST both remove the original request entity。This revision SHALL NOT expose stable rejection reasons or committed outcomes。Invalid requests MUST remain side-effect free except for request cleanup。

#### Scenario: Invalid target is supplied
- **WHEN** target intent cannot be normalized
- **THEN** the request entity MUST be removed
- **AND** no root Effect or item mutation SHALL occur

### Requirement: ItemUse MUST preserve Native/Execution boundaries
ItemUse、ItemHelper、Builder and semantic Effect resolvers SHALL NOT directly call War3 native APIs。Visual/native side effects MUST remain in existing Native/Execution systems。

#### Scenario: Item Effect includes visual intent
- **WHEN** root Effect contains an EffectVisual step
- **THEN** ItemUse MUST only create ECS Effect intent
- **AND** the existing Native system MUST execute the native visual operation

### Requirement: Thin ItemUse MUST provide independent client verification
`Projects/test` MUST provide a repeatable direct ECS/War3 client scenario without Inventory UI sync、xUnit project or production Store test seam。The scenario MUST observe root Effect/item-origin state and prove item state remains unchanged。

#### Scenario: Revision 2 is ready for acceptance
- **WHEN** implementation claims completion
- **THEN** War3Frame and Projects/test MUST build
- **AND** the direct client scenario MUST cover success、invalid requests、snapshot/origin propagation and item-state immutability
- **AND** non-Native changed files MUST pass direct native-call scanning

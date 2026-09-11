## 1. Spec and design approval

- [x] 1.1 Review current `UnitHelper`, `ItemHelper`, `Unit.cs`, and `Item.cs`
- [x] 1.2 Confirm scope is `full` because it introduces public authoring API and updates examples
- [x] 1.3 Define the final boundary: `SpecBuilder` for templates, `Helper` for runtime entry, `System` for workflow, `NativeSystem` for War3 native
- [x] 1.4 Block runtime implementation until user approval

## 2. Unit authoring model

- [ ] 2.1 Add `UnitSpec` / `UnitSpecData` / `UnitSpecBuilder`
- [ ] 2.2 Support unit identity: template id and display name
- [ ] 2.3 Support base attributes through named `Attr(...)` methods or a shared attribute spec
- [ ] 2.4 Support item slot declaration without moving runtime inventory logic into builder
- [ ] 2.5 Support ability template references without directly creating runtime ability entities in builder
- [ ] 2.6 Add concise Chinese comments for new public types and methods

## 3. Item authoring model

- [ ] 3.1 Add `ItemSpec` / `ItemSpecData` / `ItemSpecBuilder`
- [ ] 3.2 Support item identity, stack count, max stack, usable/consumable/instantiate flags
- [ ] 3.3 Support equipment attribute contributions
- [ ] 3.4 Support `UseAbility(...)` for active item behavior through ability template references
- [ ] 3.5 Decide and implement minimal `UseEffect(...)` only if it remains clearly one-shot and non-runtime-flow owning
- [ ] 3.6 Add concise Chinese comments for new public types and methods

## 4. Example migration

- [ ] 4.1 Migrate `FootmanTemplate` to `UnitSpecBuilder`
- [ ] 4.2 Migrate `AmuletOfVigorTemplate` to `ItemSpecBuilder`
- [ ] 4.3 Migrate `ScrollFireballTemplate` to `ItemSpecBuilder`, preferring `UseAbility(...)` for active behavior
- [ ] 4.4 Keep old direct component authoring buildable during the migration

## 5. Boundary checks

- [ ] 5.1 Verify `UnitSpecBuilder` does not call `UnitHelper.CreateUnit(...)` or create runtime world entities
- [ ] 5.2 Verify `ItemSpecBuilder` does not call `ItemHelper.EquipToUnit(...)`, `UnequipToInventory(...)`, or `DropToGround(...)`
- [ ] 5.3 Verify builders do not call War3 native APIs
- [ ] 5.4 Verify runtime helper responsibilities remain documented and intact

## 6. Verification

- [ ] 6.1 Build `War3Frame/War3Frame.csproj`
- [ ] 6.2 Build `Projects/test/test.csproj`
- [ ] 6.3 Summarize migration status, compatibility risks, and follow-up proposals

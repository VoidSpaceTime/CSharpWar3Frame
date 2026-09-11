## 1. Architecture definition

- [ ] 1.1 Define the sync registry/list as the owner of static sync declarations
- [ ] 1.2 Define the per-unit snapshot as the owner of runtime baselines
- [ ] 1.3 Define the iteration contract for `UnitNativeSystem`

## 2. Migration planning

- [ ] 2.1 Migrate current Health/Mana sync rules into the registry/list
- [ ] 2.2 Replace handwritten snapshot fields with a scalable per-unit baseline structure
- [ ] 2.3 Replace handwritten native sync branches with registry-driven iteration

## 3. Verification planning

- [ ] 3.1 Verify Health compare-sync semantics remain unchanged
- [ ] 3.2 Verify Mana compare-sync semantics remain unchanged
- [ ] 3.3 Verify no redundant native writes occur when values do not change
- [ ] 3.4 Verify per-unit baselines remain isolated across multiple units

## 4. Approval gating

- [ ] 4.1 Block implementation until proposal, design, spec, and tasks review is approved
- [ ] 4.2 Forbid global registry/list from storing mutable per-unit state

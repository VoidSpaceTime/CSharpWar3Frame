## 1. Architecture definition

- [ ] 1.1 Define `AbilityBase` as identity/minimal-semantics only
- [ ] 1.2 Lock the initial whitelist to `level` and `targetType`
- [ ] 1.3 Define the blacklist of tunable numeric, runtime-state, and behavior-structure fields
- [ ] 1.4 Restate `AbilityAttribute` as canonical numeric ownership

## 2. TDD-oriented verification planning

- [ ] 2.1 Define red-first tests proving common systems only require minimal semantics from `AbilityBase`
- [ ] 2.2 Define red-first tests proving numeric readers no longer depend on `AbilityBase`
- [ ] 2.3 Define red-first tests proving cooldown remaining is runtime-state-owned
- [ ] 2.4 Define red-first tests proving projectile/damage/heal/buff structure remains component-owned
- [ ] 2.5 Define regression tests proving no new numeric bucket behavior is reintroduced into `AbilityBase`

## 3. Inventory and migration planning

- [ ] 3.1 Inventory current `AbilityBase` fields and all readers/writers
- [ ] 3.2 Classify each field as whitelist / numeric / runtime-state / structure
- [ ] 3.3 Define vertical slices for migrating readers to canonical owners
- [ ] 3.4 Define compatibility or bridge rules for any temporary transition state

## 4. Runtime migration sequencing

- [ ] 4.1 Migrate numeric readers to `AbilityAttribute`
- [ ] 4.2 Migrate cooldown/channel/charges readers to runtime state components
- [ ] 4.3 Migrate projectile/damage/heal/buff structure readers to dedicated components
- [ ] 4.4 Remove blacklisted fields from `AbilityBase` after coverage passes

## 5. Cross-project impact review

- [ ] 5.1 Document direct runtime impact within `War3Frame`
- [ ] 5.2 Review `War3Frame.Generator` for generated template or registration implications
- [ ] 5.3 Review `FrameBuild` for build-time assumptions
- [ ] 5.4 Review `CSharpWar3Frame` for CLI/tooling assumptions
- [ ] 5.5 Review `Projects/*` for authored template breakage, UI assumptions, and migration examples

## 6. Approval and execution gating

- [ ] 6.1 Block implementation until proposal, design, spec, and tasks review is approved
- [ ] 6.2 Require TDD-first execution
- [ ] 6.3 Forbid reintroducing tunable numeric ownership into `AbilityBase`
- [ ] 6.4 Forbid reintroducing runtime-state ownership into `AbilityBase`
- [ ] 6.5 Forbid reintroducing projectile/damage/heal/buff structure ownership into `AbilityBase`

## 7. Atomic commit strategy

- [ ] 7.1 Plan one OpenSpec-only proposal commit
- [ ] 7.2 Plan one red-tests-only commit
- [ ] 7.3 Plan one `AbilityBase` inventory/classification commit
- [ ] 7.4 Plan one numeric reader migration commit
- [ ] 7.5 Plan one runtime-state reader migration commit
- [ ] 7.6 Plan one structure-reader migration commit
- [ ] 7.7 Plan one final `AbilityBase` slimming commit after verification passes

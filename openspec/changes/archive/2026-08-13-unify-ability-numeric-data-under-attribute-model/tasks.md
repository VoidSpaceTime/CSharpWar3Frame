## 1. Architecture definition

- [ ] 1.1 Define the boundary between ability numeric ownership and ability behavior structure
- [ ] 1.2 Define the target set of ability numeric values that move onto the attribute/value/modifier model
- [ ] 1.3 Define the authoritative ownership model that replaces `AbilityStat*`
- [ ] 1.4 Restate that ability templates remain level-aware
- [ ] 1.5 Restate that unit/item templates keep their own non-ability signatures

## 2. TDD-oriented verification planning

- [ ] 2.1 Define red-first tests for template-authored level-aware numeric values
- [ ] 2.2 Define red-first tests for runtime cost reads from attribute-backed ability numbers
- [ ] 2.3 Define red-first tests for cooldown and cast-range reads from attribute-backed ability numbers
- [ ] 2.4 Define red-first tests for damage/heal/radius/projectile-speed/duration reads from attribute-backed ability numbers
- [ ] 2.5 Define red-first tests for modifier application to ability-owned numeric values
- [ ] 2.6 Define regression tests proving behavior structure remains component-owned
- [ ] 2.7 Define regression tests proving unit/item templates were not forced into ability-like level semantics

## 3. Numeric mapping plan

- [ ] 3.1 Inventory existing ability numeric fields across `AbilityBase`, `AbilityCost`, `AbilityEffect`, projectile, and area-search components
- [ ] 3.2 Classify each field as numeric-owned vs structure-owned
- [ ] 3.3 Define target attribute identifiers and naming conventions for moved ability numbers
- [ ] 3.4 Define access helpers for ability numeric lookup and mutation
- [ ] 3.5 Define compatibility or bridge rules for any temporary dual-read phase

## 4. Runtime migration sequencing

- [ ] 4.1 Migrate template authoring to attribute-backed base numeric values
- [ ] 4.2 Migrate cost evaluation and deduction paths
- [ ] 4.3 Migrate cooldown and cast-range consumers
- [ ] 4.4 Migrate damage/heal consumers
- [ ] 4.5 Migrate area/projectile numeric consumers
- [ ] 4.6 Remove legacy `AbilityStat*` ownership after coverage passes

## 5. Cross-project impact review

- [ ] 5.1 Document direct runtime impact within `War3Frame`
- [ ] 5.2 Review `War3Frame.Generator` for template-signature or generated-registration implications
- [ ] 5.3 Review `FrameBuild` for build-time assumptions or asset-generation implications
- [ ] 5.4 Review `CSharpWar3Frame` for CLI/tooling assumptions
- [ ] 5.5 Review `Projects/*` for authored template breakage, test fixtures, and migration examples

## 6. Approval and execution gating

- [ ] 6.1 Block implementation until proposal, design, spec, and tasks review is approved
- [ ] 6.2 Require TDD-first execution
- [ ] 6.3 Forbid mixing behavior structure with numeric attribute ownership
- [ ] 6.4 Forbid forcing unit/item templates into ability-like level semantics
- [ ] 6.5 Forbid long-term dual ownership between attribute-backed values and `AbilityStat*`

## 7. Atomic commit strategy

- [ ] 7.1 Plan one OpenSpec-only proposal commit
- [ ] 7.2 Plan one red-tests-only commit for target behavior coverage
- [ ] 7.3 Plan one numeric classification plus helper foundation commit
- [ ] 7.4 Plan one template-authoring migration commit
- [ ] 7.5 Plan one runtime-reader migration commit
- [ ] 7.6 Plan one legacy `AbilityStat*` removal commit after verification passes

## 1. Architecture definition

- [ ] 1.1 Define compare-sync ownership for `Health` and `Mana`
- [ ] 1.2 Define the minimal snapshot component responsibility and limits
- [ ] 1.3 Define `Position` as a separate native-observe path
- [ ] 1.4 Restate that `Death/Remove` remain owned by the A-plan lifecycle architecture

## 2. TDD-oriented verification planning

- [ ] 2.1 Define red-first tests for `Health` compare-sync
- [ ] 2.2 Define red-first tests for `Mana` compare-sync
- [ ] 2.3 Define red-first tests for snapshot initialization and baseline update behavior
- [ ] 2.4 Define red-first tests proving `Position` remains outside compare-sync ownership
- [ ] 2.5 Define red-first tests proving `Death/Remove` remain lifecycle-driven
- [ ] 2.6 Define regression tests proving no fallback to dirty-flag primary ownership

## 3. Implementation sequencing

- [ ] 3.1 Add snapshot component and compare helpers
- [ ] 3.2 Convert `Health` to compare-sync
- [ ] 3.3 Convert `Mana` to compare-sync
- [ ] 3.4 Preserve or isolate `Position` native-observe path
- [ ] 3.5 Remove obsolete dirty-sync runtime usage

## 4. Legacy-path exit planning

- [ ] 4.1 Remove runtime dependency on `UnitNativeDirtyFlags`
- [ ] 4.2 Remove runtime dependency on `UnitNativeDirtyHelper`
- [ ] 4.3 Remove `UnitNativeDirty` component usage after compare-sync validation passes
- [ ] 4.4 Define explicit shim removal conditions if any temporary bridge is introduced

## 5. Cross-project impact review

- [ ] 5.1 Document runtime impact within `War3Frame`
- [ ] 5.2 Document explicit non-impact or deferred impact for `War3Frame.Generator`
- [ ] 5.3 Document explicit non-impact or deferred impact for `FrameBuild`
- [ ] 5.4 Document explicit non-impact or deferred impact for `CSharpWar3Frame`
- [ ] 5.5 Define validation expectations for `Projects/*`

## 6. Approval and execution gating

- [ ] 6.1 Block implementation until proposal, design, spec, and tasks review is approved
- [ ] 6.2 Require TDD-first execution
- [ ] 6.3 Forbid reintroducing dirty-flag primary ownership
- [ ] 6.4 Forbid mixing lifecycle actions with continuous sync fields

## 7. Atomic commit strategy

- [ ] 7.1 Plan one OpenSpec-only review commit
- [ ] 7.2 Plan one red-tests-only commit
- [ ] 7.3 Plan one snapshot foundation commit
- [ ] 7.4 Plan one `Health/Mana` compare-sync commit
- [ ] 7.5 Plan one `Position` separation commit if needed
- [ ] 7.6 Plan one legacy dirty-path removal commit after verification passes

## 1. Architecture definition

- [ ] 1.1 Confirm the canonical projectile runtime path and explicitly document legacy freeze vs adapter policy
- [ ] 1.2 Define the shared projectile core contract and family-specific movement component boundaries
- [ ] 1.3 Define `ProjectileSpaceMode` / `ProjectileMovementMode` scope for Phase 1
- [ ] 1.4 Define cubic / quartic bezier control-point topology requirements
- [ ] 1.5 Lock Phase 1 bezier control-point expression to `RelativeToStartEnd`

## 2. Numeric vs structure ownership planning

- [ ] 2.1 Classify movement space / movement family / curve degree as structure-owned semantics
- [ ] 2.2 Classify projectile speed / distance / duration / arrival threshold and new motion tunables against canonical numeric ownership
- [ ] 2.3 Define temporary bridge rules for any motion numeric values that cannot migrate in Phase 1

## 3. Lifecycle and structural-mutation redesign planning

- [ ] 3.1 Define the canonical projectile lifecycle progression states/events for in-flight, arrived, expired, and cleanup
- [ ] 3.2 Define the safe mutation pattern that replaces direct `AddTag/RemoveTag/DeleteEntity` inside projectile query loops
- [ ] 3.3 Compare “query-outside apply” vs “request/state + consumer system” and choose one canonical pattern for projectile runtime
- [ ] 3.4 Document one-time arrival gating rules so downstream effects are consumed exactly once

## 4. Legacy-path handling

- [ ] 4.1 Document why `AbilityEffectExtend` is not the canonical expansion path
- [ ] 4.2 Define the long-lived Hook Bridge Layer role for `ProjectileBase` / `IProjectileOn*`
- [ ] 4.3 Define the frozen role of `IProjectileOnStart/Travel/Arrive` and prohibit new movement-family ownership there
- [ ] 4.4 Define bridge/dispatch responsibilities between canonical projectile lifecycle and legacy hooks
- [ ] 4.5 Define whether and how a new `IProjectileHooksV2` contract coexists with legacy hooks
- [ ] 4.6 Define the canonical mapping from legacy `OnTravel(bool)` to explicit V2 travel decisions

## 5. Canonical movement-family planning

- [ ] 5.1 Define planar projectile semantics and how they use `Position`
- [ ] 5.2 Define 3D projectile semantics and the target/anchor data boundary
- [ ] 5.3 Define homing / directional movement-family semantics on the canonical path
- [ ] 5.4 Define bezier movement semantics and degree-specific control-point requirements
- [ ] 5.5 Define `EffectTargetInfo` boundary and prohibit curve-offset ownership there
- [ ] 5.6 Define snake movement semantics and required structure/tunable fields
- [ ] 5.7 Define orbit movement semantics and required structure/tunable fields

## 6. TDD-oriented verification planning

- [ ] 6.1 Define scenario coverage for fixed-point planar projectile arrival
- [ ] 6.2 Define scenario coverage for moving-target homing refresh
- [ ] 6.3 Define scenario coverage for cubic bezier path traversal
- [ ] 6.4 Define scenario coverage for quartic bezier path traversal
- [ ] 6.5 Define scenario coverage for snake movement oscillation while preserving canonical `Position` truth
- [ ] 6.6 Define scenario coverage for orbit movement around its anchor/center semantics
- [ ] 6.7 Define regression scenarios proving structural changes are not performed inside projectile query loops
- [ ] 6.8 Define regression scenarios proving arrival is emitted exactly once and downstream payload is gated exactly once
- [ ] 6.9 Define regression scenarios proving legacy hooks cannot seize lifecycle truth from the canonical runtime path
- [ ] 6.10 Define regression scenarios for legacy-hook and V2-hook coexistence if V2 is introduced
- [ ] 6.11 Define regression scenarios for `SuppressArrivalThisTick` behavior without stopping canonical movement progression
- [ ] 6.12 Define regression scenarios for `RequestExpire` flowing through canonical cleanup rather than direct hook-side structural mutation

## 7. Cross-project impact review

- [ ] 7.1 Document direct runtime impact within `War3Frame`
- [ ] 7.2 Document explicit non-impact or deferred impact for `War3Frame.Generator`
- [ ] 7.3 Document explicit non-impact or deferred impact for `FrameBuild`
- [ ] 7.4 Document explicit non-impact or deferred impact for `CSharpWar3Frame`
- [ ] 7.5 Document `Projects/test` / `Projects/demo` validation and authoring migration expectations

## 8. Approval and execution gating

- [ ] 8.1 Block implementation until proposal, design, tasks, and spec review are approved
- [ ] 8.2 Require Phase 1 implementation to stay on the canonical projectile path and not expand legacy path feature scope
- [ ] 8.3 Require live-path verification before runtime implementation begins

## 9. Atomic commit strategy

- [ ] 9.1 Plan one review-only commit for projectile architecture OpenSpec artifacts
- [ ] 9.2 Plan one test-first commit for canonical projectile lifecycle/movement scenarios
- [ ] 9.3 Plan one implementation commit for canonical projectile core/lifecycle changes
- [ ] 9.4 Plan one implementation commit for movement-family contract/runtime support
- [ ] 9.5 Plan one implementation commit for Hook Bridge Layer / hook-dispatch integration
- [ ] 9.6 Plan one optional implementation commit for `IProjectileHooksV2` introduction if approved

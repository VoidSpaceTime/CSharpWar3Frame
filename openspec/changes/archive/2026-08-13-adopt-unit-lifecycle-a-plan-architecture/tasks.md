## 1. Architecture definition

- [ ] 1.1 Define the canonical A-plan ownership model around `UnitLifeState`, `UnitLifecycleTransitionSystem`, `UnitLifecycleNativeEffectSystem`, and `UnitLifecycleDisposeSystem`
- [ ] 1.2 Define legal lifecycle progression boundaries for death, corpse retention, clear-corpse progression, and terminal removal
- [ ] 1.3 Document the separation between lifecycle truth, phase progression, native side effects, and terminal ECS cleanup

## 2. Legacy-path exit planning

- [ ] 2.1 Document how `CorpseCleanupSystem` leaves the primary lifecycle path
- [ ] 2.2 Document how `CorpseExpired` leaves the primary lifecycle path
- [ ] 2.3 Document how `Death` dirty flags leave the primary lifecycle path
- [ ] 2.4 Document how `Remove` dirty flags leave the primary lifecycle path
- [ ] 2.5 Define temporary compatibility shims and explicit removal conditions

## 3. TDD-oriented verification planning

- [ ] 3.1 Define red-first scenario coverage for `Alive -> Death -> Corpse`
- [ ] 3.2 Define red-first scenario coverage for corpse-retention expiry advancing only to `ClearCorpse`
- [ ] 3.3 Define red-first scenario coverage for lifecycle-driven native death and native remove side effects
- [ ] 3.4 Define red-first scenario coverage for terminal ECS cleanup being owned only by `UnitLifecycleDisposeSystem`
- [ ] 3.5 Define regression scenarios proving `TimerTaskSystem` no longer owns disposal behavior
- [ ] 3.6 Define regression scenarios proving `UnitHelper` no longer owns patchwork lifecycle orchestration
- [ ] 3.7 Define regression scenarios for direct remove under the canonical lifecycle path

## 4. Cross-project impact review

- [ ] 4.1 Document direct runtime impact within `War3Frame`
- [ ] 4.2 Document explicit non-impact or deferred impact for `War3Frame.Generator`
- [ ] 4.3 Document explicit non-impact or deferred impact for `FrameBuild`
- [ ] 4.4 Document explicit non-impact or deferred impact for `CSharpWar3Frame`
- [ ] 4.5 Document validation expectations for `Projects/demo` and `Projects/test`

## 5. Approval and execution gating

- [ ] 5.1 Mark implementation as blocked until proposal, design, spec, and tasks review is approved
- [ ] 5.2 Require implementation to preserve canonical A-plan ownership and not drift back to helper-driven, timer-driven, cleanup-tag-driven, or dirty-flag-driven lifecycle behavior
- [ ] 5.3 Require legacy-path removal only after TDD-oriented verification passes

## 6. Atomic commit strategy

- [ ] 6.1 Plan one review-only commit for OpenSpec artifacts
- [ ] 6.2 Plan one test-first commit for lifecycle scenario coverage
- [ ] 6.3 Plan one implementation commit for lifecycle transition ownership
- [ ] 6.4 Plan one implementation commit for lifecycle-native side-effect ownership
- [ ] 6.5 Plan one implementation commit for terminal ECS disposal ownership
- [ ] 6.6 Plan one cleanup commit for removing the legacy corpse-cleanup and dirty-flag lifecycle path after verification passes

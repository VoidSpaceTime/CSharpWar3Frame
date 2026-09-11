## 1. Spec and test planning

- [ ] 1.1 Map each approved ownership scenario to a concrete test target
- [ ] 1.2 Define unit and integration coverage for attachment, lifetime, appearance, animation, and projectile visuals
- [ ] 1.3 Define verification for sustained `0.02s` cadence behavior

## 2. Failing tests first

- [ ] 2.1 Add red tests proving ECS remains authoritative when native handles are missing or recreated
- [ ] 2.2 Add red tests proving movable effects use `Position` as the only motion truth
- [ ] 2.3 Add red tests proving helpers do not own persistent effect truth
- [ ] 2.4 Add red tests proving attached sustained effects reconcile from ECS-owned attachment intent
- [ ] 2.5 Add red tests proving lifetime semantics are ECS-visible
- [ ] 2.6 Add red tests proving animation requests are consumable ECS-visible intents
- [ ] 2.7 Add red tests proving projectile visuals follow projectile ECS state through movement and arrival
- [ ] 2.8 Add red tests proving sustained reconciliation runs at `0.02s`

## 3. Core model alignment

- [ ] 3.1 Introduce or refine ECS components for attachment, lifetime, appearance, animation requests, and native spawn/despawn intent
- [ ] 3.2 Align existing effect state with ECS-owned long-lived semantics
- [ ] 3.3 Preserve helper ergonomics while removing helper ownership of sustained truth

## 4. Native execution separation

- [ ] 4.1 Refactor native effect systems so native handles are execution-only resources
- [ ] 4.2 Route appearance and animation application through native execution systems
- [ ] 4.3 Ensure effects can be recreated or rebound from ECS state alone
- [ ] 4.4 Remove or isolate direct native truth mutations that bypass ECS ownership

## 5. Sustained 0.02s reconciliation

- [ ] 5.1 Add a sustained effect update path running at `0.02s`
- [ ] 5.2 Reconcile attached follow behavior through that sustained path
- [ ] 5.3 Reconcile movable positional effects through ECS `Position`
- [ ] 5.4 Reconcile timed lifetime progression and expiry cleanup through ECS-owned lifetime state

## 6. Projectile visual alignment

- [ ] 6.1 Formalize projectile visuals as derived from projectile ECS state
- [ ] 6.2 Align projectile visual movement with projectile `Position`
- [ ] 6.3 Align projectile visual cleanup with ECS arrival semantics
- [ ] 6.4 Ensure no second projectile motion truth is introduced

## 7. Cross-project impact review

- [ ] 7.1 Document direct runtime impact within `War3Frame`
- [ ] 7.2 Review `War3Frame.Generator` for template-signature or generated-registration implications
- [ ] 7.3 Review `FrameBuild` for build-time assumptions or asset-generation implications
- [ ] 7.4 Review `CSharpWar3Frame` for CLI/tooling assumptions
- [ ] 7.5 Review `Projects/*` for authored effect breakage, test fixtures, and migration examples

## 8. Approval and execution gating

- [ ] 8.1 Block implementation until proposal, design, spec, and tasks review is approved
- [ ] 8.2 Require TDD-first execution
- [ ] 8.3 Forbid helper-owned sustained truth
- [ ] 8.4 Forbid native/Lua-owned semantic truth
- [ ] 8.5 Forbid introducing a second position-truth model

## 9. Atomic commit strategy

- [ ] 9.1 Plan one OpenSpec-only proposal commit
- [ ] 9.2 Plan one red-tests-only commit for target behavior coverage
- [ ] 9.3 Plan one effect-state alignment commit
- [ ] 9.4 Plan one native execution-separation commit
- [ ] 9.5 Plan one sustained `0.02s` reconciliation commit
- [ ] 9.6 Plan one projectile-visual alignment commit
- [ ] 9.7 Plan one helper cleanup commit after verification passes

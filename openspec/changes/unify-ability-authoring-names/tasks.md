## 1. Spec and design approval

- [x] 1.1 Review `proposal.md`, `design.md`, `tasks.md`, and `specs/ability-authoring-model/spec.md`
- [x] 1.2 Confirm scope remains `full`, not `architecture`
- [x] 1.3 Confirm naming direction: `AbilitySpec` / `AbilityBehavior` / `AbilityEffect` / `AbilityValue`
- [x] 1.4 Block runtime implementation until user approval

## 2. Naming foundation

- [x] 2.1 Define `AbilitySpec` / `AbilitySpecData` / `AbilitySpecBuilder` authoring shape
- [x] 2.2 Define `AbilityEffectSpec` naming or compatibility wrapper for current `EffectSpec`
- [x] 2.3 Define `AbilityValue` and resolver requirements for ability stat and unit attr values
- [x] 2.4 Add concise Chinese comments for new public types

## 3. Behavior foundation

- [x] 3.1 Define `AbilityBehaviorSpec` / `AbilityBehaviorData` / `AbilityBehaviorBuilder`
- [x] 3.2 Implement minimal `OnCast().Do(effect)` behavior path or map it to current cast flow
- [ ] 3.3 Implement lifecycle trigger model for `OnGranted` / `OnRemoved`
- [ ] 3.4 Ensure behavior systems emit ECS requests/effects and do not call War3 native

## 4. Passive and periodic behavior slice

- [ ] 4.1 Add repeat behavior driven by `AbilityValue` interval
- [ ] 4.2 Add circle search and target filter/metric selection needed by `治疗之鸟`
- [ ] 4.3 Add projectile wait/on-arrive behavior integration
- [ ] 4.4 Add healing by target attribute value through unified effect/value path

## 5. Examples and migration

- [x] 5.1 Migrate one existing active example to new naming
- [ ] 5.2 Add or migrate `治疗之鸟` as a passive/periodic behavior example
- [x] 5.3 Keep existing examples building during migration
- [x] 5.4 Document any old API compatibility names left in place
- [x] 5.5 Add AbilitySpecBuilder.OnCast(...) convenience authoring path for common active skills

## 6. Verification

- [x] 6.1 Build `War3Frame/War3Frame.csproj`
- [x] 6.2 Build `Projects/test/test.csproj`
- [x] 6.3 Verify no direct War3 native calls are introduced outside Native/Execution layer
- [x] 6.4 Verify behavior layer does not directly mutate health/buff truth outside request systems
- [x] 6.5 Summarize migration status, compatibility risks, and follow-up proposals

## 1. Spec and design approval

- [ ] 1.1 Review `proposal.md`, `design.md`, `tasks.md`, and `specs/ground-area-effect-model/spec.md`
- [ ] 1.2 Confirm scope remains `full`, not `architecture`
- [ ] 1.3 Confirm first implementation slice targets `凝固汽油` and `喷火`
- [ ] 1.4 Block runtime implementation until user approval

## 2. Ground area core model

- [ ] 2.1 Add ECS components for ground area identity, source, radius, lifetime, tags, and expiry
- [ ] 2.2 Add lifetime and cleanup systems for ground area entities
- [ ] 2.3 Ensure ground areas use existing `Position` as location truth
- [ ] 2.4 Add concise Chinese comments for new public components/systems

## 3. Area buff behavior

- [ ] 3.1 Add data model for ground-area-applied Buff
- [ ] 3.2 Add system that applies Buff to units inside area
- [ ] 3.3 Add system or cleanup path that removes area-owned Buff when units leave or area expires
- [ ] 3.4 Verify `凝固汽油` can apply `MoveSpeed -20` for 10 seconds or until area expiry

## 4. Periodic damage behavior

- [ ] 4.1 Add data model for periodic area damage
- [ ] 4.2 Add system that emits `DamageRequest` on tick rather than directly modifying health
- [ ] 4.3 Verify burning ground emits 10 damage per second for 5 seconds

## 5. Shape search and reaction behavior

- [ ] 5.1 Add line search effect payload or equivalent shape search step using `GroupHelper.FindInLine`
- [ ] 5.2 Add reaction request or hit marker for fire contacting oil ground areas
- [ ] 5.3 Add minimal reaction system for `Oil + Fire -> BurningGround`
- [ ] 5.4 Avoid hardcoding behavior in templates or helpers beyond data configuration

## 6. Ability templates

- [ ] 6.1 Add `napalm_oil` / `凝固汽油` template in `Projects/test/Scripts/Template/Ability.cs`
- [ ] 6.2 Add `flamethrower` / `喷火` template in `Projects/test/Scripts/Template/Ability.cs`
- [ ] 6.3 Ensure templates only configure data and do not own runtime behavior
- [ ] 6.4 Document any placeholder target filters or missing unit-type constraints

## 7. Verification

- [ ] 7.1 Build `War3Frame/War3Frame.csproj`
- [ ] 7.2 Build `Projects/test/test.csproj`
- [ ] 7.3 Verify no direct War3 native calls are introduced outside Native/Execution layer
- [ ] 7.4 Verify ground area systems emit requests/outcomes rather than direct native side effects
- [ ] 7.5 Summarize remaining limitations and follow-up proposals

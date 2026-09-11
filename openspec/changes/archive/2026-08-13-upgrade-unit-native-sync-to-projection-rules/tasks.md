## 1. Registry widening

- [ ] 1.1 Redefine the registry/list as projection-rule-based rather than native-state-based
- [ ] 1.2 Keep per-unit baselines outside the registry/list

## 2. Projection migration planning

- [ ] 2.1 Migrate current Health projection into the new rule form
- [ ] 2.2 Migrate current Mana projection into the new rule form
- [ ] 2.3 Define the first non-`SetUnitState(...)` projection example (e.g. AttackRange)

## 3. Verification planning

- [ ] 3.1 Verify Health behavior remains identical after migration
- [ ] 3.2 Verify Mana behavior remains identical after migration
- [ ] 3.3 Verify a non-standard native write path can be expressed without handwritten top-level branches

## 4. Approval gating

- [ ] 4.1 Block implementation until proposal, design, spec, and tasks review is approved
- [ ] 4.2 Forbid collapsing runtime baselines back into the global registry/list

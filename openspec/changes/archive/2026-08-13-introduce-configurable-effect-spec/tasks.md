# Tasks

## 1. Review

- [ ] 1.1 Confirm `full` level classification and approve this OpenSpec change.
- [ ] 1.2 Confirm first implementation scope: additive spec layer, no generator/build/CLI format changes.
- [ ] 1.3 Decide parameter table shape for first pass: float-only or typed values.

## 2. Data Contracts

- [ ] 2.1 Add `EffectSpec`, `EffectStepSpec`, typed step payloads, and `EffectSpecData`.
- [ ] 2.2 Add `EffectValueSpec` / `EffectFormulaSpec` with `formulaId`, optional `statId`, and parameters.
- [ ] 2.3 Extend damage/heal runtime data or add companion components so settlement can resolve data formulas.

## 3. Builder

- [ ] 3.1 Add `EffectSpecBuilder` for common damage/heal/buff/area/projectile chains.
- [ ] 3.2 Ensure builder output is exactly the same data model used by config/editor input.
- [ ] 3.3 Add examples for ordinary skills that do not use delegates.

## 4. Formula Registry

- [ ] 4.1 Add formula registry keyed by `formulaId`.
- [ ] 4.2 Add built-in formulas for stat final value, constant value, and linear scaling.
- [ ] 4.3 Define missing formula/stat behavior with clear exceptions or documented fallback.
- [ ] 4.4 Keep delegate override support for advanced C# formulas.

## 5. Runtime Integration

- [ ] 5.1 Expand `EffectSpecData` in `AbilityEffectHelper.CreateEffectEntity(...)`.
- [ ] 5.2 Preserve existing direct effect component authoring.
- [ ] 5.3 Update damage/heal settlement to use formula resolution precedence.
- [ ] 5.4 Verify area/projectile compositions keep deterministic behavior within approved scope.

## 6. Validation

- [ ] 6.1 Build `CSharpWar3Frame.slnx`.
- [ ] 6.2 Validate a stat-id damage spec.
- [ ] 6.3 Validate a formula-id + parameter spec.
- [ ] 6.4 Validate delegate override precedence.
- [ ] 6.5 Validate existing component-authored effects still work.

## 7. Summarize

- [ ] 7.1 Summarize actual changed files, validation results, global impact, risks, and follow-up proposals if any.
- [ ] 7.2 Do not create a git commit unless the user explicitly asks.

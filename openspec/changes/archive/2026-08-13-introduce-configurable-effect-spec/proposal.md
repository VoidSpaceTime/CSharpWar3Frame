# Introduce Configurable Effect Spec

- Change ID: `introduce-configurable-effect-spec`
- Level: `full`
- Status: pending review

## Why

Current ability effects are authored mainly by attaching runtime effect components such as `DamageEffectData`, `HealEffectData`, `ApplyBuffData`, `AreaSearchData`, and `ProjectileData` directly to the ability entity. Damage and heal formulas also support delegates (`DamageFormulaFunc`, `HealFormulaFunc`), which is powerful for advanced cases but not friendly to configuration files, editor tooling, validation, preview, or data-driven skill authoring.

This change introduces an editor/configuration-friendly effect spec layer so common skills can be described as data: effect chains, stat ids, formula ids, and formula parameters. Delegates remain available as advanced override hooks, but ordinary skill authoring should not require C# delegate code.

## What

- Add a data-oriented `EffectSpec` model that can describe ordered effect chains.
- Add a builder API that produces the same spec model for C# authoring convenience.
- Add formula references based on `formulaId`, `statId`, and parameter tables for ordinary damage/heal/buff values.
- Add formula registry/resolution infrastructure used by effect settlement systems.
- Keep existing delegate formulas as explicit advanced/custom overrides.
- Bridge specs into existing ECS effect execution by expanding effect specs into effect entities/components at cast time.
- Provide migration examples for a normal damage/heal/buff/projectile chain.

## Global Impact

- `War3Frame/`: affected. Runtime components, helpers, and ability effect systems need new data contracts and resolution paths.
- `War3Frame.Generator/`: not directly affected in the first implementation unless templates/generated ability authoring later emit effect specs. The proposal will avoid generator output changes.
- `FrameBuild/`: not affected. No build orchestration or asset pipeline changes are required.
- `CSharpWar3Frame/`: not affected. CLI entry behavior and configuration file formats are not changed in this step.
- `Projects/`: affected only for examples/tests that demonstrate ordinary effect specs. Existing project behavior should remain compatible.

Because this changes public ability effect authoring contracts and runtime effect settlement semantics, it is `full` level.

## Native Call Layering

No new War3 native calls are proposed. The change remains in ECS/runtime authoring and settlement. Projectile visuals continue to use the existing native/effect separation already owned by the current `EffectHelper` / `EffectNativeSystem` path.

## Risks

- Public contract drift: existing direct component authoring must continue to work.
- Formula resolution ambiguity: missing formula ids or stat ids must fail predictably or fall back only where explicitly documented.
- Chain ordering bugs: ordered specs must settle deterministically with projectile/area/search behavior.
- Data/editor friendliness can be undermined if delegate paths remain the default examples.

## Validation

- Build `CSharpWar3Frame.slnx`.
- Add focused tests or demo checks for:
  - `statId` value resolution.
  - formula id + parameter table resolution.
  - delegate override precedence.
  - ordered damage/heal/buff chain expansion.
  - compatibility with existing component-authored ability effects.

## Acceptance Criteria

- A normal skill can define its effect chain without a delegate.
- Damage/heal formulas can be expressed through `formulaId`, optional `statId`, and parameters.
- Delegates still work for advanced custom formulas and have clearly documented precedence.
- Existing direct `DamageEffectData`, `HealEffectData`, `ApplyBuffData`, `AreaSearchData`, and `ProjectileData` authoring remains compatible.
- The implementation introduces no new War3 native calls outside approved native/execution layers.

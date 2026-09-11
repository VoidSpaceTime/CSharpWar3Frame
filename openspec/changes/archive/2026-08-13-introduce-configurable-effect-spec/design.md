# Design

## Current State

Ability effect execution currently copies effect components from an ability entity to a runtime effect entity in `AbilityEffectHelper.CreateEffectEntity(...)`. Settlement systems then process runtime components:

- `DamageEffectData` uses `damageFunc` when present, otherwise `AbilityHelper.GetDamageAmount(...)`.
- `HealEffectData` uses `healFunc` when present, otherwise `amount` or `AbilityHelper.GetHealAmount(...)`.
- `ApplyBuffData`, `AreaSearchData`, and `ProjectileData` are already mostly data-shaped but still authored as ECS components.

This is convenient for C# code but awkward for configuration and editor tooling because ordinary formulas are executable delegates rather than data references.

## Proposed Model

Introduce a spec layer under `War3Frame/Src/Components/Ability/` or a nearby ability-effect namespace:

- `EffectSpec`: root ordered chain.
- `EffectStepSpec`: one step in the chain with a kind and typed payload.
- `EffectValueSpec`: data reference for numeric values.
- `EffectFormulaSpec`: `formulaId`, optional `statId`, and parameter table.
- `EffectSpecData`: ECS component attached to ability entities.
- `EffectSpecBuilder`: C# convenience API that creates the same data model.

The data model should be serializable/editor-friendly: primitive fields, string ids, enum ids, stat ids, and parameter dictionaries/lists. It should not require delegates for ordinary authoring.

## Formula Resolution

Add a formula registry used at settlement time:

- Built-in formulas:
  - `stat.final`: returns `AbilityHelper.GetFinalValue(ability, statId)`.
  - `constant`: returns a provided parameter value.
  - `linear`: `base + stat * scale + bonus`.
- Custom formula registration:
  - keyed by `formulaId`.
  - receives caster, ability, target, value spec, and effect context.

Delegate formulas remain in `DamageEffectData.damageFunc` and `HealEffectData.healFunc`. Precedence:

1. Explicit delegate override, for advanced C# customization.
2. Formula id resolution from spec/component.
3. Direct amount field.
4. Existing default stat helper fallback (`DamageAmount`, `HealAmount`) where compatibility requires it.

## Effect Chain Expansion

When an ability has `EffectSpecData`, `AbilityEffectHelper.CreateEffectEntity(...)` should expand the spec into one or more runtime effect entities/components. The first implementation should keep the current settlement pipeline and component systems rather than replacing them.

Recommended mapping:

- Damage step -> `DamageEffectData` with a data value/formula reference.
- Heal step -> `HealEffectData` with a data value/formula reference.
- Buff step -> `ApplyBuffData`.
- Area step -> `AreaSearchData` plus child-effect propagation.
- Projectile step -> `ProjectileData` and existing projectile lifecycle.

If existing systems cannot preserve ordered semantics for all chains, the implementation should start with deterministic sequential groups and document unsupported compositions instead of pretending every graph shape works.

## Builder API

The builder should be a thin authoring convenience:

```csharp
var spec = EffectSpecBuilder
    .Chain()
    .Projectile(model: "...", speedStatId: AbilityHelper.ProjectileSpeed)
    .Area(radiusStatId: AbilityHelper.Radius, filter: TargetFilter.EnemyAlive)
    .Damage(formulaId: "stat.final", statId: AbilityHelper.DamageAmount)
    .Buff("burning", durationStatId: AbilityHelper.ChannelDuration)
    .Build();
```

Builder output must be the same data structure used by config/editor input. The builder must not hide extra behavior that cannot be represented in data.

## Compatibility

Existing abilities that directly attach effect components must keep working. The spec path is additive at first. During implementation, avoid changing `War3Frame.Generator`, `FrameBuild`, or CLI behavior.

## Open Questions

- Whether the parameter table should use `Dictionary<string, float>` only for the first implementation, or support typed scalar/string/bool parameters immediately.
- Whether chain ordering should be strict sequential execution in this step or only ordered expansion with current settlement timing.
- Whether formula ids should be string ids for editor readability or integer ids for performance. The proposal recommends string ids first, with optional cached resolution later.

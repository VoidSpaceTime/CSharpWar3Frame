# Codebase Structure

## Directory Layout

```
CSharpWar3Frame/
├── CSharpWar3Frame.slnx      # Solution file (.slnx XML, groups Core/Projects/War3Builder)
├── AGENTS.md                 # Repo governance: OpenSpec design→review→implement→test→summarize→commit
├── GAMEPROCESSOR_REFACTOR.md # ⚠️ DEPRECATED — describes an abandoned refactor, not current code
├── CSharpWar3Frame/          # CLI tool (console exe): run/we/new/multi verbs
├── FrameBuild/               # War3FrameBuild library: build orchestration, commands, templates
├── War3Frame/                # Runtime framework: ECS runtime, native interop, authoring surface
├── War3Frame.Generator/      # Roslyn source generators: system + template registration
├── BridgeToJIT/              # C++/CLI host DLL for the JIT payload run path
├── FastMDX/                  # MDX/MDL model parser library
├── ModelFormat/              # Standalone console: model texture extraction/normalization
├── Projects/                 # Per-map payload projects (class libraries → payload DLL)
│   ├── demo/                 # Demo project template (source for `new <name>`)
│   └── test/                 # Test project: framework validation scenario
├── War3Frame.Tests/          # Stale directory — build artifacts only, no .csproj, not in solution
├── Vendor/                   # Third-party tools: w3x2lni (map packer), we (World Editor / YDWE)
├── openspec/                 # OpenSpec governance: proposals, specs, templates, archive
├── .temp/                    # Build scratch (git-ignored)
├── .codegraph/ .codex/ .cortexkit/ .claude/ .omo/ .opencode/ .sisyphus/ .idea/ .vs/ .vscode/
│                             # Tooling/IDE state (not part of the build)
```

## Directory Purposes

**`CSharpWar3Frame/`:**
- Purpose: CLI entry point that parses `run <name>`, `we <name>`, `new <name>`, `multi [n]` and drives the build pipeline.
- Contains: `Program.cs`, console project file, `Properties/Resources.resx`.
- Key files: `CSharpWar3Frame/Program.cs`, `CSharpWar3Frame/CSharpWar3FrameConsole.csproj`

**`FrameBuild/`:**
- Purpose: Build orchestration library (`War3FrameBuild`): project creation, w3x sync, payload publish, map packing, War3/WE launch.
- Contains: Partial `CommandManager` commands, `ConfigPath`, Serilog/YAML bootstrap, scaffold templates under `Template/`.
- Key files: `FrameBuild/CommandManager/CommandManager.cs`, `FrameBuild/CommandManager/Run.cs`, `FrameBuild/CommandManager/AssetsBuild.cs`, `FrameBuild/ApplicationBuilderExtensions.cs`, `FrameBuild/ConfigPath.cs`, `FrameBuild/War3FrameBuild.csproj`

**`War3Frame/`:**
- Purpose: The runtime framework — ECS store/root, War3 native interop facade, typed API wrappers, components, systems, helpers, template registry.
- Contains: `initialization/`, `Library/`, `Src/`, `Docs/`, `Properties/`.
- Key files: `War3Frame/initialization/ECSInit.cs`, `War3Frame/initialization/War3Init.cs`, `War3Frame/War3Frame.csproj`

**`War3Frame/Library/JassVM/`:**
- Purpose: The `War3` static facade — Jass VM memory access, native function table, cdecl marshalling, console allocation, version detection.
- Contains: `JassVM.cs`, `Native.cs`, `Console.cs`, `TypeVersion.cs`, `War3Helpers.cs`, `Storm.cs`.
- Key files: `War3Frame/Library/JassVM/Native.cs`, `War3Frame/Library/JassVM/TypeVersion.cs`

**`War3Frame/Library/Api/`:**
- Purpose: Typed static wrappers over War3 native functions.
- Contains: `JassApi.cs` (~11k lines), `DzApi.cs`, `KKApi.cs`, `KKPRApi.cs`, `YDApi.cs`, `Blizzard.cs` (constants), `JassType.cs`, `Extension.cs`.
- Key files: `War3Frame/Library/Api/JassApi.cs`, `War3Frame/Library/Api/DzApi.cs`

**`War3Frame/Src/Components/`:**
- Purpose: Friflo ECS components, tags, link components, relations, and enums — the semantic truth model.
- Contains: Domain subfolders (`Ability/`, `Attribute/`, `Unit/`, `AbilityEffectExtend/`, `Item/`) plus root-level component files (`Item.cs`, `MoveCommand.cs`, `Effects.cs`, `Buff.cs`, `Damage.cs`, `Combat.cs`, `CastState.cs`, `Time.cs`, `Player.cs`, `Settlement.cs`, `LevelExperience.cs`, `ItemUse.cs`, `UnitItemAuthoringSpec.cs`).
- Key files: `War3Frame/Src/Components/Unit/Units.cs`, `War3Frame/Src/Components/Attribute/Attribute.cs`, `War3Frame/Src/Components/MoveCommand.cs`, `War3Frame/Src/Components/Ability/Ability.cs`

**`War3Frame/Src/Systems/`:**
- Purpose: ECS systems that advance state and emit requests/outcomes. All systems carry `[SystemRegister(...)]`.
- Contains: Domain subfolders (`Ability/`, `Attribute/`, `Item/`, `Native/`, `Time/`, `Unit/`) plus root-level systems (`AuraSystem.cs`, `BuffSystem.cs`, `EffectRuntimeSystem.cs`, `LevelExperienceSystem.cs`, `SpatialGridSystem.cs`, `TimedSystemRoot.cs`, `SystemRegisterAttribute.cs`).
- Key files: `War3Frame/Src/Systems/TimedSystemRoot.cs`, `War3Frame/Src/Systems/SystemRegisterAttribute.cs`, `War3Frame/Src/Systems/Ability/CastingSystem.cs`, `War3Frame/Src/Systems/Ability/AbilityEffectSystems.cs`, `War3Frame/Src/Systems/Unit/MoveSystem.cs`, `War3Frame/Src/Systems/Native/UnitNativeSystem.cs`

**`War3Frame/Src/Systems/Native/`:**
- Purpose: The only layer allowed to execute War3 native side effects (per the native-call layering rules).
- Contains: `UnitCreateNativeSystem.cs`, `UnitNativeSystem.cs`, `UnitNativeSyncRegistry.cs`, `UnitRemoveNativeSystem.cs`, `UnitMoveNativeSystem.cs`, `ItemCreateNativeSystem.cs`, `PlayerNativeSystem.cs`, `EffectNativeSystem.cs`, `War3NativeBootstrap.cs`.
- Key files: `War3Frame/Src/Systems/Native/War3NativeBootstrap.cs`, `War3Frame/Src/Systems/Native/UnitNativeSyncRegistry.cs`

**`War3Frame/Src/Helpers/`:**
- Purpose: OOP authoring and workflow helpers — write ECS intent (components/requests), do not own long-lived native semantics.
- Contains: `AbilityHelper*.cs`, `AbilitySpecBuilder.cs`, `AttributeHelper.cs`, `AuraHelper.cs`, `BuffHelper.cs`, `ControlHelper.cs`, `EffectHelper.cs`, `EffectChainBuilder.cs`, `EffectFormulaRegistry.cs`, `GroupHelper.cs`, `ItemHelper.cs`, `ItemCompanionAbilityHelper.cs`, `ItemSpecBuilder.cs`, `ModifyHelper.cs`, `PlayerHelper.cs`, `SpatialGrid.cs`, `TargetFilterRegistry.cs`, `TeamHelper.cs`, `UnitHelper.cs`, `UnitSpecBuilder.cs`, `FourCc.cs`, plus `Native/HandleRefHelper.cs`.
- Key files: `War3Frame/Src/Helpers/EffectHelper.cs`, `War3Frame/Src/Helpers/AbilityHelper.cs`, `War3Frame/Src/Helpers/UnitSpecBuilder.cs`

**`War3Frame/Src/TemplateInit/`:**
- Purpose: Declarative template system — attributes, interfaces, and registries consumed by `UnitTemplateGenerator`.
- Contains: `UnitTemplateAttribute.cs`, `AbilityTemplateAttribute.cs`, `ItemTemplateAttribute.cs`, `InlineItemAbilityTemplate.cs`.
- Key files: `War3Frame/Src/TemplateInit/UnitTemplateAttribute.cs`, `War3Frame/Src/TemplateInit/AbilityTemplateAttribute.cs`

**`War3Frame/Src/EntityRef/` and `War3Frame/Src/EntityExtension/`:**
- Purpose: Typed `readonly struct` wrappers over `Entity` that limit the surface, plus extension methods to convert.
- Contains: `UnitEntityRef.cs`, `AbilityEntityRef.cs`, `AbilityEntityExtensions.cs`.
- Key files: `War3Frame/Src/EntityRef/UnitEntityRef.cs`

**`War3Frame/Src/Core/`:**
- Purpose: Cross-cutting runtime support.
- Contains: `Sync/SyncHelper.cs` (DzSyncData lockstep sync with Base36 entity encoding), `Sync/AsyncHelper.cs`, `UI/FrameHelper.cs`, `UI/UIComponent.cs`, `UI/UIManager.cs`.
- Key files: `War3Frame/Src/Core/Sync/SyncHelper.cs`

**`War3Frame.Generator/`:**
- Purpose: Compile-time code generation for system and template registration.
- Contains: `SystemGenerator.cs`, `UnitTemplateGenerator.cs`.
- Key files: `War3Frame.Generator/SystemGenerator.cs`, `War3Frame.Generator/UnitTemplateGenerator.cs`

**`BridgeToJIT/`:**
- Purpose: C++/CLI (net10.0, Win32) host DLL that loads `project.dll` in an isolated `AssemblyLoadContext` and calls `War3Frame.Game.BridgeMain`.
- Contains: `BridgeEntry.cpp`, `dllmain.cpp`, `pch.h`, `framework.h`, `.vcxproj`.
- Key files: `BridgeToJIT/BridgeEntry.cpp`

**`FastMDX/`:**
- Purpose: MDX/MDL model parsing library (blocks, geosets, materials, animations, save-back).
- Contains: `src/Objects/*` (model object types), `src/Parsers/*`, `src/MDX.cs`, `src/DataStream.cs`, `src/DataTypes.cs`, `src/Tags.cs`.
- Key files: `FastMDX/src/MDX.cs`

**`ModelFormat/`:**
- Purpose: Standalone console that opens MDX/MDL files, rewrites texture references, and copies referenced textures next to the model.
- Contains: `Program.cs`.
- Key files: `ModelFormat/Program.cs`

**`Projects/`:**
- Purpose: Per-map payload projects — class libraries that reference `War3Frame` and ship inside the map. The `test` project sets `AssemblyName=project`, the payload name the build pipeline and `BridgeToJIT` expect.
- Contains: `demo/` and `test/`, each with `Program.cs`, `Assets/*.cs` resource manifests, `Scripts/Template/*` templates, `Scripts/Process/*` scenarios (test only), `w3x/` map sources.
- Key files: `Projects/test/Program.cs`, `Projects/test/Scripts/Process/ItemCompanionAbilityValidationScenario.cs`, `Projects/test/Scripts/Template/Unit.cs`, `Projects/demo/Program.cs`

**`Vendor/`:**
- Purpose: Third-party tools and data required by the build pipeline.
- Contains: `w3x2lni/` (w2l.exe map packer + data), `we/` (World Editor/YDWE install: `KKWE.exe`, JASS libs, plugins). The build pipeline also resolves a separately configured WE install via `appsettings.yml` (`Config.We`) for `YDWEConfig.exe`/`WE.exe`.
- Key files: `Vendor/w3x2lni/w2l.exe`, `Vendor/we/KKWE.exe`

**`openspec/`:**
- Purpose: OpenSpec governance artifacts — proposals, designs, task lists, capability specs, archives.
- Contains: `changes/`, `specs/`, `templates/`, `README.md`.
- Key files: `openspec/README.md`, `openspec/templates/proposal-levels.md`

## Key File Locations

**Entry Points:**
- `CSharpWar3Frame/Program.cs`: CLI verb dispatch (`run`/`we`/`new`/`multi`).
- `War3Frame/initialization/ECSInit.cs`: `Game.ECSInit()` — creates `EntityStore` + `TimedSystemRoot`, registers generated systems.
- `War3Frame/initialization/War3Init.cs`: `Game.War3Init()` — creates players, starts the 0.01s main timer.
- `Projects/demo/Program.cs`: AOT payload entry `Game.MainAOT` (`[UnmanagedCallersOnly(EntryPoint = "main")]`).
- `Projects/test/Program.cs`: AOT entry `Game.AotMain` + bridge entry `Game.BridgeMain`.
- `BridgeToJIT/BridgeEntry.cpp`: exported `main()` that loads `project.dll` and invokes `BridgeMain`.
- `War3Frame.Generator/SystemGenerator.cs` + `UnitTemplateGenerator.cs`: compile-time generation entry.

**Configuration:**
- `FrameBuild/ConfigPath.cs`: `ConfigPath` model (`war3`, `pwd`, `we`, `w3x2lni`, `assets`).
- `FrameBuild/ApplicationBuilderExtensions.cs`: Serilog setup + `appsettings.yml` load/default-write.
- `CSharpWar3Frame/CSharpWar3FrameConsole.csproj`: CLI tool packages (`CommandLineParser`, `Serilog`, `YamlDotNet`).
- `War3Frame/War3Frame.csproj`: runtime package (`Friflo.Engine.ECS`) + generator analyzer wiring + AOT-compat flags.

**Core Logic:**
- `War3Frame/Src/Systems/TimedSystemRoot.cs`: per-system interval scheduling; Immediate = every tick.
- `War3Frame/Src/Systems/SystemRegisterAttribute.cs`: `[SystemRegister(SystemKind, order)]` marker.
- `War3Frame/Library/JassVM/Native.cs`: native table lookup, cdecl marshalling, callback dispatch.
- `War3Frame/Library/JassVM/JassVM.cs`: in-process Jass VM memory access.
- `War3Frame/Src/Systems/Ability/CastingSystem.cs`: cast request → move → cast → channel → finish workflow.
- `War3Frame/Src/Systems/Ability/AbilityEffectSystems.cs`: effect settlement pipeline (projectile, visual, area, damage, heal, buff).
- `War3Frame/Src/Systems/Native/UnitNativeSyncRegistry.cs`: declared ECS→native projection rules.
- `FrameBuild/CommandManager/Run.cs`: build + publish + pack + launch orchestration.

**Tests:**
- `Projects/test/Scripts/Process/ItemCompanionAbilityValidationScenario.cs`: in-map synchronous ECS validation scenario (throws on invariant failure).
- `Projects/test/Program.cs`: test client entry that initializes the scenario and drives `Root.Update`.
- `War3Frame.Tests/`: no active test project — build artifacts only (no `.csproj`, not in `CSharpWar3Frame.slnx`).

## Naming Conventions

**Files:** PascalCase `.cs`; systems end in `System`, helpers end in `Helper`, spec builders end in `SpecBuilder`, templates end in `Template`. Examples: `CastingSystem.cs`, `EffectHelper.cs`, `UnitSpecBuilder.cs`, `InlineItemAbilityTemplate.cs`.

**Directories:** PascalCase domain folders under `War3Frame/Src/` (`Components/`, `Systems/`, `Helpers/`); component/system subfolders group by gameplay domain (`Ability/`, `Attribute/`, `Unit/`, `Native/`, `Time/`, `Item/`). Command files under `FrameBuild/CommandManager/` are partial `CommandManager` classes named after the verb (`Run.cs`, `WE.cs`, `New.cs`, `Clear.cs`).

**Components:** Structs implementing `IComponent` (data), `ITag` (markers), `ILinkComponent`/`ILinkRelation` (ownership). Domains use `Base`/`State`/`Request`/`Runtime` suffixes: `AbilityBase`, `CastState`, `UnitCreateNativeRequest`, `AbilityRuntime`.

**Enums:** Stateful domain enums with explicit members, e.g. `SystemKind.Interval|Immediate`, `UnitLifecyclePhase`, `CastPhase`, `MoveOutcomeType`, `AbilityBehaviorTrigger`.

## Where to Add New Code

**New system:** `War3Frame/Src/Systems/<Domain>/<Name>System.cs` — derive from `QuerySystem<...>`, implement `ITimedSystem` when it needs a custom interval, and mark `[SystemRegister(SystemKind.Interval|Immediate, <order>)].` The generator registers it automatically.

**New component:** `War3Frame/Src/Components/<Domain>/<Name>.cs` — implement `IComponent` (data), `ITag` (marker), or `ILinkComponent`/`ILinkRelation` (ownership).

**New helper:** `War3Frame/Src/Helpers/<Name>Helper.cs` — write ECS intent (components/requests/outcomes); never hold long-lived native handles or drive native side effects directly.

**New native execution system:** `War3Frame/Src/Systems/Native/<Name>NativeSystem.cs` — the only layer that may call `JassApi`/`DzApi`/`KKApi`/`YDApi` directly; business systems write requests instead.

**New War3 API surface:** `War3Frame/Library/Api/<Name>Api.cs` — static wrapper resolving pointers via `War3.GetNativeFunction` and calling through `War3.CallNative<T>`.

**New unit/ability/item template in a project:** `Projects/<name>/Scripts/Template/<Name>.cs` — mark with `[UnitTemplate("name")]`, `[AbilityTemplate("name")]`, or `[ItemTemplate("name")]`; the generator registers it.

**New native projection rule:** `War3Frame/Src/Systems/Native/UnitNativeSyncRegistry.cs` — add a `UnitNativeSyncSpec` to `Specs` instead of scattering native setters in business systems.

**New build command:** `FrameBuild/CommandManager/<Name>.cs` as a partial `CommandManager`, plus a CLI verb class in `CSharpWar3Frame/Program.cs`.

**New source generator:** `War3Frame.Generator/<Name>Generator.cs` — `IIncrementalGenerator` driven by `ForAttributeWithMetadataName`; wire as an analyzer in `War3Frame/War3Frame.csproj`.

**New project payload:** `Projects/<name>/` — copy the `demo` layout (`Program.cs`, `Assets/`, `Scripts/Template/`, `w3x/`), or run `new <name>` to scaffold from `Projects/demo`.

**Shared runtime utilities:** `War3Frame/Src/Helpers/` (authoring) or `War3Frame/Src/Core/` (cross-cutting support).
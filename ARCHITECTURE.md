# Architecture

## Pattern Overview

**Overall:** ECS + OOP hybrid modding framework that runs C# game logic inside a Warcraft 3 (1.27a) process. Friflo.Engine.ECS is the single source of gameplay truth; War3 native calls are confined to a dedicated Native/Execution layer; a CLI build pipeline compiles a per-map "payload" assembly, packs a `.w3x` map, and launches the game.

**Key Characteristics:**
- Friflo.Engine.ECS `EntityStore` holds all long-lived gameplay state; OOP is reserved for templates, helpers, registries, and build orchestration.
- Request-driven workflows: business systems write `*Request` components, Immediate systems consume them in the same tick, and only Native/Execution systems touch `JassApi`/`DzApi`/`KKApi`/`YDApi`.
- Source generators auto-register `[SystemRegister]` systems and `[UnitTemplate]`/`[AbilityTemplate]`/`[ItemTemplate]` templates at compile time.
- Native interop relies on hardcoded `game.dll` 1.27a memory offsets behind version gates (`War3.TypeVersion`), cdecl marshalling, and a Jass VM memory wrapper.
- Two payload run paths: Native AOT (`[UnmanagedCallersOnly(EntryPoint = "main")]`) and JIT via a C++/CLI host bridge (`BridgeToJIT`).
- Each game project (`Projects/*`) is a class library that references the framework; the `test` project sets `AssemblyName=project` (the payload name the bridge and callback expect), and the framework's `Game` static class is re-declared per project to define the actual entry.

## Layers

**CLI Entry (`CSharpWar3Frame/`):**
- Purpose: Parse `run` / `we` / `new` / `multi` verbs and drive the build pipeline.
- Location: `CSharpWar3Frame/Program.cs`
- Contains: CommandLine verbs, Serilog console setup, `appsettings.yml` config load.
- Depends on: `FrameBuild` (`War3FrameBuild`), `CommandLineParser`, `Serilog`, `YamlDotNet`.
- Used by: Developers invoking the tool.

**Build Orchestration (`FrameBuild/`):**
- Purpose: Create projects, sync map sources, publish the payload DLL (JIT or AOT), pack the map with `w2l`, and launch War3/YDWE.
- Location: `FrameBuild/CommandManager/*.cs`, `FrameBuild/ApplicationBuilderExtensions.cs`, `FrameBuild/ConfigPath.cs`
- Contains: Partial `CommandManager` command classes (`Run`, `WE`, `New`, `Clear`, `AssetsBuild`, `SyncW3xFile`), template scaffolds in `FrameBuild/Template/`.
- Depends on: `War3Frame` (for `Assets` manifest records), `FastMDX` (texture extraction), `Vendor/w3x2lni` and `Vendor/we` tool paths, `NAudio`, `IniParser`, Roslyn scripting.
- Used by: `CSharpWar3Frame/Program.cs`.

**Native Host Bridge (`BridgeToJIT/`):**
- Purpose: JIT-path host. A C++/CLI DLL that loads the published `project.dll` payload in an isolated `AssemblyLoadContext` and invokes `War3Frame.Game.BridgeMain`.
- Location: `BridgeToJIT/BridgeEntry.cpp`, `BridgeToJIT/dllmain.cpp`, `BridgeToJIT/BridgeToJIT.vcxproj`
- Contains: Exported `main()`, `PayloadLoadContext` (`AssemblyDependencyResolver`-based), managed entry dispatch.
- Depends on: .NET runtime hosting (`Ijwhost.dll`), `project.dll` payload.
- Used by: War3 JIT builds (Debug/Test mode); Release AOT builds bypass it.

**Runtime Framework (`War3Frame/`):**
- Purpose: The ECS runtime, native interop facade, and authoring surface for map logic.
- Location: `War3Frame/`
- Contains: Initialization (`initialization/`), native facade (`Library/JassVM/`), API wrappers (`Library/Api/`), ECS components (`Src/Components/`), systems (`Src/Systems/`), helpers (`Src/Helpers/`), template registry (`Src/TemplateInit/`), entity refs (`Src/EntityRef/`, `Src/EntityExtension/`), sync/UI core (`Src/Core/`), asset manifest model (`Library/Assets.cs`).
- Depends on: `Friflo.Engine.ECS` package; analyzer reference to `War3Frame.Generator`.
- Used by: All `Projects/*` payload assemblies.

**Source Generators (`War3Frame.Generator/`):**
- Purpose: Emit registration code so systems and templates require no manual wiring.
- Location: `War3Frame.Generator/SystemGenerator.cs`, `War3Frame.Generator/UnitTemplateGenerator.cs`
- Contains: Incremental generators driven by `ForAttributeWithMetadataName`.
- Depends on: `Microsoft.CodeAnalysis.CSharp`, referenced as an analyzer from `War3Frame.csproj`.
- Used by: `War3Frame` compilation.

**Payload Projects (`Projects/demo`, `Projects/test`):**
- Purpose: Per-map C# logic — the code that ships inside the `.w3x`.
- Location: `Projects/demo/Program.cs`, `Projects/test/Program.cs`
- Contains: `Program.cs` entry (AOT `main` and/or `BridgeMain`), `Assets/*.cs` resource manifests, `Scripts/Template/*` authoring templates, `Scripts/Process/*` scenarios, `w3x/` map sources.
- Depends on: `War3Frame` (`Projects/test` via `extern alias War3FrameRuntime`), `Friflo.Engine.ECS`.
- Used by: The War3 client through the AOT or bridge load path.

**Model Tooling (`FastMDX/`, `ModelFormat/`):**
- Purpose: Parse and rewrite MDX/MDL models; extract referenced textures for map assets.
- Location: `FastMDX/src/`, `ModelFormat/Program.cs`
- Contains: `MDX` model reader/writer, block parsers, object types; `ModelFormat` console that normalizes models and copies textures.
- Depends on: `FastMDX` only (used by both `FrameBuild` and `ModelFormat`).
- Used by: `FrameBuild` asset pipeline, standalone `ModelFormat` runs.

**Vendor Tools (`Vendor/`):**
- Purpose: Third-party executables and data referenced by the build pipeline.
- Location: `Vendor/w3x2lni/` (`w2l.exe` map packer), `Vendor/we/` (World Editor / YDWE).
- Contains: Binaries, JASS libraries, localization data, prebuilt map data.
- Depends on: Nothing in-repo.
- Used by: `FrameBuild` (`CommandManager.Run`, `CommandManager.WE`).

## Data Flow

**System auto-registration:**
1. Mark a system `[SystemRegister(SystemKind.Interval|Immediate, order)]` — `War3Frame/Src/Systems/SystemRegisterAttribute.cs`
2. `SystemGenerator` collects them and emits `Game.SystemRegistration.g.cs` — `War3Frame.Generator/SystemGenerator.cs`
3. `Game.ECSInit()` calls `RegisterGeneratedSystems()`, adding each system to `TimedSystemRoot` — `War3Frame/initialization/ECSInit.cs`
4. `TimedSystemRoot.Add` wraps each system in a `SystemGroup` and gives it a per-system `TimerInfo` — `War3Frame/Src/Systems/TimedSystemRoot.cs`

**Game tick loop:**
1. `Game.War3Init()` creates 16 player entities via `War3NativeBootstrap.CreatePlayers` and starts a native timer — `War3Frame/initialization/War3Init.cs`
2. `TimerStart` fires every `TICK_RATE = 0.01f` and calls `Root.Update(tick)` — `War3Frame/Src/Systems/Native/War3NativeBootstrap.cs`
3. `TimedSystemRoot.Update` runs each enabled system when its accumulated time reaches its interval; systems without `TimerInfo` (Immediate, registered at `0f`) run on every tick — `War3Frame/Src/Systems/TimedSystemRoot.cs`

**Request → workflow → native execution (unit creation):**
1. `UnitTemplate.Create` writes `UnitCreateNativeRequest` alongside `UnitLifeState` + `Position` — `War3Frame/Src/TemplateInit/UnitTemplateAttribute.cs`
2. `UnitCreateNativeSystem` (Immediate) consumes the request and creates the native unit — `War3Frame/Src/Systems/Native/UnitCreateNativeSystem.cs`
3. The native unit handle is stored in `UnitNative`; lifecycle phase transitions are driven separately by `UnitLifecycleTransitionSystem` → `UnitRemoveNativeSystem`/`UnitLifecycleDisposeSystem` — `War3Frame/Src/Systems/Unit/*.cs`

**Move command → outcome → continuation:**
1. Business layers write `MoveCommand` (`MoveReason`, `MoveOrderType`, `commandToken`) — `War3Frame/Src/Components/MoveCommand.cs`
2. `MoveSystem` (0.1s interval) measures distance, writes `MoveOutcome`, and requests the native command through `UnitHelper.RequestMoveCommand` — `War3Frame/Src/Systems/Unit/MoveSystem.cs`
3. `UnitMoveNativeSystem` (Immediate) executes the native `Issue*Order` — `War3Frame/Src/Systems/Native/UnitMoveNativeSystem.cs`
4. Consumers like `MoveToTaskSystem` bridge the outcome into a task state — `War3Frame/Src/Systems/Unit/MoveSystem.cs`

**Cast workflow:**
1. `CastRequestSystem` validates range/cost/owner and either starts `CastState` or issues a `MoveCommand` with `MoveContinuation` — `War3Frame/Src/Systems/Ability/CastingSystem.cs`
2. `MoveToCastSystem` consumes the move outcome and promotes the cast to `Casting` — `War3Frame/Src/Systems/Ability/CastingSystem.cs`
3. `CastingSystem` (0.05s) advances front-swing → commit cost + trigger `OnEffect` → channel or backswing → `OnFinished` — `War3Frame/Src/Systems/Ability/CastingSystem.cs`
4. `ChannelingSystem` ticks channel ticks and finishes — `War3Frame/Src/Systems/Ability/CastingSystem.cs`
5. Effect settlement systems (orders 100–130) process damage/heal/buff/area/projectile/visual steps — `War3Frame/Src/Systems/Ability/AbilityEffectSystems.cs`

**Native state projection (compare-sync):**
1. `UnitNativeSyncRegistry.Specs` declares which attributes project to native state and how — `War3Frame/Src/Systems/Native/UnitNativeSyncRegistry.cs`
2. `UnitNativeSystem` (0.03125s) compares ECS attr values against `UnitNativeSyncSnapshot`, applies only meaningful changes via `JassApi.SetUnitState`, and reads native position back into `Position` — `War3Frame/Src/Systems/Native/UnitNativeSystem.cs`
3. Position is the one field where the native world is the source of truth; all other semantics live in ECS.

**Template authoring:**
1. Mark a class `[UnitTemplate("footman")]` implementing `IUnitTemplate.Configure` — `Projects/test/Scripts/Template/Unit.cs`
2. `UnitTemplateGenerator` emits `Register("footman", new FootmanTemplate())` — `War3Frame.Generator/UnitTemplateGenerator.cs`
3. `UnitTemplate.Initialize()`/`Create(name)` looks up the registry and builds the entity — `War3Frame/Src/TemplateInit/UnitTemplateAttribute.cs`
4. The same pattern applies to `AbilityTemplate` and `ItemTemplate`; inline item abilities use the reserved `__item_inline__:` prefix — `War3Frame/Src/TemplateInit/AbilityTemplateAttribute.cs`

**Build → map → run:**
1. `run <project>` parses to `CommandManager.Run` — `CSharpWar3Frame/Program.cs`
2. `BuildMap` (sync w3x, copy template assets, patch `callback`) and `PublishProject` (dotnet publish, JIT or AOT) run in parallel — `FrameBuild/CommandManager/Run.cs`
3. `PackupMap` invokes `w2l.exe` to produce `Maps/Test/<project>.w3x` — `FrameBuild/CommandManager/Run.cs`
4. `RunTest` launches War3 through `YDWEConfig.exe -launchwar3` with retry — `FrameBuild/CommandManager/Run.cs`

## Key Abstractions

**`Game` (static):**
- Purpose: Global facade exposing the `EntityStore`, the `TimedSystemRoot`, tick rate, and init sequence.
- Location: `War3Frame/initialization/ECSInit.cs`, `War3Frame/initialization/War3Init.cs`; per-project redeclarations in `Projects/*/Program.cs`
- Pattern: Static partial class; generated `RegisterGeneratedSystems()` is filled by the source generator.

**`TimedSystemRoot` / `ITimedSystem`:**
- Purpose: Run each system on its own update interval while preserving Friflo ordering; Immediate systems (registered at `0f`) execute every tick.
- Location: `War3Frame/Src/Systems/TimedSystemRoot.cs`
- Pattern: `SystemRoot` subclass wrapping each system in a `SystemGroup` with a `TimerInfo`.

**`SystemRegisterAttribute` / `SystemKind`:**
- Purpose: Declarative system registration with an execution order.
- Location: `War3Frame/Src/Systems/SystemRegisterAttribute.cs`
- Pattern: Attribute consumed by `SystemGenerator`; `Immediate` means same-tick request consumption.

**`War3` (static partial):**
- Purpose: Native interop facade — Jass VM memory access, native function table lookup, cdecl calling convention marshalling, console allocation, version detection.
- Location: `War3Frame/Library/JassVM/*.cs`
- Pattern: Static facade; all addresses are `Lazy<nint>` behind `SelectVersion` gates; invalid addresses fail to `0`.

**`JassApi` / `DzApi` / `KKApi` / `YDApi` / `Blizzard`:**
- Purpose: Typed wrappers over War3 native functions.
- Location: `War3Frame/Library/Api/*.cs`
- Pattern: Static classes resolving function pointers via `War3.GetNativeFunction` and calling through `War3.CallNative<T>`.

**`UnitNativeSyncRegistry`:**
- Purpose: Single declaration point for how ECS attributes project to native unit state.
- Location: `War3Frame/Src/Systems/Native/UnitNativeSyncRegistry.cs`
- Pattern: `UnitNativeSyncSpec[]` registry consumed by `UnitNativeSystem`.

**Template registries (`UnitTemplate`, `AbilityTemplate`, `ItemTemplate`):**
- Purpose: OOP authoring entry — named templates configure entities.
- Location: `War3Frame/Src/TemplateInit/UnitTemplateAttribute.cs`, `AbilityTemplateAttribute.cs`, `ItemTemplateAttribute.cs`
- Pattern: SortedDictionary registries (`StringComparer.Ordinal`) populated by generated `RegisterGenerated()`.

**Spec builders (`UnitSpecBuilder`, `ItemSpecBuilder`, `AbilitySpecBuilder`):**
- Purpose: Fluent authoring API that writes spec data components onto entities.
- Location: `War3Frame/Src/Helpers/UnitSpecBuilder.cs`, `ItemSpecBuilder.cs`, `AbilitySpecBuilder.cs`
- Pattern: Builder → `BuildTo(entity)`.

**`CommandManager`:**
- Purpose: Orchestrates every CLI build verb.
- Location: `FrameBuild/CommandManager/CommandManager.cs` (+ partial files)
- Pattern: Partial class per command area; owns all temp/build/vendor paths.

**`PayloadLoadContext`:**
- Purpose: Isolated load of `project.dll` for the JIT run path.
- Location: `BridgeToJIT/BridgeEntry.cpp`
- Pattern: `AssemblyLoadContext` subclass with `AssemblyDependencyResolver`.

## Entry Points

**CLI tool:**
- Location: `CSharpWar3Frame/Program.cs`
- Triggers: `dotnet run` / executable invocation with `run <project>`, `we <project>`, `new <project>`, `multi [count]`.
- Responsibilities: Load config, construct `CommandManager`, dispatch to the build/run pipeline.

**Framework init:**
- Location: `War3Frame/initialization/ECSInit.cs`, `War3Frame/initialization/War3Init.cs`
- Triggers: Called by a project's entry (`Game.Main`/`BridgeMain`/`MainAOT`).
- Responsibilities: Create `EntityStore`, build `TimedSystemRoot`, register generated systems, create players, start the 0.01s main timer.

**AOT payload entry:**
- Location: `Projects/demo/Program.cs`, `Projects/test/Program.cs`
- Triggers: War3 loads the native AOT export `main`.
- Responsibilities: `War3.EnableConsole()`, init the framework, drive `Root.Update` from a native timer.

**Bridge payload entry:**
- Location: `Projects/test/Program.cs` (`Game.BridgeMain`)
- Triggers: `BridgeToJIT` exports `main`, loads `project.dll`, resolves `War3Frame.Game.BridgeMain`.
- Responsibilities: Same as AOT path but through the JIT host bridge.

**Source generators:**
- Location: `War3Frame.Generator/SystemGenerator.cs`, `War3Frame.Generator/UnitTemplateGenerator.cs`
- Triggers: Compilation of `War3Frame` (generators are wired as analyzers).
- Responsibilities: Emit `Game.SystemRegistration.g.cs`, `UnitTemplate.g.cs`, `AbilityTemplate.g.cs`, `ItemTemplate.g.cs`.

## Error Handling

**Strategy:** Fail closed at the native boundary; fail fast in workflows.

- Native addresses are version-gated (`War3.TypeVersion.SelectVersion`); unresolved or unsafe addresses return `0`, and `War3.EnsureNativeFunctionAvailable` throws `InvalidOperationException` before any call.
- Jass VM unavailability is surfaced as sentinel values (`JassVM.IsAvailable == false`, empty strings, `false` from `TryGet*`), with `War3.ReportNativeSafetyIssue` writing diagnostics to the console.
- Callback dispatch validates register types and index bounds before invoking a delegate; invalid inputs are skipped with a safety report (`War3Frame/Library/JassVM/Native.cs`).
- Template lookups throw `ArgumentException` for unknown names; inline item ability names are validated against the reserved `__item_inline__:` prefix and a 256-char owner limit (`War3Frame/Src/TemplateInit/AbilityTemplateAttribute.cs`).
- Build pipeline checks exit codes for `dotnet publish` and `w2l.exe`, logs failures via Serilog, cleans stale build output before release builds, and retries the War3 launch up to 3 times (`FrameBuild/CommandManager/Run.cs`).
- Validation scenarios assert invariants with `Require`, throwing `InvalidOperationException` on failure (`Projects/test/Scripts/Process/ItemCompanionAbilityValidationScenario.cs`).

## Cross-Cutting Concerns

**Logging:** Serilog console sink with a custom colored theme, configured in `FrameBuild/ApplicationBuilderExtensions.cs` and used throughout `FrameBuild`; the runtime framework prints diagnostics via `Console.WriteLine` from the native safety reporter.

**Caching:** `FourCc` caches four-character-code → int conversions; `SyncHelper`/`Extension` keep `ConcurrentDictionary` caches for string ↔ id conversions; `TimedSystemRoot` caches per-system `TimerInfo`.

**Storage:** No persistence layer. Build artifacts live in `.temp/` (git-ignored); packed maps go to `Maps/Test/` under the War3 directory; `appsettings.yml` holds environment paths.

**Config:** YAML (`appsettings.yml`) deserialized into `ConfigPath` (`FrameBuild/ConfigPath.cs`); a default file is generated on first run.

**Threading:** All gameplay logic is single-threaded inside the War3 timer callback; the build pipeline uses `Task.WhenAll` for map copy and publish, with explicit exit-code verification afterward.
# Add Src Essential Chinese Comments

- Change ID: `add-src-essential-chinese-comments`
- Level: `fast`
- Status: pending review

## Change Goal

Scan C# code under `War3Frame/Src` and add necessary Chinese comments for public contracts, complex workflows, non-obvious ECS/native boundaries, and important side effects.

## Impact Files

- `War3Frame/Src/**/*.cs`
- No generator, build, CLI, project dependency, or runtime behavior changes are intended.

## Why Low Risk

This change is documentation-only:

- It does not change behavior semantics.
- It does not change public API signatures.
- It does not add or move War3 native calls.
- It is locally verifiable by build.

The implementation should avoid mechanical over-commenting. Add comments only where they improve understanding, especially around ECS state ownership, request/outcome flow, native execution boundaries, formula/spec resolution, lifecycle transitions, and helper responsibilities.

## Validation

- Run `dotnet build War3Frame/War3Frame.csproj`.
- Optionally run `dotnet build Projects/test/test.csproj` if touched comments are near sample-facing templates or shared runtime paths.

## Acceptance Criteria

- Key non-obvious types and systems under `War3Frame/Src` have useful Chinese comments.
- Existing behavior and public signatures are unchanged.
- No comments are added merely to restate obvious line-level code.

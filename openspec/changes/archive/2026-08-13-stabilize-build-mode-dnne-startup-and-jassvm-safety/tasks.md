## 1. Capability formalization

- [x] 1.1 Record the confirmed Build-mode publish/startup evidence in proposal and spec
- [x] 1.2 Record the confirmed JassVM version/callback safety evidence in proposal and spec
- [x] 1.3 Keep Release/AOT redesign explicitly out of scope for this change

## 2. Build-mode publish stabilization

- [x] 2.1 Make `PublishProject()` drain redirected stdout/stderr correctly and observe process exit clearly
- [x] 2.2 Remove or isolate concurrent writes from `BuildMap()` and `PublishProject()` to the same `BuildDstPath/map` tree
- [x] 2.3 Ensure Build-mode map output contains the required DNNE side-by-side payload

## 3. Export and probe alignment

- [x] 3.1 Reduce `Projects/test` to one canonical Build-mode export contract
- [x] 3.2 Align `Projects/TestDnne` export probing and invocation with that canonical contract
- [x] 3.3 Keep Build-mode callback/module expectations aligned with the canonical contract

## 4. JassVM safety hardening

- [x] 4.1 Make unsupported Warcraft versions fail closed before address resolution
- [x] 4.2 Guard current JassVM access before reading registers or dereferencing VM-backed pointers
- [x] 4.3 Guard native callback dispatch against invalid delegate indices or invalid register payloads
- [x] 4.4 Keep checked-in callback and generated callback template safety behavior aligned

## 5. Verification

- [x] 5.1 Re-run Build-mode `test` flow and confirm `dotnet publish` no longer hangs
- [x] 5.2 Verify `.temp/Build/test/map` contains the expected Build-mode DNNE/JIT payload
- [x] 5.3 Run `Projects/TestDnne` and confirm canonical export probing and invocation succeed (`TestDnne` was executed with a project-local x86 .NET runtime, successfully loaded `testNE.dll`, found canonical export `main`, invoked it, printed `Hello World! isAot: False`, and wrote `.temp/war3frame_test_bootstrap.log`)
- [ ] 5.4 Verify unsupported-version and invalid-current-VM paths fail safely (code paths were hardened and compile cleanly, but no local Warcraft runtime is available here to execute those branches end-to-end)

## 6. Approval gating

- [x] 6.1 Block code implementation until this proposal/design/tasks/spec set is reviewed and approved
- [x] 6.2 If scope expands into Release/AOT runtime-model redesign, open or extend a separate change instead of broadening this one silently

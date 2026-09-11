## 1. Proposal gating

- [ ] 1.1 Wait for approval before modifying code or build wiring

## 2. Bridge project

- [ ] 2.1 Add a new x86 C++/CLI bridge project under `Projects/`
- [ ] 2.2 Make the bridge export native `main`
- [ ] 2.3 Ensure the bridge has no `ProjectReference` to `Projects/test`
- [ ] 2.4 Make the bridge load `test.dll` from its own directory and invoke a fixed managed entry

## 3. Managed payload migration

- [ ] 3.1 Remove DNNE package/configuration from `Projects/test/test.csproj`
- [ ] 3.2 Replace `[UnmanagedCallersOnly]` export ownership with a bridge-callable managed entry in `Projects/test/Program.cs`
- [ ] 3.3 Keep the managed payload output name aligned with `test.dll`

## 4. FrameBuild migration

- [ ] 4.1 Replace DNNE copy logic in `FrameBuild/CommandManager/Run.cs`
- [ ] 4.2 Stage bridge output and managed payload together into `.temp/Build/test/map`
- [ ] 4.3 Update staged artifact verification to validate the bridge-based payload set

## 5. Validation and cleanup

- [ ] 5.1 Update or retain `Projects/TestDnne` so it validates the new `testNE.dll`
- [ ] 5.2 Verify `callback` still loads `testNE.dll` without renaming changes
- [ ] 5.3 Search the active test route for remaining DNNE references and remove them
- [ ] 5.4 Decide whether `dnne_smoke` remains as a historical diagnostic project or is separately removed later

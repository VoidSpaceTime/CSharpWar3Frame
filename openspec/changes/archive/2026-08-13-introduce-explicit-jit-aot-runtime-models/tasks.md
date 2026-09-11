## 1. Model definition

- [ ] 1.1 Introduce an explicit `RuntimeModuleModel` concept covering `Jit` and `Aot`
- [ ] 1.2 Define the legal combinations of `BuildMode` and `RuntimeModuleModel`

## 2. Publish branching plan

- [ ] 2.1 Define how `Run.cs` selects publish/build behavior from the chosen runtime model
- [ ] 2.2 Ensure `PublishAot=true` appears only in the AOT branch
- [ ] 2.3 Define how JIT/DNNE projects are handled without AOT assumptions

## 3. Callback and artifact alignment

- [ ] 3.1 Define callback module naming for JIT mode
- [ ] 3.2 Define callback module naming for AOT mode
- [ ] 3.3 Define verification rules to prove callback expectations match actual generated artifacts

## 4. Verification planning

- [ ] 4.1 Define a verification step proving which runtime model was selected at run time
- [ ] 4.2 Define a verification step proving the expected module artifact exists in the actual load path
- [ ] 4.3 Define a verification step proving callback `ModuleName` matches the actual artifact
- [ ] 4.4 Define a verification step proving Warcraft launches without black screen under the selected model

## 5. Approval gating

- [ ] 5.1 Block implementation until proposal, design, spec, and tasks review is approved
- [ ] 5.2 Forbid mixing AOT and JIT assumptions inside one publish branch

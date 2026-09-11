## 1. Diagnosis formalization

- [ ] 1.1 Record the current code-proven mismatch between `Run.cs` Release publish strategy and `Projects/test` / `Projects/demo` module model
- [ ] 1.2 Record the callback/module naming expectations and generated artifact expectations

## 2. Target model decision

- [ ] 2.1 Decide whether `test` and `demo` remain DNNE/JIT modules or migrate to AOT modules
- [ ] 2.2 Document the chosen model as the only canonical Release runtime module path

## 3. Implementation planning

- [ ] 3.1 Define the minimal `Run.cs` strategy changes required for the chosen module model
- [ ] 3.2 Define the callback/module naming changes required for the chosen model
- [ ] 3.3 Define whether Win10 SDK remains a hard environment dependency after the design change

## 4. Verification planning

- [ ] 4.1 Define a verification step for build-mode selection (`Test` vs `Release`)
- [ ] 4.2 Define a verification step for actual generated module presence in the runtime load path
- [ ] 4.3 Define a verification step for callback/module-name correctness
- [ ] 4.4 Define a verification step for final map launch without black screen

## 5. Approval gating

- [ ] 5.1 Block implementation until proposal, design, spec, and tasks review is approved
- [ ] 5.2 Require any fix to avoid mixing AOT and DNNE assumptions in the same path

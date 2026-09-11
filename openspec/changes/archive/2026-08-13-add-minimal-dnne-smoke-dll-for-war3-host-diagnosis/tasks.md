## 1. Proposal gating

- [x] 1.1 Wait for approval before creating the smoke project

## 2. Implementation

- [x] 2.1 Add a minimal `Projects/dnne_smoke` project with DNNE configuration matching the current Build/JIT path
- [x] 2.2 Export unmanaged `main` that directly returns `0`
- [x] 2.3 Build/publish the smoke project into a standalone output folder for manual replacement testing

## 3. Verification

- [x] 3.1 Verify the produced native DLL exports `main`
- [x] 3.2 Verify the output folder contains the matching managed DLL, runtimeconfig, and deps files
- [x] 3.3 Return absolute file paths for manual replacement testing

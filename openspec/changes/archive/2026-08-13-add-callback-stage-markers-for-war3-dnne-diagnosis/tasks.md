## 1. Proposal gating

- [ ] 1.1 Wait for approval before modifying `Projects/test/w3x/map/callback`

## 2. Implementation

- [ ] 2.1 Add a directory marker when callback `main` begins execution
- [ ] 2.2 Add a directory marker after `LoadLibraryA` succeeds
- [ ] 2.3 Add a directory marker after `GetProcAddress("main")` succeeds
- [ ] 2.4 Keep the change scoped only to `Projects/test/w3x/map/callback`

## 3. Verification

- [ ] 3.1 Rebuild `Build/test` and confirm `.temp/Build/test/map/callback` contains the marker logic
- [ ] 3.2 Launch or probe the path and confirm marker directories distinguish failure stages

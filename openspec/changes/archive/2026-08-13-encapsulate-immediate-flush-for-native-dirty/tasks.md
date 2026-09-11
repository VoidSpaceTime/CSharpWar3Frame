## 1. Game-level encapsulation

- [x] 1.1 Add a Game-level helper that wraps ImmediateRoot.Update(default)
- [x] 1.2 Replace direct business-layer immediate flush calls with the new Game helper

## 2. Dirty helper encapsulation

- [x] 2.1 Add an immediate-mark helper alongside the existing non-immediate Mark helper
- [x] 2.2 Use the immediate-mark helper only for immediate lifecycle paths, not for regular interval dirty marking

## 3. Verification

- [x] 3.1 Run diagnostics on the updated files and confirm no new errors
- [x] 3.2 Verify the change scope remains limited to flush encapsulation and helper call sites

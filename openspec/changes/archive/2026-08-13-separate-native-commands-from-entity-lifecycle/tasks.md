## 1. Responsibility boundaries

- [x] 1.1 Define UnitLifecycle as the sole source of truth for long-lived unit state
- [x] 1.2 Define UnitNativeDirtyFlags as immediate native commands only

## 2. Death-flow alignment

- [x] 2.1 Align KillUnit and related entry points to advance entity lifecycle first, then emit native commands
- [x] 2.2 Align immediate native systems to consume Death/Remove/Reborn as commands without owning long-lived lifecycle meaning

## 3. Cleanup policy alignment

- [x] 3.1 Separate native death from native removal in corpse cleanup logic
- [x] 3.2 Ensure future pooling can swap cleanup policy without redefining lifecycle semantics

## 4. Verification

- [x] 4.1 Review death/corpse/remove/reborn paths for semantic consistency
- [x] 4.2 Confirm the design remains AOT-friendly and callback-free

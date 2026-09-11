## 1. Boundary definition

- [ ] 1.1 Define lifecycle cleanup responsibility boundaries between native command systems, timer expiry signaling, cleanup policy, and terminal removal
- [ ] 1.2 Document that native command systems do not own lifecycle cleanup completion semantics

## 2. Semantic separation

- [ ] 2.1 Specify separate meanings for Death, CorpseCleanup, and Remove
- [ ] 2.2 Specify that CorpseExpired remains a transient expiration signal instead of being folded into UnitLifecyclePhase

## 3. Cross-project review

- [ ] 3.1 Document runtime impact on UnitHelper, UnitNativeSystem, UnitNativeRemoveSystem, TimerTaskSystem, and CorpseCleanupSystem in War3Frame
- [ ] 3.2 Document explicit non-impact or deferred impact for War3Frame.Generator, FrameBuild, CSharpWar3Frame, and Projects/*

## 4. Verification planning

- [ ] 4.1 Define future verification checkpoints for death path, corpse expiry path, cleanup path, and direct remove path
- [ ] 4.2 Mark runtime implementation as blocked until proposal/design/spec review is approved

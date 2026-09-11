## Why

仓库中已经具备 `UnitLifeState` 与 `UnitLifecyclePhase` 作为生命周期持久状态的基础，但死亡、尸体保留、尸体清理与终态删除的主路径仍然分散在 helper、timer、native system、cleanup system 与 dirty flag 之间。当前生命周期语义不是由单一架构层统一拥有，而是被多个执行点共同塑形，导致 `KillUnit`、`RemoveUnit`、`TimerTaskSystem`、`UnitNativeRemoveSystem` 与 `CorpseCleanupSystem` 之间存在职责重叠与时序冲突。

选定的 A 方案将这条路径明确收敛为三层架构：

- `UnitLifeState` 负责生命周期真相
- `UnitLifecycleTransitionSystem` 负责 phase 推进
- `UnitLifecycleNativeEffectSystem` 负责 native side effects
- `UnitLifecycleDisposeSystem` 负责 terminal ECS cleanup

`TimerTaskSystem` 仅负责把尸体保留超时推进到 `ClearCorpse`，`UnitHelper` 仅负责写入生命周期意图并按需触发 `Game.FlushImmediateSystems()`。这样可以把当前散落的 lifecycle ownership 收敛成一个稳定、可验证、可扩展的架构。

## What Changes

- 采用 A 方案的 unit lifecycle architecture。
- 规定 `UnitLifeState.lifePhase` 是 unit lifecycle 的唯一真相源。
- 新增 `UnitLifecycleTransitionSystem` 作为唯一的 phase progression owner。
- 新增 `UnitLifecycleNativeEffectSystem` 作为唯一的 lifecycle-native side-effect owner。
- 新增 `UnitLifecycleDisposeSystem` 作为唯一的 terminal ECS cleanup owner。
- 规定 `TimerTaskSystem` 对 unit corpse-retention 的职责仅限于推进到 `ClearCorpse`。
- 规定 `UnitHelper` 对 lifecycle 的职责仅限于写 phase intent 与触发 immediate flush。
- 将 `CorpseCleanupSystem`、`CorpseExpired` 与 `Death/Remove` dirty flags 从 primary lifecycle path 中退出。

## Capabilities

### New Capabilities
- `unit-lifecycle-a-plan-architecture`: 定义 A 方案的生命周期真相层、推进层、native side-effect 层与 terminal cleanup 层。

### Modified Capabilities
- `ecs-lifecycle-state-model`
- `ecs-timer-task-model`
- `lifecycle-cleanup-responsibility-boundaries`

## Impact

- 直接影响 `War3Frame` 运行时的 unit lifecycle architecture、system ownership 与 cleanup flow。
- `War3Frame.Generator` 无直接行为变更预期，但后续实现需要确认系统注册与命名不依赖旧生命周期结构。
- `FrameBuild` 无直接行为变更预期，但后续实现需要确认构建编排不依赖旧生命周期系统结构。
- `CSharpWar3Frame` 无直接行为变更预期，但后续实现需要确认 CLI / tooling entry 不受影响。
- `Projects/*` 在提案阶段不改代码，但后续实现后必须重新验证 demo/test 中的 death/remove/corpse-retention 行为。
- 本次变更仅新增 OpenSpec 工件，不进入任何运行时代码修改。

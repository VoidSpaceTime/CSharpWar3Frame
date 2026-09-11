## Why

当前单位死亡、尸体保留、最终清理与直接删除这几条路径已经初步分层，但职责边界仍然不够清晰。`UnitNativeRemoveSystem` 在消费立即型 native 命令时，既承担原生副作用，又部分推进 entity 生命周期；`CorpseCleanupSystem` 负责尸体到期后的最终销毁；`TimerTaskSystem` 则通过 `CorpseExpired` 发出到期信号。与此同时，`UnitNativeSystem` 作为 native 同步系统，仍然混合了 native 同步与部分 entity 数据回写语义，容易让后续实现继续模糊“谁负责生命周期、谁负责原生副作用、谁负责最终清理”。

如果这组边界不先在规格层面澄清，后续围绕尸体保留、复活、对象池、原生句柄回收与直接删除的任何调整，都容易在系统职责上产生漂移。

## What Changes

- 新增一个专门的 OpenSpec capability，用于定义生命周期清理责任边界。
- 明确 `Death`、`CorpseCleanup`、`Remove` 是三个不同语义阶段，而不是可互相替代的同义操作。
- 明确 native 命令消费系统不拥有完整生命周期清理责任；它们执行立即型 native 副作用，但不定义“清理完成”的全部语义。
- 明确尸体到期后的最终销毁归属于 cleanup policy / cleanup system，而不是直接折叠到 native 命令系统中。
- 明确 `CorpseExpired` 作为瞬时到期信号保留，不直接并入 `UnitLifecyclePhase`。
- 明确后续若要调整 `UnitNativeSystem`，应朝“native 同步职责收敛”方向演进，而不是继续扩张生命周期清理责任。

## Capabilities

### New Capabilities
- `lifecycle-cleanup-responsibility-boundaries`: 定义生命周期阶段、到期信号、native 立即动作与最终清理之间的职责边界。

### Modified Capabilities

## Impact

- 主要影响 `War3Frame` 运行时中的生命周期 helper、timer 路由、native 系统与 cleanup 系统的职责定义。
- 对 `War3Frame.Generator`、`FrameBuild`、`CSharpWar3Frame` 没有直接行为影响；本提案不会改变 Source Generator、构建编排或 CLI 功能。
- 对 `Projects/*` 没有直接代码修改，但会影响未来样例项目理解“死亡 / 尸体 / 删除”语义的方式，因此需要保持术语一致。
- 本次变更仅创建提案工件，不进入实现阶段；任何代码修改仍需等待用户审核通过后再进行。

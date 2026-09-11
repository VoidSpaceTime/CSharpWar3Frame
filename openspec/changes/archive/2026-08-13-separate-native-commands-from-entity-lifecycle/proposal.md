## Why

当前单位死亡流程虽然已经拆出了生命周期组件与延迟清理 timer，但 `UnitNativeDirtyFlags.Death` 仍然和死亡流程存在语义纠缠：它既像是原生命令，又容易被误读为生命周期状态的一部分。为了给后续尸体保留、复活、单位池复用、native reset 等优化留出稳定扩展面，需要正式把“entity 生命周期状态”和“native 立即执行命令”拆成两个不同层次。

## What Changes

- 明确 `UnitLifecycle` 是单位长期状态的唯一真相源。
- 明确 `UnitNativeDirtyFlags` 只用于表达立即型 native 命令，而不是长期状态。
- 将死亡流程重构为“生命周期阶段推进 + native 命令消费”协作模型。
- 为后续单位池提供前提：死亡不等于删除 native，删除 native 只是一个可选命令阶段。

## Capabilities

### New Capabilities
- `native-command-vs-lifecycle-separation`: 定义 native 命令层与 entity 生命周期层的职责边界。

### Modified Capabilities

## Impact

- 主要影响单位死亡、删除、复活以及未来单位池相关流程。
- 不要求本轮直接实现单位池，但会为其铺平职责边界。
- 会影响 `KillUnit`、`RemoveUnit`、`UnitNativeRemoveSystem`、`UnitLifecycle` 与 corpse cleanup 相关逻辑。

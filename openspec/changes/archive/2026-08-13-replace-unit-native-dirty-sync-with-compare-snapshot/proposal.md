## Why

`UnitNativeDirtyFlags` 当前已经不再适合作为运行时主同步概念。随着 A 方案生命周期架构的确立，`Death/Remove` 等动作语义已经由 `UnitLifeState`、transition/native-effect/dispose 三层系统拥有，dirty flag 不应再承担生命周期意义。与此同时，`Health/Mana` 仍沿用“写入属性后打脏，再由 `UnitNativeSystem` 消费”的同步方式，这要求每个修改入口都遵守标记纪律，容易在未来随着属性入口增多而产生漏标问题。

更合适的结构是将 `Health/Mana` 收敛为 compare-sync：ECS 中的属性值是真相源，`UnitNativeSystem` 周期比较当前 ECS 值与“上次已同步到原生的快照”，只有值变化时才调用 `SetUnitState(...)`。这样既能避免无意义的原生调用，也能把同步责任从分散的写点收回到统一的 native sync layer。

在这套模型里：

- `UnitNativeDirtyFlags` 退出运行时主架构
- `Health/Mana` 变为 compare-sync 字段
- `Position` 继续保留为独立的 native-observe path
- `Death/Remove` 继续保留在 phase-driven lifecycle path 中，而不会重新回流到 sync flag 机制里

## What Changes

- 让 `UnitNativeDirtyFlags` 从 runtime architecture 中退场。
- 引入最小化 native sync snapshot 组件，仅服务于 `Health/Mana` compare-sync。
- 将 `Health/Mana` 从 dirty-sync 改为 compare-sync。
- 保持 `Position` 为单独的 native-observe path，而不是折叠进 compare-sync。
- 明确 `Death/Remove` 继续由 A 方案生命周期架构拥有，不通过 sync flags 重新承载动作语义。

## Capabilities

### New Capabilities
- `unit-compare-sync-and-native-observe-separation`: 定义 compare-sync 字段、snapshot baseline 与 native-observe path 的边界。

### Modified Capabilities
- `unit-lifecycle-a-plan-architecture`

## Impact

- 直接影响 `War3Frame` 运行时的 native sync architecture。
- `War3Frame.Generator` 预期无直接行为变更，但后续实现要确认生成/注册逻辑不依赖 dirty-sync 约定。
- `FrameBuild` 预期无直接行为变更，但后续实现要确认构建编排不依赖旧同步结构。
- `CSharpWar3Frame` 预期无直接行为变更，但后续实现要确认 CLI / tooling entry 不受影响。
- `Projects/*` 在提案阶段不改代码，但后续实现后必须验证 `Health/Mana` 同步、`Position` 读取、`Death/Remove` 生命周期行为仍符合预期。
- 本次变更仅新增 OpenSpec 工件，不进入运行时代码修改。

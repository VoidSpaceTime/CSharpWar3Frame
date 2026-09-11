## Why

当前 `UnitNativeSystem` 的 compare-sync 实现仍然依赖手写字段和手写分支：

- `UnitNativeSyncSnapshot` 手写 `lastHealthCurrent/lastHealthFinal/lastManaCurrent/lastManaFinal`
- `UnitNativeSystem` 中对 `Health` 与 `Mana` 分别写死同步分支

这种实现对当前两项同步来说足够简单，但一旦继续扩展到更多原生同步字段，组件结构与系统逻辑都会持续膨胀，并与仓库当前已经广泛采用的“`typeId + registry` 驱动”风格不一致。

更适合当前仓库的方案是：

- 用静态 registry/list 声明“哪些属性需要同步到原生，以及映射到哪个 native state”
- snapshot 只保留每个单位自己的运行时 baseline，不再继续手写固定字段

## What Changes

- 引入 unit native sync registry/list 作为同步声明层。
- 定义 registry 只保存静态同步规则，不保存运行时状态。
- 将每个单位的同步 baseline 责任保留在 per-unit snapshot 中。
- 逐步消除 `UnitNativeSystem` 中的手写属性分支。

## Capabilities

### New Capabilities
- `unit-native-sync-registry-driven-compare-sync`: 定义原生同步的声明层与运行时快照层分离。

## Impact

- 直接影响 `War3Frame` 的 `UnitNativeSystem` 与 `UnitNativeSyncSnapshot` 设计。
- 对 `War3Frame.Generator` / `FrameBuild` / `CSharpWar3Frame` 无直接行为变更预期。
- 本次变更仅新增 OpenSpec 工件，不进入代码实现。

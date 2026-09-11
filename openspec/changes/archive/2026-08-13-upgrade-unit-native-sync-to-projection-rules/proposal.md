## Why

当前 unit native sync 已经从“手写 Health/Mana 分支”升级到了“registry/list + per-unit snapshot”方向，但 registry 仍然过窄：它只表达 `attrTypeId -> nativeState`。这意味着当前设计仍默认所有原生同步都能通过同一种 `SetUnitState(...)` 写法完成。

这对 `Health` / `Mana` 足够，但对 `AttackRange` 这类需要 `YD_SetUnitState(...)` 或其它专用原生写法的字段就不再成立。继续把 registry 限定为“state 映射表”，会让系统迟早重新膨胀出多种特例分支。

因此，需要把 registry/list 的抽象再上提一层：从“native state mapping”升级成“native projection rule”，也就是显式声明每个同步项如何将 ECS 值投影到原生层。

## What Changes

- 将 unit native sync registry 从“state 映射表”升级为“projection rule 表”。
- 明确 projection rule 可以为不同同步项绑定不同的原生写入逻辑。
- 明确 baseline 仍然保留在 per-unit snapshot 中。
- 明确 projection 规则优先采用静态 apply 逻辑，而不是无边界散落在系统体中。

## Capabilities

### New Capabilities
- `unit-native-projection-rule-registry`: 定义 unit ECS 数据到 native world 的投影规则注册表。

## Impact

- 直接影响 `War3Frame` 的 `UnitNativeSyncRegistry`、`UnitNativeSystem` 与未来所有新增原生同步字段的扩展方式。
- 对 `War3Frame.Generator` / `FrameBuild` / `CSharpWar3Frame` 无直接行为变更预期。
- 本次变更仅新增 OpenSpec 工件，不进入代码实现。

# 提案：内联 UnitNativeSyncRegistry 到 UnitNativeSystem

## 元信息

- **状态**：已实施
- **等级**：`fast`
- **变更 ID**：`inline-unit-native-sync-registry`
- **日期**：2026-09-23
- **请求来源**：用户指示"先做第一步"（Native 层文件收敛的第 1 步，独立于第 2 步的并文件/命名空间统一）
- **默认实施后审查强度**：`R0 Direct`（编译 + 静态引用核对）
- **spec delta**：无（`.openspec.yaml` 声明 `skip_specs: true`）

## Why

`UnitNativeSyncRegistry`（`War3Frame/Src/Systems/Native/UnitNativeSyncRegistry.cs`，48 行）不是系统：它没有 query、没有 `[SystemRegister]`，只是 `UnitNativeSyncSpec` 声明记录 + `Specs` 静态数组 + 两个 `Apply` 委托。

该类型与其 `Specs` 的**唯一使用点**是 `UnitNativeSystem.cs:34`（`foreach (var spec in UnitNativeSyncRegistry.Specs)`）。当前是"单消费者 + 独立公开类型"的间接层，读者需要跨文件跳转才能看到投影规则。

目标：把投影声明与规则收进唯一的消费者 `UnitNativeSystem`，让"哪个属性投影到哪个原生状态"与"谁在执行投影"在同一处可读。

## What Changes

| 区域 | 影响 |
|---|---|
| `War3Frame/` | 仅 `Systems/Native/UnitNativeSystem.cs` 新增内联成员；删除 `Systems/Native/UnitNativeSyncRegistry.cs` |
| `War3Frame.Generator/` | 无。不新增/删除 `[SystemRegister]`，生成注册顺序与系统数不变 |
| `FrameBuild/` | 无 |
| `CSharpWar3Frame/` | 无 |
| `Projects/` | 无。回归场景不引用 `UnitNativeSyncRegistry` / `UnitNativeSyncSpec` |

逐项处置：

| # | 项 | 位置 | 处置 |
|---|---|---|---|
| 1 | `UnitNativeSyncSpec` | 同级公开 `readonly record struct` | 移为 `UnitNativeSystem` 的私有嵌套类型（唯一消费者，收缩可见性安全） |
| 2 | `UnitNativeSyncRegistry.Specs` | 静态公开数组 | 移为 `UnitNativeSystem.SyncSpecs` 私有静态数组，语义不变 |
| 3 | `ApplyHealth` / `ApplyMana` / `ToNativeStateValue` | 静态私有方法 | 原样移入 `UnitNativeSystem`，逻辑逐行不变 |

## 非目标

- 不改投影语义、不改 `SetUnitState` 的入参计算（`(current / final) * 10000f`）。
- 不动 `[SystemRegister(SystemKind.Interval)]`、`ITimedSystem.Interval`、`QuerySystem<UnitNative>`、compare-sync 容差。
- 不动其余 5 个 Native 文件的命名空间或文件组织（属第 2 步）。
- 不合并/重命名任何系统类。

## 风险与回滚

- **风险**：`UnitNativeSyncSpec` / `UnitNativeSyncRegistry` 若被反射、生成器或 `Projects/` 外部消费，收缩可见性会破坏编译。缓解：全仓（含 `Projects/`）引用核对；构建即验证。
- **回滚**：`git revert` 单次提交即可；无数据、无原生副作用影响。

## Verification

1. 全仓对 `UnitNativeSyncRegistry` / `UnitNativeSyncSpec` 的引用为 0（含 `Projects/`）。
2. `War3Frame`（及依赖它的项目）编译 0 error。
3. 系统注册数量与顺序不变（未新增/删除 `[SystemRegister]`）。
4. `UnitNativeSyncRegistry.cs` 已删除，投影规则可在 `UnitNativeSystem.cs` 内完整读到。

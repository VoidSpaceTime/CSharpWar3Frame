# 提案：合并 UnitControl/UnitMove 两个 Native 请求消费系统到同一文件

## 元信息

- **状态**：已实施
- **等级**：`fast`
- **变更 ID**：`consolidate-unit-native-request-files`
- **日期**：2026-09-23
- **请求来源**：用户指示"先做 2A"（Native 层文件收敛第 2 步；第 1 步为 `inline-unit-native-sync-registry`）
- **默认实施后审查强度**：`R0 Direct`（编译 + 注册顺序表比对）
- **spec delta**：无（`.openspec.yaml` 声明 `skip_specs: true`）

## 1. 背景与目标

第 1 步已把 `Systems/Native/` 从 6 个文件收敛到 5 个。剩余 5 个文件中：

| 文件 | 类 | Kind / Order | 命名空间 |
|---|---|---|---|
| `UnitControlNativeSystem.cs` | `UnitControlNativeSystem` | Immediate / 1 | `War3Frame.Systems.Native` |
| `UnitMoveNativeSystem.cs` | `UnitMoveNativeSystem` | Immediate / 1 | `War3Frame.Systems.Native` |
| `UnitCreateNativeSystem.cs` | `UnitCreateNativeSystem` | Immediate / 1 | `War3Frame.Src.Systems` |
| `UnitNativeSystem.cs` | `UnitNativeSystem` | Interval / 1 | `War3Frame.Src.Systems` |
| `UnitRemoveNativeSystem.cs` | `UnitRemoveNativeSystem` | Immediate / 10 | `War3Frame.Systems.Native` |

**目标**：把 `UnitControlNativeSystem` 与 `UnitMoveNativeSystem` 合并为一个文件。

**为什么只合这两个**：生成注册顺序为 `OrderBy(Order).ThenBy(FQN, Ordinal)`，且 `TimedSystemRoot.Update` 按**插入顺序**执行（`TimedSystemRoot.cs:122`）。这两个类**已同命名空间、Kind/Order 相同、且在排序结果中相邻**（基线位置 26 / 27，中间无其他系统）。因此合并**不改变任何 FQN、不改变任何执行位次**，是当前唯一可证明为零行为变更的文件合并。

`UnitCreateNativeSystem` 不纳入本变更：它属 `War3Frame.Src.Systems`，一旦并入 `War3Frame.Systems.Native` 会被排到**全部** `War3Frame.Src.Systems.*` order-1 系统之后，从而翻到 `UnitNativeSystem` 之后，使新建单位的血蓝投影晚一帧。修这个需要跨 order 桶抢位（`order 0`），属顺序契约变更，留给后续独立变更处理。

## 2. 影响范围

| 区域 | 影响 |
|---|---|
| `War3Frame/` | 新增 `Systems/Native/UnitNativeRequestSystems.cs`（含两个类，代码逐行搬移）；删除 `UnitControlNativeSystem.cs`、`UnitMoveNativeSystem.cs` |
| `War3Frame.Generator/` | 无。两个 `[SystemRegister(SystemKind.Immediate)]` 原样保留，注册数量 67 与排序结果不变 |
| `FrameBuild/` | 无 |
| `CSharpWar3Frame/` | 无 |
| `Projects/` | 无。回归用例只读取 `EffectNativeSystem.cs` 与 `PlayerNativeSystem.cs` |

## 3. 非目标

- 不改命名空间、不改 `[SystemRegister]` 的 Kind/Order、不改任何方法体。
- 不合并 `UnitCreateNativeSystem`（原因见 §1）。
- 不处理"order-1 桶依赖 FQN 字母序"这个根因（属后续顺序契约变更）。
- 不把两个类合成一个类——`QuerySystem<T>` 泛型签名不同（`<ControlStateNativeRequest>` vs `<MoveNativeRequest, UnitNative>`），合并需退化为 `BaseSystem` 手写查询，收益不足。

## 4. 风险与回滚

- **风险**：搬移时误改类名/命名空间会改变 FQN，从而静默重排。缓解：搬移后对 `Order|FQN|Kind` 全表做基线比对，必须逐行一致。
- **已知无害差异**：`UnitMoveNativeSystem.cs` 的 `using System.Numerics;` 在该文件中未被使用（无 `Vector2/3` 引用），合并文件中不保留。该 using 不参与行为。
- **回滚**：`git revert` 单次提交；无数据与原生副作用影响。

## 5. 验收标准

1. `Order|FQN|Kind` 全表与改动前基线**逐行一致**（67 项）。
2. `War3Frame` Release 构建 0 error。
3. 两个类的 `OnUpdate` 方法体与改动前逐行等价。
4. `Systems/Native/` 文件数 5 → 4，系统类数仍为 5。

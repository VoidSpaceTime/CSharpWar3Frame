# 提案：region 域评估（region-support-eval）

## 状态

- 状态：待用户裁决
- 等级：fast（纯评估，不改行为契约）
- 复盘强度：R0 Direct（文档核对 + `openspec validate`）
- 范围：只评估是否纳入原生 `region` / 通用进出事件；本 change 不包含实现

## Why

W5.2 要求重新评估 region 域。绑定层已有 `JRegion` 与 `CreateRegion` / `RegionAddRect` / `TriggerRegisterEnterRegion` / `TriggerRegisterLeaveRegion`，但运行时没有任何调用方。2026-09-01 用户已取消过一次独立 Region 系统（`cd77e7c`）。取消后 `SpatialGridSystem` 已注册并被回归场景使用，取消理由里的「网格是死代码」不再成立；「没有实际进出事件需求、不要再做一层通用抽象」仍然成立。

本提案只冻结现状、分工、成本和建议。用户裁决前不新增组件、系统、Helper 或原生调用。

## 现状

### 绑定存在，业务未用

- `JRegion`：`War3Frame/Library/Api/JassType.cs:268`。
- 创建与几何：`JassApi.CreateRegion`（`:2735`）、`RemoveRegion`（`:2745`）、`RegionAddRect`（`:2754`）、`RegionClearRect`（`:2763`）、`RegionAddCell`（`:2772`）、`IsUnitInRegion`（`:2863`）。
- 原生进出：`TriggerRegisterEnterRegion`（`:3196`）、`GetEnteringUnit`（`:3217`）、`TriggerRegisterLeaveRegion`（`:3227`）、`GetLeavingUnit`（`:3238`）。
- 全仓检索：上述符号只出现在 `JassApi.cs` 的声明与函数体。`War3Frame/Src`、`Projects/` 没有调用。

原生 region 是不规则区域句柄（rect 与 32×32 cell 的并集），进出靠 War3 trigger。它只认识原生 `unit`，不认识 ECS 里没有 `UnitNative` 的实体。

### 已取消先例

| 项 | 证据 |
|---|---|
| 提交 | `cd77e7c`（`cd77e7c6d401693739677e1e59f57e772c61a3d3`），2026-09-01，`chore(openspec): 取消 region-system 提案…` |
| 提交说明 | 「用户重新评估后取消（现有 SpatialGrid + AuraSystem 已覆盖核心场景，独立 Region 系统复用价值存疑）」 |
| 归档 | `openspec/changes/archive/2026-09-11-region-system/proposal.md`，状态「已取消」，取消日期 2026-09-01 |
| 被取消的设计 | `RegionComponent` / `RegionOccupancy` / `RegionSystem` / `RegionEnterEvent` / `RegionLeaveEvent`；明确不接触 War3 原生 rect/region 触发器 |

取消时写明的四条理由，本次复核后仍然适用：

1. 空间查询已有 `SpatialGrid.QueryCircle/Rect/Line/Cone`，经 `GroupHelper.FindInCircle/FindInRect` 暴露；`AuraSystem` 用周期 diff 做进入/离开，输出绑定为 Buff；`GroundAreaQueryHelper` 用 `GroundAreaBuffLink` 跟踪地面区域占用。
2. 当时「持久区域 + 通用进出事件」的实际需求数是 0。本次检索仍然是 0：没有任务区、Boss 房、关卡边界等待这类调用方。
3. 不照搬 lik/xlik 的原生触发器或手写轮询。
4. 按需组合优于预设抽象；取消文约定「未来有 3+ 场景再重提」。该阈值仍未达到。

取消文还要求立刻给 `SpatialGridSystem` 补注册。该项已落地，见下一节，不再作为本评估的遗留修复。

## 与 SpatialGridSystem 的分工

`SpatialGridSystem`（`War3Frame/Src/Systems/SpatialGridSystem.cs:13`）注册为 `[SystemRegister(SystemKind.Interval, 99)]`。每轮 `GroupHelper.Grid.Clear()` 后，把带 `Position` 的实体重新 `Insert`。`GroupHelper.Grid` 是 cellSize=256 的网格。回归场景 `Projects/Regression/DomainRegression.cs` 的 `spatial-search` 已断言：注册后的索引能让 `FindInCircle` 命中范围内实体。

| | SpatialGrid + GroupHelper | 原生 JRegion | 已取消的 ECS Region |
|---|---|---|---|
| 回答的问题 | 这一刻，形状里有哪些实体 | 原生单位何时走进/走出不规则区域 | 持久区域的进入/离开事实 |
| 形状 | 圆、轴对齐矩形、线段、扇形 | rect 并集 + 32×32 cell | 圆 / 矩形（提案已作废） |
| 对象 | 任何带 `Position` 的实体 | 仅原生 `unit` | ECS 实体 |
| 时间 | 调用时的快照，不保留占用 | War3 引擎边沿事件 | 周期 diff 快照 |
| 副作用 | 无；查询方自己决定 | 原生 trigger，须桥接成 ECS Event | 独立 `XxxEvent` 实体 |

重叠只在「此刻谁在一块轴对齐矩形里」：`QueryRect` 已覆盖，且能包含没有原生句柄的实体。原生 region 多出来的只有三件事：非矩形并集、引擎侧边沿（不用自建快照）、以及必须跟编辑器里已放好的 region/trigger 对齐。这三件今天都没有调用方。

`AuraSystem`（order 42，`Interval = 0.1f`）已经是「范围内集合 vs 上一轮 link，差集即进入/离开」的实例，但差集直接变成 Buff，不对外发 Event。`GroundAreaQueryHelper` 对地面区域做同样的 link diff。两者都是特定输出，不是通用区域事件。

## 区域进入/离开若要做，应长什么样

下面是与现有事件链对齐的用法，不是已批准需求。当前代码里零处这样用。

- 任务/剧情边界：单位进入某块持久区域，发一次事实，`TriggerSystem`（order 131）匹配规则后只产生已有 `XxxRequest`。
- 关卡门控：进入 Boss 房或离开安全区，驱动刷怪/关门请求；停留期间不得重复 Enter。
- 区域销毁：区域内成员应收到一次 Leave，避免占用泄漏。这是取消提案里的验收第 5 条，仍然是正确的事件语义，只是没有场景消费它。

若将来真要做，事件形态必须沿用现有契约，而不是原生 trigger 回调里直接调 Jass：

```text
区域检测系统（周期 diff，order < 131）
  -> 独立实体 RegionEnterEvent / RegionLeaveEvent + TriggerEventMarker
  -> TriggerSystem（131）只读匹配，动作只写 XxxRequest
  -> EventCleanupSystem（132）删除事件实体
```

`EventCleanupSystem` 在 `TriggerSystems.cs:243`，order 132。监听者不得删除事件实体；漏挂 `TriggerEventMarker` 会泄漏。

不建议的路径：业务系统直接 `CreateRegion` + `TriggerRegisterEnterRegion`。那会让原生 trigger 成为长期语义 owner，和「ECS 持有真相、Native 层只做副作用」冲突；原生事件也不覆盖无句柄实体，还要处理句柄 `HandleAdd` / `HandleRemove` 配对。

## 成本与风险

| 路线 | 成本 | 风险 |
|---|---|---|
| A. 不纳入（推荐） | 无代码。继续用 `GroupHelper` 做即时查询；某个具体系统需要进出时，照 `AuraSystem` 的 diff 写自己的占用，不抽公共 Region | 低。将来若编辑器 region 或 3+ 个进出场景出现，再立项 |
| B. 另立 ECS 进出实施提案 | 新组件、周期系统、两种事件、占用快照、Regression 场景；还要选定与 `SpatialGridSystem`(99) 的先后 | 中。取消提案已判定复用价值存疑；现在仍是 0 个消费方，容易变成无人监听的事件层 |
| C. 原生 region + trigger 桥 | Native 系统持有 `JRegion`、注册 enter/leave、把 `GetEnteringUnit` 映回 ECS，再发 Event | 高。句柄配对、只见原生单位、与 SpatialGrid 双真相、触发器回调不得直接写业务 |

路线 B/C 都是跨运行时公共契约（新事件类型 + 新系统 order），实施时至少 `full`，不能塞进本评估 change。

## 建议

**建议：不纳入。** 标记：**待用户裁决**。

理由：取消记录的需求阈值（3+ 个持久进出场景）仍未达到；`SpatialGridSystem` 已从死代码变成已注册索引；原生 region 相对网格的增量（不规则并集、引擎边沿、编辑器对齐）没有调用方。纳入会重复 2026-09-01 已否决的抽象。

可选裁决（实施不在本 change 内）：

1. **不纳入**（建议）：本评估归档关闭（`skip_specs: true`）。不新增 region 代码。
2. **纳入**：另立实施提案。默认只评估 ECS diff 事件（路线 B），不默认上原生 region（路线 C），除非裁决写明要对接编辑器 region。

## Capabilities

### New Capabilities

- 无。本 change 不新增当前系统行为。

### Modified Capabilities

- 无。`.openspec.yaml` 声明 `skip_specs: true`。不把未批准的区域语义写入 `openspec/specs/`。

## Impact

- `War3Frame/`：不受影响。不改 `SpatialGridSystem`、`AuraSystem`、`GroupHelper`、`JassApi`。
- `War3Frame.Generator/`：不受影响。不新增 `[SystemRegister]` 类型。
- `FrameBuild/`：不受影响。不改构建、模板或 WE。
- `CSharpWar3Frame/`：不受影响。不改 CLI。
- `Projects/`：不受影响。不新增 Regression 场景；`spatial-search` 保持现状。

## 验收标准

- `proposal.md` 引用 `cd77e7c` 与 `openspec/changes/archive/2026-09-11-region-system/proposal.md`，并写明取消原因。
- 写明 `SpatialGridSystem` 与原生 region / 已取消 ECS Region 的分工，以及重叠点。
- 进入/离开需求对齐 `TriggerSystem`(131) / `EventCleanupSystem`(132)，且标明当前 0 个调用方。
- 建议明确为「不纳入」，并标记「待用户裁决」。
- `openspec validate region-support-eval` 通过。
- 提交后停止。不写组件、系统、绑定调用或测试。

## 风险与开放问题

- 若用户实际要对接 WE 里已摆好的 region，路线 A 不够，应在裁决里改选路线 C，而不是在本 change 里补实现。
- `AuraSystem` 的 diff 不是通用事件；「照抄 Aura」只适用于单个、输出已定的系统，不能当成已经存在 Region API。
- 取消文里的 `fix-spatial-grid-registration` 独立提案目录不存在；注册已直接落在 `SpatialGridSystem.cs:13`，本评估不回头补那个 change。

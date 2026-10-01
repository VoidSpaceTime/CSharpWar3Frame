# 提案：AI/任务子系统评估（ai-task-subsystem-eval）

## 状态

- 状态：待用户裁决
- 等级：fast（纯评估，不改行为契约）
- 复盘强度：R0 Direct（系统清单核对 + `openspec validate`）
- 范围：只评估任务队列与 AI 层是否纳入框架。本 change 不包含实现、不写 tasks、不写 spec delta。

## Why

W5.6 要求回答：框架要不要做独立的任务系统或 AI 层。现状是没有独立系统。唯一相关代码是移动结果桥 `MoveToTaskSystem`，它把一次移动的到达/取消写成主体上的 `MoveTaskState`，之后没有消费者。

本提案只冻结系统清单、范围判断、成本和建议。用户裁决前不新增组件、系统、Helper 或行为树。

外部备忘存在，未入库，本评估不依赖其结论。计划提到的 xlik 对标分析只在 OpenViking，不在本仓库；下面的判断只来自仓库文件。

## 现状证据

### 系统清单

2026-10-01，`ECS-Framework`，检索 `War3Frame/Src/Systems` 的 `[SystemRegister`：

| 口径 | 数量 |
|---|---|
| `[SystemRegister` 命中 | 68 处，分布在 32 个文件 |
| 类名或路径含 Task / AI / Ai 的已注册系统 | 0 |
| 名称里带 Task、但不是任务域的系统 | 1：`TimerTaskSystem`（`War3Frame/Src/Systems/Time/TimerTaskSystem.cs`），计时器到期，不是单位命令队列 |

68 处覆盖的是现有域：单位生命周期与移动、属性、Buff、光环、控制、施法与技能槽、物品、伤害击杀奖励、特效、空间网格、触发器、UI 同步、Native 同步。没有 `TaskQueueSystem`、`OrderQueueSystem`、`AiSystem`、`BehaviorTreeSystem` 或同类注册。

`MoveToTaskSystem` 与 `MoveSystem` 同文件（`War3Frame/Src/Systems/Unit/MoveSystem.cs:146-205`），注册为 `[SystemRegister(SystemKind.Immediate)]`。它查询的是 `MoveOutcome` + `MoveContinuation`，只处理 `MoveContinuationKind.ExecuteTask`。到达写 `MoveTaskState.completed`，取消/覆盖/打断/失败写 `cancelled`，然后删掉 outcome 和 continuation。它不排队、不选下一个动作、不调用 War3 命令。

### 桥没有调用方

| 事实 | 位置 | 含义 |
|---|---|---|
| 入口 | `UnitHelper.MoveToTask`（`War3Frame/Src/Helpers/UnitHelper.cs:190-216`） | 写 `MoveCommand` + `MoveContinuationKind.ExecuteTask` |
| 调用 | 全仓 `*.cs` 仅定义处一处 | `Projects/` 与其它 `Src` 都没有调用 |
| 结果 | `MoveTaskState`（`MoveCommand.cs:129-133`） | 注释写明「供任务系统判断」；该任务系统不存在，也没有读取方 |
| 施法对照 | `CastingSystem` 写 `MoveContinuationKind.CastAbility`（`CastingSystem.cs:78-95`）；`MoveToCastSystem`（同文件 `:169`）消费到达后继续施法 | 同一套移动结果已经服务施法，不经过 `MoveToTaskSystem` |

`MoveReason` 里有 `Following`、`Fleeing`、`Patrol`、`AutoAttack`（`MoveCommand.cs:44-52`）。这是移动来源枚举，不是 AI。没有任何系统按这些原因做决策。

## 是否在框架范围

`AGENTS.md` 把框架定成 ECS 与 OOP 混用，分工是：

- 大量同质数据走 ECS：移动、弹道、Buff tick、属性计算。
- 独特、不大量复制的逻辑走 OOP：技能模板、AI 决策、特殊单位行为。
- 施法是混合：ECS 推进前摇/持续/后摇，OOP 定义行为。

「AI 决策」被点名为 OOP 特征，不是「必须先有一个 AI 子系统」。当前战斗、生命周期、Buff、技能、物品、移动已经是框架主体。任务队列和 AI 层会在这些主体之上再加一层「谁在什么时候发命令」。

现有底座已经能表达单次意图和单次结果：

```text
MoveCommand（挂在主体，直到到达/打断）
  -> MoveSystem 推进
  -> MoveOutcome（同主体，token 对齐）
  -> MoveToCastSystem 继续施法
     或 MoveToTaskSystem 只写下 MoveTaskState（无人读取）

CastRequest -> CastingSystem 校验、扣费、写 CastState
```

这覆盖「走过去再施法」。它不覆盖：

1. 命令队列：当前命令被打断或完成后，自动取出下一条。
2. 优先级抢占：玩家命令、施法、逃跑、巡逻谁覆盖谁。现在后写的 `MoveCommand` 直接顶掉前一条（`MoveSystem` 用 token 判 Overridden）。
3. 感知与决策：索敌、巡逻点、逃跑阈值、技能选择。`AGENTS.md` 把这类逻辑归到 OOP，不应做成每帧扫全部单位的 ECS AI 系统，除非同质单位规模大到需要批处理。
4. War3 原生命令队列。框架不把 `IssuePointOrder` 当长期真相；移动只经 `MoveNativeRequest`。再包一层原生 order queue 会把 native 状态当成第二套真相。

因此：单次移动和施法工作流是框架范围内的底座，已经落地。独立任务队列和 AI 层不是当前契约的缺口，而是新的玩法层。没有调用方、没有示例、没有回归场景要求它。

## 成本与风险

- **若另立实施提案，爆炸半径在 `War3Frame/`。** 最小队列就要新组件（队列或当前任务）、一个消费系统、以及和 `MoveCommand` / `CastRequest` 的抢占规则。`[SystemRegister]` 会让生成注册多一个系统，不改 `War3Frame.Generator/` 本身。`FrameBuild/`、`CSharpWar3Frame/` 不受影响。`Projects/` 只在批准实施后补场景。
- **和现有工作流抢所有权。** `CastingSystem` 已经自己写 `MoveCommand`。队列若再写同一组件，施法走位和「任务的下一步」会互相 Overridden。必须先规定谁是唯一写入者，否则施法会被任务取消，或任务会被施法吃掉。
- **AI 默认不该进 ECS 热路径。** 决策是每单位不同的流程，不是均匀数据。做成 QuerySystem 会把独特逻辑摊进每轮扫描；做成 OOP 则要明确它只发已有的 `MoveCommand` / `CastRequest`，不直接调 `JassApi`。
- **`MoveToTaskSystem` 容易被误读成任务系统。** 它只做一次移动的完成标记，没有队列，也没有调用方。保留它的成本低；把它扩成队列会把一个证明用的桥变成第二套施法恢复逻辑。
- **本 change 的风险：** 无代码风险。唯一风险是把评估误读成已批准实施。

## 建议

**建议：不纳入当前框架范围。** 保留 `MoveCommand` / `MoveOutcome` / 施法 continuation 作为唯一的命令底座。不新增任务队列，不新增 AI 系统，不实现 `MoveReason` 里的跟随、逃跑、巡逻、自动攻击。

理由：

- 任务/AI 域已注册系统数量是 0。`MoveToTask` 全仓无调用方，`MoveTaskState` 无读取方。
- 框架已声明的主体是战斗、生命周期、Buff、技能和移动批处理。AI 决策被写成 OOP 特征，不是待补的 ECS 域。
- 施法已经复用同一条移动结果链。再加队列会和 `CastingSystem` 争 `MoveCommand`。
- 没有玩法调用方时，队列和 AI 都是预设抽象。

若用户否决本建议、决定纳入，必须另立实施提案。第一阶段只做：一个主体上的显式命令队列，完成或取消后发下一条已有的 `MoveCommand` 或 `CastRequest`；不接原生 order 队列；不在队列系统里调 `JassApi`；AI 仍由调用方用 OOP 决定下一条命令，不在本阶段实现。

## 待裁决项

**待用户裁决。** 本提案不代用户选择。

- 选项 A：不纳入。归档本评估，不另立实施提案。
- 选项 B：纳入。另立实施提案，范围限于上一节的第一阶段。
- 选项 C：维持现状并挂起。不归档、不实施，等出现具体玩法再开。

未收到用户回复前，本 change 停在审核门。

## Capabilities

### New Capabilities

- 无。纯评估，不新增当前系统行为契约。

### Modified Capabilities

- 无。`.openspec.yaml` 设置 `skip_specs: true`。没有可归档的行为 delta。

## Impact

- `War3Frame/`：仅作证据。不改 `MoveSystem`、`MoveToTaskSystem`、`CastingSystem`、`UnitHelper` 或 `MoveCommand`。
- `War3Frame.Generator/`：无影响。未新增 `[SystemRegister]` 类型。
- `FrameBuild/`：无影响。
- `CSharpWar3Frame/`：无影响。
- `Projects/`：无影响。未加示例或回归场景。
- `openspec/specs/`：无变更。

## 验收标准

- `proposal.md` 覆盖背景、系统清单、范围判断、成本与风险、建议、待裁决项。
- 任务/AI 域系统数量有 `[SystemRegister` 检索依据，并区分 `TimerTaskSystem` 与 `MoveToTaskSystem`。
- 建议明确，同时标为「待用户裁决」，不把建议写成已批准。
- `openspec validate ai-task-subsystem-eval` 通过。
- 不创建实现代码、`tasks.md` 实施清单或 spec delta。

> 裁决（2026-10-01，用户确认）：不纳入；变更归档关闭。

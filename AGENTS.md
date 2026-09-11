# AGENTS.md

## 仓库协作总规则

本仓库采用官方 `OpenSpec`（`@fission-ai/openspec`）作为变更治理机制，统一遵循：

`propose -> review -> apply -> verify -> sync -> archive`

- 任何实现前都必须先有 **OpenSpec 提案**，并经 **用户审核批准**。
- `openspec/specs/<capability>/spec.md` 是当前系统行为的**唯一真相层**；change 目录只放 delta。
- 完成一个变更 = 成功执行 `openspec archive`：合并 delta 进 `openspec/specs/`，并把 change 移入 `openspec/changes/archive/`。
- 仓库文件与工具验证结果是第一事实源；OpenViking 只作为长期上下文辅助。
- 详细流程与命令见 `openspec/README.md`；分级、总结与复盘规则见 `openspec/specs/repository-governance/spec.md`。

## 官方工作流

- 探索（可选）：`/opsx:explore`。
- 提案：`openspec new change` 或 `/opsx:propose`，在 `openspec/changes/<id>/` 下写 `proposal.md`（必要时 `design.md`、`tasks.md`、以及 `specs/<capability>/spec.md` 形式的 delta）。
- 实施：`/opsx:apply`，只实现已批准范围。
- 验证：`openspec validate <change>`，并按风险执行构建、测试或静态检查。
- 归档：`openspec archive <change> --yes`；不影响行为契约的无 delta 变更使用 `--skip-specs`，或在 change 元数据声明 `skip_specs: true`。


## 工件与分级（附加层）

官方机制不强制工件分级。本仓库保留 `fast | light | full | architecture` 作为**建议强度**，用于决定需要哪些工件与复盘强度。该分级是仓库附加约定，**不阻塞官方 `openspec archive`**。

- `fast`：`proposal.md` 即可；总结 2-4 行。
- `light`：`proposal.md`，必要时补 `design.md` / `tasks.md` / spec delta；总结一个短段落。
- `full`：`proposal.md`、`design.md`、`tasks.md`、相关 spec delta；完整总结。
- `architecture`：`full` 全套，并在 `proposal.md` / `design.md` 额外覆盖方案比较、迁移、回滚、阶段拆分。

升级触发器、复盘强度矩阵（`R0~R3`）、审查工具门禁等细则，以 `openspec/specs/repository-governance/spec.md` 为准；`AGENTS.md` 只定义入口规则，不替代 change 内的正式提案记录。

## 归档与生命周期

- 归档即官方“完成动作”，由 `openspec archive` 执行：校验 change 与 delta → 按 `RENAMED → REMOVED → MODIFIED → ADDED` 合并 delta 进 `openspec/specs/` → 移动到 `openspec/changes/archive/<yyyy-MM-dd>-<change-id>/`（日期用归档当天）。
- 归档失败时，官方会回滚主 specs 并把 change 留在原位。
- 若已批准提案的验证计划包含真实 War3 客户端验证：默认阻塞；只有在审核阶段显式声明非阻塞，并在 `summary.md` 记录未执行原因与剩余风险后，才可推迟并归档。
- 状态字段（`待审核 / 已批准 / 实施中 / 已实施 / 已取消 / 已取代`）与 `summary.md` 为仓库附加约定，用于人类可读追踪，不阻塞官方 `archive`。

## 实施后验证与复盘强度（附加层）

提案等级、实施后复盘强度和审查工具启用是三个独立层次。复盘强度使用有序等级：

- `R0 Direct`：直接测试、构建、静态检查或文档验证。
- `R1 Focused`：`R0` + 1 个技术准确性视角。
- `R2 Targeted`：`R0` + 2-3 个与实际风险匹配的专项视角。
- `R3 Comprehensive`：`R0` + 目标/约束、技术质量、安全、QA、上下文五类视角。

默认映射：`fast → R0`；`light → R0`（复杂或版本敏感时 `R1`）；`full → R2`；`architecture → R3`。每个视角都必须有独立证据和 verdict。

以下风险至少要求 `R2`：公共 API / 对外契约、Source Generator 输出、配置/构建/发布契约、持久化与迁移、性能与资源、多系统跨边界协作。命中 `architecture` 等级、安全敏感、重大实现，或用户明确要求完整五路时，使用 `R3`。

工具门禁：`R3` 不自动授权完整 `review-work`；只有用户明确要求“全面复盘”“完整 QA”或直接指定 `review-work` 时才启用。未获授权但必须执行 `R3` 时，用当前获准且可用的方式覆盖五类视角。

任一测试、构建、静态检查或专业复核失败，都不得进入成功总结；必须修复并重新验证，或明确标记为阻塞/未完成。失败本身不机械触发 `R3`，但必须重新判断它是否揭示安全敏感、重大实现或未批准范围。

## 全局影响分析要求

所有提案都要从架构师视角检查本仓库多项目结构，至少说明以下区域是否受影响：

- `War3Frame/`：运行时框架。
- `War3Frame.Generator/`：Source Generator。
- `FrameBuild/`：构建编排。
- `CSharpWar3Frame/`：CLI / 入口项目。
- `Projects/`：示例、测试或集成验证项目。

即使看起来只是局部修改，也要说明为什么其他区域 **不受影响**。

## 架构设计原则

### ECS 与 OOP 混用准则

本框架采用 ECS + OOP 混用方案，不强制纯 ECS 或纯 OOP。
**决策依据是数据访问模式和实体规模，不是功能数量投票。**

**倾向 ECS（QuerySystem + Component + Tag）的特征：**

- 大量同质实体需要每帧批量推进（移动、Buff tick、弹道推进、属性计算）
- 数据访问均匀，可从脏标记机制或批量迭代中获益
- 存在明确的性能热点，需要批处理优化

**倾向 OOP 的特征：**

- 实体逻辑独特、复杂，不会大量复制（技能模板、AI 决策、特殊单位行为）
- 有强父子关系或生命周期绑定（UI 树、Builder、注册表）
- 流程编排语义，而非数据并行（施法状态机、物品 companion 生命周期）

**混用是正确结果，不是架构妥协。**

典型分工：
- 移动 / 弹道 / Buff / 属性计算 → ECS 批处理
- 技能模板行为 / Helper / 注册表 / UI → OOP
- 施法流程（前摇/持续/后摇）→ ECS 推进状态 + OOP 定义行为，混合合理

**触发重新评估的信号**（不是立即重构，而是值得关注的时机）：
- 新增一个同质实体类型超过几百个，但用的是 OOP 对象 → 考虑迁移到 ECS
- 新增一个施法阶段需要同时改 3 个以上 System → 考虑 OOP 状态机重构
- 某个 Helper 同时持有长期状态 + 原生句柄 + 流程控制 → 按原生调用分层规则拆分

## War3 原生调用分层规则

### 总原则

- ECS / 组件 / 工作流层持有长期语义真相。
- War3 原生调用优先集中在专门的 `Native` / `Execution` 层。
- helper 只允许做薄入口与一次性便利调用，不得重新成为长期语义 owner。

### 允许直接调用 War3 原生函数的层

以下层级可以直接调用 `JassApi` / `KKApi` / `YDApi` / `DzApi` 等 War3 原生函数：

- `Systems/Native/*`：原生执行层、同步层、句柄层。
- 明确以 `*ExecutionSystem`、`*NativeSystem` 命名的执行系统。
- 少量 fire-and-forget 的即时 helper（例如一次性短效特效），前提是不承载长期状态真相。

这些调用的职责应该是：

- 执行原生副作用。
- 同步 ECS 真相到原生世界。
- 创建、更新或销毁原生句柄。

### 不应直接调用 War3 原生函数的层

以下层级不得直接拥有 War3 原生调用语义，除非经过额外架构评审并明确说明例外理由：

- 生命周期推进系统。
- 施法、任务、AI、交互等业务工作流系统。
- 纯规则系统（过滤、判定、冷却、数值推进、状态机推进）。
- 持续语义型 helper。

这些层只应负责：

- 产生命令、请求或脏标记。
- 推进 ECS 状态。
- 监听结果与决定下一步流程。

### Helper 规则

- helper 可以包装常用入口，但默认只写 ECS 意图或发一次性请求。
- helper 不得长期持有原生句柄语义。
- helper 不得持续驱动原生行为并同时定义业务真相。
- 若 helper 直接调用原生函数，该调用必须满足“瞬时、无长期语义、无复杂流程回放要求”。

### 推荐结构

- 业务系统 / 工作流层：写 `Command` / `Request` / `State` / `Outcome`。
- Native 执行层：消费 ECS 真相并执行 War3 原生调用。
- 结果桥接层：把原生执行结果重新表达为 ECS outcome，而不是反向把 native 状态当真相。

### 典型正例

- `UnitLifecycleTransitionSystem` 推进阶段，`UnitNativeRemoveSystem` 执行 `KillUnit/RemoveUnit`。
- `UnitNativeSystem` 统一执行血蓝同步，而不是各业务入口直接 `SetUnitState`。
- `EffectHelper` 写 `Position` / `EffectAnimationRequest`，由 `EffectNativeSystem` 同步到原生特效。
- `MoveSystem` / `UnitMoveNativeSystem` 负责原生命令执行，施法系统只消费 move outcome。

### 典型反例

- 在施法系统里直接 `IssuePointOrder`、`KillUnit`、`AddSpecialEffect`。
- 在生命周期推进系统里直接执行终态原生移除。
- 在 helper 里同时持有长期状态、原生句柄和业务流程控制。

### 审查要求

- 任何新增 War3 原生调用，都必须先回答：为什么它不能放进现有 `Native` / `Execution` 层。
- 如果某个系统或 helper 既推进语义状态又直接执行原生副作用，默认判定为高风险设计，需要单独提案审查。
- 若只是一次性、短生命周期、无需重放的便利调用，可以保留在 helper，但必须在代码注释中说明其“非长期语义 owner”身份。

## ECS 消息与 Tag 命名规则

信号事实与请求-响应同时存在，但语义不能合并成一种类型。统一的是生命周期、命名和挂载位置，不是把所有消息收成 `Message`。`Signal` 只作口头说法，代码一律叫 `Event`。

标准链路：

```text
XxxRequest / XxxCommand
  -> Resolve / Workflow System
  -> XxxEvent（对外事实）或 XxxOutcome（该主体工作流结果）
```

触发器是规则（匹配事件 + 条件 + 策略 + 动作），不是第四种消息类型。触发命中后只允许产生已有的 `XxxRequest` / `XxxCommand`，不得在触发回调里直接调用 War3 原生 API。

### 五种消息

| 概念 | 类型 | 挂在哪 | 命名 | 正例 |
|---|---|---|---|---|
| 事实 | `IComponent` | 独立事件实体 | `XxxEvent` | `DamageEvent`、`HealEvent`、`BuffAppliedEvent` |
| 一次性意图 | `IComponent` | 主体或独立请求实体 | `XxxRequest` | `DamageRequest`、`HealRequest`、`CastRequest`、`ItemUseRequest` |
| 持续到完成的意图 | `IComponent` | 主体 | `XxxCommand` | `MoveCommand` |
| 该主体工作流结果 | `IComponent` | 主体自己 | `XxxOutcome` | `MoveOutcome` |
| Native 副作用意图 | `IComponent` | 主体 | `{领域}{动作}NativeRequest` | `UnitCreateNativeRequest`、`MoveNativeRequest` |

规则：

- `Request`：一次性，由唯一 Resolve 消费后删除；不能当成已经成功。
- `Command`：挂在主体上直到完成或取消。
- `Event`：独立实体，只读，允许多监听者；由统一清理系统删除，监听者不得删除。
- `Outcome`：挂主体，只给该主体后续流程看（施法、任务）；同实体同类型通常一份，用 token 对齐发起方。
- 不要把 `Outcome` 收进 `Command` 或 `Request`。`Command` 还在表示未完成；`Request` 表示意图。`Outcome` 与 `Event` 都是已发生，差别只在广播范围。

### Tag 规则

`ITag` 只用于零数据的分类、脏标记或同实体内部阶段。有字段就必须用 `IComponent`。

| 用途 | 命名 | 正例 |
|---|---|---|
| 持续分类状态 | `XxxTag`，或稳定领域名词 | `MovingTag`、`ItemGroundTag`、`Buff`、`Aura` |
| 需重算 | `XxxDirty` | `AttrDirty`、`AbilityStatDirty`、`LevelStatDirty` |
| 同实体内部阶段 | 过去式：`XxxExpired` / `XxxArrived` / `XxxCompleted` | `TimerExpired`、`BuffExpired`、`ProjectileArrived`、`EffectCompleted` |

后缀判定：分类状态一律 `XxxTag` 结尾（`EffectPendingTag`，不用现在式裸名）；`XxxDirty` 只用于需重算标记；过去式阶段**不加** `Tag`（`TimerExpiredTag` 冗余）；核心稳定领域名词（`Buff`、`Aura`）可豁免不加后缀。

禁止：

- 用 Tag 对外广播、给多个无关系统监听，或表达同帧可重复发生的事实 → 用独立 `XxxEvent` 实体。
- 把意图做成 `ITag`，即使零数据也不行 → 用 `IComponent` 的 `XxxRequest`。
- Tag 名字带 `Request` 或 `Event`。
- 让全局 Trigger 去扫内部阶段 Tag（如 `ProjectileArrived`）。内部阶段只给拥有这条生命周期的实体看；对外监听另发 `XxxEvent`。

脏标记例外：无载荷用 `ITag`（`AttrDirty`）；有载荷用 `IComponent`（`EffectDirty` 带 flags）。

### 新代码检查清单

1. 已经发生还是希望发生？ → `Event` / `Request`
2. 有没有字段？ → 有字段必 `IComponent`
3. 谁消费、能否多次、要不要多监听？ → 多次或多监听 = 独立 `XxxEvent` 实体
4. 只是给本实体分类或标阶段、且零数据？ → `ITag`，名字不得带 `Request`/`Event`

### 系统 Order 契约

- 事件监听系统 order 必须小于 132：`EventCleanupSystem`（order 132）是全仓事件清理边界，删除所有带 `TriggerEventMarker` 的事件实体；order ≥ 132 的监听系统在清理后读不到事件实体。
- 新增事件类型创建点必须挂 `TriggerEventMarker`，否则事件实体不会被清理，造成泄漏。

### 历史命名，新代码不要复制

内部阶段继续用 `ProjectileArrived` / `ProjectileExpired`；若剧情或其他系统要监听命中，另发独立 `ProjectileHitEvent`。零数据意图必须是 `IComponent` 的 `XxxRequest`，不得再把 `Request` 做成 `ITag`。

## Native 同步三模式规则

ECS 是唯一运行时真相，War3 原生对象只是同步后的代理。ECS 状态与原生之间的同步按数据特征选择三种模式之一，不得随意混用。

### 模式决策树

| 数据特征 | 模式 | 机制 | 示例 |
|---|---|---|---|
| 高频、连续、常态化变化，多路径写入 | **Compare-Sync** | Native 系统每轮刷新对比 ECS 值与上一快照，只同步有意义的差异；业务写入点无需打标记 | 单位血量/法力（`UnitNativeSyncRegistry` + `UnitNativeSystem`） |
| 低频、离散、集中修改的持久状态 | **Dirty-Driven** | 修改入口写 ECS 状态 + 附加 Dirty 标记；Native 系统只处理带标记实体，同步后清除标记 | 特效外观（`EffectBase` + `EffectDirty` + `EffectTransform`）、玩家名称/颜色、物品状态 |
| 一次性、无持久状态的副作用 | **Request** | 写入 `XxxRequest`，Native 系统消费后删除；不保存最终值 | 动画播放、特效销毁、单位创建、移动命令 |

### 判定规则

1. 该字段会被多个系统从不同路径修改，且追踪写入点成本高 → **Compare-Sync**
2. 修改点集中（通常在 Helper/Modifier 内），大部分时间不变，且状态需要被查询 → **Dirty-Driven**
3. 执行完即可忘记、无需回读最终值 → **Request**

### Dirty 契约

- Dirty 不是业务状态，只表示"ECS 状态等待同步到原生"。
- 同一实体同帧多次修改：合并 flags（按位 OR），同步完成后统一清除。
- 有载荷脏标记用 `IComponent`（`EffectDirty` 带 flags）；无载荷脏标记用 `ITag`（`AttrDirty`）。
- 累积型状态（旋转/位移/计数器）必须存状态组件（如 `EffectTransform`），不允许只用一次性 Request 表达，否则 ECS 丢失真相。

### 修改入口与分层

```
ECS 组件层（数据）  ← 业务系统内部受控写入
Helper 层（入口）   ← 对外唯一修改入口：写 ECS 状态 + 打 Dirty / 写 Request
Modifier 层（链式） ← Helper 之上的流式封装，返回 this，不绕过 Helper
Native 层（执行）   ← 消费 Dirty/Request，调用 War3 API，不承担业务决策
```

- 对外业务调用一律走领域 Helper（`EffectHelper` / `PlayerHelper` / `ItemHelper` 等）；Helper 负责在修改持久状态后立即打 Dirty 或写 Request，集中维护字段与 Dirty flag 的对应关系，防止调用方漏标。
- 链式 `XxxModifier` 建立在 Helper 之上，返回自身支持连续调用；不得绕过 Helper 直接操作原生 API。
- 不绝对禁止系统内部直接写组件（结算、初始化、同领域核心逻辑可以），但涉及需要同步到原生的字段时，必须满足对应模式的契约（Compare-Sync 无需标记；Dirty-Driven 必须打标；Request 必须写请求组件），否则视为违规。
- Native 系统只消费 ECS 状态/请求并调用 War3 API，不承担业务决策。

### 迁移原则

- 新代码按决策树选型，禁止默认全用 Compare-Sync 或全用 Request。
- 已有代码迁移按领域分批（先 Effect，再 Player/Item），每批核对全部写入点后改造，不做机械全局替换。
- 高频数值（血量/蓝量）保持 Compare-Sync，不为形式统一改成 Dirty。

## 原生句柄引用配对规则（检查提醒）

创建原生对象后必须 `HandleHelper.HandleAdd` 登记（对应 lik 的 `HandleRef`），销毁前必须 `HandleHelper.HandleRemove` 注销（对应 `HandleUnRef`），配对在同一 Native 系统内相邻完成。

代码审查时逐项核对：

- [ ] 创建原生对象（`CreateUnit` / `AddSpecialEffect` / `CreateItem` / `CreateTrigger` 等）后，下一行是否立即 `HandleAdd`？
- [ ] 销毁原生对象（`RemoveUnit` / `DestroyEffect` 等）前，是否相邻调用 `HandleRemove`？
- [ ] 同一对象类型是否只有唯一销毁执行点（同一 Native 系统 / 同一函数）？销毁路径分散 = 必然有一条漏注销。
- [ ] 新增原生对象类型时，创建系统与销毁系统是否成对添加、配对登记/注销？

## 执行要求

### 1. Design（提案）

- 先确定提案等级。
- 用 `openspec new change` / `/opsx:propose` 建立 `openspec/changes/<id>/`，在 `proposal.md` 留下正式提案记录，并按等级补齐 `design.md`、`tasks.md`、spec delta。
- 提案必须先覆盖目标、边界、风险、验证。

### 2. Review

- 提交给用户审核。
- 用户未明确批准前，不进入实现阶段。

### 3. Implement

- 只能实现已批准范围内的内容。
- 发现新增范围，立即回到提案阶段升级。

### 4. Test

- 至少执行与改动对应的验证：文档检查、类型检查、构建、测试或局部运行验证。
- 不允许以“理论上可行”代替实际验证。

### 5. Summarize

- 按等级写 `summary.md`：`fast` 2-4 行；`light` 一个短段落；`full` 完整总结；`architecture` 架构级完整总结（含阶段结果、迁移状态、剩余风险）。
- `summary.md` 是仓库附加质量证据，不阻塞官方 `archive`。

### 6. Archive / Commit

- 归档：`openspec archive <change> --yes`；无 delta 的变更使用 `--skip-specs`。
- 只有在用户明确要求提交时才允许 commit。
- 未经用户要求，不主动创建 git commit。

## 模板与入口

- 官方工作流与命令：`openspec/README.md`
- 仓库附加治理规则（分级 / 复盘 / 总结）：`openspec/specs/repository-governance/spec.md`
- 工件模板由官方 CLI 提供（`openspec templates`）；仓库不再维护独立模板文件。
- 历史治理变更：`openspec/changes/archive/`（以日期前缀定位）
- 当前活跃变更：以 `openspec list` 为准。

## 特别说明

- 本仓库已初始化 `openspec/`（`openspec/config.yaml`，schema: `spec-driven`），后续用 `openspec update` 刷新，不重复 `init`。
- 如果外部记忆、历史对话与仓库文件冲突，以仓库当前内容为准。
- 如果用户只要求讨论、评估、审查，则先分析，不直接实现。

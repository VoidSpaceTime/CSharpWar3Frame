# AGENTS.md

## 仓库协作总规则

本仓库已经引入 `OpenSpec`，未来所有代码改动、架构调整、治理更新与跨项目修改，统一遵循：

`design -> review -> implement -> test -> summarize -> commit`

- 任何实现前都必须先有 **OpenSpec 提案**。
- 任何实现前都必须先经过 **用户审核批准**。
- 未经批准，不得直接改代码、补实现、顺手修复或提交 commit。
- 仓库文件与工具验证结果是第一事实源；OpenViking 只作为长期上下文辅助。

## OpenSpec 分级治理

### Level 0: fast

适用于低风险、强局部、可快速回滚的改动，例如：

- 注释、文案、命名微调。
- 不改变公共契约的局部样式或格式整理。
- 明显低风险的单点 bug 修复。

最小提案内容：

- 正式记录在 `openspec/changes/<change-id>/proposal.md`。
- 变更目标。
- 影响文件。
- 为什么判定为低风险。
- 如何验证。

### Level 1: light

适用于单模块或少量文件内的常规改动，不涉及跨项目架构边界，例如：

- 已有模块内新增小能力。
- 局部逻辑优化。
- 小范围重构。

最小提案内容：

- 正式记录在 `openspec/changes/<change-id>/proposal.md`。
- 背景与目标。
- 影响范围。
- 方案摘要。
- 风险与回滚。
- 验收标准。

补充规则：

- `light` 默认不要求完整四件套。
- 当方案存在多步骤、边界条件、局部规格约束或仅凭 `proposal.md` 无法完成审查时，再补 `tasks.md`、`design.md` 或相关 `spec.md`。

### Level 2: full

适用于需要完整全局分析的改动，例如：

- 跨模块改动。
- 公共 API / 生成器输出契约变更。
- 构建流程、核心业务流程、公共数据结构调整。
- 可能影响多个项目边界的实现。

必须补齐：

- `proposal.md`
- `design.md`
- `tasks.md`
- 对应 `specs/.../spec.md`

### Level 3: architecture

适用于架构级事项，例如：

- 基础设施引入或替换。
- 目录结构 / 分层边界重构。
- 跨项目依赖关系调整。
- 框架迁移、领域模型重划、长期治理策略调整。

除 `full` 级工件外，还必须明确：

- 备选方案比较。
- 迁移路径。
- 阶段拆分。
- 回滚策略。
- 长期维护影响。

## 分级判定规则

满足任一项，至少按 `full` 处理：

- 改公共接口或对外契约。
- 改生成器输出、构建链路或项目依赖关系。
- 改核心业务流程、全局状态流转、跨模块协作。
- 改持久化结构、配置规范、安全边界。
- 影响 `War3Frame`、`War3Frame.Generator`、`FrameBuild`、`CSharpWar3Frame`、`Projects/*` 中两个及以上区域。

满足任一项，可考虑按 `fast` 处理：

- 不改变行为语义。
- 不改变接口契约。
- 局部即可验证。
- 可快速回滚。

其余默认按 `light` 处理。

如果分析过程中发现范围扩大，必须 **自动升级提案等级**，重新提交审核。

## 工件矩阵

- `fast`：必须 `proposal.md`；`design.md`、`tasks.md`、相关 `spec.md` 按需要补充。
- `light`：必须 `proposal.md`；当存在多步骤、边界条件、局部规格约束或审查复杂度提高时，再补 `design.md`、`tasks.md`、相关 `spec.md`。
- `full`：必须 `proposal.md`、`design.md`、`tasks.md`、相关 `spec.md`。
- `architecture`：必须具备 `full` 全套工件，并在 `proposal.md` 与 `design.md` 中额外覆盖方案比较、迁移、回滚与阶段拆分。

规则说明：

- 所有等级都必须在对应 change 的 `proposal.md` 中留痕。
- `AGENTS.md` 只定义入口规则，不替代 change 内的正式提案记录。
- 若文档之间对工件要求有冲突，以 `openspec/.../spec.md` 中的 capability requirement 为准。

## OpenSpec 提案生命周期与归档规范

### 提案生命周期状态

所有提案在 `proposal.md` 中必须显式标注以下状态之一：
- `待审核`：提案已提交，等待用户批准。
- `已批准`：用户已批准，尚未开始实施。
- `实施中`：正在进行代码编写或文档修改。
- `已实施`：变更已完成实施、验证与总结。
- `已取消`：提案被放弃，不再实施。
- `已取代`：提案被新的提案替代。

### 实施完成（已实施）的硬性要求

一个变更要标记为 `已实施`，必须同时满足以下条件：
1. 批准的变更范围已全部完成。
2. 要求的验证（如测试、构建、静态检查）已全部通过。
3. 实施后总结已完成，且必须在 change 目录下存在 `summary.md`。
4. 无阻塞性未完成项。

`R0/R1/R2/R3` 证据和 verdict 用于支持验证，但绝不能替代 `summary.md`。

### 归档前置条件与规则

1. 归档位置（`openspec/changes/archive/`）表示变更已关闭且不再活跃，并不等同于已实施。
2. 状态为 `已实施` 的变更，在满足上述实施完成要求后可以归档。
3. 状态为 `已取消` 或 `已取代` 的变更，只要在 `proposal.md` 中记录了关闭原因，即可直接归档，不要求提供 `summary.md`。
4. 归档目录命名规范：`openspec/changes/archive/<yyyy-MM-dd>-<change-id>/`，日期使用归档当天。
5. 若已批准提案的验证计划包含真实 War3 客户端验证，该验证默认是阻塞的。只有在审核阶段显式声明为非阻塞，并在 `summary.md` 中记录未执行原因与剩余风险时，才允许推迟该验证并归档。

## 实施后验证与复盘强度

OpenSpec 提案等级、实施后复盘强度和审查工具启用是三个独立层次：提案等级决定治理工件与默认强度，实际风险可以提高最终强度，具体工具还必须满足独立授权与可用性要求。实施前的 `review` 仍是用户批准门禁；实施后的直接验证与专业复盘属于 `test` 阶段，不能相互替代。

复盘强度使用有序等级：

- `R0 Direct`：直接测试、构建、静态检查或文档验证。
- `R1 Focused`：在 `R0` 基础上增加 1 个技术准确性视角。
- `R2 Targeted`：在 `R0` 基础上增加 2-3 个与实际风险匹配的专项视角。
- `R3 Comprehensive`：在 `R0` 基础上覆盖目标/约束、技术质量、安全、QA、上下文五类视角。

每个视角都必须有独立证据和 verdict；计数单位不是代理数量或工具调用次数。

默认强度如下：

- `fast`：`R0 Direct`。
- `light`：默认 `R0`；当变更仍满足 `light` 边界，但包含多步骤技术推理、版本敏感事实或技术事实不确定性时，必须使用 `R1 Focused`。
- `full`：`R2 Targeted`，按风险选择 2-3 路专项复核。
- `architecture`：`R3 Comprehensive`，必须覆盖完整五类视角。

以下风险至少要求 `R2 Targeted`，但不会仅因分类名称自动触发 `R3`：

- 公共 API 或对外行为契约。
- Source Generator 输出契约。
- 配置格式、构建链或发布契约。
- 持久化、迁移、数据兼容性或数据丢失可能性。
- 性能、资源、实时性或大规模数据影响。
- 多系统、多项目或跨边界状态协作。

命中以下任一条件时，必须使用 `R3 Comprehensive`：

- 提案等级为 `architecture`。
- 涉及权限、认证授权、敏感数据、不可信外部输入、供应链，或具有可利用后果的 native / 进程边界等安全敏感事项。
- 属于改变核心行为或架构边界、影响半径较大、涉及复杂跨项目迁移、难以快速回滚，或失败会显著影响运行与交付的重大实现。
- 用户明确要求完整五路或更高强度。
- 更高优先级 system / developer 指令要求。

不得仅按目录名、文件数量或代码行数判定重大实现；普通原生调用也不自动等同于安全敏感。如果新风险超出已批准提案范围，必须先修订或升级 OpenSpec 并重新取得用户批准，不能只增加复盘路数后继续实施。

工具门禁：

- `light/R1` 的技术准确性复核在 Oracle 可用时优先使用 Oracle；不可用时可使用等价复核，但必须记录替代原因、证据和 verdict。
- 完整五路复盘是一种强度要求，不等同于完整 `review-work`。
- 不得仅因 `architecture`、安全敏感、重大实现或其他 `R3` 要求自动启用完整 `review-work`。
- 只有用户明确要求“全面复盘”“完整 QA”或直接指定 `review-work` 时，才允许启用完整 `review-work`；更高优先级 system / developer 指令要求时必须遵守。
- 未获完整 `review-work` 授权但必须执行 `R3` 时，使用当前获准且可用的检查方式覆盖五类视角。

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
- `MoveSystem` / 后续 `MoveNativeExecutionSystem` 负责原生命令执行，施法系统只消费 move outcome。

### 典型反例

- 在施法系统里直接 `IssuePointOrder`、`KillUnit`、`AddSpecialEffect`。
- 在生命周期推进系统里直接执行终态原生移除。
- 在 helper 里同时持有长期状态、原生句柄和业务流程控制。

### 审查要求

- 任何新增 War3 原生调用，都必须先回答：为什么它不能放进现有 `Native` / `Execution` 层。
- 如果某个系统或 helper 既推进语义状态又直接执行原生副作用，默认判定为高风险设计，需要单独提案审查。
- 若只是一次性、短生命周期、无需重放的便利调用，可以保留在 helper，但必须在代码注释中说明其“非长期语义 owner”身份。

## 执行要求

### 1. Design

- 先确定提案等级。
- 在 `openspec/changes/<change-id>/proposal.md` 中留下正式提案记录，并按等级补齐对应工件。
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

- `fast`：可用 2-4 行短摘要，写清实际改动、验证结果、是否有遗留风险或后续事项。
- `light`：可用一个短段落或少量要点，写清改动范围、验证结果、是否需要后续提案。
- `full`：保持完整总结，说明实际改动范围、全局影响、验证覆盖、风险与后续建议。
- `architecture`：在 `full` 基础上，额外说明阶段结果、迁移状态、剩余风险与未完成事项。
- 所有进入 `已实施` 状态的变更都必须完成 `summarize` 阶段并生成 `summary.md`；`已取消` 或 `已取代` 的变更可跳过此阶段，但须在 `proposal.md` 中记录关闭原因。

### 6. Commit

- 只有在用户明确要求提交时才允许 commit。
- 未经用户要求，不主动创建 git commit。

## 模板与入口

- 分级提案模板：`openspec/templates/proposal-levels.md`
- 审核检查清单：`openspec/templates/review-checklist.md`
- OpenSpec 使用说明：`openspec/README.md`
- 历史治理变更：`openspec/changes/archive/2026-08-13-establish-openspec-governance/`
- 历史治理澄清变更：`openspec/changes/archive/2026-08-13-clarify-graded-governance-artifact-rules/`
- 当前活跃变更：`openspec/changes/define-openspec-implemented-and-archive-markers/`

## 特别说明

- 本仓库已经有 `openspec/`，后续一律复用，不重复初始化。
- 如果外部记忆、历史对话与仓库文件冲突，以仓库当前内容为准。
- 如果用户只要求讨论、评估、审查，则先分析，不直接实现。

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

## 全局影响分析要求

所有提案都要从架构师视角检查本仓库多项目结构，至少说明以下区域是否受影响：

- `War3Frame/`：运行时框架。
- `War3Frame.Generator/`：Source Generator。
- `FrameBuild/`：构建编排。
- `CSharpWar3Frame/`：CLI / 入口项目。
- `Projects/`：示例、测试或集成验证项目。

即使看起来只是局部修改，也要说明为什么其他区域 **不受影响**。

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
- 所有等级都不得跳过 `summarize` 阶段。

### 6. Commit

- 只有在用户明确要求提交时才允许 commit。
- 未经用户要求，不主动创建 git commit。

## 模板与入口

- 分级提案模板：`openspec/templates/proposal-levels.md`
- 审核检查清单：`openspec/templates/review-checklist.md`
- OpenSpec 使用说明：`openspec/README.md`
- 当前治理变更：`openspec/changes/establish-openspec-governance/`
- 当前治理澄清变更：`openspec/changes/clarify-graded-governance-artifact-rules/`

## 特别说明

- 本仓库已经有 `openspec/`，后续一律复用，不重复初始化。
- 如果外部记忆、历史对话与仓库文件冲突，以仓库当前内容为准。
- 如果用户只要求讨论、评估、审查，则先分析，不直接实现。

## ADDED Requirements

### Requirement: OpenSpec approval before implementation
仓库中的任何代码变更、架构调整、跨项目改动或治理文件更新，在实施前都 MUST 先创建对应的 OpenSpec 变更，并生成 proposal、design、specs 与 tasks 等所需工件，提交给用户审核；在用户明确批准之前，系统 SHALL NOT 进入实际实现阶段。

#### Scenario: Implementation request arrives without approved proposal
- **WHEN** 协作者或 AI 收到一个需要修改仓库内容的请求
- **THEN** 系统 MUST 先创建或补齐对应的 OpenSpec 变更工件，并停止在待审核状态，直到用户批准后才允许实施

### Requirement: Architect-level global impact analysis
每一个 OpenSpec proposal 与 design 都 MUST 从架构师视角评估全局影响，而不能只描述局部改动。该分析 MUST 覆盖运行时框架 `War3Frame`、Source Generator `War3Frame.Generator`、构建编排 `FrameBuild`、CLI 入口 `CSharpWar3Frame` 与样例项目 `Projects/*` 的相关影响边界，并明确哪些部分受影响、哪些部分不受影响。

#### Scenario: Proposal for a localized change
- **WHEN** 某个变更看似只涉及单个项目或单个模块
- **THEN** proposal 与 design 仍 MUST 明确说明该改动对其他关联项目是否有影响，以及为什么可以判定为无影响或低影响

### Requirement: Reuse existing OpenSpec installation
如果仓库已经存在可用的 OpenSpec 目录与工具链，后续治理或功能变更 SHALL 复用既有 OpenSpec 结构，而 MUST NOT 重复初始化或生成无必要的重复脚手架。

#### Scenario: Repository already contains openspec directory
- **WHEN** 协作者在仓库根目录发现现有 `openspec/` 目录且相关 CLI 可用
- **THEN** 系统 MUST 直接在既有结构内创建或更新变更，而不是重新执行初始化流程

### Requirement: Repository files remain source of truth
仓库文件与工具验证结果 MUST 作为项目当前状态的第一事实源；OpenViking 记忆 SHALL 仅作为长期上下文辅助，不得替代仓库内实际文件、规格与验证结果。

#### Scenario: Memory conflicts with repository state
- **WHEN** OpenViking 中的历史记忆与仓库当前文件状态不一致
- **THEN** 系统 MUST 以仓库当前文件和验证结果为准，并将 OpenViking 只作为解释上下文而不是事实判定依据

### Requirement: Graded OpenSpec proposal levels
仓库中的每一次变更都 MUST 先经过 OpenSpec 提案与用户审核，但提案深度 SHALL 根据改动风险、影响范围、可逆性与跨项目程度分级为 `fast`、`light`、`full`、`architecture` 四级。系统 MUST 先判级，再生成对应深度的工件，并在发现范围扩大时自动升级等级。

#### Scenario: Low-risk localized change uses fast proposal
- **WHEN** 变更仅涉及局部注释、文案、命名整理、低风险单点修复或不改变公共契约的微小调整
- **THEN** 系统 MAY 使用 `fast` 级提案，但仍 MUST 记录目标、影响文件、低风险理由与验证方式，并等待用户批准后才能实施

#### Scenario: Single-module bounded change uses light proposal
- **WHEN** 变更主要局限在单个模块或少量关联文件内，且不改变跨项目架构边界、公共接口或持久化契约
- **THEN** 系统 MUST 使用 `light` 级提案，并至少说明背景目标、影响范围、方案摘要、风险回滚与验收标准

#### Scenario: Cross-module or contract change uses full proposal
- **WHEN** 变更涉及跨模块协作、公共 API、核心业务流程、生成器输出契约、构建流程或其他需要全局兼容性分析的内容
- **THEN** 系统 MUST 使用 `full` 级提案，并补齐 proposal、design、tasks 及相关 specs，明确全局影响、兼容性与验证计划

#### Scenario: Architecture-level change uses architecture proposal
- **WHEN** 变更涉及目录体系重构、基础设施引入、框架迁移、领域边界重划、跨项目依赖重组或其他架构级决策
- **THEN** 系统 MUST 使用 `architecture` 级提案，并提供备选方案比较、迁移路径、回滚策略、阶段拆分与长期维护影响分析

#### Scenario: Scope grows during implementation planning
- **WHEN** 某个原本按 `fast` 或 `light` 级处理的变更在分析过程中发现涉及公共契约、跨项目影响或高风险依赖
- **THEN** 系统 MUST 立即升级提案等级、补齐缺失工件，并重新提交用户审核后才允许继续实施

### Requirement: Unified lifecycle after proposal approval
无论采用哪一个提案等级，后续执行顺序都 MUST 统一遵循 `design -> review -> implement -> test -> summarize -> commit` 生命周期；等级差异仅体现在设计与审查工件的深度，而不得跳过审核、验证或总结步骤。

#### Scenario: Fast proposal approved by user
- **WHEN** 用户批准一个 `fast` 级提案
- **THEN** 系统 MUST 先完成对应层级的设计说明与 review 结论，再进入实现、验证、总结，并且在用户未要求前不得擅自提交 commit

#### Scenario: Full proposal approved by user
- **WHEN** 用户批准一个 `full` 或 `architecture` 级提案
- **THEN** 系统 MUST 按既定设计与任务拆分执行，完成测试与总结后才可进入可能的提交阶段，且每一步都 SHALL 保持与已批准提案一致

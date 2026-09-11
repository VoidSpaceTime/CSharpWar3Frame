## Context

当前仓库已经具备 `openspec/` 目录与可用的 OpenSpec CLI，但缺少一个位于仓库根层、对后续协作者和 AI 代理都直接可见的治理入口文件。与此同时，仓库是一个多项目 .NET 方案，包含运行时框架、Source Generator、构建编排、CLI 与示例项目，任何局部改动都可能带来跨项目影响，因此治理要求不能只约束“写代码前先提 proposal”，还需要要求 proposal 显式评估全局架构影响。

用户还明确要求：如果 OpenSpec 已存在则跳过重复引入，仅补齐缺失治理项；未来任何代码变更都必须先提案、交由用户审核，通过后才允许实施。

## Goals / Non-Goals

**Goals:**
- 建立仓库根级治理入口，让所有后续协作都能直接看到并遵守 OpenSpec-first 流程。
- 将“proposal → 用户审核 → implementation”固化为实施前置条件，而不是口头约定。
- 建立按风险分级的提案强度机制，避免低风险小改也被完整重流程拖慢。
- 约束后续提案必须从架构师视角评估全局影响，覆盖 `War3Frame`、`War3Frame.Generator`、`FrameBuild`、`CSharpWar3Frame` 与 `Projects/*` 的关系边界。
- 明确 OpenSpec、仓库文件、OpenViking 三者职责，避免以后把记忆系统误当成事实源。

**Non-Goals:**
- 不在本次变更中引入新的运行时代码、生成器逻辑、构建逻辑或 CLI 功能。
- 不在本次变更中补写完整的所有 capability specs；仅补齐治理 capability 所需的最小规格。
- 不把 OpenViking 深度接入仓库运行时或构建流程。

## Decisions

### 1. 使用根级 `AGENTS.md` 作为治理入口
**Decision:** 在仓库根目录新增 `AGENTS.md`，作为对人类协作者与 AI 代理都可见的第一治理文件。

**Rationale:** 与仅存在于会话上下文或外部记忆相比，仓库内根级文件更稳定、可审计、可随代码版本一起演化。

**Alternatives considered:**
- 仅依赖 OpenViking 记忆：不可审计，且无法保证新协作者可见。
- 仅依赖 `openspec/` 内部文件：对“开始任何工作前先看什么”这件事不够直接。

### 2. 采用“已有 OpenSpec 则复用，不重复初始化”的策略
**Decision:** 本次治理变更不重新初始化 OpenSpec，只在已有 `openspec/` 基础上新增治理变更与根级规则文件。

**Rationale:** 当前仓库已存在 `openspec/` 且 CLI 可用，重复初始化会制造噪音与潜在冲突，也违背用户“如果已经引入则跳过”的要求。

**Alternatives considered:**
- 重新执行初始化：会造成重复脚手架，且无新增价值。

### 3. 将“全局架构影响评估”定义为提案强制要求
**Decision:** 对未来 proposal 的要求不止是描述局部改动，还必须明确跨项目影响边界、依赖关系、兼容性与实施顺序。

**Rationale:** 当前仓库是多项目方案，局部改动可能影响生成器注册、构建编排、CLI 入口和示例工程。若 proposal 不做全局分析，后续实现阶段容易出现返工和设计漂移。

**Alternatives considered:**
- 仅要求最小 proposal：速度快，但治理价值不足，无法满足用户的架构师视角要求。

### 4. 明确事实源分层
**Decision:** 仓库文件是第一事实源，OpenSpec 是变更约束与审计源，OpenViking 是长期记忆辅助源。

**Rationale:** 这能避免将外部记忆误当当前实现真相，也能让治理规则与代码状态保持一致。

### 5. 引入分级提案机制，但不放松审核门槛
**Decision:** 保留“实现前必须提案并等待用户审核”这一硬门槛，同时把提案深度分成 `fast`、`light`、`full`、`architecture` 四级。

**Rationale:** 当前最大问题不是是否需要治理，而是所有变更都按同等重度处理，导致低风险改动等待时间过长。分级后可以保持审批权和架构约束不变，只缩短低风险改动的提案准备时间。

**Alternatives considered:**
- 对所有改动继续统一使用完整 proposal：治理最简单，但吞吐量差，小改动成本过高。
- 允许小改动完全绕过 OpenSpec：速度快，但会破坏“先设计后实现”的统一制度，长期不可审计。

## Proposal Level Matrix

| Level | Typical Scope | Required Artifacts | Review Depth |
|---|---|---|---|
| `fast` | 文案、注释、命名、低风险单点修复 | 简版提案记录（目标/文件/风险/验证） | 快速审核 |
| `light` | 单模块或少量文件内常规改动 | 轻量提案（背景/范围/方案/风险/验收） | 常规审核 |
| `full` | 跨模块、公共契约、全局兼容性变更 | `proposal.md` + `design.md` + `tasks.md` + `spec.md` | 完整审核 |
| `architecture` | 架构级设计、边界重组、基础设施调整 | `full` 全套 + 方案比较/迁移/回滚/阶段拆分 | 架构审核 |

## Decision Tree

1. 是否涉及公共契约、构建链路、生成器输出、跨项目依赖或两个以上顶层项目？若是，至少 `full`。
2. 是否涉及架构边界、目录重组、长期治理或迁移？若是，使用 `architecture`。
3. 是否只是低风险局部改动且可快速回滚？若是，可用 `fast`。
4. 其余默认 `light`。

若在分析或实现准备过程中发现作用域扩大，必须立即升级提案等级并重新审核。

## Risks / Trade-offs

- [风险] 协作者错误地把高风险改动降级为 `fast` 或 `light` → [缓解] 提供明确升级触发器与审核清单，并要求在分析出新影响时自动升级。
- [风险] 治理规则过重，导致小改动也需要完整提案流程 → [缓解] 使用分级机制控制工件深度，而不是放弃提案与审核。
- [风险] 根级治理文件与 OpenSpec 规格文件内容漂移 → [缓解] 在治理文档中规定：流程规则变更时，优先更新 OpenSpec 变更，再同步根级文件。
- [风险] 协作者把 OpenViking 记忆当作仓库事实 → [缓解] 在规则中明确其仅为辅助上下文，实际结论必须以仓库文件和工具验证为准。

## Migration Plan

1. 创建治理 OpenSpec 变更并补齐 proposal/design/specs/tasks。
2. 在治理规则中加入提案等级矩阵、升级触发器与统一生命周期。
3. 待用户审核通过后，再新增根级 `AGENTS.md`、模板文件与 `openspec/README.md`。
4. 验证新增文档存在、路径正确、OpenSpec 变更可被状态命令识别。
5. 后续所有代码变更都复用该治理基线执行。

## Open Questions

- 当前仓库是否还需要补充更细的 capability-level specs（例如 runtime、generator、build orchestration）作为下一轮治理工作的一部分；本次先不扩展，等待用户后续指令。

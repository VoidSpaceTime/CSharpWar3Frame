## Why

当前仓库已经建立了 `fast / light / full / architecture` 四级治理模型，但仍存在两个会在执行中持续放大成本的歧义点。

第一，`fast` 与 `light` 级提案虽然被定义为轻量化路径，但其正式留痕位置尚未被写死，导致执行者可能把说明写在临时对话、`AGENTS.md`、README 或其他非标准位置，从而失去可审计性。

第二，不同文档对“各等级需要哪些工件”的措辞还不完全统一。尤其是 `README` 中“根据等级补齐 proposal.md、design.md、tasks.md 和 specs/.../spec.md”的说法，容易被理解为所有等级都必须补齐四件套，这会把轻量级提案重新拖回重流程。

本次变更的目标不是重设计治理制度，而是对现有四级制度进行释义与收口：统一 `fast/light` 的标准记录载体，统一四级工件矩阵，并明确当 README、模板与 spec 文字不一致时，以哪一个为准。

## What Changes

- 澄清 `fast` 与 `light` 级提案的标准留痕位置与最小记录格式。
- 建立单一的 “proposal level -> required artifacts” 工件矩阵，并将其定义为仓库内统一口径。
- 规定治理规则冲突时的优先级：规范源、入口文档、模板文档之间如何解释。
- 只调整治理文档与 OpenSpec 规格，不改变现有四级制度、审批流程或生命周期顺序。

## Capabilities

### Modified Capabilities
- `repository-governance`: 补充分级提案留痕规则、工件映射规则与文档冲突解释规则。

## Impact

- 影响范围仅限仓库治理文档与 OpenSpec 规格。
- 不修改 `War3Frame`、`War3Frame.Generator`、`FrameBuild`、`CSharpWar3Frame` 或 `Projects/*` 的运行时代码与构建行为。
- 该澄清将直接影响未来所有变更如何创建和记录提案，从而降低小改动被重流程拖慢的概率。

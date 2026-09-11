## Why

当前仓库虽然已经引入 OpenSpec，但缺少一个在仓库根层面可见、可执行、可持续约束未来协作的治理入口。为避免后续改动绕过架构审查直接落地，需要把“先 OpenSpec 提案、用户审核、再实施”的流程正式固化为仓库规则，并明确提案必须覆盖跨项目全局影响。

另外，如果所有改动都被迫走同样重度的提案流程，小修小补会明显拖慢协作节奏。因此本次治理不仅要固化“必须先提案”，还要建立按风险分级的提案强度机制，让低风险改动更快、高风险改动仍然保持完整架构审查。

## What Changes

- 新增根级 `AGENTS.md`，作为仓库协作与变更治理入口文件。
- 在治理规则中明确：任何代码变更、架构调整、跨项目改动，必须先创建 OpenSpec proposal/design/tasks/specs，再提交用户审核，审核通过后方可实施。
- 在治理规则中新增 `fast / light / full / architecture` 四级提案制度，按改动风险与影响范围决定工件深度。
- 在治理规则中明确：提案必须从架构师视角进行全局设计，至少评估运行时框架、Source Generator、构建编排、CLI 与示例项目之间的影响边界。
- 在治理规则中明确：若 OpenSpec 已存在则复用现有目录与工具，不重复初始化；仅补齐缺失治理文件。
- 在治理规则中明确：仓库文件为事实源，OpenViking 为长期记忆辅助源，二者职责边界需清晰。
- 新增 `openspec/templates/` 与 `openspec/README.md`，把分级提案与审核清单文档化，降低后续协作成本。

## Capabilities

### New Capabilities
- `repository-governance`: 定义仓库级 AI 协作、OpenSpec 提案审核、实施前置条件与全局架构评审要求。

### Modified Capabilities

## Impact

- 受影响范围主要为仓库治理与文档层，不直接修改运行时代码、生成器逻辑、构建管线或 CLI 行为。
- 新增的治理要求将约束后续对 `War3Frame`、`War3Frame.Generator`、`FrameBuild`、`CSharpWar3Frame`、`Projects/*` 的所有变更流程。
- 需要新增根级入口与模板文档，使未来所有变更都能先按等级提案，再进入审核与实现阶段。

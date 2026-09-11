## Why

当前仓库已经明确所有变更都要遵循 `design -> review -> implement -> test -> summarize -> commit` 生命周期，但尚未规定 `summarize` 阶段的输出深度如何按变更等级分层。

在实际协作中，`fast` 与 `light` 级改动已经通过提案分级降低了设计成本，如果总结阶段仍默认要求与 `full` 或 `architecture` 同等详细，就会重新拉高轻量改动的尾部成本，削弱分级治理的效率收益。

因此，本次变更不是要削弱总结要求，而是要明确：所有等级都必须保留 `summarize` 阶段，但轻量级改动允许使用更短的总结格式，而高风险、高影响改动继续维持完整总结深度。

## What Changes

- 为 `summarize` 阶段增加按 `fast / light / full / architecture` 分级的输出深度规则。
- 明确 `fast` 与 `light` 可以使用短摘要，但不得跳过总结。
- 明确 `full` 与 `architecture` 仍应保持完整总结，覆盖影响、验证、风险与后续事项。
- 只修改治理规格与文档，不调整提案分级制度本身，也不改变审批门禁。

## Capabilities

### Modified Capabilities
- `repository-governance`: 为统一生命周期中的 `summarize` 阶段补充分级深度要求。

## Impact

- 影响范围仅限仓库治理规则与文档说明。
- 不修改 `War3Frame`、`War3Frame.Generator`、`FrameBuild`、`CSharpWar3Frame` 或 `Projects/*` 的运行时代码与构建行为。
- 该变更会影响未来所有已批准变更在收尾阶段的总结格式与详略程度。

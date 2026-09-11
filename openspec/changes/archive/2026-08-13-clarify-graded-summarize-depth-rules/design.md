## Context

仓库现有治理已经建立了统一生命周期与四级提案制度，也已经对提案深度和工件矩阵进行了分层治理。但 `summarize` 阶段仍只有统一存在要求，没有细化“不同等级的总结应当详细到什么程度”。

这带来一个实际问题：轻量改动虽然在设计前置阶段变快了，但如果收尾总结仍要求大量背景、全局影响和细节说明，执行者还是会在最后一个阶段付出与重型改动接近的文档成本。

因此，本次设计只补一条规则：总结阶段保留为必经环节，但摘要深度与变更等级匹配。

## Goals / Non-Goals

**Goals:**
- 保留 `summarize` 作为统一生命周期必经阶段。
- 允许 `fast` 与 `light` 使用更短总结，降低轻量改动收尾成本。
- 保持 `full` 与 `architecture` 的总结深度，确保高风险改动仍有充分审计记录。
- 将该规则同步到 spec、AGENTS、README 与模板/检查清单，避免入口文档与规范源再次漂移。

**Non-Goals:**
- 不改变 `design -> review -> implement -> test -> summarize -> commit` 的顺序。
- 不允许任何等级跳过 `summarize`。
- 不改变现有 `fast / light / full / architecture` 判级标准。
- 不修改运行时代码、构建逻辑、生成器逻辑或示例工程。

## Decisions

### 1. `summarize` 阶段继续强制保留，但允许深度分级
**Decision:** 所有变更在完成验证后都必须进入 `summarize`，但总结内容 SHALL 按 proposal level 分层，而不再一律按最重形式输出。

**Rationale:** 这样既保留流程闭环，又避免轻量改动在最后一步被重型文档成本拖慢。

### 2. `fast` 与 `light` 使用短摘要格式
**Decision:**
- `fast`: 总结只要求简述实际改动、验证结果、是否存在遗留风险或后续事项。
- `light`: 总结控制在一个短段落或少量要点内，覆盖改动范围、验证结果、后续是否需要进一步提案。

**Rationale:** 这两级本身就是低风险或局部性改动，其总结目标是快速留痕与风险交代，而不是完整架构复盘。

### 3. `full` 与 `architecture` 继续维持完整总结
**Decision:**
- `full`: 需要说明实际改动范围、验证覆盖、风险与后续建议。
- `architecture`: 在 `full` 基础上，还应说明阶段性结果、迁移状态、剩余风险与未完成事项。

**Rationale:** 高影响改动需要更强审计能力，不能因为流程分级而降低总结质量。

### 4. 入口文档与模板只镜像规范，不自行发明总结标准
**Decision:** 总结深度的最终真值表写入 capability spec；`AGENTS.md`、`README.md`、模板与 checklist 只做镜像与执行提示。

**Rationale:** 这样可以延续现有“spec 为规范源”的治理一致性机制。

## Summary Depth Matrix

| Level | Summary Requirement | Expected Length |
|---|---|---|
| `fast` | 实际改动、验证结果、遗留风险/后续事项 | 2-4 行或等价短摘要 |
| `light` | 改动范围、验证结果、是否需要后续提案 | 1 个短段落或 3 个以内要点 |
| `full` | 改动范围、全局影响、验证覆盖、风险与后续建议 | 完整总结 |
| `architecture` | `full` 内容 + 阶段结果、迁移状态、剩余风险、未完成事项 | 架构级完整总结 |

## Risks / Trade-offs

- [风险] 轻量总结被误解为“可以草率带过” → [缓解] 明确短摘要仍必须覆盖改动、验证、风险三项核心信息。
- [风险] `light` 与 `full` 的边界再次模糊 → [缓解] 继续沿用既有 proposal level 规则，summary 深度不反向影响判级。
- [风险] 不同入口文档再次出现表述不一致 → [缓解] 本次任务要求同步 spec、AGENTS、README、模板与检查清单。

## Migration Plan

1. 在 `repository-governance/spec.md` 中补充 summary depth rule 与对应 scenarios。
2. 在 `AGENTS.md` 的 `Summarize` 段落写明轻量总结与完整总结的差异。
3. 在 `openspec/README.md`、模板与 checklist 中同步分级说明。
4. 后续所有新变更按新规则执行；旧变更不追溯重写。

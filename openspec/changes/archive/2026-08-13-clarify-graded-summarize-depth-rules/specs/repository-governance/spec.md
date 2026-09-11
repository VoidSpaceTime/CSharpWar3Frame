## MODIFIED Requirements

### Requirement: Unified lifecycle after proposal approval
无论采用哪一个提案等级，后续执行顺序都 MUST 统一遵循 `design -> review -> implement -> test -> summarize -> commit` 生命周期；等级差异可以体现在设计、审查与总结工件的深度，但不得跳过审核、验证或总结步骤。

`summarize` 阶段的输出深度 SHALL 按 proposal level 分级如下：

- `fast`: MUST 用短摘要记录实际改动、验证结果，以及是否存在遗留风险或后续事项。
- `light`: MUST 用一个短段落或少量要点记录改动范围、验证结果，以及是否需要后续提案或补充工作。
- `full`: MUST 提供完整总结，覆盖实际改动范围、全局影响、验证覆盖、风险与后续建议。
- `architecture`: MUST 在 `full` 基础上额外说明阶段结果、迁移状态、剩余风险与未完成事项。

#### Scenario: Fast change is completed
- **WHEN** 一个 `fast` 级变更完成实现与验证
- **THEN** 系统 MAY 使用 2-4 行等价短摘要完成 `summarize`，但 MUST 仍然覆盖实际改动、验证结果与遗留风险/后续事项，而 SHALL NOT 直接省略总结阶段

#### Scenario: Light change is completed
- **WHEN** 一个 `light` 级变更完成实现与验证
- **THEN** 系统 MAY 使用一个短段落或少量要点完成 `summarize`，但 MUST 说明改动范围、验证结果，以及是否需要后续提案

#### Scenario: Full or architecture change is completed
- **WHEN** 一个 `full` 或 `architecture` 级变更完成实现与验证
- **THEN** 系统 MUST 保持完整总结深度，而 SHALL NOT 因为流程分级而把高影响改动降格为轻量摘要

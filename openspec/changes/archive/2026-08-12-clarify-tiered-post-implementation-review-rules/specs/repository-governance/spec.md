## MODIFIED Requirements

### Requirement: Unified lifecycle after proposal approval
无论采用哪一个提案等级，变更都 MUST 遵循 `design -> review -> implement -> test -> summarize -> commit` 生命周期。`review` SHALL 表示实施前的用户审核批准门禁；实施后的直接验证与专业复盘 SHALL 属于 `test` 阶段。提案等级可以影响工件、总结与默认复盘深度，但不得跳过用户批准、实际验证或总结。

#### Scenario: Pre-implementation review is not replaced by post-implementation review
- **WHEN** 一个变更计划使用 Oracle、QA 或完整 `review-work` 做实施后复盘
- **THEN** 系统仍 MUST 在实施前取得用户明确批准，而 SHALL NOT 用实施后代理审查替代 OpenSpec `review` 门禁

#### Scenario: Scope growth returns to review
- **WHEN** 实施准备或实施中发现安全敏感、重大实现、公共契约或跨项目范围超出已批准提案
- **THEN** 系统 MUST 修订或升级 OpenSpec、补齐对应工件并重新取得用户批准，而 SHALL NOT 仅通过增加复盘路数继续实施

#### Scenario: Failed validation blocks successful completion
- **WHEN** 测试、构建、静态检查或专业复盘出现失败
- **THEN** 系统 MUST 修复根因并重新验证，或将变更标记为阻塞/未完成，而 MUST NOT 进入成功总结或宣称验收通过

## ADDED Requirements

### Requirement: Proposal level, review intensity, and tool activation are independent
仓库 MUST 将 OpenSpec 提案等级、实施后复盘强度和审查工具启用视为三个独立层次。提案等级 SHALL 决定治理工件与默认复盘强度；实际风险 SHALL 可以提高最终复盘强度；具体工具 SHALL 受独立授权和可用性约束。

复盘强度 SHALL 使用以下有序等级：

- `R0 Direct`：只执行直接测试、构建、静态检查或文档验证。
- `R1 Focused`：在 `R0` 基础上，完成 1 个技术准确性视角的复核并给出证据与 verdict。
- `R2 Targeted`：在 `R0` 基础上，完成 2-3 个与实际风险匹配的专项复核，并分别给出证据与 verdict。
- `R3 Comprehensive`：在 `R0` 基础上，覆盖目标/约束、技术质量、安全、QA 与上下文五类视角，并分别给出证据与 verdict。

一个专项复核 MUST 表示一个独立风险视角，而不是一个代理或一次工具调用。一个执行者或工具 MAY 覆盖多个视角，但每个视角 MUST 独立给出证据与 verdict；多个工具重复同一视角只计为一个专项复核。

未命中独立升级条件时，默认强度 SHALL 如下：

- `fast`：MUST 使用 `R0 Direct`。
- `light`：MUST 默认使用 `R0 Direct`；当变更仍满足 `light` 边界，但包含多步骤技术推理、版本敏感事实或技术事实不确定性时，MUST 使用 `R1 Focused`。
- `full`：MUST 使用 `R2 Targeted`，并按实际风险选择 2-3 路专项复核。
- `architecture`：MUST 使用 `R3 Comprehensive`，覆盖全部五类视角。

最终复盘强度 MUST 取提案等级默认值、风险升级要求与用户明确强度要求中的最高等级。复盘强度提高 MUST NOT 降低或替代直接验证义务。

#### Scenario: Fast documentation correction completes with direct checks
- **WHEN** 一个 `fast` 文档修正未命中任何升级条件
- **THEN** 系统 MUST 使用 `R0` 完成读取、格式或路径等直接验证，并 MUST NOT 因外部审查工具可用而自动调用审查代理

#### Scenario: Light version-sensitive work receives one accuracy review
- **WHEN** 一个仍满足 `light` 边界的变更包含复杂技术推理、版本敏感事实或技术事实不确定性
- **THEN** 系统 MUST 使用 `R1`，在直接验证之外完成 1 路技术准确性复核，而 SHALL NOT 自动扩张为 `R2` 或 `R3`

#### Scenario: Full change selects two or three risk reviews
- **WHEN** 一个 `full` 变更不属于安全敏感或重大实现
- **THEN** 系统 MUST 使用 `R2`，从实际风险中选择 2-3 路专项复核，而 MUST NOT 仅因公共契约、生成器或构建链标签自动执行五路复盘

#### Scenario: Architecture change receives five-perspective review
- **WHEN** 一个变更的提案等级为 `architecture`
- **THEN** 系统 MUST 使用 `R3` 覆盖目标/约束、技术质量、安全、QA 与上下文五类视角

### Requirement: Security-sensitive and major implementations require comprehensive review
安全敏感事项和重大实现 MUST 使用 `R3 Comprehensive`，无论其原默认复盘强度为何。

安全敏感 SHALL 指权限、认证授权、敏感数据、不可信外部输入、供应链，或具有可利用后果的 native/进程边界风险。系统 MUST NOT 仅因存在普通原生调用就自动判定为安全敏感。

重大实现 SHALL 指改变核心行为或架构边界、具有较大影响半径、涉及跨项目契约或迁移、难以快速回滚，或失败会显著影响运行与交付的实现。系统 MUST 根据行为和风险判断，而 MUST NOT 仅按目录名、文件数量或代码行数判断。

公共 API、生成器输出、配置/构建/发布契约、持久化/迁移、性能/资源/实时性以及多系统/多项目协作等风险 MUST 至少使用 `R2 Targeted`。这些风险只有在实际符合安全敏感或重大实现定义时，才 MUST 升为 `R3`。

#### Scenario: Local generator correction is not automatically major
- **WHEN** 一个 Source Generator 变更影响公共输出契约，但范围局部、迁移可控且可快速回滚
- **THEN** 系统 MUST 至少使用 `R2`，但 MUST NOT 仅因文件位于 `War3Frame.Generator` 就自动判定为重大实现或升级 `R3`

#### Scenario: Security boundary requires five perspectives
- **WHEN** 一个变更涉及权限、不可信外部输入、敏感数据或可利用的 native/进程边界
- **THEN** 系统 MUST 使用 `R3` 并覆盖全部五类视角；若该事实超出已批准范围，还 MUST 先返回 OpenSpec review 门禁

#### Scenario: Major implementation requires five perspectives
- **WHEN** 一个变更改变核心流程或架构边界，并具有较大影响半径、复杂迁移或困难回滚
- **THEN** 系统 MUST 使用 `R3` 并覆盖全部五类视角

### Requirement: Oracle is preferred for focused technical-accuracy review
当 `light` 变更需要 `R1 Focused` 时，系统 SHOULD 在 Oracle 可用时使用 Oracle 完成技术准确性复核。Oracle 不可用时，系统 MAY 使用等价复核，但 MUST 记录不可用或替代原因、证据与 verdict。

Oracle 的存在 MUST NOT 阻止直接验证，也 MUST NOT 让原本需要升级为 `full` 或 `architecture` 的范围继续停留在 `light`。

#### Scenario: Oracle is unavailable for version-sensitive light work
- **WHEN** 一个版本敏感的 `light` 变更需要 `R1`，但当前环境没有 Oracle
- **THEN** 系统 MAY 使用等价技术准确性复核，并 MUST 在总结中记录替代原因、证据和 verdict

### Requirement: Comprehensive review does not automatically authorize review-work
`R3 Comprehensive` SHALL 定义必须覆盖的五类风险视角，而 SHALL NOT 自动授权完整 `review-work`。系统 MUST NOT 仅因提案为 `architecture`、事项安全敏感、事项属于重大实现或其他规则要求 `R3`，就自动启用完整 `review-work`。

只有用户明确要求“全面复盘”“完整 QA”或直接指定 `review-work` 时，系统 MAY 启用完整 `review-work`。用户直接指定 `review-work` 时，系统 MUST 在工具可用且不与更高优先级指令冲突的前提下按该工作流执行；工具不可用时 MUST 明确报告，而 MUST NOT 静默替换。

未获得完整 `review-work` 授权但必须执行 `R3` 时，系统 MUST 使用当前获准且可用的检查方式覆盖五类视角。更高优先级 system / developer 指令要求特定工具时，系统 MUST 遵守，并在总结中记录额外要求及结果。

#### Scenario: Architecture review does not auto-run review-work
- **WHEN** 一个 `architecture` 变更需要 `R3`，但用户未要求全面复盘、完整 QA 或 `review-work`
- **THEN** 系统 MUST 覆盖五类视角，但 MUST NOT 自动启用完整 `review-work`

#### Scenario: User explicitly requests comprehensive QA
- **WHEN** 用户明确要求全面复盘、完整 QA 或直接指定 `review-work`
- **THEN** 系统 MUST 使用 `R3`，并 MAY 启用完整 `review-work`；若用户直接指定该工具，则 MUST 按可用性规则执行或明确报告不可用

#### Scenario: Higher-priority instruction mandates a review tool
- **WHEN** system / developer 指令明确要求对当前任务调用特定审查工具
- **THEN** 系统 MUST 遵守该高优先级指令，并 SHALL 在总结中说明实际工具和审查强度为何高于仓库默认值

### Requirement: Failed validation blocks completion without mechanically forcing five reviews
任何测试、构建、静态检查或专业复核失败都 MUST 阻止成功总结。系统 MUST 修复根因并重新执行受影响验证，或明确标记为阻塞/未完成。

失败本身 MUST NOT 机械触发 `R3`。系统 MUST 重新判定失败是否揭示安全敏感、重大实现或超出批准范围的新事实；命中时按对应规则升级强度或返回 OpenSpec review 门禁。

#### Scenario: One-time environment failure is repaired
- **WHEN** 一次构建失败被证明为环境或文件锁问题，根因已修复且串行重验通过
- **THEN** 系统 MUST 记录证据，但 MUST NOT 仅因该一次性失败自动执行五路复盘

#### Scenario: Failure reveals unapproved major scope
- **WHEN** 失败揭示核心流程设计不匹配、重大迁移需求或其他未批准范围
- **THEN** 系统 MUST 停止实施、修订 OpenSpec 并重新取得用户批准，而 MUST NOT 仅增加代理数量后继续

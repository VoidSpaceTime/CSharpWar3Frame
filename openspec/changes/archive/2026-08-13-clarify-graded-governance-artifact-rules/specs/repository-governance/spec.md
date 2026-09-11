## MODIFIED Requirements

### Requirement: Graded OpenSpec proposal levels
仓库中的每一次变更都 MUST 先经过 OpenSpec 提案与用户审核，但提案深度 SHALL 根据改动风险、影响范围、可逆性与跨项目程度分级为 `fast`、`light`、`full`、`architecture` 四级。所有等级都 MUST 在对应 change 的 `openspec/changes/<change-id>/proposal.md` 中留下正式记录；等级差异体现在记录深度与额外工件要求，而不是是否留档。

四级工件矩阵 SHALL 如下：

- `fast`: MUST 提供 `proposal.md`；`design.md`、`tasks.md` 与相关 `spec.md` 为可选，仅在额外风险或边界约束需要时补充。
- `light`: MUST 提供 `proposal.md`；`design.md`、`tasks.md` 与相关 `spec.md` 为条件性可选，当方案存在多步骤、边界条件、局部规格约束或验收复杂度提高时应补充。
- `full`: MUST 提供 `proposal.md`、`design.md`、`tasks.md` 与相关 `spec.md`。
- `architecture`: MUST 提供 `full` 级完整工件，并在 `proposal.md` 与 `design.md` 中额外覆盖方案比较、迁移路径、回滚策略与阶段拆分。

#### Scenario: Fast change is prepared for review
- **WHEN** 一个低风险局部变更被判定为 `fast`
- **THEN** 系统 MUST 在该 change 的 `proposal.md` 中记录目标、影响文件、低风险理由与验证方式，而 SHALL NOT 仅把这些内容留在聊天记录、`AGENTS.md` 或 README 中

#### Scenario: Light change requires extra structure
- **WHEN** 一个 `light` 级变更虽然不需要完整四件套，但已经出现多步骤实施、局部规格约束或审查者无法仅凭 `proposal.md` 评估风险
- **THEN** 系统 MUST 补充所需的 `tasks.md`、`design.md` 或相关 `spec.md`，并在用户审核前达到足够可审查的完整度

#### Scenario: Full change is proposed
- **WHEN** 一个变更被判定为 `full` 或 `architecture`
- **THEN** 系统 MUST 补齐完整工件集合，而 SHALL NOT 仅依赖轻量 `proposal.md` 进入审核或实施阶段

### Requirement: Repository governance documents remain consistent
仓库中的治理入口文档、说明文档与模板文档 MUST 与 `repository-governance` capability 的规范要求保持一致；当文档之间出现措辞冲突时，系统 SHALL 按规范优先级解释并修正较低优先级文档。

规范优先级 SHALL 如下：

1. `openspec/changes/.../specs/.../spec.md` 中的 requirement。
2. 仓库根级 `AGENTS.md`。
3. `openspec/README.md`。
4. `openspec/templates/*`。

#### Scenario: README implies broader artifact requirements than the spec
- **WHEN** README 或模板的描述被理解为所有等级都需要 `proposal.md`、`design.md`、`tasks.md` 与 `spec.md`
- **THEN** 系统 MUST 以 capability spec 中定义的工件矩阵为准，并同步修正文档以消除误导

#### Scenario: Template omits the standard record location
- **WHEN** 某个模板或说明文档没有明确 `fast/light` 的标准留痕位置
- **THEN** 系统 MUST 以 capability spec 中“所有等级都记录到对应 change 的 `proposal.md`”规则为准，并更新低优先级文档保持一致

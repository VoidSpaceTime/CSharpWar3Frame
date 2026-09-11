# repository-governance Specification

## Purpose
定义本仓库的变更治理契约：以 `openspec/specs/` 为唯一真相层，change 只表达 delta，用官方 `openspec archive` 完成变更；同时明确本仓库附加治理层（工件分级、复盘强度、`summary.md`）与官方机制的关系——附加层补充质量证据，但不阻塞官方归档门禁。

## Requirements

### Requirement: 单一真相层
仓库 SHALL 以 `openspec/specs/<capability>/spec.md` 作为当前系统行为的唯一真相入口。

#### Scenario: 查询当前行为
- **WHEN** 需要确认某 capability 的当前契约
- **THEN** 读取 `openspec/specs/<capability>/spec.md`，而非 `archive/` 或 `AGENTS.md`

### Requirement: 变更以 delta 表达
每个 change 的 `openspec/changes/<id>/specs/<capability>/spec.md` SHALL 只包含 delta 段：`ADDED` / `MODIFIED` / `REMOVED` / `RENAMED`，且不得跨段冲突。

#### Scenario: 提交变更规格
- **WHEN** 一个 change 影响行为契约
- **THEN** 其 `specs/` 下以 delta 形式记录新增/修改/删除的需求

#### Scenario: 不影响契约的变更
- **WHEN** 变更不产生 spec delta
- **THEN** 该 change 在元数据中声明 `skip_specs: true`，或归档时使用 `--skip-specs`

### Requirement: 归档即完成动作
仓库 SHALL 使用官方 `openspec archive` 完成 change：校验 change 与 delta → 合并 delta 进 `openspec/specs/` → 移动目录到 `archive/YYYY-MM-DD-<id>/`；失败时回滚且不改动主 specs。

#### Scenario: 正常归档
- **WHEN** 执行 `openspec archive <change> --yes`
- **THEN** delta 按 `RENAMED → REMOVED → MODIFIED → ADDED` 顺序合并，目录移动到带日期前缀的 archive

#### Scenario: 合并校验失败
- **WHEN** 重建后的 spec 未通过校验
- **THEN** 不写入任何文件，change 留在原位，命令返回非零

### Requirement: CLI 可用
仓库 SHALL 保持官方 OpenSpec CLI（`@fission-ai/openspec`）可安装、可运行，用于 `init / new / validate / list / archive` 等操作。

#### Scenario: 校验环境
- **WHEN** 在新环境上手
- **THEN** `openspec --version` 与 `openspec validate` 可正常执行

### Requirement: 仓库附加治理层不阻塞官方动作
本仓库 MAY 在官方机制之上保留附加治理证据（`summary.md`、`fast/light/full/architecture` 分级、`R0~R3` 复盘强度）；附加层 SHALL NOT 阻塞官方 `openspec archive` 的门禁。

#### Scenario: 附加证据缺失
- **WHEN** 官方归档门禁（tasks 勾选、delta 合法性）满足但附加 `summary.md` 缺失
- **THEN** 官方 `archive` 仍可执行；附加 `summary.md` 由仓库流程在归档前补齐，不改变官方归档语义

### Requirement: 工件分级（附加）
本仓库 SHALL 以 `fast | light | full | architecture` 作为建议强度，决定所需工件与默认复盘强度；该分级为附加约定，不改变官方归档门禁。

#### Scenario: 确定等级
- **WHEN** 提案一个变更
- **THEN** 在 `proposal.md` 标注等级，并按等级补齐工件：`fast` 仅 `proposal.md`；`light` 视复杂度补 `design/tasks/spec`；`full` 补齐四类工件；`architecture` 在 `full` 上补方案比较、迁移、回滚、阶段拆分

#### Scenario: 范围扩大
- **WHEN** 实施中发现范围超出已批准等级
- **THEN** 自动升级等级并重新提交审核，不得边扩大边实施

### Requirement: 实施总结（附加）
进入 `已实施` 的变更 SHALL 在 change 目录保留 `summary.md`，覆盖改动范围、验证结果、遗留风险；`已取消` / `已取代` 可免除，但须在 `proposal.md` 记录关闭原因。

#### Scenario: 完成实施
- **WHEN** 变更范围完成且验证通过
- **THEN** 生成 `summary.md`，其深度按等级分层（`fast` 2-4 行；`light` 短段落；`full`/`architecture` 完整）

### Requirement: 复盘强度（附加）
本仓库 SHALL 使用有序复盘强度 `R0 Direct / R1 Focused / R2 Targeted / R3 Comprehensive`；默认映射为 `fast→R0`、`light→R0`（复杂或版本敏感→`R1`）、`full→R2`、`architecture→R3`；每个视角须有独立证据与 verdict。

#### Scenario: 判定强度
- **WHEN** 变更涉及公共契约、生成器输出、构建/发布契约、持久化迁移、性能资源或多系统协作
- **THEN** 至少使用 `R2`

#### Scenario: 高风险强度
- **WHEN** 变更属 `architecture`、安全敏感或重大实现
- **THEN** 使用 `R3`，覆盖目标/约束、技术质量、安全、QA、上下文五类视角

#### Scenario: 工具授权
- **WHEN** 需要 `R3` 但用户未明确要求完整复盘
- **THEN** 不得自动启用完整 `review-work`，改用当前获准且可用的方式覆盖五类视角

#### Scenario: 验证失败
- **WHEN** 任一测试、构建、静态检查或专业复核失败
- **THEN** 阻止成功总结，修复并重新验证或标记阻塞/未完成

### Requirement: 交付门禁（附加）
仓库 SHALL 在实现前与成功总结前设置门禁：实现前须用户明确批准、提案等级与工件完整度匹配；成功总结前须完成对应验证与独立风险视角，且无阻断项。

#### Scenario: 进入实现前
- **WHEN** 准备开始实现
- **THEN** 仅当用户已批准、工件完整度与等级匹配、风险与验证路径被接受、无升级信息时才进入实现

#### Scenario: 进入成功总结前
- **WHEN** 准备把变更标记为完成
- **THEN** 仅当直接验证与独立风险视角已完成、阻断问题已修复重验、`summary.md` 已生成且未以 `R0~R3` 证据替代它时，才可总结为完成

#### Scenario: 提案留痕
- **WHEN** 形成提案
- **THEN** 正式提案记录在对应 change 的 `proposal.md`，而非仅留在聊天、`AGENTS.md` 或 README

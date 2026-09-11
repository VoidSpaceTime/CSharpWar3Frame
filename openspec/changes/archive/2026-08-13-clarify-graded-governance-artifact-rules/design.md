## Context

仓库上一轮治理已经通过 `establish-openspec-governance` 建立了四级提案制度与统一生命周期，但 Oracle 复核后发现两个执行空档：

1. `fast/light` 提案虽被允许轻量化，却没有被明确绑定到标准文件路径与标准字段，导致“可快速”与“可审计”之间缺少统一落点。
2. `spec.md`、`README.md` 与模板文件对工件要求的表达粒度不同，执行者可能回退到“为了安全，所有等级都走四件套”，从而抵消分级治理的价值。

因此，本次设计不重建制度本身，而是给制度补齐“记录位置 + 真值表 + 解释优先级”三块基础设施。

## Goals / Non-Goals

**Goals:**
- 为 `fast` 与 `light` 提案指定统一、可审计、低摩擦的记录载体。
- 用单一工件矩阵定义四级提案的必需工件，避免多文档各说各话。
- 明确治理文档发生措辞冲突时的解释顺序，防止未来再出现执行偏差。

**Non-Goals:**
- 不改变 `fast / light / full / architecture` 四级制度本身。
- 不新增新的流程阶段，仍然沿用 `design -> review -> implement -> test -> summarize -> commit`。
- 不在本次变更中修改任何运行时代码、生成器逻辑、构建逻辑或示例工程。

## Decisions

### 1. `fast` 与 `light` 统一记录到对应 change 的 `proposal.md`
**Decision:** 无论是 `fast`、`light`、`full` 还是 `architecture`，都必须在 `openspec/changes/<change-id>/proposal.md` 中留下正式记录。区别不在于是否存在 `proposal.md`，而在于其内容深度。

**Rationale:** 这样可以用一个稳定入口同时满足轻量执行与审计追踪，避免把轻量提案散落到 `AGENTS.md`、README、聊天记录或临时说明中。

**Alternatives considered:**
- 允许 `fast/light` 只记录在聊天或 `AGENTS.md`：速度快，但不可审计，也无法随 change 一起归档。
- 允许 `fast/light` 使用独立文件名：会增加路径复杂度，降低执行一致性。

### 2. 四级工件矩阵以 `spec` 为规范真值表
**Decision:** 将四级工件要求写入 `repository-governance/spec.md`，并规定该矩阵为规范真值表；`AGENTS.md`、`openspec/README.md` 与模板文档必须与其保持镜像一致，但当文字冲突时，以 `spec` 为准。

**Rationale:** `spec` 才是仓库级治理能力的规范源，其他文档更适合作为入口、说明和填写辅助。如果不明确优先级，未来文档演进中仍会出现相互漂移。

### 3. 工件要求按“最小必需”而非“全部四件套”执行
**Decision:**
- `fast`: 必须有 `proposal.md`，其余工件按需要补充，不默认强制。
- `light`: 必须有 `proposal.md`；当方案需要拆分步骤或存在边界风险时，应补 `tasks.md` 或 `design.md`，但不默认要求完整四件套。
- `full`: 必须有 `proposal.md`、`design.md`、`tasks.md` 与相关 `spec.md`。
- `architecture`: 在 `full` 基础上，`proposal.md` 与 `design.md` 必须额外覆盖方案比较、迁移、回滚与阶段拆分。

**Rationale:** 这样既保留统一入口，又避免对低风险改动强制套用高成本工件。

### 4. 范围扩大时升级等级并补齐缺失工件
**Decision:** 只要在分析、审查或实现准备阶段发现 `fast/light` 已无法覆盖风险，就必须升级等级，并补齐升级后所要求的工件集合，再重新提交审核。

**Rationale:** 分级治理的前提不是“一开始永远判对”，而是允许先走轻量入口，但在事实变化时强制升级。

## Artifact Matrix

| Level | Required Artifacts | Optional Artifacts | Notes |
|---|---|---|---|
| `fast` | `proposal.md` | `tasks.md`, `design.md` | `proposal.md` 内按 fast 模板记录目标、文件、低风险理由、验证 |
| `light` | `proposal.md` | `tasks.md`, `design.md`, related `spec.md` | 当存在多步骤、边界条件或局部规格约束时补充对应工件 |
| `full` | `proposal.md`, `design.md`, `tasks.md`, related `spec.md` | - | 必须完整评估全局影响与验证计划 |
| `architecture` | `proposal.md`, `design.md`, `tasks.md`, related `spec.md` | phase appendices | 必须在 `proposal/design` 中覆盖方案比较、迁移、回滚、阶段拆分 |

## Conflict Resolution

当治理文档之间出现冲突时，解释顺序如下：

1. `openspec/changes/.../specs/.../spec.md` 中的 requirement 为规范源。
2. `AGENTS.md` 为仓库协作入口，应与规范源保持一致。
3. `openspec/README.md` 为使用说明，不得扩张或缩减规范要求。
4. `openspec/templates/*` 为填写辅助，不得与规范源冲突。

## Risks / Trade-offs

- [风险] 所有等级都要求 `proposal.md` 可能仍被误解为“还是太重” → [缓解] 明确轻量等级只要求最小内容，不默认强制四件套。
- [风险] `light` 级是否需要 `tasks/design/spec` 仍有判断空间 → [缓解] 在模板与 checklist 中增加“何时补充可选工件”的触发条件说明。
- [风险] 旧文案仍在其他位置遗留冲突 → [缓解] 本次任务要求同步更新 spec、README、模板与审核清单。

## Migration Plan

1. 在治理 spec 中写入工件矩阵、记录位置与冲突优先级。
2. 更新 `AGENTS.md`、`openspec/README.md`、模板与 checklist，保证入口层与规范层一致。
3. 之后所有新 change 按新矩阵落档；旧 change 不追溯重写，仅在必要时参考新规则解释。

## Open Questions

- 是否需要在后续再加一份 `change-id` 命名与目录初始化模板；本次先不扩展，避免治理工具继续变重。

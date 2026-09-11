# 设计：对齐官方 OpenSpec 工作流

> 配套 `proposal.md`。本文件覆盖 `architecture` 级要求的方案比较、迁移、回滚、阶段拆分与长期维护。

## 1. 架构目标

把"变更治理"从**本仓库手工重写**回到**官方 OpenSpec 机制**：

```
探索/提案 ──► 实施 ──► 验证 ──► 归档(合并 delta)
                                  │
                    openspec/specs/<capability>/spec.md  = 唯一当前真相
```

- 真相层：`openspec/specs/`。
- 增量层：`openspec/changes/<id>/specs/...`（`ADDED/MODIFIED/REMOVED/RENAMED`）。
- 历史层：`openspec/changes/archive/YYYY-MM-DD-<id>/`。
- 完成动作：`openspec archive`（校验 → 合并 delta → 移动目录；失败回滚）。

## 2. 现状与目标的差距

| 维度 | 现状 | 目标（官方） |
|---|---|---|
| `openspec/specs/` | 空 | 唯一当前真相，按 capability 分域 |
| 归档动作 | 手工搬目录 | `openspec archive`（校验+合并+移动） |
| delta spec | 冻结在 archive | 归档时合并进 specs |
| CLI | 孤儿 `openspec.ps1` | `@fission-ai/openspec@1.13.0` 可用 |
| 状态 | 自定义 `状态` 字段 | 官方无；本仓库可选保留为附加层 |
| 完成证据 | `summary.md` 硬要求 | 官方无；本仓库可选保留为附加层 |
| 分级/复盘 | 自定义 4 级 + `R0~R3` | 官方无；本仓库可选保留为附加层 |

## 3. 候选方案比较

### 方案 A：完全照搬官方（纯官方）
- 删除 `状态` 字段、`summary.md` 要求、分级、`R0~R3`、自定义模板。
- 优点：心智模型最简，与官方文档 100% 一致。
- 缺点：丢失用户历史批准的质量治理资产；删除后难以快速恢复；`R3` 等复盘纪律消失。

### 方案 B：官方机制 + 薄附加层（推荐）
- 官方机制负责：真相层、delta 合并、归档、CLI。
- 本仓库附加层继续保留：`summary.md` 作为附加证据、`R0~R3` 复盘强度、分级（作为"建议强度"映射）。
- 附加层**不得阻塞**官方 `openspec archive`：即 `archive` 只依据官方门禁（tasks 勾选、delta 合法性），`summary.md` 与复盘由本仓库流程在归档前完成。
- 优点：既拿到官方单一真相，又不丢治理资产；迁移可分阶段，回滚容易。
- 缺点：文档需同时说明两层，存在轻微认知成本。

### 方案 C：仅修 CLI，不动结构
- 只重装 CLI 并手工适配目录，不改真相层。
- 缺点：没有解决"`specs/` 空置 + delta 不合并"的根本问题，等于没对齐。否。

**推荐：方案 B。**

## 4. 阶段拆分

### Phase 1 — 机制生效（本次实施）
1. 备份 `openspec/`，安装 `@fission-ai/openspec@latest` 并验证版本。
2. 在受控模式下执行 `openspec init`（先看它将创建/修改什么，逐项审查 diff 后接受）；确认 `openspec/` 元数据与 `specs/` 生效。
3. 落地 `openspec/specs/repository-governance/spec.md`（本 change 的 delta 经官方 `archive` 合并后生成）。
4. 用官方流程归档 `arcane-missile-stun-example` 与 `slim-item-test-templates`（二者无 delta → `--skip-specs`；非交互环境加 `--yes`）。
5. 以本 change 自身跑一遍 `openspec archive`，验证 delta 合并 → specs 落地的完整闭环。

### Phase 2 — 精选回填（独立评估，不在本次批准范围）
- 从 `AGENTS.md` + 最新 delta 提炼核心 capability spec：`native-sync`、`ecs-message-naming`、`unit-lifecycle`、`effect-settlement`、`ability`、`item`。
- 每个 capability 必须先核对当前代码，确认"当前真相"，再合并 delta；避免把历史中间态写成真相。

### Phase 3 — 治理文档收口
- 改写 `AGENTS.md` OpenSpec 章节：固定流程改为官方 `apply → verify → sync → archive`，并说明附加层。
- 改写 `openspec/README.md` 与 `archive/README.md`，删除与官方冲突的"归档不等于已实施"等表述（改为"归档 = 官方完成动作；本仓库附加 `summary.md` 证据"）。
- 处置 `openspec/templates/`：内容迁移进 repository-governance spec 后删除，或保留并标注"附加约定"。

## 5. 迁移与回滚策略

- **迁移前**：`git status` 干净；对 `openspec/` 做一次快照（git 提交或临时副本）。
- **迁移中**：每个 Phase 单独可审阅；`init` 产生的意外改动（如 `.opencode/`、`AGENTS.md` 追加）必须逐项确认，不盲目接受。
- **回滚**：
  - CLI：`npm uninstall -g @fission-ai/openspec`。
  - 目录/文档：`git restore openspec/ AGENTS.md`（或回滚对应提交）。
  - `openspec/specs/` 允许为空，回滚不产生数据损失。
- **不可逆点**：无。全部改动受 git 管控。

## 6. 与官方 archive 的行为对齐要点

- 校验：`proposal.md` 结构 + delta spec 语法（`ADDED/MODIFIED/REMOVED/RENAMED`，禁止跨段冲突）。
- 合并顺序：`RENAMED → REMOVED → MODIFIED → ADDED`。
- 任务检查：扫 `tasks.md` 的 `- [x]/[ ]`；有未完成在非交互（`--json`/CI）会直接 `ArchiveBlockedError`。
- 无 delta 的 change：用 `--skip-specs`，或在 change 的 `.openspec.yaml` 写 `skip_specs: true`。
- 命名：`archive/YYYY-MM-DD-<change>/`；已是日期前缀的名字不再叠加。
- 无终端必须 `--yes`，否则 exit 1、不改动任何文件。

## 7. 长期维护影响

- **收益**：单一真相入口；归档由 CLI 保证合并与回滚；新 AI/协作者只需一套心智模型。
- **成本**：每个改动若影响行为契约，必须写 delta spec；`AGENTS.md` 从"规则真相"降级为"人类入口/摘要"，规则内容逐步迁入 specs。
- **纪律**：完成 = `openspec archive` 成功；`summary.md` 与复盘为附加质量证据，不替代 archive。

## 8. 验证计划

- `openspec --version`；`openspec list` / `openspec view` 能识别 active 与 archive。
- 对本 change 执行 `openspec archive`（先 `openspec validate`），确认 `openspec/specs/repository-governance/spec.md` 被正确创建/合并、目录移动到 `archive/2026-09-12-adopt-official-openspec-workflow/`。
- 归档后 `openspec/specs/` 非空且内容与 delta 一致。
- `git status` 审阅全部改动。
- 依据 `architecture` → `R3`：覆盖目标/约束、技术质量、安全、QA、上下文五类视角，各自独立证据与 verdict。

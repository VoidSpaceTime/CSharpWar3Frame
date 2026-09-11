# 提案：对齐官方 OpenSpec 工作流

- **Change ID**: `adopt-official-openspec-workflow`
- **提案等级**: `architecture`
- **状态**: `已批准`（用户 2026-09-12 确认"按你建议来"，采纳 D1=保留附加层 / D2=仅向前 / D3=迁移后删除）→ 实施中
- **目标一句话**: 把本仓库手工演化的 OpenSpec 治理回收到官方 OpenSpec（Fission-AI）的标准机制上，让 `openspec/specs/` 成为唯一"当前真相"，`archive` 成为完成动作。
- **提案日期**: 2026-09-12
- **请求来源**: 用户 2026-09-12 指示"按官网的来吧"
- **默认实施后审查强度**: `R3 Comprehensive`（提案等级为 `architecture`）
- **命中的审查升级触发器**: 架构边界 / 目录结构重组；影响半径覆盖整个 `openspec/` 与治理入口
- **最终实施后审查强度**: `R3 Comprehensive`
- **Oracle 可用性与 `R1` 回退方式**: 不适用（`R3` 不受 `R1` 回退约束）
- **完整 `review-work` 授权来源**: 无（不得自动启用完整 `review-work`；按 `R3` 五类视角用当前获准方式覆盖）

---

## 1. 背景 / Why

本仓库当前把官方 OpenSpec 的语义**手工重写**了一遍，并且偏离了官方最核心的一步：

1. **真相层空置**：`openspec/specs/` 是空目录。官方定义它是"current reality"，本仓库的实际真相散落在 `AGENTS.md`、skill 文档和 `changes/archive/**/specs/*.md` 里，靠人肉保持同步。
2. **delta 不合并**：归档时官方要求把 change 的 delta specs 合并进 `openspec/specs/`（`RENAMED → REMOVED → MODIFIED → ADDED`）。本仓库**从不合并**，69 份 delta spec 被冻结在 archive 里。
3. **CLI 不可用**：全局仅有孤儿 `openspec.ps1`，`node_modules` 无对应包；官方 `openspec archive / validate / list` 全部用不了，归档靠手工搬目录。
4. **自定义治理层**：本仓库额外定义了 `状态` 字段、`summary.md` 硬要求、`fast/light/full/architecture` 分级、`R0~R3` 复盘强度、以及 `archive/README.md` 的"归档不等于已实施"——这些官方都没有。

后果：没有单一契约入口；归档动作分散在脚本与人工；治理规则与官方文档各说各话，新协作者（或 AI）需两套心智模型。

## 2. 目标

1. 恢复官方 CLI（`@fission-ai/openspec@latest`，当前最新 `1.13.0`），并在本仓库内可用。
2. 建立官方目录语义：`openspec/specs/<capability>/spec.md` 为唯一当前真相；`openspec/changes/<id>/specs/...` 只放 delta（`ADDED/MODIFIED/REMOVED/RENAMED`）。
3. 采用官方完成流程：`apply → (verify) → (sync) → archive`，归档时由 `openspec archive` 合并 delta 并移动目录到 `archive/YYYY-MM-DD-<id>/`。
4. 把 `AGENTS.md` / `openspec/README.md` / `openspec/templates/` 与官方流程对齐，消除冲突表述。
5. 收尾两个已完成但未归档的 change（`arcane-missile-stun-example`、`slim-item-test-templates`）。

## 3. 非目标

- 不重写 `War3Frame/`、`War3Frame.Generator/`、`FrameBuild/`、`CSharpWar3Frame/`、`Projects/` 的任何运行时代码。
- 不在本提案内把全部 69 份历史 delta 回填成 specs（见 Phase 2，另行评估）。
- 不引入 OpenSpec `Stores`（beta）或其他未使用特性。
- 不删除用户已有的分级/复盘/`summary.md` 等治理约定——是否保留见 §5 决策点 D3。

## 4. 变更范围 / What

- 工具：全局安装并验证 `@fission-ai/openspec`；清理孤儿 `openspec.ps1`。
- 目录：用官方脚手架重建 `openspec/` 元数据（`project.md` 等），确认 `specs/` 生效。
- 治理文档：改写 `AGENTS.md` 的 OpenSpec 章节、`openspec/README.md`、`openspec/changes/archive/README.md`；处置 `openspec/templates/`。
- 变更记录：本 change 自身按官方格式提供 delta spec。
- 收尾：归档两个已完成 change。

## 5. 决策点（已于 2026-09-12 批准）

> 批准结论：D1 = 保留附加层（方案 B）；D2 = Phase 1 仅向前，Phase 2 再精选回填；D3 = 迁移进 repository-governance spec 后删除 `templates/`。以下保留备选记录。
>
> 补充决策（实施中发现 `.gitignore` 忽略 `/openspec`，2026-09-12 追加批准）：
> - **D4 = 解除忽略，纳入版本控制**：删除 `.gitignore` 的 `/openspec`，specs、changes、archive 全部纳入 git。
> - **D5 = 既有未提交归档并入本次实施提交**：工作区已有的 67 个未提交删除（20 个 change 移入 archive）随本次改动一并 commit。
>
> 这两项扩展了原提案范围，用户已明确批准，故本提案升级为覆盖 D1~D5。

### D1. 仓库自定义治理层是否保留
- **官方**：无 `状态` 字段、无 `summary.md`、无分级、无 `R0~R3`；"完成"由 tasks 勾选 + delta 合并 + 目录位置证明。
- **本仓库**：上述全部都有，且由用户历史批准。
- **建议**：**保留为"薄附加层"**——官方机制负责真相与归档，`summary.md` / `R0~R3` 作为本仓库附加质量证据继续保留，但要在文档中明确"附加、不阻塞官方 `archive`"。删除它们属于治理资产损失，且不可快速回滚。
- **备选**：完全纯官方，删除 `状态`/`summary.md`/分级/R 强度。

### D2. `openspec/specs/` 如何初始化
- **方案 (a) 仅向前**：`specs/` 只承载本提案引入的 `repository-governance` capability，未来归档时逐步合入。
- **方案 (b) 精选回填**：从 `AGENTS.md` + 最新 delta 提炼核心 capability（native-sync、ecs-message-naming、unit-lifecycle、effect-settlement、ability、item）。
- **方案 (c) 全量回填**：把 69 份 delta 全部合并——工作量与冲突风险最大。
- **建议**：Phase 1 用 (a)，Phase 2 用 (b)。(c) 不做。

### D3. `openspec/templates/`（`proposal-levels.md` / `review-checklist.md`）
- 官方由 CLI 的 schemas/templates 提供脚手架，本仓库模板与之重叠。
- **建议**：保留内容但标注"本仓库附加约定，官方模板由 CLI 提供"，或迁移进 specs 的 repository-governance capability 后删除文件。倾向后者。

## 6. 全局影响分析

- `War3Frame/`: 不受影响——本提案只改 `openspec/` 与治理文档。
- `War3Frame.Generator/`: 不受影响。
- `FrameBuild/`: 不受影响。
- `CSharpWar3Frame/`: 不受影响。
- `Projects/`: 不受影响——两个待归档 change 的功能已实施，本提案不改其代码。

## 7. 风险与回滚

- **风险 1**：`openspec init` 可能覆盖/追加 `AGENTS.md` 或写入 `.opencode/` 等工具目录，与现有自定义治理冲突。→ 缓解：先备份、逐项审查 diff，不盲目接受。
- **风险 2**：官方 `archive` 在无 TTY/CI 环境必须 `--yes`，否则 exit 1；且会尝试合并 delta。→ 缓解：对无 delta 的 change 使用 `--skip-specs`。
- **风险 3**：误删用户治理资产（`summary.md`/分级/R 强度）。→ 缓解：D1 默认保留。
- **风险 4**：`openspec/specs/` 空置若被 Phase 2 仓促回填，可能写入非当前真相。→ 缓解：Phase 2 逐 capability 核对代码后再合入。
- **回滚**：官方 CLI 可卸载；`openspec/` 目录天然受 git 管控，可 `git restore`。`openspec/specs/` 保持可空，回滚成本低。

## 8. 验收标准

1. `openspec --version` 可输出 `1.13.x`（或当时最新稳定版），`openspec validate --all`（或等价）可运行。
2. `openspec/specs/repository-governance/spec.md` 存在，且本 change 的 delta 可被 `openspec archive` 正确合并。
3. `AGENTS.md` / `openspec/README.md` / `archive/README.md` 不再含与官方冲突的表述。
4. `openspec/changes/` 下不再有 `待审核`-之外的两个已完成 change；`arcane-missile-stun-example`、`slim-item-test-templates` 已入 `archive/2026-09-12-*`。
5. 仓库可 `git status` 审阅全部改动，无意外文件。

## 9. 阶段拆分（见 design.md）

- **Phase 1**：CLI 恢复 + 官方目录语义生效 + 本 change 走一遍官方 archive（dogfood）。
- **Phase 2**：精选回填核心 capability 到 `openspec/specs/`（独立评估）。
- **Phase 3**：治理文档收口（AGENTS.md / README / templates）。

## 相关文档

- `openspec/README.md`、`AGENTS.md`、`openspec/changes/archive/README.md`
- `openspec/templates/proposal-levels.md`、`review-checklist.md`
- 官方：`github.com/Fission-AI/OpenSpec`（`docs/workflows.md`、`docs/cli.md`、`src/core/archive.ts`）

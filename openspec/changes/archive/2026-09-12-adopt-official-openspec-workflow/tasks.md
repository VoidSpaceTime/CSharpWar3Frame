# 任务：对齐官方 OpenSpec 工作流

> 仅实现已批准范围。发现新增范围立即回到提案阶段升级。
> 状态: `已实施`（2026-09-12）

## Phase 0 — 准备与决策确认

- [x] 0.1 确认 D1/D2/D3，并追加 D4（解除忽略）/D5（既有归档并入提交）
- [x] 0.2 `git status` 确认工作区状态，记录基线 commit `e0bd026`
- [x] 0.3 备份：既有改动内容已记录于对话与 change 工件；未跟踪文件不再回滚（见 summary 剩余风险）

## Phase 1 — 机制生效

- [x] 1.1 `npm install -g @fission-ai/openspec@latest`，`openspec --version` 输出 1.13.0
- [x] 1.2 原 `openspec.ps1` 由本次安装重建为有效 shim，无需额外清理
- [x] 1.3 受控执行 `openspec init --tools none --language 中文`，审查 diff：仅新增 `config.yaml` 与 `specs/.gitkeep`，未触碰 `AGENTS.md` / `.opencode/`
- [x] 1.4 确认 `openspec/specs/` 生效、`openspec/config.yaml`（schema: spec-driven）生成
- [x] 1.5 `openspec validate` 正常运行

## Phase 1b — 归档两个已完成 change

- [x] 1b.1 归档 `slim-item-test-templates`（`--skip-specs --yes`）→ `archive/2026-09-12-slim-item-test-templates/`
- [x] 1b.2 归档 `arcane-missile-stun-example`（`--skip-specs --yes`）→ `archive/2026-09-12-arcane-missile-stun-example/`
- [x] 1b.3 按 D1 更新其 `proposal.md` 状态为 `已实施`，保留 `summary.md`

## Phase 1c — 本 change 自证（dogfood）

- [x] 1c.1 `openspec validate adopt-official-openspec-workflow` 通过
- [x] 1c.2 `openspec archive adopt-official-openspec-workflow --yes`
- [x] 1c.3 确认 `openspec/specs/repository-governance/spec.md` 由 delta 合并生成
- [x] 1c.4 确认目录移到 `archive/2026-09-12-adopt-official-openspec-workflow/`

## Phase 3 — 治理文档收口

- [x] 3.1 改写 `AGENTS.md` OpenSpec 章节为官方流程 + 附加层引用
- [x] 3.2 重写 `openspec/README.md`
- [x] 3.3 改写 `openspec/changes/archive/README.md`（归档=官方完成动作）
- [x] 3.4 处置 `openspec/templates/`：内容迁入 repository-governance spec 后删除

## Phase 4 — 验证与总结

- [x] 4.1 `git status` 审阅全部改动
- [x] 4.2 按 `R3 Comprehensive` 覆盖五类视角（见 `summary.md`）
- [x] 4.3 生成 `summary.md`
- [x] 4.4 输出架构级总结（见 `summary.md`）
- [x] 4.5 提交（用户 D5 明确授权）

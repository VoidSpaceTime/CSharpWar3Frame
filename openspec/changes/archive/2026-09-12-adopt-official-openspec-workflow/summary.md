# Summary：对齐官方 OpenSpec 工作流

- Change ID: `adopt-official-openspec-workflow`
- 等级: `architecture` → 状态: `已实施`
- 归档: 2026-09-12，`openspec/changes/archive/2026-09-12-adopt-official-openspec-workflow/`
- 基线 commit: `e0bd026`

## 阶段结果

**Phase 1（机制生效）**
- 安装官方 `@fission-ai/openspec@1.13.0`，`openspec --version` 可执行。
- `openspec init --tools none --language 中文` 只新增 `openspec/config.yaml`（schema: `spec-driven`）与 `specs/.gitkeep`；未触碰 `AGENTS.md` / `.opencode/`。
- 解除双重屏蔽：`.gitignore` 删除 `/openspec`；`.git/info/exclude` 移除 `openspec/README.md`、`openspec/templates/`、`openspec/changes/`。openspec 由此纳入版本控制（D4）。

**Phase 1b（既有欠账收尾）**
- `slim-item-test-templates`、`arcane-missile-stun-example` 经官方 `openspec archive --skip-specs --yes` 归档到 `archive/2026-09-12-*`；`proposal.md` 状态更新为 `已实施`，保留 `summary.md`。
- 工作区既有的 67 个未提交删除（20 个 change 移入 archive）纳入本次提交（D5）。

**Phase 1c（自证 dogfood）**
- `openspec validate adopt-official-openspec-workflow` → valid。
- `openspec archive adopt-official-openspec-workflow --yes` → 合并 delta：`repository-governance: create`，`+9` requirements；目录移入 archive。

**Phase 3（文档收口）**
- `AGENTS.md`：OpenSpec 章节由自定义六段流程改为官方 `propose→review→apply→verify→sync→archive` + 附加层引用。
- `openspec/README.md`：重写为三层结构 + 官方命令 + delta 格式 + 附加层说明。
- `openspec/changes/archive/README.md`：语义改为"归档即官方完成动作"。
- `openspec/templates/`：两个模板的内容（分级、复盘强度、总结、交付门禁）迁入 `repository-governance` spec 后删除。

## 迁移状态

| 项 | 状态 |
|---|---|
| 官方 CLI | 已装 1.13.0，可用 |
| `openspec/specs/` | 已建立，含 `repository-governance`（9 requirements），Purpose 已实写 |
| openspec 版本控制 | 已纳入（.gitignore + local exclude 双重解除） |
| 已有 change delta | 历史 69 份仍冻结在归档中（Phase 2 待评估，非本次范围） |
| 治理文档 | 已对齐 |

## 验证覆盖（R3 Comprehensive）

- **目标/约束**：验收标准 1–5 全部达成——CLI 版本、spec 生成、文档无冲突表述、两 change 入档、`git status` 可审阅。verdict：**通过**。
- **技术质量**：`openspec validate adopt-official-openspec-workflow` = valid；归档后 `validate repository-governance --type spec` = valid（Purpose 占位警告已修复）；`list`/`list --specs` 输出符合预期。verdict：**通过**。
- **安全**：无密钥/敏感信息引入；仅删除 `openspec/templates/`（内容已迁入 spec）；`.git/info/exclude` 保留 `.opencode/`、`.sisyphus/`、`.codegraph` 等本地 scratch 不受影响。verdict：**通过**。
- **QA**：实际执行 `init` / `validate` / `archive`（含 `--skip-specs`）+ dogfood 全链路，产出 `openspec/specs/repository-governance/spec.md`。verdict：**通过**。
- **上下文**：`AGENTS.md`、`openspec/README.md`、`archive/README.md` 无悬挂 `templates/` 引用、无旧流程残留。verdict：**通过**。

## 剩余风险 / 未完成事项

1. **Phase 2（精选回填）未执行**：`AGENTS.md` 中的领域规则（ECS 命名、Native 分层、句柄配对等）与 69 份历史 delta 尚未迁入 `openspec/specs/`。当前真相仍部分依赖 `AGENTS.md`，需后续独立提案处理。
2. **未跟踪文件不可回滚**：Phase 0 未对未跟踪的 openspec 文件做副本备份；本次已提交后由 git 提供回滚能力。
3. **opencode 集成未生成**：本次用 `--tools none`，未生成 `/opsx:*` 命令文件；如需可在后续用 `openspec update --tools opencode` 生成。
4. **历史归档 delta 未合并**：`archive/` 中历史 change 的 delta specs 保持冻结，不构成当前真相。

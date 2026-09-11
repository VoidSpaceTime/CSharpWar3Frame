# OpenSpec 使用说明

本仓库采用官方 `OpenSpec`（`@fission-ai/openspec`）作为变更治理机制。版本化真相随仓库提交，所有人（含 AI）读同一份。

## 三层结构

| 位置 | 含义 |
|---|---|
| `openspec/specs/<capability>/spec.md` | **当前真相**（current reality），已部署行为的唯一入口 |
| `openspec/changes/<id>/` | 提案与 **delta specs**（`ADDED / MODIFIED / REMOVED / RENAMED`） |
| `openspec/changes/archive/<yyyy-MM-dd>-<id>/` | 已关闭的变更，保留审计历史 |

## 固定流程

`propose -> review -> apply -> verify -> sync -> archive`

- **propose**：`openspec new change` 或 `/opsx:propose`，在 `openspec/changes/<id>/` 下写 `proposal.md`（必要时 `design.md`、`tasks.md`、`specs/<capability>/spec.md`）。
- **review**：提交用户审核，未批准不得实现。
- **apply**：`/opsx:apply`，只实现已批准范围。
- **verify**：`openspec validate <change>`，并按风险执行构建、测试或静态检查。
- **sync**：归档时官方自动合并 delta；也可先执行 `/opsx:sync`。
- **archive**：`openspec archive <change> --yes`。

## 常用命令

- `openspec list`、`openspec list --specs`
- `openspec validate <change>`、`openspec validate --all`
- `openspec show <change>`
- `openspec archive <change> --yes`；无 delta 的变更用 `--skip-specs`
- `openspec update`（刷新 agent 指令，不重复 `init`）

## delta 格式

change 的 `specs/<capability>/spec.md` 只写增量，且每个 requirement 至少含一个 scenario：

```markdown
## ADDED Requirements

### Requirement: 示例能力
系统 SHALL ...

#### Scenario: 正常路径
- **WHEN** ...
- **THEN** ...
```

## 归档行为

`openspec archive` 会：校验 change 与 delta → 按 `RENAMED → REMOVED → MODIFIED → ADDED` 合并进 `openspec/specs/` → 移动目录到 `archive/<yyyy-MM-dd>-<id>/`。任一步失败会回滚主 specs 并把 change 留在原位。无终端/CI 场景必须加 `--yes`。

## 仓库附加层（不阻塞官方）

- 工件分级 `fast | light | full | architecture`、复盘强度 `R0~R3`、`summary.md` 为本仓库附加约定。
- 细则见 `openspec/specs/repository-governance/spec.md`。
- 附加层不改变官方 `archive` 门禁（tasks 勾选 + delta 合法性）。

## 冲突仲裁

若 `README`、模板与 capability spec 有冲突，以 `openspec/specs/.../spec.md` 中的 requirement 为准。

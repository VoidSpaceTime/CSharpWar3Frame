# 提案：把 ECS 消息与 Tag 命名规则写入 AGENTS.md

- Change ID: `document-ecs-message-naming`
- 提案等级: `light`
- 状态: `已实施`
- 目标一句话: 把已讨论定稿的 Event / Request / Command / Outcome / Tag 开发规则写入 `AGENTS.md`，作为后续代码的命名与生命周期约束。
- 请求来源: 用户要求统一触发器开发规则，并明确写入 `AGENTS.md`。
- 默认实施后审查强度: `R0 Direct`
- 命中的审查升级触发器: 无（纯治理文档，不改运行时契约）
- 最终实施后审查强度: `R0 Direct`
- Oracle 可用性与 `R1` 回退方式: 不需要 `R1`
- 完整 `review-work` 授权来源: 无

## 3.1 背景与目标

框架同时使用信号事实（`XxxEvent`）和请求-响应（`XxxRequest`）。现有代码里 Tag、Request、Event、Outcome 后缀混用，需要在 `AGENTS.md` 固化命名、挂载位置和生命周期，避免新代码继续把 `Request` 做成 `ITag`，或把工作流结果与对外事件混名。

## 3.2 影响范围

- 模块：仓库治理文档
- 文件：`AGENTS.md`；本 change 的 `proposal.md` / `summary.md`
- 不受影响区域：`War3Frame/`、`War3Frame.Generator/`、`FrameBuild/`、`CSharpWar3Frame/`、`Projects/` 运行时与构建代码均不改

## 3.3 方案摘要

在 `架构设计原则` 中、`War3 原生调用分层规则` 之后新增一节，记录：

- `Event` / `Request` / `Command` / `Outcome` / `Dirty` / 分类 Tag / 内部阶段 Tag 的职责与命名
- 挂载位置：独立事件实体 vs 主体
- 禁止事项：零数据 `Request` 做成 `ITag`；用 Tag 对外广播；把 `Outcome` 收进 `Command`/`Request`
- 现有违规只作反例，本次不改代码

## 3.4 风险与回滚

- 风险：文档与个别历史命名不一致，可能被误读为必须立刻重构。缓解：明确“新代码遵守，旧代码不在本 change 清理”。
- 回滚：删除新增章节即可。

## 3.5 验收标准

- `AGENTS.md` 含完整消息/Tag 规则，且与会话中已确认的结论一致。
- 未改任何 `.cs` 文件。
- 本 change 有 `summary.md`。

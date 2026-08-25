# 总结：document-ecs-message-naming

在 `AGENTS.md` 的「架构设计原则」中、原生调用分层规则之后新增「ECS 消息与 Tag 命名规则」。未改任何运行时代码。

写入内容：`Event` / `Request` / `Command` / `Outcome` / `XxxNativeRequest` 的职责与挂载位置；Tag 仅用于分类、脏标记、同实体内部阶段；禁止把 `Request` 做成 `ITag`、用 Tag 对外广播、把 `Outcome` 收进 `Command`/`Request`；历史违规只作反例。

验证：`AGENTS.md` 新章节与会话定稿一致；无 `.cs` 改动。R0 Direct。无阻塞项。

跟踪风险：`.git/info/exclude` 忽略 `openspec/changes/`，本 change 的 `proposal.md` / `summary.md` 当前不出现在 `git status` 中。`AGENTS.md` 已由 Git 跟踪。提交归档或变更记录前需先处理这些文件的跟踪状态。本 change 不调整 exclude。

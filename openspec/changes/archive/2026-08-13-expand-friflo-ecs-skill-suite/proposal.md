# 扩展 Friflo ECS 项目级 Skill 套件

## 等级

`light`

本变更只新增或修订 `.opencode/skills/` 下的项目知识文档，不修改运行时代码、公共 API、依赖、生成器输出或构建链路。由于涉及多份 skill、版本边界和覆盖矩阵，补充 `design.md` 与 `tasks.md` 以便审核和验收。

## 背景

仓库目前只有 `friflo-ecs-relations`，内容集中于 `IRelation<TKey>`、`ILinkComponent` 和 `ILinkRelation`。实际项目还大量使用 `EntityStore`、Entity/Component/Tag、Query、Filter、`QuerySystem<T>`、`SystemRoot` 与 `SystemGroup`，现有 skill 无法覆盖日常设计、实现和审查所需知识。

当前运行时依赖 `Friflo.Engine.ECS 3.4.2`，而官方文档已包含 3.5/3.6 功能。若不固定版本基线，最新版文档可能被误用为当前项目能力。

## 目标

- 以 `3.4.2` 为唯一可执行基线，完整覆盖当前项目实际采用的 Friflo ECS 主路径。
- 新增 `friflo-ecs-core`、`friflo-ecs-query`、`friflo-ecs-systems`，并校订现有 `friflo-ecs-relations`。
- 每个 skill 提供简体中文职责摘要、官方来源、项目实例、关键约束和版本信息。
- 区分 Friflo 官方能力与 CSharpWar3Frame 的调度、注册及 Native 分层约定。
- 建立后续 Friflo 版本升级时同步复核 skill 的维护规则。

## 非目标

- 不镜像或逐页翻译 Friflo 官方文档与 API Reference。
- 不升级 `Friflo.Engine.ECS` NuGet 版本。
- 不修改任何 C# 代码、系统注册行为、ECS 数据模型或 War3 Native 分层。
- 不为当前仓库未采用的 Events/Signals、Index/Search、Batch、序列化、Query Generator、Boost 单独建立 skill。
- 不把 3.5/3.6 API 描述为 `3.4.2` 已可用能力。

## 影响范围

- `.opencode/skills/`：新增三个 skill，校订一个现有 skill。
- `openspec/changes/expand-friflo-ecs-skill-suite/`：记录方案、设计和实施任务。
- `War3Frame/`：仅作为真实用例与版本基线来源，不修改行为。
- `War3Frame.Generator/`：不受影响；skill 只说明官方系统能力与项目生成注册边界。
- `FrameBuild/`：不受影响。
- `CSharpWar3Frame/`：不受影响。
- `Projects/`：仅作为集成用例来源，不修改项目或行为。

## 方案摘要

采用四个职责互斥的专题 skill：

- `friflo-ecs-core`：`EntityStore`、Entity 生命周期、Component/Tag、结构变更与 `CommandBuffer` 基础约束。
- `friflo-ecs-query`：Query、Filter、`ForEachEntity` 与查询期间结构变更限制。
- `friflo-ecs-systems`：`BaseSystem`、`QuerySystem`、`SystemRoot/SystemGroup`、`UpdateTick`，以及项目调度和生成注册边界。
- `friflo-ecs-relations`：继续专门覆盖 Relations 与 Relationships，不并入 core。

Events/Signals、Index/Search、Batch、序列化、Query Generator 和 Boost 仅作为“当前仓库未采用的条件性能力”列出用途与引入前核验要求。

## 风险与回滚

- **版本漂移**：所有无版本标记的内容必须适用于 `3.4.2`；更高版本内容只能标为升级后候选并注明最低版本。
- **职责重复**：通过 `design.md` 的主题所有权矩阵确保每项知识只有一个主 skill。
- **误导性本地化**：内容必须是项目化摘要并附官方链接，不复制整段官方文档。
- **项目扩展与官方能力混淆**：项目特有调度、生成注册和 Native 分层必须显式标注来源。
- **回滚**：删除新增 skill，并恢复 `friflo-ecs-relations` 到变更前版本；不涉及运行时迁移。

## 验收标准

- 四个 skill 均包含触发场景、`3.4.2` 基线、官方来源、真实仓库实例和关键约束。
- 覆盖矩阵中的每个当前项目需求均映射到唯一主 skill，并能定位到至少一个真实仓库文件。
- 3.5/3.6 内容均有明确最低版本和“升级后候选”标识，不进入当前推荐路径。
- 所有引用的仓库路径与标识符可检索，官方链接可访问。
- 四个 skill 之间不存在相互冲突的 API 所有权、结构变更建议或关系语义。
- 未修改 `War3Frame/`、`War3Frame.Generator/`、`FrameBuild/`、`CSharpWar3Frame/` 和 `Projects/` 的行为文件。

## 审核门禁

本提案获用户明确批准后，才进入 skill 实施与验证阶段。若实施中需要升级 NuGet、修改代码示例或改变系统注册约定，必须停止并另开至少 `full` 级提案。

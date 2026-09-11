# 设计：Friflo ECS 项目级 Skill 套件

## 设计原则

1. 以仓库实际使用面决定覆盖范围，不按官方站点目录机械拆分。
2. 以 `Friflo.Engine.ECS 3.4.2` 为当前行为基线；官方最新版仅作为来源和升级参考。
3. 每个主题只有一个主 skill，其他 skill 只做链接，不重复维护完整说明。
4. 本地内容采用中文摘要、仓库实例和决策规则，不镜像官方正文或完整 API Reference。
5. Friflo 官方语义、CSharpWar3Frame 扩展和 War3 Native 分层必须明确区分。

## Skill 结构

每个 `SKILL.md` 使用一致结构：

1. 适用场景与不适用场景。
2. 版本基线与官方来源。
3. 核心概念及 API 边界。
4. CSharpWar3Frame 当前实例。
5. 常见陷阱和结构变更约束。
6. 与其他 Friflo skill 的职责边界。
7. 条件性能力及引入前检查。

## 职责边界

### `friflo-ecs-core`

- 主责：`EntityStore`、Entity 生命周期、`IComponent`、`ITag`、Archetype 基础、增删组件与 Tag。
- 主责：结构变更的通用安全规则和 `CommandBuffer` 的基础用途。
- 不负责：具体 Query 写法、System 调度、Link/Relation 语义。

### `friflo-ecs-query`

- 主责：Query、Filter、`AnyTags`、`ForEachEntity`、查询迭代和过滤规则。
- 主责：查询期间结构变更限制；需要延迟变更时链接到 core 的 `CommandBuffer` 规则。
- 不负责：系统树调度、Link/Relation 建模。

### `friflo-ecs-systems`

- 主责：`BaseSystem`、`QuerySystem`、`SystemRoot`、`SystemGroup`、`UpdateTick`。
- 说明 `TimedSystemRoot`、`ITimedSystem` 和 `SystemRegisterAttribute` 属于项目扩展或集成约定，不冒充 Friflo 官方 API。
- 只描述 System 中如何使用 `CommandBuffer`，不重复其通用安全规则。

### `friflo-ecs-relations`

- 主责：`IRelation<TKey>`、`ILinkComponent`、`ILinkRelation`、incoming links、删除联动与数量/性能边界。
- 保留现有仓库案例和迁移审查清单，但逐项核验是否适用于 `3.4.2`。
- 不负责普通 Component/Tag 或工作流请求组件的通用说明。

## 项目需求覆盖矩阵

| 项目需求 | 主 Skill | 仓库证据 |
| --- | --- | --- |
| `EntityStore` 初始化与实体生命周期 | `friflo-ecs-core` | `War3Frame/initialization/ECSInit.cs`、`Projects/test/Scripts/Process/ItemCompanionAbilityValidationScenario.cs` |
| Component/Tag 的增删与读取 | `friflo-ecs-core` | `War3Frame/Src/Systems/Ability/AbilitySlotSystem.cs`、`War3Frame/Src/Systems/Native/UnitNativeSystem.cs` |
| Query、Filter 与迭代 | `friflo-ecs-query` | `War3Frame/Src/Systems/Ability/AbilityEffectSystems.cs` |
| 系统树、分组与 Tick 调度 | `friflo-ecs-systems` | `War3Frame/Src/Systems/TimedSystemRoot.cs`、`War3Frame/initialization/ECSInit.cs` |
| 项目生成注册边界 | `friflo-ecs-systems` | `War3Frame.Generator/`、`SystemRegisterAttribute` 使用点 |
| Link/Relation 建模与反向查询 | `friflo-ecs-relations` | `War3Frame/Src/Components/Attribute/Attribute.cs`、`War3Frame/Src/Helpers/AttributeHelper.cs` |
| 请求/结果式 ECS 工作流 | core + query 引用，不建立独立事件 skill | `War3Frame/Src/Systems/Ability/AbilitySlotSystem.cs` |

## 版本治理

- 当前基线由 `War3Frame/War3Frame.csproj` 中的 `Friflo.Engine.ECS 3.4.2` 决定。
- 无版本标记的示例与建议必须在 `3.4.2` 下成立。
- 3.5/3.6 内容使用“升级后候选”标记并注明最低版本，不提供当前项目可直接采用的代码路径。
- 官方最新版文档与 `3.4.2` 行为发生冲突或无法确认兼容性时，记录为待核验，不进行推断。
- 后续 NuGet 版本升级必须触发四个 skill 的同步复核；若只修正文档版本标注且不改变项目约定，可另走 `fast` 提案。

## 条件性能力

以下主题当前没有直接使用证据，不建立独立 skill：

- Events / Signals
- Component Index / Search
- Batch Operations
- Serialization
- Query Generator / Parallel Job / SIMD / Boost
- Native AOT 与 Unity Extension

四个核心 skill 可在相关位置简述其用途、当前仓库状态、最低版本核验要求和启用条件，但不得称为“不支持”。实际引入任一能力时，应根据代码与架构影响另开 OpenSpec 提案。

## 来源策略

- 文档目录优先引用 `https://friflo.gitbook.io/friflo.engine.ecs/documentation/` 与官方具体主题页。
- API 类型优先引用官方 `friflo/Friflo.Engine-docs`。
- 版本变化优先引用官方 Release Notes 和对应 release tag。
- 仓库实例只引用路径和必要标识符，不复制大段代码。

## 备选方案

### 单一大型 Skill

入口简单，但上下文体积大、主题职责混杂、版本维护困难，不采用。

### 按官方目录建立完整 Skill 镜像

覆盖表面最广，但包含大量当前未使用能力，并带来版权、版本漂移和维护成本，不采用。

### 四个项目导向 Skill

覆盖当前真实主路径，保留专题边界，并允许按需加载，采用此方案。

## 验证设计

- 检查四个 skill 的元数据、章节结构、版本基线与官方来源。
- 检索覆盖矩阵中的路径和标识符，确认实例真实存在。
- 交叉审查主题所有权，确认无重复或矛盾结论。
- 搜索 `3.5`、`3.6`、`Query Generator` 等内容，确认均带升级候选与最低版本说明。
- 检查 Git diff，确认没有运行时、生成器、构建、CLI 或 Projects 行为改动。

# OpenSpec Proposal

## 0. 基本信息

- Change ID: `fix-friflo-entity-link-relation-startup`
- 提案等级: `full`
- 目标一句话: 修正 `War3Frame` 中以 `Entity` 为 key 的 relation 契约，使 Friflo 在 JIT 启动路径下能够完成 relation schema materialization 并继续创建 `EntityStore`
- 请求来源: 用户在 `BridgeToJIT.dll -> project.dll -> War3Frame.Game.BridgeMain()` 的 JIT 测试链路中，定位到 `EntityStore()` 初始化失败并要求继续推进修复

## 1. 分级判定

### 1.1 为什么是这个等级

- 影响范围：同时涉及 `War3Frame/` 的公开运行时数据结构与 `Projects/test/` 的集成验证路径。
- 风险等级：中。改动很小，但会改变 Friflo 看到的 relation 接口契约。
- 可逆性：高。若验证失败，可以回退为原接口定义。
- 是否跨项目：是，至少覆盖 `War3Frame/` 与 `Projects/test/`。
- 是否改公共契约：是，`HasAbilityStat` / `HasAttr` 是公开 relation struct，直接参与 Friflo schema 注册。

### 1.2 升级触发器检查

- [x] 涉及 `War3Frame/` 与其他项目联动
- [ ] 涉及 `War3Frame.Generator/` 输出或契约
- [ ] 涉及 `FrameBuild/`、构建链路或发布流程
- [ ] 涉及 `CSharpWar3Frame/` 入口行为
- [x] 涉及 `Projects/` 示例或集成验证行为
- [x] 涉及公共 API / 数据结构 / 配置契约
- [ ] 涉及架构边界、目录结构、依赖关系重组

## 2. 背景 / Why

当前 JIT 启动路径已经能够通过 `BridgeToJIT.dll` 成功进入托管入口，但 `new EntityStore()` 仍在 Friflo schema materialization 阶段失败。诊断已把失败点收缩到：

- `War3Frame/Src/Components/AbilityStat.cs` 中的 `HasAbilityStat : IRelation<Entity>`
- `War3Frame/Src/Components/Attribute.cs` 中的 `HasAttr : IRelation<Entity>`

运行时内部异常显示，Friflo 在构造 `EntityLinkRelations<TRelation>` 时要求 `TRelation` 满足 link relation 约束，而当前这两个类型仍仅实现 `IRelation<Entity>`，从而触发泛型约束失败。

## 3. 变更范围 / What

本提案批准后的实现范围仅限：

- 将 `HasAbilityStat` 从 `IRelation<Entity>` 调整为 `ILinkRelation`
- 将 `HasAttr` 从 `IRelation<Entity>` 调整为 `ILinkRelation`
- 仅在编译器或 Friflo API 需要时，调整它们的直接 helper callsite
- 使用现有 `Projects/test/Program.cs` 诊断路径验证 `EntityStore()` 在 JIT 模式下继续执行

本提案**不**包含：

- 不修改 `BridgeToJIT/`、callback、FrameBuild、发布链
- 不修改 Friflo 包或 fork Friflo 上游
- 不扩大为 relation 架构重构
- 不把 AOT relation 注册问题并入本次范围

## 4. 全局影响分析

- `War3Frame/`：直接受影响。公开 relation struct 的接口契约会变化，但字段、构造函数与 `GetRelationKey()` 语义保持不变。
- `War3Frame.Generator/`：无直接变化。本次不改生成器输出。
- `FrameBuild/`：无直接变化。本次不改打包与 staging 行为。
- `CSharpWar3Frame/`：无直接变化。本次不改宿主入口与桥接逻辑。
- `Projects/`：`Projects/test/` 作为集成验证路径会被使用，但不预设必须保留所有临时诊断代码。

## 5. 设计要点

- 以 `Entity` 为 relation key，且语义上表示 entity-to-entity link 的结构，必须与 Friflo 的 `ILinkRelation` 契约对齐。
- 保持现有数据形状不变：`Entity` key 字段、附加 `typeId` 等字段仍保留。
- 不尝试在本次实现中引入新的 relation 抽象层。

## 6. 风险、兼容性、迁移

- 风险：若仓内还有其他 `IRelation<Entity>` 类型，同类问题可能在后续 relation materialization 中继续暴露。
- 兼容性：调用者如果只依赖 `GetRelationKey()` 与构造函数，行为不变；若显式检查接口类型，则会从 `IRelation<Entity>` 变为 `ILinkRelation`。
- 回滚：恢复两个 struct 的接口定义，并撤销直接 helper 适配即可。

## 7. 验证计划

- 构建 `War3Frame` 与 `Projects/test`
- 重新运行 JIT 测试路径
- 确认 relation materialization 不再在 `HasAbilityStat` / `HasAttr` 处触发 `EntityLinkRelations<TRelation>` 泛型约束失败
- 确认 `new EntityStore()` 能继续执行到后续日志

## 8. 拆分任务

- 提案审核通过
- 修改 relation 接口契约
- 重新编译与运行诊断
- 汇总结果并决定是否需要后续 relation 扫描清理提案

## 9. 审批请求

本提案已把范围压到最小：只修正 `HasAbilityStat` / `HasAttr` 的 Friflo relation 契约，并用现有 JIT 验证链确认 `EntityStore` 启动结果。

请审核是否批准按该范围实施。

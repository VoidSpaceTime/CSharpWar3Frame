# Design

## 1. 现状证据

当前仓库中：

- `War3Frame/Src/Components/AbilityStat.cs` 定义 `HasAbilityStat : IRelation<Entity>`
- `War3Frame/Src/Components/Attribute.cs` 定义 `HasAttr : IRelation<Entity>`

JIT 测试诊断已证明：

- `BridgeToJIT` 与托管入口链路正常
- `GetTypes()` 已完成
- 真正失败点在 relation schema materialization
- 运行时内部异常为 `EntityLinkRelations<TRelation>` 对 `TRelation` 的泛型约束不满足

因此问题不在 bridge、callback 或程序集探测噪音，而在**本地 relation 类型与 Friflo relation contract 不匹配**。

## 2. 设计决定

### Decision A: Entity-key relation 改为 `ILinkRelation`

对语义上表示 entity-to-entity link 的 relation：

- `HasAbilityStat`
- `HasAttr`

统一改为实现 `ILinkRelation`，而不再直接使用 `IRelation<Entity>`。

### Decision B: 保持数据结构不变

本次不改：

- 字段名
- 字段数量
- 构造函数参数
- `GetRelationKey()` 的返回逻辑

也就是说，变更仅发生在**Friflo 看到的接口契约层**。

### Decision C: 不改 Friflo 包

虽然也可以通过 fork 或 patch 上游包规避约束，但这会把一次本地类型契约修复升级成依赖管理问题。当前最小可执行修复应优先保持在业务仓库内。

## 3. 备选方案比较

### 方案 A：保持 `IRelation<Entity>`，改 Friflo 或绕开关系路径

- 优点：本地业务类型不变
- 缺点：需要 patch 第三方库或继续扩展诊断/绕行逻辑，范围明显变大

### 方案 B：把本地 Entity-key relation 改为 `ILinkRelation`

- 优点：直接对齐 Friflo 预期，改动最小，验证路径清晰
- 缺点：会改变这两个公开 struct 的接口类型

**推荐：方案 B**

## 4. 影响文件

预计实现涉及：

- `War3Frame/Src/Components/AbilityStat.cs`
- `War3Frame/Src/Components/Attribute.cs`
- 若编译器要求，则包括直接 helper callsite：
  - `War3Frame/Src/Helpers/AbilityHelper.Stat.cs`
  - `War3Frame/Src/Helpers/AttributeHelper.cs`
- 验证路径：`Projects/test/Program.cs`

## 5. 兼容性与风险

- 若仓内还有其他 `IRelation<Entity>` 类型，则它们可能在后续成为同类问题，但不属于本次最小范围。
- 若 helper API 只依赖 relation 的 key 访问行为，改为 `ILinkRelation` 不应改变业务语义。
- 若后续发现 AOT 路径也需要 relation 注册补充，应另开范围，不混入本次修复。

## 6. 验证准则

- `Projects/test` 诊断应不再在 `HasAbilityStat` / `HasAttr` 处出现 relation 泛型约束异常
- `EntityStore()` 应能继续执行到后续日志
- 不引入新的 build 失败

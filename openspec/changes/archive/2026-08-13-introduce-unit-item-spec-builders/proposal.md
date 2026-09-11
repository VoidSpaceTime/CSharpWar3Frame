## 基本信息

- Change ID: `introduce-unit-item-spec-builders`
- 提案等级: `full`
- 目标一句话: 按统一 authoring 原则新增 `UnitSpecBuilder` 与 `ItemSpecBuilder`，让单位/物品模板配置与运行时 helper 职责分离。
- 请求来源: 用户确认“可以按你这个最终统一原则来，拟定 `UnitSpecBuilder` 配置单位模板、`ItemSpecBuilder` 配置物品模板方案”。

## Why

当前单位和物品模板直接在 `IUnitTemplate.Configure(Entity e)` / `IItemTemplate.Configure(Entity item)` 中手写组件，例如 `UnitBase`、`ItemBase`、属性、物品效果和局部技能配置。随着技能侧已经形成 `AbilitySpecBuilder`、`AbilityBehaviorBuilder`、`AbilityEffectSpecBuilder` 的 authoring 风格，Unit/Item 继续散写组件会导致模板配置风格不一致，也容易让“模板定义”和“运行时实体创建/状态切换”边界混淆。

本提案采用统一原则：

- `SpecBuilder` 负责描述“这个东西是什么”。
- `Helper` 负责运行时入口，创建实体或写入状态/请求。
- `System` 推进生命周期和规则副作用。
- `NativeSystem` 执行 War3 原生调用。

因此，`UnitSpecBuilder` / `ItemSpecBuilder` 只进入模板 authoring 层，不替代 `UnitHelper.CreateUnit(...)`、`ItemHelper.EquipToUnit(...)`、`ItemHelper.DropToGround(...)` 等运行时入口。

## What Changes

本提案只拟定方案和约束；用户批准前不实现代码。

计划新增或规范以下 authoring 能力：

- `UnitSpec` / `UnitSpecData` / `UnitSpecBuilder`：配置单位模板的 ID、名称、基础属性、技能、物品槽位、标签/组件扩展。
- `ItemSpec` / `ItemSpecData` / `ItemSpecBuilder`：配置物品模板的 ID、名称、堆叠、可用/可消耗状态、属性贡献、使用技能或使用效果。
- 模板集成方式：现有 `[UnitTemplate]` / `[ItemTemplate]` 与 `IUnitTemplate.Configure` / `IItemTemplate.Configure` 保持不变，模板内部改为调用对应 builder 的 `BuildTo(entity)`。
- 运行时边界：`UnitHelper` / `ItemHelper` 保持 runtime helper 职责，不被 `SpecBuilder` 取代。
- 示例迁移：优先迁移 `Projects/test/Scripts/Template/Unit.cs` 与 `Projects/test/Scripts/Template/Item.cs` 中的代表性示例。

## Capabilities

### New Capabilities

- `unit-item-authoring-model`: 定义 Unit/Item authoring 的 SpecBuilder 命名、职责边界、模板集成与运行时 helper 分层。

## Impact

- `War3Frame/`: 直接新增 Unit/Item authoring 类型和 builder API；不改变 Native 分层。
- `War3Frame.Generator/`: 第一阶段不改 Source Generator；继续复用现有 `[UnitTemplate]` / `[ItemTemplate]` 注册方式。
- `FrameBuild/`: 无直接影响；构建流程、项目模板、资源管线不变。
- `CSharpWar3Frame/`: 无直接影响；CLI 入口不变。
- `Projects/`: `Projects/test` 示例会迁移到新 builder 写法，用于验证 authoring 可读性。

## Non-Goals

- 不让 `UnitSpecBuilder` 直接创建运行时单位 entity。
- 不让 `ItemSpecBuilder` 直接执行装备、卸下、丢弃、使用等状态流转。
- 不改变 `UnitHelper.CreateUnit(...)` 的职责边界。
- 不改变 War3 Native 调用分层；builder 不调用 `JassApi` / `KKApi` / `YDApi` / `DzApi`。
- 不在第一阶段重写 Source Generator 或替换 `[UnitTemplate]` / `[ItemTemplate]`。
- 不一次性迁移全部历史模板，只迁移最小代表示例。

## 验证计划

- 文档阶段：检查 `proposal.md`、`design.md`、`tasks.md`、`spec.md` 内容完整。
- 实现阶段：构建 `War3Frame/War3Frame.csproj` 与 `Projects/test/test.csproj`。
- API 阶段：至少迁移一个单位模板和两个物品模板，证明 `UnitSpecBuilder` / `ItemSpecBuilder` 可读。
- 分层阶段：确认新 builder 不创建运行时 entity、不调用 War3 native、不取代 runtime helper。

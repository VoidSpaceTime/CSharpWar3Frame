## 0. 基本信息

- Change ID: `remove-provisional-ui-kits`
- 提案等级: `full`
- 目标一句话: 暂时移除尚未继续开发的 `War3Frame/Src/Kits` UI 代码，不提供替代 UI，也不联动修改其他代码。
- 请求来源: 用户要求先移除 `src/kits`，暂不编写 UI 或更新相关代码。

## 1. 背景与目标

`War3Frame/Src/Kits` 当前包含 Ability、Attribute、Buff、Inventory 四组未完成 UI 代码，共 5 个 C# 文件。这些文件会被 SDK 默认通配符编译进 `War3Frame`，但仓库内未发现其他源码或项目文件显式引用其中类型。

本变更只删除该目录及其中代码，使当前框架暂不携带这些试验性 UI 套件。后续 UI 设计、基础 UI 层调整和业务接入均不属于本次范围。

## 2. 变更范围

删除以下文件：

- `War3Frame/Src/Kits/Ability/AbilityUI.cs`
- `War3Frame/Src/Kits/Attribute/AttrUI.cs`
- `War3Frame/Src/Kits/Buff/BuffUI.cs`
- `War3Frame/Src/Kits/Inventory/InventoryUI.cs`
- `War3Frame/Src/Kits/Inventory/InventoryUISystem.cs`

不修改 `War3Frame.csproj`、其他 UI 基础代码、调用方、文档、生成器、构建链路或示例项目。

## 3. 分级判定

上述文件声明了多个 `public` Panel 和 System 类型。即使仓库内没有显式引用，删除后这些类型也不再存在于 `War3Frame` 程序集公共 API 中，因此按公共契约变更以 `full` 级处理。

## 4. 全局影响分析

- `War3Frame/`: 受影响。移除 `Src/Kits` 中 5 个 UI 源文件及其公开类型；其他运行时代码不修改。
- `War3Frame.Generator/`: 不受影响。被删除的 UI System 未使用 `SystemRegisterAttribute`，不改变生成器逻辑或输出契约。
- `FrameBuild/`: 不受影响。不改变构建编排、资源和发布流程。
- `CSharpWar3Frame/`: 不受影响。不改变 CLI、参数或配置。
- `Projects/`: 不修改。仓库扫描未发现对被删除类型的显式引用；外部程序集若直接引用这些公开类型，需要自行停止使用。

## 5. 风险与回滚

- 公共 API 风险: 外部调用方若引用被删除的 Panel 或 System 类型，将无法继续编译。
- 功能风险: 依赖这些类型的 UI 功能暂时不可用，这是本次删除的预期结果。
- 范围风险: 不联动更新相关代码；若验证发现仓库内存在遗漏引用，只记录结果，不在本次变更中顺手修复。
- 回滚方式: 从版本控制恢复整个 `War3Frame/Src/Kits` 目录。

## 6. 验收标准

- `War3Frame/Src/Kits` 目录及其中 5 个源文件均已删除。
- 不新增或修改任何 UI 实现，不修改其他运行时代码。
- 仓库静态扫描确认不再包含本次删除的公开类型声明。
- 执行 `dotnet build War3Frame/War3Frame.csproj`，如失败则明确区分删除导致的问题与既有问题。

## 7. 实施前置

本提案及其 `design.md`、`tasks.md`、`specs/provisional-ui-kits-removal/spec.md` 经用户审核批准后，才能删除代码。

## Capability: provisional-ui-kits-removal

### Requirement: Provisional Kits UI source is absent

`War3Frame` MUST NOT 包含当前 `War3Frame/Src/Kits` 下的 Ability、Attribute、Buff 和 Inventory UI 源文件，也 MUST NOT 继续公开这些文件中声明的 Panel 与 System 类型。

### Requirement: Removal remains isolated

本次变更 MUST NOT 新增替代 UI、兼容 shim 或条件编译实现，且 MUST NOT 修改 `War3Frame/Src/Kits` 之外的运行时代码、生成器、构建链路、CLI 或示例项目。

### Requirement: Removal is verifiable

验证 MUST 确认 `War3Frame/Src/Kits` 已无文件、目标类型声明与仓库内显式引用均不存在，并 MUST 执行 `dotnet build War3Frame/War3Frame.csproj`。若构建失败，结果 MUST 明确区分本次删除引起的问题与既有问题。

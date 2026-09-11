## 1. 删除边界

本变更采用整目录删除，移除 `War3Frame/Src/Kits` 下现有 5 个 C# 文件。不会把其中代码迁移到其他目录，也不会保留兼容类型、空壳类或条件编译分支。

## 2. 非目标

- 不设计或实现 Ability、Attribute、Buff、Inventory UI。
- 不修改 `UIManager`、`UIPanel`、`UIFrame`、同步辅助类或 ECS 业务系统。
- 不修改项目文件来显式排除目录，因为源文件删除后 SDK 默认编译项会自然消失。
- 不更新外部或潜在调用方。

## 3. 公共契约处理

删除会移除 `AbilityPanel`、`AbilityUISystem`、`AttrPanel`、`AttrUISystem`、`BuffPanel`、`BuffUISystem`、`InventoryPanel` 和 `InventoryUISystem` 等公开类型。本次按用户要求直接移除，不提供 obsolete 过渡期或兼容 shim。

## 4. 验证策略

1. 检查 `War3Frame/Src/Kits` 不再存在任何文件。
2. 扫描仓库，确认被删除类型没有残留声明或仓库内显式引用。
3. 构建 `War3Frame/War3Frame.csproj`，验证默认编译项删除后项目状态。
4. 检查变更清单，确认除 OpenSpec 工件和目标目录删除外没有其他文件改动。

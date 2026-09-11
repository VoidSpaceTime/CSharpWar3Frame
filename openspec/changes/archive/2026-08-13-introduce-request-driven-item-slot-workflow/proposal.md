## Why

当前物品系统已经完成了“状态语义分层”和“装备态属性生效”的基础设计，但还缺少一套与 `AbilitySlot` 对称、又不污染物品语义的容器工作流。若继续直接在 helper 里处理物品挂载/移除/交换/扩槽，后续背包、装备栏、仓库等容器行为会逐步耦合到状态标签和属性生效流程里。

最合理的方向是引入 `ItemSlot`：

- slot 只负责“物品放在哪个容器位置”
- tag 只负责“物品当前是什么状态”
- 装备效果继续只和 `ItemEquippedTag` 绑定

## What Changes

- 定义 `ItemSlotContainer` 作为物品容器位置模型。
- 定义 request-driven 的 item attach/remove/swap/resize workflow。
- 明确 slot 与状态标签的职责边界。
- 明确物品属性生效只与装备态绑定，而不与 slot 本身绑定。

## Capabilities

### New Capabilities
- `request-driven-item-slot-workflow`: 定义 item 容器位置与状态语义分离的即时工作流。

## Impact

- 直接影响 `War3Frame` 中物品背包、装备栏、仓库与掉落容器工作流。
- 间接影响 item attr apply/remove 的触发时机。
- 本次变更仅新增 OpenSpec 工件，不进入运行时代码修改。

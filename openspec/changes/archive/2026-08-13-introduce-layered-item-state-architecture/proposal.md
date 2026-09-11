## Why

当前仓库已经具备 `ItemBase`、`ItemOwner`、`ItemSlotIndex` 等基础组件，但还没有明确的物品状态架构。若后续直接把“拥有物品”和“物品属性生效”混在一起，背包、装备栏、仓库、地上掉落等语义会很快互相污染：

- 物品属于某单位，不一定应该立即生效
- 物品放在仓库里，不应影响当前单位属性
- 地上的物品是世界实体，不属于任何单位
- 装备态才应触发属性加成

因此，需要正式定义物品的分层状态与状态迁移规则，明确：**归属不等于生效**，属性生效只跟装备态绑定，并且通过 modifier source 机制映射到单位属性系统中。

## What Changes

- 定义四种主状态：地上、背包、装备、仓库。
- 定义 `ItemOwner` 只表达归属，不表达效果是否已生效。
- 定义装备态通过请求/系统把物品属性映射为单位属性 modifier。
- 定义卸下/丢弃/销毁通过 source-based modifier removal 撤销效果。

## Capabilities

### New Capabilities
- `layered-item-state-architecture`: 定义物品状态分层、归属/生效分离与属性应用流程。

## Impact

- 直接影响 `War3Frame` 中物品掉落、拾取、背包、装备、仓库与属性结算的架构。
- `War3Frame.Generator` 需要确认 ItemTemplate authoring 与状态分层设计兼容。
- `FrameBuild` / `CSharpWar3Frame` 预期无直接行为变更。
- `Projects/*` 在实现后必须验证拾取、装备、卸下、丢弃、堆叠与地面物品行为。
- 本次变更仅新增 OpenSpec 工件，不进入运行时代码修改。

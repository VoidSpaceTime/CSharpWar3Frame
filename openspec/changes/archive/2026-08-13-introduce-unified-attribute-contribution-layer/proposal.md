## Why

当前仓库中，多个来源都会长期影响单位属性：

- 物品装备
- 挂载技能
- 光环
- Buff

这些来源的上层语义不同，但底层效果非常相似：它们都会对某个单位的属性施加一组长期贡献，并在来源失效时撤销贡献。当前 item 已经开始走 source-based modifier 路线，buff/aura 也部分落在同一机制上，但仓库还没有把这层正式收敛成统一架构。

如果不把这层统一，后续每新增一种来源，都容易重复发明 apply/remove 流程和来源追踪逻辑。

## What Changes

- 定义统一的“单位属性贡献层”。
- 引入统一的来源标识与属性贡献条目概念。
- 明确 item / ability / aura / buff 在上层生命周期不同，但底层贡献执行链一致。
- 统一 apply/remove 走 source-based modifier 路径。

## Capabilities

### New Capabilities
- `unified-unit-attribute-contribution-layer`: 定义长期单位属性贡献的统一执行层。

## Impact

- 直接影响 `War3Frame` 中 item、挂载技能、aura、buff 对单位属性的持久影响机制。
- 间接影响后续 inventory/equipment、ability contribution、buff/aura 的实现扩展方式。
- 本次变更仅新增 OpenSpec 工件，不进入运行时代码修改。

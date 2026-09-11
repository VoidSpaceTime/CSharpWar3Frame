## Why

当前能力系统已经有统一的 effect pipeline，但伤害语义仍然偏平：默认倾向于把伤害理解成“某次 effect 命中后造成一个数值的伤害”。这不足以表达当前明确存在的需求：

- 冲击波飞过单位时每 0.1s 对范围内单位造成伤害
- 冲击波飞过单位但每个单位只命中一次
- 某区域内每秒造成伤害并持续一段时间

这些需求说明伤害不只是一种“单次命中 payload”，而至少要区分：

- 单次伤害
- 周期伤害
- 命中去重/限频策略
- 区域/轨迹语义

## What Changes

- 为技能伤害建立显式效果分类体系。
- 明确单次伤害、周期伤害、命中策略、区域搜索与轨迹语义分层。
- 明确 effect 系统负责“何时产生伤害”，统一伤害系统负责“最终如何结算伤害”。

## Capabilities

### New Capabilities
- `skill-damage-effect-taxonomy`: 定义技能伤害效果的分类与组合规则。

## Impact

- 直接影响 `War3Frame` 的技能效果组件设计和 damage pipeline 边界。
- 间接影响 projectile、area search、periodic effects、buff 触发和 future item-use effect reuse。
- 本次变更仅新增 OpenSpec 工件，不进入运行时代码修改。

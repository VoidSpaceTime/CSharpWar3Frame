## Why

当前技能系统的核心组织方式仍然偏向“技能模板 + 一组固定 payload 组件”，这在简单技能上足够工作，但随着需求扩展，已经持续暴露结构性不足：

- 一部分技能并不通过技能槽挂载（天赋、被动、系统赋予、物品使用）
- 一部分技能不是主动施法，而是通过受伤、死亡、命中、装备等事件触发
- 一部分技能是复合技能，包含多个阶段（飞行、命中、落地、持续伤害、生成子效果）
- 一部分效果是长期属性贡献，一部分是瞬时伤害/治疗/Buff，一部分又是周期和轨迹语义

这说明当前“组件包式技能”思路已经不足以成为长期主模型。技能系统需要从根上重构为：

- Mount（挂载）
- Trigger（触发）
- Flow（多阶段执行流）
- Settlement（统一结算）

## What Changes

- 将能力系统的主抽象从“模板 + payload 组件集合”提升为“挂载 + 触发 + 执行流 + 结算”四层模型。
- 明确主动技能、被动技能、天赋技能、物品技能、复合技能都可以在同一高层框架内表达。
- 明确效果 payload 只是 flow 节点结果，而不是能力定义本体。

## Capabilities

### New Capabilities
- `mount-trigger-flow-ability-architecture`: 定义新的技能流主架构。

## Impact

- 直接影响 `War3Frame` 中能力模板、能力运行时、事件触发、effect pipeline、属性贡献层与未来的 item-use/被动技能结构。
- 间接影响 ability slot、item slot、buff/aura、move continuation、effect hook 等相关设计。
- 本次变更仅新增 OpenSpec 工件，不进入运行时代码修改。

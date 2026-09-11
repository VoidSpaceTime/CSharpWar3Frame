# 统一效果结算管线

- Change ID: `unify-effect-settlement-pipeline`
- 等级：`full`
- 状态：待审核

## 背景与目标

当前能力效果流程已经有 `DamageEffectSystem`、`HealEffectSystem`、`BuffEffectSystem` 和 `EffectSettlementHelper`，但最终结算责任仍然分散：

- Damage 已经通过 `DamageRequest` 进入 `DamageResolveSystem`，再由 `DamageResolveSystem` 扣血并发布 `DamageEvent`。
- Heal 仍然在 `HealEffectSystem` 中直接调用 `AttributeHelper.ModifyCurrent(...)` 修改 Health。
- Buff 仍然在 `BuffEffectSystem` 中直接调用 `BuffHelper.AddTimedBuff(...)`。
- AuraSystem 当前直接创建 `ModifyValue` / `ModifyTarget` 属性 modifier，没有复用已有 Buff 生命周期、刷新和过期规则。

这样会让后续结算监听、战斗日志、触发效果、治疗修正、护盾、吸血、Buff 结果反馈等功能依赖不同执行点，扩展边界会越来越散。

本 change 的目标是：把 Damage / Heal / Buff 的能力效果统一成 settlement 管线，并让 Aura 产生的属性影响接入已有 Buff 系统。

## 变更内容

计划变更：

- 新增或调整 settlement request / outcome 组件，用来表达效果结算意图和结算结果。
- Damage 继续走 `DamageRequest` -> `DamageResolveSystem` -> `DamageEvent`，保留 `DamageEvent` 作为权威伤害结果事件。
- Heal 不再由 effect system 直接改 Health，而是提交 heal settlement request，由 resolver 执行回血并发布 outcome。
- Buff 不再由 effect system 直接拥有 Buff 生命周期语义，而是提交 buff settlement request，由 resolver 调用已有 Buff 系统入口。
- Aura 不再手工创建裸属性 modifier，而是用 Buff-backed modifier 表达光环影响。
- 保持 projectile、area search、effect lifecycle 的时序语义不变，只替换最终结算入口。

## 影响范围

- `War3Frame/`：受影响。会涉及运行时 effect 组件/系统、伤害结算、Buff/Aura helper/system。
- `War3Frame.Generator/`：不受影响。不改变 Source Generator 输入、输出或生成契约。
- `FrameBuild/`：不受影响。不改变构建编排、模板或发布流程。
- `CSharpWar3Frame/`：不受影响。CLI / 入口项目不需要感知 runtime 内部 settlement 管线。
- `Projects/`：主要作为验证目标。示例 ability/item 声明应尽量保持源码兼容。

公共契约说明：

- `DamageEvent` 必须保留，并继续作为权威伤害结果。
- Heal / Buff 可能新增 settlement outcome 组件。
- `DamageEffectData`、`HealEffectData`、`ApplyBuffData` 尽量保持源码兼容；如果实现阶段发现必须改字段，需要回到提案审查。

## 风险

- 一个 effect 同时包含 Damage + Buff 等多个 payload 时，统一 settlement 可能改变完成时机。
- Aura 接入 Buff 后，buffId、刷新策略、过期清理和 `AttrDirty` 标记必须稳定，否则可能出现光环残留或属性不刷新。
- Heal 改成 request/outcome 后，原本依赖同 tick 立即回血的逻辑需要验证。
- 统一 helper 必须避免重复结算，也不能让 effect entity 过早删除。

## 回滚策略

- 新增 settlement 组件和 resolver system 应保持局部可回滚。
- 如果 Aura 接入 Buff 后行为不稳定，可以先回滚 Aura 部分，保留 Damage/Heal/Buff settlement 重构。
- 如果统一时序影响过大，可以临时保留旧直写系统，同时并行验证 request/outcome 路径。

## 验收标准

- Damage effect 仍创建 `DamageRequest`，由 `DamageResolveSystem` 结算，并发布 `DamageEvent`。
- Heal effect 不再从 effect system 直接调用 `AttributeHelper.ModifyCurrent(...)`；Health 变化由 settlement resolver 完成。
- Buff effect 不直接拥有 Buff 生命周期语义；settlement resolver 通过已有 Buff 系统入口处理。
- Aura 产生的属性影响由 Buff-backed modifier 表达，单位进入/离开光环范围时能正确添加和移除。
- Effect lifecycle 不重复结算，也不会在 Damage/Heal/Buff payload 全部提交前删除 effect entity。
- `dotnet build CSharpWar3Frame.slnx` 或等价局部构建通过。

## 审核说明

本 change 按 `full` 处理，因为它会改变 `War3Frame/` 内核心 runtime 效果结算流程，并涉及多个模块协作。用户明确批准前，不进入实现阶段。

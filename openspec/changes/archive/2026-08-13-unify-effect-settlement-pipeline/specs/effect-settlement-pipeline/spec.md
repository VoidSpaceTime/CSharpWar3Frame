# 能力：效果结算管线

## ADDED Requirements

### Requirement: 统一 settlement 边界

能力效果系统必须通过 settlement request 或等价结算意图组件表达 Damage、Heal、Buff 的最终副作用，而不是各自直接拥有最终副作用。

#### Scenario: Damage effect 提交伤害结算

- **GIVEN** 一个带有 `DamageEffectData`、`EffectSource`、`EffectTargetInfo` 的 effect entity
- **WHEN** 该 effect 可以结算
- **THEN** 系统必须通过 `DamageRequest` 或等价路径提交伤害
- **AND** 伤害必须由 `DamageResolveSystem` 结算
- **AND** 最终伤害结果必须通过 `DamageEvent` 表达

#### Scenario: Heal effect 提交治疗结算

- **GIVEN** 一个带有 `HealEffectData`、`EffectSource`、`EffectTargetInfo` 的 effect entity
- **WHEN** 该 effect 可以结算
- **THEN** 系统必须提交 heal settlement intent
- **AND** Health 必须由 settlement resolver 修改
- **AND** resolver 必须发布包含 source、target、final heal、remaining health 的 heal outcome / event

#### Scenario: Buff effect 提交 Buff 结算

- **GIVEN** 一个带有 `ApplyBuffData`、`EffectSource`、`EffectTargetInfo` 的 effect entity
- **WHEN** 该 effect 可以结算
- **THEN** 系统必须提交 buff settlement intent
- **AND** resolver 必须通过已有 Buff 系统语义应用或刷新 Buff

### Requirement: DamageEvent 保持权威

Damage settlement 必须继续发布 `DamageEvent`，并将它作为已结算伤害的权威 outcome。

#### Scenario: 监听系统读取已结算伤害

- **GIVEN** 一个监听 `DamageEvent` 的系统
- **WHEN** 一个 damage request 被结算
- **THEN** 监听系统必须能够从 `DamageEvent` 读取 source、target、base damage、final damage、remaining health

### Requirement: Aura 使用 Buff-backed modifier

Aura 施加的属性影响必须表示为 Buff-backed modifier，而不是 AuraSystem 自己拥有的裸属性 modifier。

#### Scenario: 单位进入光环范围

- **GIVEN** 一个带有属性效果的 Aura entity
- **AND** 一个有效目标单位进入 Aura 范围
- **WHEN** AuraSystem 更新
- **THEN** 目标必须获得一个链接到该 Aura 的 Buff-backed modifier
- **AND** 受影响属性必须被标记 dirty

#### Scenario: 单位离开光环范围

- **GIVEN** 一个目标单位拥有链接到 Aura 的 Buff-backed modifier
- **WHEN** 该目标不再受 Aura 影响
- **THEN** 关联 Buff 必须被移除
- **AND** 受影响属性必须被标记 dirty

### Requirement: Effect settlement 按 payload 幂等

同一个 effect entity 对同一种 Damage、Heal 或 Buff payload 不得重复结算。

#### Scenario: 多 payload effect 每类 payload 只结算一次

- **GIVEN** 一个 effect entity 同时拥有多个 settlement payload 组件
- **WHEN** 系统跨多个 tick 处理该 effect
- **THEN** 每种 payload 类型最多只能提交一次 settlement
- **AND** 只有当所有 settlement payload 都已提交或移除后，该 effect 才能完成

## MODIFIED Requirements

### Requirement: 现有 ability effect data 尽量保持源码兼容

现有 `DamageEffectData`、`HealEffectData`、`ApplyBuffData` 声明应该尽量保持源码兼容；除非实现证明必须改字段，并且提案已经更新并重新通过审核。

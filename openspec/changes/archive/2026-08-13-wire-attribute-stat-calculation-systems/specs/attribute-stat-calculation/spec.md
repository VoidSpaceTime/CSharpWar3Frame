# attribute-stat-calculation 规格

## ADDED Requirements

### Requirement: Attribute calculation system MUST be registered

`AttrCalculationSystem` SHALL 通过 `[SystemRegister]` 接入运行调度，消费 `AttrDirty` 标签并重算 `AttrValue.finalValue`。

#### Scenario: Dirty attribute recalculation

- **WHEN** 某属性实体被打上 `AttrDirty` 标签
- **THEN** `AttrCalculationSystem` MUST 在后续调度中重算该属性的 `finalValue`
- **AND** MUST 使用 `(base + flat) × (1 + percentAdd) × percentMul` 公式聚合 `ModifyTarget` 修改器
- **AND** MUST 在重算后移除 `AttrDirty` 标签

#### Scenario: No structural change inside query loop

- **WHEN** `AttrCalculationSystem` 遍历属性实体
- **THEN** 查询循环内 MUST NOT 执行 `AddTag` / `RemoveTag` / `DeleteEntity` 等结构变更
- **AND** dirty 标签移除 MUST 在查询结束后统一执行

### Requirement: Ability stat calculation system MUST be registered

`AbilityStatCalculationSystem` SHALL 通过 `[SystemRegister]` 接入运行调度，消费 `AbilityStatDirty` 标签并重算 `AbilityStatValue.finalValue`。

#### Scenario: Dirty ability stat recalculation

- **WHEN** 某技能数值实体被打上 `AbilityStatDirty` 标签
- **THEN** `AbilityStatCalculationSystem` MUST 在后续调度中重算该数值的 `finalValue`
- **AND** MUST 在重算后移除 `AbilityStatDirty` 标签

#### Scenario: No structural change inside query loop

- **WHEN** `AbilityStatCalculationSystem` 遍历技能数值实体
- **THEN** 查询循环内 MUST NOT 执行结构变更
- **AND** dirty 标签移除 MUST 在查询结束后统一执行

### Requirement: Calculation order MUST follow dirty writers

两个计算系统 MUST 在属性/技能数值 dirty 写入方之后、数值消费方之前调度。

#### Scenario: Buff modify then recalc then consume

- **WHEN** `BuffExpireSystem` / `AuraSystem` / 属性贡献系统写入 `AttrDirty`
- **AND** 效果结算系统需要读取最新 `finalValue`
- **THEN** `AttrCalculationSystem` MUST 在该 tick 的 dirty 写入之后、结算之前完成重算

### Requirement: Existing formulas and tags MUST remain unchanged

属性与技能数值的公式、组件结构与 dirty 标签语义 SHALL 保持本 change 前的定义不变。

#### Scenario: No formula change

- **WHEN** 完成接线
- **THEN** `AttrValue` / `AbilityStatValue` / `AttrDirty` / `AbilityStatDirty` 的定义 MUST 不变
- **AND** 计算公式 MUST 不变

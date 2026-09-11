## ADDED Requirements

### Requirement: 通用等级数值规格

系统 SHALL 提供 `LevelValue` 作为 Unit / Item / Ability 共享的按等级解析数值规格。

#### Scenario: 固定值

- **GIVEN** authoring 使用纯数字或 `LevelValue.Fixed(20)`
- **WHEN** 当前等级为任意正整数
- **THEN** 解析结果 SHALL 为 `20`

#### Scenario: 线性成长

- **GIVEN** authoring 使用 `LevelValue.PerLevel(20, 20)`
- **WHEN** 当前等级为 `2`
- **THEN** 解析结果 SHALL 为 `40`

#### Scenario: 等级表

- **GIVEN** authoring 使用 `LevelValue.LevelTable(20, 40, 75)`
- **WHEN** 当前等级为 `3`
- **THEN** 解析结果 SHALL 为 `75`

### Requirement: 纯数字兼容语义

Unit / Item / Ability builder 中已有纯数字数值入口 SHALL 保持固定值语义，不得解释为每级增量。

#### Scenario: Ability 纯数字基础值

- **GIVEN** `.BaseValue(AbilityHelper.DamageAmount, 20)`
- **WHEN** ability 等级为 `3`
- **THEN** `DamageAmount` 的等级解析基础值 SHALL 仍为 `20`

### Requirement: 等级重算触发

系统 SHALL 提供 `LevelStatDirty` 作为等级相关基础数值重算触发标记。

#### Scenario: Dirty 后重算

- **GIVEN** entity 拥有模板 spec、当前等级和 `LevelStatDirty`
- **WHEN** `LevelStatRebuildSystem` 运行
- **THEN** 系统 SHALL 根据当前等级解析模板中的 `LevelValue`
- **AND** 写入等级解析后的基础值
- **AND** 移除 `LevelStatDirty`

### Requirement: LevelStatRebuildSystem 边界

`LevelStatRebuildSystem` SHALL 只负责等级驱动的基础数值解析，不得负责 Buff 合成、装备最终聚合、War3 native 同步或即时伤害治疗结算。

#### Scenario: Native 分层

- **GIVEN** `LevelStatRebuildSystem` 处理任意 Unit / Item / Ability
- **WHEN** 系统执行重算
- **THEN** 系统 SHALL NOT 直接调用 `JassApi` / `KKApi` / `YDApi` / `DzApi`

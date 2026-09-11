## ADDED Requirements

### Requirement: Long-lived unit attribute contributions MUST share one execution layer
所有对单位属性的长期贡献（来自 item、ability、aura、buff）MUST 共享同一条执行层，而 SHALL NOT 各自发明独立的属性应用/撤销机制。

#### Scenario: Item and ability both add strength to a unit
- **WHEN** 物品与挂载技能都对同一单位增加同类属性
- **THEN** they MUST ultimately flow through the same modifier execution path

### Requirement: Source lifecycles MAY differ, contribution execution MUST converge
item、ability、aura、buff 的上层生命周期 MAY 保持不同，但它们的属性贡献执行 MUST 收敛到统一 apply/remove 路径。

#### Scenario: Aura leaves range while item remains equipped
- **WHEN** 光环离开范围而物品仍然装备
- **THEN** aura removal and item persistence MAY follow different lifecycle semantics
- **AND** both MUST still use the same contribution execution model

### Requirement: Source markers SHOULD exist for long-lived contributions
长期属性贡献来源 SHOULD 具备显式来源标识。

#### Scenario: Debugging a unit’s final attribute value
- **WHEN** 系统需要追踪某单位最终属性由哪些来源构成
- **THEN** the contribution sources SHOULD be distinguishable by type

### Requirement: Contribution entries SHOULD be explicit
具体的属性贡献内容 SHOULD 通过显式贡献条目表达，而不是散落在各来源系统流程代码中。

#### Scenario: A source grants multiple attribute modifiers
- **WHEN** 某个来源同时影响多个属性
- **THEN** those contributions SHOULD be expressible as explicit entries

### Requirement: Ability self stats SHALL NOT be merged into the unit contribution layer
`AbilityStat` / `AbilityHelper.Stat` SHALL NOT 被纳入统一单位属性贡献层。

#### Scenario: Ability cooldown and mana cost are evaluated
- **WHEN** 系统读取技能自身冷却或蓝耗
- **THEN** those values SHALL remain in the ability stat system rather than the unit attribute contribution layer

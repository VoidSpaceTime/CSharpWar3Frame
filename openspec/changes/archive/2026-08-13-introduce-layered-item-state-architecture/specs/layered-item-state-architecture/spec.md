## ADDED Requirements

### Requirement: Item ownership MUST NOT imply item effect application
`ItemOwner` MUST 只表示物品归属关系，而 SHALL NOT 直接代表该物品效果已经应用到 owner 身上。

#### Scenario: Item is owned but stored in backpack
- **WHEN** 某个物品属于某单位，但处于背包态
- **THEN** ownership MUST be true
- **AND** item effects MUST NOT be assumed active solely from ownership

### Requirement: The repository MUST model four primary item states
物品系统 MUST 显式支持 Ground、Inventory、Equipped、Stored 四种主状态。

#### Scenario: Item lies on the ground
- **WHEN** 某个物品在世界中可拾取
- **THEN** it MUST be representable as Ground state

#### Scenario: Item is equipped by a unit
- **WHEN** 某个物品被装备
- **THEN** it MUST be representable as Equipped state

### Requirement: Only equipped items MAY apply attribute effects by default
默认情况下，只有装备态物品 MAY 对单位属性生效；Ground、Inventory、Stored 状态 SHALL NOT 自动生效。

#### Scenario: Item is stored in warehouse
- **WHEN** 某个物品被存入仓库
- **THEN** its attribute effects SHALL NOT automatically apply to the current unit

### Requirement: Item attribute effects MUST use source-based modifiers
装备物品带来的属性影响 MUST 通过 source-based modifiers 作用到单位属性系统，而 SHALL NOT 直接永久写入单位基础属性。

#### Scenario: Equipped item grants health bonus
- **WHEN** 某个装备态物品提供生命加成
- **THEN** the bonus MUST be represented through modifier/source linkage to the unit attribute

### Requirement: Item apply/remove flow SHOULD be request-driven
物品效果的应用与移除 SHOULD 通过一次性请求/tag 或等价流程驱动，而 SHALL NOT 以长期 `bool applied` 作为主机制。

#### Scenario: Item is newly equipped
- **WHEN** 某个物品刚进入装备态
- **THEN** the repository SHOULD trigger an explicit application flow rather than rely on a long-lived bool flag

### Requirement: Item effect removal MUST be reversible by source
卸下、丢弃或销毁物品时，系统 MUST 能通过 item source 撤销它带来的属性影响。

#### Scenario: Equipped item is dropped
- **WHEN** 某个装备态物品被丢弃到地上
- **THEN** all attribute effects sourced from that item MUST be removable without requiring permanent attribute rewrites

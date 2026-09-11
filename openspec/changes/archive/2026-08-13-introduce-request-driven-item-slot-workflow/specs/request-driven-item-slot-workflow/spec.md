## ADDED Requirements

### Requirement: Item slot MUST represent container position only
`ItemSlot` MUST 只表达物品在容器中的位置，而 SHALL NOT 直接等同于业务状态。

#### Scenario: Item is in a backpack slot but not equipped
- **WHEN** 某个物品位于背包槽位中
- **THEN** slot presence SHALL NOT by itself imply that the item is equipped or active

### Requirement: Item state tags MUST represent business state
`ItemGroundTag`、`ItemInventoryTag`、`ItemEquippedTag`、`ItemStoredTag` MUST 表达物品业务状态。

#### Scenario: Item is stored in warehouse
- **WHEN** 某个物品处于仓库存储状态
- **THEN** that state MUST be represented through item state tags rather than only slot location

### Requirement: Item slot operations MUST be request-driven immediate workflows
物品 attach/remove/swap/resize MUST 通过 request-driven immediate workflow 处理。

#### Scenario: Item is attached into a slot
- **WHEN** 某个 item attach 请求被发起
- **THEN** it MUST be handled as a discrete immediate workflow action

### Requirement: Equipped effects MUST remain bound to equipped state
物品属性效果 MUST 继续只与 `ItemEquippedTag` 绑定，而 SHALL NOT 因为位于某个 slot 就自动生效。

#### Scenario: Item is in storage slot
- **WHEN** 某个物品有 owner 和 slot，但处于 `Stored` 状态
- **THEN** its attribute effects SHALL NOT automatically apply

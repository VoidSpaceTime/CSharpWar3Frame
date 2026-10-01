## ADDED Requirements

### Requirement: 远程攻击在弹道到达后结算

系统 SHALL 在消费 `AttackType.Ranged` 的 `UnitAttackedRequest` 时创建跟踪弹道，并 SHALL NOT 在弹道到达前创建 `DamageRequest`。弹道到达后，系统 SHALL 按弹道创建时记录的攻击力快照创建 `DamageRequest`，其 `damageSrc` MUST 为 `Ranged`。近战分支 MUST 保持直接结算。

#### Scenario: 远程命中

- **WHEN** 攻击者形态为 `Ranged`、攻击力快照大于 0，且其攻击弹道进入 `ProjectileArrived`
- **THEN** 系统创建一条指向原目标的 `DamageRequest`，伤害等于该快照，且请求实体上的 `UnitAttackedRequest` 已在弹道创建时删除

#### Scenario: 远程飞行中不结算

- **WHEN** 远程攻击弹道仍处于 `PendingStart` 或 `InFlight`
- **THEN** 系统不创建该次攻击的 `DamageRequest`

#### Scenario: 近战保持直接结算

- **WHEN** 攻击者形态为 `Melee` 或没有 `AttackTypeState`，且攻击力大于 0
- **THEN** 系统在消费 `UnitAttackedRequest` 时直接创建 `damageSrc` 为 `Melee` 的 `DamageRequest`，不创建弹道

### Requirement: 闪电链按跳数衰减并在无目标时停止

系统 SHALL 以 `UnitAttackedRequest.target` 为第一跳结算闪电链，并 SHALL 在攻击射程内选择尚未命中的后续敌对目标。每一跳的伤害 MUST 按该跳序号乘以衰减系数。找不到下一跳时，系统 MUST 停止且不得结算空目标。

#### Scenario: 链跳

- **WHEN** 攻击者形态为 `Chain`，攻击力大于 0，且第一跳目标射程内存在另一个尚未命中的敌对单位
- **THEN** 系统按跳序创建多条 `DamageRequest`，后一跳伤害等于前一跳伤害乘以衰减系数

#### Scenario: 无后续目标

- **WHEN** 攻击者形态为 `Chain`，且第一跳目标射程内没有其他可命中的敌对单位
- **THEN** 系统只结算第一跳，不创建目标为空的 `DamageRequest`

### Requirement: 攻击弹道过期后不结算

攻击弹道在收到 `ProjectileExpireRequest` 并进入过期终态后，系统 SHALL 放弃该弹道的攻击结算，且 MUST NOT 创建 `DamageRequest`。过期请求 MUST 挂在弹道 effect 实体上，MUST NOT 携带字段。超距与超时的判定语义以本变更 `design.md`「接口冻结：弹道超距/超时」为准。

#### Scenario: 超距联动

- **WHEN** 一枚带攻击结算 payload 的弹道 effect 被写上 `ProjectileExpireRequest` 并完成过期终态
- **THEN** 系统移除攻击结算 payload，且不创建 `DamageRequest`

#### Scenario: 到达优先于过期

- **WHEN** 同一帧弹道既进入到达阈值又满足超距或超时
- **THEN** 系统只发出到达请求，不发出 `ProjectileExpireRequest`，并按到达路径结算

# runtime-execution Specification

## Purpose
定义默认 ECS 执行链、时间守恒、Buff/Once/装备生命周期，以及属性 Store 归属和技能等级重算的运行时契约。

## Requirements

### Requirement: 默认执行链可用
默认系统注册 SHALL 仅包含已实现的执行能力，并在搜索之前更新空间索引；不通过吞异常掩盖未实现系统。

#### Scenario: 无请求启动
- **WHEN** 初始化默认系统树且没有物品请求
- **THEN** 不因未实现的物品 Native 占位抛异常，空间索引系统处于正确顺序

### Requirement: 调度时间守恒
系统 SHALL 区分调度余量和未交付真实时间，每份经过时间只交付一次；Immediate 每 tick 执行。

#### Scenario: 非整除间隔
- **WHEN** 以 0.01 秒输入更新间隔 0.03125 秒的系统
- **THEN** 已交付时间加未交付时间等于累计输入，误差仅限浮点容差

### Requirement: Buff 时间与续期一致
Buff SHALL 使用实际 delta，补跳不超过其存活区间；续期后过期阶段标记不得覆盖新的正剩余时间。周期 Buff 的存活余数不足一个 tick 间隔时 MUST NOT 产生额外 tick。带周期行为的限时 Buff 在到期后 MUST 先完成这次存活时间的周期观察，然后才允许领域回收。

#### Scenario: 过期待清理时续期
- **WHEN** DurationExpired 已设置但 Buff 尚未清理，此时成功续期
- **THEN** 无效的过期标记清除，Buff 按新持续时间继续存在

#### Scenario: 卡顿补跳
- **WHEN** 存活 Buff 一次经过多个 tick 间隔
- **THEN** 在存活区间内产生对应次数行为，不用常量更新频率替代真实时间

#### Scenario: 不足一个间隔的尾段不补跳
- **WHEN** 周期 Buff 的存活时间不是 tick 间隔的整数倍，且一次更新推进的时间超过其寿命
- **THEN** tick 次数等于存活时间除以 tick 间隔后向下取整
- **AND** 最后不足一个间隔的余数不产生额外 tick

#### Scenario: 尾段观察后才回收
- **WHEN** 带周期行为的限时 Buff 到达寿命并被打上 DurationExpired
- **THEN** 周期消费者先观察这段已封顶的存活时间并完成应有的完整间隔 tick
- **AND** 观察完成前不得删除该 Buff
- **AND** 观察完成后的同一次更新内删除该 Buff

#### Scenario: 无周期行为不延迟回收
- **WHEN** 没有周期 tick 的限时 Buff 到达寿命并被打上 DurationExpired
- **THEN** 领域回收不等待周期观察
- **AND** 同一次更新内删除该 Buff

### Requirement: Once 策略消费唯一
Once 规则 SHALL 在首次命中时立即消耗，实体删除可延迟到安全阶段。

#### Scenario: 同轮多事件
- **WHEN** 同轮存在两个通过条件的事件
- **THEN** 同一个 Once 规则仅执行一次动作

### Requirement: 装备状态往返
同 owner/slot 的装备请求 SHALL 仅在已装备时视为幂等；重新装备恢复状态和属性请求且不重复计数。

#### Scenario: 卸下再装备
- **WHEN** 已装备物品进入同槽背包后再次装备
- **THEN** 恢复 ItemEquippedTag 和属性应用，清除移除请求，槽位数量不增加

### Requirement: 归属与等级重算
属性实体 SHALL 创建在其 owner 所属 Store；技能等级重算 SHALL 更新与创建时相同的数值及施法阶段参数。

#### Scenario: 独立 Store
- **WHEN** 非全局 Store 的单位首次创建属性
- **THEN** 属性及所有链接均属于该 Store

#### Scenario: 施法阶段随等级变化
- **WHEN** 技能升级使 LevelValue 定义的前摇、后摇或引导参数变化
- **THEN** 重算系统写入新等级解析值并保持计算脏标记契约

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

### Requirement: 弹道超距或超时后过期且不结算

系统 SHALL 在弹道飞行中检测最大距离与最大存活时间，并在任一条件成立且本帧未到达时，把过期意图写到该弹道实体上。最大距离小于或等于 0 MUST 表示不启用超距，不得被解释成立刻过期。最大存活时间小于或等于 0，或弹道未携带存活上限，MUST 表示不启用超时。超距与超时 MUST 走过期，MUST NOT 走到达。过期后系统 MUST NOT 结算该弹道的到达效果或伤害。

直线弹道在启用最大距离且累计飞行距离达到该距离时 MUST 过期，MUST NOT 再被当作到达。累计飞行距离 MUST 从弹道首次进入飞行时的位置起算，且 MUST 包含追踪弹道。存活时间 MUST 只在飞行期间累加，首次进入飞行的那一帧 MUST NOT 累加。

同一帧既到达又满足超距或超时时，系统 MUST 只按到达处理。

过期终态 MUST 销毁弹道视觉、标记该效果已过期，并 MUST 发出可被触发器观察的独立过期事实。该事实 MUST 在事件清理边界之前发出，且 MUST NOT 由监听者删除。系统 MUST NOT 因过期移除攻击结算载荷；攻击载荷的丢弃由攻击结算变更负责。

#### Scenario: 超距过期

- **WHEN** 弹道的最大距离大于 0，且自首次进入飞行后的累计飞行距离达到该距离，同时本帧未进入到达阈值
- **THEN** 弹道进入过期终态，视觉被销毁，不产生到达标记、到达效果或伤害请求，并存在一条指向该弹道的独立过期事实

#### Scenario: 直线飞完最大距离不再到达

- **WHEN** 直线弹道启用了大于 0 的最大距离，且飞行距离达到该距离
- **THEN** 弹道过期，不进入到达，不执行其到达效果

#### Scenario: 超时过期

- **WHEN** 弹道携带大于 0 的最大存活时间，飞行中的已存活时间达到该上限，且本帧未到达也未超距
- **THEN** 弹道进入过期终态，不产生到达标记或伤害请求，并存在一条独立过期事实

#### Scenario: 未启用时不因 0 立刻过期

- **WHEN** 弹道的最大距离小于或等于 0，且未携带大于 0 的最大存活时间
- **THEN** 系统不因距离或时间写出过期意图

#### Scenario: 到达优先于过期

- **WHEN** 同一帧弹道进入到达阈值，同时累计距离或存活时间也达到上限
- **THEN** 系统只按到达处理，不写出过期意图，不发出过期事实

#### Scenario: 攻击载荷在过期后仍留待攻击结算丢弃

- **WHEN** 带攻击结算载荷的弹道因超距或超时进入过期终态
- **THEN** 本管线不创建伤害请求，且不移除该攻击结算载荷

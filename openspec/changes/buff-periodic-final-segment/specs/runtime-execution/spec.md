## MODIFIED Requirements

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

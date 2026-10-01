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
Buff SHALL 使用实际 delta，补跳不超过其存活区间；续期后过期阶段标记不得覆盖新的正剩余时间。

#### Scenario: 过期待清理时续期
- **WHEN** DurationExpired 已设置但 Buff 尚未清理，此时成功续期
- **THEN** 无效的过期标记清除，Buff 按新持续时间继续存在

#### Scenario: 卡顿补跳
- **WHEN** 存活 Buff 一次经过多个 tick 间隔
- **THEN** 在存活区间内产生对应次数行为，不用常量更新频率替代真实时间

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

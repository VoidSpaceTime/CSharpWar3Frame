# target-selection Specification

## Purpose
定义基于 ECS 玩家归属、生命周期与显式目标特征的筛选，统一光环归属和贡献回收，并约束空间索引的执行时序。

## Requirements

### Requirement: 统一 ECS 阵营与存活判断
UnitHelper、光环和预设筛选 SHALL 使用同一 ECS 阵营查询；未知归属不默认判敌，规则层不得为填补 TODO 调用原生 API。

#### Scenario: 关系与状态过滤
- **WHEN** 目标具有可解析的玩家归属及生命周期
- **THEN** Self、Ally、Enemy、Neutral 和 Alive/Dead 筛选按相应 ECS 状态决定

### Requirement: 光环归属与贡献一致
光环 SHALL 使用 AuraOwner 作为统一归属，查询、移除、目标差异和贡献回收遵循同一关系。

#### Scenario: 公开入口创建
- **WHEN** 通过 CreateAura 创建影响自身的光环并推进系统
- **THEN** 产生对应 Buff，多轮更新不重复累加同源贡献

#### Scenario: 离开与多来源
- **WHEN** 目标离开一个光环范围或该光环销毁
- **THEN** 只移除该光环拥有的贡献并重算，其他来源不受影响

### Requirement: 空间搜索读取当前索引
空间索引 SHALL 在位置写回后、区域和线形搜索前更新。

#### Scenario: 默认注册的范围查询
- **WHEN** 单位已具有 Position 并在本轮更新位置
- **THEN** 默认执行链中的后续范围查询能够按更新后位置找到该单位

### Requirement: 光环不作用于死亡单位

光环 SHALL NOT 为已死亡的单位创建贡献；光环 owner 死亡时，系统 SHALL 回收该光环的全部贡献，owner 复活后按正常范围判定重新施加。

#### Scenario: owner 死亡

- **WHEN** 光环 owner 死亡并经过一次光环更新
- **THEN** 该光环产生的 Buff 全部被删除

#### Scenario: 目标死亡

- **WHEN** 光环范围内的友方单位死亡并经过一次光环更新
- **THEN** 该单位不再持有该光环的 Buff

### Requirement: 闪电链只跳向存活敌人

闪电链选择后续目标时 SHALL 只选择存活的敌对单位。

#### Scenario: 射程内只有尸体

- **WHEN** 第一跳目标射程内只有已死亡的敌对单位
- **THEN** 系统只结算第一跳

### Requirement: 范围查询可重入

范围与线形查询 SHALL 每次返回独立结果列表；在自定义筛选回调中再次发起查询 MUST NOT 改变外层查询的结果。

#### Scenario: 筛选回调内嵌套查询

- **WHEN** 自定义筛选回调内部再次调用圆形查询
- **THEN** 外层查询返回的目标集合与不嵌套时相同

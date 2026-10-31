## ADDED Requirements

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

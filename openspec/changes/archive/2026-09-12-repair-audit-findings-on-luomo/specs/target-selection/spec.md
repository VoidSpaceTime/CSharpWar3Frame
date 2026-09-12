## ADDED Requirements

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

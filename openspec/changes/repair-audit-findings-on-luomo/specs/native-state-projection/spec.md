## ADDED Requirements

### Requirement: 玩家关系投影一致
玩家关系缓存 SHALL 从最终联盟位派生；切换 Basic/Neutral 后 ECS 判断与 Native PASSIVE 投影一致。

#### Scenario: 中立切换
- **WHEN** 先设置中立再设置敌对，或取消中立
- **THEN** Neutral 位按请求撤销，关系缓存和 Native 应用读取同一最终状态

### Requirement: 累计特效变换投影
Native 特效矩阵 SHALL 表达 ECS 累计变换，重复同步不得重复应用历史角度；重建保持缩放契约。

#### Scenario: 跨 tick 累积旋转
- **WHEN** 两轮分别追加 10 度 Z 旋转
- **THEN** 最终矩阵表达累计 20 度而非 30 度，缩放不丢失

### Requirement: 句柄注销顺序
已实现对象销毁 SHALL 在同一 Native 执行路径中先 HandleRemove，再执行原生 Destroy。

#### Scenario: 特效销毁
- **WHEN** 消费 EffectDestroyRequest
- **THEN** 注销发生在 DestroyEffect 之前，并完成一次 ECS 回收

### Requirement: 未实现 Native 能力边界
未实现 Native 功能 SHALL 保留明确 TODO，不作为默认活跃系统，不以业务层原生调用替代。

#### Scenario: 地面物品能力尚未实现
- **WHEN** 初始化框架
- **THEN** 未实现 ItemCreateNativeSystem 不被自动执行，后续功能提案的请求类型仍可保留

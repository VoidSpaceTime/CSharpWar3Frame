# 设计：Native 同步三模式规则

**提案**：native-sync-policy  
**等级**：architecture

## 1. 目标与非目标

### 目标
- 建立 ECS ↔ War3 Native 的三模式同步契约（Compare-Sync / Dirty-Driven / Request）
- Player 领域从 Request 迁移到 Dirty-Driven，修复"名称/颜色持久状态由 Request 表达"的语义错位
- 消除联盟同步的自相矛盾与默认结盟覆盖风险

### 非目标
- 不实现 ItemCreateNativeSystem 功能（地面物品走特效+UI，另行提案）
- 不改造 UnitNativeSystem 的 Compare-Sync（高频血量/蓝量保持）
- 不引入通用事件总线

## 2. 方案对比

### 模式选型（三种，按数据特征）

| 候选 | 机制 | 适用 | 取舍 |
|---|---|---|---|
| **Compare-Sync** | 每轮对比 ECS 值 vs 快照，同步差异 | 高频连续、多路径写入 | 无需打标，但每轮全量遍历 |
| **Dirty-Driven** | 修改打标，Native 只处理标记实体 | 低频离散、集中修改、可查询 | 需显式打标，但零无效遍历 |
| **Request** | 写入请求，消费即删 | 一次性无状态副作用 | 不保存最终值，不能表达累积状态 |

结论：不统一为单一模式；按字段修改特征选型。对比了"全量 Dirty"（需给血量/蓝量所有写入点打标，成本高且易漏）与"全量 Compare"（特效低频字段每轮遍历浪费），混合策略在正确性与成本间最优。

### 联盟状态表达（两种候选）

| 候选 | 机制 | 取舍 |
|---|---|---|
| **A：位数组组件** | `PlayerAllianceState.bits[target]` 低 5 位表达 5 类联盟 | 紧凑、可增量 dirty；选了它 |
| **B：关系实体 + Link** | 每对玩家一个关系实体 | 语义清晰但实体量大（16×16），且与 Helper 静态缓存重复 |

结论：选 A。16 玩家上限固定，位数组开销最小；`dirty[target]` 增量标记解决首次全量重放问题。

### 联盟位语义（修复冲突）

| 原生位 | Basic 同盟 | Neutral | 最终取值 |
|---|---|---|---|
| ALLIANCE_PASSIVE | true | true | `isNeutral \|\| isBasic`（互斥，不双写覆盖） |
| HELP_REQUEST / RESPONSE / SHARED_SPELLS | true | false | `isBasic` |
| SHARED_VISION / CONTROL / ADVANCED_CONTROL | 独立位 | false | 各自位 |

对比旧实现（Basic 写 PASSIVE 后 Neutral 再写 PASSIVE 覆盖），新设计保证同一原生位单次语义写入。

## 3. 分层职责

```
ECS 组件层（PlayerNative / PlayerAllianceState / PlayerDirty）
    ↑ 写状态 + 打标
Helper 层（PlayerHelper：SetName/SetColor/SetAlliance*）
    ↑ 链式封装
Modifier 层（可选 XxxModifier，返回 this）
    ↑ 消费 Dirty + 调 War3 API
Native 层（PlayerNativeSyncSystem）
```

- Helper 是唯一对外修改入口；Dirty 合并用按位 OR，同步后清除。
- Native 层只做执行映射（位 → 原生联盟类型），不做业务决策。
- `GetPlayer` 返回 `ref PlayerNative` 是已知旁路（镜像数组），当前无调用方直接改字段，记录为遗留风险。

## 4. 迁移与回滚

### 迁移（已完成）
1. AGENTS.md 规则录入
2. Player 组件/Helper/System 改造
3. 全项目审计其余 Native 系统归类

### 回滚
- 恢复 `Player*NativeRequest` 组件 + 旧 PlayerNativeSystem 三系统；PlayerDirty/PlayerAllianceState 保留不影响编译。
- 成本：低，Player 领域独立。

## 5. 风险

| 风险 | 缓解 |
|---|---|
| GetComponent 未初始化抛异常 | SetAllianceBit 用 TryGetComponent + 自动初始化 |
| 首次全量重放抹默认结盟 | dirty[target] 增量同步 |
| PASSIVE 位双写覆盖 | Basic/Neutral 互斥取值 |
| 联盟单向同步 | SetAlliance/SetNeutral 双向写位 + Dirty |
| PlayerNative 镜像数组双真相 | 记录遗留；SetName/SetColor 双写一致 |
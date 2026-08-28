# Spec：Native 同步

## 能力：NativeSync

### 目标
定义 ECS 运行时真相与 War3 原生代理之间的同步契约，按数据特征选择 Compare-Sync / Dirty-Driven / Request 三种模式之一。

### 需求

#### NS-1：三模式决策
- **高频连续、多路径写入**（血量/蓝量）→ Compare-Sync：Native 系统每轮对比 ECS 值与快照，同步有意义的差异；业务写入点无需打标。
- **低频离散、集中修改、状态需查询**（特效外观、玩家名称/颜色/联盟）→ Dirty-Driven：修改入口写 ECS 状态并打标，Native 系统只处理带标实体，同步后清除。
- **一次性无状态副作用**（动画、销毁、创建、移动）→ Request：写入 `XxxRequest`，消费后删除，不保存最终值。

#### NS-2：Dirty 契约
- Dirty 只表示"ECS 状态等待同步到原生"，不是业务状态。
- 同实体同帧多次修改：按位 OR 合并 flags，同步完成后统一清除。
- 有载荷脏标记用 `IComponent`（`EffectDirty`/`PlayerDirty` 带 flags）；无载荷用 `ITag`（`AttrDirty`）。
- 累积型状态（旋转/位移/计数器）必须存状态组件（`EffectTransform`），禁止只用一次性 Request 表达。

#### NS-3：修改入口
- 对外业务调用一律走领域 Helper；Helper 修改持久状态后立即打 Dirty 或写 Request，集中维护字段 ↔ flag 映射。
- 链式 `XxxModifier` 建立在 Helper 之上，返回自身；不得绕过 Helper 直接操作原生 API。
- Native 系统只消费 ECS 状态/请求并调 War3 API，不承担业务决策。

#### NS-4：Player 联盟同步
- 联盟位真相存 `PlayerAllianceState.bits[target]`（低 5 位：Basic/Vision/Control/FullControl/Neutral）。
- `dirty[target]` 标记目标待同步；同步只处理 dirty 目标，同步后清除，禁止全量重放覆盖默认结盟。
- Basic 与 Neutral 互斥：`PASSIVE = isNeutral || isBasic`，同一原生位单次语义写入。
- `SetAlliance`/`SetNeutral` 双向写位 + Dirty（Relations 为双向关系）；Vision/Control/FullControl 单向（A 授予 B）。

#### NS-5：初始化
- 玩家实体创建（`CreatePlayers`）后由 `InitializePlayers` 挂载 `PlayerAllianceState` 与初始关系缓存。
- 名称/颜色初始值从原生读入 ECS（引导方向 Native→ECS），无需打标。

### 验收
- 全仓无 `PlayerNameNativeRequest` / `PlayerColorNativeRequest` / `PlayerAllianceNativeRequest` 残留。
- `PlayerHelper` 所有持久状态写入口均打 Dirty；无直接写 Request 残留。
- `PlayerNativeSyncSystem` 同步后清除 `PlayerDirty` 与 `dirty[target]`。
- `dotnet build War3Frame.csproj` 0 错误。
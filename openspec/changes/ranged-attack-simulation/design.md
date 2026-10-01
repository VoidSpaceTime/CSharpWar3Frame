# 设计：远程与闪电链攻击结算

## 1. 现状（合并后代码）

| 事实 | 位置 |
|---|---|
| 近战创建 `DamageRequest` 后删除 `UnitAttackedRequest` 实体 | `AttackSimulationSystem.cs:35-66` |
| Ranged/Chain 空分支 | `AttackSimulationSystem.cs:56-59` |
| 攻击形态枚举；缺省 Melee | `Units.cs:24-44`；`AttackHelper.cs:13-17` |
| 攻击请求字段只有 attacker/target | `UnitAttackedEvent.cs:11-18` |
| 弹道推进只在到达时收集；`_expireRequests` 每帧清空且无写入 | `AbilityEffectSystems.cs:20-34,68-81` |
| 过期消费分支已写好、无来源 | `AbilityEffectSystems.cs:329-342` |
| `ApplyRequests` 给过期实体加零字段 `ProjectileExpireRequest` | `AbilityEffectSystems.cs:1613-1629` |
| `ProjectileExpireRequest` 无字段 | `AbilityEffect.cs:363-366` |
| `maxDistance` 仅 Linear 轨迹用于「飞完即到达」 | `AbilityEffectSystems.cs:169-182` |
| Tracking 只按到达阈值返回 true，不看 `maxDistance` | `AbilityEffectSystems.cs:144-166` |
| `elapsedTime` 无写入 | `AbilityEffect.cs:325` |
| 伤害效果 → `DamageRequest`；结算 order 125 | `AbilityEffectSystems.cs:754-825,988` |
| 攻击力/射程/间隔属性 ID | `Combat.cs:13-18` |
| 圆形选目标 | `GroupHelper.cs:36-46` |
| 空间网格 order 99 | `SpatialGridSystem.cs:13-14` |

`AttackSimulationSystem` 是 Immediate 124。`DamageResolveSystem` 是 Interval 125。同一次根更新里，124 创建的 `DamageRequest` 能被 125 看到，近战已依赖这一点（`AttackSimulationSystem.cs:15`）。

## 2. 方案

### 2.1 Ranged

`AttackSimulationSystem` 在 `AttackType.Ranged` 分支：

1. attacker 或 target 缺 `Position`：不创建弹道，不创建 `DamageRequest`，仍删除请求（与空引用分支一致，避免悬挂请求）。
2. 读取 `AttributeHelper.GetFinalValue(attacker, AttackDamage)`。`<= 0` 仍创建弹道（飞行是可见语义），但 payload 伤害为 0 时命中系统不发 `DamageRequest`（对齐近战 `damage > 0`）。
3. 读取 `AttackRange` finalValue 作为 `ProjectileData.maxDistance`。`<= 0` 时 maxDistance 记 0，语义见接口冻结：0 表示不启用超距。
4. `Game.Store.CreateEntity` 挂上下表组件后删除请求实体。

| 组件 | 值 |
|---|---|
| `EffectSource` | caster = attacker；ability = default（空） |
| `EffectTargetInfo` | targetUnit = target；targetX/Y 取目标 Position |
| `Position` | 攻击者 Position |
| `ProjectileData` | `trajectoryType = Tracking`；`maxDistance` = AttackRange；`arrivalThreshold` 留 0，由 `NormalizeProjectileDefaults` 回退 30（`AbilityEffectSystems.cs:120-121`）；`speed` 使用常量 900（本变更不新增攻击弹速属性） |
| `ProjectileRuntimeState` | `phase = PendingStart` |
| `EffectPendingTag` | 使 order 100 的 `Filter.AnyTags(EffectPendingTag)` 能扫到（`AbilityEffectSystems.cs:26-28`） |
| `AttackProjectileImpact`（新增） | 见 2.3 |

不走 `EffectChainBuilder.Projectile` / `AbilityEffectHelper.CreateEffectEntity`：二者要求 ability 步骤（`AbilityEffectHelper.cs:285-302`）。攻击没有技能实体。

`ProjectileSystem.UpdateTracking` 会跟随 `targetUnit` 的 Position（`AbilityEffectSystems.cs:149-155`）。目标实体已删除时，现有代码仍用 `EffectTargetInfo` 上最后写入的坐标；本变更不改该函数。

### 2.2 Chain

仍在 `AttackSimulationSystem.OnUpdate` 内完成，不建弹道、不跨帧。

1. 第一跳 = `request.target`。伤害 = `AttackDamage` finalValue。`> 0` 才发 `DamageRequest`。
2. 后续跳：以**当前跳目标**的 Position 为圆心，半径 = 攻击者 `AttackRange` finalValue（`<= 0` 则没有后续跳）。调用 `GroupHelper.FindInCircle(attacker, x, y, radius, TargetFilter.Enemy, maxTargets: 0)`，再排除攻击者自身与已跳集合，取距离最近的一个。
3. 跳数与衰减来自攻击者可选组件 `AttackChainConfig`；缺省 `maxJumps = 4`（含第一跳），`decay = 0.85`。第 n 跳（从 0 计）伤害 = 基础伤害 × decay^n。衰减后 `<= 0` 停止。
4. 每一跳 `DamageRequest.damage.damageSrc = DamageSrc.Ranged`，`damageType = Physical`，source = attacker，target = 该跳目标。
5. 无后续目标：已发出的跳保留，循环结束。这是 `ChainAttackNoTarget` 场景。

`DamageSrc` 不新增枚举值。Chain 与 Ranged 都是攻击伤害，区分靠 `AttackType` 与是否存在弹道，不靠 `DamageSrc`。

选目标不调用 `CreateGroup` / 任何 Jass Group API。`GroupHelper` 只读 `SpatialGrid`。

### 2.3 命中结算系统

新增 `AttackProjectileImpactSystem`：

- `[SystemRegister(SystemKind.Interval, 123)]`
- `QuerySystem<AttackProjectileImpact, ProjectileRuntimeState>`
- 实体同时具 `ProjectileArrived` 时：若 `impact.damage > 0` 且 `impact.target` 非空，创建 `DamageRequest`（数值用 payload 快照，不重读属性）；然后 `RemoveComponent<AttackProjectileImpact>()`。
- 实体具 `EffectExpired`（Todo 18 会打上，见冻结节）时：移除 payload，**不**创建 `DamageRequest`。
- 不删除 effect 实体，不销毁视觉；视觉销毁仍是 `ProjectileLifecycleApplySystem`（到达）与 Todo 18（过期）的职责。

新增组件（`IComponent`，有字段）：

```text
AttackProjectileImpact
  Entity attacker
  Entity target
  float damage          // 创建弹道时的 AttackDamage 快照
  int token             // 等于被消费的 UnitAttackedRequest 所在实体 Id，仅用于对齐与回归断言

AttackChainConfig       // 可选，挂攻击者
  int maxJumps          // 含第一跳，< 1 视为 1
  float decay           // 每跳乘数，缺省 0.85；< 0 视为 0
```

`token` 不是第二套身份系统：请求实体在创建弹道后被删除，token 只是快照下来的实体 id，供回归把「哪次攻击」和「哪枚弹道」对齐。不参与过期请求的匹配（过期请求挂在弹道实体自身，不需要 token）。

### 2.4 顺序

```text
99  SpatialGridSystem          网格
100 ProjectileSystem           推进；到达请求（过期请求的写入方见冻结节，实现属 Todo 18）
102 ProjectileLifecycleApplySystem
    到达 → ProjectileArrived + 视觉销毁
    过期 → Expired + EffectExpired（消费已存在，写入方 Todo 18）
123 AttackProjectileImpactSystem
    ProjectileArrived → DamageRequest
    EffectExpired → 丢弃 payload
124 AttackSimulationSystem     创建弹道 / Chain 跳（Immediate）
125 DamageResolveSystem        唯一扣血
```

123 与 124 的先后不影响正确性：124 创建的弹道同帧还在 `PendingStart`，要等下一帧 100 才可能到达；123 只消费已经 `ProjectileArrived` 的实体。

## 3. 接口冻结：弹道超距/超时

本节是 `ranged-attack-simulation` 与 `projectile-expire-pipeline` 的共享契约。Todo 18 应原样采用，不再改名、改挂载点或改分支语义。若实现时发现做不到，先回到本变更修订本节并重新审核，禁止单方漂移。

### 3.1 最大距离（max range）

- 存储：`ProjectileData.maxDistance`（`AbilityEffect.cs:350`），单位与 `Position` 相同（地图坐标）。
- 含义：弹道自**首次进入 `InFlight` 时的 Position** 起，水平位移累计 `ProjectileRuntimeState.traveled` 达到 `maxDistance` 即超距。Tracking 必须开始累计 `traveled`（当前 Tracking 不累计，这是 Todo 18 的实现缺口，不是契约缺口）。
- `maxDistance <= 0`：**不启用**超距。不得把 0 解释成「立刻过期」。
- 攻击弹道的写入者：`AttackSimulationSystem` 在创建弹道时把攻击者 `AttackRange` finalValue 写入 `maxDistance`。技能弹道的写入者不变：`AbilityEffectHelper` 从步骤规格写入 `maxDistanceValue`（`AbilityEffectHelper.cs:293`），由 `NormalizeProjectileDefaults` 解出 `maxDistance`（`AbilityEffectSystems.cs:104-110`）。
- 超距是过期，不是到达。禁止为了复用到达链把超距写成 `ProjectileArriveRequest`。当前 Linear 在飞完 `maxDistance` 时返回 arrived（`AbilityEffectSystems.cs:174-182`）与本契约冲突；**修正归 Todo 18**：Linear 在 `maxDistance > 0` 且 `traveled` 达到该值时改为过期请求，不再打到达请求。技能若需要「飞到点算命中」，应使用 Tracking / 曲线，或把 `maxDistance` 留 0。

### 3.2 最大存活（max lifetime）

- 存储：`ProjectileRuntimeState.elapsedTime`（`AbilityEffect.cs:325`），秒。仅 `InFlight` 期间按 `Tick.deltaTime` 累加。`PendingStart` 那一帧不累加。
- 上限不放进 `ProjectileData`（该结构是技能步骤镜像，攻击弹道没有对应的 EffectValueSpec 字段）。上限放在新增组件：

```text
ProjectileLifetime ： IComponent
  float maxLifetime   // 秒；<= 0 表示不启用超时
```

- 攻击弹道：`AttackSimulationSystem` 不挂 `ProjectileLifetime`（攻击超距只由射程表达）。超时能力对技能弹道开放；谁挂这个组件由 Todo 18 的配置入口决定，比较对象必须是本字段。
- `elapsedTime >= maxLifetime` 且 `maxLifetime > 0` → 超时过期。
- 同一帧既满足到达阈值又满足超距或超时：**到达优先**。先打 `ProjectileArriveRequest`，不再打过期请求。理由：已经进入到达阈值说明命中语义成立，不应被边界上的寿命抢走。

### 3.3 谁写 `ProjectileExpireRequest`

- 唯一写入点：`ProjectileSystem.OnUpdate` 在实体循环外调用既有 `ProjectileFlowHelper.ApplyRequests(arrive, expire)`（`AbilityEffectSystems.cs:81,1617-1629`）。
- `ProjectileSystem` 只把「应过期」的 effect 实体放进 `_expireRequests`。禁止其他系统、helper、触发器直接 `AddComponent(new ProjectileExpireRequest())`。
- 组件保持零字段（`AbilityEffect.cs:366`）。原因不是 Tag：仓库规则禁止用带 Request 后缀的 Tag 表达意图；零字段 `IComponent` 是合法的一次性意图。超距与超时的区别不编码进请求，需要区分时读 `traveled` 与 `elapsedTime`。
- 写入条件（全部满足）：`ProjectileFlowHelper.ShouldProcess` 为真（`PendingStart` 或 `InFlight`）；本帧未被判定为到达；超距或超时成立。
- 已有 `ProjectileArriveRequest` 或 `ProjectileArrived` 或 `EffectExpired` 的实体不得再写入。

### 3.4 实体与 token

- `ProjectileExpireRequest` 挂在**弹道 effect 实体自身**，不是独立请求实体，也不是攻击者/目标。
- 不携带 token、来源、目标或原因枚举。一次弹道至多一个过期请求；`ApplyRequests` 已用 `HasComponent` 去重（`:1627`）。
- 攻击侧对齐使用 `AttackProjectileImpact.token`（请求实体 id 快照），与过期请求无关。过期时 123 只看「这个 effect 上还有没有 payload」。
- 请求是一次性意图：`ProjectileLifecycleApplySystem` 消费后必须 `RemoveComponent<ProjectileExpireRequest>()`（现有 `:339` 已这样做）。监听者不得删除该组件。

### 3.5 结算分支

消费点保持 `ProjectileLifecycleApplySystem` 的 `toExpire` 循环（`AbilityEffectSystems.cs:329-342`），Todo 18 只允许在这个循环内补全，不得另起一套过期系统：

1. `runtimeState.phase = ProjectileLifecyclePhase.Expired`。
2. 移除 `ProjectileExpireRequest`。
3. `ProjectileFlowHelper.DestroyProjectileVisual`。
4. 打 `EffectExpired`（已有 `:341`）。`EffectExpired` 是 effect 实体上的终态 Tag，不是对外广播。
5. 不打 `ProjectileArrived`，不调用 `CreateArriveEffect`，不创建 `DamageRequest`。
6. 若实体上有 `AttackProjectileImpact`，由 order 123 看到 `EffectExpired` 后移除 payload且不结算。Todo 18 不要移除 `AttackProjectileImpact`，避免两个变更抢组件所有权。
7. 对外若要让触发器听到「弹道未命中而结束」，由 Todo 18 另发独立事件实体（建议名 `ProjectileExpiredEvent`，带 `TriggerEventMarker`，order < 132）。本变更不创建该事件，也不把它算进攻击验收。

### 3.6 本变更 vs Todo 18 的实施切分

| 项 | 本变更实施 | Todo 18 实施 |
|---|---|---|
| 契约文本 | 本节 | 原样引用，不改语义 |
| 攻击弹道写入 `maxDistance` | 是 | 否 |
| `ProjectileLifetime` 类型 | 本变更声明；攻击路径不挂 | 技能配置入口与挂载 |
| `ProjectileSystem` 累计 `traveled` / `elapsedTime` 并填充 `_expireRequests` | 否 | 是 |
| Linear 飞完改为过期而非到达 | 否 | 是 |
| `:329` 消费分支 | 不改 | 按 3.5 补全（分支主体已存在） |
| `AttackProjectileImpact` 在 `EffectExpired` 时丢弃 | 是（123 系统） | 否 |

Todo 18 落地前，攻击弹道的超距**不会**自动过期。这是刻意的顺序：17 先行冻结，18 随后接线。回归 `RangedAttackExpireLink` 只构造一枚已写好 `maxDistance` 与 `AttackProjectileImpact` 的弹道，并直接挂上 `ProjectileExpireRequest`，断言 123 不产生 `DamageRequest` 且 payload 被移除。它不要求 `ProjectileSystem` 已经会自己写这个请求。

## 4. 能力选择

Delta 放入既有 `runtime-execution`，不新建 capability。

- 攻击结算是默认执行链上的一步，和 Buff、装备、等级重算一样属于运行时行为契约。
- 新建 `attack-simulation` 会把「谁创建 DamageRequest」从运行时规格里拆出去，随后归档还要再迁一次。
- 过期管线的行为增量属于 Todo 18；本 delta 只添加攻击分流与「攻击弹道通过既有过期请求放弃结算」。

## 5. 架构影响

- `War3Frame/`：改 `AttackSimulationSystem`；新增 `AttackProjectileImpact`、`AttackChainConfig`、`ProjectileLifetime`、`AttackProjectileImpactSystem`。文件放在现有 `Src/Systems/Unit/` 与 `Src/Components/Unit/`（`ProjectileLifetime` 放 `Src/Components/Ability/AbilityEffect.cs` 旁，因为它描述的是弹道而不是攻击者）。
- `War3Frame.Generator/`：不受影响。新系统只用现有 `[SystemRegister]`，不改 `SystemGenerator` 输出形态。
- `FrameBuild/`、`CSharpWar3Frame/`：不受影响。
- `Projects/`：实施阶段在 `Projects/Regression` 注册四个 `runtime` 组场景。提案阶段不改。

Native 分层：新代码路径的回答是「不能放进 Native 层，因为它不执行原生副作用」。弹道模型若要显示，沿用 effect 实体上已有的视觉字段，由 `EffectNativeSystem` 执行。本变更的攻击弹道 `model` 允许为空，空模型不创建视觉实体。

消息规则：

- `UnitAttackedRequest` 仍是一次性意图，唯一消费方仍是 `AttackSimulationSystem`。
- `DamageRequest` 仍是一次性意图，唯一结算方仍是 `DamageResolveSystem`。
- `ProjectileExpireRequest` 仍是挂在主体上的一次性意图，零字段。
- 不新增 Tag 来表示「请结算攻击」或「请过期」。
- 不新增 `XxxEvent`。命中事实仍由既有 `DamageEvent` 表达。

## 6. 备选（不采用）

| 方案 | 不采用原因 |
|---|---|
| Ranged 命中帧直接发 `DamageRequest`，不建弹道 | 与 `Units.cs:29`「命中后再结算」相反，也没有超距可联动。 |
| 伪造一把隐藏 ability 再走 `EffectChainBuilder` | 把攻击耦合进技能模板注册，回滚面更大。 |
| Chain 每跳一枚弹道 | 超出 Todo 17「链跳目标选择/衰减」；视觉与超距会和 Ranged 缠在一起。 |
| 给 `ProjectileExpireRequest` 加 reason/token 字段 | 破坏「零字段意图」；Todo 18 的预留签名（`ApplyRequests` 只收实体列表）也装不下字段。 |
| 本变更直接改 `ProjectileSystem` 写过期请求 | 与 Todo 18 抢唯一写入点。 |

## 7. 风险与回滚

见 `proposal.md` 第 7、8 节。实施期额外约束：123 的查询里只移除 `AttackProjectileImpact`，不在 `ForEachEntity` 内增删其他组件或 Tag；若必须改 Tag，先收集再在循环外改，避免 `StructuralChangeException`。

# 设计：弹道超距/超时过期管线

## Context

见 `proposal.md` Why。当前代码（合并后行号）：

- `ProjectileSystem`（`AbilityEffectSystems.cs:17-82`）只在 `arrived == true` 时写入 `_arriveRequests`。`_expireRequests` 每帧清空、从不填充，循环外调用 `ProjectileFlowHelper.ApplyRequests`（`:81,1617-1629`）。
- `UpdateLinear`（`:169-182`）把 `maxDistance > 0` 且 `traveled` 达到该值当成到达并返回 `true`。`maxDistance <= 0` 时用 `float.MaxValue`，等价于不启用，这一点与冻结契约一致，但「达到即到达」与契约冲突。
- `UpdateTracking`（`:144-166`）不读写 `traveled`。曲线（`:185-274`）用 `normalizedProgress` 到达，也不累计 `traveled`。
- `elapsedTime`（`AbilityEffect.cs:325`）无写入。`PendingStart` 在同一 `OnUpdate` 内被改成 `InFlight` 后再走轨迹更新（`:47-61`），因此「首次进入 InFlight 的那一帧」已经发生位移。冻结节要求 `PendingStart` 那一帧不累加 `elapsedTime`；`traveled` 从首次 InFlight 的 Position 起算，与「该帧位移计入 traveled」不冲突。
- `ProjectileLifecycleApplySystem`（`:282-343`）已有 `toExpire`：写 `Expired`、移除请求、销毁视觉、打 `EffectExpired`。未跳过同帧已到达的实体，未发对外事件。
- 技能弹道由 `EffectChainBuilder.Projectile`（`EffectChainBuilder.cs:268-305`）写入 `ProjectileEffectStepSpec.maxDistance`（`EffectSpec.cs:312`），`AbilityEffectHelper`（`:293`）抄到 `maxDistanceValue`。没有寿命字段。
- 事件范式：独立实体 + `TriggerEventMarker.eventTypeId = EventTypeRegistry.Get<T>()`（`AbilityEffectSystems.cs:1052-1063`）。`EventCleanupSystem` 为 Interval 132（`TriggerSystems.cs`）。

`ProjectileLifetime` 类型在本工作区尚未出现（`AbilityEffect.cs` 无此结构）。冻结节把类型声明划给 Todo 17、挂载划给本变更。实施时若 17 已合并该类型则复用，否则由本变更新增。攻击路径仍然不挂。

## Goals / Non-Goals

**Goals:**

- 原样采用冻结节 3.1–3.5，不改名、不改挂载点、不改分支语义。
- 把 Linear「飞完 maxDistance = 到达」改成「飞完 = 过期」。
- 技能步骤可选配置 `maxLifetime`，并定义 `ProjectileExpiredEvent` 的字段。

**Non-Goals:**

- 见 `proposal.md` 第 3 节。本设计不决定攻击弹速（Todo 17 开放问题「常量 900」）。速度仍走现有 `NormalizeProjectileDefaults`（`AbilityEffectSystems.cs:87-94`）。

## 接口冻结采用（verbatim）

来源：`openspec/changes/ranged-attack-simulation/design.md` 第 3 节。下列为采用结果，不是改写。发现做不到时先回该变更修订冻结节并重新审核，禁止本变更单方漂移。

### 3.1 最大距离

- 存储：`ProjectileData.maxDistance`（`AbilityEffect.cs:350`）。
- 自首次进入 `InFlight` 时的 Position 起，水平位移累计到 `ProjectileRuntimeState.traveled`。Tracking 必须开始累计（当前 `:144-166` 不累计，本变更补上）。
- `maxDistance <= 0` 不启用，不得解释成立刻过期。
- 攻击弹道的 `maxDistance` 写入者仍是 `AttackSimulationSystem`（Todo 17）。技能弹道写入者不变：`AbilityEffectHelper.cs:293` → `NormalizeProjectileDefaults`（`:104-110`）。
- 超距是过期，不是到达。禁止写成 `ProjectileArriveRequest`。

**Linear 到达改过期（Todo 17 开放问题 4，本变更承接）：**

`UpdateLinear`（`:172-182`）今天在 `remaining <= 0` 或 `traveled >= maxDistance` 时返回 `true`，调用方（`:68-72`）一律打到达请求。实施时：

- `maxDistance <= 0`：保持不封顶（现有 `float.MaxValue`），函数不因距离返回到达。
- `maxDistance > 0` 且本帧位移使 `traveled >= maxDistance`：函数不得再向调用方表示「到达」。超距实体进入 `_expireRequests`，不进入 `_arriveRequests`，`phase` 不得写成 `ArriveRequested`。
- 位移仍截断在 `maxDistance` 内（现有 `MathF.Min(speed * dt, remaining)` 保留），避免视觉越过射程后再过期。

曲线轨迹的「进度到 1 = 到达」不变。曲线若同时 `maxDistance > 0`，本帧水平位移累加后达到上限，则按冻结节改走过期，且到达优先只在「本帧轨迹判定已到达」时生效。

### 3.2 最大存活

- `elapsedTime` 只在 `InFlight` 期间加 `Tick.deltaTime`。`PendingStart` 转 `InFlight` 的那一帧不加。
- 上限不放进 `ProjectileData`。组件：

```text
ProjectileLifetime : IComponent
  float maxLifetime   // 秒；<= 0 表示不启用超时
```

- 攻击弹道不挂该组件。`elapsedTime >= maxLifetime` 且 `maxLifetime > 0` → 超时过期。
- 同一帧既满足到达阈值又满足超距或超时：只打 `ProjectileArriveRequest`。

**相对冻结节的增量（配置入口，冻结节 3.2 明确留给本变更）：**

- `ProjectileEffectStepSpec` 增加 `EffectValueSpec maxLifetime`。
- `EffectChainBuilder.Projectile` 的两个重载各增加一个尾部可选参数 `maxLifetime`，默认 `default`。现有调用点不传则行为与今天相同（不启用超时）。
- `AbilityEffectHelper` 在创建弹道组件时：解析值 `> 0` 才 `AddComponent(new ProjectileLifetime { maxLifetime = 解析值 })`。解析失败或 `<= 0` 不挂。比较对象必须是该字段，不得另存一份到 `ProjectileData`。

这是冻结节授权的入口，不是契约漂移。

### 3.3 谁写请求

- 唯一写入点：`ProjectileSystem.OnUpdate` 循环外的 `ApplyRequests(_arriveRequests, _expireRequests)`（`:81`）。
- 其他系统、helper、触发器禁止 `AddComponent(new ProjectileExpireRequest())`。
- 组件保持零字段（`AbilityEffect.cs:366`）。超距与超时不编码进请求；需要区分时读 `traveled` 与 `elapsedTime`。
- 写入条件：`ShouldProcess` 为真；本帧未被判定为到达；超距或超时成立。
- 已有 `ProjectileArriveRequest` 或 `ProjectileArrived` 或 `EffectExpired` 的实体不得再写入。`ApplyRequests` 已有 `HasComponent` 去重（`:1627`）。

判定放在 `ProjectileSystem` 的轨迹 switch 之后、列表收集处，不放进各 `Update*` 的返回值里再解释成到达。`Update*` 继续只回答「本帧是否到达」；超距/超时由调用方在 `arrived == false` 时计算。

### 3.4 实体与 token

- 请求挂在弹道 effect 实体自身。无 token、无来源、无原因枚举。
- 攻击对齐仍用 `AttackProjectileImpact.token`，与过期请求无关。
- 消费后必须 `RemoveComponent<ProjectileExpireRequest>()`（现有 `:339`）。

### 3.5 结算分支

只允许在 `toExpire` 循环内补全（`:329-342`），不另起过期系统：

1. `phase = Expired`。
2. 移除 `ProjectileExpireRequest`。
3. `DestroyProjectileVisual`。
4. 打 `EffectExpired`。它是终态 Tag，不是广播。
5. 不打 `ProjectileArrived`，不调用 `CreateArriveEffect`，不创建 `DamageRequest`。
6. 不移除 `AttackProjectileImpact`。order 123 看到 `EffectExpired` 后丢弃 payload（Todo 17）。
7. 另发 `ProjectileExpiredEvent`（见下节）。

同帧防御：若实体已有 `ProjectileArrived` 或本帧已进入 `toArrive`，`toExpire` 跳过。正常路径不应发生，因为 3.3 禁止双写；这条只防止外部直接挂请求。

`ExpireRequested` 不写入。冻结节 3.5 的终态是 `Expired`，枚举值保留勿清理。

### 3.6 切分

本变更实施冻结节表格中「Todo 18 实施」列的全部项，不实施「本变更实施」列（攻击 `maxDistance`、123 丢弃 payload）。

## Decisions

### 1. 对外事件用独立实体

`EffectExpired` 留在 effect 上。触发器要听「未命中而结束」时，`ProjectileLifecycleApplySystem` 在 `toExpire` 循环内创建独立实体：

```text
ProjectileExpiredEvent : IComponent
  Entity projectile
  Entity source
  Entity target
```

并 `AddComponent(new TriggerEventMarker { eventTypeId = EventTypeRegistry.Get<ProjectileExpiredEvent>() })`。创建点 order 102，小于 `EventCleanupSystem` 132。监听者不得删除该实体。

字段取自该 effect 上已有的 `EffectSource.caster` 与 `EffectTargetInfo.targetUnit`（点目标时 target 为空实体）。不记录超距/超时原因：冻结节禁止把原因放进请求，事件若再引入原因枚举就会变成第二套契约。区分方式仍是读 effect 上的 `traveled` / `elapsedTime`（事件发出时 effect 尚未删除）。

备选：给 `ProjectileExpireRequest` 加 reason。不采用，冻结节第 6 节已拒绝。

备选：不发事件，只留 `EffectExpired`。不采用。冻结节 3.5 第 7 点把「对外可听」划给本变更，且仓库规则禁止用 Tag 做多监听者广播。

### 2. traveled 的累计口径

水平距离：`hypot(dx, dy)`，不含 z。Tracking 在本帧位移后把本次 `move` 加进 `traveled`。曲线在写回 Position 后，用本帧水平位移长度累加。Linear 已有 `traveled += move`（`:180`），保持。

起点是首次 `InFlight` 之前的 Position，因此第一帧飞行距离计入 `traveled`，但不计入 `elapsedTime`。

### 3. 不新增系统、不改 order

写入仍在 order 100，消费仍在 order 102，事件清理仍在 132。123 与 125 不在本变更修改。

### 4. 结构变更

`ForEachEntity` 内只改已有 `ProjectileRuntimeState` / `Position` 字段与本地列表。`AddComponent` / `AddTag` / `RemoveComponent` 保持在循环外（现有 `positionUpdates` 与 `ApplyRequests` / `toExpire`）。

## Risks / Trade-offs

见 `proposal.md` 第 5 节。实施期额外约束：

- [Linear 行为变化] → delta 用 ADDED 写明「飞完最大距离是过期」；不改主 spec 里不存在的旧 requirement，因此不用 MODIFIED。
- [17 尚未合并 `ProjectileLifetime`] → 本变更 tasks 包含「若类型不存在则新增，存在则复用」。
- [事件与 effect 同帧删除] → 本变更不删除 effect 实体，只打 Tag 并销毁视觉；事件上的 `projectile` 在清理前仍可解引用。

## Migration Plan

- 批准前：无代码。回滚是删除本 change 目录。
- 批准后：先补类型与配置字段，再改 `ProjectileSystem` 判定，再补 `toExpire` 与事件，最后回归。任一步失败不归档。
- 已有技能若给 Linear 配了 `maxDistance` 并依赖 `arriveEffect`：迁移为 Tracking，或把 `maxDistance` 设为 0。仓库内 `Projects/test/Scripts/Template/Ability.cs` 的 `Projectile(` 调用不传 `maxDistance`（默认 0），不受该破坏性变更影响；实施时仍要再搜一次调用点。

## Open Questions

1. 曲线弹道同时配置到达进度与 `maxDistance` 时，水平 `traveled` 可能在进度到 1 之前越界。本设计按冻结节处理（越界且本帧未到达 → 过期；本帧已到达 → 到达优先）。不需要新的配置开关。
2. `ProjectileExpiredEvent` 不区分超距与超时。若后续触发器要分支，另开变更给事件加原因字段；本变更不加，避免与「请求零字段、原因不进请求」分叉。
3. 弹速常量 900 不在本变更解决。攻击弹道速度仍由 Todo 17 决定；技能弹道速度仍由 `ProjectileSpeed` stat 与 `speedValue` 决定。

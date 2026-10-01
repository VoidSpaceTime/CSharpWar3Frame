# 提案：弹道超距/超时过期管线（projectile-expire-pipeline）

## 0. 基本信息

- Change ID: `projectile-expire-pipeline`
- 提案等级: `full`
- 状态: `待审核`
- 日期: 2026-10-01
- 目标一句话: 让 `ProjectileSystem` 成为 `ProjectileExpireRequest` 的唯一写入方（超距 / 超时），并在既有 `toExpire` 分支补全过期终态与对外事件；契约原样采用 `ranged-attack-simulation` 的接口冻结。
- 请求来源: `.omo/plans/war3frame-unlanded-features.md` Todo 18（W4.2）。
- 默认实施后审查强度: `R2 Targeted`
- 命中的审查升级触发器: 核心战斗流程；改既有弹道到达语义（Linear 飞完 `maxDistance` 从到达改为过期）；与并行变更共享弹道过期契约。
- 最终实施后审查强度: `R2 Targeted`（技术准确性 + 跨系统契约；不升 `R3`：不改 Native 分层、不改生成器、不改构建链）
- Oracle 可用性与 `R1` 回退方式: 实施后技术准确性复核优先 Oracle；不可用时由代码审查核对写入点唯一性、到达优先与回归场景，并记录替代原因。
- 完整 `review-work` 授权来源: 无

### 0.1 工件矩阵

- 本 change 为 `full`：`proposal.md`、`design.md`、`tasks.md`、`specs/runtime-execution/spec.md`

### 0.2 总结深度矩阵

- 实施完成后写完整 `summary.md`，覆盖改动范围、全局影响、验证、风险与后续。

## Why

`ProjectileExpireRequest`、`ExpireRequested` / `Expired` 与 `ProjectileLifecycleApplySystem` 的 `toExpire` 循环已经存在，但注释写明当前无写入方（`AbilityEffect.cs:302-304,363-366`；`AbilityEffectSystems.cs:22-24,329-330,1615`）。`ProjectileSystem` 每帧清空 `_expireRequests` 后从不填充（`:33-34,81`）。结果是：配置了 `maxDistance` 的 Linear 弹道飞完距离会被当成到达（`:169-182`），Tracking 完全不看 `maxDistance`（`:144-166`），`elapsedTime` 有字段无累加（`AbilityEffect.cs:325`）。并行变更 `ranged-attack-simulation` 已把超距/超时契约冻结，并明确写入与 Linear 语义修正归本变更。不接线则攻击弹道的射程无法变成「飞出射程即放弃结算」。

## What Changes

- `ProjectileSystem` 在实体循环外、既有 `ApplyRequests` 调用点，把应过期的 effect 实体放入 `_expireRequests`。唯一写入点不变。
- 超距：`ProjectileData.maxDistance > 0` 且 `ProjectileRuntimeState.traveled` 达到该值。`<= 0` 不启用。Tracking 开始累计水平 `traveled`。
- 超时：新增 `ProjectileLifetime.maxLifetime`（秒，`<= 0` 不启用），与仅在 `InFlight` 累加的 `elapsedTime` 比较。攻击弹道不挂该组件。
- **BREAKING（技能 Linear 弹道）**：`maxDistance > 0` 且飞完该距离时改为过期请求，不再打 `ProjectileArriveRequest`，因此不跑 `arriveEffect`。需要「飞到点算命中」的技能改用 Tracking / 曲线，或把 `maxDistance` 留 0。
- 同一帧到达优先于超距或超时。
- `toExpire` 循环补全：`Expired`、移除请求、销毁视觉、`EffectExpired`；不打 `ProjectileArrived`，不创建到达效果，不创建 `DamageRequest`。不移除 `AttackProjectileImpact`。
- 另发独立事件实体 `ProjectileExpiredEvent`（带 `TriggerEventMarker`，order < 132）。
- 技能配置入口增加可选 `maxLifetime`，由效果创建路径挂上 `ProjectileLifetime`。
- 不清理标注「勿清理」的预留代码；不改 `AttackProjectileImpactSystem`。

## Capabilities

### New Capabilities

- 无。过期管线是既有弹道执行链上的一步，不单独立能力。

### Modified Capabilities

- `runtime-execution`: 增加「弹道超距或超时后过期且不结算」：唯一写入方、到达优先、Linear 飞完改为过期、过期终态与对外事件。主 spec 当前无弹道 requirement（`openspec/specs/runtime-execution/spec.md`）。

## Impact

| 区域 | 是否受影响 | 理由 |
|---|---|---|
| `War3Frame/` | 是 | `ProjectileSystem` 写入过期请求并累计 `traveled` / `elapsedTime`；`ProjectileLifecycleApplySystem` 的 `toExpire` 补全；新增 `ProjectileLifetime`、`ProjectileExpiredEvent`；`EffectChainBuilder` / `ProjectileEffectStepSpec` / `AbilityEffectHelper` 增加可选寿命入口。 |
| `War3Frame.Generator/` | 否 | 不新增系统，不改 `SystemRegisterAttribute` 与生成输出。 |
| `FrameBuild/` | 否 | 不改构建、模板、WE、发布。 |
| `CSharpWar3Frame/` | 否 | 不改 CLI。 |
| `Projects/` | 是（实施阶段） | `Projects/Regression` 的 `runtime` 组增加超距与超时两个具名场景。本提案阶段不改代码。 |

Native 分层：本变更只写 ECS 请求、阶段与事件。视觉销毁继续走既有 `ProjectileFlowHelper.DestroyProjectileVisual` → `EffectHelper`。不新增 `JassApi` / `KKApi` / `YDApi` / `DzApi` 调用。

## 2. 目标

1. `ProjectileSystem` 是 `ProjectileExpireRequest` 的唯一写入方，经既有 `ProjectileFlowHelper.ApplyRequests`。
2. 超距与超时语义、到达优先、零字段请求、挂载点与 `toExpire` 消费顺序，原样采用 `openspec/changes/ranged-attack-simulation/design.md`「接口冻结：弹道超距/超时」。本变更不改名、不改挂载点、不改分支语义。
3. Linear 在 `maxDistance > 0` 且 `traveled` 达到该值时发过期请求，不再发到达请求。
4. 过期后对外可监听：独立 `ProjectileExpiredEvent`，不把 `EffectExpired` 当广播。
5. 技能步骤可配置 `maxLifetime`；未配置或 `<= 0` 时不挂或视为不启用。攻击路径仍不挂 `ProjectileLifetime`（类型若已由 Todo 17 声明，本变更不重复声明、不在攻击路径挂载）。

## 3. 非目标

- 不实现攻击弹道创建、`AttackProjectileImpact`、`AttackProjectileImpactSystem`（Todo 17）。过期时不移除该 payload。
- 不改 `DamageResolveSystem`、不新增 `DamageSrc`。
- 不清理 `AbilityEffectSystems.cs:22-24,329-330,1615` 与 `AbilityEffect.cs:302-304,363-366` 的预留注释与空列表。
- 不把 `ExpireRequested` 变成运行时必经阶段。冻结节 3.5 的终态是 `Expired`；`ExpireRequested` 保持预留，勿清理。
- 不改弹道速度默认值。Todo 17 的开放问题「弹速常量 900」属于攻击弹道创建，本变更不引入该常量。
- 不改 Source Generator、FrameBuild、CLI、`TriggerSystem`(131) / `EventCleanupSystem`(132) 的实现。
- 不在本阶段改任何 `.cs`。

## 4. 验收标准

提案阶段：

1. `openspec validate projectile-expire-pipeline` 通过。
2. `design.md` 逐条引用冻结节 3.1–3.6，并写明本变更相对该节的增量只在「技能寿命配置入口」与「`ProjectileExpiredEvent` 字段」，无契约矛盾。

批准后的实施阶段（写入 `tasks.md`，本阶段不执行）：

1. 具名回归：`runtime/ProjectileExpireOutOfRange`、`runtime/ProjectileExpireTimeout`。
2. 到达优先、`maxDistance <= 0` 不立刻过期、过期不产生 `DamageRequest` / `arriveEffect`。
3. `dotnet build War3Frame/War3Frame.csproj` 与 `dotnet build Projects/Regression/Regression.csproj` 0 错误。
4. 新增代码路径无 War3 原生 API 调用。

## 5. 风险

| 风险 | 缓解 |
|---|---|
| Linear 飞完从到达改为过期，现有「用 maxDistance 当射程命中」的技能会丢 `arriveEffect` | 在 design 与 delta 写成显式行为变更；命中语义改走 Tracking / 曲线或 `maxDistance = 0`。 |
| 与 Todo 17 抢 `ProjectileSystem` 写入点或 `AttackProjectileImpact` 所有权 | 本变更只填 `_expireRequests` 与 `toExpire`；不创建、不移除攻击 payload。类型 `ProjectileLifetime` 若 17 已加则复用。 |
| 同帧到达与过期双请求 | 写入条件要求「本帧未被判定为到达」；消费侧若两者同时存在，到达循环优先且过期循环跳过已 `ProjectileArrived` 的实体。 |
| Query 内结构变更 | 请求与 Tag 仍在循环外经 `ApplyRequests` / `toExpire` 写入，不在 `ForEachEntity` 内增删组件。 |
| 事件实体泄漏 | `ProjectileExpiredEvent` 必须挂 `TriggerEventMarker`，由 order 132 清理。 |

## 6. 回滚

- 未批准：删除 `openspec/changes/projectile-expire-pipeline/`。
- 已实施：还原 `ProjectileSystem` 的过期判定与 Linear 返回值语义，还原 `toExpire` 循环到当前形态，删除 `ProjectileExpiredEvent` 与技能 `maxLifetime` 入口。不删除 `ProjectileExpireRequest` 类型（本变更之前已存在）。若 `ProjectileLifetime` 由本变更新增，一并删除；若由 Todo 17 新增，只停止技能路径挂载。

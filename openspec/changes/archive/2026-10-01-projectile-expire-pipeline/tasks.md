# 任务：projectile-expire-pipeline

状态：待审核。批准前不实现。

## 1. 提案

- [x] 1.1 用合并后代码核对 `ProjectileSystem`、`UpdateLinear`、`toExpire` 与冻结节，并确认 `ProjectileLifetime` 当前不存在
- [x] 1.2 写 `proposal.md`、`design.md`（原样采用「接口冻结：弹道超距/超时」）、`tasks.md`、`specs/runtime-execution/spec.md`
- [x] 1.3 `openspec validate projectile-expire-pipeline` 通过
- [x] 1.4 用户审核批准（批准门；未批准不得勾选，不得开始第 2 节）。依据 `.omo/evidence/gates/user-ruling-2026-10-01.md` 第 5 项：W4 含 18 弹道超距超时，批准实施

## 2. 实施（批准后）

- [x] 2.1 若 `ProjectileLifetime` 尚不存在则在 `AbilityEffect.cs` 新增该组件（`maxLifetime <= 0` 表示不启用）；已存在则复用。新增 `ProjectileExpiredEvent`（`projectile` / `source` / `target`）。验证：`dotnet build War3Frame/War3Frame.csproj` 0 错误，且攻击创建路径不挂该组件
- [x] 2.2 `ProjectileEffectStepSpec` 与 `EffectChainBuilder.Projectile` 两个重载增加尾部可选 `maxLifetime`；`AbilityEffectHelper` 仅在解析值 `> 0` 时挂 `ProjectileLifetime`。验证：不传该参数的现有调用仍编译，默认不挂组件
- [x] 2.3 `ProjectileSystem`：Tracking 与曲线累计水平 `traveled`；仅 `InFlight` 累加 `elapsedTime`（`PendingStart` 转飞行的当帧不加）；`arrived == false` 且超距或超时时把实体放入 `_expireRequests`，仍只经 `ApplyRequests` 写入。验证：仓库内除 `ApplyRequests` 外无新的 `ProjectileExpireRequest` 构造
- [x] 2.4 `UpdateLinear`：`maxDistance > 0` 且 `traveled` 达到该值时不再向调用方表示到达，改由 2.3 写入过期；`maxDistance <= 0` 仍不封顶。验证：直线飞完距离的实体没有 `ProjectileArriveRequest`
- [x] 2.5 `toExpire` 按冻结节 3.5 补全：`Expired`、移除请求、销毁视觉、`EffectExpired`、创建带 `TriggerEventMarker` 的 `ProjectileExpiredEvent`；不打 `ProjectileArrived`、不调用 `CreateArriveEffect`、不创建 `DamageRequest`、不移除 `AttackProjectileImpact`；已到达实体跳过。不改预留注释与 `ExpireRequested`。验证：过期实体上请求已移除，且 `AttackProjectileImpact` 仍在

## 3. 验证（批准后，与实施同一闭环）

具名场景注册在 `Projects/Regression` 的 `runtime` 组：

- [x] 3.1 `runtime/ProjectileExpireOutOfRange`：`maxDistance > 0` 的直线弹道飞完距离后为 `Expired` + `EffectExpired`，无 `ProjectileArrived`、无到达效果、无 `DamageRequest`，有且仅有一条 `ProjectileExpiredEvent`
- [x] 3.2 `runtime/ProjectileExpireTimeout`：挂 `ProjectileLifetime.maxLifetime > 0` 且未超距的弹道，推进到 `elapsedTime` 达到上限后过期，无 `DamageRequest`，有一条 `ProjectileExpiredEvent`
- [x] 3.3 到达优先与未启用：同一帧进入到达阈值时只有到达请求；`maxDistance <= 0` 且无寿命组件的弹道不因 0 过期
- [x] 3.4 `dotnet build War3Frame/War3Frame.csproj` 与 `dotnet build Projects/Regression/Regression.csproj` 0 错误；`dotnet run --project Projects/Regression -- runtime` 上列场景 PASS
- [x] 3.5 新增代码路径无 `JassApi` / `KKApi` / `YDApi` / `DzApi` 调用

## 4. 收口（批准并实施后）

- [x] 4.1 写完整 `summary.md`（R2：范围、Linear 语义变化、与 Todo 17 的契约切分、验证、剩余风险）
- [x] 4.2 `openspec archive projectile-expire-pipeline --yes`

## 明确不做

- 不实现 `AttackSimulationSystem` / `AttackProjectileImpactSystem`，不在过期时移除 `AttackProjectileImpact`（Todo 17）
- 不引入攻击弹速常量 900
- 不清理标注「勿清理」的预留代码，不写入 `ExpireRequested`
- 不给 `ProjectileExpireRequest` 或 `ProjectileExpiredEvent` 增加原因字段
- 不提交实现，直到 1.4 被用户明确批准

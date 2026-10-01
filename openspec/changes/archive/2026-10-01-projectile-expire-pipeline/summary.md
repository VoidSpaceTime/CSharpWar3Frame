# 总结：projectile-expire-pipeline

## 范围

批准依据：`.omo/evidence/gates/user-ruling-2026-10-01.md` 第 5 项（W4，含 18 弹道超距超时，批准实施）。本总结不自证批准。

`ProjectileSystem`（order 100）成为 `ProjectileExpireRequest` 的唯一生产写入方。轨迹更新之后、`arrived == false` 时，超距（`ProjectileData.maxDistance > 0` 且 `traveled` 达到该值）或超时（`ProjectileLifetime.maxLifetime > 0` 且 `elapsedTime` 达到该值）把 effect 放入既有 `_expireRequests`，仍只经 `ProjectileFlowHelper.ApplyRequests` 打上零字段请求。同一帧到达只写 `ProjectileArriveRequest`。

`traveled` 从首次进入 `InFlight` 的位置起算，含当帧水平位移；Tracking 与曲线补上累计，Linear 保持原有累加。`elapsedTime` 只在已经处于 `InFlight` 的帧累加，`PendingStart` 转飞行的当帧不加。`UpdateLinear` 在 `maxDistance > 0` 时把位移截断在射程内并恒返回未到达，飞完改由过期分支处理；`maxDistance <= 0` 仍不封顶。

`ProjectileLifecycleApplySystem`（order 102）的 `toExpire` 补全为：`Expired`、移除请求、销毁视觉、`EffectExpired`，并创建带 `TriggerEventMarker` 的独立 `ProjectileExpiredEvent`（`projectile` / `source` / `target`，不区分原因）。不打 `ProjectileArrived`，不调用 `CreateArriveEffect`，不创建 `DamageRequest`，不移除 `AttackProjectileImpact`。同帧已进入到达列表或已有 `ProjectileArrived` 的实体跳过。预留注释与 `ExpireRequested` 未清理、未写入。

`ProjectileLifetime` 由 Todo 17 已落地，本变更复用，攻击创建路径仍不挂。技能入口在 `ProjectileEffectStepSpec` 与 `EffectChainBuilder.Projectile` 两个重载增加尾部可选 `maxLifetime`；`AbilityEffectHelper` 仅在解析值 `> 0` 时挂组件。

## 全局影响

| 区域 | 是否受影响 | 理由 |
|---|---|---|
| `War3Frame/` | 是 | 弹道推进、过期消费、事件类型、技能步骤与寿命挂载。 |
| `War3Frame.Generator/` | 否 | 不新增系统，不改 `SystemRegisterAttribute` 与生成输出。 |
| `FrameBuild/` | 否 | 不改构建、模板、WE、发布。 |
| `CSharpWar3Frame/` | 否 | 不改 CLI。 |
| `Projects/` | 是 | `Projects/Regression` 增加四个 `runtime` 场景。`Projects/test` 现有 `Projectile(` 调用不传 `maxLifetime`，默认不挂寿命。 |

新增路径无 `JassApi` / `KKApi` / `YDApi` / `DzApi`。视觉销毁仍走 `DestroyProjectileVisual` → `EffectHelper`。

## 验证

- `dotnet build War3Frame/War3Frame.csproj -c Release`：0 错误（既有警告，见 `.omo/evidence/task-18-build.txt`）。
- `dotnet run --project Projects/Regression -c Release`：Executed 63; passed 63; failed 0（`.omo/evidence/task-18-regression.txt`）。含既有 `runtime/RangedAttackHit`、`runtime/RangedAttackExpireLink`、`runtime/MeleeAttackDirect`，以及新增 `runtime/ProjectileExpireOutOfRange`、`runtime/ProjectileExpireTimeout`、`runtime/ProjectileExpireArrivalWins`、`runtime/ProjectileExpireDisabled`。
- 生产代码中 `new ProjectileExpireRequest()` 仍只出现在 `ApplyRequests`。回归里 `RangedAttackExpireLink` 直接挂请求，是既有夹具，不是第二写入方。

## R2 专项视角

### 过期 vs 到达优先级

写入侧先看轨迹返回值：到达则只进 `_arriveRequests`，过期判定在 `else`。消费侧 `toExpire` 再跳过本帧 `toArrive` 或已有 `ProjectileArrived` 的实体。`runtime/ProjectileExpireArrivalWins` 用「距离 40、阈值 50、maxDistance 40、寿命 0.01」在启动后的飞行帧同时满足到达与超距/超时，结果只有到达、没有过期事实。

### 写入点唯一性

收集只发生在 `ProjectileSystem.OnUpdate` 循环内的列表；组件只在循环外的 `ApplyRequests` 写入，且已有 `HasComponent` 去重。`ShouldExpire` 拒绝已有到达请求、`ProjectileArrived` 或 `EffectExpired` 的实体。攻击弹道、技能弹道、曲线弹道共用这一处，没有 helper 或触发器直接构造请求。

### 事件生命周期 order < 132

`ProjectileExpiredEvent` 在 order 102 的 `toExpire` 循环外创建，并挂 `TriggerEventMarker`，`eventTypeId = EventTypeRegistry.Get<ProjectileExpiredEvent>()`。该类型已加入 `RegisterBuiltIn`，因此 id 非 0。`EventCleanupSystem` 仍为 132，监听者不删除事件实体。`EffectExpired` 留在 effect 上，不作广播。

### 对 Todo 17 已落地行为的兼容

攻击弹道仍不挂 `ProjectileLifetime`，故不会因 0 寿命立刻超时。`maxDistance` 仍由 `AttackSimulationSystem` 写成攻击距离；本变更之后，跟踪攻击弹道飞完该距离会自过期，而不是一直飞到命中。`AttackProjectileImpactSystem`（order 123）未改：看到 `EffectExpired` 后丢弃 payload、不发 `DamageRequest`。`RangedAttackHit` 的目标在射程内，仍走到达结算；`RangedAttackExpireLink` 仍由既有 `toExpire` 打上 `EffectExpired` 后由 123 丢弃载荷。过期循环不移除 `AttackProjectileImpact`。

## 风险

- **BREAKING**：技能 Linear 弹道在 `maxDistance > 0` 且飞完该距离时改为过期，不再执行 `arriveEffect`。需要飞到点算命中时改用 Tracking / 曲线，或把 `maxDistance` 留 0。仓库内现有 `Projectile(` 调用未传 `maxDistance`（默认 0），不受此破坏影响。
- 曲线若同时进度到 1 且水平 `traveled` 越界，本帧到达优先；未到进度终点则过期。没有额外配置开关。
- `ProjectileExpiredEvent` 不区分超距与超时。后续若要分支，另开变更，不在请求或事件上加原因字段。
- 攻击弹速常量 900 仍属 Todo 17，本变更未改。
- 真实 War3 客户端视觉与原生销毁未在本回归中执行；回归只覆盖 ECS 语义。

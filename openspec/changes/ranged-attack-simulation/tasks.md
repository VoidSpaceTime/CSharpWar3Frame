# 任务：ranged-attack-simulation

状态：待审核。批准前不实现。

## 1. 提案

- [x] 1.1 用合并后代码核对 `AttackSimulationSystem`、`AttackType`、弹道到达/过期预留与 `DamageRequest` 路径
- [x] 1.2 写 `proposal.md`、`design.md`（含「接口冻结：弹道超距/超时」）、`tasks.md`、`specs/runtime-execution/spec.md`
- [x] 1.3 `openspec validate ranged-attack-simulation` 通过
- [ ] 1.4 用户审核批准（批准门；未批准不得勾选，不得开始第 2 节）

## 2. 实施（批准后）

- [ ] 2.1 新增 `AttackProjectileImpact`、`AttackChainConfig`、`ProjectileLifetime`（`ProjectileLifetime` 只声明类型；攻击路径不挂载）
- [ ] 2.2 `AttackSimulationSystem`：`Ranged` 创建 Tracking 弹道与 payload，`maxDistance` 取 `AttackRange`；`Chain` 用 `GroupHelper.FindInCircle` 选跳并按 `AttackChainConfig` 缺省（4 跳、0.85）衰减；近战分支不改
- [ ] 2.3 新增 `AttackProjectileImpactSystem`（Interval，order 123）：`ProjectileArrived` 发 `DamageRequest`；`EffectExpired` 只移除 payload
- [ ] 2.4 确认新代码路径无 `JassApi` / `KKApi` / `YDApi` / `DzApi`；不修改 `ProjectileSystem` 的过期写入，不修改 `ProjectileLifecycleApplySystem` 的 `:329` 分支

## 3. 验证（批准后，与实施同一闭环）

具名场景注册在 `Projects/Regression` 的 `runtime` 组：

- [ ] 3.1 `runtime/RangedAttackHit`：远程攻击创建弹道；推进到 `ProjectileArrived` 后恰有一条 `damageSrc = Ranged` 的 `DamageRequest`，伤害等于创建时攻击力；飞行阶段请求数为 0
- [ ] 3.2 `runtime/ChainAttackJump`：Chain 在射程内命中第二目标；第二跳伤害 = 第一跳 × 0.85；不创建弹道
- [ ] 3.3 `runtime/ChainAttackNoTarget`：射程内无第二目标时只有第一跳，无空目标 `DamageRequest`
- [ ] 3.4 `runtime/RangedAttackExpireLink`：对带 payload 的弹道直接挂 `ProjectileExpireRequest` 并跑过期终态后，payload 被移除且 `DamageRequest` 数为 0（不断言 `ProjectileSystem` 已会自行写过期请求）
- [ ] 3.5 `dotnet build War3Frame/War3Frame.csproj` 与 `dotnet build Projects/Regression/Regression.csproj` 0 错误；`dotnet run --project Projects/Regression -- runtime` 上列四个场景 PASS
- [ ] 3.6 近战抽样：缺省 `AttackType` 仍直接产生 `damageSrc = Melee` 的 `DamageRequest`

## 4. 收口（批准并实施后）

- [ ] 4.1 写完整 `summary.md`（R2：范围、验证、与 Todo 18 的契约切分、剩余风险）
- [ ] 4.2 `openspec archive ranged-attack-simulation --yes`

## 明确不做

- 不实现 `ProjectileSystem` 内的超距/超时检测与 `_expireRequests` 填充（Todo 18）
- 不把 Linear 飞完 `maxDistance` 从到达改成过期（Todo 18）
- 不新增 `DamageSrc` 枚举值，不新增对外 `XxxEvent`
- 不提交实现，直到 1.4 被用户明确批准

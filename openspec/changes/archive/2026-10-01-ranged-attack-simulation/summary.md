# ranged-attack-simulation 实施总结

## 状态

已实施并完成验证。用户批准依据为 `.omo/evidence/gates/user-ruling-2026-10-01.md` 第 5 项：W4 的 17 远程/链与其余项一并批准实施。本总结不把该记录当作自证，只引用用户裁决。

## 改动范围

- `War3Frame/Src/Components/Unit/Units.cs`：新增 `AttackProjectileImpact`（攻击者、目标、创建时攻击力快照、请求实体 id）与可选 `AttackChainConfig`。
- `War3Frame/Src/Components/Ability/AbilityEffect.cs`：只声明 `ProjectileLifetime`。攻击弹道不挂它，超时入口留给 `projectile-expire-pipeline`。
- `War3Frame/Src/Systems/Unit/AttackSimulationSystem.cs`：`Ranged` 在查询外创建 Tracking 弹道；`Chain` 同帧选跳并衰减；新增 order 123 的 `AttackProjectileImpactSystem`。
- `War3Frame/Src/Systems/Ability/AbilityEffectSystems.cs`：两处既有弹道口径修正，见下方边界。未改过期请求的写入，也未改 `:329` 过期消费分支。
- `Projects/Regression/RangedAttackRegression.cs`：注册五个 `runtime` 场景。

## 行为

- 远程攻击在创建弹道时快照 `AttackDamage`，`maxDistance` 取 `AttackRange`（`<= 0` 记 0，表示不启用超距）。速度 900，到达阈值 30。飞行中不发 `DamageRequest`；`ProjectileArrived` 后按快照发一条 `damageSrc = Ranged` 的请求。
- 闪电链第一跳是原目标。后续跳以当前目标为圆心、`AttackRange` 为半径，经 `GroupHelper.FindInCircle` 取最近的未命中敌对单位。缺省 4 跳、衰减 0.85；没有下一跳就停止，不发空目标请求，也不建弹道。
- 过期终态 `EffectExpired` 只移除 payload，不发伤害。近战仍直接发 `damageSrc = Melee`。
- 新代码路径没有 `JassApi` / `KKApi` / `YDApi` / `DzApi`。扣血仍只在 `DamageResolveSystem`。

## 与冻结接口的边界

`design.md`「接口冻结：弹道超距/超时」保持原语义。本变更没有实现 Todo 18 的写入侧：

- 不在 `ProjectileSystem` 累计 `traveled` / `elapsedTime`，不填充 `_expireRequests`。
- 不把 Linear 飞完 `maxDistance` 从到达改成过期。
- 不消费或改写 `ProjectileLifecycleApplySystem` 的过期分支。`RangedAttackExpireLink` 直接挂 `ProjectileExpireRequest`，只断言终态后不结算。

因此 Todo 18 落地前，攻击弹道超距不会自动过期。这是批准设计里的顺序，不是遗漏。

实施时改了两处既有弹道口径，否则攻击弹道无法复用这条管线：

1. 空 `EffectSource.ability` 跳过 `NormalizeProjectileDefaults`。该函数会读技能属性，攻击没有技能实体。
2. Tracking 到达由 `< threshold` 改为 `<= threshold`。步长把距离夹到阈值上时，原来会停在目标点却永不命中。

## 全局影响

- `War3Frame/`：受影响。攻击分流、命中 payload 与弹道到达口径。
- `War3Frame.Generator/`：不受影响。新系统只用现有 `[SystemRegister]`。
- `FrameBuild/`：不受影响。不改构建、模板或发布。
- `CSharpWar3Frame/`：不受影响。不改 CLI。
- `Projects/`：只新增回归场景，不改 demo/test 契约。

## 验证

- `dotnet build War3Frame/War3Frame.csproj -c Release`：0 error。
- `dotnet build Projects/Regression/Regression.csproj -c Release`：0 error。
- `dotnet run --project Projects/Regression -c Release --no-build`：59 executed，59 passed，0 failed。
- 具名场景：`runtime/RangedAttackHit`、`runtime/ChainAttackJump`、`runtime/ChainAttackNoTarget`、`runtime/RangedAttackExpireLink`，以及近战抽样 `runtime/MeleeAttackDirect`，全部 PASS。

## R2 专项视角

- 命中判定：PASS。伤害快照写在弹道创建时；`PendingStart` 与 `InFlight` 都没有 `DamageRequest`；只有 `ProjectileArrived` 才结算。到达比较包含阈值本身，避免贴脸飞行悬挂。
- 链跳目标选择与衰减：PASS。选目标只读 `SpatialGrid`，排除攻击者与已跳单位，取最近敌对单位。第二跳伤害等于第一跳乘 0.85；射程外单位不产生请求。
- 与冻结接口的边界：PASS。本变更只声明 `ProjectileLifetime` 并在 `EffectExpired` 时丢弃 payload。过期请求的检测、写入和 Linear 语义仍属于 Todo 18。

## 风险与后续

- Todo 18 落地前，攻击弹道即使写了 `maxDistance` 也不会自行过期。超距联动只锁了请求形态和放弃结算。
- Tracking 的 `<=` 到达口径同时作用于技能弹道。它只让原本贴在阈值上的弹道完成到达，不改变超距仍是过期的冻结语义。
- 未做真实 War3 客户端的弹道模型与闪电链视觉验证。本变更允许空模型，视觉仍走既有 Effect Native 层。

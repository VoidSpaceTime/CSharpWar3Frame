# 提案：远程与闪电链攻击结算（ranged-attack-simulation）

## 0. 基本信息

- Change ID: `ranged-attack-simulation`
- 提案等级: `full`
- 状态: `待审核`
- 日期: 2026-10-01
- 目标一句话: 让 `AttackSimulationSystem` 对 `Ranged` 生成可跟踪弹道并在到达后结算，对 `Chain` 做链跳选目标与伤害衰减；超距/超时只写 `ProjectileExpireRequest`，过期执行留给并行变更 `projectile-expire-pipeline`。
- 请求来源: `.omo/plans/war3frame-unlanded-features.md` Todo 17（W4.1）。
- 默认实施后审查强度: `R2 Targeted`
- 命中的审查升级触发器: 核心战斗流程；跨攻击模拟与既有弹道/伤害管线；与并行变更共享弹道过期契约。
- 最终实施后审查强度: `R2 Targeted`（技术准确性 + 跨系统契约；不升 `R3`：不改 Native 分层、不改生成器、不改构建链）
- Oracle 可用性与 `R1` 回退方式: 实施后技术准确性复核优先 Oracle；不可用时由代码审查核对请求所有权、order 与回归场景，并记录替代原因。
- 完整 `review-work` 授权来源: 无

### 0.1 工件矩阵

- 本 change 为 `full`：`proposal.md`、`design.md`、`tasks.md`、`specs/runtime-execution/spec.md`

### 0.2 总结深度矩阵

- 实施完成后写完整 `summary.md`，覆盖改动范围、全局影响、验证、风险与后续。

## 1. 背景

`AttackSimulationSystem`（`War3Frame/Src/Systems/Unit/AttackSimulationSystem.cs:11-75`，`[SystemRegister(SystemKind.Immediate, 124)]`）消费独立请求实体上的 `UnitAttackedRequest`（`War3Frame/Src/Components/Unit/UnitAttackedEvent.cs:11-18`）。近战已落地：伤害取 `AttributeHelper.AttackDamage` 的 finalValue，创建 `DamageRequest`（`damageSrc = DamageSrc.Melee`），随后删除请求实体（`:35-66`）。

`AttackType.Ranged` / `AttackType.Chain`（`War3Frame/Src/Components/Unit/Units.cs:29-33`）在 `:56-59` 被显式跳过：不生成弹道、不结算伤害，请求仍被删除。注释写明「后续增量」。

技能弹道管线已经存在，且与攻击模拟无关：

- `ProjectileSystem`（order 100）推进 `ProjectileData` + `ProjectileRuntimeState`，到达写入内部列表，循环外由 `ProjectileFlowHelper.ApplyRequests` 打 `ProjectileArriveRequest`（`AbilityEffectSystems.cs:17-82`、`:1617-1629`）。
- `ProjectileLifecycleApplySystem`（order 102）把到达请求转成 `ProjectileArrived`，并在有 `arriveEffect` 时调用 `AbilityEffectHelper.CreateArriveEffect`（`:282-327`）。
- `DamageEffectSystem`（order 120）把效果链上的伤害步骤转成 `DamageRequest`；`DamageResolveSystem`（order 125）唯一结算扣血（`:754-825`、`:988`）。
- `ProjectileExpireRequest` 与 `ExpireRequested` / `Expired` 已定义，但注释写明当前无写入方（`AbilityEffect.cs:302-304,363-366`；`AbilityEffectSystems.cs:22-24,329-330,1615`）。`ProjectileRuntimeState.elapsedTime`（`AbilityEffect.cs:325`）已有字段、无写入。

攻击数值已注册但攻击模拟未读射程与间隔：`AttackRange`、`AttackRangeMin`、`AttackInterval`、`AttackSpeed`（`War3Frame/Src/Components/Attribute/Combat.cs:13-18`）。`DamageSrc.Ranged` 已存在（`Damage.cs:12-17`），近战路径未使用。

## 2. 目标

1. `AttackType.Ranged`：消费 `UnitAttackedRequest` 时创建一枚跟踪弹道 effect；弹道到达后由既有到达链产生 `DamageRequest`（`damageSrc = DamageSrc.Ranged`，伤害仍取攻击者 `AttackDamage` finalValue）。请求实体在弹道创建后删除，不在命中前结算。
2. `AttackType.Chain`：以主目标为第一跳立即结算，再按「未命中、敌对、距离上限」选择后续跳，每跳伤害按衰减系数递减；不创建弹道实体。
3. 复用 `ProjectileData` / `ProjectileSystem` / `ProjectileLifecycleApplySystem` / `DamageRequest` / `DamageResolveSystem`。攻击模拟系统与新增 helper 不调用 `JassApi` / `KKApi` / `YDApi` / `DzApi`。
4. 与并行变更 `projectile-expire-pipeline`（Todo 18）冻结弹道超距/超时契约：本变更只定义语义并成为 `ProjectileExpireRequest` 的写入约定方；过期分支的执行、清理与对外事件由 Todo 18 实现。契约正文见 `design.md`「接口冻结：弹道超距/超时」，Todo 18 应原样采用。
5. 近战路径行为不变。

## 3. 非目标

- 不实现 `ProjectileExpireRequest` 的消费、视觉销毁、`EffectExpired` 或过期事件广播（Todo 18）。
- 不把 `elapsedTime` 的累加、`maxDistance` 的飞行中止从「到达」改成「过期」——那是 Todo 18 在 `ProjectileSystem` 内的写入实现。本变更只冻结判定与请求形态。
- 不新增 War3 原生攻击命令、弹道模型句柄或闪电链特效；视觉仍走既有 `EffectHelper` → `EffectNativeSystem`。
- 不实现攻击冷却、攻击动画、弹道碰撞体积多目标、最小射程拒绝、闪避、分裂、弹射以外的 Chain 形态。
- 不改 `DamageResolveSystem` 的计算器、暴击与减免。
- 不改 Source Generator、`SystemRegisterAttribute`、FrameBuild、CLI。
- 不修改 `TriggerSystem`(131) / `EventCleanupSystem`(132)。
- 不清理 `AbilityEffectSystems.cs` 中标注「勿清理」的过期预留代码。

## 4. 影响范围

| 区域 | 是否受影响 | 理由 |
|---|---|---|
| `War3Frame/` | 是 | `AttackSimulationSystem` 分流；新增攻击弹道 payload 与 Chain 选择；可能新增薄 helper。组件落在现有 `Components/Unit` 或 `Components/Ability`。 |
| `War3Frame.Generator/` | 否 | 新逻辑若需独立系统，只加 `[SystemRegister]`，不改生成器契约。默认不新增系统：Ranged 复用 order 100/102/120，Chain 在 order 124 内完成选目标并写 `DamageRequest`。 |
| `FrameBuild/` | 否 | 不改构建、模板、WE、发布。 |
| `CSharpWar3Frame/` | 否 | 不改 CLI。 |
| `Projects/` | 是（实施阶段） | `Projects/Regression` 增加具名场景。本提案阶段不改代码。 |

能力归属：delta 写入既有 `runtime-execution`。攻击结算是默认执行链上的战斗步骤，不单独立能力；理由见 `design.md` 能力选择。

## 5. 方案摘要

- **Ranged**：`AttackSimulationSystem` 在删除 `UnitAttackedRequest` 之前，按攻击者 `Position` 与目标 `Position` 创建 effect 实体，挂 `EffectSource`（caster = attacker，ability = 空）、`EffectTargetInfo`（targetUnit = 原目标）、`ProjectileData`（`trajectoryType = Tracking`，`maxDistance` 取攻击者 `AttackRange` finalValue）、`ProjectileRuntimeState`（`PendingStart`）、`EffectPendingTag`，以及本变更新增的攻击结算 payload（记录结算时伤害快照与 `DamageSrc.Ranged`）。到达后既有 `CreateArriveEffect` 路径不适用「无技能」的攻击弹道，因此到达结算由挂在同一 effect 上的 payload 被 order 120 段的现有或紧邻系统读出并创建 `DamageRequest`。具体落点在 `design.md` 固定为：**不新建技能 EffectSpec**；到达标记出现后，由 `AttackSimulationSystem` 同文件内新增的 `AttackProjectileImpactSystem`（`SystemRegister` Interval，order 123）读取 payload + `ProjectileArrived`，创建 `DamageRequest` 后移除 payload。order 123 位于到达应用（102）之后、`DamageResolveSystem`（125）之前。
- **Chain**：同一次 `OnUpdate` 内，用 `GroupHelper.FindInCircle`（`GroupHelper.cs:36`）以当前跳目标为圆心、`AttackRange` 为半径选下一跳；排除已跳实体与攻击者；跳数默认 4（含第一跳），衰减默认 0.85，写在攻击者可选组件上，缺省用常量。每一跳一个 `DamageRequest`，`damageSrc = DamageSrc.Ranged`（闪电链仍是远程攻击来源；不新增 `DamageSrc` 枚举值）。
- **超距/超时**：本变更的 Ranged 弹道写入 `ProjectileData.maxDistance`（攻击射程）与 payload 上的 `maxLifetime`。`ProjectileSystem` 发现超距或超时后只追加到 `_expireRequests`，由既有 `ApplyRequests` 打上零字段的 `ProjectileExpireRequest`。本变更的实施任务**不修改** `ProjectileSystem` 的过期判定实现，避免与 Todo 18 抢同一写入点；契约先冻结，Todo 18 落地写入与 `:329` 消费分支。在 Todo 18 落地前，超距弹道仍会按现有 Linear/Tracking 规则飞行或停在到达语义——这是已知缺口，回归场景「超距联动」只断言**请求形态与字段**，不断言过期分支已执行。

## 6. 验收标准

提案阶段：

1. `openspec validate ranged-attack-simulation` 通过。
2. `design.md` 含「接口冻结：弹道超距/超时」，定义最大距离、最大存活、写入方、实体/token、结算分支，可供 Todo 18 原样采用。

批准后的实施阶段（写入 `tasks.md`，本阶段不执行）：

1. 具名回归：`runtime/RangedAttackHit`、`runtime/ChainAttackJump`、`runtime/ChainAttackNoTarget`、`runtime/RangedAttackExpireLink`。
2. 近战既有行为不回退；`DamageResolveSystem` 仍是唯一扣血点。
3. `dotnet build War3Frame/War3Frame.csproj` 与 `dotnet build Projects/Regression/Regression.csproj` 0 错误。
4. 新增代码路径无 `JassApi` / `KKApi` / `YDApi` / `DzApi` 调用。

## 7. 风险

| 风险 | 缓解 |
|---|---|
| 与 Todo 18 同时改 `ProjectileSystem` / `ProjectileExpireRequest` | 本变更不实现过期写入与消费；契约冻结在 design，Todo 18 原样采用。超距联动场景只锁请求形态。 |
| 攻击弹道没有 ability，套不进 `EffectChainBuilder.Projectile` | 不伪造技能模板；直接创建 effect 实体并挂攻击 payload。 |
| Chain 选目标与 `SpatialGridSystem`(order 99) 的同帧可见性 | Chain 在 Immediate 124 执行，网格由 Interval 99 维护；场景用已写入 `Position` 的实体，并在设计中要求选目标前实体已进网格。若 Immediate 早于当帧网格重建，实施时改为读取 `Position` 的同 Store 查询作为回退，仍不调用原生 Group。 |
| order 123 新系统与 120/125 抢 `DamageRequest` | 新系统只创建请求，不修改、不删除 `DamageRequest`；扣血仍只在 125。 |
| 伤害快照 vs 命中时重读攻击力 | 冻结为**弹道创建时**读取 `AttackDamage` finalValue 写入 payload，命中不再重读，避免飞行期间装备变化导致不可重复测试。 |

## 8. 回滚

- 未批准：删除 `openspec/changes/ranged-attack-simulation/`。
- 已实施：还原 `AttackSimulationSystem` 的 Ranged/Chain 分支为跳过，删除攻击 payload 与 `AttackProjectileImpactSystem`；不回滚 `ProjectileExpireRequest` 类型本身（它在本变更之前已存在）。

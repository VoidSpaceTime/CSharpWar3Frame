# Proposal: remove-dead-projectile-template-hooks

**等级**: light
**状态**: 已实施
**默认复盘强度**: R0 → **实际 R1**（涉及公共类型删除，需一次技术准确性复核）

## 分级说明

升级触发器 `涉及公共 API / 数据结构` **命中**（删除 `public abstract class AbilityTemplateBase`
与 `public enum ProjectileTravelDecision`）。按仓库规则应升 `full`，但依据两点判定为 `light`：

1. 两个公共类型**零消费者**——全仓无任何类继承 `AbilityTemplateBase`，`ProjectileTravelDecision`
   仅在即将删除的 `ProjectileHookBridge` 内部流转。
2. 用户全局偏好要求简明扼要、避免过度架构化，且改动为纯删除、易回滚（单 commit revert）。

补 `tasks.md`，不补 `design.md`/`spec.md`。

## 现状

`AbilityTemplateBase`（`AbilityTemplateAttribute.cs:20`）是一个零子类抽象基类，
它的三个虚方法 `OnProjectileStart` / `OnProjectileTravel` / `OnProjectileArrive` 无任何实现者。

唯一引用链是向下转型：

```csharp
// AbilityEffectSystems.cs:1454
template = raw as AbilityTemplateBase ?? null!;
return template != null;                 // 永远 false
```

全部 17 个技能模板（`Projects/test/Scripts/Template/Ability.cs`）以及框架内
`InlineItemAbilityTemplate` 均直接实现 `IAbilityTemplate`，无一继承基类。
因此 `ProjectileHookBridge` 的三个 Dispatch 方法每帧被调用但全部提前返回，
弹道 hook 是**静默空转层**。

弹道行为的真实实现已由效果链数据表达：

```csharp
// Ability.cs:179 (LavaBallTemplate)
.Projectile("Abilities\\...", speed, ProjectileTrajectoryType.Parabolic,
    onArrive: arrive => arrive.Area(...).Damage(...))
```

这是 `2397704`（清理 Projectile Hook 遗留接口）的直接延续：那次删掉了
`IProjectile` / `IProjectileHooksV2` 两套 legacy hook，把三套收敛到 `AbilityTemplateBase` 一套，
但收敛到的这一套本身也是空的。

## 目标

删除 `AbilityTemplateBase` 与 `ProjectileHookBridge`，让 `IAbilityTemplate` 回归纯接口，
弹道行为完全由效果链数据驱动。

## 非目标（关键）

- **不删过期生命周期管线**。见下节。
- **不改 `maxDistance` 现有语义**。当前 `UpdateLinear:180` 在 `traveled >= maxDistance`
  时返回 `arrived = true`，走的是**到达**流程而非过期——弹道飞到射程尽头会照常触发落点伤害。
  这是既有行为，改它属于后续超距提案的范围。
- 不动 UI 层继承（`UIComponent` / `UIPanel` 有 7 个活跃子类，符合 `AGENTS.md:228`
  「有强父子关系的 UI 树倾向 OOP」）。
- 不动 `QuerySystem<T>` 继承（Friflo 框架要求）。
- 不改弹道运动算法、轨迹类型或 authoring API 签名。

## 预留：过期路径管线（为后续超距超时能力保留）

`ProjectileTravelDecision.RequestExpire` 是当前**唯一**进入过期流程的入口。
删掉 hook 后下列代码将暂时零写入方，但**不删除**，因为后续超距/超时能力正是它们的消费者：

| 位置 | 内容 |
|---|---|
| `AbilityEffectSystems.cs:21,31,79` | `_expireRequests` 字段、Clear、ApplyRequests 传参 |
| `AbilityEffectSystems.cs:328-339` | `toExpire` 过期结算 foreach |
| `ProjectileFlowHelper.ApplyRequests` | expire 半边打标逻辑 |
| `AbilityEffect.cs:297,298` | `ProjectileLifecyclePhase.ExpireRequested` / `Expired` |
| `AbilityEffect.cs:354` | `ProjectileExpireRequest` tag |
| `AbilityEffect.cs:59` | `EffectExpired` tag（`AbilityEffectSystems.cs:1215` 的读取暂时恒假） |

这与 `AbilityAttachRequest` / `AbilityRemoveRequest`（零写入方且无计划消费者）**不同**：
后者是应清理的死路径，前者是有明确近期消费者的休眠管线。
实施时须在代码注释中标注这一区别，避免下一轮复盘误删。

## 后续提案（本次不做）

超距 / 超时能力需要独立提案，届时要解决：

1. `maxDistance` 应走过期而非到达；且当前**只有 `Linear` 轨迹**读取 `maxDistance`，
   `Tracking` / `Bezier` / `Parabolic` / `Sinusoidal` / `Spiral` 完全忽略它。
2. 超时无数据源——`ProjectileRuntimeState.elapsedTime`（`AbilityEffect.cs:319`）
   **从未被任何代码递增**，需要新增 `maxLifetime` 字段并在推进时累加 `Tick.deltaTime`。
3. 新增 authoring 字段会改 `ProjectileData`、`ProjectileEffectStepSpec`
   与 `EffectChainBuilder.Projectile` 两个重载签名 → 该提案为 `full` 级。

## 影响范围

| 文件 | 操作 |
|---|---|
| `War3Frame/Src/TemplateInit/AbilityTemplateAttribute.cs` | 删除 `AbilityTemplateBase`（20-51）与 `ProjectileTravelDecision`（10-18） |
| `War3Frame/Src/Systems/Ability/AbilityEffectSystems.cs` | 删除 `ProjectileHookBridge`（1418-1457）及 3 处调用（45、49-55、320）；`if (arrived && decision != ...)` 简化为 `if (arrived)` |
| `War3Frame/Src/Components/Ability/AbilityEffect.cs` | 仅改注释：`ProjectileTrajectoryType.Custom`（301）的「保留给模板 hook」已失效；过期路径标注休眠原因 |
| `War3Frame/Docs/ProjectileSystem使用说明.md` | 核查并移除 hook 相关描述（若有） |

**不受影响**：`Projects/test`（无模板继承基类，无需改动）、生成器、构建链、UI 层。

## 风险与回滚

- **风险**：过期管线暂时无写入方，若后续超距提案迟迟不落地，会积累成新的死路径。
  缓解：代码注释显式标注「为超距超时保留」+ 本提案 `后续提案` 章节留痕。
- **风险**：外部地图脚本（本仓库外）若继承了 `AbilityTemplateBase` 会编译失败。
  评估：该基类的 hook 从未生效过，继承它的代码本就没有实际行为，破坏面为零。
- **回滚**：单 commit revert。

## 验收标准

- 构建通过（若 `CSharpLanguageServer` 锁 DLL，用 `-p:BuildProjectReferences=false` 绕过）
- `grep AbilityTemplateBase` / `grep ProjectileHookBridge` / `grep ProjectileTravelDecision` 全仓零命中
- `ProjectileSystem.OnUpdate` 中无 `decision` 变量
- 过期管线 6 处代码保留且带休眠标注注释
- `Projects/test` 无需修改即可构建

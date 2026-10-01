# 提案：原生桥接补全（native-bridge-completion）

## 0. 基本信息

- Change ID: `native-bridge-completion`
- 提案等级: `full`
- 状态: `待审核`
- 日期: 2026-10-01
- 目标一句话: 把 WE 预置单位与可破坏物登记进 `NativeEntityIndex`，使 `NativePlayerEventBridge` 能反查它们；并按合并后 `ControlType` 与 1.27a 原生表，只为 `Silence` 补上可解析的原生动作，其余无函数的控制保持事件层响应。
- 请求来源: `.omo/plans/war3frame-unlanded-features.md` Todo 32（W4.9）。
- 默认实施后审查强度: `R2 Targeted`
- 命中的审查升级触发器: 核心运行时桥接；跨 Native 索引、玩家事件桥与控制投影；句柄生命周期。
- 最终实施后审查强度: `R2 Targeted`（技术准确性 + 句柄配对/跨系统契约；不升 `R3`：不改生成器、不改构建链、不改公共 CLI）
- Oracle 可用性与 `R1` 回退方式: 实施后技术准确性复核优先 Oracle；不可用时由代码审查核对登记幂等、句柄配对与回归场景，并记录替代原因。
- 完整 `review-work` 授权来源: 无

### 0.1 工件矩阵

- 本 change 为 `full`：`proposal.md`、`design.md`、`tasks.md`、`specs/native-state-projection/spec.md`

### 0.2 总结深度矩阵

- 实施完成后写完整 `summary.md`，覆盖改动范围、全局影响、验证、风险与后续。

## 1. 背景

两条已归档变更留下的缺口，在当前 `ECS-Framework` 上仍然成立，但代码形态已经变过，不能按归档时的枚举名直接补。

**预置实体未进反查表。** `NativeEntityIndex`（`War3Frame/Src/Systems/Native/NativeEntityIndex.cs:14-35`）是 native handle → ECS entity 的单表。当前全仓只有两个调用点：

- `UnitCreateNativeSystem.OnUpdate` 在 `CreateUnit` 与 `HandleHelper.HandleAdd` 之后 `Register`（`UnitCreateNativeSystem.cs:30-32`）。
- `UnitRemoveNativeSystem` 在 `HandleRemove` / `RemoveUnit` 之前 `Unregister`（`UnitRemoveNativeSystem.cs:55-60`）。

`NativePlayerEventBridge.RegisterPlayerUnitAttacked` 对攻击者与目标都要求 `TryGetByNative` 成功，任一失败即返回（`NativePlayerEventBridge.cs:38-45`）。WE 摆在地图上的单位和可破坏物不经过 `UnitCreateNativeSystem`，因此被攻击时桥接直接丢弃。归档 `2026-09-11-native-unit-event-bridge/summary.md:46` 已把这项标为 TODO。

**控制枚举已合并，沉默仍无原生动作。** 归档 `2026-09-11-control-state-superposition/summary.md:11,29` 写的是 `Stun/Silence/Disarm/Root/Knockback`，且 `UnitControlNativeSystem` 只实现了 `Stun → PauseUnit`。合并后的当前枚举是 `ControlType`（`ControlState.cs:8-52`）：`Stun`、`Silence`、`NoAttack`（原 Disarm）、`Root`、`CrackFly`（原 Knockback）、`Hide`、`NoPath`、`Locust`、`Invulnerable`、`Invisible`、`Pause`、`Sorcery`。

当前 `UnitControlNativeSystem`（`UnitNativeRequestSystems.cs:37-59`）已映射：

| ControlType | 当前原生动作 |
|---|---|
| `NoAttack` | `DzApi.DzUnitDisableAttack` |
| `Hide` | `JassApi.ShowUnit`（`entered` 直接传入） |
| `Root` / `NoPath` | `JassApi.SetUnitPathing`（`entered` 直接传入） |
| `Pause` | `JassApi.PauseUnit` |
| `Locust` / `Invulnerable` / `Invisible` / `Sorcery` | 空分支，注释写明等默认地图模板提供对应 ability |
| `Silence` | **无 case**，请求被删除且无副作用 |
| `Stun` / `CrackFly` | 不进入本 switch：`ControlStateTransitionSystem` 只为它们广播 `ControlStateChangedEvent`，原生暂停由合成位 `Pause` 承担（`ControlStateTransitionSystem.cs:81-98`） |

`Silence` 的事件层已经完整：`ControlHelper.IsSilenced` / `CanCast`（`ControlHelper.cs:84-98`）禁止新施法，`ControlStateChangedEvent` 照常广播。缺的只是原生侧禁止施法。

仓库目标运行时是 War3 **1.27a**（`ARCHITECTURE.md:5,11`）。`DzApi.DzUnitSilence`（`DzApi.cs:132,1349-1351`）的指针来自 `War3.GetNativeFunction("DzUnitSilence")`；该查找在原生表未命中时返回 `0`（`Library/JassVM/Native.cs` 的 `GetNativeFunction`），**不抛异常**。1.27a 的 `game.dll` 原生表没有 `DzUnitSilence`，调用会把空指针当 cdecl 函数执行。因此「绑定文件里有这个方法」不等于「1.27a 可调用」。`DzUnitDisableAttack` 已在当前 `NoAttack` 分支使用，其 1.27a 可解析性由既有路径承担，本变更不重开。

## 2. 目标

1. 在 `NativePlayerEventBridge.Initialize()` 之前，一次性枚举当前地图上的预置单位与可破坏物，为每个尚未登记的句柄创建 ECS 代理实体并 `NativeEntityIndex.Register`。
2. 框架经 `UnitCreateNativeSystem` 创建的单位不得被第二次 `Register` 覆盖。
3. 预置单位被攻击时，`NativePlayerEventBridge` 能反查到代理实体并创建 `UnitAttackedRequest` 与 `UnitAttackedEvent`。预置可破坏物同样可被反查；攻击桥仍只消费单位事件，不新增可破坏物事件类型。
4. `ControlType.Silence` 进入/解除时，`UnitControlNativeSystem` 调用 1.27a 原生表中真实存在的函数，使原生单位不能/恢复主动施法。
5. 对合并后枚举里仍无可用 1.27a 函数的控制，保持现有事件层响应与现有空分支，不发明 ability、不调用空指针。

## 3. 非目标

- 不把预置单位提升为完整框架单位：不挂 `UnitSpec`、不跑属性/技能/物品模板、不加入 `UnitCreateNativeRequest` 创建链。
- 不实现预置单位的死亡、移除、升级或其它玩家单位事件；桥接事件类型仍只有 `EVENT_PLAYER_UNIT_ATTACKED`。
- 不枚举地面物品，不新增 `ItemNative` 反查。
- 不改 `NativeEntityIndex` 的单表结构、键（`GetHandleId`）或 `Register` 覆盖语义。
- 不把 `DzUnitSilence` 接进 `Silence` case。
- 不实现 `Locust` / `Invulnerable` / `Invisible` / `Sorcery` 的 ability 模板，不改 `Hide` / `Root` / `NoPath` / `NoAttack` / `Pause` 的现有调用。
- 不改 `ControlStateTransitionSystem` 的跳变、免疫吸收或 Pause 合成。
- 不改 `EffectNativeSystem` 是否登记 `NativeEntityIndex`（Todo 24(c) 的范围；本变更只读，不编辑该文件）。
- 不改 Source Generator、`SystemRegisterAttribute`、FrameBuild、CLI。

## 4. 影响范围

| 区域 | 是否受影响 | 理由 |
|---|---|---|
| `War3Frame/` | 是 | 新增预置实体 adoption（初始化路径 + 代理组件）；`UnitControlNativeSystem` 增加 `Silence` case。 |
| `War3Frame.Generator/` | 否 | 不新增系统，不改 `SystemRegisterAttribute` 与生成输出。 |
| `FrameBuild/` | 否 | 不改构建、模板、WE、发布。 |
| `CSharpWar3Frame/` | 否 | 不改 CLI。 |
| `Projects/` | 是（实施阶段） | `Projects/Regression` 增加两个具名场景。本提案阶段不改代码。 |

能力归属：delta 写入既有 `native-state-projection`。预置实体登记是句柄生命周期与反查投影；沉默动作是控制状态到原生副作用的投影。不写入 `control-state`：该 spec 当前只约束免疫吸收，本变更不改吸收语义。不写入 `runtime-execution`：不改默认执行链顺序。

## 5. 方案摘要

### 5.1 预置实体 adoption

在 `Game.War3Init` 中，`PlayerHelper.InitializePlayers` 之后、`NativePlayerEventBridge.Initialize()` 之前，调用一次 `PresetEntityAdoption.AdoptAll(Store)`。此时主计时器尚未启动，框架还没有 `CreateUnit`，地图上的单位与可破坏物就是 WE 预置集。

枚举使用 1.27a 已绑定、且 `GetNativeFunction` 名存在于原版 Jass 的函数：

- 单位：`GetWorldBounds` → `CreateGroup` → `GroupEnumUnitsInRect(group, world, null)` → `ForGroup` 内 `GetEnumUnit`。
- 可破坏物：同一 world rect 上 `EnumDestructablesInRect(world, null, callback)`，回调内 `GetEnumDestructable`。

每个枚举到的句柄：

1. `NativeEntityIndex.TryGetByNative` 已命中则跳过，不创建实体、不再次 `Register`。这是防双登记的唯一规则；不依赖「初始化早于创建」这一时序假设单独成立。
2. 未命中则 `store.CreateEntity`，单位挂 `UnitNative` + `PresetNativeTag`，可破坏物挂新增的 `DestructableNative` + `PresetNativeTag`。
3. 紧接着 `HandleHelper.HandleAdd`，再 `NativeEntityIndex.Register`。预置对象不是本框架 `CreateUnit` 出来的，`HandleAdd` 只增加引用计数，避免句柄在桥接回调期间被回收；不调用 `RemoveUnit` / `RemoveDestructable`。

临时 `JGroup` 与 `GetWorldBounds` 返回的 `JRect` 在枚举结束后 `DestroyGroup` / `RemoveRect`。这两个是枚举工具句柄，不进入 `NativeEntityIndex`。`HandleAdd`/`HandleRemove` 与销毁相邻，规则见 `design.md`。

代理实体不进入 `UnitRemoveNativeSystem` 的查询（该系统要求 `UnitLifeState`，代理不挂它）。预置对象的原生销毁不在本变更范围；代理与索引条目保留到进程结束。不在枚举回调里订阅 `TriggerRegisterDeathEvent`。

### 5.2 Silence 与同类控制

合并后 12 个 `ControlType` 的 1.27a 结论：

| ControlType | 1.27a 结论 | 本变更动作 |
|---|---|---|
| `Silence` | 无 `DzUnitSilence`。原版有 `UnitAddAbility` / `UnitRemoveAbility` / `IssueImmediateOrder`。沉默用魔兽原版技能 `ACsp`（Spell Shield / 控制魔法），`IssueImmediateOrder(unit, "spellshield")` 切换原生施法锁 | `UnitControlNativeSystem` 增加 case：进入时若 `GetUnitAbilityLevel != 1` 则 `UnitAddAbility(ACsp)` 并 `spellshield`；解除时若 level 为 1 则再 `spellshield` 后 `UnitRemoveAbility`。已有其它来源的 `ACsp` 则本分支不再添加或移除 |
| `NoAttack` | `DzUnitDisableAttack` 已在用 | 不改 |
| `Hide` | `ShowUnit` 已在用 | 不改。极性是否反了属于既有行为，不在本变更修正 |
| `Root` / `NoPath` | `SetUnitPathing` 已在用；1.27a 无「禁止移动但不改碰撞」的函数 | 不改。`Root` 的事件层 `IsImmobilized` 保持 |
| `Pause` | `PauseUnit` 已在用 | 不改 |
| `Stun` / `CrackFly` | 原生暂停已由 `Pause` 合成承担 | 不改，不新增 case |
| `Locust` / `Invulnerable` / `Invisible` / `Sorcery` | 依赖地图 ability，当前空分支注释已说明 | 保持空分支，不新增 ability 模板 |

`ACsp` 与命令串 `spellshield` 是 1.27 原版技能与命令，不是 KK/YD 扩展函数，不经过 `GetNativeFunction("DzUnitSilence")`。`design.md` 记录空指针证据与「不调用 `DzUnitSilence`」的决定。

## 6. 验收标准

提案阶段：

- `openspec validate native-bridge-completion` 通过。
- `proposal.md` / `design.md` / `tasks.md` / delta 覆盖防双登记、句柄配对、`Silence` 的 1.27a 函数选择，以及无函数控制保持事件层。

实施阶段（批准后）：

- `Projects/Regression` 具名场景 `PresetUnitAttackBridge`：索引中有预置单位时代理可被 `TryGetByNative` 命中；同一句柄第二次 adoption 不覆盖实体；攻击桥能创建 `UnitAttackedRequest`。
- 具名场景 `SilenceNativeMapping`：`ControlType.Silence` 进入/解除各产生一次原生调用记录，且记录里没有 `DzUnitSilence`；`Locust` 等空分支仍无调用。
- `dotnet build War3Frame/War3Frame.csproj` 0 错误。
- 真实 War3 客户端上的枚举与 `ACsp` 手感验证默认阻塞归档；若审核阶段显式声明非阻塞，`summary.md` 记录未执行原因与剩余风险。

## 7. 风险与回滚

- 全图枚举在单位很多的地图上会拉长 `War3Init`；只做一次，不进 tick。
- `ACsp` 若被地图物编改过命令或等级，`spellshield` 可能无效。缓解：只在 `GetUnitAbilityLevel != 1` 时添加；解除只移除本分支添加的实例。失败时事件层 `CanCast` 仍然有效。
- `HandleAdd` 而不 `RemoveUnit` 会把预置句柄的引用计数保持到进程结束。这是有意的，避免桥接使用已释放句柄；不在本变更做死亡回收。
- 回滚：删除 `PresetEntityAdoption` 调用与 `Silence` case，反查表回到「只有 `UnitCreateNativeSystem` 登记」。无持久化数据。

## 8. 开放问题

- `Hide` 的 `ShowUnit(unit, entered)` 极性（进入隐藏却传入 `true`）是否为既有缺陷。本变更不改。
- 预置单位死亡后索引残留。本变更不订阅死亡事件；若后续要回收，单独提案，并与 `UnitRemoveNativeSystem` 的唯一销毁点对齐。
- Todo 24(c) 若决定 `EffectNativeSystem` 也 `Register`，必须复用本变更的 `TryGetByNative` 先查规则，不得在特效系统里直接覆盖预置条目。本变更不实现该登记。

# 设计：native-bridge-completion

## 1. 能力选择

delta 只改 `native-state-projection`。

- 预置实体登记是「原生句柄 ↔ ECS 代理」的投影与句柄生命周期，和该 spec 已有的「句柄注销顺序」同一能力。
- `Silence` 的原生动作是控制请求到原生副作用的投影，消费点在 `UnitControlNativeSystem`。
- 不改 `control-state`：免疫吸收与「已施加控制不被回收」保持原样。
- 不改 `runtime-execution`：不新增系统、不改 order。

## 2. 现状锚点（合并后，非归档原文）

| 事实 | 位置 |
|---|---|
| 唯一 `Register` | `UnitCreateNativeSystem.cs:32`，在 `CreateUnit`（`:30`）与 `HandleAdd`（`:31`）之后 |
| 唯一 `Unregister` | `UnitRemoveNativeSystem.cs:57`，紧挨 `HandleRemove`（`:59`）与 `RemoveUnit`（`:60`）之前 |
| 反查消费 | `NativePlayerEventBridge.cs:42-45`；失败即 return |
| 桥初始化 | `War3Init.cs:13-17`：`CreatePlayers` → `InitializePlayers` → `NativePlayerEventBridge.Initialize` → 之后才 `StartMainTimer` |
| 索引语义 | `NativeEntityIndex.Register` 以 `GetHandleId` 为键，重复登记覆盖（`NativeEntityIndex.cs:18-22`） |
| 控制请求消费 | `UnitControlNativeSystem`，`UnitNativeRequestSystems.cs:37-59` |
| 沉默事件层 | `ControlHelper.IsSilenced` / `CanCast`，`ControlHelper.cs:84-98`；`ControlStateTransitionSystem` 对 `Silence` 既发事件也发 `ControlStateNativeRequest`（`:81-83`，仅排除 Stun/CrackFly） |
| 空指针查找 | `War3.GetNativeFunction` 未命中返回 `0`（`Library/JassVM/Native.cs`） |
| 目标版本 | War3 1.27a（`ARCHITECTURE.md:5`） |

`EffectNativeSystem` 当前不调用 `NativeEntityIndex`。Todo 24(c) 若要登记特效，必须先 `TryGetByNative`，本设计不改那个文件。

## 3. 预置实体枚举

### 3.1 时机

`PresetEntityAdoption.AdoptAll(EntityStore store)` 插在 `War3Init.cs` 的 `PlayerHelper.InitializePlayers` 与 `NativePlayerEventBridge.Initialize()` 之间。主计时器未启动，`UnitCreateNativeSystem` 尚未跑，正常启动路径上预置集与框架创建集不相交。防双登记仍以索引查询为准，不把时序当成唯一保证。

### 3.2 机制

全部使用 `JassApi` 已绑定函数，原生名均在 1.27 Jass 中：

```text
world = GetWorldBounds()          // 新 rect，用完 RemoveRect
group = CreateGroup()             // 用完 DestroyGroup
GroupEnumUnitsInRect(group, world, null)
ForGroup(group, () => adopt unit GetEnumUnit())
EnumDestructablesInRect(world, null, () => adopt GetEnumDestructable())
```

`filter` 传 `null`：预置中立敌对、中立被动与玩家单位都要能被攻击桥反查，不能按玩家或联盟过滤。

`GetWorldBounds` 覆盖地图边界外的预置物；`GetPlayableMapRect` 会漏掉边界外摆放，不采用。

回调里只做三件事：读枚举句柄、查索引、必要时建代理。不在回调里 `CreateUnit`、`RemoveUnit`、发伤害或注册死亡触发器。

### 3.3 代理实体

| 种类 | 组件 | 不挂 |
|---|---|---|
| 单位 | `UnitNative { unit, player = GetOwningPlayer(unit) }`，`PresetNativeTag` | `UnitLifeState`、`UnitCreateNativeRequest`、`UnitBase`、属性 |
| 可破坏物 | `DestructableNative { destructable }`，`PresetNativeTag` | 单位组件、生命阶段 |

`PresetNativeTag` 是 `ITag`，零字段，只标记「本实体由预置 adoption 创建」。`DestructableNative` 是 `IComponent`，因为带句柄字段。

`UnitNative.player` 用 `GetOwningPlayer` 填充，供以后 Native 系统识别归属；本变更没有任何系统读取预置单位的 `player` 做业务判断。

攻击桥不区分代理与框架单位：反查成功就建 `UnitAttackedRequest` / `UnitAttackedEvent`。可破坏物没有 `EVENT_PLAYER_UNIT_ATTACKED` 的单位句柄路径；登记它们是为了同一张表能 `TryGetByNative`，不新增可破坏物事件。

### 3.4 不双登记

```text
if (NativeEntityIndex.TryGetByNative(handle, out _))
    return;
创建代理 → HandleAdd → Register
```

因此：

- 框架单位若已在表中（测试夹具、或将来初始化顺序变化），枚举到同一 `GetHandleId` 时跳过，`Register` 的覆盖语义不会把框架实体换成代理。
- 同一轮 `ForGroup` 不会对同一单位回调两次；即便发生，第二次也被 `TryGetByNative` 挡住。
- 不修改 `NativeEntityIndex.Register`。覆盖行为仍是创建路径的契约，adoption 路径禁止走到覆盖。

### 3.5 句柄配对

预置对象的原生创建者是 WE/游戏加载，不是本框架。本框架不能在 adoption 时 `RemoveUnit`。

| 句柄 | 创建方 | 本变更 | 销毁 |
|---|---|---|---|
| 预置 `JUnit` / `JDestructable` | 地图加载 | 枚举到之后立即 `HandleAdd`，再 `Register` | 不销毁。引用保持到进程结束 |
| `GetWorldBounds` 的 `JRect` | 本调用 | 不 `HandleAdd`（短生命周期工具句柄，回调期间栈上使用） | 枚举结束立即 `RemoveRect` |
| `CreateGroup` 的 `JGroup` | 本调用 | 同上 | 枚举结束立即 `DestroyGroup` |

不把工具 rect/group 登记进 `NativeEntityIndex`。不在 `UnitRemoveNativeSystem` 增加预置分支：该系统是框架单位 `UnitLifeState.Remove` 的唯一销毁点，预置代理没有 `UnitLifeState`，不会被误销毁。

死亡回收明确排除。若以后要在预置单位死亡时 `Unregister`，必须新提案，并且只能有一个注销点。

## 4. Silence 映射

### 4.1 候选与否决

| 候选 | 1.27a 事实 | 决定 |
|---|---|---|
| `DzApi.DzUnitSilence` | `GetNativeFunction("DzUnitSilence")` 在 1.27a 原生表未命中，返回 `0`。`DzApi.cs:1349` 仍会 `CallNative` | **否决。** 空指针调用不是可用 API |
| `DzUnitDisableAttack` | 已用于 `NoAttack`，且不是沉默 | 不改，不拿来模拟沉默 |
| `PauseUnit` | 会暂停移动、攻击与动画，语义宽于沉默 | 否决。暂停已由 `Pause` 合成占用 |
| `UnitAddAbility` + 命令 | `UnitAddAbility`（`JassApi.cs:5691`）、`UnitRemoveAbility`（`:5700`）、`GetUnitAbilityLevel`（`:5056`）、`IssueImmediateOrder`（`:5874`）都在 1.27 原版 Jass | **采用** |

采用的技能是原版 `ACsp`（Spell Shield，控制魔法）。物编行为是：单位拥有该技能且处于开启时，不能主动施放命令。切换命令是 `spellshield`。这不是新函数，不依赖 KK/YD 原生表。

### 4.2 case 行为

`UnitControlNativeSystem` 在现有 switch 增加 `ControlType.Silence`，仍在确认 `UnitNative` 存在之后：

```text
entered == true:
  if GetUnitAbilityLevel(unit, 'ACsp') != 1:
      UnitAddAbility(unit, 'ACsp')
      IssueImmediateOrder(unit, "spellshield")
entered == false:
  if GetUnitAbilityLevel(unit, 'ACsp') == 1:
      IssueImmediateOrder(unit, "spellshield")
      UnitRemoveAbility(unit, 'ACsp')
```

- 等级不为 1 视为「地图已经给了别的等级或没有可切换的默认实例」，进入时不覆盖，解除时不移除。避免拆掉地图作者自己的魔法盾。
- 进入时先加再开：新添加的 `ACsp` 默认关闭，必须 `spellshield` 一次才锁施法。
- 解除时先关再移除：避免移除瞬间命令落在已删除技能上；若 `spellshield` 失败，仍移除本分支添加的等级 1 实例，事件层 `CanCast` 继续禁止施法。
- 无 `UnitNative` 的单位维持现有路径：删请求、不调用原生函数。
- 不调用 `DzUnitSilence`，包括「先探测指针再调用」。探测只允许留在注释或回归断言里，不进入生产分支。

### 4.3 同类控制（合并后枚举，不是归档四元组）

归档里的 `Disarm` / `Knockback` 已不存在。

| 当前枚举 | 处理 |
|---|---|
| `NoAttack` | 已映射 `DzUnitDisableAttack`。视为 Disarm 的合并后形态，本变更不重做 |
| `CrackFly` | 已由 `Pause` 合成驱动 `PauseUnit`；位移仍由业务监听 `ControlStateChangedEvent`。不新增击退原生函数（1.27a 无击退 API） |
| `Root` | 已映射 `SetUnitPathing`。1.27a 没有「停移动但保留碰撞」的函数；事件层 `IsImmobilized` 继续负责框架内移动判定。不把 `PauseUnit` 扩大成定身 |
| `Stun` | 同 `CrackFly`，不进 switch |
| `Locust` / `Invulnerable` / `Invisible` / `Sorcery` | 保持 `UnitNativeRequestSystems.cs:54-58` 的空分支。不在本变更制作 ability 模板 |
| `Hide` / `NoPath` / `Pause` | 已有调用，不改 |

「无可用 API」的记录对象是：`Root` 的精确停步、`CrackFly` 的位移、以及四个 ability 类控制。它们继续发 `ControlStateChangedEvent`（`Pause` 除外，它本来就不广播）。

## 5. 分层

- `PresetEntityAdoption` 放在 `War3Frame/Src/Systems/Native/` 或 `initialization/War3Init/` 旁的 Native 辅助类型。它只在初始化执行原生枚举与 `HandleAdd`，不推进业务状态。允许直接调用 `JassApi`，因为它就是 Native 执行步骤，不是施法/AI/规则系统。
- `UnitControlNativeSystem` 继续是唯一消费 `ControlStateNativeRequest` 的地方。`ControlHelper` 不调用 `UnitAddAbility`。
- 业务系统仍只看 `ControlStateChangedEvent` 与 `CanCast`。

## 6. 验证设计

回归宿主不启动 War3。两个场景都在 `Projects/Regression` 的 `runtime` 组，用真实 ECS 类型；原生函数由场景内的替身记录，不链接 `game.dll`。

### 6.1 `PresetUnitAttackBridge`

1. 建 store，先 `Register` 一个「框架单位」句柄 H1 到实体 E1。
2. 调用 adoption 的纯索引部分（或把枚举结果注入为句柄列表）：输入 H1 与未登记的 H2。
3. 断言 H1 仍反查到 E1，且 E1 没有 `PresetNativeTag`。
4. 断言 H2 反查到新实体 E2，E2 有 `UnitNative` 与 `PresetNativeTag`，`HandleAdd` 记录恰好一次且在 `Register` 之前。
5. 用 E2 作为攻击者与目标，走桥接条件里「两侧都 `TryGetByNative` 成功则创建 `UnitAttackedRequest`」的同一分支（提取成内部方法或在测试里复刻该三行创建，但断言的是索引命中后请求存在）。未登记句柄不产生请求。

不在回归里调用真 `GroupEnumUnitsInRect`。枚举函数序列写在本设计与任务里，客户端验证单独记录。

### 6.2 `SilenceNativeMapping`

1. 单位带 `UnitNative`，写入一条 `entered = true` 的 `ControlStateNativeRequest(Silence)`，跑 `UnitControlNativeSystem`。
2. 记录器断言调用序列是 `GetUnitAbilityLevel` →（level 不为 1 时）`UnitAddAbility(ACsp)` → `IssueImmediateOrder("spellshield")`，且全程没有 `DzUnitSilence`。
3. 再写 `entered = false`，断言 `spellshield` 之后 `UnitRemoveAbility`，请求实体被删除。
4. 对照：`ControlType.Locust` 的请求不产生上述调用，请求仍被删除。
5. 无 `UnitNative` 的 `Silence` 请求不产生调用。

`CanCast` 的现有行为不在本场景重测；`absorb-control-under-immune` 保持不动。

### 6.3 客户端

`openspec validate` 与回归不能证明 1.27a 上 `ACsp` 真的锁住命令卡，也不能证明 `GetWorldBounds` 枚举到了编辑器预置物。归档前默认要有一次真实客户端检查；审核若声明非阻塞，`summary.md` 写明未跑与「空地图 / 物编改过 ACsp」两类剩余风险。

## 7. 备选（不采用）

| 方案 | 不采用原因 |
|---|---|
| 给每个预置单位补发 `UnitCreateNativeRequest` | 会再次 `CreateUnit`，地图上出现副本，并触发第二次 `Register` |
| 改 `Register` 为「已存在则拒绝」 | 改变创建路径的覆盖契约，超出本变更；防双登记放在 adoption 入口即可 |
| 调用 `DzUnitSilence` 并用 try/catch 兜底 | 1.27a 上是空指针，不是托管异常；catch 接不住，也违反不吞异常的规则 |
| 用 `PauseUnit` 实现沉默 | 冻结移动与攻击，和 `Pause` 合成冲突 |
| 为 `Locust` 等四个类型在本变更做模板 ability | 归档与现有注释都把它留给地图模板；做了就是范围扩大 |

## 8. 回滚

删除 `War3Init` 中的 `AdoptAll` 调用、`PresetNativeTag` / `DestructableNative` / `PresetEntityAdoption`，以及 `Silence` case。`NativeEntityIndex` 与其余 case 不变。无存档、无生成代码、无协议数据。

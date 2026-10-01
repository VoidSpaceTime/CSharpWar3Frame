# 设计：单位复活与单位池

Change ID: `unit-reborn-and-pool`  
等级: `full`  
状态: 待审核

## 1. 现状（行号以 2026-10-01 工作树为准）

### 1.1 阶段与字段

- `UnitLifecyclePhase`：`Alive`、`Death`、`Corpse`、`ClearCorpse`、`Remove`、`Disposing`、`RebornPending`、`Pooled`（`War3Frame/Src/Components/Unit/UnitLifecycle.cs:8-18`）。后两个只有注释「待实现」。
- `UnitLifeState`：`isAlive`、`rebornTime`、`lifePhase`（同文件 :23-28）。`rebornTime` 无读取方。
- 创建入口把 `rebornTime` 留默认 0（`UnitHelper.CreateUnit`，`UnitHelper.cs:26-30`）。

### 1.2 三系统顺序

同一 Immediate 根内：

1. `UnitRemoveNativeSystem` order 10：`Death` → `JassApi.KillUnit`（有 `UnitNative` 时）；`Remove` → `Unregister` + `HandleRemove` + `RemoveUnit` + 写成 `Disposing`（`UnitRemoveNativeSystem.cs:14-68`）。
2. `UnitLifecycleTransitionSystem` order 20：`Death → Corpse`，`ClearCorpse → Remove`（`UnitLifecycleTransitionSystem.cs:14-34`）。只改 `ref` 字段，无原生调用。
3. `UnitLifecycleDisposeSystem` order 30：仅 `Disposing` 调用 `CleanupFinalizeEntityDispose`（`UnitLifecycleDisposeSystem.cs:14-34`，`UnitHelper.cs:244-252`）。

`TimerTaskSystem` 是 Interval 0.05s（`TimerTaskSystem.cs:10-16`）。`CorpseCleanup` 只有 owner 仍为 `Corpse` 时才写成 `ClearCorpse`（:52-59）。

### 1.3 句柄配对（必须保持单点）

- 创建：`JassApi.CreateUnit` 下一行 `HandleHelper.HandleAdd`，再 `NativeEntityIndex.Register`（`UnitCreateNativeSystem.cs:30-32`）。
- 销毁：仅 `Remove` 分支，`Unregister` 后 `HandleRemove` 再 `RemoveUnit`（`UnitRemoveNativeSystem.cs:57-60`）。
- `Death` 的 `KillUnit` **不**注销句柄。这是复活能成立的前提：尸体阶段原生单位已死但句柄仍被引用。

### 1.4 与「单位池 / 物品池」API 的差别

`CreateUnitPool`（`JassApi.cs:8653`）和 `CreateItemPool`（:8700）是随机类型池。全仓调用点只有定义本身。`Pooled` 不调用它们。

`JassApi.RemoveUnit` 注释：删除后不留尸体，英雄不能再复活（:4563-4568）。所以 `RebornPending` / `Pooled` 禁止进入 `Remove`，除非调用方明确放弃。

`ReviveHero`（`JassApi.cs:5092`）存在，但 `UnitBase`（`Units.cs:46-50`）没有英雄标记。本设计不调用它。

`ControlType.Hide` 已通过 `JassApi.ShowUnit` 做显隐（`UnitNativeRequestSystems.cs:42-43`）。复活/入池不复用控制请求，避免和眩晕/隐藏计数耦合；单独发一次性 Native 请求。

## 2. 状态模型

`RebornPending` 与 `Pooled` 都是 `UnitLifecyclePhase` 的值，不是新 Tag，也不是事件。

| 阶段 | `isAlive` | 实体 | `UnitNative` | 句柄引用 | 谁可以选中 |
|---|---|---|---|---|---|
| `RebornPending` | false | 保留 | 保留 | 保留 | 否（`isAlive=false`） |
| `Pooled` | false | 保留 | 保留 | 保留 | 否 |
| `Remove` / `Disposing` | false | 即将删除 | 销毁分支注销 | `HandleRemove` | 否 |

`rebornTime` 语义：

- `<= 0`：不复活。尸体到期后走今天的 `ClearCorpse → Remove`。
- `> 0`：尸体到期后进入 `RebornPending`，并把该值当作复活等待秒数。等待期间字段继续表示剩余时间，归零后清 0 并回到 `Alive`。它不是每帧自动衰减的「当前剩余」，直到进入 `RebornPending` 才开始被计时器消费。这样现有「只在创建时写一次」的用法不会在存活期间被减掉。

不新增 `RebornPendingTag` / `PooledTag`。阶段已经是零数据分类；剩余时间放在已有字段里。

## 3. 复活协作

### 3.1 进入

`KillUnit`（`UnitHelper.cs:81-111`）保持 Alive→Death、`UnitDiedEvent`、尸体 `TimerTask`。不在 Helper 里读表决定复活，只把已经写在 `UnitLifeState.rebornTime` 上的值留下。

尸体到期（现有 `TimerTaskSystem` 把 `Corpse` 写成 `ClearCorpse`）之后，`UnitLifecycleTransitionSystem`：

- `rebornTime <= 0`：`ClearCorpse → Remove`（现状）。
- `rebornTime > 0`：`ClearCorpse → RebornPending`，并挂一条新的 `TimerTask`：`mode=Once`，`kind=RebornWait`，`interval=remaining=rebornTime`，`owner=单位`。

`RebornWait` 加在 `TimerTaskKind`（`Time.cs:24-50`）。`TimerTaskSystem` 到期时只把 owner 从 `RebornPending` 推进到 `Alive`（`isAlive=true`，`rebornTime=0`），并挂 `UnitRebornNativeRequest`。不调用原生 API。

为什么不让 Transition 自己减 `deltaTime`：Transition 是 Immediate，没有独立间隔；仓库计时真相在 `TimerTask`。

### 3.2 Native 复活请求

新增 `UnitRebornNativeRequest : IComponent`，挂在单位上，字段仅 `show`（bool，复活为 true）。

消费系统放在 `Systems/Native`，`[SystemRegister(SystemKind.Immediate)]`，命名 `UnitRebornNativeSystem`：

- 无 `UnitNative`：只删请求（本地验证宿主走这条）。
- 有 `UnitNative`：`JassApi.SetUnitState` 把生命设为当前生命上限（读 ECS 属性最终值，不读原生当前生命当真相），再 `JassApi.ShowUnit(unit, true)`。
- **不** `CreateUnit`、**不** `HandleAdd`、**不** `HandleRemove`、**不** `RemoveUnit`、**不** `ReviveHero`。

请求是一次性副作用，消费后删除。生命上限的 ECS 真相仍由属性系统持有；这里只是把原生代理拉回存活表现。

### 3.3 三系统守卫

- `UnitRemoveNativeSystem`：`RebornPending` 与 `Pooled` 落入现有 if/else 的隐式忽略即可，但必须加注释断言这两阶段不得并入 `Death` 或 `Remove` 分支。禁止为它们调用 `KillUnit`（死亡当帧已经杀过；再杀无句柄语义）。
- `UnitLifecycleDisposeSystem`：条件保持「仅 `Disposing`」。不要用「非 Alive 就删」。
- `UnitLifecycleTransitionSystem`：`RebornPending` 不再转发。转发只发生在进入该阶段的那一次 `ClearCorpse`。

### 3.4 取消复活

`RebornPending` 期间调用现有 `RemoveUnit` 入口时，允许 `RebornPending → Remove`（扩展 `UnitHelper.RemoveUnit` 今天只接受 `Alive`/`Corpse` 的判断，`UnitHelper.cs:63-66`）。随后走现有销毁与句柄注销。同时移除未到期的 `RebornWait`。不广播第二条 `UnitDiedEvent`。

## 4. 单位池协作

### 4.1 语义

`Pooled` 表示「该 ECS 实体连同其 `UnitNative` 被挂起，等待同一实体被再次启用」。不是对象分配器，也不是 `JUnitPool`。

入池入口 `UnitHelper.RequestPool(Entity unit)`（薄入口，无原生调用）：

- 允许的源阶段：`Alive`、`RebornPending`。
- `Alive`：`isAlive=false`，`lifePhase=Pooled`。不经过 `Death`，因此**不**广播 `UnitDiedEvent`，也**不** `KillUnit`。
- `RebornPending`：取消 `RebornWait`，`rebornTime=0`，进入 `Pooled`。死亡事件已经在当初 `KillUnit` 广播过，不重复。
- 其它阶段：拒绝，保持原 phase。
- 入池成功后挂 `UnitRebornNativeRequest { show = false }`，由同一 Native 系统 `ShowUnit(false)`。无句柄时请求仍被消费删除。

出池入口 `UnitHelper.AcquirePooled(Entity unit)`：

- 仅 `Pooled` 成功。
- 写成 `Alive`、`isAlive=true`、`rebornTime` 保持调用前的值（入池时已清 0；若调用方在出池前重新写入 `rebornTime`，保留，供下次死亡使用）。
- 挂 `UnitRebornNativeRequest { show = true }`，复用第 3.2 节的回血与显示。不 `CreateUnit`。

放弃入口 `UnitHelper.DiscardPooled(Entity unit)`：

- 仅 `Pooled` 成功，写成 `Remove`。
- 之后完全复用 `UnitRemoveNativeSystem` 的唯一销毁点。这是池内单位唯一合法的 `HandleRemove` 路径。

### 4.2 不变量

- 池内实体不 `DeleteEntity`，故 `CleanupFinalizeEntityDispose` 不跑，物品/技能/属性链接保持。出池后这些链接仍在。调用方若要在入池前清空背包，用现有物品 API，本 change 不隐式 `Detach`。
- 不设全局空闲表。复用方必须仍持有该 Entity。避免在本 change 引入无主的句柄仓库。
- 不调用 `CreateUnitPool` / `PlaceRandomUnit`。

## 5. 时序

```text
Immediate 10  UnitRemoveNativeSystem
              Death → KillUnit（句柄保留）
              Remove → HandleRemove + RemoveUnit + Disposing
              RebornPending / Pooled → 无操作

Immediate 20  UnitLifecycleTransitionSystem
              Death → Corpse
              ClearCorpse + rebornTime<=0 → Remove
              ClearCorpse + rebornTime>0  → RebornPending + 挂 RebornWait

Interval      TimerTaskSystem（0.05s）
              CorpseCleanup 到期 → ClearCorpse（仅当前是 Corpse）
              RebornWait 到期 → Alive + UnitRebornNativeRequest

Immediate *   UnitRebornNativeSystem
              消费请求：SetUnitState + ShowUnit；不碰句柄引用

Immediate 30  UnitLifecycleDisposeSystem
              仅 Disposing → CleanupFinalizeEntityDispose
```

`UnitRebornNativeSystem` 用默认 order 1，排在 Remove(10) 之前。请求只在 Timer 或 Helper 写成 `Alive`/`Pooled` 之后的下一帧 Immediate 出现，不需要排在 Dispose 之后。

结构变更：Transition 在 Query 内只写 `ref UnitLifeState`（与今天 `Death → Corpse` 相同）。挂 `TimerTask` / `UnitRebornNativeRequest` 会增组件，必须收集到循环外 `AddComponent`。`TimerTaskSystem` 已有循环外写回（`TimerTaskSystem.cs:78-107`），新 kind 走同一列表，不在 `ForEachEntity` 内 `AddComponent`。

## 6. 方案比较

| 方案 | 结论 |
|---|---|
| A. 本设计：阶段旁路 + 保留句柄 | 采用。对齐已有枚举占位，销毁点仍只有一处 |
| B. 复活时 `RemoveUnit` 再 `CreateUnit` | 拒绝。多一次 `HandleRemove`/`HandleAdd`，实体 id 可保留但原生身份断裂，且 `RemoveUnit` 注释写明不能再复活 |
| C. 用 `JUnitPool` 当对象池 | 拒绝。该 API 是类型权重抽取，会 `PlaceRandomUnit` 出新句柄 |
| D. 复活与池拆成两个 change | 不默认。`ClearCorpse` 分支和「禁止 Dispose」要写两遍。用户仍可按 proposal 第 7 节拆 |

## 7. 回滚

1. 代码回滚：去掉 `RebornWait`、两个 Helper 入口、`UnitRebornNativeRequest` 与其系统，Transition 恢复无条件 `ClearCorpse → Remove`。
2. 数据回滚：不需要。`rebornTime` 默认 0；不写它的单位路径不变。已处于 `RebornPending`/`Pooled` 的进程内实体随地图重载消失，无存档格式。
3. 句柄回滚：不新增第二销毁点，回滚不会留下「只 Add 不 Remove」的新配对。
4. 规格回滚：change 未归档前删除 `openspec/changes/unit-reborn-and-pool/` 即可；归档后用后续 change 做 `REMOVED`，不手改已合并 spec 来假装没发生。

## 8. 验证策略（批准后才执行）

无 War3 句柄的本地场景（对齐 `UnitLifecycleValidationScenario` 的做法：不挂 `UnitNative`，Native 分支跳过）：

1. **复活倒计时**：`rebornTime=1`，Death→Corpse→（模拟尸体到期）→`RebornPending`；推进 `RebornWait` 后 phase=`Alive`、`isAlive=true`、实体未删、无第二条 `UnitDiedEvent`。`rebornTime=0` 的对照仍被 Dispose 删除。
2. **池重用**：`RequestPool` 后 phase=`Pooled` 且实体仍在；`AcquirePooled` 后 `Alive` 且 Entity.Id 不变；`DiscardPooled` 后走 `Remove` 并被 Dispose 删除。入池不产生 `UnitDiedEvent`。
3. **句柄配对**：用测试替身或源码断言固定调用点——复活/出池路径的 Native 系统不得出现 `HandleAdd`/`HandleRemove`/`CreateUnit`/`RemoveUnit`；`DiscardPooled` 后的移除仍只经过 `UnitRemoveNativeSystem` 现有三行（Unregister、HandleRemove、RemoveUnit）。

`dotnet build War3Frame/War3Frame.csproj`。不把真实客户端复活动画列为归档阻塞项。

## 9. 实施注意

- 中文 XML 注释只写职责：新系统、新请求、新 Helper 方法各一句。
- 不在 Transition、Timer、Helper 里写 `JassApi`。
- 不改 `EventCleanupSystem` order 132 契约；本 change 不新增 `XxxEvent`。

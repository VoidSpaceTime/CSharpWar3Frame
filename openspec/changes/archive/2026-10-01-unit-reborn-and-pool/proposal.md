# 提案：单位复活与单位池（unit-reborn-and-pool）

## 元信息

- **Change ID**：`unit-reborn-and-pool`
- **提案等级**：`full`
- **状态**：`待审核`
- **日期**：2026-10-01
- **目标一句话**：把已声明但未实现的 `RebornPending`（消费 `rebornTime` 的复活等待）与 `Pooled`（ECS 实体复用）接入现有生命周期三系统，并保持句柄配对与「移除 ≠ 死亡」契约。
- **请求来源**：`.omo/plans/war3frame-unlanded-features.md` Todo 19（W4.3）。
- **默认实施后审查强度**：`R2 Targeted`
- **命中的审查升级触发器**：核心生命周期路径；跨 Transition / Native / Dispose / Helper；原生句柄配对。
- **最终实施后审查强度**：`R2 Targeted`（不升 `R3`：非安全敏感、非架构级基础设施替换）
- **完整 `review-work` 授权来源**：无
- **拆分裁决**：默认合并推进。设计评审认为复活与池复用共享同一条「不进 `Disposing`」旁路，宜留在同一 change；若用户要求拆分，见第 7 节。

### 0.1 工件矩阵

- `full`：`proposal.md`、`design.md`、`tasks.md`、`specs/unit-reborn-and-pool/spec.md`

---

## 1. 分级判定

- 影响范围：`War3Frame/` 生命周期阶段、计时种类、Native 移除旁路、终态清理守卫、薄入口；`Projects/` 仅新增回归场景。
- 风险等级：中高。错误地把复活/入池推进到 `Remove` 会 `RemoveUnit` + `HandleRemove` 并删除实体，复活与复用都无法成立。
- 可逆性：高。删除新阶段分支与新入口后，现有 `Alive → Death → Corpse → ClearCorpse → Remove → Disposing` 主路径不变。
- 是否跨项目：运行时在 `War3Frame/`，验证场景在 `Projects/`。
- 是否改公共契约：是。`UnitLifecyclePhase` 两个占位值获得行为；`UnitLifeState.rebornTime` 开始被消费；新增复活/入池/出池入口。

升级触发器：核心业务流程 + 跨系统协作 + 公共生命周期契约 → `full` / `R2`。不改生成器输出、构建链或 CLI，不升 `architecture`。

---

## 2. 背景 / Why（2026-10-01 代码核实）

`UnitLifecyclePhase` 已声明两个占位，没有任何系统消费：

```16:17:War3Frame/Src/Components/Unit/UnitLifecycle.cs
    RebornPending, // 复活等待 — 待实现
    Pooled         // 单位池   — 待实现
```

`UnitLifeState.rebornTime`（同文件 :26）全仓无读取方。

现有主路径（方案 A，已落地）：

| 阶段 | 谁推进 | 副作用 |
|---|---|---|
| `Alive → Death` | `UnitHelper.KillUnit`（`UnitHelper.cs:81-111`） | 写 `isAlive=false`、挂 `TimerTaskKind.CorpseCleanup`、广播一条 `UnitDiedEvent` |
| `Death → Corpse` | `UnitLifecycleTransitionSystem` order 20（`UnitLifecycleTransitionSystem.cs:21-26`） | 无原生调用 |
| `Death` 当帧 | `UnitRemoveNativeSystem` order 10（`UnitRemoveNativeSystem.cs:37-47`） | 有 `UnitNative` 时 `JassApi.KillUnit`；**不** `HandleRemove` |
| `Corpse → ClearCorpse` | `TimerTaskSystem`（`TimerTaskSystem.cs:52-59`）仅当 phase 仍是 `Corpse` | 无原生调用 |
| `ClearCorpse → Remove` | `UnitLifecycleTransitionSystem`（:29-33） | 无原生调用 |
| `Remove` | `UnitRemoveNativeSystem`（:49-67） | `NativeEntityIndex.Unregister` → `HandleHelper.HandleRemove` → `JassApi.RemoveUnit`，然后 phase=`Disposing` |
| `Disposing` | `UnitLifecycleDisposeSystem` order 30 | `UnitHelper.CleanupFinalizeEntityDispose`（`UnitHelper.cs:244-252`）删除实体 |

`JassApi.RemoveUnit` 注释写明「被删除的单位不会留下尸体。如果是英雄则不能再被复活」（`JassApi.cs:4563-4568`）。因此复活单位**不能**进入 `Remove`。

`JassApi.CreateUnitPool` / `PlaceRandomUnit`（`JassApi.cs:8650-8694`）与 `CreateItemPool` / `PlaceRandomItem`（:8697-8741）都是 War3 **随机类型抽取池**，全仓无调用方。本提案的 `Pooled` 是 **ECS 实体复用**，不是这套 JASS API。

---

## 3. 变更范围 / What

### 3.1 做

1. **`RebornPending`**：`rebornTime > 0` 的单位在尸体阶段结束时进入复活等待，而不是 `Remove`。倒计时归零后同一 ECS 实体回到 `Alive`，保留 `UnitNative` 与 `HandleAdd` 引用，由 Native 层执行复活副作用。
2. **`Pooled`**：显式入池请求把仍存活、或已在 `RebornPending` 的单位挂起为可复用实体。出池复用同一实体与同一原生句柄；放弃池内单位才走现有 `Remove → Disposing` 销毁。
3. **与三系统协作**：`UnitRemoveNativeSystem` 对这两个阶段不 `KillUnit`、不 `RemoveUnit`、不 `HandleRemove`。`UnitLifecycleDisposeSystem` 不删除它们。`UnitLifecycleTransitionSystem` 只在「无复活」时把 `ClearCorpse` 推进到 `Remove`。
4. **句柄配对**：创建仍只在 `UnitCreateNativeSystem`（`HandleAdd` 紧挨 `CreateUnit`，`UnitCreateNativeSystem.cs:30-31`）。销毁仍只在 `UnitRemoveNativeSystem` 的 `Remove` 分支（`HandleRemove` 紧挨 `RemoveUnit`，`UnitRemoveNativeSystem.cs:57-60`）。复活与出池不新增销毁点。
5. **回归场景**（具名，见 `tasks.md`）：复活倒计时、池重用、句柄配对。

### 3.2 非目标

- 不实现 War3 `CreateUnitPool` / `PlaceRandomUnit` 随机刷怪。
- **不纳入物品池**。`CreateItemPool` / `JItemPool` 与单位复活、ECS 单位复用无共享状态；见第 6 节。
- 不改 `UnitDiedEvent`：死亡当次仍广播一条；复活不补发死亡，也不把移除当成死亡。
- 不在 Helper / Transition / Dispose 中调用 `JassApi` / `KKApi` / `YDApi` / `DzApi`。
- 不改 `War3Frame.Generator`、`FrameBuild`、`CSharpWar3Frame`。
- 不新增英雄/非英雄双路径。仓库没有英雄标记；复活统一为「保留句柄 + Native 回血/显示」。`ReviveHero`（`JassApi.cs:5092`）留待后续有英雄契约时再接，本 change 不调用它。

---

## 4. 全局影响分析

- `War3Frame/`：受影响。生命周期组件、计时种类、三个生命周期系统、`UnitHelper` 薄入口、必要时一个复活 Native 请求消费系统。
- `War3Frame.Generator/`：不受影响。新系统若新增，只加 `[SystemRegister]`，不改生成器。
- `FrameBuild/`：不受影响。不改构建、模板、JIT/AOT。
- `CSharpWar3Frame/`：不受影响。不改 CLI。
- `Projects/`：受影响的只有回归/验证场景。`demo` 不改。现有 `UnitLifecycleValidationScenario` 的 Death→删除断言保持，因为它不设置 `rebornTime`。

---

## 5. 方案摘要

```text
KillUnit（rebornTime > 0）
  → Death（Native: KillUnit，句柄保留）
  → Corpse
  → 尸体计时到期且 rebornTime > 0 → RebornPending（不进 Remove）
  → 复活倒计时归零 → Alive + UnitRebornNativeRequest
  → Native：SetUnitState 回满生命、ShowUnit(true)；不 HandleRemove、不 CreateUnit

RequestPool（Alive 或 RebornPending）
  → Pooled（取消复活计时；不 RemoveUnit）
  → Acquire：同一实体 + 同一 UnitNative → Alive + 复活 Native 请求
  → Discard：Pooled → Remove → 现有 HandleRemove + RemoveUnit + Disposing
```

`rebornTime <= 0`（含默认 0）保持今天的尸体清理删除路径。

细节、回滚与顺序见 `design.md`。

---

## 6. 物品池建议：不纳入

| | JASS `JItemPool` | 本 change 的 `Pooled` |
|---|---|---|
| 含义 | 按权重随机抽**物品类型**并 `PlaceRandomItem` | 挂起已有**单位实体**以便复用 |
| 句柄 | 池句柄 + 新物品句柄 | 复用已 `HandleAdd` 的单位句柄 |
| 调用方 | 无（定义仅在 `JassApi.cs:8697-8741` 与 `JassType.cs:373`） | 尚无，本提案定义 |

建议：**本 change 排除物品池**。它与 `rebornTime`、`UnitRemoveNativeSystem`、单位句柄配对没有共同不变量。若将来要做随机掉落，应单独立项，且不得把 `JItemPool` 当成物品实体对象池。

---

## 7. 拆分或合并（请用户裁决）

**默认：合并推进**（一个 change、一次批准、一条生命周期旁路）。

设计评审结论：二者都是「离开死亡主路径但保留实体与句柄」。拆开会让 `ClearCorpse` 的分支所有权分属两个 change，回滚也要分两次。合并的代价是验收场景多三个，仍在同一模块内。

若用户选择拆分，建议：

1. `unit-reborn`：只做 `RebornPending` 与倒计时。
2. `unit-pool`：只做 `Pooled` 入池/出池/放弃，依赖复活 Native 请求或自带等价的「重新显示」请求。

未批准前不实现，也不预先建第二个 change。

---

## 8. 风险与回滚

- **风险**：复活单位被误推进到 `Remove`，句柄被注销，实体被删。缓解：Transition 在 `rebornTime > 0` 时禁止 `ClearCorpse → Remove`；Dispose 拒绝非 `Disposing`。
- **风险**：池内单位仍被攻击/移动系统选中。缓解：`Pooled` 与 `RebornPending` 的 `isAlive=false`，现有存活过滤（`TargetFilterRegistry` 读 `isAlive`）将其排除；不在本 change 改筛选器。
- **风险**：`ShowUnit` 与 `ControlType.Hide` 抢原生显隐。缓解：复活只在离开 `RebornPending`/`Pooled` 时发一次显示请求；不改控制状态机。
- **回滚**：撤回本 change 的阶段分支与新入口。不写 `rebornTime`、不发入池请求的单位行为与今天相同。不迁移存档（无持久化）。

---

## 9. 验收标准

- `openspec validate unit-reborn-and-pool` 通过。
- 用户批准前无运行时代码改动。
- 批准后：三个具名场景通过；`dotnet build War3Frame/War3Frame.csproj` 0 错误；现有无 `rebornTime` 的死亡删除场景仍通过。
- 真实 War3 客户端上的 `KillUnit`/`SetUnitState`/`ShowUnit` 表现不作为本提案归档阻塞项；场景在无句柄宿主验证阶段与句柄调用点，客户端验证留在实施 summary 记录。

---

## 10. 开放问题

1. 复活坐标：默认留在死亡位置（不传新 x/y）。若需要祭坛坐标，批准时说明，否则不做。
2. 入池是否隐藏模型：默认发一次 `ShowUnit(false)` 请求，出池再 `ShowUnit(true)`。若希望池内单位仍可见，批准时否决。
3. 池容量与按 `templateName` 分桶：默认不做上限、不做分桶，调用方自选实体。上限留给后续 change。

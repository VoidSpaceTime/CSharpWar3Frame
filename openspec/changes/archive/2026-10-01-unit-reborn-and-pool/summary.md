# 总结：unit-reborn-and-pool

## 范围

批准依据：`.omo/evidence/gates/user-ruling-2026-10-01.md` 第 5 项（W4，含 19 复活与池，批准实施）。本总结不自证批准。

本变更把 `UnitLifecyclePhase` 中两个占位值接入现有生命周期三系统，不新增第二套生命周期：

- **复活等待**：`UnitLifecycleTransitionSystem`（order 20）在 `ClearCorpse` 且 `UnitLifeState.rebornTime > 0` 时转入 `RebornPending`，并在循环外挂 `mode=Once`、`kind=RebornWait` 的 `TimerTask`（`interval=remaining=rebornTime`、`owner=单位`）；`rebornTime <= 0` 仍进入 `Remove`。`TimerTaskSystem` 新增 `RebornWait` 分支：到期且 owner 仍为 `RebornPending` 时写回 `Alive`（`isAlive=true`、`rebornTime=0`）并挂一次性 `UnitRebornNativeRequest { show=true }`，不调用原生 API。
- **原生复活/显隐**：新增 `UnitRebornNativeRequest : IComponent`（仅 `show`）与 `UnitRebornNativeSystem`（`Systems/Native`，Immediate）。有 `UnitNative` 时按 ECS 生命属性最终值把原生生命拉满并 `ShowUnit(unit, show)`；无句柄只消费删除请求。不 `CreateUnit`、不 `HandleAdd`、不 `HandleRemove`、不 `RemoveUnit`、不 `ReviveHero`。
- **单位池**：`UnitHelper.RequestPool`（`Alive`/`RebornPending` → `Pooled`，取消未到期 `RebornWait`，写 `show=false` 请求，不广播 `UnitDiedEvent`）；`AcquirePooled`（仅 `Pooled` → `Alive`，保留 `Entity.Id` 与 `UnitNative`，写 `show=true` 请求，`rebornTime` 保持不变）；`DiscardPooled`（仅 `Pooled` → `Remove`，唯一销毁点仍是 `UnitRemoveNativeSystem` 的现有三行）。
- **协作边界**：`UnitRemoveNativeSystem` 对 `RebornPending`/`Pooled` 不 `KillUnit`、不注销句柄、不 `RemoveUnit`（加守卫注释）；`UnitLifecycleDisposeSystem` 仍只认 `Disposing`。`UnitHelper.RemoveUnit` 扩展允许从 `RebornPending` 进入 `Remove`，并移除未到期的 `RebornWait`。

按 design 决策：不纳入物品池（`CreateItemPool`/`JItemPool` 是 JASS 随机类型池）；池不设容量上限、不按 `templateName` 分桶；复活默认留在死亡位置；入池 `ShowUnit(false)`、出池再显示。

## 全局影响

| 区域 | 是否受影响 | 理由 |
|---|---|---|
| `War3Frame/` | 是 | 生命周期阶段、计时种类、Transition/Timer 推进、新增 Native 请求与系统、`UnitHelper` 薄入口。 |
| `War3Frame.Generator/` | 否 | 新系统只用 `[SystemRegister]`，不改生成器与生成输出契约。 |
| `FrameBuild/` | 否 | 不改构建、模板、WE、JIT/AOT。 |
| `CSharpWar3Frame/` | 否 | 不改 CLI。 |
| `Projects/` | 是 | `Projects/Regression` 增加三个 `runtime` 场景；`Projects/test` 现有 `UnitLifecycleValidationScenario` 无 `rebornTime` 路径不变。 |

新增路径无 `JassApi` / `KKApi` / `YDApi` / `DzApi` 出现在 Helper / Transition / Timer 层；原生调用只在 `UnitRebornNativeSystem`（Native 层）。

## 验证

- `dotnet build War3Frame/War3Frame.csproj -c Release`：0 错误（`exit=0`，见 `.omo/evidence/task-19-build.txt`）。
- `dotnet run --project Projects/Regression -c Release`：Executed 66; passed 66; failed 0（见 `.omo/evidence/task-19-regression.txt`）。基线 63 → 66。
- 新增具名场景：`runtime/复活倒计时`、`runtime/池重用`、`runtime/句柄配对`，均 PASS。既有 `existing/UnitLifecycleValidationScenario` 等 63 项仍全绿。
- 真实 War3 客户端上的 `KillUnit`/`SetUnitState`/`ShowUnit` 表现未在本回归执行；按批准范围不作为归档阻塞项，客户端验证留在本总结记录。

## R2 专项视角

### 1. 复活状态机

`Alive → Death → Corpse → ClearCorpse`（`TimerTaskSystem` 把 `Corpse` 写成 `ClearCorpse`）。`ClearCorpse` 是分叉点：`rebornTime > 0` → `RebornPending` 并在同一次 Transition 内调度 `RebornWait`，绝不进入 `Remove`；`<= 0` → 维持 `Remove`。`RebornWait` 只在 owner 仍为 `RebornPending` 时生效，之后的 `Alive` 复位清 `rebornTime=0` 并移除遗留的 `TimerExpired` 标记。`runtime/复活倒计时` 验证：正数经 `RebornPending` 回到同实体 `Alive`、实体未删、只广播一条 `UnitDiedEvent`；`rebornTime=0` 的对照经 `Remove → Disposing` 被终态删除。

### 2. 池句柄配对

创建仍只在 `UnitCreateNativeSystem`（`JassApi.CreateUnit` 后相邻 `HandleHelper.HandleAdd`），销毁仍只在 `UnitRemoveNativeSystem` 的 `Remove` 分支（`NativeEntityIndex.Unregister` → `HandleHelper.HandleRemove` → `JassApi.RemoveUnit`）。复活与出池只发 `UnitRebornNativeRequest`，不 `CreateUnit`/`RemoveUnit`/`HandleAdd`/`HandleRemove`；`DiscardPooled` 走同一 `Remove` 分支。`runtime/句柄配对` 以源码断言固定：复活系统不含五个禁用符号；创建 `HandleAdd`/`CreateUnit` 各 1；移除 `HandleRemove`/`JassApi.RemoveUnit` 各 1 且三者顺序正确；`UnitHelper` 不含 `JassApi`/`HandleHelper`。

### 3. 与 Remove / Dispose 的协作边界

`UnitRemoveNativeSystem` 只识别 `Death` 与 `Remove`；`RebornPending`/`Pooled` 落在 else 之外，既不重复 `KillUnit`，也不注销句柄（加了显式守卫注释）。`UnitLifecycleDisposeSystem` 仍只删除 `Disposing`，因此等待/池内实体不会被误删。`RemoveUnit` 从 `RebornPending` 转入 `Remove` 时先取消 `RebornWait`，避免计时器在销毁后继续触发；不广播第二条死亡事件。`runtime/池重用` 验证入池/出池保留 `Entity.Id` 且全程 0 条 `UnitDiedEvent`，放弃后经唯一销毁路径删除。

## 风险

- 复活回满采用既有 `UnitNativeSystem` 的 0..10000 归一化约定（满生命 = 10000），与 `SetUnitState` 的实际取值语义是否一致属既有投影约定，本变更沿用未改。
- 池内实体不 `DeleteEntity`，物品/技能/属性链接保持；若调用方需清空背包，须在入池前显式用物品 API，本变更不隐式 `Detach`。
- 池不设全局空闲表，复用方必须仍持有该 `Entity`；本变更未引入无主句柄仓库，容量上限与按 `templateName` 分桶留待后续变更。
- `Pooled`/`RebornPending` 的 `isAlive=false` 依赖既有存活过滤排除选中；本变更未改目标筛选器。
- 真实客户端复活动画与原生显隐未验证，仅覆盖 ECS 语义与调用点。

## 回滚

去掉 `RebornWait`、`UnitRebornNativeRequest` 与其系统、三个池入口与 `RemoveUnit` 的 `RebornPending` 分支，把 Transition 恢复为无条件 `ClearCorpse → Remove`。`rebornTime` 默认 0，不写它的单位路径与本变更前一致；无持久化格式需要迁移；不新增第二销毁点。

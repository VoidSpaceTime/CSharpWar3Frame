# 任务：unit-reborn-and-pool

状态：待审核。批准前不得勾选实现项，不得改 `War3Frame/` 运行时代码。

## 0. 提案

- [x] 0.1 用 `openspec new change unit-reborn-and-pool` 建立 change
- [x] 0.2 写 `proposal.md` / `design.md` / `tasks.md` / `specs/unit-reborn-and-pool/spec.md`
- [x] 0.3 `openspec validate unit-reborn-and-pool` 通过
- [ ] 0.4 用户批准（未批准则停止）

## 1. 复活等待（批准后）

- [ ] 1.1 `TimerTaskKind` 增加 `RebornWait`
- [ ] 1.2 `UnitLifecycleTransitionSystem`：`ClearCorpse` 且 `rebornTime > 0` 时进入 `RebornPending` 并在循环外挂 `RebornWait`；`rebornTime <= 0` 仍进入 `Remove`
- [ ] 1.3 `TimerTaskSystem`：`RebornWait` 到期且 owner 仍为 `RebornPending` 时写成 `Alive`（`isAlive=true`，`rebornTime=0`）并挂 `UnitRebornNativeRequest { show=true }`
- [ ] 1.4 新增 `UnitRebornNativeRequest` 与 `UnitRebornNativeSystem`（Native 层）：有 `UnitNative` 时 `SetUnitState` 回满生命并 `ShowUnit(true)`；无句柄只删请求
- [ ] 1.5 `UnitHelper.RemoveUnit` 允许从 `RebornPending` 进入 `Remove`，并移除未到期的 `RebornWait`

## 2. 单位池（批准后）

- [ ] 2.1 `UnitHelper.RequestPool`：`Alive` 或 `RebornPending` → `Pooled`；取消复活计时；挂 `UnitRebornNativeRequest { show=false }`；不广播 `UnitDiedEvent`
- [ ] 2.2 `UnitHelper.AcquirePooled`：仅 `Pooled` → `Alive` + `show=true` 请求；Entity 与 `UnitNative` 不变
- [ ] 2.3 `UnitHelper.DiscardPooled`：仅 `Pooled` → `Remove`，销毁只走 `UnitRemoveNativeSystem` 现有 `HandleRemove` + `RemoveUnit`
- [ ] 2.4 确认 `UnitRemoveNativeSystem` / `UnitLifecycleDisposeSystem` 不处理 `RebornPending` 与 `Pooled`（Dispose 仍只认 `Disposing`）

## 3. 回归场景（批准后，具名）

- [ ] 3.1 场景「复活倒计时」：`rebornTime>0` 经 `RebornPending` 回到 `Alive` 且实体仍在；`rebornTime<=0` 仍被删除；复活不产生第二条 `UnitDiedEvent`
- [ ] 3.2 场景「池重用」：入池实体仍在且 `Pooled`；出池后同一 Entity.Id 为 `Alive`；放弃后实体被删除；入池无 `UnitDiedEvent`
- [ ] 3.3 场景「句柄配对」：复活与出池的 Native 路径不调用 `HandleAdd` / `HandleRemove` / `CreateUnit` / `RemoveUnit`；放弃后的注销仍只有 `UnitRemoveNativeSystem` 的 `Remove` 分支
- [ ] 3.4 `dotnet build War3Frame/War3Frame.csproj` 0 错误；现有 `UnitLifecycleValidationScenario` 无 `rebornTime` 路径仍通过

## 4. 收尾（实施完成且验证通过后）

- [ ] 4.1 写 `summary.md`（full：范围、验证、风险）
- [ ] 4.2 `openspec archive unit-reborn-and-pool --yes`
- [ ] 4.3 仅在用户要求时提交实现与归档

## 明确不做

- 不调用 `CreateUnitPool` / `PlaceRandomUnit` / `CreateItemPool` / `PlaceRandomItem` / `ReviveHero`
- 不把 `JItemPool` 纳入本 change
- 不新增英雄标记或第二套销毁点

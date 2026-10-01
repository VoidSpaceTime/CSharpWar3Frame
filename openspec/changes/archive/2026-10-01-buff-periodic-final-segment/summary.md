# 总结：Buff 周期尾段语义（buff-periodic-final-segment）

## 结论

- **生产代码零改动，行为未变**。核对后确认现有实现已满足设计：`BuffTickSystem`（order 39）按 `floor` 结算并把 `tickElapsed` 对齐到封顶后的 `elapsed`；`BuffDurationSystem`（order 40）仅在 `tickElapsed < elapsed` 时延迟回收；`BuffExpireSystem`（order 41）消费 `BuffExpired` 后删除。因此本变更的交付物是「强化后的主 spec + 两个具名回归场景 + 本总结」，不修改 `BuffSystem.cs`、`DurationSystem.cs`、`TimerTaskSystem.cs`。
- 批准依据：`.omo/evidence/gates/user-ruling-2026-10-01.md` 第 5 项 W4（23 Buff 尾段「批准实施（推荐）」）。
- 等级 `light`。复盘：closeout 要求 R2 三个专项视角（floor 语义 / 观察后回收顺序 / 续期与旁路不变式），较 `proposal.md` 原自评 `R1` 提升一级以覆盖第三个视角；按具体风险执行，未启用完整 `review-work`。

## 范围与全局影响

- `War3Frame/`：**不受影响**。原路径已实现尾段不补跳与「先观察后回收」，无需改动；三个 order 契约保持不变。
- `Projects/Regression`：唯一改动点。`DomainRegression.cs` 新增并注册两个具名场景。
- `War3Frame.Generator/`、`FrameBuild/`、`CSharpWar3Frame/`：**不受影响**。无新增系统/组件/Tag，不改生成器输出、构建链路与 CLI 契约。
- 未改 `DurationSystem` 的 `elapsed` 封顶、`RefreshCore` 的 `elapsed`/`tickElapsed`/`lastTick`、`TimerTaskSystem` 的 `TimerTaskKind.BuffExpire` 旁路。

## 验证（R0）

- 构建：`dotnet build War3Frame/War3Frame.csproj -c Release` → 0 error（exit 0）；`dotnet build Projects/Regression/Regression.csproj -c Release` → 0 error（exit 0）。3 条 `CS8629` 警告来自 `Projects/test/SkillPointUpgradeScenario.cs`，既有、与本变更无关。
- 回归：`dotnet run --project Projects/Regression -c Release` → **Executed 74; passed 74; failed 0**（baseline 72，新增 2）。
- 新增场景：
  - `runtime/buff-partial-final-segment`：寿命 0.35s、间隔 0.1s、推进 1s → `DamageRequest == 3`（`floor(0.35/0.1)`，尾段 0.05s 不补第 4 次），实体已回收。
  - `runtime/buff-reclaim-ordering`：周期 DoT 0.25s/0.1s → 同帧先结算 2 次再删除（若回收早于观察则 tick 不会发生）；随后无周期 `Stun(0.25s)` → 同帧直接删除且 `DamageRequest` 仍为 2（无 tick 行为）。
- 原有 `runtime/buff-final-ticks`、`runtime/buff-catchup`、`runtime/buff-refresh` 原断言未改弱，均通过。
- 证据：`.omo/evidence/task-23-build.txt`、`.omo/evidence/task-23-regression.txt`。

## R2 专项视角

### 视角 1：floor 尾段语义

- 证据：`BuffTickSystem` 使用 `Math.Floor((accumulated + 1e-7) / tickInterval)`，余数写回 `lastTick`；新场景断言 0.35s/0.1s = 3 次而非 4 次。
- verdict：**通过**。尾段不产生额外 tick，既有 0.25s/0.1s=2 次断言与新非整除断言一致，无向上取整或比例结算。

### 视角 2：观察后回收顺序

- 证据：新场景以 order 0/39/40/41 的 `SystemRoot` 运行，断言「2 次 tick 已发生」且「实体已删除」在同一更新内同时成立；若 order 40 先于 39，门槛会阻止删除。
- verdict：**通过**。回收不得早于观察；有周期行为同帧完成观察与回收，无周期行为不进入延迟。

### 视角 3：续期相位与 TimerTaskSystem 旁路不变式

- 证据：`git diff` 未触及 `DurationSystem.cs`、`BuffHelper.RefreshCore`、`TimerTaskSystem.cs`；`runtime/buff-refresh` 继续通过（续期后剩余约 5s、过期 Tag 清除）。
- verdict：**通过**。`elapsed`/`tickElapsed`/`lastTick` 保留语义未变，续期后相位维持现状；`TimerTaskKind.BuffExpire` 仍是独立旁路，未纳入延迟。

## 风险与剩余

- 新增场景使用 `SystemRoot`（同帧强制全部系统运行）以锁定 order 契约；真实 `TimedSystemRoot` 下 `DurationSystem`(0.02)/`BuffTickSystem`(0.05)/`BuffDurationSystem`(0.1) 节奏不同，若某帧只有回收系统到点而观察系统未到点，回收会推迟到下一次观察执行。这正是「先观察后回收」的语义，但跨帧的具体时延未单独断言。
- 未执行真实 War3 客户端验证（非本变更门禁，`proposal.md` 已声明）。
- 回滚：删除 `Projects/Regression/DomainRegression.cs` 中两个场景与注册即可；无生产代码与数据迁移。

## 1. 语义落地

- [x] 1.1 核对 `BuffTickSystem`：结算次数保持 `floor`，不足一个 `tickInterval` 的余数只留在 `lastTick`，不额外执行一次 `tickActionId`。验证：diff 不把余数改成向上取整或比例结算。
- [x] 1.2 核对 `BuffDurationSystem`：仅当 `tickInterval > 0`、`tickActionId` 非空且 `tickElapsed < elapsed` 时推迟 `BuffExpired`；`tickElapsed` 追上 `elapsed` 的同一次更新内必须打上 `BuffExpired`。验证：无周期行为的 Buff 不进入这条延迟。
- [x] 1.3 不改 `DurationSystem` 的 `elapsed` 封顶，不改 `RefreshCore` 的 `elapsed` / `tickElapsed` / `lastTick`，不改 `TimerTaskSystem`。验证：`git diff` 不出现这三个文件，除非审核后另有书面范围变更。

## 2. 回归场景

- [x] 2.1 在 `Projects/Regression` 新增具名场景 `runtime/buff-partial-final-segment`：周期 DoT 的寿命不是 `tickInterval` 整数倍，一次推进超过寿命。验证：`DamageRequest` 次数等于 `floor(寿命 / tickInterval)`，更新结束后 Buff 实体已删除。
- [x] 2.2 补具名场景 `runtime/buff-reclaim-ordering` 覆盖回收时序：周期 Buff 先完成完整间隔 tick 才回收，无周期 Buff（只有 `Duration`、没有有效 `tickInterval` 与 `tickActionId`）一次推进超过寿命。验证：周期路径在本帧先出现应结算的 tick 且实体随后删除；无周期路径更新结束后实体已删除，且没有为它产生 tick 行为。
- [x] 2.3 保留并复跑 `runtime/buff-final-ticks`、`runtime/buff-catchup`、`runtime/buff-refresh`。验证：原断言不改弱，三个场景仍通过。

## 3. 收口

- [x] 3.1 构建 `War3Frame/War3Frame.csproj` 与 `Projects/Regression/Regression.csproj`，确认 0 错误。真实 War3 客户端不作为本变更门禁。
- [x] 3.2 写 light 短段落 `summary.md`（尾段规则、回收时序、新增场景结果、未改的续期与计时任务、剩余风险），然后 `openspec archive buff-periodic-final-segment --yes`。验证：归档前 `openspec validate buff-periodic-final-segment` 通过，归档后主 spec 含本 delta。

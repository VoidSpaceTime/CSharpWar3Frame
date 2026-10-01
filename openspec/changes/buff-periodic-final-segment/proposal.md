# 提案：Buff 周期尾段语义

## Why

周期 Buff 的存活时钟和 tick 时钟已经分开，但到期时「最后一段不足一个间隔的存活时间」没有契约。`runtime/buff-final-ticks` 只锁住整除情形：0.25 秒寿命、0.1 秒间隔、一次推进 1 秒，断言恰好 2 次 `DamageRequest` 且实体已删除。0.25 秒里还有 0.05 秒尾段，现有断言不说明它该不该再跳一次，也不说明回收能不能抢在这次观察之前。

当前实现（行号以本提案起草时的树为准）：

- `DurationSystem`（`DurationSystem.cs:31-36`）把 `elapsed` 封顶在原 `remaining`，到期后打 `DurationExpired`。0.25 秒寿命得到 `elapsed == 0.25`，不会把帧外的 0.75 秒算进存活期。
- `BuffTickSystem`（`BuffSystem.cs:102-106`，order 39）只按 `floor(存活时间 / tickInterval)` 结算。0.25 / 0.1 = 2 次，余数 0.05 留在 `lastTick`，不产生第三次行为。同一次更新里它把 `tickElapsed` 写成 `duration.elapsed`。
- `BuffDurationSystem`（`BuffSystem.cs:27-33`，order 40）看到 `DurationExpired` 后，若 `tickInterval > 0`、`tickActionId` 非空且 `tickElapsed < elapsed`，则推迟 `BuffExpired`。注释写的是「周期消费者尚未处理最后一段存活时间」。
- `BuffExpireSystem`（`BuffSystem.cs:45-70`，order 41）消费 `BuffExpired` 后删除实体；带 `ModifyValue` 的再给属性打 `AttrDirty`。

因此 0.25 秒这条路径在同一次 `SystemRoot.Update` 内是：2 次 tick，然后 `tickElapsed` 追上 `elapsed`，然后删除。尾段被观察到了，但没有被当成一次 tick。缺口是这条规则没有写进 `runtime-execution`，回归也没有非整除尾段和「回收不得早于观察」的具名断言。

## What Changes

- 写明尾段语义：存活时间按 `Duration.elapsed` 计；完整间隔用向下取整结算；不足一个 `tickInterval` 的尾段 MUST NOT 再产生一次 tick。
- 写明回收时序：有周期行为的 Buff 在 `DurationExpired` 之后，必须先让周期消费者把 `tickElapsed` 推进到本次封顶后的 `elapsed`，然后才允许打 `BuffExpired` 并删除。无周期行为的 Buff 不走这条延迟。
- 保持已测行为：`runtime/buff-final-ticks` 的 2 次与 5 次断言、`runtime/buff-catchup` 的 5 次断言、`runtime/buff-refresh` 的续期存活断言都不得被改弱。
- 续期（`RefreshCore`，`BuffHelper.cs:449-455`）继续只改 `remaining` / `total` 并清除两个过期 Tag，不改 `elapsed`、`tickElapsed`、`lastTick`。本变更不重定义续期后的相位。

## Capabilities

### New Capabilities

- 无。

### Modified Capabilities

- `runtime-execution`：在「Buff 时间与续期一致」下补尾段不补跳，以及周期 Buff 的领域回收不得早于这次尾段观察。

## Impact

- `War3Frame/`：行为已由 `BuffTickSystem` 与 `BuffDurationSystem` 表达。批准后只允许为满足新增断言做最小修正；不得改 `DurationSystem` 的封顶规则，不得改 `TimerTaskSystem`。`TimerTaskKind.BuffExpire`（`TimerTaskSystem.cs:61-63`）仍是另一条直接打 `BuffExpired` 的路径，本变更不接入、不改造。
- `Projects/`：批准后在 `Projects/Regression` 增加具名场景，覆盖非整除尾段次数与回收时序。`DomainRegression.BuffFinalTicks` 的现有断言保持。
- `War3Frame.Generator/`、`FrameBuild/`、`CSharpWar3Frame/`：不受影响。不新增系统注册，不改构建与 CLI。

## 分级与审查

- 等级：`light`。规则落在现有 Buff 周期与回收路径，不新增公共 API，不跨生成器或构建链路。`Projects/Regression` 只加场景，不构成升 `full` 的跨模块契约。
- 状态：待审核。未批准前不实现。
- 复盘：`R1 Focused`。light 默认 `R0`；本变更改的是已有运行时时间契约且依赖系统 order，补一个时间语义视角。未授权完整 `review-work`。
- 工件：`proposal.md`、`design.md`、`tasks.md`、`specs/runtime-execution/spec.md`。`design.md` 只固定尾段判定与回收门槛，避免把规则只写在提案正文。实施后写短段落 `summary.md`。

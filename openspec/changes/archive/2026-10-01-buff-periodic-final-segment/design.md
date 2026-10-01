## Context

见 `proposal.md` 的 Why。本变更只固定周期 Buff 在 `DurationExpired` 当次的尾段与回收，不重做计时。

当前同一次 `SystemRoot.Update` 的顺序由注册 order 决定，不由 `Add` 顺序决定：

| 系统 | order | 本路径职责 |
|---|---|---|
| `DurationSystem` | 0 | `elapsed` 最多加到原 `remaining`；`remaining <= 0` 时归零并打 `DurationExpired` |
| `BuffTickSystem` | 39 | 用 `elapsed - tickElapsed` 累加，`floor` 出完整间隔次数，然后把 `tickElapsed` 写成当前 `elapsed` |
| `BuffDurationSystem` | 40 | 有周期行为且 `tickElapsed < elapsed` 时不打 `BuffExpired` |
| `BuffExpireSystem` | 41 | 消费 `BuffExpired`，删除实体；有 `ModifyValue` 才打 `AttrDirty` |

`Duration.remaining < 0` 是永久。永久 Buff 的 `elapsed` 继续累加，不打 `DurationExpired`，因此没有尾段回收问题。

## Goals / Non-Goals

**Goals:**

- 不足一个 `tickInterval` 的存活余数不产生 tick。
- 有周期行为的限时 Buff，回收晚于本次尾段观察，且仍在尾段已被观察的同一次更新内完成。
- 无周期行为的限时 Buff 在 `DurationExpired` 后按原路径回收。
- 已有三个回归断言的次数与续期结果不变。

**Non-Goals:**

- 不把尾段向上取整，不按比例结算半次伤害。
- 不改 `Duration.elapsed` 的封顶，不改 `ITimedSystem` 的交付。
- 不改 `RefreshCore` 对 `elapsed` / `tickElapsed` / `lastTick` 的保留。续期后的相位仍是现状。
- 不改 `TimerTaskSystem`，不把 `TimerTaskKind.BuffExpire` 纳入这条延迟。
- 不新增组件、Tag 或系统。

## Decisions

### 1. 尾段用向下取整，余数不补跳

一次观察里可结算次数是 `floor((lastTick + 本次新存活时间) / tickInterval)`。`Duration.elapsed` 在到期帧已被封顶，所以新存活时间不会超过寿命。余数继续留在 `lastTick`，供未到期时的下一次观察使用；到期后实体会被删除，这个余数不再产生行为。

例：寿命 0.25 秒、间隔 0.1 秒、一次推进 1 秒。封顶后存活 0.25 秒，结算 2 次，余 0.05 秒。这 0.05 秒不是第三次 tick。

选择向下取整是因为当前 `BuffTickSystem` 与 `runtime/buff-final-ticks`、`runtime/buff-catchup` 已经是这个规则。向上取整会把 0.25 秒变成 3 次，直接打坏现有断言。按余数比例结算会改变 `DamageRequest` 的次数或数值，超出本变更。

### 2. 回收门槛是「尾段已被观察」，不是「再跳一次」

`BuffDurationSystem` 只在下面三者同时成立时推迟 `BuffExpired`：

- `tickInterval > 0`
- `tickActionId` 非空
- `tickElapsed < elapsed`

`BuffTickSystem` 即使 `hitCount == 0` 也先把 `tickElapsed` 写成 `elapsed`。因此门槛表示的是 order 39 已经看过这段存活时间，不是还欠一次 tick。

同一次更新内的结果：

1. order 0 封顶并打 `DurationExpired`。
2. order 39 结算完整间隔，并把 `tickElapsed` 对齐到封顶后的 `elapsed`。
3. order 40 看到两者相等，打 `BuffExpired`。
4. order 41 删除实体。

若 order 40 先于 order 39 运行，门槛会挡住回收，避免尾段还没被观察就删除。这是延迟的全部含义。

`tickInterval <= 0` 或 `tickActionId` 为空的 Buff 没有周期消费者，`DurationExpired` 后立即进入 `BuffExpired`。不给它们发明等待条件。

### 3. 不改续期相位

`RefreshCore` 在刷新时长时写 `remaining` 与 `total`，并移除 `DurationExpired` 与 `BuffExpired`。它不重置 `elapsed`、`tickElapsed`、`lastTick`。`runtime/buff-refresh` 只要求续期后剩余时间约为 5 秒且两个过期 Tag 不在。本变更不把「续期是否重相对齐 tick」拉进范围；那会改变 DoT 续期后的第一次结算时间。

### 4. `TimerTaskSystem` 保持旁路

`TimerTaskKind.BuffExpire` 直接给计时任务实体打 `BuffExpired`，不读 `Duration` 或 `tickElapsed`。仓库内 Buff 寿命现由 `Duration` 驱动。把这条旁路也改成尾段延迟会扩大到计时任务语义，本变更明确不动。

## Risks / Trade-offs

- [实现时把尾段补成一次 tick] → 回归必须保留 0.25 秒 / 0.1 秒 = 2 次；新增场景用不能整除的寿命断言次数等于 `floor(寿命 / 间隔)`。
- [为了延迟回收而把删除拖到下一帧] → 新增场景要求同一次更新结束后实体已删除，且删除前的 tick 次数已经满足。
- [改动 order 使观察晚于回收] → 不改这三个系统的 order；场景用真实注册顺序能跑到的 `SystemRoot`，而不是手写反序。
- [顺手重置续期相位] → `tasks.md` 禁止改 `RefreshCore` 的时钟字段；`runtime/buff-refresh` 保持原断言。

## Migration Plan

无数据迁移。回滚即还原 `BuffSystem.cs` 中与本规则有关的改动，并移除新增回归场景。若批准后核对发现当前代码已满足全部新断言，实现阶段只加场景，不改生产代码。

## Open Questions

无。尾段不补跳、回收等待观察、续期相位不动、计时任务旁路不动已在本设计定死。

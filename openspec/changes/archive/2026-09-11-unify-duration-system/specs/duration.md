# Spec：统一 Duration 组件与到期语义

## DUR-1：Duration 组件

- **需求**：存在统一持续时间组件，表达"剩余秒数"与"初始值"。
- **实现**：`Duration : IComponent`，字段 `remaining`（-1=永久，0=立即到期，>0=剩余秒数）、`total`（初始值，供进度显示）。
- **挂载**：由各领域 Helper（EffectHelper/BuffHelper/区域创建）在创建时挂载，业务代码不直接操作。
- **验收**：组件定义存在于 `War3Frame/Src/Components/Time/Duration.cs`。

## DUR-2：到期标记

- **需求**：Duration 到期时存在统一内部阶段标记，供领域系统消费。
- **实现**：`DurationExpired : ITag`，由 `DurationSystem` 打标；同一实体不重复打标。
- **命名**：符合 AGENTS.md 过去式内部阶段规则（不加 `Tag` 后缀）。
- **验收**：到期实体带 `DurationExpired`；对外广播必须另发领域 Event，不得让全局 Trigger 扫该 Tag。

## DUR-3：统一推进系统

- **需求**：唯一系统推进所有 Duration，-1 永久不递减，<=0 打标。
- **实现**：`DurationSystem`，`[SystemRegister(SystemKind.Interval, 0)]`，cadence 0.02s。
- **职责边界**：只递减 + 打标，**不做任何领域清理**；到期动作由各领域系统消费 `DurationExpired` 执行。
- **验收**：`Duration.remaining` 只被 `DurationSystem` 递减；领域系统不做递减。

## DUR-4：Effect 迁移

- **需求**：特效持续时间改用 `Duration`，公共签名与语义不变。
- **实现**：`EffectHelper.CreatePosition/CreateAttached` 内部挂 `Duration`；`0` 仍转 0.02f 下一 tick 到期；`-1` 永久；`>0` 到期销毁。
- **验收**：`EffectBase.duration` 字段移除，全仓零残留；`EffectRuntimeSystem` 消费 `DurationExpired` 执行销毁，Attach 跟随逻辑保留。

## DUR-5：Buff 迁移

- **需求**：Buff 持续时间改用 `Duration`，永久 Buff 用 `-1`。
- **实现**：`BuffDuration` 保留 `duration` 原始值（供刷新计算），删除 `remaining`/`isPermanent` 推进；`BuffHelper.GetRemaining` 读 `Duration.remaining`；`BuffDurationSystem` 消费 `DurationExpired` 执行移除与 `BuffExpired` 触发。
- **验收**：`BuffDuration.remaining` 零残留；永久 Buff（原 `isPermanent=true`）挂 `Duration.remaining=-1` 不递减；Buff 刷新语义与现状等价。

## DUR-6：GroundArea 迁移

- **需求**：地面区域持续时间改用 `Duration`。
- **实现**：删除 `GroundAreaLifetime`，区域实体挂 `Duration`；`GroundAreaLifetimeSystem` 消费 `DurationExpired` 执行 `DeleteAreaBuffs` + `DeleteEntity`。
- **验收**：`GroundAreaLifetime` 零残留；区域到期清理行为与现状等价。

## DUR-7：范围边界

- **非目标**：`TimerInfo`/`TimerTaskSystem`（任务调度）、`CastState.remaining`（施法状态）、`AbilityCooldownState.remaining`（冷却）、`UnitHelper` 尸体时长**不迁移**，保持各自语义。
- **验收**：上述组件与系统未被改动；全仓 `TimerInfo` 引用不变。
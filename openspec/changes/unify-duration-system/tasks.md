# 任务清单：统一 Duration 组件 + 单一 DurationSystem

## 批 1 - 基础（纯新增）

- [x]新增 `War3Frame/Src/Components/Time/Duration.cs`：`Duration` 组件（remaining/total）+ `DurationExpired : ITag`
- [x]新增 `War3Frame/Src/Systems/Time/DurationSystem.cs`：`[SystemRegister(SystemKind.Interval, 0)]`，0.02s 递减，-1 跳过，<=0 打 `DurationExpired`（防重复打标）
- [x]构建验证：`dotnet build War3Frame.csproj` 0 错误

## 批 2 - Effect 迁移

- [x]`EffectHelper.CreatePosition/CreateAttached`：内部改挂 `Duration`（签名与 -1/0/>0 语义不变；0 仍转 0.02f）
- [x]`EffectRuntimeSystem` 改造：去递减，改 `QuerySystem<EffectBase, DurationExpired>` 消费到期销毁；保留 Attach 跟随逻辑
- [x]`EffectBase.duration` 字段移除；全仓引用迁移（`AbilityEffectHelper` 传递点）
- [x]构建验证 + `EffectBase.duration` 零残留

## 批 3 - Buff + GroundArea 迁移

- [x]`BuffDuration` 瘦身：删除 `remaining`/`isPermanent` 推进（迁移到 Duration）；保留 `duration` 原始值
- [x]`BuffHelper`：创建 Buff 挂 `Duration`（`-1` 永久）；`GetRemaining` 改读 `Duration.remaining`
- [x]`BuffDurationSystem` 改造：监听 `DurationExpired` 执行移除 + `BuffExpired` 触发（核对原"过期前 TimerInfo 预警"消费方）
- [x]删除 `GroundAreaLifetime`；区域实体挂 `Duration`（`AbilityEffectSystems` 673/1183 创建点）
- [x]`GroundAreaLifetimeSystem` 改造：`QuerySystem<GroundAreaData, DurationExpired>` 消费到期（DeleteAreaBuffs + DeleteEntity）
- [x]构建验证 + `BuffDuration.remaining`/`GroundAreaLifetime` 零残留

## 收尾

- [x]全仓核对：`TimerInfo`/`TimerTaskSystem`/`CastState.remaining`/`AbilityCooldownState.remaining` 未被改动
- [x]行为等价验证：`EffectHelper` -1/0/>0 三条语义、Buff 到期移除、区域到期清理
- [x]写 `summary.md`，提案状态改 `已实施`

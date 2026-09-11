# 总结：统一 Duration 组件 + 单一 DurationSystem

**状态**：已实施
**日期**：2026-08-29

## 实际改动

### 新增（2 文件）
1. `War3Frame/Src/Components/Time/Duration.cs`：
   - `Duration : IComponent`（`remaining` -1=永久/0=立即/>0=剩余；`total` 初始值；`Create` 工厂）
   - `DurationExpired : ITag`（内部阶段到期标记）
2. `War3Frame/Src/Systems/Time/DurationSystem.cs`：唯一递减系统（0.02s，order 0），-1 跳过、<=0 打 `DurationExpired`（防重复），不做领域清理

### Effect 迁移
3. `EffectHelper.CreatePosition/CreateAttached`：`EffectBase.duration` 字段移除，创建时改挂 `Duration.Create(duration)`；公共签名与 -1/0/>0 语义不变（0 仍转 0.02f）
4. `EffectRuntimeSystem`：去递减逻辑，改消费 `DurationExpired` 执行销毁（hideFirst），Attach 跟随逻辑保留
5. `EffectBase.duration` 字段移除

### Buff 迁移
6. `BuffDuration` 瘦身：删除 `remaining`/`isPermanent`/`Refresh()`，保留 `duration` 原始值（供刷新计算）
7. `BuffHelper`：三处创建（限时/堆叠/永久）挂 `Duration`（永久用 `-1`）；`GetBuffRemainingTime` 改读 `Duration.remaining`；三处 `RefreshDuration` 刷新逻辑改为重置 `Duration.remaining`
8. `BuffDurationSystem` 重写：`QuerySystem<BuffDuration, Duration>` + Filter Buff，消费 `DurationExpired` → 打 `BuffExpired`；删除原 TimerTask(BuffExpire) 创建逻辑
9. `BuffExpireSystem` 不动（继续消费 `BuffExpired` 删实体 + AttrDirty）

### GroundArea 迁移
10. `GroundAreaLifetime` 组件删除
11. `AbilityEffectSystems` 两处创建点（效果结算 673、区域反应 1181）改挂 `Duration.Create(duration)`
12. `GroundAreaLifetimeSystem` 重写：`QuerySystem<GroundAreaData>` + Filter.AnyTags(DurationExpired)，消费到期执行 `DeleteAreaBuffs` + `DeleteEntity`

## 验证

- `dotnet build War3Frame/War3Frame.csproj`：0 错误 0 警告（174 个存量 KKApi nullable 警告为既有状态）
- 全仓零残留：`EffectBase.duration` / `BuffDuration.remaining` / `isPermanent` / `GroundAreaLifetime`（组件）/ `duration.Refresh` 均无命中（仅系统类名 `GroundAreaLifetimeSystem` 保留）
- 边界未动：`TimerInfo`/`TimerTaskSystem`、`AbilityCooldownState`、`CastState.remaining`、尸体时长均未迁移

## 遗留与风险

1. `TimerTaskKind.BuffExpire` 枚举成员已无生产者（原 BuffDurationSystem 不再创建该 TimerTask）；成员保留无害，后续可清理
2. Buff 原"到期前 TimerTask 预警"机制已移除：新链路 `DurationSystem(0.02s)` 到期 → `BuffDurationSystem(0.1s)` 打 BuffExpired → `BuffExpireSystem` 删除；到期判定 cadence 从 0.05s（TimerTaskSystem）变为 0.02s（DurationSystem），更精确
3. War3 客户端运行时验证未执行（阻塞验证推迟，需验收阶段统一执行）
4. 区域反应链路（GroundAreaReactionSystem → burning 区域）依赖 `DurationExpired` 与原语义一致，需客户端验证确认
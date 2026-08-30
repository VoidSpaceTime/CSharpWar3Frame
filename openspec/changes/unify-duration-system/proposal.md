# 提案：统一 Duration 组件 + 单一 DurationSystem

**状态**：已批准（用户 2026-08-29 批准）→ 已实施
**等级**：full（用户 2026-08-29 确认升级；跨 Effect/Buff/GroundArea 三领域 + 改组件契约，符合 full 判定）
**提案日期**：2026-08-29

---

## 背景

仓库现有 4 处独立的"倒计时-到期"模式，各自实现递减逻辑：

| 领域 | 组件 | 递减位置 | 到期处理 |
|---|---|---|---|
| 特效 | `EffectBase.duration` | `EffectRuntimeSystem`（0.02s） | 销毁特效 |
| Buff | `BuffDuration.remaining` | `BuffSystem` | 移除 Buff |
| 地面区域 | `GroundAreaLifetime.remaining` | `GroundAreaLifetimeSystem` | 删除区域 + 清理区域 Buff |
| 计时任务 | `TimerInfo` | `TimerTaskSystem` | 触发回调（**不纳入本次**，见非目标） |

模式重复：每个领域都要写"递减 → 判断到期 → 清理"。新领域（陷阱、临时光环、场地效果）会继续复制模板。

## 目标

1. 新增统一 `Duration` 组件：`remaining`（-1=永久，0=立即到期，>0=剩余秒数）+ `total`（原始值）
2. 新增唯一 `DurationSystem`（0.02s）：统一递减所有挂 `Duration` 的实体；`-1` 跳过、`<=0` 打 `DurationExpired` 内部阶段 Tag
3. 各领域系统监听 `DurationExpired` 做各自到期动作（销毁特效 / 移除 Buff / 清理区域），**到期动作保持领域自治**
4. 迁移 `EffectBase.duration`、`BuffDuration`、`GroundAreaLifetime` 三处引用到 `Duration`

## 非目标

- **不纳入 `TimerInfo`/`TimerTaskSystem`**：它是"到点触发动作"的任务调度（可重复、有次数），语义是触发而非生命周期到期，与 Duration 不同
- 不改变 `EffectHelper.CreatePosition/CreateAttached` 的公共签名与 `-1/0/>0` 语义（该语义已工作）
- 不改变 `BuffHelper` 公共签名
- 不做性能优化（无池化/无批处理合并）

## 影响范围

- `War3Frame/Src/Components/Effects.cs`：`EffectBase.duration` 移除（迁移到 Duration）
- `War3Frame/Src/Components/Buff.cs`：`BuffDuration` 移除或瘦身为到期标记
- `War3Frame/Src/Components/Ability/AbilityEffect.cs`：`GroundAreaLifetime` 移除
- `War3Frame/Src/Systems/EffectRuntimeSystem.cs`：改造为消费 DurationExpired
- `War3Frame/Src/Systems/BuffSystem.cs`：同上
- `War3Frame/Src/Systems/Ability/AbilityEffectSystems.cs`：`GroundAreaLifetimeSystem` 改造
- 新增：`War3Frame/Src/Components/Time/Duration.cs`、`War3Frame/Src/Systems/Time/DurationSystem.cs`
- 全仓：迁移 `EffectRuntimeSystem`/`BuffSystem`/`GroundAreaLifetimeSystem` 的引用点（`EffectHelper`、`AbilityEffectHelper`、`GroundAreaQueryHelper`、`BuffHelper` 等）

## 全局影响分析

- `War3Frame/`：核心改动（组件 + 3 个系统 + Helper 引用）
- `War3Frame.Generator/`：不受影响（无新注册机制，DurationSystem 用现有 `[SystemRegister]`）
- `FrameBuild/` / `CSharpWar3Frame/`：不受影响（无引用）
- `Projects/*`：不影响（公开 authoring API 签名不变；模板代码无直接 duration 引用）
- `BridgeToJIT/` / `FastMDX/` / `ModelFormat/`：不受影响

## 方案摘要

```
Duration { remaining, total }  +  DurationExpired : ITag
        ↑ 各领域创建时挂载（Helper 内）
        ↓
DurationSystem（0.02s）统一递减，-1 跳过，<=0 打 DurationExpired
        ↓
EffectNativeSystem/BuffSystem/GroundAreaLifetimeSystem 等监听 DurationExpired 做到期动作
```

## 风险与回滚

- 风险：跨 3 领域迁移，`EffectBase.duration` 被多处直接读写（95 处 duration 命中中特效约 20 处）；迁移需逐点核对写入路径
- 缓解：先在 `EffectRuntimeSystem` 保留原逻辑为对照，逐领域迁移后构建验证
- 回滚：组件与系统均为新增/替换，`git revert` 可完整回退；不改公共签名，模板层零影响

## 验收标准

1. `dotnet build War3Frame/War3Frame.csproj` 0 错误
2. `EffectBase.duration` / `BuffDuration` / `GroundAreaLifetime` 全仓零残留
3. `EffectHelper.CreatePosition` 的 `-1/0/>0` 语义不变（`-1` 永久、`0` 下一 tick 销毁、`>0` 到期销毁）
4. `DurationExpired` 由三个领域系统各自消费，到期动作与现状等价
5. `TimerInfo`/`TimerTaskSystem` 未被改动

## 等级说明

按 AGENTS.md 分级规则，跨 `War3Frame` 内 3 个领域（Effect/Buff/GroundArea）且改组件契约，应至少按 `full` 处理。用户已确认升级为 `full`，工件齐全：`proposal.md` + `design.md` + `tasks.md` + `specs/duration.md`。
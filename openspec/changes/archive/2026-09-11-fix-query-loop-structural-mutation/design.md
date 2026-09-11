# 设计：消除 Query 循环内结构变更

对应提案：`proposal.md`（`fix-query-loop-structural-mutation`，`full`）

## 1. 引擎契约（实测基线）

`Friflo.Engine.ECS 3.6.0`，`Query.ForEachEntity` 回调内：

| 操作 | 是否允许 | 依据 |
|---|---|---|
| `ref` 字段原地写 | ✅ | 实测 |
| `CreateEntity` | ✅ | 实测 |
| `DeleteEntity` | ✅（当前实体或他实体均不抛） | 实测 |
| `AddComponent`（新组件） | ❌ 抛 | 实测 |
| `AddComponent`（已存在组件） | ❌ 抛 | 实测（与"写回即更新"直觉不符） |
| `AddTag` / `RemoveTag` | ❌ 抛 | 实测 |
| `RemoveComponent` | ❌ 抛 | 实测 |

> 结论：`AddComponent(x)` 不能当"写回/更新"使用。要改组件值必须走 `ref`；否则移到循环外。

## 2. 统一修法规则

### R1：删除冗余写回
```csharp
Query.ForEachEntity((ref Duration d, Entity e) =>
{
    d.remaining -= dt;          // 已持久化
    // entity.AddComponent(d);  // ← 删除该行
});
```
判定：`x` 是查询 `ref` 参数 → 直接删 `AddComponent(x)`。

### R2：副本写回改 ref / 移出循环
```csharp
// 反例（会抛）：副本 + 写回
if (entity.TryGetComponent<Position>(out var pos)) { pos.x = ...; entity.AddComponent(pos); }
// 正解 A：把 Position 提入查询（ref）
Query.ForEachEntity((ref UnitNative n, ref Position pos, Entity e) => { pos.x = ...; });
// 正解 B：无法入查询的 → 收集后循环外写回
```
> 若"写回"发生在查询回调内、对象不在查询里，必须收集循环外处理。

### R3：加/删 Tag 或组件 → 收集后循环外应用
```csharp
var toMark = new List<Entity>();
Query.ForEachEntity((ref X x, Entity e) => { if (cond) toMark.Add(e); });
foreach (var e in toMark) if (!e.IsNull) e.AddTag<T>();
```
删除与新增对称；列表按 `entity.Id` 升序处理以保证锁步确定性。

### R4：删除实体
引擎允许循环内删；但删除命中当前查询匹配集的实体时，沿用仓库既有"收集后循环外删"惯例（确定性 + 可读性）。

## 3. 典型触点（本次已确认会抛，必须修）

| 文件:行 | 操作 | 修法 |
|---|---|---|
| `Systems/Time/DurationSystem.cs:37` | 循环内 `AddComponent(duration)`（无条件，每 tick） | R1 删除 |
| `Systems/Unit/UnitLifecycleTransitionSystem.cs:21,29` | 循环内 `AddComponent(state)` | R1 删除 |
| `Systems/Unit/UnitLifecycleDisposeSystem.cs:23` | 循环内经 helper 触发 `RemoveTag`/`DeleteEntity` | R3/R4 收集后处理 |
| `Systems/Time/TimerTaskSystem.cs:42,57,64` | 循环内 `AddTag`/`RemoveComponent` | R3 收集 |
| `Systems/Time/TimerTaskSystem.cs:37,70` | 循环内 `AddComponent(timer)` | R1 删除 |
| `Systems/Native/UnitNativeSystem.cs:64,69` | `TryGetComponent` 副本 + `AddComponent` | R2 |
| `Systems/Native/PlayerNativeSystem.cs:33` | 循环内 `RemoveComponent` | R3 |
| `Systems/Ability/AbilityStatCalculationSystem.cs:53` | 循环内 `RemoveTag` | R3 |
| `Systems/EffectRuntimeSystem.cs:44` | 循环内 `AddComponent` | R1/R3 |
| `Systems/ControlStateTransitionSystem.cs`、`Systems/Unit/MoveSystem.cs`、`Systems/Item/ItemSystem.cs`、`Systems/Ability/AbilitySlotSystem.cs`、`Systems/LevelExperienceSystem.cs`、`Systems/BuffSystem.cs`、`Systems/AuraSystem.cs`、`Systems/Native/EffectNativeSystem.cs` 等 | 按 `tasks.md` 逐点审计 | 视点套用 R1-R4 |

> 已确认采用"收集后应用"的正确范例（可作模板）：`Systems/Attribute/AttrCalculationSystem.cs`、`Systems/Ability/CastingSystem.cs`、`Systems/BuffSystem` 的 Expire/Tick。

## 4. 备选方案比较

- **方案 A（采用）**：`ref` 写 + 收集后循环外应用。与仓库既有惯例一致，无新依赖，改动局部。
- **方案 B**：全局引入 `CommandBuffer` 记录结构变更后回放。更系统，但需引入新机制并改造所有系统，超出本变更范围；留作后续候选。

## 5. 验证设计（关键：必须驱动循环体）

既有场景失败根因是循环体未执行。新增场景必须**先创建匹配实体**再 `root.Update`：

| 场景 | 构造 | 断言 |
|---|---|---|
| `DurationSystem` | 实体挂 `Duration{remaining=0.5}` | 一帧后不抛 + `remaining` 递减 + 到期挂 `DurationExpired` |
| `UnitLifecycleTransitionSystem` | 实体 `UnitLifeState{Death}` | 一帧后 phase=Corpse，不抛 |
| `TimerTaskSystem` | 实体 `TimerTask{Once}` + owner | 到期挂 `TimerExpired`，不抛 |
| Buff 到期 | `Buff` + `Duration` 驱动 | `BuffExpired` 产出 |
| 移动推进 | `MoveCommand` 驱动 | 到达收敛 |
| 物品挂载 | `ItemAttrApplyRequest` | 属性实体创建 |
| 光环增删 | `Aura` 进出范围 | buff 增删不抛 |

## 6. 实施顺序

1. 逐文件审计并修复（按 `tasks.md`）。
2. 每修一批 → 编译 + 跑该域场景。
3. 全量回归 + 静态复核。
4. `summary.md`。

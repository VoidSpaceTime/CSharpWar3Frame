# 设计：单位死亡事件广播（UnitDiedEvent）+ 击杀奖励迁移

> 对应 change：`unit-died-event-broadcast`（full）。
> 承接：`introduce-ability-skill-point-upgrade`（已实施未提交，`KillUnit` bool 门闩为基础）。

## 1. 问题定义

死亡是领域事实，但当前没有"死亡事实"载体——死后副作用只能内联在伤害结算死亡分支。设计最优形态：死亡入口广播事实，副作用作为独立消费方。

| 关注点 | 现状 | 目标 |
|---|---|---|
| 死亡事实 | 无（只在 damage 分支内联副作用） | `UnitDiedEvent` 广播 |
| 击杀奖励 | `DamageResolveSystem` 内联 `TryDispatch` | `KillRewardSystem` 事件消费 |
| 重复派发门闩 | `KillUnit` bool（上 change 已落） | 保留：事件只在 Alive→Death 当次创建 |
| 新死亡路径 | 需各自内联奖励 | 只需调 `KillUnit`，自动带事件 |

## 2. 职责边界

```text
KillUnit（死亡唯一入口）
  只：Alive→Death 状态转移 + 尸体清理 TimerTask（现状）
      转移成功时创建 UnitDiedEvent（广播事实，source 透传）
  不：解析奖励、调原生、做任何死后业务

KillRewardSystem（order 126，QuerySystem<UnitDiedEvent>）
  只：读 UnitDiedEvent → 死者 expReward>0 且 source 非空 → 创建 ExperienceGainRequest
  不：删事件（EventCleanupSystem 132 清理）、不调原生

DamageResolveSystem（order 125）
  只：结算伤害 → 死亡判定时 KillUnit(target, source)
  不：再感知奖励/任何死后业务

ExperienceSystem（order 0，下帧）
  不变：消费请求加经验/升级发点
```

## 3. 数据契约

### 3.1 UnitDiedEvent（独立事件实体）

```csharp
public struct UnitDiedEvent : IComponent
{
    public Entity unit;    // 死者
    public Entity source;  // 击杀者；IsNull = 非击杀死亡（环境/脚本无源）
}
```

- 挂 `TriggerEventMarker { eventTypeId = EventTypeRegistry.Get<UnitDiedEvent>() }`；登记进 `RegisterBuiltIn`。
- 一次死亡一条；允许多监听（`KillRewardSystem` + 未来触发器/掉落/任务）。

### 3.2 死亡 ≠ 移除（重要语义边界）

`UnitHelper` 提供两条互斥路径，只有"死亡"广播事件：

| 路径 | 入口 | 行为 |
|---|---|---|
| 死亡 | `KillUnit(unit, source = default)` | Alive→Death + 尸体计时 + 广播 `UnitDiedEvent` |
| 移除 | `RemoveUnit(unit)` | Alive/Corpse → Remove，跳过尸体，**不广播死亡事件** |
| 终态收尾 | `CleanupFinalizeEntityDispose(entity)` | 清属性/技能 + `DeleteEntity`，**不广播死亡事件** |

规则：
- "让单位死"用 `KillUnit`（无论有无击杀者，死亡事实都广播；无击杀者传默认 source）。
- "把单位移除掉"（卸载、清场、非死亡删除）用 `RemoveUnit`，**禁止用 `KillUnit` 当删除手段**——否则会产生不应存在的死亡事件，未来掉落/任务/击杀计数监听都会误触发。
- 不需要"跳过事件的 KillUnit"变体：那是给死亡开不广播后门，会破坏死亡事实完整性。

### 3.3 KillUnit 签名升级

```csharp
/// <summary>
/// 让单位进入死亡流程。仅 Alive → Death 转移时返回 true（致死当次）并广播 UnitDiedEvent。
/// source 为击杀者（无击杀者的环境/脚本死亡传 default）；unit 无 UnitLifeState 时不广播且返回 false。
/// 移除单位请用 RemoveUnit（不广播死亡事件），勿用本方法当删除手段。
/// </summary>
public static bool KillUnit(Entity unit, Entity source = default)
```

- 可选参数：`KillUnit(unit)` = 非击杀死亡（事件广播、`source` 空、奖励不派发）；`KillUnit(unit, killer)` = 击杀死亡。
- 无 `UnitLifeState` 或非 Alive：返回 false、不广播（同现状门闩）。
- 约束：`KillUnit` 内含结构变更（AddComponent + CreateEntity），**调用点不得在 Friflo Query 迭代内**（现状唯一调用在 `DamageResolveSystem` 快照循环外；原 `AddComponent TimerTask` 已隐含此约束，新增调用点时文档化）。

### 3.3 KillRewardSystem（order 126）

```csharp
[SystemRegister(SystemKind.Interval, 126)]
public class KillRewardSystem : QuerySystem<UnitDiedEvent>
```

- 快照收集 → 循环外处理（仓库结算系统统一模式，避免迭代内结构变更）。
- 处理：`source.IsNull → continue`；`victim.TryGetComponent<UnitKillRewardData>` 缺省 → continue；`expReward.Resolve(victimLevel) <= 0 → continue`；否则 `CreateEntity(ExperienceGainRequest { target: source, amount, sourceType: "kill" })`。
- 事件实体留给 `EventCleanupSystem`（order 132）统一清理；不删事件。

## 4. 主流程（击杀奖励全链路）

```text
DamageRequest(source, target)
  → DamageResolveSystem(125)
     remaining<=0 && !immune
       → KillUnit(target, source)   [Alive→Death]
           状态 Death + 尸体 TimerTask
           CreateEntity UnitDiedEvent { unit: target, source } + TriggerEventMarker
  → KillRewardSystem(126)（同帧）
     source 非空 && target.expReward>0
       → CreateEntity ExperienceGainRequest(target: source)
  → ExperienceSystem(0)（下帧）
     killer 无 ExperienceData → 静默忽略（现状）
     → 加经验 / 升级发点 / UnitLeveledEvent
```

- 经验相对死亡延迟一帧（order 125 → 126 → 0），与上 change 一致、显式接受。
- 事件清理在 order 132，晚于全部监听方。

## 5. 系统 order

| 系统 | kind / order | 说明 |
|---|---|---|
| `DamageResolveSystem` | Interval 125 | 死亡判定 → `KillUnit`（唯一调用点） |
| `KillRewardSystem` | Interval 126（新增） | 消费 `UnitDiedEvent`，同帧发经验请求 |
| `ExperienceSystem` | Interval 0 | 下帧结算经验 |
| `EventCleanupSystem` | Interval 132 | 清理全部 `TriggerEventMarker` 事件实体 |

约束：新增死亡事件监听系统必须 order < 132；`KillRewardSystem` 取 126（紧邻创建点，避免跨帧事件被清理竞争）。

## 6. 与 TriggerSystem 机制边界

- `TriggerSystem` 供内容作者声明规则（条件 + 动作），`KillRewardSystem` 是**框架默认成长规则**（挂 `expReward` 的怪被击杀即自动奖励），两者定位不同。
- 二者共享同一事件实体（事件允许多监听、监听者不删除）；`TriggerSystem` 无需改动即可监听 `UnitDiedEvent`。

## 7. Native / 分层

- 全程不新增 War3 原生调用；事件/奖励均为 ECS 事实与请求。
- `KillUnit` 事件广播是纯 ECS 结构变更，不违反"业务系统不直接调原生"分层（`KillUnit` 本身即 ECS 生命周期入口）。

## 8. 兼容与默认关闭

- 未配 `expReward`：死亡事件照发，奖励不派发（每死多 1 个事件实体，量级同 `DamageEvent`）。
- `source` 为空（非击杀死亡）：事件照发，奖励不派发。
- 无 `ExperienceData` 击杀者：经验系统静默忽略。
- `RemoveUnit` / `CleanupFinalizeEntityDispose` 路径不广播死亡事件（移除 ≠ 死亡），现有清理调用无回归。
- `KillRewardHelper` 删除后无生产引用（已核实仅伤害分支 + 测试 Phase 1，随本 change 迁移）。
- 现有模板、场景、Trigger 规则均无行为回归。

## 9. 候选方案

| 方案 | 结论 |
|---|---|
| A. `KillUnit` 内广播 + 独立 `KillRewardSystem` 消费 | **采用**（死亡唯一入口 = 唯一广播点，新路径自动带事件） |
| B. 各死亡调用方各自创建事件 | 广播散落、易漏；拒绝 |
| C. 独立过渡系统扫 Death 状态 + kill credit 组件再发事件 | 多组件多系统、延迟一帧才可广播；拒绝 |
| D. 奖励做成 TriggerSystem 规则动作 | 定位不符（框架默认规则 vs 内容作者规则）；拒绝，规则侧后续可自行监听 |
| E. 维持 damage 分支内联 | 本次重构动机；拒绝 |

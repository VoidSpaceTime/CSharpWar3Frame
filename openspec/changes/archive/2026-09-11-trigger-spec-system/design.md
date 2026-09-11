# 设计：触发器体系 TriggerSpec

## 1. 设计总览

```
事件实体（事实）         规则实体（配置）            动作（意图）
DamageEvent        ×    TriggerSpec              → DamageRequest
HealEvent               TriggerRuntime           → HealRequest
BuffAppliedEvent        (规则索引: eventTypeId)   → BuffApplyRequest
ControlStateChangedEvent                         → CastRequest(挂主体)
        ↓ 创建处挂 TriggerEventMarker                     ↓
TriggerSystem(131) 匹配/条件/策略 → TriggerActionRegistry
EventCleanupSystem(132) 清理消费窗口后的事件实体
```

- 事件侧保持现有"独立事件实体"模式不变；唯一新增是创建处挂 `TriggerEventMarker`。
- 规则侧为独立"触发器实体"（方案 A），挂 `TriggerSpec`（配置）+ `TriggerRuntime`（状态）。
- 动作侧经注册表转换为已有 `XxxRequest`，复用现有 Resolve 系统。

## 2. 组件定义

```csharp
// 事件类型注册表：typeId ↔ 组件类型（静态注册，同 EffectFormulaRegistry 形态）
public static class EventTypeRegistry
{
    // 内置登记：DamageEvent / HealEvent / BuffAppliedEvent / ControlStateChangedEvent
    public static int Register<T>() where T : struct, IComponent;   // 返回 typeId
    public static int Get<T>() where T : struct, IComponent;
}

// 事件标记：挂事件实体（创建点挂载，TriggerSystem 单查询发现）
public struct TriggerEventMarker : IComponent
{
    public int eventTypeId;
}

// 条件（叶子）：注册表键 + not 标志 + 三通道参数
public struct TriggerCondition
{
    public int conditionId;     // TriggerConditionRegistry 键（0 = 恒真）
    public bool not;            // 取反
    public float[] paramF;      // 数值参数（如伤害阈值）
    public string[] paramS;     // 字符串参数（如 buffId/技能名）
    public Entity[] paramE;     // 实体参数（如目标单位）
}

// 动作：注册表键 + 三通道参数
public struct TriggerAction
{
    public int actionId;        // TriggerActionRegistry 键
    public float[] paramF;
    public string[] paramS;
    public Entity[] paramE;
}

// 策略
public enum TriggerPolicyKind { Once, Cooldown, Count }

public struct TriggerPolicy
{
    public TriggerPolicyKind kind;
    public float cooldown;      // Cooldown：秒
    public int maxCount;        // Count：允许次数
}

// 规则配置组件（挂触发器实体）
public struct TriggerSpec : IComponent
{
    public int eventTypeId;             // 匹配的事件类型（0 = 匹配全部）
    public ConditionCombine combine;    // All / Any（单根组合）
    public TriggerCondition[] conditions; // 空 = 无条件
    public TriggerPolicy policy;
    public TriggerAction[] actions;
}

// 规则状态组件（挂触发器实体）
public struct TriggerRuntime : IComponent
{
    public float cooldownRemain;   // Cooldown 剩余秒
    public int triggerCount;       // 已触发次数
}

// 条件/动作上下文
public readonly struct TriggerContext
{
    public readonly EntityStore Store;
    public readonly Entity EventEntity;    // 触发的事件实体
    public readonly Entity TriggerEntity;  // 规则实体
}
```

**命名规则核对**（AGENTS.md）：`TriggerSpec`/`TriggerRuntime` 数据组件 ✓；`TriggerEventMarker` 组件（带数据，非 ITag）✓；`TriggerCondition`/`TriggerAction`/`TriggerPolicy` 配置结构 ✓；无 Tag 名带 Event/Request ✓。

## 3. 事件发现（TriggerEventMarker）

- 事件创建点（4 处）创建事件实体后附加 `TriggerEventMarker { eventTypeId = EventTypeRegistry.Get<DamageEvent>() }`。
- TriggerSystem 与 EventCleanupSystem 均 `Query<TriggerEventMarker>` 单查询发现全部事件实体，无需组合查询或反射。
- **新事件类型接入成本**：定义组件 → `EventTypeRegistry.Register<T>()` → 创建点挂 marker——TriggerSystem 无需修改（验收 6）。
- 为什么不是 ITag：需要携带 eventTypeId（零数据 Tag 无法区分类型）；为什么不是注册表组合查询：Friflo 动态 AnyOf 依赖版本 API，标记组件更简单可靠。

## 4. 注册表

```csharp
public delegate bool TriggerConditionHandler(TriggerContext ctx, TriggerCondition c);
public delegate void TriggerActionHandler(TriggerContext ctx, TriggerAction a);

public static class TriggerConditionRegistry   // 同 EffectFormulaRegistry：SortedDictionary + Register/TryGet
public static class TriggerActionRegistry
```

**内置条件**（注册表初始登记）：
| conditionId | 语义 | 参数 |
|---|---|---|
| 0 | AlwaysTrue（恒真，未登记也可用） | - |
| 1 | DamageGreater：事件为 DamageEvent 且 amount > paramF[0] | paramF[0]=阈值 |
| 2 | TargetIs：事件 target 为 paramE[0] | paramE[0]=单位 |
| 3 | SourceIs：事件 source 为 paramE[0] | paramE[0]=单位 |

**内置动作**（初始登记）：
| actionId | 语义 | 参数 |
|---|---|---|
| 1 | Damage：创建 DamageRequest 独立实体（source=事件 source/target=事件 target） | paramF[0]=amount |
| 2 | Heal：创建 HealRequest | paramF[0]=amount |
| 3 | BuffApply：创建 BuffApplyRequest（target=事件 target） | paramS[0]=buffId、paramF[0..3]=attrTypeId/modifyType/value/duration |

**扩展示例**（首期不内置，用户可按需注册）：Cast 动作——向事件 target 挂 `CastRequest`（`target.AddComponent<CastRequest>`，挂主体；ability 实体经 paramE 传入），与 `CastRequestSystem`（QuerySystem<CastRequest, Position>）消费形态对齐。

动作 handler 内**只允许**创建/附加 Request 组件，禁止调用 War3 原生 API（分层规则）。

## 5. TriggerSystem（Interval 131）

```
OnUpdate:
  1. 规则索引：Query<TriggerSpec> 收集全部规则实体，按 eventTypeId 分组
     （Dictionary<int, List<Entity>>，每 tick 重建——规则数小，先简单）
  2. 事件扫描：Query<TriggerEventMarker> 遍历事件实体
     对每个事件：
       eventTypeId → 规则组（eventTypeId=0 的通用规则附加匹配）
       对每条规则（按注册顺序）：
         a. TriggerRuntime 检查：cooldownRemain <= 0 且 (kind != Count || triggerCount < maxCount)
         b. 条件判定：combine=All 全真 / Any 任一真（叶子 not 取反）
         c. 命中 → 执行全部 actions（经注册表）
         d. 策略消耗：Once → 标记待删除（收集后删触发器实体）；Cooldown → cooldownRemain = policy.cooldown；
            Count → triggerCount++（达到 maxCount 后不再触发）
  3. 收集的结构变更（删触发器实体）统一在迭代外执行
```

- order 131：严格晚于事件创建（46/125/126/127/129）与 GroundArea（128/129/130）与 EffectLifecycle（130）；事件"滞后一拍"语义（129 的 DamageRequest 下一轮 125 才变事件）符合同步确定性。
- 每 tick 重建规则索引的代价：规则数通常 < 100，可接受；后续可改为增量维护。
- 多规则命中同一事件：按规则实体创建顺序（规则索引 List 顺序）；首期不做 priority（TriggerPolicy 不包含）。

## 6. EventCleanupSystem（Interval 132）

```
OnUpdate:
  Query<TriggerEventMarker> 遍历事件实体 → 直接删除（消费窗口 = 1 tick）
```

- 首期消费窗口 = 1 tick（事件创建帧 N，TriggerSystem 帧 N 消费，EventCleanupSystem 帧 N 清理）。
- 若未来需要跨帧事件窗口（如"3 秒内被攻击 3 次"），扩展 `TriggerEventMarker` 增加 `bornTick` + 存活阈值；首期不做。
- **影响**：现有事件实体（DamageEvent 等）此前永不清除；本系统落地后生命周期收敛为 1 tick。需核对现有监听系统 order 均 < 132（验收 5 覆盖）。

## 7. Builder API 草案

```csharp
// 链式配置（写 ECS 意图）
TriggerHelper.Register(store, builder =>
    builder.OnEvent<DamageEvent>()
           .When(c => c.DamageGreater(100f))
           .Count(3)
           .Then(a => a.Damage(50f))
           .Then(a => a.BuffApply("frost")));

// 内部实现：TriggerSpecBuilder 累积配置 → Build() → 创建触发器实体（TriggerSpec + TriggerRuntime）
```

- `TriggerHelper.Register` 返回触发器实体，供后续注销（`entity.DeleteEntity()`）。
- Builder 提供类型安全入口：`OnEvent<T>()` 直接绑定组件类型（编译期校验），避免手写 typeId。

## 8. 顺序与 order 布局

| order | 系统 | 职责 |
|---|---|---|
| 45 | AttrCalculationSystem | 属性重算 |
| 46 | ControlStateTransitionSystem | 控制跳变 → ControlStateChangedEvent（挂 marker） |
| 125-127 | 结算系统 | Damage/Heal/BuffApplied 事件创建（挂 marker） |
| 128-130 | GroundArea / EffectLifecycle | 地面区域 / 特效生命周期 |
| **131** | **TriggerSystem** | 匹配/条件/策略/动作 |
| **132** | **EventCleanupSystem** | 清理事件实体 |
| 133+ | （预留） | 未来事件窗口系统 |

## 9. 确定性约束

- 条件/动作注册表禁止使用 `Random.Shared`、`DateTime` 等非确定性源。
- 规则评估顺序 = 规则实体注册顺序（索引 List 保持创建序），保证锁步一致。
- 事件清理固定 1 tick 窗口，跨端一致。

## 10. 与现有能力边界

- `AbilityBehaviorTrigger`（技能内部生命周期）：不动，与 TriggerSpec 同族不同域。
- `EffectChainBuilder`（动作 DSL）：TriggerActionRegistry 的动作是"生成 Request"原语；复杂组合仍用 EffectChainBuilder（技能模板内），触发器不做 DSL 嵌套。
- `TimerTask`/`TimerExpired`：内部计时不动；触发器不消费计时事件（首期）。
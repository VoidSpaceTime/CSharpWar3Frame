# 设计：触发器体系 TriggerSpec

## 1. 组件设计（`War3Frame/Src/Components/Trigger/TriggerSpec.cs`）

### 1.1 TriggerEventTag（ITag）
- 挂载位置：**事件实体**（DamageEvent/HealEvent/BuffAppliedEvent 等）。
- 作用：标识"该实体是可由触发器匹配的领域事件"。由事件创建方（结算系统）创建实体时一并挂载；TriggerSystem 以此查询事件流。
- 命名遵守 Tag 规则：零数据分类标记，不带 Request/Event 后缀。

### 1.2 TriggerSpec（IComponent）
挂载位置：**触发器实体**（独立规则实体，与事件实体分离）。字段：

| 字段 | 类型 | 说明 |
|---|---|---|
| `eventTypeId` | int | 匹配的事件类型（`TriggerEventType` 注册表键值） |
| `conditions` | TriggerCondition[] | 组合条件（All/Any/Not，见 1.3） |
| `policy` | TriggerPolicy | 触发策略（见 1.4） |
| `actions` | TriggerAction[] | 命中后动作（见 1.5） |
| `priority` | int | 同事件多规则时执行优先级（大者先，默认 0） |

### 1.3 TriggerCondition（struct）
| 字段 | 类型 | 说明 |
|---|---|---|
| `kind` | TriggerConditionKind | All / Any / Not（Not 为单条件取反） |
| `conditionId` | int | `TriggerConditionRegistry` 键值 |
| `parameters` | Dictionary<string, float>? | 条件参数（阈值、单位 id 等） |

### 1.4 TriggerPolicy（struct）
| 字段 | 类型 | 说明 |
|---|---|---|
| `kind` | TriggerPolicyKind | Once（一次性）/ Cooldown（冷却）/ Count（次数上限） |
| `cooldown` | float | 冷却秒数（Cooldown 用） |
| `maxCount` | int | 最大触发次数（Count/Once 用，Once=1） |

### 1.5 TriggerAction（struct）
| 字段 | 类型 | 说明 |
|---|---|---|
| `actionId` | int | `TriggerActionRegistry` 键值 |
| `parameters` | Dictionary<string, float>? | 动作参数（伤害值、buffId、目标选择等） |

### 1.6 TriggerRuntime（IComponent）
挂载位置：**触发器实体**。系统维护的运行态：
| 字段 | 类型 | 说明 |
|---|---|---|
| `remainingCooldown` | float | 冷却剩余秒数（每 tick 递减，>0 时规则跳过） |
| `triggeredCount` | int | 已触发次数（Count/Once 策略达上限后规则失效并移除实体） |

### 1.7 TriggerEventType（静态注册表）
- 形态同 `EffectFormulaRegistry`：`Register(string name) -> int`、`TryResolve`。
- 内置事件类型与现有事件组件一一对应：`Damage`、`Heal`、`BuffApplied`（首批）；后续领域事件扩展时注册即可。
- **注册表键 vs 组件类型**：注册表提供稳定 int 键避免字符串运行时主键；匹配时由 TriggerSystem 用 `entity.Has<T>()` 校验对应组件存在（事件类型 ↔ 组件类型的映射表随注册一起登记）。

## 2. 系统设计（`War3Frame/Src/Systems/Trigger/TriggerSystem.cs`）

### 2.1 注册与顺序
```
[SystemRegister(SystemKind.Interval, 128)]
```
- order 128：晚于事件结算（Damage/Heal/Buff 结算为 125/126/127），早于生命周期清理（EffectLifecycleSystem 130）——保证事件实体在触发窗口内存活。
- 后续若出现"事件先于 TriggerSystem 被领域清理"的需求，再按域拆分 order；当前统一一个系统。

### 2.2 每 tick 流程
```
1. TriggerRuntime 递减 remainingCooldown（全量触发器实体，Once/Count 达上限的移除）
2. 收集本 tick 事件实体（Query<TriggerEventTag>，快照）
3. 对每个事件实体：
   a. 解析实体上事件组件类型 → eventTypeId
   b. 查该 eventTypeId 的规则组（按 eventTypeId 分组的 SortedDictionary<int, List<Entity>> 索引，注册/移除时维护）
   c. 按 priority 降序逐个规则：
      - TriggerRuntime 冷却中 → 跳过
      - Conditions 判定（All：全部成立；Any：任一成立；Not：取反）→ 不通过跳过
      - 通过 → 执行 actions（按序），更新 TriggerRuntime（Once：count+1 达上限移除实体；Cooldown：置 remainingCooldown；Count：count+1）
4. 事件实体不动（由统一清理系统删除）
```

### 2.3 条件判定上下文
```csharp
public struct TriggerConditionContext
{
    public Entity eventEntity;   // 事件实体
    public Entity source;        // 事件语义源（从事件组件读取）
    public Entity target;
    public TriggerCondition condition;
}
```

### 2.4 动作执行上下文
```csharp
public struct TriggerActionContext
{
    public Entity eventEntity;
    public Entity source;
    public Entity target;
    public TriggerAction action;
    public EntityStore Store;    // 生成 Request 用
}
```

## 3. 注册表设计

### 3.1 TriggerConditionRegistry
- 签名：`delegate bool TriggerConditionFunc(TriggerConditionContext ctx);`
- API：`Register(int conditionId, TriggerConditionFunc func)` / `TryResolve`。
- 内置条件（首批）：
  - `unit.is`：source/target 为指定单位（参数：entityId）
  - `attr.threshold`：目标属性 ≥ 阈值（参数：attrId、min；可选 max）
  - `damage.min`：事件伤害 ≥ 阈值（参数：min）
  - `odds`：概率命中（参数：percent，配合 `Random.Shared`，同步环境下用框架统一随机源）
  - `source.isUnit` / `target.isUnit`：来源/目标存在性

### 3.2 TriggerActionRegistry
- 签名：`delegate void TriggerActionFunc(TriggerActionContext ctx);`
- API：`Register(int actionId, TriggerActionFunc func)` / `TryResolve`。
- 内置动作（首批，全部只生成 Request）：
  - `request.damage`：向 target 生成 `DamageRequest`（参数：damage、damageSrc、damageType）
  - `request.buff`：向 target 生成 `BuffApplyRequest`（参数：buffId、duration、attrTypeId、modifyType、value）
  - `request.heal`：向 source/target 生成 `HealRequest`（参数：amount、target 选择）
  - `request.cast`：向 source 生成 `CastRequest`（参数：abilityTemplateName、target）——依赖施法域已有 CastRequest 语义
- 扩展约束：动作内不得出现 `JassApi`/`DzApi` 等原生调用；需要原生副作用的动作必须改写 Request 交给对应 Resolve/Native 系统。

## 4. Helper / Builder API

```csharp
// 触发器规则 Builder（链式，同 EffectChainBuilder 族）
public sealed class TriggerSpecBuilder
{
    public static TriggerSpecBuilder OnEvent(TriggerEventType eventType);  // 或 string 重载
    public TriggerSpecBuilder When(TriggerConditionKind kind, int conditionId, params (string key, float value)[] parameters);
    public TriggerSpecBuilder Once();
    public TriggerSpecBuilder Cooldown(float seconds);
    public TriggerSpecBuilder Count(int max);
    public TriggerSpecBuilder Then(int actionId, params (string key, float value)[] parameters);
    public TriggerSpec Build();
}

// 快捷注册入口（写 ECS 意图）
public static class TriggerHelper
{
    public static Entity Register(EntityStore store, TriggerSpec spec);   // 创建触发器实体（TriggerSpec + TriggerRuntime）
    public static void Unregister(Entity triggerEntity);
}
```

## 5. 命名与边界约定

- 触发器实体：独立实体，不挂在单位/技能上（全局规则）；若需"单位私有规则"，design 阶段预留 `owner` 可选字段，首期不做。
- 事件类型注册与事件组件一一对应，命名 `Damage`/`Heal`/`BuffApplied`，与现有组件名一致。
- 与 `AbilityBehaviorTrigger` 边界：技能生命周期触发仍在技能域（`AbilityBehaviorSpec`）；本体系只管跨领域规则。
- 触发器回调路径零原生调用；原生副作用一律经 Request → Resolve → Native 链路。

## 6. 性能与容量

- 规则索引：按 eventTypeId 分组（Dictionary<int, List<Entity>>），事件到达时只遍历同组规则，避免全量规则 × 事件。
- 事件实体快照：每 tick 复用 List 缓冲，避免分配。
- 规模预期：规则数 ≤ 数百、事件实体 ≤ 数千/帧时无压力；超标再升级为帧缓冲 + 事件队列。
- `TriggerRuntime` 递减全量扫描触发器实体（数百规模，0.01s tick 可接受；后续可改为冷却桶）。

## 7. 验证场景（`Projects/test/Scripts/Process/TriggerValidationScenario.cs`）

1. 规则 A：事件 `Damage`，条件 `damage.min ≥ 50`，动作 `request.buff`（标记被击单位）——验证条件过滤。
2. 规则 B：事件 `Dead`（首期若无 Dead 事件则用 `Damage` + `attr.threshold` 模拟），策略 `Once`——验证一次性。
3. 规则 C：事件 `Damage`，策略 `Cooldown(1.0)`——验证冷却内不重复触发。
4. 断言：`Require` 校验触发次数、冷却窗口、条件命中，不命中时零副作用。
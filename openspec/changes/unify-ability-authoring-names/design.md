## Context

当前项目已有两类技能表达方式：

1. 主动技能链：`AbilityHelper.SetEffectSpec(ability, EffectSpecBuilder.Chain()...)`。
2. 运行时 ECS 系统：`AbilityEffectHelper` 展开 effect entity，`ProjectileSystem`、`AreaSearchSystem`、`DamageEffectSystem`、`HealEffectSystem`、`BuffEffectSystem`、`GroundArea*System` 执行。

这套模型适合一次性主动技能，但用户给出的 Lua `治疗之鸟` 暴露出另一类需求：技能获得后持续运行，周期搜索友军，按血量比例选目标，发射弹道，弹道结束后按目标最大生命值治疗。该需求不应全部塞进 `EffectSpecBuilder`，否则 `EffectSpec` 会同时承担触发、目标选择、异步等待和结算职责。

## Goals / Non-Goals

**Goals:**

- 统一技能 authoring 命名，减少 `EffectSpec`、`AbilityEffect`、`AbilityBehavior` 概念混杂。
- 明确三层职责：`AbilitySpec` 管完整技能定义，`AbilityBehavior` 管触发流程，`AbilityEffect` 管最终结算效果。
- 为主动技能和被动/周期技能提供同一套可读 builder 风格。
- 保留现有 `EffectSpec` 迁移路径，避免一次性破坏现有模板。
- 为后续 `治疗之鸟` 这类技能提供正式设计基础。

**Non-Goals:**

- 不在本提案阶段实现代码。
- 不新增 Lua 回调或脚本桥接。
- 不把行为层设计成无限制命令式脚本。
- 不修改 Native/Execution 分层规则。

## Naming Decisions

### 1. Top-level: AbilitySpec

**Decision:** 新增或规范 `AbilitySpec` 作为完整技能定义的 authoring 根对象。

候选类型：

- `AbilitySpec`
- `AbilitySpecData`
- `AbilitySpecBuilder`

职责：

- 技能 ID、名称、描述、目标类型。
- 技能等级、冷却、消耗、施法距离、半径等基础数值。
- 挂载一个或多个 `AbilityBehaviorSpec`。

### 2. Flow layer: AbilityBehavior

**Decision:** `AbilityBehavior` 表达“什么时候触发、如何找目标、如何等待、如何重复、何时停止”。

候选类型：

- `AbilityBehaviorSpec`
- `AbilityBehaviorData`
- `AbilityBehaviorBuilder`
- `AbilityBehaviorSystem`

候选 builder 方法：

- `OnCast()`
- `OnGranted()`
- `OnRemoved()`
- `OnOwnerDamaged()`
- `OnOwnerDealDamage()`
- `Repeat(AbilityValue interval)`
- `SearchCircle(AbilityValue radius)`
- `SearchLine(AbilityValue range, AbilityValue width)`
- `Filter(TargetFilter filter)`
- `SelectFirst()`
- `SelectLowest(TargetMetric metric)`
- `SelectHighest(TargetMetric metric)`
- `Projectile(string model)`
- `OnArrive(AbilityEffectSpec effect)`
- `Do(AbilityEffectSpec effect)`
- `StopWhenOwnerDead()`

### 3. Settlement layer: AbilityEffect

**Decision:** 逐步将现有 `EffectSpec` 命名迁移为 `AbilityEffectSpec`，强调它属于技能效果结算层。

候选新类型：

- `AbilityEffectSpec`
- `AbilityEffectStepSpec`
- `AbilityEffectSpecData`
- `AbilityEffectSpecBuilder`

迁移策略：

- 第一阶段保留旧 `EffectSpec` 作为兼容入口。
- 新 builder 先作为新名称包装或并行类型。
- 示例代码优先使用新命名。
- 后续提案再决定是否标记旧 API 为过渡名称。

### 4. Value layer: AbilityValue

**Decision:** 引入统一数值表达，覆盖常量、技能 stat、单位属性和公式参数。

候选类型：

- `AbilityValue`
- `AbilityValueKind`

候选方法：

- `AbilityValue.Constant(float value)`
- `AbilityValue.AbilityStat(int statId, float scale = 1f, float bonus = 0f)`
- `AbilityValue.OwnerAttr(int attrId, float scale = 1f, float bonus = 0f)`
- `AbilityValue.CasterAttr(int attrId, float scale = 1f, float bonus = 0f)`
- `AbilityValue.TargetAttr(int attrId, float scale = 1f, float bonus = 0f)`
- `AbilityValue.Formula(string formulaId, ...)`

## Example Shape

### 主动技能

```csharp
AbilitySpecBuilder
    .Create("flamethrower")
    .Name("喷火")
    .TargetType(AbilityTargetType.Point)
    .Behavior(AbilityBehaviorBuilder
        .OnCast()
        .SearchLine(AbilityValue.AbilityStat(AbilityHelper.Range), AbilityValue.Constant(140f))
        .Filter(TargetFilter.EnemyAlive)
        .Do(AbilityEffectSpecBuilder
            .Chain()
            .Damage(AbilityValue.AbilityStat(AbilityHelper.DamageAmount), DamageType.Magical, DamageSrc.Skill)
            .Build())
        .Build())
    .Build();
```

### 被动周期技能

```csharp
AbilitySpecBuilder
    .Create("healing_bird")
    .Name("治疗之鸟")
    .TargetType(AbilityTargetType.None)
    .Behavior(AbilityBehaviorBuilder
        .OnGranted()
        .Repeat(AbilityValue.OwnerAttr(AttributeHelper.AttackInterval))
        .SearchCircle(AbilityValue.OwnerAttr(AttributeHelper.AttackRange))
        .Filter(TargetFilter.AllyAlive)
        .SelectLowest(TargetMetric.HealthPercent)
        .Projectile("DruidoftheTalonMissile")
        .OnArrive(AbilityEffectSpecBuilder
            .Chain()
            .Heal(AbilityValue.TargetAttr(AttributeHelper.Health, scale: 0.01f))
            .Build())
        .StopWhenOwnerDead()
        .Build())
    .Build();
```


### 主动技能便捷写法

第一阶段保留 `Behavior(AbilityBehaviorBuilder.OnCast().Do(...).Build())` 作为底层显式写法，同时在 `AbilitySpecBuilder` 上增加常用触发语法糖：

```csharp
AbilitySpecBuilder
    .Create("fire_blast")
    .OnCast(e => e
        .Area(TargetFilter.EnemyAlive)
        .Damage(AbilityValue.AbilityStat(AbilityHelper.DamageAmount)))
    .BuildTo(ability, level);
```

该写法内部等价于 `OnCast + Chain + Build + Behavior`，只隐藏样板代码，不改变 `AbilityBehavior` 与 `AbilityEffect` 的职责边界。后续 `OnGranted(...)`、`OnRemoved(...)` 可使用同一模式，但空触发行为不应写入规格。
## Runtime Ownership

- `AbilityBehaviorSystem` 只负责推进行为状态、产生命中上下文或 effect request。
- `AbilityEffectSystem` 仍负责伤害、治疗、Buff、GroundArea 等结算。
- 目标搜索继续复用 `GroupHelper` 和 `TargetFilterRegistry`。
- 数值解析扩展 `EffectFormulaRegistry` 或迁移为统一 `AbilityValueResolver`。
- 视觉和 War3 native 仍由 Native/Execution 层消费 ECS 意图。

## Migration / Phasing

1. 定义新命名和 spec delta，先不实现。
2. 第一阶段实现 `AbilityValue` 与 `AbilityEffectSpecBuilder` 新命名包装，保证主动技能示例可读。
3. 第二阶段实现最小 `AbilityBehaviorSpec`：`OnCast` + `Do`，保持与现有主动链兼容。
4. 第三阶段实现 `OnGranted` + `Repeat` + `SearchCircle` + `SelectLowest` + `Projectile` + `OnArrive`，用于 `治疗之鸟`。
5. 第四阶段逐步迁移 `Projects/test` 示例，并保留旧 `EffectSpecBuilder` 兼容入口。

## Risks / Trade-offs

- [风险] 新 builder 过度抽象，反而难读。  
  [缓解] 第一阶段只覆盖真实示例，避免一次性做完整 DSL。

- [风险] 旧 `EffectSpec` 与新 `AbilityEffectSpec` 并存造成困惑。  
  [缓解] 明确迁移期和命名 alias，不在同一示例中混用。

- [风险] `AbilityBehavior` 变成脚本语言。  
  [缓解] 行为层只表达触发、搜索、选择、等待和调用 effect，不允许任意 delegate 直接改 ECS 真相。

- [风险] `TargetFilter` 现有语义不足以表达“缺血友军”。  
  [缓解] 允许后续新增 `TargetCondition` 或 `TargetMetric`，但不在本提案直接实现全部过滤器。

## Open Questions

- `AbilityValue.TargetAttr(AttributeHelper.Health)` 表达最大生命值时，是否需要新增 `HealthMax` / `HealthCurrent` 区分。
- `AttackInterval` 是否应复用现有 `AttackInterval`，还是新增更接近 Lua `attackSpace()` 的属性别名。
- `AbilityEffectSpecBuilder` 是否应先作为 `EffectSpecBuilder` 包装，还是直接创建新类型并做转换。
- `AbilityBehaviorSpec` 的运行时状态是挂在 ability entity，还是生成独立 behavior runtime entity。

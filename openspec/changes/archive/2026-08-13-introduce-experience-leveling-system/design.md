## 设计概览

经验系统只承担两个责任：

1. 根据经验曲线计算升级需求。
2. 消费获得经验请求，判断是否升级；升级后修改 level 并添加 `LevelStatDirty`。

经验系统不负责等级属性计算。等级属性计算由 `LevelStatRebuildSystem` 完成。

## 核心组件

### ExperienceData

用于保存对象当前经验状态和曲线配置。

建议形态：

```csharp
public struct ExperienceData : IComponent
{
    public float currentExp;
    public float totalExp;
    public int maxLevel;
    public ExperienceCurve curve;
}
```

说明：

- `currentExp`: 当前等级内经验。
- `totalExp`: 历史累计经验，可用于显示或排行榜。
- `maxLevel`: 最高等级，0 或负数可表示不限制，具体实现阶段再定。
- `curve`: 每次升级所需经验曲线。

当前等级不建议放在 `ExperienceData` 中，优先复用对象自己的等级来源：

- Unit: `UnitLevel.level` 或后续确认的单位等级组件。
- Ability: `AbilityBase.level`。
- Item: `ItemLevel.level` 或后续确认的物品等级组件。

### ExperienceCurve

经验曲线应数据化，避免第一阶段使用 delegate 作为主路径。

建议形态：

```csharp
public readonly struct ExperienceCurve
{
    public readonly ExperienceCurveKind kind;
    public readonly float fixedStep;
    public readonly float baseExp;
    public readonly float perLevel;
    public readonly float growth;
    public readonly float[]? table;

    public float RequiredForNextLevel(int currentLevel);
}
```

首批支持：

- `FixedStep(100)`: 每级固定 100。
- `Linear(100, 50)`: 1->2 需要 100，2->3 需要 150。
- `LevelTable(100, 180, 300)`: 手填每级升级需求。

指数或曲线 keyframes 可以后续再加：

- `Exponential(baseExp, growth)`
- `CurveKeyframes(...)`

## 获得经验请求

建议使用请求组件，而不是让击杀、任务、技能等系统直接改经验。

```csharp
public struct ExperienceGainRequest : IComponent
{
    public Entity target;
    public float amount;
    public float multiplier;
    public Entity source;
    public string sourceType;
}
```

处理规则：

```text
finalExp = amount * multiplier
```

第一阶段倍率来自 request。后续如果有经验倍率 Buff，可由经验系统读取目标经验倍率属性，但不放进第一阶段。

## 经验系统流程

```text
击杀/任务/技能/物品等来源
 -> 创建 ExperienceGainRequest
 -> ExperienceSystem 消费请求
 -> 找到 target 的 ExperienceData
 -> currentExp += amount * multiplier
 -> while currentExp >= RequiredForNextLevel(level)
      currentExp -= RequiredForNextLevel(level)
      level += 1
      add LevelStatDirty
 -> totalExp += finalExp
 -> 删除或清理 ExperienceGainRequest
```

是否允许一次获得经验连升多级：建议允许，使用 while 循环直到经验不足或达到 `maxLevel`。

## Unit / Ability / Item 适配

### Unit

单位是经验系统的主路径。

```text
Unit + ExperienceData + UnitLevel
```

获得经验后提升 `UnitLevel.level`，并给 unit 添加 `LevelStatDirty`。

### Ability

技能可用于熟练度或成长技能。

```text
Ability + ExperienceData + AbilityBase.level
```

获得经验后提升 `AbilityBase.level`，并给 ability 添加 `LevelStatDirty`。

### Item

物品可用于成长装备、杀敌成长武器等。

```text
Item + ExperienceData + ItemLevel
```

获得经验后提升 `ItemLevel.level`，并给 item 添加 `LevelStatDirty`。如果物品已装备，后续属性聚合系统应让 owner unit 重新计算最终属性；这不是经验系统直接职责。

## 杀敌数需求

第一阶段建议把杀敌数视为经验来源，而不是单独做一套 kill count leveling。

示例：

```text
每击杀 1 个敌人 -> target 获得 1 点经验
曲线 LevelTable(10, 25, 50)
```

如果未来 UI 需要区分普通经验、杀敌数、熟练度，可再引入：

```csharp
public enum ExperienceKind
{
    Normal,
    KillCount,
    Proficiency
}
```

第一阶段暂不引入多经验条，避免过早复杂化。

## 与 LevelStatRebuildSystem 的边界

经验系统的输出只有两个：

- 等级变化。
- 添加 `LevelStatDirty`。

属性重算流程：

```text
ExperienceSystem
 -> 修改 level
 -> AddTag<LevelStatDirty>()
 -> LevelStatRebuildSystem
 -> 根据 LevelValue 重算等级基础属性/技能数值/物品贡献
```

## 分层约束

- `ExperienceSystem` 不调用 War3 native。
- `ExperienceSystem` 不直接修改最终属性。
- 击杀判定由战斗/生命周期系统负责，经验系统只消费请求。
- 任务奖励、技能熟练度、物品成长都通过 `ExperienceGainRequest` 接入。

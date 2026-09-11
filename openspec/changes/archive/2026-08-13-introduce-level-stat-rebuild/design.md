## 设计概览

本变更把“按等级变化的数值”抽成通用 `LevelValue`，让 Unit / Item / Ability 共享同一套 authoring 与解析规则。

核心原则：

- 纯数字默认固定值，不随等级变化。
- 线性成长必须显式使用 `PerLevel`。
- 非线性优先使用 `LevelTable`。
- `LevelStatRebuildSystem` 只负责等级驱动的基础数值解析，不负责最终聚合。

## LevelValue

建议形态：

```csharp
public readonly struct LevelValue
{
    public readonly LevelValueKind kind;
    public readonly float fixedValue;
    public readonly float baseValue;
    public readonly float perLevel;
    public readonly float[]? table;

    public float Resolve(int level);
}
```

支持类型：

```csharp
public enum LevelValueKind
{
    Fixed,
    PerLevel,
    LevelTable
}
```

authoring 入口：

```csharp
LevelValue.Fixed(20)
LevelValue.PerLevel(20, 20)
LevelValue.LevelTable(20, 40, 75)
```

解析规则：

- `Fixed`: 返回固定值。
- `PerLevel`: `baseValue + (max(level, 1) - 1) * perLevel`。
- `LevelTable`: 1 级取第 0 项；超过表长时取最后一项。

## Builder 形态

Ability：

```csharp
.BaseValue(AbilityHelper.ManaCost, 20)
.BaseValue(AbilityHelper.DamageAmount, LevelValue.PerLevel(100, 50))
.BaseValue(AbilityHelper.CooldownDuration, LevelValue.LevelTable(8, 7, 6))
```

Unit：

```csharp
.Attr(AttributeHelper.Health, 500)
.Attr(AttributeHelper.Damage, LevelValue.PerLevel(20, 5))
```

Item：

```csharp
.Attr(AttributeHelper.Strength, ModifyType.Flat, 2)
.Attr(AttributeHelper.Strength, ModifyType.Flat, LevelValue.PerLevel(2, 1))
```

## 数据存储调整

- `UnitAttributeSpec.baseValue`: `float` -> `LevelValue`
- `ItemAttributeContributionSpec.value`: `float` -> `LevelValue`
- `AbilitySpec.baseValues`: `Dictionary<int, float>` -> `Dictionary<int, LevelValue>`

纯数字重载会转换为 `LevelValue.Fixed(value)`，保持旧模板语义。

## Dirty 触发

新增 tag：

```csharp
public struct LevelStatDirty : ITag
{
}
```

触发来源：

- Unit 升级 / 降级。
- Item 升级 / 降级。
- Ability 升级 / 降级。
- 模板首次 BuildTo 后，如果存在等级数值，也可以直接解析一次当前等级。

## LevelStatRebuildSystem

命名使用用户指定的：

```csharp
LevelStatRebuildSystem
```

职责：

- 查询带 `LevelStatDirty` 的 Unit / Item / Ability entity。
- 读取当前等级。
- 读取 `UnitSpecData` / `ItemSpecData` / `AbilitySpecData`。
- 调用 `LevelValue.Resolve(level)`。
- 写入等级解析后的基础值。
- 添加后续 dirty 或 apply request。
- 移除 `LevelStatDirty`。

非职责：

- 不处理 Buff 合成。
- 不处理装备总属性最终聚合。
- 不调用 War3 native。
- 不结算伤害或治疗。

## 当前等级来源

第一阶段优先复用现有等级字段：

- Ability: `AbilityBase.level`
- Unit: 若已有等级组件则复用，否则新增 `UnitLevel`
- Item: 若已有等级组件则复用，否则新增 `ItemLevel`

若 Unit / Item 暂无等级组件，则新增：

```csharp
public struct UnitLevel : IComponent
{
    public int level;
}

public struct ItemLevel : IComponent
{
    public int level;
}
```

## 与现有系统关系

- Unit 属性：解析后写入基础属性层，再交由既有属性/同步流程处理。
- Item 属性：解析后更新单条 `AttributeContributionEntry` 或多条 `ItemAttributeContributionListData` 的当前值，并触发现有 apply request。
- Ability 数值：解析后调用或复用 `AbilityHelper.SetBaseValue(...)` 写入技能基础数值。

## 分阶段

1. 新增 `LevelValue` / `LevelValueKind` / `LevelStatDirty`。
2. 扩展 Unit / Item / Ability spec 和 builder，使纯数字保持固定值。
3. 实现 `LevelStatRebuildSystem` 的 Unit / Item / Ability 三段解析。
4. 迁移或新增少量示例，验证固定值、线性成长、表格成长。
5. 后续再考虑 `Exponential` / `Polynomial` / 曲线 keyframes。

# 设计：接线属性/技能数值计算系统

## 1. 目标

将 `AttrCalculationSystem` 与 `AbilityStatCalculationSystem` 接入运行调度，并修复查询循环内结构变更问题，使 `AttrDirty` / `AbilityStatDirty` 标签真正被消费。

## 2. 现状确认

### 2.1 写入方（已有）

`AttrDirty` 写入方（10+ 处）：`BuffHelper`（Buff 增删/刷新）、`AuraHelper`、`ModifyHelper`、`BuffSystem`（BuffExpire 后）、`AuraSystem`、`LevelExperienceSystem`（升级重算）、`AbilityEffectSystems`（效果结算后）。

`AbilityStatDirty` 写入方：`AbilityHelper.Stat.cs:178/210`。

### 2.2 消费方缺失

两个计算系统均无 `[SystemRegister]`，`AttrDirty`/`AbilityStatDirty` 标签永不被消费，最终值永不过期重算。

## 3. 修复方案

### 3.1 注册标注

```csharp
// AttrCalculationSystem
[SystemRegister(SystemKind.Interval, 45)]
public class AttrCalculationSystem : QuerySystem<AttrValue>
```

```csharp
// AbilityStatCalculationSystem
[SystemRegister(SystemKind.Interval, 30)]
public class AbilityStatCalculationSystem : QuerySystem<AbilityStatValue>
```

### 3.2 order 选址依据

现有 order 分布（Interval）：
- `LevelExperienceSystem` / `ItemAttributeContributionListApplySystem` / `ItemSystem`：order 0（dirty 写入）
- `BuffDurationSystem` 40 / `BuffExpireSystem` 41 / `AuraSystem` 42（dirty 写入）
- `AbilityEffectSystems`：100+（数值消费）
- `UnitNativeSystem`：order 1（读 `finalValue` 同步原生）

选址原则：
- `AttrCalculationSystem` order 45：位于所有 dirty 写入方（0/40/41/42）之后，效果结算（100+）之前。同 tick 内先消费当轮 dirty，再供结算读取最新值。
- `AbilityStatCalculationSystem` order 30：位于 Buff/Aura 写入之后、技能效果 100+ 之前。

注意：`UnitNativeSystem`（order 1）早于 45，存在一 tick 内的读取顺序差异。但 `UnitNativeSystem` 是独立 interval（0.03125s）的投影系统，读的是"最近一次已计算 finalValue"，跨 tick 即可拿到新值，不构成一致性问题。此点记录为已知边界。

### 3.3 查询循环内结构变更修复

现状（问题代码）：

```csharp
protected override void OnUpdate()
{
    Query.ForEachEntity((ref AttrValue attr, Entity attrEntity) =>
    {
        // ... 计算 ...
        attrEntity.RemoveTag<AttrDirty>();  // 查询迭代中结构变更 ← 风险
    });
}
```

修复为"收集后统一 apply"模式：

```csharp
private readonly List<Entity> _dirty = new();

protected override void OnUpdate()
{
    _dirty.Clear();
    Query.ForEachEntity((ref AttrValue attr, Entity attrEntity) =>
    {
        _dirty.Add(attrEntity);   // 只收集
        // ... 计算（修改 struct 字段，AddComponent 回写）...
        attrEntity.AddComponent(attr);
    });

    foreach (var entity in _dirty)
        entity.RemoveTag<AttrDirty>();   // 查询外统一结构变更
}
```

与 `BuffExpireSystem`（`toDelete` 列表模式）、`CastingSystem`（`_pending` 列表模式）保持一致。

注意：`AddComponent(attr)` 对 struct 的字段修改回写不属于结构变更（组件已存在，只是值更新），可留在循环内；仅 `RemoveTag`/`AddTag`/`DeleteEntity` 属于结构变更，必须移出。

### 3.4 不改变的内容

- 计算公式：`(base + flat) × (1 + percentAdd) × percentMul`。
- `AttrValue` / `AbilityStatValue` / `AttrDirty` / `AbilityStatDirty` 组件与标签定义。
- 修改器遍历方式（`GetIncomingLinks<ModifyTarget>`）。

## 4. 验证方案

1. `dotnet build War3Frame/War3Frame.csproj` 通过。
2. `dotnet build Projects/test/test.csproj` 通过。
3. 静态检查：两个系统带 `[SystemRegister]`；循环体内无结构变更调用。
4. 运行时场景（`Projects/test`）：
   - `talent_vitality` / `talent_mana_focus`：属性贡献在获得/移除天赋后正确反映到 `finalValue`。
   - Buff 施加/过期：属性加成出现与回落，`AttrDirty` 被消费。
   - 原生血蓝同步：`UnitNativeSystem` 使用最新 `finalValue` 投影。

## 5. 边界与拒绝项

- **不接线 `SpatialGridSystem`**：空间网格另有 `GroupHelper.Grid` 静态持有与重建语义，属于独立问题，不在本 change。
- **不接线 `AbilitySlotSystem`**：空壳系统（`OnUpdate` 无逻辑），无价值。
- **不调整公式**：属性计算数学保持现状。
- **不新增 dirty 写入方**：现有写入方已覆盖主要路径；若验证发现缺失再单独立项。

# Buff 应用入口统一与生命周期扩展 — Proposal

## 元信息

- **提案等级**：full
- **变更 ID**：unify-buff-apply
- **状态**：已实施
- **创建日期**：2026-09-03
- **实施日期**：2026-09-03
- **影响模块**：`War3Frame/Src/Helpers/BuffHelper.cs`, `War3Frame/Src/Systems/BuffSystem.cs`, `War3Frame/Src/Components/Buff.cs`, `War3Frame/Src/Helpers/EffectChainBuilder.cs`, `Projects/test/Scripts/Template/`

## 背景与动机

### 当前问题

Buff 系统存在三个独立入口（`AddTimedBuff` / `AddStackableBuff` / `AddPermanentBuff`），导致：

1. **重复触发语义不一致**：
   - 限时 buff 每次刷新都重置 duration，但 value 沿用旧值（P2）。
   - 可叠层 buff 每次触发都用新值覆盖所有层（P3）。
   - 永久 buff 无法顶替（同 key 并存）。
   - 缺少"独占控制"语义（眩晕/定身应取最晚结束时间，不累加）。

2. **净化/驱散缺少分类依据**：
   - 无法区分 buff/debuff（净化技能要清 debuff 保留 buff）。
   - 无法按元素分类（火焰免疫要清火系 debuff）。

3. **DoT/周期效果无统一机制**：
   - 点燃/中毒等 DoT 无法复用 buff 生命周期，需手工维护独立 tick。

4. **级联清理缺失**：
   - 净化 buff 时，无法自动清理由它创建的子效果（如光环创建的范围 buff、召唤的单位）。

5. **UI 显示缺少图标路径**：
   - Buff 无 icon 字段，UI 层需要额外映射表。

### 设计目标

1. **统一入口**：一个 `ApplyBuff(BuffSpec)` 工厂方法替代三个旧入口，旧入口改为兼容别名。
2. **顶替语义完整**：新增 `Replace`（删旧建新）和 `ReplaceIfLonger`（独占控制取最晚结束）。
3. **刷新语义修正**：`RefreshDuration` / `RefreshAndStack` 使用新 spec 的 duration/value（修复 P2/P3）。
4. **来源独立**：不同来源的同属性 buff 独立共存（如两个技能各给 -30% 移速，叠加为 -60%）。
5. **分类标签**：buff 带 `tags` 字段（`["Debuff", "Fire", "DoT"]`），支持按 tag 净化/免疫。
6. **周期 tick**：buff 可设 `tickInterval`，由 `BuffSystem` 驱动周期行为（如 DoT 每秒扣血）。
7. **级联清理**：buff 带 `buffInstanceId`，子效果记录 `sourceBuffId`，净化时自动清理子效果。
8. **UI 支持**：buff 带 `icon` 字段，直接映射贴图路径。

## 非目标

- 不改变现有 buff 的底层存储结构（仍是独立实体 + `ModifyTarget` link）。
- 不实现"buff 内部承载多个子效果"（如单 buff 同时减速 + DoT），保持"一个 buff = 一个数值贡献"的简洁模型。
- 不实现复杂光环（如"每 2 秒对范围内敌人施加中毒"），光环 tick 由单独提案处理。
- 不实现召唤物系统（`SummonedUnit` 组件），只预留 `sourceBuffId` 扩展口。

## 方案概述

### 核心组件变更

```csharp
// 1. Buff 组件扩展
public struct Buff : IComponent {
    public string buffId;              // 类型 ID（如 "ignite"）
    public long buffInstanceId;        // 实例 ID（全局唯一，用于级联清理）
    public List<string> tags;          // 分类标签（["Debuff", "Fire", "DoT"]）
    public float tickInterval;         // 周期 tick 间隔（0 = 不 tick）
    public string? tickActionId;       // Tick 行为 ID（注册表查询）
}

// 2. BuffBehavior 扩展
public struct BuffBehavior : IComponent {
    public string? icon;               // UI 图标路径
    public BuffRefreshBehavior onDuplicate;
}

// 3. BuffRefreshBehavior 枚举扩展
public enum BuffRefreshBehavior {
    RefreshDuration,      // 刷新时长，保留旧层数（现有）
    RefreshAndStack,      // 刷新时长，叠加层数到 max（现有）
    AddStack,             // 不刷新时长，只叠层（现有）
    Independent,          // 不做任何操作（现有）
    Replace,              // 删旧建新（新增）
    ReplaceIfLonger       // 仅当新 duration 更长时替换（新增，独占控制用）
}

// 4. BuffSpec 结构体（数据容器）
public readonly struct BuffSpec {
    public string buffId;
    public string? icon;
    public int attrTypeId;
    public ModifyType modifyType;
    public float value;
    public float duration;
    public int maxStacks;
    public BuffRefreshBehavior onDuplicate;
    public float tickInterval;
    public string? tickActionId;
    public List<string> tags;
}
```

### 入口统一

```csharp
// 新统一入口
public static Entity ApplyBuff(EntityStore store, Entity unit, Entity source, in BuffSpec spec);

// 旧入口改为别名
public static Entity AddTimedBuff(...) 
    => ApplyBuff(store, unit, source, new BuffSpec {
        duration = duration, maxStacks = 1, onDuplicate = RefreshDuration, ...
    });
```

### Tick 系统

`BuffSystem.Update` 增加 tick 逻辑：

```csharp
foreach (var (buff, duration, behavior) in Query<Buff, Duration, BuffBehavior>()) {
    if (buff.tickInterval <= 0) continue;
    duration.elapsed += deltaTime;
    while (duration.elapsed - buff.lastTick >= buff.tickInterval) {
        buff.lastTick += buff.tickInterval;
        BuffTickActionRegistry.Get(buff.tickActionId).Execute(buff.entity, target);
    }
}
```

内置 tick 行为：
- `"DealDamage"`：读 buff 的 `damagePerTick` 字段（需补充到 `Buff` 或用 `ModifyValue` 存），发 `DamageRequest`。

### 级联清理

```csharp
public static void PurgeDebuffsWithCascade(EntityStore store, Entity unit, string tagFilter = "Debuff") {
    var buffIdsToRemove = new HashSet<long>();
    
    // 收集要删的 buff 实例 ID
    foreach (var (buff, target) in store.Query<Buff, ModifyTarget>()) {
        if (buff.tags.Contains(tagFilter) && target.target == unit) {
            buffIdsToRemove.Add(buff.buffInstanceId);
        }
    }
    
    // 删 buff 本身
    foreach (var (buff, _) in store.Query<Buff, ModifyTarget>()) {
        if (buffIdsToRemove.Contains(buff.buffInstanceId)) {
            buff.entity.DeleteEntity();
        }
    }
    
    // 级联删子效果（预留扩展，当前无独立子效果实体）
    // 未来如果实现 SummonedUnit / DotEffect 等，在此添加级联查询
    
    unit.AddTag<AttrDirty>();
}
```

## 影响范围

### 组件变更

- `War3Frame/Src/Components/Buff.cs`：`Buff` 增加 `buffInstanceId` / `tags` / `tickInterval` / `tickActionId` 字段。
- `War3Frame/Src/Components/Buff.cs`：`BuffBehavior` 增加 `icon` 字段。
- `War3Frame/Src/Components/Buff.cs`：`BuffRefreshBehavior` 增加 `Replace` / `ReplaceIfLonger` 枚举值。

### 系统变更

- `War3Frame/Src/Systems/BuffSystem.cs`：增加 tick 循环逻辑。
- `War3Frame/Src/Systems/Attribute/AttrCalculationSystem.cs`：无需改动（已支持多来源 `ModifyValue` 独立叠加）。

### Helper 变更

- `War3Frame/Src/Helpers/BuffHelper.cs`：
  - 新增 `ApplyBuff(BuffSpec)` 方法。
  - 修正 `RefreshDuration` / `RefreshAndStack` 使用新 spec 的 duration/value。
  - 新增 `PurgeDebuffsWithCascade` 方法。
  - 新增 `Stun` / `Root` / `Silence` / `ApplyDoT` 便捷方法。
- `War3Frame/Src/Helpers/EffectChainBuilder.cs`：新增 `Stun` / `Root` / `DoT` 便捷步骤。

### 调用点变更

- `War3Frame/Src/Systems/Ability/BuffApplyResolveSystem.cs`：`AddTimedBuff` → `ApplyBuff`。
- `War3Frame/Src/Systems/Ability/GroundAreaBuffSystem.cs`：`AddPermanentBuff` → `ApplyBuff`。
- `War3Frame/Src/Systems/AuraSystem.cs`：`AddPermanentBuff` → `ApplyBuff`（保留 `AuraBuffLink`）。

### 模板示例

- `Projects/test/Scripts/Template/Ability.cs`：补充控制类技能示例（眩晕/定身）。
- `Projects/test/Scripts/Template/Ability.cs`：补充 DoT 技能示例（点燃）。

## 验收标准

1. **编译通过**：`dotnet build War3Frame` 和 `Projects/test` 无错误。
2. **旧入口兼容**：所有调用 `AddTimedBuff` / `AddStackableBuff` / `AddPermanentBuff` 的代码行为不变。
3. **顶替语义正确**：
   - `Replace`：2s 后 5s → 剩 5s，使用新 value。
   - `ReplaceIfLonger`：2s 后 5s → 剩 5s；5s 后 2s → 剩 5s（不替换）。
4. **来源独立**：两个不同来源的 `-30%` 移速 buff 叠加为 `-60%`（`AttrCalculationSystem` 已支持，无需额外验证）。
5. **Tick 正确**：点燃 buff 每 1s 触发一次 `DamageRequest`，删 buff 后 tick 停止。
6. **净化正确**：`PurgeDebuffsWithCascade` 删除所有 `tags.Contains("Debuff")` 的 buff，触发 `AttrDirty`，属性重算正确。

## 风险与缓解

### 风险 1：旧代码调用点遗漏

**风险**：如果某个调用点没有被发现，仍在用旧逻辑（如直接操作 `Buff` 组件），导致行为不一致。

**缓解**：
1. Grep 全仓库 `AddTimedBuff` / `AddStackableBuff` / `AddPermanentBuff` 调用点，逐一核对。
2. Grep `new Buff` 直接构造，确认是否有绕过 Helper 的代码。

### 风险 2：Tick 性能开销

**风险**：如果大量 buff 同时 tick（如 100 个单位各带 3 个 DoT），每帧遍历开销可能较大。

**缓解**：
1. 当前设计下，tick 检查是 O(N) 遍历（N = 带 tick 的 buff 数量），War3 场景下 N 通常 < 200，可接受。
2. 如果后续性能成为瓶颈，可升级为"下次 tick 时间堆"（优先队列），本提案预留扩展口。

### 风险 3：级联清理遗漏子效果

**风险**：如果未来新增独立子效果实体（如 `SummonedUnit`），但忘记在 `PurgeDebuffsWithCascade` 里添加级联查询，导致子效果泄漏。

**缓解**：
1. 在 `PurgeDebuffsWithCascade` 方法注释中显式标注"扩展点：新增子效果实体时，在此补充级联查询"。
2. 未来新增子效果实体的提案，必须在验收标准中包含"净化时正确级联清理"。

## 后续工作

本提案**不包含**以下能力（需单独提案）：

1. **反应系统（油 + 火 → 点燃）**：需要 `TriggerConditionRegistry` 补充 `TargetHasBuff` / `DamageTypeIs` 条件，独立提案处理。
2. **召唤物系统**：`SummonedUnit` 组件 + 生命周期绑定，独立提案处理。
3. **复杂光环（周期范围施加 buff）**：光环 tick 系统，独立提案处理。
4. **DoT 伤害类型/元素标签**：`DamageEvent` 增加 `elementTags` 或 `DamageElement` 枚举，独立提案处理。

## 参考资料

- Dota 2 Modifier 系统：`OnIntervalThink` / `OnRefresh` / `OnDestroy` 钩子。
- Unreal GAS：`GameplayEffect` Duration Policy / Stacking / `ConditionalGameplayEffects`。
- MacacaGames/EffectSystem：`EffectInfo` 数据驱动 + `TriggerTransType` 重复行为。
- Effectio (.NET)：`StatusEngine` 栈管理 + `ReactionEngine` 级联触发。
- xlik (Lua)：功能参考（不照搬实现），参见 memory #49。

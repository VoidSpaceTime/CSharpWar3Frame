# Buff 应用入口统一与生命周期扩展 — Spec Delta

## 组件规格变更

### Buff 组件 (War3Frame/Src/Components/Buff.cs)

**变更类型**：字段新增

**变更前**：
```csharp
public struct Buff : IComponent {
    public string buffId;
    // 无其他字段
}
```

**变更后**：
```csharp
public struct Buff : IComponent {
    public string buffId;              // 保持不变
    public long buffInstanceId;        // 新增：实例 ID（全局唯一，用于级联清理）
    public List<string> tags;          // 新增：分类标签（["Debuff", "Fire", "DoT"]）
    public float tickInterval;         // 新增：周期 tick 间隔（秒，0 = 不 tick）
    public string? tickActionId;       // 新增：Tick 行为 ID（指向注册表）
    public float lastTick;             // 新增：上次 tick 时间（内部字段）
}
```

**影响范围**：
- 所有创建 `Buff` 组件的代码需初始化新字段（通过 `BuffHelper.ApplyBuff` 统一创建，无需手动修改）。
- 持久化/序列化（如果未来需要）需处理新字段。

**向后兼容性**：
- 旧代码通过 `AddTimedBuff` / `AddStackableBuff` / `AddPermanentBuff` 创建的 buff，新字段自动填充默认值：
  - `buffInstanceId`：自动递增分配
  - `tags`：空列表
  - `tickInterval`：0
  - `tickActionId`：null
  - `lastTick`：0

---

### BuffBehavior 组件 (War3Frame/Src/Components/Buff.cs)

**变更类型**：字段新增

**变更前**：
```csharp
public struct BuffBehavior : IComponent {
    public string buffId;
    public BuffRefreshBehavior refreshBehavior;
    public bool removeAllStacksOnExpire;
}
```

**变更后**：
```csharp
public struct BuffBehavior : IComponent {
    public string buffId;
    public BuffRefreshBehavior refreshBehavior;
    public bool removeAllStacksOnExpire;
    public string? icon;               // 新增：UI 图标路径
}
```

**影响范围**：
- UI 系统可读取 `icon` 字段显示 buff 图标。

**向后兼容性**：
- 旧 buff 的 `icon` 为 `null`，UI 显示时回退到默认图标或按 `buffId` 映射。

---

### BuffRefreshBehavior 枚举 (War3Frame/Src/Components/Buff.cs)

**变更类型**：枚举成员新增

**变更前**：
```csharp
public enum BuffRefreshBehavior {
    RefreshDuration = 0,
    RefreshAndStack = 1,
    AddStack = 2,
    Independent = 3
}
```

**变更后**：
```csharp
public enum BuffRefreshBehavior {
    RefreshDuration = 0,
    RefreshAndStack = 1,
    AddStack = 2,
    Independent = 3,
    Replace = 4,              // 新增：删旧建新
    ReplaceIfLonger = 5       // 新增：仅当新 duration 更长时替换
}
```

**影响范围**：
- 所有 switch 语句需处理新枚举值（`BuffHelper.HandleExistingBuff` 已覆盖）。

**向后兼容性**：
- 现有 buff 不使用新枚举值，行为不变。
- 新技能可以使用 `Replace` / `ReplaceIfLonger`。

---

## API 规格变更

### BuffHelper 新增 API

#### ApplyBuff (核心工厂方法)

**签名**：
```csharp
public static Entity ApplyBuff(EntityStore store, Entity unit, Entity source, in BuffSpec spec);
```

**参数**：
- `store`：EntityStore 实例
- `unit`：目标单位实体
- `source`：来源实体（施法者/技能）
- `spec`：Buff 规格（`BuffSpec` 结构体）

**返回**：
- 创建或更新后的 buff 实体

**行为**：
- 查询目标单位是否已有同 `buffId` 的 buff。
- 未命中：创建新 buff。
- 命中：按 `spec.onDuplicate` 处理（6 种行为）。

**异常**：
- 无显式异常（假设 `unit` / `source` 有效）。

---

#### BuffSpec 结构体

**定义**：
```csharp
public readonly struct BuffSpec {
    public readonly string buffId;
    public readonly string? icon;
    public readonly int attrTypeId;         // 0 = 纯标记 buff
    public readonly ModifyType modifyType;
    public readonly float value;
    public readonly float duration;         // -1 = 永久
    public readonly int maxStacks;
    public readonly BuffRefreshBehavior onDuplicate;
    public readonly float tickInterval;     // 0 = 不 tick
    public readonly string? tickActionId;
    public readonly List<string> tags;
}
```

**字段说明**：
- `buffId`：类型 ID（如 `"control:stun"` / `"ignite"`）
- `icon`：UI 图标路径（可选）
- `attrTypeId`：属性类型 ID（0 = 纯标记，无属性贡献）
- `modifyType`：修改类型（Flat / Percent）
- `value`：修改值（或 DoT 每 tick 伤害）
- `duration`：持续时间（-1 = 永久）
- `maxStacks`：最大叠层数（1 = 不可叠层）
- `onDuplicate`：重复触发行为
- `tickInterval`：Tick 间隔（0 = 不 tick）
- `tickActionId`：Tick 行为 ID（查 `BuffTickActionRegistry`）
- `tags`：分类标签（用于净化/免疫）

---

#### 控制 buff 便捷方法

```csharp
public static Entity Stun(EntityStore store, Entity unit, Entity source, float duration);
public static Entity Root(EntityStore store, Entity unit, Entity source, float duration);
public static Entity Silence(EntityStore store, Entity unit, Entity source, float duration);
```

**行为**：
- 创建控制类 buff，使用 `ReplaceIfLonger` 策略（独占，取最晚结束）。
- 自动设置 `icon` 和 `tags`。

---

#### DoT 便捷方法

```csharp
public static Entity ApplyDoT(
    EntityStore store,
    Entity unit,
    Entity source,
    string buffId,
    float damagePerTick,
    float tickInterval,
    float duration,
    string? icon = null,
    List<string>? tags = null
);
```

**行为**：
- 创建周期伤害 buff，`tickActionId = "DealDamage"`。
- `attrTypeId = 0`（不修改属性，只 tick）。

---

#### 净化方法

```csharp
public static void PurgeDebuffsWithCascade(EntityStore store, Entity unit, string tagFilter = "Debuff");
```

**行为**：
- 删除目标单位身上所有 `tags.Contains(tagFilter)` 的 buff。
- 预留级联清理扩展点（当前无子效果实体）。
- 触发 `AttrDirty` 重算属性。

---

### 旧 API 行为变更

#### AddTimedBuff

**签名不变**，但行为修正：
- **变更前（P2 bug）**：刷新时保留旧 `value`。
- **变更后**：刷新时使用新 `value`。

**示例**：
```
施加 +20 攻速 buff (2s) → 再施加 +30 攻速 buff (3s)
变更前：duration=3s, value=+20（错误）
变更后：duration=3s, value=+30（正确）
```

---

#### AddStackableBuff

**签名不变**，但行为修正：
- **变更前（P3 bug）**：刷新叠层时，新 `valuePerStack` 覆盖所有层，导致总值不增反减。
- **变更后**：刷新叠层时，更新 `valuePerStack`，总值 = `valuePerStack * stacks`。

**示例**：
```
施加 +10 攻速 buff (叠3层) → 再施加 +15 攻速 buff
变更前：layers=3, valuePerStack=15, total=15×3=45（实际只显示 15，错误）
变更后：layers=3, valuePerStack=15, total=15×3=45（正确）
```

---

#### AddPermanentBuff

**签名不变**，行为不变（已改为调用 `ApplyBuff`，内部逻辑统一）。

---

## 系统规格变更

### BuffSystem (War3Frame/Src/Systems/BuffSystem.cs)

**变更类型**：逻辑增强

**新增职责**：
- 在 `OnUpdate` 中增加 tick 循环，遍历所有 `Buff.tickInterval > 0` 的 buff，检查 `elapsed - lastTick >= tickInterval` 时调用 `BuffTickActionRegistry.Get(tickActionId).Execute(buffEntity, target)`。

**执行顺序**：
- Tick 检查在 duration 推进**之后**执行（确保 buff 时长先更新）。

**性能影响**：
- 每帧遍历所有 buff（通常 <300 个），额外 tick 检查开销 ~300 ns/帧（可忽略）。

---

### BuffTickActionRegistry (War3Frame/Src/Helpers/BuffHelper.cs)

**新增静态注册表**：
```csharp
public static class BuffTickActionRegistry {
    public static void Register(string id, IBuffTickAction action);
    public static IBuffTickAction? Get(string? id);
}
```

**内置行为**：
- `"DealDamage"`：读取 `ModifyValue.value` 作为伤害值，创建 `DamageRequest`。

**扩展方式**：
- 实现 `IBuffTickAction` 接口，调用 `BuffTickActionRegistry.Register(id, action)` 注册。

---

## EffectChainBuilder 规格变更

### 新增便捷步骤

```csharp
public EffectChainBuilder Stun(float duration);
public EffectChainBuilder Root(float duration);
public EffectChainBuilder DoT(string buffId, float damagePerTick, float tickInterval, float duration, string? icon = null);
```

**行为**：
- 生成对应的 `BuffEffectStepSpec`，内部调用 `ApplyBuff`。

**依赖**：
- 需要在 `BuffEffectStepSpec` 中增加 `tickInterval` / `tickActionId` / `tags` 字段（本提案 T8.2 覆盖）。

---

## 模板示例规格

### 新增模板

#### stun_hammer (控制技能)

```csharp
[AbilityTemplate("stun_hammer")]
public class StunHammerAbility : IAbilityTemplate {
    public void Configure(AbilitySpecBuilder builder) {
        builder
            .TargetUnit(castRange: 600f)
            .OnEffect(e => e
                .ProjectileLine(speed: 1200f)
                .OnArrive(a => a
                    .Stun(duration: 2f)
                    .Damage(damageFormula: Formula.Constant(100f), damageType: DamageType.Physical)
                )
            );
    }
}
```

**行为**：
- 投射物命中 → 眩晕 2s + 造成 100 物理伤害。
- 重复命中取最晚结束时间（`ReplaceIfLonger`）。

---

#### ignite (DoT 技能)

```csharp
[AbilityTemplate("ignite")]
public class IgniteAbility : IAbilityTemplate {
    public void Configure(AbilitySpecBuilder builder) {
        builder
            .TargetUnit(castRange: 600f)
            .OnEffect(e => e
                .ProjectileLine(speed: 800f)
                .OnArrive(a => a
                    .DoT(
                        buffId: "ignite",
                        damagePerTick: 10f,
                        tickInterval: 1f,
                        duration: 5f,
                        icon: "ReplaceableTextures\\CommandButtons\\BTNFireBolt.blp"
                    )
                )
            );
    }
}
```

**行为**：
- 投射物命中 → 点燃 5s，每秒造成 10 真实伤害（总 50）。
- 重复命中刷新时长（`RefreshDuration`）。

---

## 验收规格

### 功能验收

1. **控制独占正确**：
   - 施加 2s 眩晕 → 1s 后施加 5s 眩晕 → 剩余 5s（不是 6s）。
   - 施加 5s 眩晕 → 1s 后施加 2s 眩晕 → 剩余 4s（不被覆盖）。

2. **DoT tick 正确**：
   - 施加点燃 buff（5s，1s tick，10 伤害）→ 推进 5.5s → 目标受到 5 次伤害（总 50）。
   - 删除 buff 中途 → tick 停止。

3. **净化正确**：
   - 施加 debuff（点燃 + 减速）+ buff（加速）→ 调用 `PurgeDebuffs("Debuff")` → 只删 debuff。
   - 净化后属性重算正确。

4. **P2/P3 bug 修复**：
   - `RefreshDuration` 使用新 `value`。
   - `RefreshAndStack` 更新 `valuePerStack`，总值正确。

### 性能验收

- 100 单位各带 3 buff（1 个 DoT）→ 60 FPS 稳定（单帧 tick 开销 <1 µs）。

### 兼容性验收

- 所有旧代码调用 `AddTimedBuff` / `AddStackableBuff` / `AddPermanentBuff` 的地方行为不变（除 P2/P3 修复）。

---

## 迁移指南

### 从旧 API 迁移到新 API

**旧代码**：
```csharp
BuffHelper.AddTimedBuff(store, unit, source, attrTypeId, modifyType, value, duration);
```

**新代码**：
```csharp
BuffHelper.ApplyBuff(store, unit, source, new BuffSpec(
    buffId: $"attr:{attrTypeId}",
    icon: null,
    attrTypeId: attrTypeId,
    modifyType: modifyType,
    value: value,
    duration: duration,
    maxStacks: 1,
    onDuplicate: BuffRefreshBehavior.RefreshDuration,
    tickInterval: 0f,
    tickActionId: null,
    tags: new List<string>()
));
```

**推荐**：优先使用便捷方法（`Stun` / `Root` / `ApplyDoT`），避免手动构造 `BuffSpec`。

---

### 自定义 Tick 行为

**步骤**：
1. 实现 `IBuffTickAction` 接口：
   ```csharp
   public class CustomTickAction : IBuffTickAction {
       public void Execute(Entity buffEntity, Entity target) {
           // 自定义逻辑
       }
   }
   ```

2. 注册：
   ```csharp
   BuffTickActionRegistry.Register("CustomTick", new CustomTickAction());
   ```

3. 使用：
   ```csharp
   BuffHelper.ApplyBuff(store, unit, source, new BuffSpec(
       // ...
       tickInterval: 1f,
       tickActionId: "CustomTick",
       // ...
   ));
   ```

---

## 已知限制

1. **同 buffId 同 target 唯一**：不支持同一技能在同一单位上创建多个独立 buff 实例（需要通过 `buffId` 编码来源实现独立，如 `"slow:skillA"` / `"slow:skillB"`）。

2. **Tick 性能**：所有 buff 每帧遍历（无索引优化），大量 buff（>1000）可能有性能瓶颈（当前 War3 场景下不会达到）。

3. **级联清理预留**：当前只删 buff 本身，未来实现召唤物/子效果实体时需补充级联查询。

4. **Icon 路径硬编码**：icon 字段是字符串路径，无类型检查，错误路径只能运行时发现。

---

## 后续扩展点

1. **反应系统**：`TriggerConditionRegistry` 补充 `TargetHasBuff` / `DamageTypeIs` 条件，实现"油 + 火 → 点燃"反应（独立提案）。

2. **召唤物系统**：`SummonedUnit` 组件 + `sourceBuffId` 关联，净化 buff 时级联删除召唤物（独立提案）。

3. **伤害元素标签**：`DamageEvent` 增加 `elementTags` 或 `DamageElement` 枚举，支持元素反应（独立提案）。

4. **Buff UI 系统**：读取 `Buff.tags` / `BuffBehavior.icon` 显示 UI 列表（独立提案）。

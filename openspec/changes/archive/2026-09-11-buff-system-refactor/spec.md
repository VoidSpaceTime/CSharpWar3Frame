# Spec：Buff 组件模型（重构后）

- **变更 ID**：buff-system-refactor
- **关联 capability**：Buff 生命周期 / 属性贡献 / DoT / 净化

## 组件模型

### `Buff`（IComponent，每 buff 实体必挂）

```csharp
public struct Buff : IComponent
{
    public string buffId;        // 类型 ID（同 key 唯一判定，如 "control:stun"）
    public long buffInstanceId;  // 实例 ID（全局唯一，级联清理）
    public BuffKind kind;        // Attribute / Tick / PureTag
    public BuffTag tags;         // [Flags] 分类标签
    public Entity caster;        // 伤害/效果来源单位（创建时解析）
    public float tickInterval;   // Tick 型：周期间隔（秒，0 = 不 tick）
    public string? tickActionId; // Tick 型：行为 ID（注册表）
    public float tickValue;      // Tick 型：每跳数值
    public float lastTick;       // Tick 型：内部计时
}
```

### `BuffKind`

| 成员 | ModifyValue | Tick | attrTypeId 语义 | 示例 |
|---|---|---|---|---|
| `Attribute` | 挂 | 否 | 贡献目标属性 | 战吼加攻击、Stun 门闩 |
| `Tick` | 不挂 | 是 | 载体属性（净化反查）| DoT 中毒 |
| `PureTag` | 不挂 | 否 | 载体属性或 0 | 燃油标记（TriggerSpec 反应用）|

### `BuffBehavior`（每 buff 实体必挂，Query 锚点 + 表现配置）

```csharp
public struct BuffBehavior : IComponent
{
    public string? icon; // UI 图标路径
}
```

### `[Flags] BuffTag`

```csharp
[Flags]
public enum BuffTag
{
    None = 0, Debuff = 1<<0, Control = 1<<1, Stun = 1<<2,
    Root = 1<<3, Silence = 1<<4, DoT = 1<<5,
    Fire = 1<<6, Frost = 1<<7, Poison = 1<<8,
}
```

净化规则：`(tags & BuffTag.Debuff) != 0`。
免疫规则：按元素位匹配（Fire 免疫清 Fire 位 debuff）。

### `BuffSpec`（工厂参数，只增 kind/tags 类型）

构造定型：`kind` 在构造时指定；`Tick` 型强制 value=0（不贡献属性）、attrTypeId=载体。

## 行为契约

1. **创建**（CreateBuffInternal）：`Buff + BuffBehavior + ModifyTarget(attr) + ModifySource(source) + Duration + [ModifyValue 按 kind] + [BuffStacks max>1]`；创建后 `attr.AddTag<AttrDirty>()`。
2. **刷新**（HandleExistingBuff）：六策略（Independent/Replace/ReplaceIfLonger/RefreshDuration/AddStack/RefreshAndStack）；决策读**新 spec.onDuplicate**；打脏经 `DirtyAttr` 统一；Replace/ReplaceIfLonger 删旧建新。
3. **到期**：DurationSystem 递减 remaining → DurationExpired → BuffDurationSystem（`Buff+BuffBehavior+Duration`）翻译 BuffExpired → BuffExpireSystem（`Buff+BuffBehavior+ModifyTarget`）删实体 + 打脏。
4. **查询**：`FindBuffByIdOnUnit` 读 `Buff.buffId`（不读 BuffBehavior）；索引命中 O(1) / 全扫回退。
5. **DoT tick**：BuffTickSystem（`Buff+Duration`，0.05s）累积 lastTick，到点收集、循环外执行 BuffTickActionRegistry 行为；`DealDamageTickAction` 用 `Buff.caster` 作伤害源。
6. **净化**：`PurgeDebuffs` 位运算收集 Debuff → 删实体 → 打脏受影响属性。
7. **移除**：RemoveBuff 删单 buff + 打脏；RemoveAllBuffs 删全部 + 打脏。

## 迁移

- `List<string>` tags → `BuffTag`：便捷方法语义等价迁移（Debuff/Control/Stun → 位组合）。
- 删 `BuffDuration`：总时长读 `Duration.total`。
- 删 `BuffBehavior.refreshBehavior/buffId/removeAllStacksOnExpire`。
- `DurationSystem` order 不变；BuffDurationSystem order 40 Query 收窄。

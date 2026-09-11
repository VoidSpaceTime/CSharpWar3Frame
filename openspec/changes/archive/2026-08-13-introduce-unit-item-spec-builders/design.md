## Context

当前项目已有三条相关 authoring 路径：

1. 技能模板逐步迁移到 `AbilitySpecBuilder` / `AbilityBehaviorBuilder` / `AbilityEffectSpecBuilder`。
2. 单位模板仍直接手写 `UnitBase` 与属性组件，例如 `FootmanTemplate.Configure(Entity e)`。
3. 物品模板直接手写 `ItemBase`、属性贡献、`AbilityBase`、`ProjectileData`、`AreaSearchData`、`DamageEffectData` 等组件。

这导致模板层代码既承担“定义这个单位/物品是什么”，又暴露大量底层组件细节。Unit/Item 需要跟随技能 authoring 风格统一，但不能把 runtime 创建和状态切换塞进 builder。

## Goals / Non-Goals

**Goals:**

- 新增 `UnitSpecBuilder` 作为单位模板配置入口。
- 新增 `ItemSpecBuilder` 作为物品模板配置入口。
- 保持 `[UnitTemplate]` / `[ItemTemplate]` 与 `IUnitTemplate.Configure` / `IItemTemplate.Configure` 现有注册模式。
- 保持 `UnitHelper` / `ItemHelper` 的 runtime helper 职责。
- 让模板示例更接近自然配置语言，减少直接散写组件。

**Non-Goals:**

- 不在 builder 中直接调用 `EntityStore.CreateEntity()` 创建运行时世界对象。
- 不在 builder 中执行装备、卸下、丢弃、杀死、移除、移动等流程。
- 不改变 Source Generator、FrameBuild、CLI 行为。
- 不在第一阶段覆盖全部高级单位 AI、掉落表、装备槽规则或物品使用流程。

## Naming Decisions

### 1. Unit authoring: UnitSpec

**Decision:** 新增 `UnitSpec` / `UnitSpecData` / `UnitSpecBuilder`。

职责：

- 单位模板 ID 与显示名称。
- 基础属性：生命、魔法、攻击力、攻击间隔、攻击距离、移动速度等。
- 技能引用：挂载一个或多个 ability template id，具体授予流程仍由现有 ability slot/helper 或后续系统承接。
- 物品槽容器：声明最大槽位数、初始槽位状态。
- 标签/扩展组件：允许少量显式扩展入口，但不鼓励任意业务逻辑塞入 builder。

候选 API 形态：

```csharp
UnitSpecBuilder
    .Create("footman")
    .Name("步兵")
    .Attr(AttributeHelper.Health, 420)
    .Attr(AttributeHelper.Mana, 0)
    .Attr(AttributeHelper.Damage, 24)
    .ItemSlots(6)
    .Ability("fire_blast")
    .BuildTo(entity);
```

`BuildTo(Entity entity)` 只把模板数据写入传入 entity，不创建运行时单位。

### 2. Item authoring: ItemSpec

**Decision:** 新增 `ItemSpec` / `ItemSpecData` / `ItemSpecBuilder`。

职责：

- 物品模板 ID、显示名称、堆叠数量、最大堆叠。
- 可使用、可消耗、是否实例化等 `ItemBase` 语义。
- 装备属性贡献，例如增加生命、攻击力或其他属性。
- 使用行为引用：优先引用 `AbilitySpec` / ability template id；简单一次性效果可以包装为 item use spec，但不在第一阶段扩展成完整物品行为 DSL。

候选 API 形态：

```csharp
ItemSpecBuilder
    .Create("amulet_of_vigor")
    .Name("力量护符")
    .Stack(max: 1)
    .Usable(consumable: false)
    .Attr(AttributeHelper.Health, ModifyType.Flat, 150)
    .UseEffect(AbilityEffectSpecBuilder
        .Chain()
        .Heal(AbilityValue.Constant(120f))
        .Build())
    .BuildTo(item);
```

卷轴类物品推荐复用技能 authoring：

```csharp
ItemSpecBuilder
    .Create("scroll_fireball")
    .Name("火球卷轴")
    .Stack(max: 10)
    .Usable(consumable: true)
    .UseAbility("scroll_fireball_cast")
    .BuildTo(item);
```

如果后续需要内联使用技能，也应复用 `AbilitySpecBuilder`，而不是在 `ItemSpecBuilder` 内重造技能 DSL。

### 3. Runtime boundary

`UnitSpecBuilder` 和 `ItemSpecBuilder` 不承担运行时入口职责。

保留边界：

- `UnitHelper.CreateUnit(templateName, player, x, y, facing)`：运行时创建单位入口，调用 `UnitTemplate.Create(...)`，补充生命周期入口状态。
- `UnitHelper.KillUnit(...)` / `RemoveUnit(...)` / `MoveToTask(...)`：运行时状态流转或请求入口。
- `ItemHelper.EquipToUnit(...)` / `UnequipToInventory(...)` / `DropToGround(...)`：物品状态切换入口。
- `System` / `NativeSystem`：消费组件和请求执行规则推进、同步或 War3 原生副作用。

## Data Shape

### UnitSpecData

候选字段：

- `string templateName`
- `string name`
- `List<AttributeSpec>` 或等价属性配置
- `List<string> abilityTemplateNames`
- `int? itemSlotCount`
- 可选扩展组件配置集合

### ItemSpecData

候选字段：

- `string templateName`
- `string name`
- `int stackCount`
- `int maxStack`
- `bool isUsable`
- `bool isConsumable`
- `bool isInstantiate`
- `List<AttributeContributionSpec>`
- `string? useAbilityTemplateName`
- `AbilityEffectSpec? useEffectSpec`

## Migration / Phasing

1. 定义 OpenSpec 工件并等待审核。
2. 第一阶段实现最小 `UnitSpecBuilder`：`Create`、`Name`、`Attr`、`ItemSlots`、`Ability`、`BuildTo`。
3. 第一阶段实现最小 `ItemSpecBuilder`：`Create`、`Name`、`Stack`、`Usable`、`Attr`、`UseAbility`、`UseEffect`、`BuildTo`。
4. 迁移 `FootmanTemplate`、`AmuletOfVigorTemplate`、`ScrollFireballTemplate` 作为示例。
5. 构建 `War3Frame` 与 `Projects/test`。
6. 后续如要扩展单位 AI、物品主动使用流程、掉落表、装备限制，另开提案，不塞进本次最小 authoring 层。

## Risks / Trade-offs

- [风险] `SpecBuilder` 变成万能组件写入器，削弱 ECS 显式性。  
  [缓解] 第一阶段只提供高频 authoring 方法；任意组件扩展若需要，必须命名为显式 escape hatch，并避免示例滥用。

- [风险] `ItemSpecBuilder.UseEffect` 与 `AbilitySpecBuilder` 职责重叠。  
  [缓解] 卷轴/主动使用物品优先 `UseAbility`；`UseEffect` 只用于非常简单的一次性效果，复杂流程转为 ability。

- [风险] `UnitSpecBuilder.Ability(...)` 的授予时机不清晰。  
  [缓解] 本阶段只表达模板拥有的 ability id；实际授予、槽位、冷却状态仍由 ability helper/system 或后续提案明确。

- [风险] 旧模板和新 builder 并存造成风格不一致。  
  [缓解] 先迁移 `Projects/test` 的代表性模板作为推荐风格，旧写法暂不删除。

## Open Questions

- `UnitSpecBuilder.Ability("id")` 是否应该立刻创建 ability entity，还是只写入待授予配置由后续系统处理？推荐第一阶段只写配置，不创建子实体。
- `ItemSpecBuilder.UseEffect(...)` 是否应该保留，还是统一要求物品使用全部走 `UseAbility(...)`？推荐保留最小入口，但示例优先展示 `UseAbility`。
- 属性配置是否复用 `AbilityValue`，还是新增更简单的 `AttributeSpec`？推荐 Unit/Item 固定基础属性先用直接数值，公式型属性留给后续提案。

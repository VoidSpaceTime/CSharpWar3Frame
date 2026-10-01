# item-ground Specification

## Purpose
地面物品的 ECS 模拟：以物品实体（`ItemGroundTag` + `Position` + `ItemGroundVisual`）为运行时真相，由独立特效实体承载世界表现，并定义拾取转背包与丢弃/装备/销毁多路径视觉清理语义。

## Requirements

### Requirement: 地面物品持有 ECS 语义与独立视觉引用
地面物品实体 SHALL 带 `ItemGroundTag` 与 `Position`，并持有模型路径与特效引用（`ItemGroundVisual`）。地面视觉 MUST 是独立特效实体，`ItemGroundVisual.effect` 为独立实体引用。物品实体 MUST NOT 挂 `EffectBase`。销毁视觉 MUST NOT `DeleteEntity` 物品本身。

#### Scenario: 创建地面物品
- **WHEN** 以有效 `ItemBase` 与模型路径调用 `ItemHelper.CreateOnGround`
- **THEN** 实体为 `ItemGroundTag` + `Position` + `ItemGroundVisual`，并产生 `ItemCreateNativeRequest`；Native 消费后 `visual.effect` 为独立特效实体；世界表现来自特效而非原生物品

### Requirement: ItemCreateNativeRequest 只确保地面特效表现
`ItemCreateNativeRequest` SHALL 表示“确保地面特效表现”，MUST NOT 表示 `CreateItem`。字段 SHALL 为 `item`/`x`/`y`/`z`/`model`，MUST NOT 含 `itemTypeId` 或 `facing`。`ItemCreateNativeSystem` SHALL 消费请求后删除请求，且 MUST NOT 再抛 `NotImplementedException`。系统 SHALL 调用 `EffectHelper.CreatePosition(..., -1)` 并写回 `visual.effect`，MUST NOT 直接调用 `JassApi.AddSpecialEffect`。

#### Scenario: Native 消费创建请求
- **WHEN** `ItemCreateNativeSystem` 消费有效的 `ItemCreateNativeRequest`
- **THEN** 请求被删除，`visual.effect` 写回独立特效实体，且调用栈进入 `EffectHelper.CreatePosition` 而不是 `CreateItem` 或 `JassApi.AddSpecialEffect`

### Requirement: 特效句柄只在 EffectNativeSystem 内配对
特效原生句柄创建后 SHALL `HandleAdd`，销毁前 SHALL `HandleRemove`，配对 MUST 在 `EffectNativeSystem` 内相邻完成。Item 层 SHALL 只调用 `EffectHelper`。`ItemHelper.CreateOnGround` / `RequestPickup` / `EquipToUnit` SHALL 只写 ECS 意图，MUST NOT 调用 `JassApi`/`DzApi`/`YDApi`。

#### Scenario: 句柄创建与销毁成对
- **WHEN** 地面视觉被创建并随后被销毁
- **THEN** `HandleAdd` 与 `HandleRemove` 都发生在 `EffectNativeSystem` 内，Item 层没有直接原生句柄操作

### Requirement: 拾取使用独立工作流并按距离与空槽判定
拾取 SHALL 由 `ItemPickupRequest` 工作流处理，MUST NOT 复用 `ItemUseSystem` 施法转换。地面物品无 `ItemOwner` 时，Use 校验 SHALL 失败。拾取成功条件 SHALL 为：目标为地面物品、未 `DestroyPending`、距离 ≤ `pickRange.radius`、背包有空槽。空槽判定 SHALL 复用 Attach 现有逻辑：`container.currentCount < maxSlots` 或 `GetItemAtSlot` 空槽扫描。拾取失败 MUST NOT 改变物品 Tag、槽位或视觉。

#### Scenario: 拾取成功
- **WHEN** 地面物品在半径内、背包有空槽（`container.currentCount < maxSlots`），且调用 `ItemHelper.RequestPickup(user, item)`
- **THEN** `visual.effect` 在非空时被删除并清空，物品进入 Attach 状态（`ItemInventoryTag` + `ItemEquippedTag`），无 `ItemGroundTag`，无句柄泄漏

#### Scenario: 拾取失败（超距）
- **WHEN** 距离大于 `ItemGroundPickRange.radius` 且调用 `RequestPickup`
- **THEN** 请求被删除，物品仍为 `ItemGroundTag`，`visual.effect` 仍在，状态不变

#### Scenario: 无空槽拾取失败
- **WHEN** 地面物品在半径内但 Attach 现有空槽判定失败
- **THEN** 请求被删除，物品 Tag、槽位与视觉都不变

### Requirement: 同帧创建并拾取不得留下孤儿特效
拾取成功后，若 `visual.effect` 非空，系统 SHALL 调用 `EffectHelper.Destroy`、清 `visual.effect`、去 `ItemGroundTag` 并进入 Attach。若同帧 `visual.effect.IsNull`，系统 SHALL 跳过特效销毁，并额外删除未消费的 `ItemCreateNativeRequest`。

#### Scenario: 同帧 Create+Pickup
- **WHEN** 同帧创建并拾取地面物品，且 `ItemCreateNativeSystem` 尚未消费请求
- **THEN** `visual.effect.IsNull`，跳过 `EffectHelper.Destroy`，删除未消费的 `ItemCreateNativeRequest`，物品进入背包，无崩溃，无孤儿特效

### Requirement: 丢弃、直接装备与销毁都必须先清理地面视觉
从背包或装备丢到地面时，`Detach(dropToGround: true)` SHALL 补 `ItemGroundVisual` 与 `ItemCreateNativeRequest`。`Attach` / `EquipToUnit` / `BeginDestroy` / `ItemCompanionDeferredDeleteSystem` 在物品仍在地面且 `visual.effect` 有效时，MUST 先拆特效再继续。

#### Scenario: 丢弃再落地
- **WHEN** 已装备物品调用 `ItemHelper.DropToGround(item, x, y, z)`
- **THEN** 物品回到 `ItemGroundTag` + `Position`，并再次产生 `ItemCreateNativeRequest`，特效重新创建

#### Scenario: 地面物品直接装备
- **WHEN** 地面物品带有效 `visual.effect`，并调用 `ItemHelper.EquipToUnit(item, unit, slot)` 或 `Attach`
- **THEN** 先销毁 `visual.effect` 并清引用，再进入装备状态，无句柄泄漏

#### Scenario: 销毁地面物品
- **WHEN** 地面物品带有效 `visual.effect`，并调用 `ItemHelper.RequestDestroy(item)`，或进入 `BeginDestroy` / `ItemCompanionDeferredDeleteSystem`
- **THEN** 先销毁 `visual.effect` 并清引用，再删除物品实体，无孤儿特效

### Requirement: 本能力禁止原生物品与新事件实体
本能力路径 MUST NOT 调用 `CreateItem`，MUST NOT 持有原生物品句柄。`ItemCreateNativeSystem` MUST NOT 直接 `AddSpecialEffect`，只调用 `EffectHelper.CreatePosition`。P0 MUST NOT 要求 UI 按键提示；P1 另案。本 change MUST NOT 引入新 `XxxEvent` 实体，也不涉及 `TriggerEventMarker`。

#### Scenario: 禁止原生物品
- **WHEN** 检查地面创建路径（`ItemCreateNativeSystem`、`ItemHelper`、`ItemPickupWorkflowSystem`）
- **THEN** 无 `CreateItem`，Item 层无 `JassApi.AddSpecialEffect`，无原生物品句柄字段作为真相

#### Scenario: P0 不依赖 UI 提示
- **WHEN** 验收 P0
- **THEN** 没有 P1 UI 按键提示代码也可以通过 P0 验收

#### Scenario: 不引入事件实体
- **WHEN** 审查本 change 的组件与系统
- **THEN** 无独立 `XxxEvent` 实体，也不要求 `TriggerEventMarker`

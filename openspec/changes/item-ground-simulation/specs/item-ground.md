# Capability: ItemGroundSimulation

Change ID: `item-ground-simulation`  
日期: 2026-08-31  
修订: 2026-09-01（opus5 审查后）

地面物品由 ECS 实体持有语义真相，用独立特效实体做世界表现，用独立拾取请求转入背包。不使用 War3 `CreateItem` 与原生物品栏句柄。物品实体不挂 `EffectBase`。

## Requirements

| ID | 描述 | 验证方式 |
|---|---|---|
| IG-01 | 地面物品实体必须带 `ItemGroundTag` 与 `Position`，并持有模型路径与特效引用（`ItemGroundVisual`）。 | 代码审查 + 创建路径走查 |
| IG-02 | 地面视觉必须是独立特效实体，`ItemGroundVisual.effect` 为独立实体引用。销毁视觉不得 `DeleteEntity` 物品本身。 | 组件注释禁止物品挂 `EffectBase`；对照 `EffectNativeSystem` 销毁路径（§12 行 104-114）只删特效实体 |
| IG-03 | `ItemCreateNativeRequest` 表示"确保地面特效表现"，不表示 `CreateItem`。字段为 `item`/`x`/`y`/`z`/`model`，无 `itemTypeId` 或 `facing`（CreateItem 参数）。 | 组件字段审查（§9）；Native 实现无 `CreateItem` 调用 |
| IG-04 | `ItemCreateNativeSystem` 消费请求后删除请求，且不得再抛 `NotImplementedException`。调用 `EffectHelper.CreatePosition(..., -1)`，写回 `visual.effect`，**禁止**直接 `JassApi.AddSpecialEffect`。 | 构建 + 源码审查 |
| IG-05 | 特效原生句柄创建后 `HandleAdd`（§12 行 140-142），销毁前 `HandleRemove`（§12 行 112），配对在 `EffectNativeSystem` 内相邻完成。Item 层只调 `EffectHelper`。 | 句柄审查清单（design.md 第 8 节） |
| IG-06 | `ItemHelper.CreateOnGround` / `RequestPickup` / `EquipToUnit` 只写 ECS 意图，不调用 `JassApi`/`DzApi`/`YDApi`。 | Helper 源码审查（§11） |
| IG-07 | 拾取由 `ItemPickupRequest` 工作流处理，不复用 `ItemUseSystem` 施法转换。地面物品无 `ItemOwner`，Use 校验失败。 | 系统职责审查 |
| IG-08 | 拾取成功条件：目标为地面物品、未 `DestroyPending`、距离 ≤ `pickRange.radius`、背包有空槽（复用 Attach 现有逻辑：`container.currentCount < maxSlots` 或 `GetItemAtSlot` 空槽扫描，§10 行 194-228）。 | 工作流分支走查 |
| IG-09 | 拾取成功后：若 `visual.effect` 非空则 `EffectHelper.Destroy` 且句柄注销、清 `visual.effect`、去 `ItemGroundTag`、进 Attach（背包/装备 Tag 与槽位）。同帧边界：若 `visual.effect.IsNull`，跳过特效销毁，**额外删除未消费的 ItemCreateNativeRequest**。 | Attach 衔接走查 + 同帧边界测试 |
| IG-10 | 拾取失败不得改变物品 Tag、槽位或视觉。 | 失败分支走查 |
| IG-11 | 从背包/装备丢到地面时，`Detach(dropToGround: true)` 必须补 `ItemGroundVisual` + `ItemCreateNativeRequest`。 | Detach 路径审查（§10 行 244-274） |
| IG-12 | `Attach` / `EquipToUnit` / `BeginDestroy` / `ItemCompanionDeferredDeleteSystem` 若物品仍在地面且 `visual.effect` 有效，必须先拆特效再继续，避免句柄泄漏。 | 多路径销毁审查（design.md 4.1、tasks.md P0 工作流） |
| IG-13 | 本能力路径不得调用 `CreateItem`，不得持有原生物品句柄。`ItemCreateNativeSystem` 不得直接 `AddSpecialEffect`，只调 `EffectHelper.CreatePosition`。 | 全仓库地面相关源码 grep（`CreateItem` / `JassApi.AddSpecialEffect`） |
| IG-14 | P0 不要求 UI 按键提示；P1 另案。 | 范围检查：无 P1 UI 代码也可验收 P0 |
| IG-15 | `dotnet build War3Frame/War3Frame.csproj` 成功（0 错误）。 | 构建日志 |
| IG-16 | 本 change 不引入新 `XxxEvent` 实体，不涉及 `TriggerEventMarker`（`EventCleanupSystem` order 132 边界）。 | 组件与系统审查：无独立事件实体 |

## Acceptance scenarios

### 创建地面物品

- **Given** 有效 `ItemBase` 与模型路径  
- **When** 调用 `ItemHelper.CreateOnGround`  
- **Then** 实体为 `ItemGroundTag` + `Position` + `ItemGroundVisual`，并产生 `ItemCreateNativeRequest`；Native 消费后 `visual.effect` 为独立特效实体；世界表现来自特效而非原生物品。

### 拾取成功

- **Given** 地面物品在半径内且背包有空槽（`container.currentCount < maxSlots`）  
- **When** `ItemHelper.RequestPickup(user, item)`  
- **Then** `visual.effect` 删除（若非空）、`visual.effect` 清空、物品进入 Attach 状态（`ItemInventoryTag` + `ItemEquippedTag`）、无 `ItemGroundTag`、无句柄泄漏。

### 同帧 Create+Pickup

- **Given** 同帧创建并拾取地面物品，`ItemCreateNativeSystem` 尚未消费请求  
- **When** `ItemPickupWorkflowSystem` 执行拾取  
- **Then** `visual.effect.IsNull`，跳过 `EffectHelper.Destroy`，**删除未消费的 ItemCreateNativeRequest**，物品进入背包，无崩溃，无孤儿特效。

### 拾取失败（超距）

- **Given** 距离大于 `ItemGroundPickRange.radius`  
- **When** `RequestPickup`  
- **Then** 请求被删除，物品仍为 `ItemGroundTag`，`visual.effect` 仍在，状态不变。

### 丢弃再落地

- **Given** 已装备物品  
- **When** `ItemHelper.DropToGround(item, x, y, z)`  
- **Then** 物品回到 `ItemGroundTag` + `Position`，并再次产生 `ItemCreateNativeRequest`，特效重新创建。

### 地面物品直接装备

- **Given** 地面物品带有效 `visual.effect`  
- **When** `ItemHelper.EquipToUnit(item, unit, slot)` 或 `Attach`  
- **Then** 先销毁 `visual.effect`，清引用，再进入装备状态，无句柄泄漏。

### 销毁地面物品

- **Given** 地面物品带有效 `visual.effect`  
- **When** `ItemHelper.RequestDestroy(item)` 或 `BeginDestroy` / `ItemCompanionDeferredDeleteSystem`  
- **Then** 先销毁 `visual.effect`，清引用，再删除物品实体，无孤儿特效。

### 禁止原生物品

- **Given** P0 实现完成  
- **When** grep 地面创建路径（`ItemCreateNativeSystem`, `ItemHelper`, `ItemPickupWorkflowSystem`）  
- **Then** 无 `CreateItem`，无 `JassApi.AddSpecialEffect`（Item 层），无原生物品句柄字段作为真相。

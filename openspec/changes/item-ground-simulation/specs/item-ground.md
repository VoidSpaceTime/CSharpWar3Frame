# Capability: ItemGroundSimulation

Change ID: `item-ground-simulation`  
日期: 2026-08-31

地面物品由 ECS 实体持有语义真相，用特效模型做世界表现，用独立拾取请求转入背包。不使用 War3 `CreateItem` 与原生物品栏句柄。

## Requirements

| ID | 描述 | 验证方式 |
|---|---|---|
| IG-01 | 地面物品实体必须带 `ItemGroundTag` 与 `Position`，并持有模型路径（`ItemGroundVisual` 或等价）。 | 代码审查 + 创建路径走查 |
| IG-02 | 地面视觉必须是独立特效实体（或等效代理），销毁视觉不得 `DeleteEntity` 物品本身。 | 对照 `EffectNativeSystem` 销毁路径；禁止物品挂 `EffectBase` 作为被删主体 |
| IG-03 | `ItemCreateNativeRequest` 表示“确保地面特效表现”，不表示 `CreateItem`。 | 组件字段与 Native 实现无 `CreateItem` |
| IG-04 | `ItemCreateNativeSystem` 消费该请求后删除请求，且不得再抛 `NotImplementedException`。 | 构建 + 源码 |
| IG-05 | 特效原生句柄创建后 `HandleAdd`，销毁前 `HandleRemove`，配对在同一 Native 执行点相邻完成。默认复用 `EffectNativeSystem`。 | 句柄审查清单 |
| IG-06 | `ItemHelper.CreateOnGround` / `RequestPickup` 只写 ECS 意图，不调用 `JassApi`/`DzApi`。 | Helper 源码审查 |
| IG-07 | 拾取由 `ItemPickupRequest` 工作流处理，不复用 `ItemUseSystem` 施法转换。 | 系统职责审查 |
| IG-08 | 拾取成功条件：目标为地面物品、未 DestroyPending、使用者与物品距离 ≤ 拾取半径、背包有空槽。 | 工作流分支走查 |
| IG-09 | 拾取成功后：特效销毁且句柄注销、清除有效 visual 引用、去掉 `ItemGroundTag`、进入现有 Attach（背包/装备 Tag 与槽位）。 | 与 `ItemLifecycleOperations.Attach` 衔接走查 |
| IG-10 | 拾取失败不得改变物品 Tag、槽位或视觉。 | 失败分支走查 |
| IG-11 | 从背包/装备丢到地面时，必须重新请求地面视觉。 | Detach 路径审查 |
| IG-12 | 销毁仍在地面的物品前必须拆除特效，避免句柄泄漏。 | Destroy 工作流审查 |
| IG-13 | 本能力路径不得调用 `CreateItem`，不得持有原生物品句柄。 | 全仓库地面相关源码检索 |
| IG-14 | P0 不要求 UI 按键提示；P1 另案。 | 范围检查：无 P1 UI 代码也可验收 P0 |
| IG-15 | `dotnet build War3Frame/War3Frame.csproj` 成功（0 错误）。 | 构建日志 |

## Acceptance scenarios

### 创建地面物品

- **Given** 有效 `ItemBase` 与模型路径  
- **When** 调用 `CreateOnGround`  
- **Then** 实体为 `ItemGroundTag` + `Position`，并产生已被或将被 Native 消费的表现请求；世界表现来自特效而非原生物品。

### 拾取成功

- **Given** 地面物品在半径内且背包有空槽  
- **When** `RequestPickup`  
- **Then** 视觉实体删除、物品进入 Attach 状态、无 GroundTag。

### 拾取失败（超距）

- **Given** 距离大于 `ItemGroundPickRange.radius`  
- **When** `RequestPickup`  
- **Then** 请求被丢弃，物品仍为地面且视觉仍在。

### 丢弃再落地

- **Given** 已装备物品  
- **When** `DropToGround`  
- **Then** 物品回到 `ItemGroundTag` 并再次请求特效表现。

### 禁止原生物品

- **Given** P0 实现完成  
- **When** 检索地面创建路径  
- **Then** 无 `CreateItem`，无原生物品句柄字段作为真相。

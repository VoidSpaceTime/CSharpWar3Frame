# 设计：地面物品特效模拟

Change ID: `item-ground-simulation`  
日期: 2026-08-31  
修订: 2026-09-01（opus5 审查后）

## 1. 决策：为什么不用原生物品

War3 `CreateItem` 会生成可被攻击、可死亡、可被单位默认拾取、可进入原生 6 格物品栏的 widget。自定义 ECS 背包、companion ability、受控销毁（`ItemDestroyPendingTag`）无法干净接管这些语义。xlik 用特效模型摆在地上，拾取完全由脚本控制。本仓库采用同一思路：

- ECS 物品实体是唯一真相。
- 地上看到的模型是特效代理，不是 `item` 句柄。
- 背包格子是 `ItemSlotContainer` + `ItemOwner`，不是原生物品栏。

因此 `ItemCreateNativeRequest` 不再表示 `CreateItem`。

## 2. 强制结构：独立特效实体，物品实体不挂 EffectBase

`EffectNativeSystem` 在 `EffectDestroyRequest` 后会 `entity.DeleteEntity()`（见 §12 行 113）。若把 `EffectBase` 直接挂在物品上，拾取销毁视觉会删掉物品实体。

**强制要求**：

```
物品实体（长期）          特效实体（视觉代理，duration = -1）
ItemBase                  EffectBase
ItemGroundTag             Position
Position                  EffectNative（Native 层）
ItemGroundVisual --------> effect Entity 引用（独立实体）
```

- `ItemGroundVisual.effect` 是独立特效实体引用。`IsNull` 或 `effect.IsNull` 表示无表现或已销毁。
- 创建：`EffectHelper.CreatePosition(model, x, y, z, duration: -1)`。返回独立特效实体，挂 `EffectBase` + `Position` + `Duration`。
- 销毁：`EffectHelper.Destroy(effect)`，由现有 `EffectNativeSystem` 执行 `HandleRemove` + `DestroyEffect` + `entity.DeleteEntity()`（只删特效实体，不删物品）。

`ItemCreateNativeSystem` 职责：消费 `ItemCreateNativeRequest`，调用 `EffectHelper.CreatePosition`，将返回的特效实体写回物品的 `ItemGroundVisual.effect`，然后删除请求。它**不得**直接调用 `JassApi.AddSpecialEffect`，必须通过 Helper 委托给现有 `EffectNativeSystem`。

### 2.1 ItemCreateNativeRequest 生命周期与挂载

当前 `ItemCreateNativeRequest` 在 `Items.cs` 中定义为 `IComponent`（§9）。若该请求直接挂在物品实体上，删除请求组件（`RemoveComponent<ItemCreateNativeRequest>`）不会删物品；但若请求作为独立实体且用 `DeleteEntity` 删除，则不影响物品。

**推荐实现路径**：

- **选项 A**（独立请求实体）：`ItemHelper.CreateOnGround` 创建独立请求实体，挂 `ItemCreateNativeRequest` 组件并引用物品实体。`ItemCreateNativeSystem` 消费后 `DeleteEntity(requestEntity)`。
- **选项 B**（组件挂物品实体）：请求作为组件挂在物品实体上。`ItemCreateNativeSystem` 消费后 `item.RemoveComponent<ItemCreateNativeRequest>()`，不删物品。

实施时选择其一，但必须在 summary 中说明实际路径，并确保删除请求不会误删物品实体。

## 3. 组件

### 3.1 复用

| 组件/Tag | 用途 |
|---|---|
| `ItemGroundTag` | 地上分类状态 |
| `Position` | 地面坐标真相 |
| `ItemBase` | 模板、名称、堆叠 |
| `ItemAttachRequest` / `ItemRemoveRequest` | 进出背包 |
| `EffectBase` / `EffectDestroyRequest` | 仅挂在特效实体 |

### 3.2 新增（P0）

```csharp
/// <summary>地面物品视觉代理引用与模型路径。禁止物品实体挂 EffectBase。</summary>
public struct ItemGroundVisual : IComponent
{
    public string model;
    public Entity effect; // 独立特效实体引用；IsNull 表示无表现或已销毁
}

/// <summary>地面拾取半径（世界坐标）。</summary>
public struct ItemGroundPickRange : IComponent
{
    public float radius; // 默认建议 120~200，实施时给常量
}

/// <summary>请求确保地面物品具备特效表现。一次性，Resolve 后删除。</summary>
public struct ItemCreateNativeRequest : IComponent
{
    public Entity item;      // 目标物品实体
    public float x;
    public float y;
    public float z;
    public string model;
    // 废弃 itemTypeId / facing 作为 CreateItem 参数；facing 若需旋转走 EffectTransform
}

/// <summary>请求拾取地面物品到使用者背包。一次性。</summary>
public struct ItemPickupRequest : IComponent
{
    public Entity user;
    public Entity item;
}
```

`ItemCreateNativeRequest` 现有字段（§9：`x, y, facing, itemTypeId`）无生产调用方，实施时直接替换，不做 CreateItem 兼容垫片。

### 3.3 P1（可选，本阶段只设计）

- `ItemGroundHintTag` 或 UI 状态组件：当前本地玩家选中的可拾取地面物品。
- 不在 P0 实现按键绑定与 Frame 提示条。

## 4. 系统职责

### 4.1 业务 / 工作流（禁止 JassApi）

| 系统 | 职责 |
|---|---|
| `ItemHelper.CreateOnGround` | 创建或复用物品实体，打 `ItemGroundTag`，写 `Position` / `ItemGroundVisual` / `ItemGroundPickRange`，创建 `ItemCreateNativeRequest`（独立实体或挂物品，见 2.1） |
| `ItemPickupWorkflowSystem` | 消费 `ItemPickupRequest`：校验 `ItemGroundTag`、距离、非 `DestroyPending`、背包空槽（复用 Attach 现有逻辑：`container.currentCount < maxSlots` 或空槽扫描，见 §10 行 194-228）；**同帧边界**：若 `visual.effect.IsNull`（Create+Pickup 同帧未完成 Native），跳过特效销毁，**额外删除未消费的 ItemCreateNativeRequest**；成功则 `EffectHelper.Destroy(visual.effect)` + 清 `visual.effect` + `ItemAttachRequest`；失败删请求不改状态 |
| `ItemLifecycleOperations.Detach` | `dropToGround` 时除现有 Tag+Position 外（§10 行 265-269），补 `ItemGroundVisual` + `ItemCreateNativeRequest` |
| `ItemLifecycleOperations.Attach` | 若物品仍带 `ItemGroundTag` 且 `visual.effect` 有效，**先销毁视觉**再 Attach（§10 行 183-228 现有 Attach 未处理此路径，需补充） |
| `BeginDestroy` (§10 行 121-128) | 若物品带 `ItemGroundTag` 且 `visual.effect` 有效，**先销毁视觉**再进入 `DestroyPending` |
| `ItemCompanionDeferredDeleteSystem` (§10 行 134-173) | 删物品前，若仍带 `ItemGroundTag` 且 `visual.effect` 有效，**先销毁视觉** |
| `ItemHelper.EquipToUnit` (§11 行 42-52) | 若物品带 `ItemGroundTag` 且 `visual.effect` 有效，**先销毁视觉**再创建 `ItemAttachRequest` |
| `ItemUseSystem` | **不**处理地面拾取。地面物品无 `ItemOwner`，现有校验已失败 |

### 4.2 Native

| 系统 | 职责 |
|---|---|
| `ItemCreateNativeSystem` | 消费 `ItemCreateNativeRequest`；对有效物品调用 `EffectHelper.CreatePosition(model, x, y, z, -1)`（**duration = -1 表示永久，见 §13 行 19**）；写回物品的 `ItemGroundVisual.effect`；删除请求（独立实体用 `DeleteEntity(requestEntity)`，挂物品用 `item.RemoveComponent<ItemCreateNativeRequest>()`）。**禁止**直接调 `JassApi.AddSpecialEffect` |
| `EffectNativeSystem`（现有，§12） | 真正 `AddSpecialEffect` + `HandleAdd`（行 118-144）；销毁时 `HandleRemove` + `DestroyEffect` + `DeleteEntity`（行 104-114） |
| 拾取/销毁时的视觉拆除 | 工作流调用 `EffectHelper.Destroy(visual.effect)`（§13 行 99-102），不在工作流里调 JassApi |

拾取/销毁后必须清除 `ItemGroundVisual.effect` 引用（设为 `default` 或 `Entity.Null`），不得保留指向已删实体的悬垂引用。

## 5. Helper API 草案

```csharp
public static class ItemHelper
{
    /// <summary>在地面创建物品实体并请求特效表现。</summary>
    /// <param name="data">物品基础数据（ItemBase）</param>
    /// <param name="model">特效模型路径</param>
    /// <param name="x">世界坐标 X</param>
    /// <param name="y">世界坐标 Y</param>
    /// <param name="z">世界坐标 Z（默认 0，由地形高度决定）</param>
    /// <param name="pickRadius">拾取半径（默认 160）</param>
    /// <returns>物品实体</returns>
    public static Entity CreateOnGround(ItemBase data, string model, float x, float y, float z = 0, float pickRadius = 160f);

    /// <summary>已有物品丢到地面（内部写 ItemRemoveRequest 或直接 Detach 路径）。</summary>
    public static void DropToGround(Entity item, float x, float y, float z = 0); // 已有，实施时补视觉请求

    /// <summary>请求拾取地面物品到使用者背包。不保证成功（需校验距离、空槽等）。</summary>
    public static Entity RequestPickup(Entity user, Entity item);

    /// <summary>将物品装备到单位身上。若物品仍带 ItemGroundTag 且 visual.effect 有效，先销毁视觉。</summary>
    public static void EquipToUnit(Entity item, Entity unit, int slotIndex); // 已有，实施时补地面视觉销毁
}
```

Helper 只写 ECS。不调用 `AddSpecialEffect` / `DestroyEffect`。

## 6. 四态 Tag 流转

```
                    CreateOnGround
                          |
                          v
                   [ItemGroundTag]
                    Position + Visual
                          |
            Pickup 成功 / Attach
                          v
              [ItemInventoryTag] + [ItemEquippedTag]
              （现有 Attach 同时打两 Tag，本提案不改该历史行为）
                          |
         UnequipToInventory          DropToGround
                |                         |
                v                         v
      仅 InventoryTag               GroundTag + Visual 重建
                          |
                    StoredTag
                   （本提案不改仓库）
```

约束：

- 地上不得同时有 `ItemOwner` / `ItemSlotIndex`。
- 背包/装备不得保留有效 `ItemGroundVisual.effect`（拾取时先 Destroy 特效）。
- `ItemStoredTag` 与地面互斥（现有 Detach 已 `RemoveTag<ItemStoredTag>`）。

## 7. 拾取流程

```
ItemHelper.RequestPickup(user, item)
  -> ItemPickupRequest 实体
  -> ItemPickupWorkflowSystem
       前置校验: ItemGroundTag、距离、非 DestroyPending、空槽（复用 Attach 逻辑）
       失败: 距离/非地面/无空槽/DestroyPending -> 删请求，不改物品状态
       成功:
         同帧边界检查:
           if visual.effect.IsNull:
             // 同帧 Create+Pickup，Native 未完成特效创建
             跳过 EffectHelper.Destroy
             删除未消费的 ItemCreateNativeRequest（若仍存在）
           else:
             EffectHelper.Destroy(visual.effect)
         清 ItemGroundVisual.effect（设为 default）
         写 ItemAttachRequest(owner=user, item, slot)
  -> ItemAttachWorkflowSystem / ItemLifecycleOperations.Attach
       Remove ItemGroundTag, Add Inventory+Equipped, 槽位++
```

距离：`Distance2D(user.Position, item.Position) <= pickRange.radius`。无 `ItemGroundPickRange` 时用设计常量（建议 160），不得无限距离。

空槽判断：复用 `ItemLifecycleOperations.Attach` 现有逻辑（§10 行 194-228）：`container.currentCount < container.maxSlots` 或现有 `GetItemAtSlot` 空槽扫描，不重新发明算法。

## 8. 句柄配对路径（AGENTS.md 强制要求）

唯一允许路径（复用 Effect）：

1. 创建：`EffectNativeSystem.CreateNativeEffect`（§12 行 118-144）→ `AddSpecialEffect` → `HandleAdd`（行 140-142）。
2. 销毁：`EffectDestroyRequest` → `EffectNativeSystem.OnUpdate`（行 104-114）→ `HandleRemove` + `DestroyEffect` + `DeleteEntity`。

**Item 层句柄审查清单（P0 验收强制项）**：

- [ ] `ItemCreateNativeSystem` 是否直接调用 `JassApi.AddSpecialEffect`？**必须为否**。只允许调 `EffectHelper.CreatePosition`。
- [ ] `ItemHelper` 是否直接调用 `JassApi` / `DzApi` / `YDApi`？**必须为否**。
- [ ] `ItemPickupWorkflowSystem` 是否直接调用 `JassApi.DestroyEffect`？**必须为否**。只允许调 `EffectHelper.Destroy`。
- [ ] 拾取成功路径是否遗漏 `EffectHelper.Destroy(visual.effect)` 或同帧边界处理？**必须完整覆盖**。
- [ ] `BeginDestroy` / `ItemCompanionDeferredDeleteSystem` / `Attach` / `EquipToUnit` 若物品仍为 `ItemGroundTag` 且 `visual.effect` 有效，是否先拆视觉？**必须先拆**。
- [ ] 是否有第二条 `DestroyEffect` 分散点？**必须只有 EffectNativeSystem 一处**。

违反任一项视为句柄泄漏高风险，阻止验收。

## 9. 阶段拆分

### P0（本提案实施范围）

- 组件与 Request 语义
- `CreateOnGround` / `RequestPickup`
- Native 占位落地（转发 Effect）
- Detach 丢弃补视觉
- 地面销毁时拆特效
- 构建与句柄审查

### P1（后续提案）

- 本地玩家靠近地面物品时 UI 按键提示
- 提示与拾取键绑定
- 不阻塞 P0 归档

### 明确不做

- `CreateItem`、原生物品栏句柄、掉落物理、自动走过拾取、物品作为可攻击 widget

## 10. Native 分层自检（AGENTS.md War3 原生调用分层规则）

为什么不把 `AddSpecialEffect` 放进拾取工作流或 ItemHelper？

- 工作流层（`ItemPickupWorkflowSystem`）负责推进 ECS 状态（Tag、槽位、意图），不持有原生句柄语义。
- Helper 层（`ItemHelper`）是对外唯一修改入口，只写 ECS 状态和意图，不调用 War3 原生 API。
- Native 层（`EffectNativeSystem`）消费 ECS 真相并执行原生副作用，拥有句柄配对职责。

`ItemCreateNativeSystem` 职责仅为"确保地面物品有视觉表现"，不直接拥有句柄。它调用 `EffectHelper.CreatePosition` 委托给 Effect Native 层，避免开第二条 `AddSpecialEffect` + `HandleAdd` 路径。

若实施时发现必须在 Item Native 内直接 `AddSpecialEffect`（例如 EffectHelper 不满足需求），则必须在同一系统内相邻成对 `HandleAdd` + `HandleRemove`，且销毁路径唯一。

## 11. 测试要点（实施后）

- 创建地面：有 `ItemGroundTag`、`Position`、`ItemGroundVisual`；请求被消费后 `visual.effect` 非空且为独立特效实体。
- 拾取成功：无 `ItemGroundTag`，有 `ItemOwner`/`ItemSlotIndex`，`visual.effect` 引用已清除（`IsNull` 或 `default`），特效实体已删。
- 同帧 Create+Pickup：允许拾取，不崩溃，未消费的 `ItemCreateNativeRequest` 被删除，无孤儿特效。
- 超距拾取：请求被删，物品状态不变（仍为 `ItemGroundTag`），视觉仍在。
- 丢弃（Detach dropToGround）：从装备到地面后，物品重新带 `ItemGroundTag` + `Position`，并再次发出 `ItemCreateNativeRequest` 产生视觉。
- Attach / EquipToUnit / BeginDestroy 路径：若物品仍带 `ItemGroundTag` 且 `visual.effect` 有效，先销毁视觉再继续，无句柄泄漏。
- 源码无 `CreateItem` 调用（地面路径 grep 检查）。
- `dotnet build War3Frame/War3Frame.csproj` 0 错误。
- 句柄审查清单全通过（第 8 节）。

## 12. TriggerEventMarker 与事件清理

本 change **不引入**新的 `XxxEvent` 实体（如 `ItemPickupEvent`、`ItemDropEvent`）。拾取与丢弃通过 `ItemPickupRequest` / `ItemRemoveRequest` 意图表达，消费后删除请求实体，不产生独立事件实体供多监听者订阅。

因此本 change **不涉及** `TriggerEventMarker`（`EventCleanupSystem` order 132 边界）。若后续提案引入地面物品相关事件广播（如物品掉落事件供触发器监听），该提案必须为事件实体挂 `TriggerEventMarker`，并确保监听系统 order < 132。

## 13. 文件路径约定

实施时文件路径必须匹配现有仓库约定：

- 组件：`War3Frame/Src/Components/Item/Items.cs` 或新增 `War3Frame/Src/Components/Item/ItemGround.cs`
- 系统：`War3Frame/Src/Systems/Native/ItemCreateNativeSystem.cs`（已存在），`War3Frame/Src/Systems/Item/ItemPickupWorkflowSystem.cs`（新增）
- Helper：`War3Frame/Src/Helpers/ItemHelper.cs`（已存在）

不得虚构路径如 `Systems/Workflow/` 或 `Components/Ground/`，除非先在本 change 提案中明确新增目录结构并说明理由。

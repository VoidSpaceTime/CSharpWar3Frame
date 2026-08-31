# 设计：地面物品特效模拟

Change ID: `item-ground-simulation`  
日期: 2026-08-31

## 1. 决策：为什么不用原生物品

War3 `CreateItem` 会生成可被攻击、可死亡、可被单位默认拾取、可进入原生 6 格物品栏的 widget。自定义 ECS 背包、companion ability、受控销毁（`ItemDestroyPendingTag`）无法干净接管这些语义。xlik 用特效模型摆在地上，拾取完全由脚本控制。本仓库采用同一思路：

- ECS 物品实体是唯一真相。
- 地上看到的模型是特效代理，不是 `item` 句柄。
- 背包格子是 `ItemSlotContainer` + `ItemOwner`，不是原生物品栏。

因此 `ItemCreateNativeRequest` 不再表示 `CreateItem`。

## 2. 推荐结构：独立特效实体，物品实体不挂 EffectBase

`EffectNativeSystem` 在 `EffectDestroyRequest` 后会 `entity.DeleteEntity()`。若把 `EffectBase` 直接挂在物品上，拾取销毁视觉会删掉物品实体。

推荐：

```
物品实体（长期）          特效实体（视觉代理，duration = -1）
ItemBase                  EffectBase
ItemGroundTag             Position（拷贝或跟随）
Position                  EffectNative（Native 层）
ItemGroundVisual --------> effect Entity 引用
```

`ItemGroundVisual.effect` 为空表示尚未创建或已销毁。创建走 `EffectHelper.CreatePosition(model, x, y, z, duration: -1)`。销毁走 `EffectHelper.Destroy(effect)`，由现有 Effect Native 做 `DestroyEffect` + `HandleRemove`。

`ItemCreateNativeSystem` 职责收窄为：消费“需要地面表现”的请求，调用 Helper 创建特效并写回 `ItemGroundVisual`。它**不**直接 `AddSpecialEffect`，除非审查认定不能复用 Effect 路径。默认不新开第二条句柄路径。

若实施时发现必须在 Item Native 内创建特效，则该系统必须在 `AddSpecialEffect` 下一行 `HandleAdd`，销毁路径唯一且相邻 `HandleRemove`。

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
/// <summary>地面物品视觉代理引用与模型路径。</summary>
public struct ItemGroundVisual : IComponent
{
    public string model;
    public Entity effect; // 独立特效实体；IsNull 表示无表现
}

/// <summary>地面拾取半径（世界坐标）。</summary>
public struct ItemGroundPickRange : IComponent
{
    public float radius; // 默认建议 120~200，实施时给常量
}

/// <summary>请求确保地面物品具备特效表现。一次性，Resolve 后删除。</summary>
public struct ItemCreateNativeRequest : IComponent
{
    public Entity item;
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

`ItemCreateNativeRequest` 现有字段（`itemTypeId`、`facing`、无 `item` 引用）无生产调用方，实施时直接替换，不做 CreateItem 兼容垫片。

### 3.3 P1（可选，本阶段只设计）

- `ItemGroundHintTag` 或 UI 状态组件：当前本地玩家选中的可拾取地面物品。
- 不在 P0 实现按键绑定与 Frame 提示条。

## 4. 系统职责

### 4.1 业务 / 工作流（禁止 JassApi）

| 系统 | 职责 |
|---|---|
| `ItemCreateOnGround` 入口（Helper） | 创建或复用物品实体，打 `ItemGroundTag`，写 `Position` / `ItemGroundVisual` / `ItemGroundPickRange`，写 `ItemCreateNativeRequest` |
| `ItemPickupWorkflowSystem` | 消费 `ItemPickupRequest`：校验地面 Tag、距离、非 DestroyPending、背包空槽；成功则销毁视觉意图 + `ItemAttachRequest`；失败删除请求不改状态 |
| `ItemLifecycleOperations.Detach` | `dropToGround` 时除 Tag+Position 外，补 `ItemGroundVisual` + `ItemCreateNativeRequest` |
| `ItemUseSystem` | **不**处理地面拾取。地面物品无 `ItemOwner`，现有校验已会失败 |

### 4.2 Native

| 系统 | 职责 |
|---|---|
| `ItemCreateNativeSystem` | 消费 `ItemCreateNativeRequest`；对有效地面物品调用 `EffectHelper.CreatePosition`；写回 `ItemGroundVisual.effect`；删除 Request。不 throw |
| `EffectNativeSystem`（现有） | 真正 `AddSpecialEffect` + `HandleAdd`；销毁 `HandleRemove` + `DestroyEffect` |
| 拾取时的视觉拆除 | 工作流调用 `EffectHelper.Destroy`，不在工作流里调 JassApi |

拾取后清除 `ItemGroundVisual.effect`，移除或保留空组件均可，但不得留下指向已删实体的引用当真相。

## 5. Helper API 草案

```csharp
public static class ItemHelper
{
    /// <summary>在地面创建物品实体并请求特效表现。</summary>
    public static Entity CreateOnGround(ItemBase data, string model, float x, float y, float z = 0, float pickRadius = 160f);

    /// <summary>已有物品丢到地面（内部写 ItemRemoveRequest 或直接 Detach 路径）。</summary>
    public static void DropToGround(Entity item, float x, float y, float z = 0); // 已有，实施时接视觉

    /// <summary>请求拾取地面物品。不保证成功。</summary>
    public static Entity RequestPickup(Entity user, Entity item);
}
```

Helper 只写 ECS。不调用 `AddSpecialEffect`。

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
       失败: 距离/非地面/无空槽/DestroyPending -> 删请求
       成功:
         1. EffectHelper.Destroy(visual.effect) 若有效
         2. 清 ItemGroundVisual.effect
         3. 写 ItemAttachRequest(owner=user, item, slot)
  -> ItemAttachWorkflowSystem / ItemLifecycleOperations.Attach
       Remove ItemGroundTag, Add Inventory+Equipped, 槽位++
```

距离：`Distance2D(user.Position, item.Position) <= pickRange.radius`。无 `ItemGroundPickRange` 时用设计常量，不得无限距离。

## 8. 句柄配对路径

唯一推荐路径（复用 Effect）：

1. 创建：`EffectNativeSystem.CreateNativeEffect` → `AddSpecialEffect` → `HandleAdd`（已有）。
2. 销毁：`EffectDestroyRequest` → `DestroyEffect` → `HandleRemove` → 删特效实体（已有）。

Item 层审查清单：

- [ ] Item Native / Helper 是否直接 `AddSpecialEffect`？默认否。
- [ ] 拾取/销毁物品时是否漏 `EffectHelper.Destroy`？
- [ ] 物品实体删除（`ItemDestroyRequest`）若仍在地面，是否先拆视觉？
- [ ] 无第二条 DestroyEffect 分散点。

若地面物品走 `ItemDestroyRequest`：Destroy 工作流在删物品前必须拆特效，否则特效实体可能泄漏或成为孤儿。

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

## 10. Native 分层自检

为什么不把 `AddSpecialEffect` 放进拾取工作流或 ItemHelper？工作流推进 Tag 与槽位，Helper 是薄入口。特效句柄已由 `EffectNativeSystem` 拥有。Item Native 只在无法复用 Effect 时才允许直接调 JassApi，且必须成对 Handle。

## 11. 测试要点（实施后）

- 创建地面：有 GroundTag、Position、非空 visual.effect（或请求已被消费且 Effect 实体存在）。
- 拾取成功：无 GroundTag，有 Owner/Slot，特效实体已删。
- 超距：状态不变。
- 丢弃：从装备到地面后再次出现视觉。
- 源码无 `CreateItem` 调用（地面路径）。
- `dotnet build War3Frame/War3Frame.csproj` 通过。

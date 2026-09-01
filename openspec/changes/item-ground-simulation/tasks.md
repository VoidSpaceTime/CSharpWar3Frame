# 任务清单：item-ground-simulation

状态：待审核通过后实施。本文件仅为计划，当前不改代码。  
修订：2026-09-01（opus5 审查后）

## P0 组件与请求

- [ ] 将 `ItemCreateNativeRequest` 改为地面表现请求（`item` 引用、坐标、`model`），删除 CreateItem 语义字段（`itemTypeId`, `facing`）。验证：`Items.cs` 无 `itemTypeId` 残留，无 `CreateItem` 调用。
- [ ] 新增 `ItemGroundVisual`（`model: string`, `effect: Entity`）、`ItemGroundPickRange`（`radius: float`）、`ItemPickupRequest`（`user: Entity`, `item: Entity`）。验证：有字段用 `IComponent`，Tag 不带 Request/Event 后缀。
- [ ] 确认地面物品不挂 `EffectBase`（避免 Effect 销毁删物品）。验证：组件注释明确禁止，`ItemGroundVisual` 注释写清"独立特效实体引用"。

## P0 Helper

- [ ] `ItemHelper.CreateOnGround(ItemBase, model, x, y, z, pickRadius)`：创建物品实体，打 `ItemGroundTag`，写 `Position` / `ItemGroundVisual` / `ItemGroundPickRange`，创建 `ItemCreateNativeRequest`（独立实体或挂物品，二选一）。验证：无 `JassApi` / `DzApi` / `YDApi` 调用。
- [ ] `ItemHelper.RequestPickup(user, item)`：只创建 `ItemPickupRequest` 实体。验证：不直接 Attach，不调 Native API。
- [ ] `ItemHelper.DropToGround` 保持写 `ItemRemoveRequest`；由 Detach 补视觉。验证：丢弃路径最终发出 `ItemCreateNativeRequest`。

## P0 工作流

- [ ] `ItemPickupWorkflowSystem`：校验 `ItemGroundTag`、距离（`user.Position` vs `item.Position` ≤ `pickRange.radius`）、空槽（复用 Attach 现有 `container.currentCount < maxSlots` 或 `GetItemAtSlot` 空槽扫描）、非 `DestroyPending`；**同帧边界**：若 `visual.effect.IsNull`，跳过 `EffectHelper.Destroy`，**额外删除未消费的 ItemCreateNativeRequest**；成功则 `EffectHelper.Destroy(visual.effect)` + 清 `visual.effect` + `ItemAttachRequest`；失败删请求不改 Tag。验证：失败不改 Tag、状态；成功无句柄泄漏。
- [ ] `ItemLifecycleOperations.Detach(dropToGround: true)` 补 `ItemGroundVisual`（含 `model`）+ `ItemCreateNativeRequest`。验证：仅 Position+Tag 的旧行为被补全。
- [ ] `ItemLifecycleOperations.Attach`：若物品仍带 `ItemGroundTag` 且 `visual.effect` 有效，**先 `EffectHelper.Destroy(visual.effect)` + 清 visual**，再 Attach。验证：从地面直接 Attach 无句柄泄漏。
- [ ] `ItemHelper.EquipToUnit`：若物品带 `ItemGroundTag` 且 `visual.effect` 有效，**先销毁视觉**再创建 `ItemAttachRequest`。验证：直接装备地面物品无泄漏。
- [ ] `BeginDestroy` (ItemDestroyRequestSystem)：若物品带 `ItemGroundTag` 且 `visual.effect` 有效，**先销毁视觉**再进入 `DestroyPending`。验证：删地面物品无孤儿特效。
- [ ] `ItemCompanionDeferredDeleteSystem`：删物品前，若仍带 `ItemGroundTag` 且 `visual.effect` 有效，**先销毁视觉**。验证：延迟删除地面物品无泄漏。
- [ ] `ItemUseSystem` 不承担拾取。验证：地面无 Owner 的 Use 仍失败，不误走施法。

## P0 Native

- [ ] 实现 `ItemCreateNativeSystem`：消费 `ItemCreateNativeRequest`；调用 `EffectHelper.CreatePosition(model, x, y, z, duration: -1)`（**duration = -1 表示永久**）；写回物品的 `ItemGroundVisual.effect`；删除请求（独立实体用 `DeleteEntity(requestEntity)`，挂物品用 `item.RemoveComponent<ItemCreateNativeRequest>()`）。验证：不再 `NotImplementedException`，**禁止**直接调 `JassApi.AddSpecialEffect`。
- [ ] 不新增第二条 `AddSpecialEffect` 路径。验证：地面创建调用栈进入现有 `EffectNativeSystem.CreateNativeEffect`。
- [ ] 句柄审查（AGENTS.md 强制清单）：`ItemCreateNativeSystem` / `ItemHelper` / 工作流无直接 JassApi；创建 `HandleAdd` 与销毁 `HandleRemove` 成对且相邻；销毁点唯一（只在 `EffectNativeSystem`）。验证：对照 design.md 第 8 节审查清单全通过。

## P0 验证

- [ ] `dotnet build War3Frame/War3Frame.csproj` 0 错误。
- [ ] 静态检查：地面路径 grep 无 `CreateItem` / `JassApi.AddSpecialEffect`（Item 层）。
- [ ] 逻辑走查或测试：创建 → `ItemGroundTag` + 请求 → Native 消费 → `visual.effect` 非空 → 拾取成功 → `ItemInventoryTag`/`ItemEquippedTag` + 特效删除；超距拾取失败物品状态不变；同帧 Create+Pickup 不崩溃。
- [ ] 真实 War3 客户端看模型：**非阻塞**（proposal 已声明），记录于 summary。

## P1（不在本提案实施）

- [ ] UI 靠近提示与按键拾取。验证：独立 change，不阻塞 P0。

## 实施后必做

- [ ] 在 summary.md 中说明 `ItemCreateNativeRequest` 实际挂载方式（独立实体或组件）。
- [ ] 在 summary.md 中确认所有视觉销毁路径（Pickup / Attach / EquipToUnit / BeginDestroy / DeferredDelete）均已覆盖。

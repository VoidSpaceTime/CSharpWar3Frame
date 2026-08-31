# 任务清单：item-ground-simulation

状态：待审核通过后实施。本文件仅为计划，当前不改代码。

## P0 组件与请求

- [ ] 将 `ItemCreateNativeRequest` 改为地面表现请求（`item`、坐标、`model`），删除 CreateItem 语义字段。验证：无残留 `itemTypeId` 作为原生物品类型用途。
- [ ] 新增 `ItemGroundVisual`、`ItemGroundPickRange`、`ItemPickupRequest`。验证：有字段用 `IComponent`，Tag 不带 Request/Event 后缀。
- [ ] 确认地面物品不挂 `EffectBase`（避免 Effect 销毁删物品）。验证：设计约束写进组件注释。

## P0 Helper

- [ ] `ItemHelper.CreateOnGround`：写 `ItemBase`、GroundTag、Position、Visual、PickRange、CreateNativeRequest。验证：无 JassApi。
- [ ] `ItemHelper.RequestPickup`：只创建 `ItemPickupRequest` 实体。验证：不直接 Attach。
- [ ] `DropToGround` 保持写 `ItemRemoveRequest`；由 Detach 补视觉。验证：丢弃路径最终发出创建表现请求。

## P0 工作流

- [ ] `ItemPickupWorkflowSystem`：距离、GroundTag、空槽、DestroyPending 校验；成功 Destroy 特效 + Attach。验证：失败不改 Tag。
- [ ] `ItemLifecycleOperations.Detach(dropToGround: true)` 补 Visual + CreateNativeRequest。验证：仅 Position+Tag 的旧行为被补全。
- [ ] `ItemDestroy` 路径若物品仍在地面，先 `EffectHelper.Destroy`。验证：无孤儿特效实体。
- [ ] `ItemUseSystem` 不承担拾取。验证：地面无 Owner 的 Use 仍失败。

## P0 Native

- [ ] 实现 `ItemCreateNativeSystem`：消费请求，`EffectHelper.CreatePosition(..., -1)`，写回 `effect`，删除请求。验证：不再 `NotImplementedException`。
- [ ] 不新增第二条 `AddSpecialEffect` 路径（默认）。验证：地面创建调用栈进入现有 `EffectNativeSystem`。
- [ ] 句柄审查：创建 `HandleAdd`、销毁 `HandleRemove` 相邻成对，销毁点唯一。验证：对照 AGENTS.md 清单。

## P0 验证

- [ ] `dotnet build War3Frame/War3Frame.csproj` 0 错误。
- [ ] 静态检查：地面路径无 `CreateItem`。
- [ ] 逻辑走查或测试：创建 → 显示意图 → 拾取 → 背包 Tag；超距失败。
- [ ] 真实 War3 看模型：非阻塞，记录于 summary（若实施）。

## P1（不在本提案实施）

- [ ] UI 靠近提示与按键拾取。验证：独立 change，不阻塞 P0。

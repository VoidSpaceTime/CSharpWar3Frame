# 设计：ItemUse 到 EffectSpec 的薄适配层

## 1. 决策

ItemUse 只承担物品领域到 Effect 领域的 handoff：

```text
ItemHelper / gameplay
    -> ItemUseRequest + ItemUseTarget
    -> ItemUseEffectSystem
    -> item-origin EffectSpec snapshot
    -> root Effect
    -> 删除 request
```

ItemUse 不再是消费事务、网络命令或 Ability casting 系统。

## 2. 数据契约

```csharp
public struct ItemUseRequest : IComponent
{
    public Entity user;
    public Entity item;
}

public struct ItemUseTarget : IComponent
{
    public ItemUseTargetKind kind;
    public Entity targetUnit;
    public float targetX;
    public float targetY;
}

public struct ItemEffectOrigin : IComponent
{
    public Entity item;
    public Entity user;
}
```

删除 `requestToken` 以及全部 outcome、receipt、replay、deferred 和 retention 契约。

## 3. 请求入口

仅保留以下本地 ECS 入口：

- `ItemHelper.RequestUse(user, item)`。
- `ItemHelper.RequestUse(user, item, target)`。

helper 只创建 request entity，不读取 UI、不执行 Effect、不修改 item。`InventoryUISystem` 不消费该能力，也不注册 `inv/use` 协议。

## 4. 基础校验

系统处理每个 request 时按顺序检查：

1. user 和 item 均为存活实体。
2. item 包含 `ItemBase` 与 `ItemOwner`，且 owner 等于 user。
3. item 处于 inventory 或 equipped 状态，不处于 ground/stored。
4. `ItemBase.isUsable == true`。
5. item 仅包含有效非空 `ItemUseEffectData`；`ItemUseAbilityData` 不执行。
6. target 能规范化为合法 unit/point context。
7. EffectSpec 通过 item-origin 有界递归 preflight，并成功创建深快照。

系统不读取或校验 `stackCount`、`maxStack`、`isConsumable`，也不检查 ItemRemove claim。上述字段不属于本层语义。

任何校验失败都直接删除 request，不创建 outcome，不修改 item。

## 5. Target 规范化

- `None`: `targetUnit = user`；必须从 user 当前 `Position` 快照 point context。
- `Unit`: target unit 必须存活并具有 `Position`；快照其坐标。
- `Point`: x/y 必须为有限值且在既有安全坐标范围内；target unit 为空。

若 EffectSpec 需要单位目标而规范化结果没有 unit，request 直接结束。

## 6. EffectSpec 执行

系统在创建 root Effect 前完成：

- 检查 spec 非空和 step 图有界。
- 拒绝循环、超深、超总 step、超参数预算和重复非视觉 payload。
- 要求所有启用数值字段显式 `hasValue=true`。
- 允许 `constant`、`caster.attr.final`、`target.attr.final`、映射到 user 的 `owner.attr.final`。
- 拒绝 `stat.final`、依赖 statId 的 `linear`、未知/custom 和 source-ability formula。
- 深复制 nested arrive、GroundArea 数据、列表、字典和 formula parameters。

创建 root Effect 时：

- `EffectSource.caster = user`。
- ability 保持空值。
- 写入 `ItemEffectOrigin { item, user }`。
- 写入规范化后的 target unit 与 point context。
- child、arrive、GroundArea、reaction 继续传播 item-origin。

若 root expansion 抛错，必须删除本次创建的部分 Effect 实体；无 item 状态需要回滚。无论成功或失败，原 request 都必须删除。

## 7. 明确不存在的语义

- 没有 success/reject outcome。
- 没有 request token 或幂等。
- 没有 receipt、replay high-water 或 deferred state。
- 没有限流、排序或 per-item critical section。
- 没有 stack 扣减、ItemRemoveRequest、自动删除或 modifier cleanup。
- 没有 UI/sync ingress。
- 没有 `CastRequest`、mana cost、cooldown 或 channel。

两个独立 request 是两个独立执行意图，允许创建两个 Effect。

## 8. 代码迁移边界

### 保留并精简

- `Components/ItemUse.cs`
- `Systems/ItemUseSystem.cs`
- `Helpers/ItemHelper.cs`
- `Components/Ability/AbilityEffect.cs`
- `Helpers/AbilityEffectHelper.cs`
- `Helpers/EffectFormulaRegistry.cs`
- `Helpers/ItemEffectSpecRuntimeHelper.cs`
- `Systems/Ability/AbilityEffectSystems.cs` 中 item-origin 必需适配
- `Helpers/ItemSpecBuilder.cs` 的 authoring 互斥校验
- `Helpers/EffectAuthoring.md`

### 回退 Revision 1 改动

- `Components/Item.cs` 与 `Systems/ItemSystem.cs` 的 exact-item remove/outcome/claim 扩展
- `Core/Sync/AsyncHelper.cs` 的 token overload
- `Core/Sync/SyncHelper.cs` 的本 change identity 协议扩展
- `Kits/Inventory/InventoryUI.cs` 与 `InventoryUISystem.cs` 的 `inv/use`、token、共享 budget 和相关 drop/swap 重构
- `initialization/ECSInit.cs`、solution 文件中能够由 diff 明确归因于 Revision 1 的 hunk
- Revision 1 大型客户端状态机场景

所有回退都必须按具体 hunk 执行。不得使用整文件 checkout/reset 覆盖用户原有或其他 change 的未提交改动；无法明确归因于 Revision 1 的差异保持原样并在总结中说明。

## 9. 系统与 Friflo 边界

ItemUse system 在 query 中只收集 request entity，查询结束后再创建 Effect 或删除 request，避免在自身 query callback 内执行结构变更。Effect resolver 内既有结构变更风险不在本轮修复范围内，但必须保留为客户端验收风险。

## 10. 验证设计

`Projects/test` 使用直接 `ItemHelper.RequestUse` 的小型 War3 客户端场景，不接 Inventory UI，不增加 xUnit 或生产 Store seam。测试程序集继续暴露既有 `War3Frame.Game.BridgeMain` 入口，并通过 `War3FrameRuntime` project-reference alias 显式访问框架程序集中的 `Game.Store/Root/ECSInit`，避免两个程序集的同名 `Game` 类型互相遮蔽。

验证内容：

- self、Unit、Point root Effect handoff。
- invalid owner/state/target/spec 不创建 root Effect。
- item-origin 与深快照传播，明确覆盖 child、arrive、GroundArea 和 reaction 派生实体。
- 请求前后 stack、owner、slot、tags 和 remove state 完全不变。
- 两个项目构建和 Native 分层扫描。

## 11. 回滚

若薄适配仍不适合当前阶段，可删除 ItemUse request/system/helper 和 item-origin 扩展，使 `UseEffect` 回到仅 authoring 状态；不影响 Ability casting、Item stack 或 Inventory UI。

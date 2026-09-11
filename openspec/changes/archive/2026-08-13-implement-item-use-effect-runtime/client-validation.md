# 独立 War3 客户端验证：Revision 2

## 前置构建

```powershell
dotnet build War3Frame/War3Frame.csproj --no-restore --configuration Release --verbosity minimal
dotnet build Projects/test/test.csproj --no-restore --configuration Release --verbosity minimal
```

## 执行方式

按现有 War3 客户端流程加载 `Projects/test`。场景直接调用 `ItemHelper.RequestUse(...)` 创建 ECS request，不初始化或依赖 Inventory UI 的 `inv/use` 同步入口。

## 验收范围

客户端日志与 ECS 状态必须覆盖：

1. `None` target 创建 self-target root Effect，并携带 `ItemEffectOrigin(item, user)`。
2. `Unit` target 快照目标单位及其 Position。
3. `Point` target 使用显式有限坐标。
4. invalid user/item/owner/state、`isUsable=false`、invalid target、空/ambiguous/unsupported spec 均不创建 root Effect，且 request 被清理。
5. Area/Line 展开生成的 child Effect 继承 item-origin。
6. nested arrive Effect 使用独立深快照并继承 item-origin。
7. GroundArea 与其 reaction 派生实体使用独立深快照并继承 item-origin。
8. valid 与 invalid 请求前后，ItemBase stack、ItemOwner、ItemSlotIndex、item tags 和 remove state 均保持不变。
9. 场景不检查 token、receipt、outcome、重放、限流、消费或自动移除，因为 Revision 2 明确不提供这些语义。

最终应输出：

```text
[ItemUseEffectThinValidation] finished with 0 failure(s)
```

## 已知风险

既有 Effect systems 仍可能因 Friflo 查询回调内结构变更触发 `StructuralChangeException`。该修复已被用户取消，不得以构建成功替代真实客户端结果；若风险触发，应如实记录为外部验收失败。

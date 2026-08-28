# 总结：fix-historical-request-tags

6 个历史 `XxxRequest : ITag` 全部改为空 `IComponent`：`ItemAttrApplyRequest`、`ItemAttrRemoveRequest`、`AbilityAttrApplyRequest`、`AbilityAttrRemoveRequest`、`ProjectileArriveRequest`、`ProjectileExpireRequest`。查询从 `Filter.AnyTags` 改为 `QuerySystem<..., XxxRequest>`；写入/删除从 `AddTag`/`RemoveTag` 改为 `AddComponent`/`RemoveComponent`。弹道到达后仍打内部阶段 `ProjectileArrived`。`AGENTS.md` 删除这 6 项反例清单。

验证：仓库内 `Request : ITag` 为零；`dotnet build War3Frame/War3Frame.csproj --no-restore /p:BuildProjectReferences=false` 成功，0 错误。R1 Focused。无阻塞项。未改属性计算、装备/施法工作流或 Native 调用。

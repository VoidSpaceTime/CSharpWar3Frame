# 总结：unify-native-request-naming

Native 副作用意图统一为 `{领域}{动作}NativeRequest`，执行系统统一为 `{领域}{动作}NativeSystem`。实际重命名：`NativeUnitCreateRequest` → `UnitCreateNativeRequest`；实际物品类型为 `ItemNativeCreateRequest` → `ItemCreateNativeRequest`；`MoveNativeCommandRequest` → `MoveNativeRequest`；`MoveNativeExecutionSystem` / 文件 `UnitMoveNaitveSystem.cs` → `UnitMoveNativeSystem`。`AGENTS.md`、`ARCHITECTURE.md`、`STRUCTURE.md` 正例同步。`ItemCreateNativeSystem` 仍为未实现占位（`NotImplementedException`），注册方式未改。

验证：全仓旧标识符扫描为零命中；`dotnet build War3Frame/War3Frame.csproj --no-restore /p:BuildProjectReferences=false` 成功，0 错误。R1 Focused。无阻塞项。未改业务流程。

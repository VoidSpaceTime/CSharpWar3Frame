## 任务清单

- [x] 将 `AbilityBehaviorTrigger.OnCast` 改为 `OnEffect`，并补充 channel/中断/结束触发枚举。
- [x] 将 `AbilitySpecBuilder.OnCast(...)` 改为 `OnEffect(...)`，并补充 `CastPoint(...)`、`Backswing(...)`、`Channel(...)` 配置入口。
- [x] 更新 `AbilityBehaviorBuilder` 对应命名入口。
- [x] 扩展 `AbilitySpec` / `CastState` / `ChannelState` 以保存前摇、后摇、channel tick 信息。
- [x] 调整 `CastingSystem`：开始施法不扣资源，`OnEffect` 条件通过后扣资源并触发效果。
- [x] 调整 `ChannelingSystem`：按 tick 间隔触发 `OnChannelTick`，打断不返还资源。
- [x] 迁移 `Projects/test/Scripts/Template/Ability.cs` 到 `.OnEffect(...)`。
- [x] 检查仓库内不再使用 `.OnCast(...)` 或 `AbilityBehaviorTrigger.OnCast`。
- [x] 执行 `dotnet build War3Frame/War3Frame.csproj`。
- [x] 执行 `dotnet build Projects/test/test.csproj`。

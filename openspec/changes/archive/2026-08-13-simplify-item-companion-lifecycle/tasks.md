# Tasks: simplify-item-companion-lifecycle

## Phase 1: 移除 `ItemCompanionSource` + 修复 `IsOwnedCompanion`
- [x] 删除 `War3Frame/Src/Components/Item.cs` 中的 `ItemCompanionSource` struct
- [x] 重写 `IsOwnedCompanion`：用 `ItemActiveAbility` forward check 替代 back-pointer
- [x] 移除 `TryEnsureCompanion` 中的 `created.AddComponent(new ItemCompanionSource(item))`

## Phase 2: 原地等级同步替代重建换链
- [x] 简化 `TrySynchronizeLevel` 签名（去除 `item`、`out Entity` 参数）
- [x] 重写 `TrySynchronizeLevel`：原地 `AbilityTemplate.Apply` + 保留运行时状态
- [x] 更新 `TryEnsureCompanion` 和 `SynchronizeLevel` 的调用点

## Phase 3: 修复 helper→System 分层
- [x] `AbilityHelper.cs` 新增 `EnterCooldownOrReady(Entity ability)`
- [x] 删除 `CastingSystem.EnterCooldownOrReady` internal 方法
- [x] `CastingSystem.FinishCast` 改调 `AbilityHelper.EnterCooldownOrReady`
- [x] `ItemCompanionCastCleanup.CleanupUnit` 4 处调用改为 `AbilityHelper.EnterCooldownOrReady`
- [x] 移除 `ItemCompanionAbilityHelper.cs` 中的 `using War3Frame.Src.Systems`

## Phase 4: 构建验证
- [x] `dotnet build` 通过，无错误（`War3Frame.csproj` 与 `Projects/test/test.csproj` 均 0 错误）

## Phase 5: 补修构建暴露的遗漏

Phase 1 删除 `ItemCompanionSource` 时漏改测试场景，由后续构建暴露。

- [x] `Projects/test/Scripts/Process/ItemCompanionAbilityValidationScenario.cs`：
      back-pointer 断言改为 `companion.GetIncomingLinks<ItemActiveAbility>()` 反查
- [x] 同文件唯一性检查：由扫 `ItemCompanionSource` 改为扫 `AbilityMountInfo` 的 `ItemGranted` 计数
      （较原写法更严，可捕获重建换链遗留的孤儿实体；Phase 2 改为原地重配后已不产生）

## 备注

- 未运行 `ItemCompanionAbilityValidationScenario` 运行时场景。用户决定以编译通过作为打包门槛。
  该场景覆盖 companion 创建、施法派发、效果来源、受控删除全链路，后续如需回归可直接执行。

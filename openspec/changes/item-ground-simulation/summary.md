# item-ground-simulation 实施总结

## 状态

已实施并完成验证。用户批准依据为 `.omo/evidence/gates/user-ruling-2026-10-01.md` 第 2 项：
按 P0 范围实施，沿用现有 Attach 的调用方 `slotIndex` 规则、不新增自动首空槽算法，默认拾取半径为 160，请求挂载方式在总结中说明；同时保留“不补 `## Why` 标题”的裁决。

## 改动范围

- `War3Frame/Src/Components/Item/Items.cs`：将 `ItemCreateNativeRequest` 改为
  `item/x/y/z/model`；新增 `ItemGroundVisual`、`ItemGroundPickRange`、`ItemPickupRequest`。
- `War3Frame/Src/Helpers/ItemHelper.cs`：新增 `CreateOnGround`、带 `slotIndex` 的
  `RequestPickup`，并统一提供地面视觉清理入口；`EquipToUnit` 接入视觉清理。
- `War3Frame/Src/Systems/Native/ItemCreateNativeSystem.cs`：实现请求消费，调用
  `EffectHelper.CreatePosition(..., duration: -1)`，写回独立特效实体引用并移除请求。
- `War3Frame/Src/Systems/Item/ItemPickupWorkflowSystem.cs`：实现距离、地面 Tag、
  `DestroyPending` 和指定槽位占用校验，成功后销毁视觉并派发 Attach 请求。
- `War3Frame/Src/Systems/Item/ItemSystem.cs`：补齐 Attach、Detach、BeginDestroy 和
  `ItemCompanionDeferredDeleteSystem` 的视觉清理与重新落地请求。
- `Projects/Regression/ItemGroundRegression.cs`：新增六个 named regression 场景，并在
  `Program.cs` 注册；`RuntimeRegression` 更新 Native 系统已实现的注册断言。

## 请求挂载决策

实际采用“组件挂物品实体”路径：`ItemCreateNativeRequest` 与物品同实体挂载，
`ItemCreateNativeSystem` 消费后仅移除 `ItemCreateNativeRequest`，不会删除物品实体。
这满足同帧 Create+Pickup 的 gate：当 `ItemGroundVisual.effect.IsNull` 时跳过
`EffectHelper.Destroy`，并移除尚未消费的创建请求，避免产生孤儿特效。

## 行为与验收

- 地面物品的 ECS 真相是 `ItemGroundTag` + `Position` + `ItemGroundVisual`；视觉是独立
  Effect 实体，物品实体不挂 `EffectBase`。
- 拾取成功条件为地面 Tag、距离不超过 `ItemGroundPickRange.radius`、未处于
  `ItemDestroyPendingTag`、调用方指定 `slotIndex` 未被其他物品占用。无自动找首个空槽逻辑。
- 缺省拾取半径为 **160**。
- Attach、EquipToUnit、BeginDestroy、DeferredDelete 和 Pickup 均先拆地面视觉；Detach
  丢到地面时重新写入视觉组件和创建请求。
- 不调用 `CreateItem`，Item 层不直接调用 `AddSpecialEffect`，句柄配对只在
  `EffectNativeSystem`。
- 未实现 P1 UI 按键提示，未引入新的 `XxxEvent` 或 `TriggerEventMarker`。

## 全局影响

- `War3Frame/`：受影响，新增地面物品组件、工作流、Native 请求消费和生命周期接入。
- `War3Frame.Generator/`：不受影响；仅复用现有 `SystemRegisterAttribute`，未改生成器逻辑。
- `FrameBuild/`：不受影响；未改构建编排、资源管线或发布行为。
- `CSharpWar3Frame/`：不受影响；未改 CLI 参数、入口和日志契约。
- `Projects/`：仅新增 `Projects/Regression` 回归证据，不改变 demo/test 项目契约；新增场景
  用于本 change 的验收。

## 验证证据

- `dotnet build War3Frame/War3Frame.csproj --nologo`：0 errors，原始输出见
  `.omo/evidence/task-12-build.txt`。
- `dotnet run --project Projects/Regression -c Release --no-build`：54 executed，54 passed，
  0 failed；原始输出见 `.omo/evidence/task-12-regression.txt`。
- Named scenarios：创建、拾取成功、同帧 Create+Pickup、超距失败、无空槽失败、地面直接
  Attach，全部 PASS。
- R2 专项视角（句柄配对、Native 分层、跨系统状态协作）全部 PASS，见
  `.omo/evidence/task-12-handle-review.txt`。
- 真实 War3 客户端模型显示未执行，按批准提案声明为非阻塞；回归宿主也不替代客户端、
  多人锁步和真实 native 环境验证。

## 风险与后续

当前仍需在真实 War3 客户端验证模型路径、特效句柄生命周期和多人锁步表现；这些不属于本
P0 的阻塞验收。P1 UI 按键提示、自动走过拾取和原生物品行为均保持在本 change 范围之外。

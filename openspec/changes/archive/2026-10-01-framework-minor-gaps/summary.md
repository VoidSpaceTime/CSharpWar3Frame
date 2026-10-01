# 总结：微缺口包（framework-minor-gaps）

- 等级：`fast`；spec delta：无（`.openspec.yaml` 声明 `skip_specs: true`）。
- 批准依据：`.omo/evidence/gates/user-ruling-2026-10-01.md` 第 5 项（W4 批准门；24 微缺口，e 项依赖 12）。
- 起始 HEAD：`08776f3`（`ECS-Framework`）。
- 实施后审查强度：`R2 Targeted`（提案默认 R0，按 closeout 要求提升）。

## 逐项结论

| 项 | 裁决 | 落实/关闭结论 |
|---|---|---|
| (a) | 实施 | `Projects/demo/Scripts/Template/FootMan.cs` 由 2 行空壳填为最小步兵模板，与 test 的 `FootmanTemplate` 一致（`[UnitTemplate("footman")]`、`IUnitTemplate`、`Health 420 / Mana 0 / AttackDamage 24`）。demo/test 分属不同程序集，生成器按程序集查重，demo Release 构建未出现 `WFGEN002`。 |
| (b) | 关闭，不改代码 | `FrameBuild/CommandManager/Run.cs` 已无旧 `//AOT` 注释开关；`PublishProject` 由 `BuildMode == Release` 显式切换 `IsNative` 与 `PublishAot`，同一模式驱动 callback 与打包模式。缺口由合并解决，本 change 未改 `Run.cs`。 |
| (c) | 关闭，不移交 | 全仓无 `EffectNativeIndex`；`EffectNativeSystem` 无 `Register`/`Unregister`。`NativeEntityIndex.Register/Unregister` 仅用于单位句柄（`UnitCreateNativeSystem.cs:32` / `UnitRemoveNativeSystem.cs:59`）。无特效句柄反查消费方，登记只会是只写项；未新增、未移交、未改 `EffectNativeSystem`。 |
| (d) | 实施 | 删除 `War3Frame/War3Frame.csproj` 中排除已不存在文件 `Src\Systems\EffectSystem.cs` 的死配置 `ItemGroup`；`Test-Path` 该路径为 False，编译集不变。 |
| (e) | 实施（Todo 12 已落地） | `CostConditionRegistry` 的 `DepleteItem` 在数量充足时逐物品创建 `ItemDestroyRequest`（经 `ItemHelper.RequestDestroy`），删除直接改槽位计数 / 清归属 / `DeleteEntity` 的旧路径；复用 `ItemDestroyRequestSystem` → `ItemDestroyPendingTag` → `ItemCompanionDeferredDeleteSystem` 受控流程，解除归属并释放 companion 后再删实体，与 ground-item 语义对齐。未新增系统、未调用原生 API。 |

## 全局影响

- `War3Frame/`：仅 (d) 删除 csproj 死配置、(e) 改 `CostConditionRegistry.RequestItemDestroy` 删除入口（复用既有组件/系统）。
- `War3Frame.Generator/`：不受影响。未改生成器；(a) 只是 demo 程序集内多一个可被现有 `UnitTemplateGenerator` 发现的模板。
- `FrameBuild/`：不受影响。(b) 关闭，未改 `Run.cs`。
- `CSharpWar3Frame/`：不受影响。未触及 CLI。
- `Projects/`：(a) 改 `Projects/demo`；测试面新增 `Projects/Regression/ItemCostDestroyRegression.cs` 并调整 `Projects/test/.../CostValidationScenario.cs` 的 (e) 语义断言；未改 test 既有 `FootmanTemplate`。

## 验证

- `openspec validate framework-minor-gaps` → `Change 'framework-minor-gaps' is valid`（`skip_specs: true`，零 delta 接受）。
- 构建（Release）0 error：`War3Frame`、`Projects/demo`、`Projects/Regression`。
- 回归：`dotnet Projects/Regression/bin/Release/net10.0/War3Frame.Regression.dll` → `Executed 76; passed 76; failed 0`（基线 74 + 新增 2）。
  - 新增：`item-cost/受控销毁请求`、`item-cost/伴随技能安全删除`。
  - 调整：`existing/CostValidationScenario` Phase 3 断言受控销毁语义。
- 证据：`.omo/evidence/task-24-build.txt`、`.omo/evidence/task-24-regression.txt`、`.omo/evidence/task-24-notes.md`。

## R2 视角

1. **技术准确性**：受控路径时序自洽——消耗时只产生 `ItemDestroyRequest`，归属解除与实体删除交由 `ItemDestroyRequestSystem`（Immediate, order 1）与 `ItemCompanionDeferredDeleteSystem`（Interval, order 131）执行；伴随技能在 `IsOwnedCompanion` 校验通过后被 `AbilityHelper.RemoveAbility` 释放。证据：`ItemCostDestroyRegression` 两场景实测通过。verdict：pass。
2. **QA 覆盖**：新增具名场景覆盖「请求数 / 延迟归属 / 槽位归零 / 实体删除 / companion 释放」；既有 `CostValidationScenario` 同步更新并在同套件运行通过。证据：`task-24-regression.txt` 76/76。verdict：pass。
3. **架构分层**：`CostConditionRegistry`（Helper）只写 `ItemDestroyRequest` 意图，不直接调用 `JassApi/KKApi` 等原生 API，也未长期持有句柄；native 副作用仍集中在 Native/Execution 层。证据：改动仅新增 `ItemHelper.RequestDestroy` 调用。verdict：pass。

## 风险与剩余项

- (e) 将消耗删除从「同调用内同步删除」改为「受控流程延迟删除」；若同一帧内在请求被消费前再次 `CheckCost`，可能仍观察到待删物品。缓解：施法流程单次 `ApplyCost`，且请求系统 order 1 尽早消费。属受控路径固有语义，已被批准方向接受。
- 真实 War3 客户端未验证（消耗视觉、联机锁步、AOT 运行时）；与本 change 范围一致，本包不改发布契约。
- (a) 依赖生成器按程序集去重同名模板；已用 demo Release 构建验证，无 `WFGEN002`。
- (b)(c) 为只读代码检索结论；若未来新增特效句柄回调消费方，需重新评估。

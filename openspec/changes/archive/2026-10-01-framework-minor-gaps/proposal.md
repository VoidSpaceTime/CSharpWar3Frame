# 提案：微缺口包（framework-minor-gaps）

## 元信息

- **状态**：待审核
- **等级**：`fast`
- **变更 ID**：`framework-minor-gaps`
- **日期**：2026-10-01
- **请求来源**：计划 Todo 24（W4.8）
- **默认实施后审查强度**：`R0 Direct`
- **spec delta**：无（`.openspec.yaml` 声明 `skip_specs: true`）

## Why

合并后仍有五处被点名的微缺口：demo 单位模板是空壳、csproj 排除已不存在的文件、物品消耗删除绕过受控销毁。AOT 开关与特效 `NativeEntityIndex` 登记经核对后原缺口已不成立，本提案把它们记为关闭，避免后续重复开工。

## What Changes

逐项裁决（批准后只实施「实施」项；(e) 另受 Todo 12 门禁）：

| 项 | 位置 | 裁决 | 批准后动作 |
|---|---|---|---|
| (a) | `Projects/demo/Scripts/Template/FootMan.cs` | 实施 | 按 `Projects/test/Scripts/Template/Unit.cs` 的 `FootmanTemplate` 填成最小单位模板 |
| (b) | `FrameBuild/CommandManager/Run.cs` `PublishProject` | 关闭，不改代码 | 记录：旧 `//AOT` 注释开关已不存在，Release/非 Release 已显式切换 `PublishAot` |
| (c) | `War3Frame/Src/Systems/Native/EffectNativeSystem.cs` | 关闭，不移交、不改代码 | 记录：无特效句柄反查消费方；与 Todo 32(a) 的单位/可破坏物 adoption 不重叠 |
| (d) | `War3Frame/War3Frame.csproj` | 实施 | 删除对已不存在的 `Src\Systems\EffectSystem.cs` 的 `Compile Remove` |
| (e) | `War3Frame/Src/Helpers/CostConditionRegistry.cs` `RemoveItems` | 条件实施 | 仅在 Todo 12（`item-ground-simulation`）获批并落地后，改为写 `ItemDestroyRequest`；此前不改 |

(a) 具体新增（替换当前仅有 `namespace War3Frame.Scripts.Template;` 的空壳）：

- `using Friflo.Engine.ECS;`、`using War3Frame.Helpers;`、`using War3Frame.TemplateInit;`
- `[UnitTemplate("footman")] public partial class FootmanTemplate : IUnitTemplate`
- `Configure` 内 `UnitSpecBuilder.Create("footman").Name("步兵").Attr(Health, 420).Attr(Mana, 0).Attr(AttackDamage, 24).BuildTo(e)`
- 数值、模板名、类名与 test 的 `FootmanTemplate` 一致；demo 与 test 分属不同程序集，生成器按程序集查重，不产生 `WFGEN002`

(e) 对齐方式（条件满足后）：`DepleteItem` 在数量充足时，对每个命中的背包物品创建独立 `ItemDestroyRequest { item }` 实体，由现有 `ItemDestroyRequestSystem` → `ItemDestroyPendingTag` → `ItemCompanionDeferredDeleteSystem` 解除归属、释放 companion 再删实体。删除 `RemoveItems` 内直接 `RemoveComponent` / `RemoveTag` / `DeleteEntity` 与手改 `ItemSlotContainer.currentCount` 的路径。不在本 change 内实现地面视觉。

## Capabilities

### New Capabilities

- 无。

### Modified Capabilities

- 无。`.openspec.yaml` 使用 `skip_specs: true`。

理由：本包不改已发布 capability 的 requirement。(b)(c) 不改代码；(d) 只删无效编译排除；(a) 是 demo 示例模板，不进入 `openspec/specs/`。(e) 若实施，行为变化（消耗物品改走受控销毁、补上 companion 清理）落在 Todo 12 的 `item-ground` delta 与既有物品生命周期实现上，不在本 change 另写一份会与 `item-ground-simulation` 抢归档的 delta。不为通过校验而发明 requirement。

## Impact

- `War3Frame/`：(d) 只动 `War3Frame.csproj` 一项排除；(e) 条件满足后只改 `CostConditionRegistry.RemoveItems` 的删除入口，复用已有 `ItemDestroyRequest`，不新增系统、不调用 War3 原生 API。
- `War3Frame.Generator/`：无。不改生成器；(a) 只是 demo 程序集里多一个可被现有 `UnitTemplateGenerator` 发现的模板。
- `FrameBuild/`：无。(b) 关闭，不改 `Run.cs`。
- `CSharpWar3Frame/`：无。
- `Projects/`：(a) 只改 `Projects/demo/Scripts/Template/FootMan.cs`。不改 `Projects/test`。

不升级到 `light`/`full`：不改公共 API、生成器输出、构建/发布契约或跨模块流程；(e) 的地面语义本身留在 Todo 12 的 `full` change。

## 非目标

- 不改 AOT / `PublishAot` / `IsNative` 行为。
- 不给特效句柄做 `NativeEntityIndex.Register` / `Unregister`。
- 不实现地面物品视觉、拾取或 `ItemCreateNativeSystem`（属 `item-ground-simulation`）。
- 不改 test 工程已有 `FootmanTemplate`。
- 不在 Todo 12 未落地时提前改 `RemoveItems`。

## 风险与回滚

- (a) 低。demo 当前无其他脚本；模板名 `footman` 与 test 同名但不同程序集。回滚即还原该文件。
- (d) 低。`EffectSystem.cs` 不存在，排除项是死配置；删除后编译集不变。回滚即加回该 `ItemGroup`。
- (e) 中，且被门禁挡住。提前改会在地面视觉与 Detach 清理未落地时改变消耗删除时序（pending + 延迟删除，不再同调用内 `DeleteEntity`）。因此批准本提案也不等于立刻改 (e)。

## Verification

批准并实施后：

1. `dotnet build Projects/demo/demo.csproj` 0 error；(a) 生成注册含 `footman`。
2. `dotnet build War3Frame/War3Frame.csproj` 0 error；(d) 后工程内不再出现 `EffectSystem.cs` 排除项，且该路径仍不存在。
3. (b)(c) 无代码差异。
4. 仅当 Todo 12 已落地才改 (e)：消耗不足不删；数量充足时产生 `ItemDestroyRequest` 且不再直接 `DeleteEntity`；有 companion 的物品等到引用释放后再删。未落地则 (e) 保持现状并在 `summary.md` 记为未实施。

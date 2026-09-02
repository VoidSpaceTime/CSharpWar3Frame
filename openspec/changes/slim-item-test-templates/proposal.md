# 提案：精简冗余 Item 测试技能模板

- Change ID: `slim-item-test-templates`
- 提案等级: `fast`
- 状态: `已批准`（用户明确批准删除三个 `item_test_*` 模板）
- 目标一句话: 删除 `Projects/test/Scripts/Template/Ability.cs` 中三个无人引用的 `item_test_*` 僵尸技能模板。
- 请求来源: 技能模板精简梳理 + 功能示例缺口补齐（本次先落地删除，光环补齐因框架链路问题暂停）。

## 背景与目标

模板覆盖梳理发现：`Projects/test/Scripts/Template/Ability.cs` 中三个 `item_test_*` 模板（`item_test_unit_cast`、`item_test_area_cast`、`item_test_phased_cast`）除自身定义外零引用。其验证意图已被其他示例与内联场景覆盖：

- Unit 目标 companion 语义 → `ItemCompanionAbilityValidationScenario` 内联 `TargetType.Unit` + companion 全生命周期覆盖。
- 地面区域（GroundArea）→ `napalm_oil` / companion 场景已覆盖地面区域语义。
- 施法阶段 builder 形态 → `AbilitySpecBuilder` 的 CastPoint/Channel/Backswing 用法存在于其它模板与内联能力。

故删除不产生功能示例缺口。

## 影响范围

- 模块：`Projects/test`（示例模板）。
- 文件：`Projects/test/Scripts/Template/Ability.cs`（删除 `ItemTestUnitCastTemplate`、`ItemTestAreaCastTemplate`、`ItemTestPhasedCastTemplate` 三个类及其注册，净删约 63 行）。
- 本 change 的 `proposal.md` 与 `summary.md`。
- 不受影响区域：`War3Frame/`、`War3Frame.Generator/`、`FrameBuild/`、`CSharpWar3Frame/`、`Projects/demo` 均零改动（模板不参与运行路径，见仓库记忆 #61）。

## 方案摘要

在 `Ability.cs` 中删除上述三个模板类。无其它代码改动。

## 风险与回滚

- 风险：极低。仅示例池文件删除，运行时无引用（`UnitTemplate.Initialize()`/`AbilityTemplate.Initialize()` 在 Projects 下无调用点，模板是编译期登记示例）。
- 回滚：从 git 恢复 `Ability.cs` 的三个模板类即可。

## 验收标准

- `dotnet build Projects/test/test.csproj` 通过（0 错误）。
- `Ability.cs` 不再包含 `item_test_*` 三个模板。
- 本 change 目录存在 `proposal.md` 与 `summary.md`。

## 后续事项（不在本 change 内实施）

- **光环（Aura）能力链路当前不生效**：`AuraHelper.CreateAura` 给光环实体挂 `ModifyTarget(owner)`，而 `AuraSystem.OnUpdate` 读取 `AuraOwner` 组件（不存在即 return）。全仓无任何代码挂 `AuraOwner` → 光环永远不会生效。补 Aura 示例/场景前必须先修这条链路（独立提案），用户已确认光环事项暂缓。
- 模板缺口清单其余项（行为触发器被动、额外弹道轨迹等）待后续逐个补齐。

## 全局影响分析

- `War3Frame/`：零改动。
- `War3Frame.Generator/`：零改动。删除模板后生成器不再登记这三个名字，无其它引用。
- `FrameBuild/` / `CSharpWar3Frame/`：零改动。
- `Projects/demo`：零改动。
- `Projects/test`：本 change 唯一影响区域。

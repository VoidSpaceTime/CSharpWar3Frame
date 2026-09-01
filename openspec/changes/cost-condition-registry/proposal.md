# 提案：消耗类型注册表 CostConditionRegistry

**状态**：待审核（2026-08-31 用户确认设计后修订：Mana 组件化双读、ItemCost 纳入、ApplyCost 单项不足不扣）→ 已实施（2026-08-31，见 summary.md）
**等级**：light
**提案日期**：2026-08-31
**请求来源**：现有 `AbilityCostHelper` 硬编码蓝/血/属性三类消耗，新增怒气、充能、物品等类型必须改 helper；借鉴 xlik `_ability.lua` 的 `costAdv`（cond / deplete / value）。

---

## 背景与目标

`War3Frame/Src/Helpers/AbilityCostHelper.cs` 的 `CheckCost` / `ApplyCost` 按固定顺序检查蓝量、`HealthCost`、`AttributeCost`。新消耗类型无法注册，只能改 helper。且 `ItemCost` 组件已定义（`itemTypeId: int` + `count`）但**从未被消费**——其 int 类型 ID 与 `ItemBase.templateName`（string）模型不一致，是死代码。

目标：
1. 新增 `CostConditionRegistry`：**有序检查器列表**（每项自查"该项存不存在、够不够"），注册顺序 = 判定/扣除顺序。
2. `AbilityCostHelper.CheckCost` 短路判定（任一不足 → false）；`ApplyCost` 逐项扣除（**单项不足则跳过该项，不扣成负数**）。
3. 内置四项：Mana / Health / Attribute / **Item（补齐死代码）**。
4. Mana 消耗来源组件化：`GetManaCost` 双读（`ManaCost` 组件优先，回退 AbilityStat），封装侧小改。
5. 公开 `CheckCost` / `ApplyCost` 签名保持兼容，`CastingSystem` 两处调用零改动。

## 影响范围

- 模块：`War3Frame/Src/Helpers/`、`War3Frame/Src/Components/Ability/`、`War3Frame/Src/Helpers/AbilityHelper.Value.cs`
- 文件：
  - 新增 `CostConditionRegistry.cs`：注册表与内置四项。
  - 改造 `AbilityCostHelper.cs`：`CheckCost` / `ApplyCost` 走注册表，签名不变。
  - 修改 `AbilityCost.cs`：`ItemCost` 增加 `templateName` 字段（int 类型 ID 无法匹配模板名；无外部消费方，向后兼容）。
  - 修改 `AbilityHelper.Value.cs`：`GetManaCost` 双读（`ManaCost` 组件优先，无则回退 `GetFinalValue(ManaCost)`）。
- 不受影响区域：
  - `War3Frame.Generator/`：无生成器契约变化。
  - `FrameBuild/`、`CSharpWar3Frame/`：构建链与 CLI 不涉及。
  - `Projects/`：仅新增验证场景；模板侧可选用 `ManaCost` 组件覆盖蓝耗。
  - Native 分层：不新增 War3 原生调用（物品扣除走 ECS 简化解除，不触原生）。

## 方案摘要

```
CostConditionRegistry：有序 List<CostConditionEntry>(check, deplete)
  注册顺序 = 判定顺序（短路）与扣除顺序（全量）

CheckCost(unit, ability) → 按注册顺序调用 check，任一 satisfied=false → false
ApplyCost(unit, ability) → 按注册顺序调用 deplete；单项不足 → applied=false 跳过（不扣负）
```

内置检查器（每项自查存在性）：
| 类型 | check | deplete |
|---|---|---|
| Mana | `GetManaCost(ability)` > 0 时 current ≥ cost | current ≥ cost 才扣（不足跳过） |
| Health | 有 `HealthCost` 组件时 current ≥ value；无组件视为满足 | 有组件且足才扣 |
| Attribute | 有 `AttributeCost` 组件时 current ≥ value | 同上 |
| Item | 有 `ItemCost` 组件时背包内 `templateName` 数量 ≥ count | 数量足则移除 count 个；不足跳过（物品栏计数同步减） |

- 未知/未注册消耗类型：无（内置四项覆盖全部现有组件；自定义类型经 `Register` 追加）。
- Mana 迁移语义：`ManaCost` 组件成为蓝耗的**优先来源**，未挂组件的能力回退 Stat——现有模板零改动，新模板可组件化覆盖。

## 风险与回滚

- 风险：
  1. `GetManaCost` 双读改变蓝耗来源优先级：已配置 Stat 蓝耗且未挂组件的能力行为不变（回退）；挂了组件的按组件——需确认现有模板无 `ManaCost` 组件（当前为死代码，无消费方）。
  2. `ItemCost` 组件加字段：无外部消费方，向后兼容。
  3. `ApplyCost` 单项不足跳过 = 施法可能部分免费（用户已确认接受该语义，用于物品数量竞争场景）。
- 回滚：删除注册表，恢复 helper 三段硬编码；`ItemCost` 字段回退。成本低。

## 验收标准

1. 注册自定义消耗后，`CheckCost` / `ApplyCost` 走该类型回调。
2. 现有蓝/血/属性检查与扣除行为不变（双读回退验证）。
3. `ItemCost`：背包数量满足 → 扣除并减计数；数量不足 → CheckCost false；直接 ApplyCost → 不足不扣（物品数不变）。
4. `GetManaCost`：能力挂 `ManaCost` 组件时组件值优先；未挂时 Stat 回退（现有行为）。
5. `dotnet build War3Frame/War3Frame.csproj` + `Projects/test` 0 错误；`CostValidationScenario` 本地 PASS。

## 分级判定

- 影响范围：`War3Frame` Helper + Components + AbilityHelper.Value 单模块。
- 风险等级：低（内部查表，公开签名兼容；组件加字段向后兼容）。
- 可逆性：高。
- 是否跨项目：否。
- 是否改公共契约：`CheckCost`/`ApplyCost` 签名不变；`ItemCost` 加字段（无消费方）与 `Register` 为新增。
- 实施后审查：`R0 Direct`（light 默认；无版本敏感或跨边界协作）。

## 后续事项

- 消耗不足文案接到 UI，不在本提案。
- 不把消耗注册表做成 Source Generator 发现。
- 有 companion 的消耗物品建议走 `ItemDestroyRequest` 受控流程；本提案物品扣除为简化解除（无 companion 场景）。
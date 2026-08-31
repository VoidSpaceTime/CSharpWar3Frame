# 提案：消耗类型注册表 CostConditionRegistry

**状态**：待审核
**等级**：light
**提案日期**：2026-08-31
**请求来源**：现有 `AbilityCostHelper` 硬编码蓝/血/属性三类消耗，新增怒气、充能等类型必须改 helper；借鉴 xlik `_ability.lua` 的 `costAdv`（cond / deplete / value + reason）。

---

## 背景与目标

`War3Frame/Src/Helpers/AbilityCostHelper.cs` 的 `CheckCost` / `ApplyCost` 按固定顺序检查蓝量、`HealthCost`、`AttributeCost`。新消耗类型无法注册，只能改 helper。

目标：新增 `CostConditionRegistry`（形态对齐 `EffectFormulaRegistry`：`SortedDictionary` + `Register` / `TryResolve`），键为消耗类型。每项三个回调：`cond`（是否足够）、`deplete`（实扣）、`value`（数值）以及不足原因 `reason`。`AbilityCostHelper` 改为查表执行；内置蓝/血/属性迁入静态构造注册。公开 `CheckCost` / `ApplyCost` 签名保持兼容，扩展走新增重载或 `Register`。

## 影响范围

- 模块：`War3Frame/Src/Helpers/`
- 文件：
  - 新增 `CostConditionRegistry.cs`：注册表与内置三项。
  - 改造 `AbilityCostHelper.cs`：`CheckCost` / `ApplyCost` 走注册表，不改调用方语义。
- 不受影响区域：
  - `War3Frame.Generator/`：无生成器契约变化。
  - `FrameBuild/`、`CSharpWar3Frame/`：构建链与 CLI 不涉及。
  - `Projects/`：仍只调现有 helper 入口。
  - Native 分层：不新增 War3 原生调用。

## 方案摘要

```
CostConditionRegistry.Register(costType, cond, deplete, value, reason)
  → SortedDictionary，OrdinalIgnoreCase（或枚举键，实施时二选一并写死）

AbilityCostHelper.CheckCost(unit, ability)
  → 对技能上声明的各消耗项 TryResolve，全部 cond 通过才返回 true

AbilityCostHelper.ApplyCost(unit, ability)
  → 对同一批项调用 deplete（对应 xlik costAdv.deplete）
```

内置：Mana（`AbilityHelper.GetManaCost`）、Health（`HealthCost`）、Attribute（`AttributeCost`）。自定义类型由地图/模组在初始化时 `Register`。未知类型显式失败，不静默当 0。

## 风险与回滚

- 风险：
  1. 注册顺序或多项同时不足时的短路语义若与现实现不一致，可能改变施法失败点。
  2. 公开 API 若误改签名会波及 `CastingSystem`。本提案要求签名兼容。
- 回滚：删除注册表，恢复 helper 内三段硬编码。成本低。

## 验收标准

1. 注册自定义消耗后，`CheckCost` / `ApplyCost` 走该类型回调。
2. 现有蓝/血/属性检查与扣除行为不变。
3. `dotnet build War3Frame/War3Frame.csproj` 0 错误。

## 分级判定

- 影响范围：`War3Frame` Helper 单模块。
- 风险等级：低（内部查表，公开签名兼容）。
- 可逆性：高。
- 是否跨项目：否。
- 是否改公共契约：否（仅新增 `Register` 与可选重载）。
- 实施后审查：`R0 Direct`（light 默认；无版本敏感或跨边界协作）。

## 后续事项

- 消耗不足文案如何接到 UI，不在本提案。
- 不把消耗注册表做成 Source Generator 发现。

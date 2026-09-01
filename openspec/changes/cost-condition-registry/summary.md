# 实施总结：消耗类型注册表 CostConditionRegistry

**状态**：已批准（用户 2026-08-31 确认：Mana 组件化迁移 + 数量不足不扣）→ 已实施

## 实际改动范围

- `War3Frame/Src/Helpers/CostConditionRegistry.cs`（新增）：有序检查器列表（CostResult + CostCheckHandler/CostDepleteHandler + CostConditionEntry），注册顺序 = 判定/扣除顺序；内置 Mana/Health/Attribute/Item 四项。
- `War3Frame/Src/Helpers/AbilityCostHelper.cs`（改造）：`CheckCost` 走注册表短路判定、`ApplyCost` 逐项扣除（单项不足跳过不扣负）；公开签名不变；删除无用字段与扩展方法。
- `War3Frame/Src/Components/Ability/AbilityCost.cs`（修改）：`ItemCost` 从 `itemTypeId: int` 改为 `templateName: string`（匹配 `ItemBase.templateName`；原组件无消费方，向后兼容）。
- `War3Frame/Src/Helpers/AbilityHelper.Value.cs`（修改）：`GetManaCost` 双读——`ManaCost` 组件优先，回退 AbilityStat。
- `Projects/test/Scripts/Process/CostValidationScenario.cs`（新增）+ `Program.cs` 接入。

## 验证覆盖

- `War3Frame` + `Projects/test` build：0 错误。
- `CostValidationScenario: PASS`：
  - Phase 1 Mana 组件路径：检查/两次扣除/不足拒绝。
  - Phase 2 双读回退：无组件无 Stat 蓝耗视为无消耗项。
  - Phase 3 ItemCost：数量满足扣除并减计数、不足拒绝。
  - Phase 4 **数量不足不扣（原子）**：绕过 CheckCost 直接 ApplyCost，不足时完全不扣。
  - Phase 5 AttributeCost：三轮扣除与不足拒绝。

## 全局影响

- `CastingSystem`（CheckCost/ApplyCost 唯一调用方）：签名兼容，零改动。
- Generator / FrameBuild / CLI / Native 分层：不受影响。
- 既有模板：蓝耗走 Stat 回退路径，行为不变；`ManaCost` 组件成为可选覆盖通道。

## 风险与遗留

- 有 companion 的消耗物品建议走 `ItemDestroyRequest` 受控流程（本提案物品扣除为简化解除，不处理 companion）。
- 消耗不足文案接 UI 不在本提案范围。
- 真实 War3 客户端物品扣除未验证（本地场景无原生物品句柄），非阻塞。
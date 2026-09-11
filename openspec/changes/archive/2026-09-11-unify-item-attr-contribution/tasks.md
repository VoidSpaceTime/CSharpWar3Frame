# Tasks — unify-item-attr-contribution

## T1. O3 统一 Item 属性贡献系统
- [x] 新建 `ItemAttributeContributionApplySystem`（order 0，Query `ItemOwner + ItemAttrApplyRequest`）
  - [x] `ItemEquippedTag` false 或 owner 无效 → 仅 Remove 请求返回
  - [x] `RemoveModifiersFromSource(item)` 清旧
  - [x] 有 `ItemAttributeContributionListData` → 遍历 `attributes` 逐条 `AddModifierToUnit`
  - [x] 有 `AttributeContributionEntry` → 单条 `AddModifierToUnit`
  - [x] 双分支都完成才 Remove 请求
- [x] 删除 `ItemAttributeApplySystem`
- [x] 删除 `ItemAttributeContributionListApplySystem`
- [x] `ItemAttributeRemoveSystem` 保持不动

## T2. O2 冷却组件残留清理
- [x] `AbilityCooldownSystem` 冷却完成分支补 `RemoveComponent<AbilityCooldownState>`
- [x] 结构变更移出 Query 循环（收集后循环外删）

## T3. C2 注册表 GetName
- [x] `TriggerConditionRegistry`：加 `_names` 表 + `GetName(int)`；内置登记名字；Register 用 handler.Method.Name
- [x] `TriggerActionRegistry`：同构加 `_names` 表 + `GetName(int)`

## T4. O4 冗余别名删除
- [x] 删 `AbilityHelper.AddAbility` / `AbilityHelper.AddAbilityToSlot`
- [x] 确认 `GrantAbility`/`GrantAbilityToSlot` 为唯一入口

## T5. 验证
- [x] `dotnet build War3Frame/War3Frame.csproj` 0 error
- [x] `dotnet build Projects/test/test.csproj` 0 error
- [x] 全仓零 `ItemAttributeApplySystem`/`ItemAttributeContributionListApplySystem` 类引用
- [x] `AbilityHelper.AddAbility`/`AddAbilityToSlot` 零引用

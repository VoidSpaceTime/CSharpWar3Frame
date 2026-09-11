# Summary — unify-item-attr-contribution

## 实施结果

对照 Unreal GAS / Dota 2 / Godot 结构审查后落地的 4 项低风险收敛，全部完成并验证。

## 实际改动范围

1. **O3 — 统一 Item 属性贡献系统**
   - 删除 `ItemAttributeApplySystem`（ItemSystem.cs 旧类）与 `ItemAttributeContributionListApplySystem.cs`（整个文件）
   - 新建 `ItemAttributeContributionApplySystem`（ItemSystem.cs，order 0，Query `ItemOwner + ItemAttrApplyRequest`）
   - 统一消费 `ItemAttrApplyRequest`：装备校验 → `RemoveModifiersFromSource(item)` → 按载荷分支（`ItemAttributeContributionListData` 多贡献逐条 `value.Resolve(1)` / `AttributeContributionEntry` 单条直用）→ 都完成才 Remove 请求
   - 消除旧双系统"先到者 Remove 请求、后到者静默跳过"的竞态

2. **O2 边角 — 冷却组件残留清理**
   - `AbilityCooldownSystem`：冷却完成（remaining≤0）时收集实体，循环外 `RemoveComponent<AbilityCooldownState>`（结构变更移出查询循环）

3. **C2 — 触发器注册表可观测性**
   - `TriggerConditionRegistry` / `TriggerActionRegistry` 各加 `_names` 表 + `GetName(int)`；内置条件/动作静态构造登记方法名，自定义 `Register` 用 `handler.Method.Name`；未注册返回 `#id`
   - 纯只读新增 API，不触碰 `TryGet`/执行路径

4. **O4 — Ability 冗余别名删除**
   - 删 `AbilityHelper.AddAbility` / `AbilityHelper.AddAbilityToSlot`（纯转发、零外部调用者）
   - `GrantAbility` / `GrantAbilityToSlot` 保留为唯一入口；`AbilitySlotHelper.AddAbilityToSlot` 真方法不受影响

## 验证覆盖

- `dotnet build War3Frame/War3Frame.csproj`：0 error
- `dotnet build Projects/test/test.csproj`：0 error
- 全仓零 `ItemAttributeApplySystem` / `ItemAttributeContributionListApplySystem` 类引用
- `ItemAttributeContributionApplySystem` 单类存在且含双分支
- `AbilityHelper.AddAbility` / `AddAbilityToSlot` 别名零残留（仅剩 `GrantAbilityToSlot` 调用 `AbilitySlotHelper` 真方法的合法引用）
- 冷却完成路径已删除 `AbilityCooldownState` 组件

## 风险与遗留

- 无公共 API / 跨项目 / Source Generator 契约变更，light 级收敛
- 遗留：C2 的 `GetName` 目前无调用点（纯 API 就位）；后续 Trigger UI/日志展示可用 `GetName(id)` 显示可读条件/动作名，不属本变更范围
- git 工作区未提交（与多轮累积改动一起，待用户指示）

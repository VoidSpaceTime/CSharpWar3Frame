# 合并 Item 属性贡献双路径 + 冷却组件残留清理 + 触发器注册表可观测性 + Ability 冗余别名清理

- **状态**：已实施
- **等级**：light

## 背景

对照 Unreal GAS / Dota 2 / Godot 结构审查 War3Frame 全部系统后，发现以下 4 项低风险优化（均不改变行为语义、不触碰公共契约、局部可验证）。

## 变更目标

1. **O3：合并 Item 属性贡献双路径为统一系统**——消除"单条/多条贡献结构共享同一请求、双系统抢 Remove"的隐式竞态。
2. **O2 边角：冷却完成后移除残留 `AbilityCooldownState` 组件**——当前 remaining≤0 置 Ready 后组件不删，残留零值组件。
3. **C2：触发器条件/动作注册表加名字映射（GetName）**——int 键加可读名，改善调试可观测性，不动匹配逻辑。
4. **O4：删除 `AbilityHelper.AddAbility`/`AddAbilityToSlot` 冗余别名**——纯转发、无外部调用者。

## 影响范围

| 文件 | 改动 | 理由 |
|---|---|---|
| `War3Frame/Src/Systems/Item/ItemAttributeApplySystem.cs` | 删除 | 并入统一系统 |
| `War3Frame/Src/Systems/Item/ItemAttributeContributionListApplySystem.cs` | 删除 | 并入统一系统 |
| `War3Frame/Src/Systems/Item/ItemSystem.cs` | 新建统一 `ItemAttributeContributionApplySystem`（或放同文件）| 统一处理两条载荷 |
| `War3Frame/Src/Systems/Ability/AbilityCooldownSystem.cs` | 修改 | 完成时 RemoveComponent |
| `War3Frame/Src/Helpers/Trigger/TriggerConditionRegistry.cs` | 修改 | +`_names` 映射 + GetName |
| `War3Frame/Src/Helpers/Trigger/TriggerActionRegistry.cs` | 修改 | +`_names` 映射 + GetName |
| `War3Frame/Src/Helpers/AbilityHelper.cs` | 修改 | 删 AddAbility/AddAbilityToSlot 别名 |

`War3Frame/` 其余系统、`War3Frame.Generator/`、`FrameBuild/`、`CSharpWar3Frame/`、`Projects/`：**不受影响**——纯 War3Frame 内部重构，无跨项目依赖、无公共 API 变更、无 Source Generator 契约变化。

## 方案摘要

### 1. O3 — 统一 Item 属性贡献系统

新建一个系统替换现有两个（均 order 0）：

```
ItemAttributeContributionApplySystem (order 0)
Query: ItemOwner + ItemAttrApplyRequest
处理逻辑:
  item.Tags.Has<ItemEquippedTag>() 为 false 或 owner 无效 → 仅 Remove 请求，返回
  ModifyHelper.RemoveModifiersFromSource(item)          // 先清旧
  if item 有 ItemAttributeContributionListData → 遍历 contributions.attributes 逐条 AddModifierToUnit
  else if item 有 AttributeContributionEntry           → 单条 AddModifierToUnit
  item.RemoveComponent<ItemAttrApplyRequest>()
```

**要点**：
- 两种载荷分支处理完才 Remove 请求（消除"先到者 Remove、后到者跳过"竞态）
- 双分支都写同一 modifier 层，来源追踪不变（`ModifySource(item)`）
- 删除原 `ItemAttributeApplySystem` 与 `ItemAttributeContributionListApplySystem`
- `ItemAttributeRemoveSystem`（Query `ItemAttrRemoveRequest`）保持独立不变

### 2. O2 边角 — 冷却组件清理

`AbilityCooldownSystem` 冷却完成分支补：

```csharp
cooldown.remaining = 0;
ability.state = AbilityState.Ready;
entity.RemoveComponent<AbilityCooldownState>();  // 新增
```

注意：删除时组件在查询循环内，属结构变更——需先收集再循环外删（与现有模式一致）。

### 3. C2 — 注册表 GetName

两个注册表各加：

```csharp
private static readonly SortedDictionary<int, string> _names = new();
static ctor 内 _names.Add(id, "DamageGreater");  // 内置名
public static string GetName(int id) => _names.GetValueOrDefault(id, $"#{id}");
public static int Register(...) { ...; _names.Add(id, handler.Method.Name); ... }  // 自定义条件用方法名
```

调用方不变（纯新增可观测性 API）。Action 同构。

### 4. O4 — 冗余别名删除

删 `AbilityHelper.AddAbility` / `AbilityHelper.AddAbilityToSlot`（已核实无外部调用，全仓引用仅在 AbilityHelper 自身转发）。保留 `GrantAbility` / `GrantAbilityToSlot` 为唯一入口。

## 非目标

- **不改 Item 数据结构**（单条/多条结构本身保留，只是消费端合并）
- **不改 Trigger 匹配语义**（GetName 只加只读 API，不碰 TryGet/执行路径）
- 不改冷却语义（仅补组件清理）

## 风险与回滚

- 低风险：全部是 War3Frame 内部收敛，无公共契约变更
- O3 删除两个系统后如回归，可从 git 恢复原两系统 + 改回 order 0 即可（无 schema 迁移）
- C2 加字段不动执行路径，零行为风险

## 验收标准

1. `dotnet build War3Frame/War3Frame.csproj` 与 `Projects/test` 0 error
2. 全仓零 `ItemAttributeApplySystem` / `ItemAttributeContributionListApplySystem` 类引用
3. `ItemAttributeContributionApplySystem` 同时处理 ListData 与单条 Entry 两分支
4. `AbilityCooldownSystem` 冷却完成路径删除 `AbilityCooldownState` 组件
5. `TriggerConditionRegistry.GetName(1)` == `"DamageGreater"`；`TriggerActionRegistry.GetName(1)` == `"Damage"`
6. `AbilityHelper.AddAbility`/`AddAbilityToSlot` 零引用（已删除）

## 后续工作

- 注册表 GetName 落地后，可在 Trigger UI/日志层用 `GetName(id)` 展示可读条件/动作（不属本变更范围）。

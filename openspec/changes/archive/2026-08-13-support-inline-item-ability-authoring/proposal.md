## 0. 基本信息

- Change ID: `support-inline-item-ability-authoring`
- 提案等级: `full`
- 目标一句话: 为不需要跨模板复用的物品提供 inline `UseAbility(...)` 语法糖，同时保持 companion Ability 为唯一运行时路径。
- 请求来源: 用户希望物品独有 Ability 可以直接写在 Item template 中，而无需额外声明具名 Ability template。

## 1. 背景与目标

当前物品只能通过 `.UseAbility("ability_template")` 引用独立 Ability template。该方式适合复用和复杂能力，但简单的物品独有逻辑需要额外创建模板类和全局名称，authoring 成本偏高。

本变更增加两种 inline `UseAbility` 重载：完整 Ability 配置和即时 Effect 简写。它们在 authoring 阶段构造标准 `AbilitySpec`，注册为由当前 Item template 唯一拥有的内部 Ability template，并继续把生成名称写入既有 `useAbilityTemplateName`。ItemUse、companion、Casting、Cooldown 和 Effect 运行时不增加分支。

## 2. 方案比较

### 方案 A：恢复 Item `UseEffect`

不采用。该命名和数据模型容易重新形成 Item 直接执行 Effect 的第二条运行时路径。

### 方案 B：把 inline `AbilitySpec` 直接挂在 Item runtime 上

不采用。Companion 创建和等级同步需要同时支持 template name 与 inline spec，扩大运行时分支。

### 方案 C：authoring 阶段生成物品私有 Ability template

采用。Item runtime 仍只保存 template name，既有 companion 生命周期与 Ability workflow 无需改变。

## 3. API 与行为范围

保留现有入口：

```csharp
.UseAbility("shared_ability")
```

增加完整 inline Ability：

```csharp
.UseAbility(a => a
    .TargetType(AbilityTargetType.Unit)
    .BaseValue(AbilityHelper.Cooldown, 5f)
    .OnEffect(e => e.Heal(AbilityValue.Constant(120f))))
```

增加即时 `None` 目标简写：

```csharp
.UseAbility(e => e.Heal(AbilityValue.Constant(120f)))
```

简写默认使用 `AbilityTargetType.None`、零前摇、零引导、零后摇和零冷却。需要其他目标或施法配置时必须使用完整 inline Ability 重载。

## 4. 全局影响分析

- `War3Frame/`：受影响。扩展 `ItemSpecBuilder`、Ability spec 应用入口和内部 template 注册能力。
- `War3Frame.Generator/`：不受影响。Inline template 在 Item authoring 时命令式注册，不修改生成器发现规则和输出契约。
- `FrameBuild/`：不受影响。不改变构建、发布、JIT/AOT 或地图资源流程。
- `CSharpWar3Frame/`：不受影响。不改变 CLI、参数和配置。
- `Projects/`：受影响。测试模板增加 inline Ability 示例和行为验证。

## 5. 风险、迁移与回滚

- 重载风险：两个 lambda 重载可能出现 C# 推断歧义，实施前必须用真实调用形式做编译验证。
- 注册风险：重复 Item 实例会重复执行 authoring；必须通过专用原子注册入口实现顺序无关的幂等和冲突检查，禁止普通注册占用保留前缀或覆盖 inline template。
- 确定性风险：配置 lambda 必须立即求值为 `AbilitySpec`。Registry 以 Item template name 为逻辑 owner，并保存规范化结构指纹；同 owner 产生不同 spec 时必须失败。
- 引用风险：`AbilitySpec` 可能包含 Projectile 等步骤的 Entity 字段；inline 注册前必须递归拒绝所有非空运行时 Entity，且不得保存 authoring lambda。
- 迁移：现有 `.UseAbility(string)` 无需修改；只有适合内联的物品按需迁移。
- 回滚：删除两个重载和内部 template wrapper，内联调用方恢复为独立具名 Ability template。

## 6. 验收标准

- 三种 `UseAbility` 写法均可编译，且同一 ItemSpec 只能选择一种形式。
- 生成名称由 Item template name 确定，并使用保留前缀避免普通名称冲突。
- Inline 注册在 `AbilityTemplate.Initialize()` 前后行为一致，普通/generated template 均不能覆盖保留名称。
- 同一 Item template 的重复实例化复用同一内部 template；不同 Item template 不共享内部定义。
- 同一 Item template 重复产生不同 `AbilitySpec` 时明确失败。
- 每个 Item entity 仍创建独立 companion，冷却和状态不共享。
- `ItemSpec` 与 `ItemUseAbilityData` 仍只保存 template name；运行时不存在 inline Effect/Ability 分支。
- 当前 Item companion 场景、Release 构建和静态旧路径扫描通过。

## 7. 实施前置

本提案及其 `design.md`、`tasks.md`、`specs/inline-item-ability-authoring/spec.md` 经用户审核批准后，才能修改代码。

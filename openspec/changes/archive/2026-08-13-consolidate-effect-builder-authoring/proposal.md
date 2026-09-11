# 收敛 Effect Builder authoring 层级

## 0. 基本信息

- Change ID: `consolidate-effect-builder-authoring`
- 提案等级: `light`
- 目标一句话: 删除 `AbilityEffectSpecBuilder` 兼容包装层，让技能/物品效果链 authoring 统一使用一个主 Builder 入口。
- 请求来源: 用户反馈 `AbilityEffectSpecBuilder`、`AbilitySpecBuilder`、`EffectSpecBuilder`、`EffectValueSpec` 层数偏多，希望减少认知负担。

## 1. 分级判定

### 1.1 为什么是 light

- 影响范围: 主要影响 `War3Frame/` 内技能/物品效果链 authoring API 和 `Projects/test` 示例模板。
- 风险等级: 中；涉及公开 Builder 命名与模板写法，但不改变运行时 ECS effect 执行语义。
- 可逆性: 中；会同步修改模板调用点，但可通过恢复包装层和模板写法回滚。
- 是否跨项目: 主要在 `War3Frame/`，示例验证触及 `Projects/test`。
- 是否改公共契约: 是，但目标是 authoring 表层收敛，不改 `EffectSpec` / `EffectValueSpec` 数据契约。

### 1.2 升级触发器检查

- [x] 涉及 `War3Frame/` 与其他项目联动
- [ ] 涉及 `War3Frame.Generator/` 输出或契约
- [ ] 涉及 `FrameBuild/`、构建链路或发布流程
- [ ] 涉及 `CSharpWar3Frame/` 入口行为
- [x] 涉及 `Projects/` 示例或集成验证行为
- [x] 涉及公共 API / 数据结构 / 配置契约
- [ ] 涉及架构边界、目录结构、依赖关系重组

> 说明: 虽然会移除 `AbilityEffectSpecBuilder` 兼容包装层并迁移现有模板，但本变更仍不改运行时数据结构、不改 Native 分层、不改 Source Generator；变更集中在技能/物品 authoring 表层，按 `light` 处理。如后续进一步重命名 `EffectSpec` / `EffectValueSpec` 等核心数据契约，应升级为 `full`。

## 2. 背景与目标

当前技能 authoring 路径中存在四个容易混淆的名字:

- `AbilitySpecBuilder`: 技能整体 Builder，负责名称、目标类型、基础数值、生命周期行为。
- `AbilityEffectSpecBuilder`: 技能/物品效果链公开入口，目前主要委托给 `EffectSpecBuilder`。
- `EffectSpecBuilder`: 实际构建 `EffectSpec.steps` 的底层 Builder。
- `EffectValueSpec`: 运行时可解析的数值描述，不是 Builder，但名字与 Builder 层混在一起时容易被误认为又一层。

分析结果表明，`AbilitySpecBuilder` 与 `EffectValueSpec` 各自有明确职责，不应收掉；真正重复的是 `AbilityEffectSpecBuilder` 与 `EffectSpecBuilder` 的公开/内部双层。

目标:

- 让普通模板作者只需要理解一个效果链 Builder 入口。
- 保留 `AbilitySpecBuilder` 作为技能整体 authoring 入口。
- 保留 `EffectValueSpec` 作为运行时公式解析数据契约。
- 同步修改现有模板调用点，不保留 `AbilityEffectSpecBuilder` 兼容入口。

## 3. 影响范围

- 模块:
  - `War3Frame/Src/Helpers/AbilitySpecBuilder.cs`
  - `War3Frame/Src/Helpers/AbilityEffectSpecBuilder.cs`
  - `War3Frame/Src/Helpers/EffectSpecBuilder.cs`
  - `War3Frame/Src/Components/Ability/AbilityAuthoringSpec.cs`
  - `Projects/test/Scripts/Template/Ability.cs`
  - `Projects/test/Scripts/Template/Item.cs`
- 不受影响区域:
  - `War3Frame.Generator/`: 不改系统注册或生成器输出。
  - `FrameBuild/`: 不改构建编排。
  - `CSharpWar3Frame/`: 不改 CLI。
  - `Systems/Native/`: 不新增或迁移 War3 native 调用。
  - `EffectSpec` / `EffectValueSpec` 运行时数据结构: 不作为本轮目标修改。

## 4. 方案摘要

推荐采用“公开入口一次性收敛，底层数据不动”的轻量方案:

1. 选择一个主效果链 Builder 名称。
   - 推荐候选: `EffectChainBuilder`。
   - 备选: 继续使用 `EffectSpecBuilder` 作为公开名。
2. 让 `AbilitySpecBuilder.OnEffect`、`OnChannelTick`、`OnInterrupted`、`OnFinished`、`OnGranted`、`OnRemoved` 使用主效果链 Builder。
3. 主效果链 Builder 直接支持现有 `AbilityValue` 参数，保留模板中的 `AbilityValue.Constant(...)`、`AbilityValue.AbilityStat(...)` 写法。
4. 删除或停用 `AbilityEffectSpecBuilder` 包装层，不保留兼容转发入口。
5. 同步迁移 `AbilitySpecBuilder`、`ItemSpecBuilder` 和 `Projects/test` 模板调用点到主效果链 Builder。
6. `AbilityEffectSpec` 是否保留取决于实现阶段的最小改动判断；若仅作为行为数据包装仍有价值，可暂时保留，但不得再要求模板作者接触 `AbilityEffectSpecBuilder`。

预期 authoring 形态:

```csharp
AbilitySpecBuilder
    .Create("fire_blast")
    .OnEffect(e => e
        .Effect(EffectVisualKind.Point, "explosion.mdl", duration: AbilityValue.Constant(0.8f))
        .Area(TargetFilter.EnemyAlive, radius: AbilityValue.AbilityStat(AbilityHelper.Radius))
        .Damage(AbilityValue.AbilityStat(AbilityHelper.DamageAmount)))
    .BuildTo(ability, level);
```

解释口径:

- `AbilitySpecBuilder`: 技能整体。
- 主效果链 Builder: 效果步骤链。
- `AbilityValue`: 作者友好的数值写法。
- `EffectValueSpec`: 底层可解析数值数据。

## 5. 非目标

- 不修改 `EffectSpec` / `EffectStepSpec` / `EffectValueSpec` 的运行时数据结构。
- 不改变 Damage / Heal / Buff / Area / Line / GroundArea / Projectile / Effect 的执行顺序和语义。
- 不移除 `AbilityValue`。
- 不改变 ECS / Native 分层。
- 不修改 Source Generator、构建链路或 CLI。
- 不保留 `AbilityEffectSpecBuilder` 兼容入口。

## 6. 风险与回滚

- 风险: 移除 `AbilityEffectSpecBuilder` 后，现有模板和下游项目需要同步改写。
  - 控制: 本轮同步修改仓库内 `Projects/test` 技能/物品模板；外部下游以编译错误显式暴露迁移点。
- 风险: 如果新增 `EffectChainBuilder` 同时保留公开 `EffectSpecBuilder`，仍会产生两个效果链 Builder 名称。
  - 控制: 实现阶段只保留一个公开主 Builder 名称；另一个若存在，只作为 internal 实现细节。
- 风险: 如果主 Builder 仍只接受 `EffectValueSpec`，模板会退回底层公式写法。
  - 控制: 主 Builder 必须直接支持 `AbilityValue` 或等价 authoring sugar。
- 回滚方式: 恢复 `AbilityEffectSpecBuilder` 包装层、`AbilitySpecBuilder` 原始签名和模板原始写法。

## 7. 验收标准

- 普通技能模板可以只通过一个效果链 Builder 入口声明效果链。
- 仓库内模板不再引用 `AbilityEffectSpecBuilder`。
- `AbilitySpecBuilder` 仍负责技能整体信息和生命周期行为，不被效果链 Builder 吞并。
- `EffectValueSpec` 仍作为运行时数值解析数据，不暴露为普通模板作者必须理解的 Builder 层。
- 现有 `Projects/test` 技能/物品模板迁移后编译通过。
- `War3Frame/War3Frame.csproj` 构建通过。
- `Projects/test/test.csproj` 构建通过。
- 未新增任何 `JassApi` / `KKApi` / `YDApi` / `DzApi` 直接调用。

## 8. 审核问题

- 主效果链 Builder 是否命名为 `EffectChainBuilder`，还是继续沿用 `EffectSpecBuilder`?
- `AbilityEffectSpec` 是否本轮保留为行为数据包装，还是一并收敛为直接使用 `EffectSpec`?

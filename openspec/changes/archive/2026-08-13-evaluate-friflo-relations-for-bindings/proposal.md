## 基本信息

- Change ID: `evaluate-friflo-relations-for-bindings`
- 提案等级: `full`
- 目标一句话: 系统评估并规范 Friflo.Engine.ECS `Relations` / `Relationships` 在本仓库单对单、单对多 entity 绑定中的使用边界。
- 请求来源: 用户提供 Friflo 官方文档并要求生成项目 skill，重点检测 `Relations` 和 `Relationships` 是否可安全用于绑定关系表达。

## Why

本仓库已经部分采用 Friflo 的 entity link / relation 能力，例如属性、技能属性、物品持有、技能持有、单位持有、光环与地面区域 Buff 等。但仍存在大量经典 `IComponent + Entity` 字段和 request/tag 流程，例如效果挂载、弹道、技能/物品 attach request、slot index、一次性结算上下文等。

如果不先统一判断标准，后续会出现两类风险：

- 能用 Friflo link/relation 表达的长期绑定继续散落在普通组件里，无法利用 incoming links、自动清理和调试导航。
- 不适合迁移的瞬时上下文或 slot/order 语义被过度改成 relation，导致流程复杂、性能退化或分层违规。

因此需要先建立项目级规则和迁移候选清单，再决定后续小步迁移范围。

## What Changes

本提案阶段只做规范和评估，不直接迁移运行时代码。

计划明确：

- Friflo `IRelation<TKey>`、`ILinkComponent`、`ILinkRelation` 的项目使用边界。
- 当前已使用 link/relation 的正例清单。
- 当前仍使用普通 `Entity` 字段的候选清单。
- 识别哪些属于长期绑定，哪些属于瞬时命令/结算上下文。
- 为后续 Effect / Projectile / AbilitySlot / ItemSlot 等迁移建立评审标准。
- 记录项目级 skill：`.opencode/skills/friflo-ecs-relations/SKILL.md`。

## 非目标

- 不在本提案中直接迁移 `EffectAttachment`、`ProjectileData`、slot 系统或 attach request。
- 不改变 War3 Native / Execution 分层。
- 不改变 Source Generator 注册机制。
- 不引入新的 UI 或编辑器。
- 不把所有 `Entity` 字段机械迁移为 relation。

## 当前事实

官方文档结论：

- `IRelation<TKey>`：entity + 非 entity key，多条同类型 relation。
- `ILinkComponent`：单个 directed entity-to-entity link，适合 1:1 或 child -> owner。
- `ILinkRelation`：多个 directed entity-to-entity links，适合 1:N 或 owner -> children。
- target 可用 `GetIncomingLinks<T>()` 反查 source。
- 删除 target entity 时，Friflo 会自动移除指向 target 的 links。
- `ILinkRelation` / `IRelation<TKey>` 不存入 archetype，可降低 archetype fragmentation。
- 单 entity 上同类型 relations 不建议超过 100 条。

仓库已使用示例：

- `AttrOwner : ILinkComponent`
- `HasAttr : ILinkRelation`
- `ModifyTarget` / `ModifySource : ILinkComponent`
- `AbilityStatOwner : ILinkComponent`
- `HasAbilityStat : ILinkRelation`
- `ItemOwner : ILinkComponent`
- `AbilityOwner : ILinkComponent`
- `UnitOwner : ILinkComponent`
- `AuraBuffLink : ILinkComponent`
- `GroundAreaBuffLink : ILinkComponent`

候选但需审查：

- `EffectAttachment`
- `EffectSource`
- `EffectTargetInfo`
- `AbilityEffectContext`
- `GroundAreaSource`
- `ProjectileData.effectEntity`
- `AbilityAttachRequest` / `AbilityRemoveRequest`
- `ItemAttachRequest` / `ItemRemoveRequest`
- slot container / slot index 组合
- 各类 damage / settlement / cast / move 的瞬时 source-target 上下文

## 影响范围

- `War3Frame/`: 直接影响关系建模规范、组件设计标准、系统迁移策略。
- `War3Frame.Generator/`: 本提案不直接修改；后续若新系统或 source generator 需要关系感知，再单独提案。
- `FrameBuild/`: 预���无影响。
- `CSharpWar3Frame/`: 预期无影响。
- `Projects/`: 后续迁移示例时可能影响 demo/test 模板，本提案阶段仅记录标准。

## 风险与控制

- 风险: 误把瞬时上下文建成长期关系。
  - 控制: 只有长期语义绑定才可迁移；命令/request/payload 默认保留普通组件。
- 风险: slot/order/index 语义丢失。
  - 控制: relation 只能表达 link，slot index 必须保留在 payload 或独立组件中。
- 风险: 业务系统直接操作 native 或破坏分层。
  - 控制: relation 迁移不得改变 Native / Execution 层边界。
- 风险: relation 数量过多导致性能退化。
  - 控制: 单 entity 同类型 relation 超过 100 条的关系不得直接采用 relation。
- 风险: 修改 `ILinkComponent` target 时 index 不更新。
  - 控制: 更新 link component 必须使用 `AddComponent(new Link { target = ... })`。

## 验证计划

- 文档阶段：检查 OpenSpec 工件和 skill 是否完整。
- 后续实现阶段：每个迁移都必须构建 `War3Frame/War3Frame.csproj` 与相关 `Projects/test/test.csproj`。
- 分层阶段：确认新增或迁移系统不直接调用 War3 native。
- 行为阶段：针对 incoming links、删除 target 自动清理、slot index 保留做专项验证。

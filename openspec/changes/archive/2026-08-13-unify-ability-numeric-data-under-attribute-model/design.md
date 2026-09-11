## Context

当前仓库已经有一套较完整的通用数值模型：

- `AttrValue` / `AttrTypeId` / `HasAttr`
- `ModifyValue` / modifier pipeline

但 ability 侧仍存在独立的数值通道与混合式建模：

- 一部分能力数值通过 ability-stat path 表达
- 一部分数值直接挂在 `AbilityBase`、`ManaCost`、`DamageEffectData`、`HealEffectData`、`ProjectileData`、`AreaSearchData` 等组件字段上

这会使技能系统长期处于“双数值系统并存”的状态：

- 通用 attribute system
- ability-specific numeric system

同时，模板语义已经显示出三类模板的天然不对称：

- ability template 已经 level-aware
- unit/item template 并不天然需要 level

因此，最优结构不是接口一刀切，而是 ownership 一刀切：

1. ability numeric values → attribute/value/modifier model
2. ability behavior/structure → ordinary ECS components
3. ability templates remain level-aware
4. unit/item templates keep their non-ability signatures

## Goals / Non-Goals

**Goals:**
- 让 attribute/value/modifier 成为 ability numeric data 的唯一 ownership model。
- 明确哪些 ability fields 属于 numeric ownership，哪些属于 behavior/structure ownership。
- 保持 ability templates 的 level-aware authoring 语义。
- 保持 unit/item templates 的独立签名语义。
- 为 future buffs/talents/equipment/UI tooltip/level scaling 提供统一数值基础。

**Non-Goals:**
- 本提案不要求所有 ability component 字段都迁移到 attribute。
- 本提案不把 targeting/filter/projectile kind/damage type 等结构语义数值化。
- 本提案不强制单位/物品模板 adopt ability-like level semantics。
- 本提案不在提案阶段实施运行时代码修改。

## Decisions

### 1. Ability numeric values move to the shared attribute model
**Decision:** mana cost、cooldown、cast range、damage、heal、radius、projectile speed、duration、width、distance、charges 等 tunable numeric values MUST 迁移到共享的 attribute/value/modifier 模型。

**Rationale:** 这些值都属于“等级可变、modifier 可变、UI 可读”的数值所有权问题，应该复用仓库已有的数值系统，而不是继续维护 ability-specific numeric duplication。

### 2. Ability behavior and structure remain ordinary components
**Decision:** targeting kind、projectile kind、search/filter、damage type、damage source、model path、semantic execution flags 等 SHALL 保持为普通 ECS 组件或组件字段。

**Rationale:** 这些内容描述的是行为结构，不是数值所有权。

### 3. Ability templates remain level-aware
**Decision:** ability templates MUST 保持 `Configure(Entity entity, int level)` 语义，并负责按等级 author base numeric values 到 attribute 模型。

**Rationale:** 技能模板天然需要 level-aware 数值 authoring，而这不是单位/物品模板的通用语义。

### 4. Unit and item templates keep their own signatures
**Decision:** unit/item templates SHALL NOT 因能力模板设计而被强制统一到 `Configure(Entity entity, int level)`。

**Rationale:** 形式统一而语义不统一只会引入长期设计噪音。

### 5. Runtime numeric reads must converge to one authoritative source
**Decision:** cost、cooldown、cast-range、damage、heal、projectile-speed、radius、duration 等 runtime numeric reads MUST 在迁移后收敛到单一 attribute-backed source；如果存在 bridge，它必须是临时且有明确退出条件。

**Rationale:** 长期混用字段值与 attribute 值会制造不可验证的双真相源。

## Classification Rubric

### Numeric-owned (应迁移到 attribute)
- ManaCost
- Cooldown
- CastRange
- Damage
- Heal
- Radius
- Duration
- ProjectileSpeed
- ProjectileDistance
- Width
- ArrivalThreshold
- Charges

### Structure-owned (应保留普通组件)
- AbilityTargetType
- Projectile kind / delivery type
- TargetFilter
- DamageType
- DamageSrc
- Buff identity
- Model path / effect path / icon path
- `canHitSameTarget` 等语义执行规则

## Risks / Trade-offs

- [风险] 数值字段与结构字段边界划分不清，导致迁移时反复横跳。  
  [缓解] 用显式 classification rubric 先定边界，再逐个映射现有字段。

- [风险] runtime reader 在迁移期同时读旧字段和新 attribute。  
  [缓解] 禁止长期双源；如需桥接，必须定义短期 bridge 和退出条件。

- [风险] `AbilityStat*` 与 attribute ownership 并存过久。  
  [缓解] 在 tasks 中把 legacy `AbilityStat*` 的退场列为显式步骤。

- [风险] unit/item templates 被“接口统一冲动”错误带偏。  
  [缓解] 在 spec 中明确禁止把它们强制拉进 ability-like level semantics。

## Migration Plan

1. 审核通过本次 OpenSpec 提案。
2. 先定义 red-first 测试，覆盖 ability numeric ownership、modifier 行为、template level authoring 与结构字段非数值化约束。
3. 盘点当前 ability numeric fields，按 rubric 分类。
4. 设计或补充 ability attribute ids 与 access helpers。
5. 迁移 template authoring 到 attribute-backed base numeric values。
6. 迁移 runtime readers 到 attribute-backed reads。
7. 在验证通过后，移除 legacy `AbilityStat*` ownership。
8. 重新验证 `War3Frame`、`War3Frame.Generator`、`FrameBuild`、`CSharpWar3Frame` 与 `Projects/*`。

## Open Questions

- ability-owned numeric attributes 是否直接复用现有 attr ownership 结构，还是需要更通用的命名/辅助 API。
- 诸如 `ArrivalThreshold`、`MaxTargets` 等边界字段是否全部视为 numeric-owned，还是部分保留在行为组件中。
- cost/effect/projectile/search 系统在迁移期如何定义 bridge，而不长期保留双真相源。

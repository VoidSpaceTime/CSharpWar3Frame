## Context

当前仓库已经有一套统一的 modifier 执行底层：

- `ModifyValue`
- `ModifyTarget`
- `ModifySource`
- `ModifyHelper.AddModifierToUnit(...)`
- `ModifyHelper.RemoveModifiersFromSource(...)`

这说明仓库其实已经具备统一“属性贡献执行层”的技术基础。当前真正缺的不是底层能力，而是：

- 缺少一个明确的“来源标识层”
- 缺少一个统一的“贡献条目层”
- item / ability / aura / buff 还没有被正式约束到同一 apply/remove 语义上

## Goals / Non-Goals

**Goals:**
- 统一长期单位属性贡献的执行层。
- 保持上层来源生命周期（item/ability/aura/buff）各自独立。
- 统一 apply/remove 为 source-based modifier 流程。
- 允许来源类型可追踪、可调试。

**Non-Goals:**
- 本提案不统一所有来源系统的完整生命周期。
- 本提案不把技能自身 `AbilityStat` 参数系统并入单位属性贡献层。
- 本提案不在提案阶段实现代码修改。

## Decisions

### 1. What gets unified is the contribution layer, not the whole source lifecycle
**Decision:** item、ability、aura、buff SHALL 保持各自的上层生命周期与触发语义，但它们对单位属性的长期贡献 MUST 统一走同一执行层。

**Rationale:** 统一整个来源系统会过度抽象，而统一贡献执行层则刚好命中共性。

### 2. A source marker concept must exist
**Decision:** 来源实体 SHOULD 能够显式标识自身属于哪一类属性贡献来源（例如 Item / Ability / Aura / Buff）。

**Rationale:** 这能让调试、追踪、撤销和后续扩展更清晰。

### 3. Contribution entries must be explicit
**Decision:** 每个来源对单位属性产生的具体贡献 SHOULD 以显式条目表达，而不是由各来源系统把数值散落在流程代码里。

**Rationale:** 这样 apply/remove 才能保持统一，并且便于扩展多属性贡献。

### 4. Apply/remove must converge to source-based modifier flow
**Decision:** 所有长期单位属性贡献 MUST 最终通过 source-based modifier 路径执行与撤销。

**Rationale:** 仓库现有底层已经支持这条路径，重复发明其它 apply/remove 机制只会增加复杂度。

### 5. Ability self stats stay separate
**Decision:** `AbilityStat` / `AbilityHelper.Stat` SHALL NOT 被并入统一单位属性贡献层。

**Rationale:** 技能自身参数与“给单位加属性”不是同一种语义。

## Minimal Unified Shape

- source marker
- contribution entry
- apply request
- remove request
- unified modifier execution

## Risks / Trade-offs

- [风险] 过度统一导致 item/ability/aura/buff 生命周期语义被抹平。  
  [缓解] 只统一贡献执行层，不统一上层状态机。

- [风险] 如果没有来源标识，调试时难以区分 modifier 是谁加的。  
  [缓解] 提前明确来源标识层。

- [风险] 若把 `AbilityStat` 也并进来，会混淆技能参数与单位属性贡献。  
  [缓解] 在 spec 中显式禁止这一点。

## Migration Plan

1. 审核通过本次 OpenSpec 提案。
2. 定义来源标识与贡献条目结构。
3. 让 item 先迁移到统一贡献层。
4. 让 ability / aura / buff 逐步接入统一贡献层。
5. 保持 `AbilityStat` 独立。
6. 验证 apply/remove 和 source-based modifier 语义一致。

## Open Questions

- 来源标识是否只做调试/追踪用途，还是参与运行时逻辑分发。
- 贡献条目采用单条组件、多条关系，还是小集合结构最合适。

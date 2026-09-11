## Context

当前仓库中，物品相关实现还处于很早期：

- `ItemBase` 定义了模板、名称、堆叠与可使用性
- `ItemOwner` 表示归属关系
- `ItemSlotIndex` 表示槽位索引

与此同时，属性系统已经很成熟：

- `AttrValue` / `HasAttr`
- `ModifyValue` / `ModifyTarget` / `ModifySource`
- `AttrDirty` 重算链

这说明物品系统最自然的落点不是“直接把属性硬写到单位上”，而是：

1. 物品状态在 ECS 中显式表达
2. 物品生效通过 modifier source 映射到单位属性
3. 归属与生效严格分离

## Goals / Non-Goals

**Goals:**
- 定义地上、背包、装备、仓库四种主状态。
- 明确 `ItemOwner` 只表达归属。
- 明确属性效果只和装备态绑定。
- 让物品效果应用复用现有 modifier/source 架构。

**Non-Goals:**
- 本提案不直接实现完整 inventory UI。
- 本提案不直接实现物品原生句柄或地面视觉方案细节。
- 本提案不在提案阶段实施运行时代码修改。

## Decisions

### 1. Item ownership and item effect are separate concerns
**Decision:** `ItemOwner` MUST 只表达归属关系，SHALL NOT 直接等同于“该物品效果已生效”。

**Rationale:** 持有、存储、掉落、背包、装备是不同状态，不应该通过单一 owner 语义混为一谈。

### 2. Four primary item states must be explicit
**Decision:** 物品系统 MUST 至少显式支持以下四种主状态：

- Ground（地上）
- Inventory（背包）
- Equipped（装备）
- Stored（仓库）

**Rationale:** 这四种状态对应不同交互语义和属性生效规则。

### 3. Only equipped items apply attribute effects
**Decision:** 物品属性效果 MUST 只在装备态生效，背包和仓库状态 SHALL NOT 自动生效，地上状态更不得生效。

**Rationale:** 归属与效果生效必须分层，否则系统会快速失去可维护性。

### 4. Item attribute effects use source-based modifiers
**Decision:** 装备物品带来的属性变化 MUST 通过 source-based modifier 映射到单位属性系统，而 SHALL NOT 直接永久改写单位基础值。

**Rationale:** 现有 modifier/source 架构已经为这种需求提供了天然支撑，并且易于卸下、丢弃、替换时撤销。

### 5. Apply/remove should be request-driven, not bool-state-driven
**Decision:** 装备生效与失效 SHOULD 通过一次性请求/tag 或等价的流程触发完成，而 SHALL NOT 长期依赖 `bool applied` 这类脆弱状态位作为主机制。

**Rationale:** bool 容易在换槽、丢弃、堆叠变化、销毁等场景中漂移。

## State Model

### Ground
- 可交互世界实体
- 无 owner
- 不生效属性

### Inventory
- 属于某单位
- 有槽位
- 默认不生效属性

### Equipped
- 属于某单位
- 有槽位
- 生效属性

### Stored
- 属于某单位/仓库实体
- 默认不生效属性

## Risks / Trade-offs

- [风险] 如果未来要支持“背包中即生效”的特殊物品，状态规则会扩展。  
  [缓解] 先以装备态生效为默认规则，必要时引入明确的例外状态或标签。

- [风险] 若同时存在 stack/merge/拆分逻辑，装备态与背包态切换会复杂化。  
  [缓解] 先锁定 ownership/effect 边界，再叠加堆叠规则。

- [风险] 若不通过 source-based modifiers，而直接写单位属性，会导致撤销复杂。  
  [缓解] 明确禁止直接把 item bonus 永久写死到单位属性基础值中。

## Migration Plan

1. 审核通过本次 OpenSpec 提案。
2. 引入显式 item state tags/components。
3. 定义拾取、装备、卸下、丢弃、存入仓库的状态迁移。
4. 引入 item attribute apply/remove request。
5. 通过 modifier source 将装备效果映射到单位属性。
6. 验证状态迁移与属性撤销行为。

## Open Questions

- 仓库是否需要区分“背包中生效”和“装备栏生效”两种物品类别。
- 仓库中仓库是否建模为玩家所有，还是独立存储实体。
- 地上物品最终采用原生 item 还是 ECS/特效地物，这不影响本提案中的状态分层原则。

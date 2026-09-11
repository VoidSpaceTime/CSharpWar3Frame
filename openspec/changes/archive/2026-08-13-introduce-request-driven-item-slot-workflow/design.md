## Context

当前物品系统已经建立了正确的状态语义分层：

- `ItemGroundTag`
- `ItemInventoryTag`
- `ItemEquippedTag`
- `ItemStoredTag`

同时，装备态属性生效也已经通过 request + modifier source 的方式接入：

- `ItemAttrApplyRequest`
- `ItemAttrRemoveRequest`

但容器位置层仍未显式收敛：

- `ItemSlotIndex` 已存在
- `ItemOwner` 已存在
- helper 仍然承担较强的状态切换职责

如果不把 slot 层与状态层明确分离，后续背包、装备栏、仓库会逐渐混成同一概念。

## Goals / Non-Goals

**Goals:**
- 引入 item slot container/workflow。
- 明确 slot = 位置，tag = 状态。
- 让 item attach/remove/swap/resize 成为 request-driven immediate actions。
- 保持装备效果仍然只和 `ItemEquippedTag` 绑定。

**Non-Goals:**
- 本提案不直接实现背包 UI。
- 本提案不在提案阶段实现代码修改。
- 本提案不让 slot 本身隐式决定属性是否生效。

## Decisions

### 1. Slot and state are separate concepts
**Decision:** `ItemSlot` SHALL 表达容器位置，状态标签 SHALL 表达业务语义。

**Rationale:** “放在哪”与“当前处于什么物品状态”不是一回事。

### 2. Item slot workflow is request-driven and immediate
**Decision:** attach/remove/swap/resize MUST 被视为离散动作，由 request-driven immediate workflow 处理。

**Rationale:** 这些不是周期扫描逻辑，而是明确的容器变更动作。

### 3. Equipped effect remains bound to equipped state, not slot presence
**Decision:** item attribute effect MUST continue to bind to `ItemEquippedTag` rather than slot occupancy itself.

**Rationale:** 背包槽和仓库槽都可能持有 item，但不应自动生效。

### 4. Container ownership and effect application remain separate
**Decision:** `ItemOwner + ItemSlotIndex` 表示容器归属与位置；`ItemAttrApply/RemoveRequest` 表示效果生效与撤销。

**Rationale:** 容器位置变化与属性生效变化不应强绑定成同一步骤。

## Minimal Request Set

- `ItemAttachRequest`
- `ItemRemoveRequest`
- `ItemSwapRequest`
- `ItemSlotResizeRequest`

## Risks / Trade-offs

- [风险] 如果把 slot 直接等同于 equipped，仓库/背包会错误生效。  
  [缓解] 在 spec 中明确 slot 与 state 分离。

- [风险] helper 如果继续直接做全部流程，会再次变成 workflow owner。  
  [缓解] 提案明确 helper 应逐步降级为 request 入口。

- [风险] remove 与 drop/unequip 语义混在一起。  
  [缓解] 在实现阶段分别定义 detach/drop 路径。

## Migration Plan

1. 审核通过本次 OpenSpec 提案。
2. 引入 `ItemSlotContainer` 与 canonical request 结构。
3. 建立 immediate item slot workflow system。
4. 将 helper 逐步收敛为 request 入口。
5. 验证 item attach/remove/swap/resize 与 equipped effect 触发时机。

## Open Questions

- `ItemOwner` 是否未来应进一步泛化为容器 owner，而不只指向 unit。
- 仓库是否建模为特殊 owner，还是独立 storage entity。

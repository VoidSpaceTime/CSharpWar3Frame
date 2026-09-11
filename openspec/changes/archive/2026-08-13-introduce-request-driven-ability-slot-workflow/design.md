## Context

当前技能槽能力已经有较完整的 helper 接口，但系统层非常轻：

- `AbilitySlotHelper` 几乎承担了全部 attach/remove/swap/resize 语义
- `AbilitySlotSystem` 仍然接近空壳

这对早期开发足够，但随着 ability contribution、slot rules、后续 UI/输入层接入，helper 继续直接拥有 workflow 会导致：

- 状态变更分散
- 校验与更新逻辑难以统一
- 后续 request/outcome/events 难以引入

最优结构是：

1. helper 退化为 request 入口
2. immediate workflow system 成为 canonical slot owner
3. attach/remove/swap/resize 作为离散动作统一走请求驱动

## Goals / Non-Goals

**Goals:**
- 让 attach/remove/swap/resize 成为 request-driven immediate actions。
- 让 workflow system 成为槽位业务逻辑 owner。
- 让 helper 只负责 ergonomics，不再直接拥有 slot state mutation。
- 让 ability contribution apply/remove 继续由 workflow 在合适时机触发。

**Non-Goals:**
- 本提案不直接实现 UI。
- 本提案不在提案阶段引入复杂事件总线。
- 本提案不要求所有高层调用方立即迁移到新 workflow。

## Decisions

### 1. Slot operations are discrete workflow actions, not periodic logic
**Decision:** attach/remove/swap/resize MUST 被视为离散 workflow actions，并由即时 request-driven 系统处理。

**Rationale:** 这些动作不是持续过程，不应通过周期系统轮询处理。

### 2. Helper should only enqueue requests
**Decision:** `AbilitySlotHelper` SHOULD 收敛为 request 入口，而 SHALL NOT 继续直接拥有完整槽位业务流程。

**Rationale:** helper 应提供入口便利，不应成为 workflow owner。

### 3. One canonical workflow layer owns slot mutations
**Decision:** 一个 canonical immediate workflow layer MUST 负责：

- 合法性校验
- owner/index/container 更新
- 贡献 apply/remove 请求触发

**Rationale:** 当前这些逻辑散在 helper 中，后续会难以统一维护。

### 4. Remove and destroy must be separable concepts
**Decision:** ability 从槽位移除与 ability 实体销毁 SHOULD 被视为可分离概念，而不是默认永远绑定在一起。

**Rationale:** 后续可能存在解绑但保留实体的场景。

### 5. Swap should not re-trigger contribution semantics unnecessarily
**Decision:** slot swap SHOULD 只交换槽位归属索引，而 SHALL NOT 被实现为 remove+attach 的组合流程，除非未来规则明确要求。

**Rationale:** 当前 contribution 语义应保持稳定，不应因 swap 产生重复 apply/remove。

## Minimal Request Set

- `AbilityAttachRequest`
- `AbilityRemoveRequest`
- `AbilitySwapRequest`
- `AbilitySlotResizeRequest`

## Risks / Trade-offs

- [风险] 如果 helper 与 workflow 并存过久，调用方会混用两套路径。  
  [缓解] 在实现阶段给 helper 明确降级定位，逐步转发到 request-driven workflow。

- [风险] remove/destroy 不分会限制后续玩法。  
  [缓解] 在提案中先明确它们应可分离。

- [风险] swap 如果走 remove+attach，可能造成 contribution 重复抖动。  
  [缓解] 在 spec 中明确 swap 的优先语义是仅改 index。

## Migration Plan

1. 审核通过本次 OpenSpec 提案。
2. 定义 canonical request 结构。
3. 引入 immediate workflow system。
4. 将 `AbilitySlotHelper` 逐步改成请求入口。
5. 验证 attach/remove/swap/resize 基本流程与 contribution 触发时机。

## Open Questions

- 当前阶段是否允许 remove 默认 destroy，detach-only 作为可选路径。
- 后续是否需要显式 slot workflow outcome/event。

## Context

当前能力系统已经显露出清晰的分层方向，但 `AbilityBase` 仍然卡在“旧中心化结构”里：

- 模板 authoring 需要一个统一入口
- casting/targeting/UI 需要少量公共语义字段
- 与此同时，数值属性正在向 attribute 模型收敛
- cooldown / charges / channel 等过程值更适合作为 runtime state
- projectile / damage / heal / buff / area search 等结构语义已经天然更适合作为独立组件

如果继续让 `AbilityBase` 混合持有这些字段，仓库会同时维护：

- ability identity truth
- ability numeric truth
- ability runtime-state truth
- ability behavior-structure truth

这会让任何后续 ability 重构都回到“先改 `AbilityBase` 再到处修引用”的高耦合模式。

## Goals / Non-Goals

**Goals:**
- 将 `AbilityBase` 收敛为技能身份层 / 最小公共语义层。
- 明确 `AbilityBase` 的白名单字段与黑名单字段。
- 让 ability numeric values 收敛到 `AbilityAttribute`。
- 让 cooldown/channel/charges 等过程状态收敛到 runtime state components。
- 让行为结构字段继续保留在独立 ECS 组件中。

**Non-Goals:**
- 本提案不直接重构所有 ability runtime systems。
- 本提案不在提案阶段实施代码迁移。
- 本提案不要求彻底删除 `AbilityBase`。
- 本提案不把 `AbilityBase` 演化成新的 mega-config bucket。

## Decisions

### 1. `AbilityBase` remains, but only as identity/minimal semantics
**Decision:** `AbilityBase` MUST 保留，但其职责 MUST 被限制为“技能身份层 / 最小公共语义层”。

**Rationale:** 完全删除 `AbilityBase` 会让模板 authoring 与 casting/targeting 失去统一入口；继续扩张 `AbilityBase` 则会维持高耦合大杂烩。

### 2. Whitelist fields stay in `AbilityBase`
**Decision:** `AbilityBase` 的白名单字段初始仅包括：

- `level`
- `targetType`

`castType` 或 `abilityFlags` MAY 在未来被证明属于公共语义时纳入，但必须通过显式评估。

**Rationale:** 这些字段属于 ability identity / minimal semantic ownership，而不是可修正数值或运行时状态。

### 3. Numeric fields leave `AbilityBase`
**Decision:** 下列字段 SHALL NOT 保留在 `AbilityBase`：

- mana cost
- cooldown length
- cast range
- cast time
- channel duration
- radius
- duration
- projectile speed
- projectile distance
- width
- arrival threshold
- damage amount
- heal amount
- max targets
- charges max

这些 MUST 迁移到 `AbilityAttribute` ownership。

**Rationale:** 这些字段属于 tunable numeric values，应与 modifier pipeline、template level authoring、UI 读取、buff/talent/equipment 修正统一收敛。

### 4. Runtime process values leave `AbilityBase`
**Decision:** 下列字段 SHALL NOT 保留在 `AbilityBase`：

- current cooldown
- current charges
- channel progress
- ammo current
- recover remaining

这些 MUST 由 runtime state components 拥有。

**Rationale:** 这些值不是能力身份，而是能力运行时过程状态。

### 5. Behavior structure fields leave `AbilityBase`
**Decision:** projectile、linear projectile、area search、damage type、damage source、target filter、buff apply、model path、effect path、hit semantics 等结构字段 SHALL NOT 保留在 `AbilityBase`。

**Rationale:** 这些字段描述的是技能执行结构，而不是技能公共身份。

## Whitelist / Blacklist

### Whitelist
- `level`
- `targetType`
- optional future candidates only after explicit review: `castType`, `abilityFlags`

### Blacklist
- all tunable numeric fields
- all runtime process fields
- all projectile / area / damage / heal / buff / filter / visual structure fields

## Risks / Trade-offs

- [风险] 迁移期 `AbilityBase` 与 `AbilityAttribute` / runtime state 双读并存。  
  [缓解] 明确 canonical owner，迁移时逐类 reader 收口，不允许长期双真相源。

- [风险] 团队为了短期方便继续向 `AbilityBase` 塞字段。  
  [缓解] 在 spec 中定义白名单与黑名单，并把越界字段视为架构违规。

- [风险] 一次性重构 ability 全链路改动面过大。  
  [缓解] 按 vertical slice 分阶段迁移：先 base slimming，再 numeric readers，再 runtime states，再结构 reader。

## Migration Plan

1. 审核通过本次 OpenSpec 提案。
2. 先定义 red-first tests，覆盖 `AbilityBase` 白名单字段、numeric ownership、cooldown state、template authoring 与 UI/casting 读取路径。
3. 盘点当前 `AbilityBase` 字段和读取者。
4. 建立 `AbilityAttribute` + runtime state 的 canonical ownership。
5. 迁移 casting / UI / slot / template readers。
6. 在验证通过后，从 `AbilityBase` 中移除黑名单字段。

## Open Questions

- `castType` 是否确实属于公共语义，值得留在 `AbilityBase`；还是应该继续由更细粒度结构组件表达。
- `abilityFlags` 是否会成为新的大杂烩入口，需要谨慎限制。
- 第一批 vertical-slice migration 该优先清理 `Cooldown` 还是 `CastRange` / `ManaCost` 读取路径。

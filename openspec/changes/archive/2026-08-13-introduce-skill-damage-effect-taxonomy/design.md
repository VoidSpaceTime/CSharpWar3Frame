## Context

当前仓库已有统一的能力效果执行框架：

- `AbilityEffectHelper.CreateEffectEntity(...)`
- `EffectPending`
- `DamageEffectSystem`
- `HealEffectSystem`
- `BuffEffectSystem`
- `ProjectileSystem`
- `AreaSearchSystem`

但伤害语义目前仍然偏向“命中一次 -> 造成一次伤害”的单一模式，难以自然表达更复杂的伤害需求。

用户明确提出了三类典型需求：

1. 移动中的冲击波，每 0.1s 对范围内单位造成伤害
2. 移动中的冲击波，但每个单位只受一次伤害
3. 固定区域内每秒造成伤害，持续 3s

这些需求说明：

- 轨迹是否存在
- 是否周期 tick
- 每目标是否去重
- 是否区域持续

都应成为明确的 effect 语义，而不该挤进单个 damage 组件里。

## Goals / Non-Goals

**Goals:**
- 定义单次伤害与周期伤害的分层。
- 定义命中策略（单次/重复/限频）作为独立语义。
- 定义区域搜索与轨迹系统继续保持独立。
- 让伤害结算统一由 damage pipeline 负责。

**Non-Goals:**
- 本提案不在提案阶段实现完整伤害公式框架。
- 本提案不把所有 effect payload 混成一个超级组件。
- 本提案不把 item 身份层与 ability 身份层合并。

## Decisions

### 1. Instant damage and periodic damage are distinct effect semantics
**Decision:** 单次伤害与周期伤害 MUST 被视为不同 effect semantics，而 SHALL NOT 继续共用一个过于扁平的 damage payload 模型。

**Rationale:** 它们的触发时机、生命周期和状态需求不同。

### 2. Hit policy must be explicit
**Decision:** “每目标只命中一次”“允许重复命中”“最小重复命中间隔”等命中策略 SHOULD 成为显式 effect 语义。

**Rationale:** 这类差异正是冲击波/线性技能/周期伤害之间的重要区别。

### 3. Area and trajectory remain separate concerns
**Decision:** 区域搜索与轨迹系统 SHALL 保持独立，不应被重新混入伤害组件本体中。

**Rationale:** 搜索/移动语义与伤害结算语义不同。

### 4. Effect systems decide when damage is emitted; damage system decides final settlement
**Decision:** effect systems MUST 负责“什么时候产生一次伤害请求”；damage system MUST 负责“最终伤害如何计算与结算”。

**Rationale:** 这样才能兼容目标身上的增伤/减伤、免疫、护盾等复杂需求。

## Minimal Taxonomy

- `DamageEffectData`：单次伤害语义
- `PeriodicDamageData`：周期伤害语义
- `HitPolicyData`：命中去重/限频语义
- `AreaSearchData`：区域搜索语义
- `ProjectileData` / `LinearProjectileData`：轨迹语义

## Risks / Trade-offs

- [风险] 如果继续把所有伤害形式塞进一个组件，后续复杂技能会导致大量条件分支。  
  [缓解] 先在语义上拆分类别，再逐步实现。

- [风险] 如果 effect 系统直接做最终伤害结算，会和未来 buff/debuff 结算冲突。  
  [缓解] 保持“effect 负责触发，damage system 负责结算”的边界。

- [风险] 一步把伤害公式也完全抽象化，会超出当前实现范围。  
  [缓解] 本提案只先分类 effect semantics，不在本轮锁死完整公式框架。

## Migration Plan

1. 审核通过本次 OpenSpec 提案。
2. 定义 `PeriodicDamageData` 与 `HitPolicyData` 等新语义层。
3. 让现有单次伤害路径保持兼容。
4. 增量实现“移动中的周期伤害”“单次命中去重”“区域持续伤害”三类典型场景。
5. 后续再单独提案整理 damage formula/settlement framework。

## Open Questions

- `DamageEffectData` 是否应继续承载基础值来源标识，还是完全交给后续公式层。
- `HitPolicyData` 是按 effect entity 记录命中历史，还是交给专用 hit-tracking 结构。

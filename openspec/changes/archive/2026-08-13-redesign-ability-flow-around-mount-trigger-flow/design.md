## Context

当前仓库实际上已经存在多个“局部正确但彼此分散”的方向：

- ability slot workflow（槽位挂载）
- item/equipment contribution layer（长期属性贡献）
- effect entity pipeline（伤害/治疗/Buff/弹道/范围）
- move continuation（到达后继续执行）
- damage formula registry（基础伤害求值入口）

这些方向本身并不矛盾，但缺少一个更高层的统一抽象。结果就是：

- 主动技能、被动技能、天赋技能、物品技能、复合技能还没有同一个“母模型”
- 技能是否挂载、何时触发、如何执行、如何结算，仍然分散在多处系统中

因此，需要把能力系统提升为四层主模型：

1. **Mount**：这个能力如何属于某个单位/物品/来源
2. **Trigger**：这个能力如何启动（主动、受伤、死亡、命中、装备、Tick 等）
3. **Flow**：这个能力执行时经历哪些阶段/节点
4. **Settlement**：节点效果最终如何落地（伤害、治疗、Buff、属性贡献、移动、生命周期变化）

## Goals / Non-Goals

**Goals:**
- 为主动、被动、天赋、物品、复合技能建立同一高层主模型。
- 把“被动”从能力类型问题转换为触发方式问题。
- 把“复合技能”从堆组件问题转换为执行流问题。
- 保持效果 payload、属性贡献、伤害结算、轨迹、区域搜索作为 flow 节点能力，而不是技能本体。

**Non-Goals:**
- 本提案不在提案阶段推翻所有现有组件。
- 本提案不要求立即删除现有 `AbilityBase`、payload 组件或 effect 系统。
- 本提案不立即定义完整节点 DSL 或编辑器。

## Decisions

### 1. Ability identity and ability execution must be separated
**Decision:** 能力的“被谁持有/如何挂载”与“如何执行” MUST 分离建模。

**Rationale:** 技能槽、天赋、被动、物品来源都是 mount 问题，不应和施法/触发/执行混为一谈。

### 2. Passive is a trigger mode, not a separate architectural class
**Decision:** “被动技能” SHOULD 被视为 trigger 模式，而不是与主动技能完全分裂的另一套主架构。

**Rationale:** 这样才能自然支持 `OnDamaged`、`OnDeath`、`OnAttack` 等触发能力。

### 3. Effect payloads are flow nodes, not the whole ability model
**Decision:** `DamageEffectData`、`HealEffectData`、`ApplyBuffData`、`AreaSearchData`、`ProjectileData` 等 SHALL 被视为 flow node semantics，而不是能力本体定义的唯一表达方式。

**Rationale:** 复合技能需要多个阶段和多个节点，而不是一个 payload 包打天下。

### 4. Composite skills should be modeled as multi-stage flows
**Decision:** 诸如“熔岩球飞行 → 命中 → 落地生成持续地面”的复杂技能 MUST 以多阶段 flow 表达，而不应通过单体组件堆叠硬编码。

**Rationale:** 这是复杂技能天然的结构，不应被扁平成单个 payload。

### 5. Settlement remains centralized
**Decision:** 伤害、治疗、Buff、长期属性贡献等最终落地逻辑 SHOULD 继续集中在各自的 settlement layer 中，而不是散落到 flow trigger 层。

**Rationale:** 触发与结算的分层是当前仓库已经逐步形成的正确方向。

## Target Architecture

### Mount layer
- Slot-mounted abilities
- Non-slot-mounted abilities (talent/passive/system-granted)
- Item-use abilities

### Trigger layer
- Active cast
- OnDamaged
- OnDeath
- OnHit / OnAttack
- OnEquip / OnUnequip
- Periodic tick

### Flow layer
- Select target
- Launch projectile
- Area search
- Spawn child effect
- Periodic node
- Apply payload node

### Settlement layer
- Damage
- Heal
- Buff apply/rollback
- Attribute contribution apply/remove
- Move/lifecycle side effects where appropriate

## Risks / Trade-offs

- [风险] 如果继续沿现有“payload 组件包”模式叠加需求，复杂技能会越来越不可维护。  
  [缓解] 先用高层模型重定义技能流，再按阶段迁移现有实现。

- [风险] 如果一次性重写全部能力系统，改动面会过大。  
  [缓解] 提案阶段先锁高层边界，实施时按 Mount / Trigger / Flow / Settlement 四层分步迁移。

- [风险] 过度抽象成过重 DSL。  
  [缓解] 当前只定义概念层，不强行要求完整 DSL/编辑器。

## Migration Plan

1. 审核通过本次 OpenSpec 提案。
2. 先把现有能力系统映射到四层模型中。
3. 先处理 Mount 层统一（slot 与非 slot）。
4. 再处理 Trigger 层统一（主动 / 事件驱动 / 周期）。
5. 再逐步把复杂技能迁移为多阶段 flow。
6. 保持 settlement layer 的独立性。

## Open Questions

- Flow 节点最终采用组件化节点、显式 graph、还是受限的阶段链。
- 非槽位挂载能力是否需要显式 mount type 分类。
- item use 是否直接接入同一 flow 层，还是先做轻量桥接。

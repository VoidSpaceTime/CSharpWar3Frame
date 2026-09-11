## Context

当前仓库的 `AttributeHelper` 已经注册了 `HealthRegenPercent` 与 `ManaRegenPercent`，但 `HealthSystem` 和 `ManaSystem` 的回复公式仍只读取固定回复值。现有生命/魔法系统都属于 interval 计算层，数值变化后通过 `UnitNativeDirtyFlags` 标记脏位，再由原生同步系统写回 War3 native。

## Goals / Non-Goals

**Goals:**
- 让百分比生命回复与百分比魔法回复属性真正参与回复计算。
- 保持现有系统职责不变：回复系统只负责计算，native system 只负责同步。
- 让回复公式足够简单、可预测、便于后续扩展。

**Non-Goals:**
- 不修改属性注册机制。
- 不重构属性实体结构。
- 不改变 native 同步、死亡处理或单位创建链路。

## Decisions

### 1. 百分比回复以当前属性的 `finalValue` 为基数
**Decision:** `HealthRegenPercent` 与 `ManaRegenPercent` 均按对应属性当前 `finalValue` 计算每秒回复量，再与固定回复值相加。

**Rationale:** `finalValue` 表示当前所有修饰器结算后的最大生命/最大魔法，是最自然的百分比回复基准。

**Alternatives considered:**
- 以 `baseValue` 为基数：会忽略最大生命/魔法加成，和直觉不一致。
- 以 `current` 为基数：会导致回复量动态波动，不符合常规 RPG / War3 风格回复语义。

### 2. 回复公式保持在线性叠加层完成
**Decision:** 回复系统内直接采用 `flatRegen + finalValue * percentRegen` 的线性叠加，再乘 `Tick.deltaTime`。

**Rationale:** 这与当前固定回复实现最接近，改动最小，也足够清晰。

**Alternatives considered:**
- 将百分比回复改为 modifier 管线特化处理：复杂度高，超出本次小改动范围。

## Risks / Trade-offs

- [风险] 百分比属性的数值单位不清晰（例如 0.05 还是 5） → [缓解] 在实现中默认按小数比例处理，并保持与现有属性数值风格一致。
- [风险] 最大值为 0 时出现无意义回复 → [缓解] 公式天然返回 0，不额外引入特殊分支。
- [风险] 固定回复与百分比回复叠加后刷新过快 → [缓解] 保持现有 tick 节奏不变，后续如需平衡再调属性值而不是改结构。

## Migration Plan

1. 更新 `HealthSystem` 回复公式，加入 `HealthRegenPercent`。
2. 更新 `ManaSystem` 回复公式，加入 `ManaRegenPercent`。
3. 验证数值变化仍会正确标记 native dirty。

## Open Questions

- 当前仓库是否约定百分比属性使用 0~1 小数还是 0~100 百分数；本次提案默认使用 0~1 小数比例。

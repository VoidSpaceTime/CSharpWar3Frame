## Context

当前 unit native compare-sync 已经证明这种机制是有效的：

- ECS 属性为真相
- snapshot 记录上次同步值
- 只有变化时才调用 `SetUnitState(...)`

但 snapshot 与系统逻辑仍是“手写字段 + 手写 if 分支”结构：

- `Health` 一套分支
- `Mana` 一套分支

这与当前仓库其它成熟设计不一致：

- `AttributeHelper.Register(...)`
- `AbilityHelper.Register(...)`
- `TargetFilterRegistry`

当前仓库更自然的扩展方向是：

1. 用 registry/list 表达同步规则
2. 用 per-unit snapshot 保存运行时 baseline

## Goals / Non-Goals

**Goals:**
- 消除继续手写 snapshot 字段的扩展方式。
- 消除继续手写 native sync if-branch 的扩展方式。
- 保持 compare-sync 语义不变。
- 保持 registry 为静态声明层，不承载动态状态。

**Non-Goals:**
- 本提案不引入重型 descriptor 框架。
- 本提案不把运行时 baseline 放进全局 registry/list。
- 本提案不在提案阶段改代码。

## Decisions

### 1. Registry/list owns static sync declarations only
**Decision:** registry/list MUST 只保存静态同步声明，例如 `attrTypeId -> native state`，SHALL NOT 保存运行时 baseline。

**Rationale:** 全局列表适合做静态规则，不适合承载 per-unit 运行时状态。

### 2. Snapshot remains per-unit runtime state
**Decision:** snapshot MUST 继续作为每个单位自己的运行时 baseline 容器存在，但 SHALL NOT 再继续扩展为手写固定字段集合。

**Rationale:** 真正会变化的是“某个单位上次同步到哪里”，不是全局同步规则。

### 3. UnitNativeSystem should iterate registry entries instead of handwritten branches
**Decision:** `UnitNativeSystem` SHOULD 遍历 registry/list 中的同步声明，而不是为每个属性继续写死分支。

**Rationale:** 这样可以把扩展点集中在注册表，而不是系统体和 snapshot 结构体里。

### 4. Compare-sync semantics remain unchanged
**Decision:** compare-sync 语义 MUST 保持不变：比较 current/final 与 baseline，仅在有效变化时同步原生。

**Rationale:** 当前 compare-sync 行为已经被证明可用，提案目标是收敛结构，不是重写语义。

## Risks / Trade-offs

- [风险] 如果 snapshot 也被错误做成全局存储，会造成跨单位状态污染。  
  [缓解] 在 spec 中明确禁止 registry/list 承载运行时 baseline。

- [风险] 一步引入过重 descriptor 结构，增加理解成本。  
  [缓解] 仅引入轻量 registry/list，保留现有 compare-sync 核心逻辑。

- [风险] 后续同步字段并不都是 `current/final` 模式。  
  [缓解] 先以当前连续属性型同步为范围，未来若出现异构字段再单独扩展。

## Migration Plan

1. 审核通过本次 OpenSpec 提案。
2. 引入静态 sync registry/list。
3. 将当前 `Health` / `Mana` 同步声明迁入 registry。
4. 将 per-unit snapshot 重构为可按 `attrTypeId` 记录 baseline 的结构。
5. 将 `UnitNativeSystem` 从手写分支迁移为 registry-driven 遍历。
6. 验证语义不变后，再允许扩展其他同步字段。

## Open Questions

- baseline 容器最终采用小数组/条目列表，还是更通用的映射结构。
- registry 是否只保存 `attrTypeId + nativeState`，还是未来允许带轻量转换策略。

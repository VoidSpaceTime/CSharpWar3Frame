## Context

当前实现已经迈出了正确一步：

- 用 registry/list 声明同步项
- 用 per-unit snapshot 保存 runtime baseline

但 registry 目前仍假设所有同步项都能表示为：

- `attrTypeId`
- `JUnitState`

这对于 `Health` / `Mana` 成立，但很快会遇到不兼容情况，例如：

- `AttackRange` 需要 `YD_SetUnitState(...)`
- 未来某些字段可能来自不同 ECS 来源
- 未来某些字段的写入规则与 `current/final -> SetUnitState` 模式不同

因此，registry 的真正职责不应只是“映射到哪个 native state”，而应是：

> **定义一条 projection rule：如何把 ECS 数据写到 native world。**

## Goals / Non-Goals

**Goals:**
- 扩宽 unit native sync registry 的表达能力。
- 允许不同同步项拥有不同的原生写入逻辑。
- 保持 per-unit snapshot 与 compare-sync baseline 设计不变。
- 避免把复杂特例逻辑重新散落回 `UnitNativeSystem` 里。

**Non-Goals:**
- 本提案不在提案阶段实现具体委托或静态方法签名。
- 本提案不引入重型脚本化/反射化 descriptor 框架。
- 本提案不把 per-unit baseline 放入全局 registry/list。

## Decisions

### 1. Registry must represent projection rules, not just native states
**Decision:** unit native sync registry/list MUST 表达 projection rule，而 SHALL NOT 继续局限于 `attrTypeId -> nativeState` 映射。

**Rationale:** 未来同步项的 native 写法不止一种，过窄 registry 只会把复杂度重新推回系统分支里。

### 2. Baselines remain per-unit runtime state
**Decision:** per-unit snapshot MUST 继续作为 baseline owner 保留，registry/list SHALL NOT 承载动态 baseline。

**Rationale:** projection rule 是全局静态声明，baseline 是单位级动态状态，这两者必须分离。

### 3. Projection apply logic should be explicit and centralized
**Decision:** 每个 projection rule SHOULD 绑定显式的 apply 逻辑（例如静态方法引用或等价机制），而不是要求 `UnitNativeSystem` 为每种原生写法继续堆特例分支。

**Rationale:** 这样可以在扩宽表达能力的同时，保持逻辑集中、可读、可扩展。

### 4. Compare-sync semantics remain unchanged
**Decision:** projection 规则升级后，compare-sync 的基本语义 MUST 保持不变：只有在 baseline 与当前 ECS 值产生有效差异时，才触发原生写入。

**Rationale:** 当前结构问题在于投影表达不足，而不是 compare-sync 本身有误。

## Risks / Trade-offs

- [风险] 直接把 lambda/闭包塞进 registry，导致配置层与执行逻辑强耦合。  
  [缓解] 倾向使用受控的静态 apply 逻辑，而不是任意匿名函数扩散。

- [风险] 过早引入重型 descriptor 框架，超过当前项目需要。  
  [缓解] 保持“中等宽度”抽象，只解决多种原生写法的扩展问题。

- [风险] projection rule 过宽后，baseline 结构不再适配所有字段。  
  [缓解] 先以当前连续数值型同步为主范围，异构字段另行评估。

## Migration Plan

1. 审核通过本次 OpenSpec 提案。
2. 将 `UnitNativeSyncRegistry` 从 state mapping 升级为 projection rule registry。
3. 将当前 `Health` / `Mana` 同步迁移到 projection rule 形式。
4. 以 `AttackRange` 作为第一个需要非 `SetUnitState(...)` 写法的扩展例子。
5. 保持 per-unit snapshot baseline 设计不变。
6. 验证 compare-sync 行为与现有语义保持一致。

## Open Questions

- 最终 apply 逻辑采用静态方法引用、受限委托，还是更显式的 writer object。
- baseline entry 是否需要从固定双 float 进一步扩宽，以支持更多 projection 形态。

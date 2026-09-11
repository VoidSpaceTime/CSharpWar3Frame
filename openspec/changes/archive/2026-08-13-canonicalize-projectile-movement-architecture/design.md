## Context

当前 projectile 体系存在三个同时成立的问题：

1. **双 runtime path 并存**：legacy `AbilityEffectExtend` 与 newer `AbilityEffectSystems` 都在仓库里，但数据形态和生命周期不一致。
2. **movement family 缺正式契约**：repo 里已有平面追踪、方向弹道入口，以及 bezier 数学工具，但没有正式的 movement-type / curve-type / control-point authoring contract。
3. **结构变更处理不安全**：至少 legacy 路径已经证明，在 `Query.ForEachEntity(...)` 内直接 `AddTag/RemoveTag/DeleteEntity` 会导致运行时 structural change failure。

同时，仓库已有规范对本次设计施加了硬约束：

- `Position` 必须继续是 effect/projectile motion 的唯一长期真相，不允许再造第二套 position truth。
- `projectile kind / movement type / search behavior` 这类结构语义保持 component-owned。
- `projectile speed / distance / duration / arrival threshold` 这类 tunable numeric values 应最终收敛到统一 numeric ownership。

## Goals / Non-Goals

**Goals:**
- 选定一个 canonical projectile runtime path。
- 定义 movement space 与 movement family 的正式 component contract。
- 为 cubic / quartic bezier 定义正式 control-point data contract。
- 为 snake / orbit 等特殊 movement family 留出正式扩展位，而不是继续塞进 ad-hoc 字段。
- 定义 projectile lifecycle 的安全 mutation strategy，避免在 movement query 内直接做结构变更。
- 明确 legacy path 的冻结 / 兼容 / 迁移策略。

**Non-Goals:**
- 本提案不进入运行时代码实现。
- 本提案不一次性实现所有 movement family 的完整运行时算法。
- 本提案不直接改 native effect execution 层。
- 本提案不强行把所有 motion tunables 立刻迁完到最终 numeric ownership，只定义收敛方向和桥接边界。
- 本提案不要求旧 legacy path 与新 path 在 Phase 1 立即 feature parity。

## Candidate Options

### Option A — Canonicalize the newer effect-entity projectile path (Recommended)

- 以 `ProjectileData` / `ProjectileLinearData` 所在的新 effect-entity 路径为 canonical runtime path。
- legacy `ProjectileBase` / `ProjectileOn*` 路径进入冻结状态；若需要兼容，走 adapter/shim，而不再扩成主线。
- 所有新 movement family / bezier control-point contract 都加在 canonical path 上。

**Pros:**
- 与 `Projects/test` 当前 authoring 路径一致。
- 更贴合当前 `EffectPending -> ProjectileArrived -> downstream effect` 的主线 effect flow。
- 更容易把 lifecycle 和 movement contract 收敛到一条线上。

**Cons:**
- 需要定义 legacy compatibility 策略，而不是继续忽略旧路径。
- `ProjectileLinearData` 目前只有数据入口，意味着 canonical 化后还要补齐运行时系统。

### Option B — Expand the legacy `AbilityEffectExtend` path into the primary projectile architecture

- 继续把 `ProjectileBase` 扩展成平面 / 三维 / 蛇形 / 环绕 / bezier 的主承载体。
- 新路径改为兼容层或过渡层。

**Pros:**
- 理论上可复用旧回调接口 `IProjectileOnStart/Travel/Arrive` 的开放性。

**Cons:**
- 当前 evidence 显示旧路径更像遗留链路，且 mutation strategy 已知不安全。
- 与 `Projects/test` 的当前 authoring 主线不一致。
- 会把更多 feature 继续压到历史包袱上。

### Option C — Keep dual runtime paths and evolve both

- 新旧两条 projectile path 同时升级，分别支持 movement family。

**Pros:**
- 兼容性表面最好。

**Cons:**
- ownership、验证、迁移和维护成本都会显著上升。
- 极易形成永久双真相源。

## Decisions

### 1. The newer effect-entity projectile path becomes the canonical runtime path
**Decision:** canonical projectile runtime path MUST 建立在 `ProjectileData` / `ProjectileLinearData` 所在的新 effect-entity flow 上。legacy `AbilityEffectExtend` projectile path SHALL NOT 继续扩张为长期主线。

**Rationale:** 当前样例 authoring 已经站在新路径上；新路径与现有 effect gate 更一致；legacy 路径缺少运行价值证据且已有安全问题。

### 2. Projectile movement semantics split into shared core + family-specific components
**Decision:** projectile 架构 SHALL NOT 只靠一个不断膨胀的 “万能 `ProjectileData`” 或 “万能 type + 海量可选字段” 承载全部 movement family。

建议结构：
- shared core：`ProjectileData` / `ProjectileLinearData` 或其后继核心组件，只承载共享 lifecycle/runtime identity
- movement space discriminator：例如 `ProjectileSpaceMode`（`Planar`, `Spatial`）
- movement family discriminator：例如 `ProjectileMovementMode`（`Homing`, `Directional`, `Bezier`, `Snake`, `Orbit`）
- family-specific structure component：
  - `ProjectileBezierMotionData`
  - `ProjectileSnakeMotionData`
  - `ProjectileOrbitMotionData`
  - 以及必要时的 `ProjectileHomingMotionData` / `ProjectileDirectionalMotionData`

**Rationale:** movement family 的结构字段差异很大，单一大组件会迅速变成 sparse bucket，不利于 ECS 清晰性。

### 3. Movement space and family topology remain structure-owned; motion tunables converge to numeric ownership
**Decision:** 
- `space mode` / `movement mode` / `curve degree` / `control point topology` MUST remain component-owned structure semantics.
- `speed` / `distance` / `duration` / `arrival threshold` / `snake amplitude` / `snake frequency` / `orbit radius` / `orbit angular speed` 等 tunable motion numbers MUST 有明确的 canonical numeric ownership 方向，而 SHALL NOT 永久散落成各路径私有硬编码字段。

**Rationale:** 这与仓库已有 `ability-numeric-attribute-ownership` 约束一致，也能避免未来 projectile movement 再次进入字段散落状态。

### 4. Bezier motion is a first-class movement family with explicit cubic/quartic topology
**Decision:** bezier SHALL 作为正式 movement family，而不是 helper-only math。Phase 1 contract MUST 至少覆盖：
- `ProjectileBezierDegree`: `Cubic`, `Quartic`
- 控制点数据：cubic 至少 2 个 control points；quartic 至少 3 个 control points
- control-point 数据作为 structure-owned authoring/runtime parameters
- Phase 1 control-point 坐标表达固定为 `RelativeToStartEnd`

这意味着 bezier 组件显式承载“相对起点/终点偏移”，而不是把这些偏移塞进 `EffectTargetInfo`。

**Rationale:** 仓库已有 bezier 数学工具，但还没有可 author / 可调度 / 可验证的 projectile 曲线语义。

### 5. Snake and orbit are explicit movement families, not ad-hoc modifiers hidden inside one generic projectile mode
**Decision:** snake / orbit SHALL 作为明确 movement family 被建模，并拥有各自 component-owned structure contract。

示例：
- `ProjectileSnakeMotionData`: offset basis / phase semantics / traversal basis
- `ProjectileOrbitMotionData`: orbit center semantics / orbit relation / exit semantics

**Rationale:** 这两类运动不是简单的“给直线弹道多加一个布尔值”，它们需要自己的语义面和测试面。

### 6. Projectile movement systems MUST NOT perform structural changes directly inside query iteration
**Decision:** projectile movement / tracking / arrival progression systems MUST NOT 在 `Query.ForEachEntity(...)` 内直接 `AddTag`、`RemoveTag`、`DeleteEntity`。

允许的 canonical pattern：
- query 内只推进普通组件状态/意图/请求
- 结构变更通过：
  - query 外统一 apply（仓库里已有 `toDelete` 收集后统一处理的模式）
  - 或独立 request/dirty/lifecycle consumer system

**Rationale:** 这与仓库中已有的 `BuffExpireSystem`、`EffectRuntimeSystem`、`MoveCommand` / `EffectDirty` / lifecycle-state 分层 pattern 一致，也修复了当前 legacy projectile 已暴露的 structural change 问题。

### 7. `Position` remains the sole motion truth
**Decision:** 无论 movement family 是 planar / spatial / snake / orbit / bezier，runtime 最终运动真相 MUST 仍落在 ECS `Position` 上。附加 movement data 仅作为生成/推进 `Position` 的参数，SHALL NOT 变成第二套长期位置真相。

**Rationale:** 与 effect architecture 现有 spec 一致，避免特效/弹道/逻辑三套位置链条重新分裂。

### 8. `EffectTargetInfo` stays generic and SHALL NOT absorb projectile-family path data
**Decision:** `EffectTargetInfo` SHALL 继续只承载通用目标信息（当前是 `targetUnit` 与 `targetX/targetY` 这一层语义），而 SHALL NOT 承载 bezier 控制点偏移、snake 波形参数、orbit 中心关系等 projectile-family-specific path data。

这类数据必须进入 projectile-specific movement components，例如 `ProjectileBezierMotionData`、`ProjectileSnakeMotionData`、`ProjectileOrbitMotionData`。

**Rationale:** `EffectTargetInfo` 是通用 effect 目标语义，不应被 projectile 特有 authoring/runtime contract 污染；否则会把整个 effect 流程强行 projectile-aware。

### 9. Legacy `AbilityEffectExtend` path is frozen as non-canonical runtime, but its hook surface may remain long-lived
**Decision:** `AbilityEffectExtend` projectile path 在新架构下 SHALL NOT 继续承担新增 movement family 的主 runtime 实现；但 legacy hook surface MAY 作为长期 Hook Bridge Layer 保留，而不要求短期移除。

**Rationale:** 长期框架需要稳定扩展点，但不应让历史 movement runtime 继续膨胀。保留 hook surface、冻结旧 runtime，是兼容性与架构收敛之间更稳的平衡。

### 10. Legacy `IProjectileOnStart/Travel/Arrive` callbacks become a long-lived Hook Bridge Layer
**Decision:** `IProjectileOnStart`、`IProjectileOnTravel`、`IProjectileOnArrive` SHALL 作为长期保留的 Hook Bridge Layer 存在，但 SHALL NOT 再承担 canonical projectile lifecycle truth。

- 它们通过显式 bridge/dispatch system 接入 canonical projectile lifecycle
- `OnStart` 可作为初始化型 hook
- `OnTravel` 最多作为受限观察/修饰钩子或 request writer，而 SHALL NOT 拥有 lifecycle truth、arrival ownership 或结构变更所有权
- `OnArrive` 可作为到达后的 payload/扩展 hook，但到达事实本身由 canonical lifecycle 决定

**Rationale:** 这样可以长期保留模板/脚本扩展面，同时避免新架构重新滑回“流程由隐式回调塑形”的旧模式。

### 11. The repository SHOULD define a clearer long-term hook contract such as `IProjectileHooksV2`
**Decision:** repository SHOULD 定义一个更明确的长期 hook contract（例如 `IProjectileHooksV2`），用来逐步替代旧 `OnTravel` 的模糊 `bool` 语义，同时允许旧 hook 与新 hook 在 Hook Bridge Layer 中长期并存。

建议方向：
- 最小 surface 保持为：
  - `OnStart(...)`
  - `OnTravel(...) -> ProjectileTravelDecision`
  - `OnArrive(...)`
- 只有 `OnTravel` 返回显式结果，而不是单个难以解释的 `bool`
- `ProjectileTravelDecision` 在 Phase 1 最小化为：
  - `Continue`
  - `SuppressArrivalThisTick`
  - `RequestExpire`
- bridge layer 统一分发 legacy 与 V2 hook，并把它们转换成 canonical request/state
- legacy `OnTravel(false)` 的语义被正式收敛为 `SuppressArrivalThisTick`，而不是“停止移动”或“接管生命周期”

**Rationale:** 长期保留旧接口不代表要永久承受模糊语义；V2 contract 提供更成熟的框架扩展面。

## Risks / Trade-offs

- [风险] 若直接扩 `EffectTargetInfo` 为 3D，会波及非 projectile effect path。  
  [缓解] 在实现前先决定 3D target/anchor 是否通过 projectile-specific component 承载，而不是默认扩全局 target info。

- [风险] snake/orbit 的 tunable 参数归属不清，可能在 Phase 1 落成新的 numeric 双真相。  
  [缓解] 在 design 里先定义归属边界和 bridge 退出条件，再进入实现。

- [风险] canonicalize 新路径后，legacy shim 长期残留。  
  [缓解] 将 legacy surface 的长期角色明确限定为 Hook Bridge Layer，并禁止旧 runtime 继续扩 movement family。

- [风险] 当前 repo 尚未完全证明两条 projectile 路径都已接线运行。  
  [缓解] 在实现前把“live path verification”列为前置验证任务。

## Migration Plan

1. 审核通过本次 projectile architecture proposal。
2. 确认 canonical path 与 legacy freeze/adapter 边界。
3. 定义 movement space / movement family / bezier topology 的正式 component contract。
4. 定义 projectile lifecycle 的安全 mutation strategy（request/state/apply）。
5. 补 canonical path 的运行时执行与 scenario validation。
6. 只在 canonical path 上扩 movement family；legacy path 若仍保留，仅作为兼容层。
7. 对 `Projects/test` 样例进行 authoring / build / runtime validation。
8. 若决定引入 `IProjectileHooksV2`，定义其与 legacy hook surface 的 bridge/dispatch 关系。
9. 明确 legacy `OnTravel(false)` 到 `SuppressArrivalThisTick` 的映射，并验证它不会夺取 canonical lifecycle truth。

## Open Questions

- 3D projectile target/anchor 是否应扩展通用目标语义，还是继续通过 projectile-specific target/anchor component 承载？
- snake/orbit 的 tunable 参数在 Phase 1 是否直接进入 canonical numeric ownership，还是允许短期 bridge？
- `RequestExpire` 是否已足够覆盖 Phase 1 的 lifecycle 请求，还是需要保留额外 decision 以承载特殊 payload 请求？

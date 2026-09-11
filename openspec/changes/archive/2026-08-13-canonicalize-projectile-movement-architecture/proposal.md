## Why

当前仓库中存在两条 projectile 路线：

- 旧的 `War3Frame/Src/Systems/AbilityEffectExtend/ProjectileSystem.cs`，基于 `ProjectileBase` 与 `ProjectileOnStart/Travel/Arrive` 标签回调。
- 新的 `War3Frame/Src/Systems/Ability/AbilityEffectSystems.cs`，基于 `ProjectileData` / `ProjectileLinearData` / `ProjectileArrived` 的 effect-entity 流程。

这两条路线在数据模型、生命周期语义、运动维度和结构变更安全性上都不一致：

- 旧路径带有 `targetZ` / `height` / `startX` / `startY` / `distance` 等字段，但实际推进仍主要是平面 XY。
- 新路径已被 `Projects/test` 模板作者使用，但目前只完整覆盖了基础追踪弹道，线性方向弹道只有数据入口，还缺完整执行层。
- 仓库已有 `BezierQuadratic` / `BezierCubic` 数学工具，但 projectile 体系还没有正式的 bezier authoring/runtime contract。
- 当前 review 已确认 legacy `ProjectileSystem` 在查询循环内直接 `AddTag/RemoveTag/DeleteEntity` 会触发 `StructuralChangeException`，说明 projectile lifecycle 不能继续依赖“在 movement loop 内直接做结构变更”的方式。

用户现在希望正式支持：

- 平面 / 三维 / 蛇形 / 环绕等 movement type
- 三阶 / 四阶 bezier 的控制点数据
- 安全的 projectile lifecycle / tag / request 处理方式

这已经不是单文件 bugfix，而是 projectile runtime / authoring contract / lifecycle ownership 的架构级议题。

## What Changes

- 采用一个 canonical projectile architecture，明确哪条 runtime path 是主线，以及 legacy callback surface 如何作为长期 Hook Bridge Layer 存在。
- 定义 projectile movement contract：
  - movement space（例如平面 / 三维）
  - movement family（例如 homing / linear / bezier / snake / orbit）
  - control-point topology（至少支持 cubic / quartic bezier）
- 规定 `EffectTargetInfo` 继续只承载通用目标语义，而 SHALL NOT 扩成承载曲线相对偏移或 family-specific motion data 的容器。
- 规定 Phase 1 的 bezier control points 采用“相对起点/终点偏移”表达，而不是把控制点塞进通用 target info。
- 规定 movement topology / curve topology 等结构语义保持 component-owned。
- 规定 projectile speed / distance / duration / arrival-threshold 以及新增的 motion tunables 与统一 numeric ownership 的关系，避免再次散落成长期双真相源。
- 规定 projectile movement systems 不再在查询循环内直接做结构变更，而改为仓库已有的安全 pattern：query 内只推进 state/request，结构变更由查询外 apply 或独立消费系统处理。
- 明确 legacy `AbilityEffectExtend` projectile 路径在本次架构中的位置：不再扩成主线 movement runtime，但其 callback surface 可作为长期 Hook Bridge Layer 保留。
- 明确 legacy `IProjectileOnStart/Travel/Arrive` 回调链长期保留为 compatibility/extension hook surface：继续存在，但不再拥有 canonical lifecycle truth、arrival ownership 或结构变更 ownership。
- 定义推荐的长期 hook 升级方向（例如 `IProjectileHooksV2`）：
  - `OnStart(...)`
  - `OnTravel(...) -> ProjectileTravelDecision`
  - `OnArrive(...)`
  其中只有 `OnTravel` 返回显式决策，用于替代旧 `OnTravel` 的模糊 `bool` 语义，同时允许新旧 hook surface 在桥接层长期并存。

## Candidate Directions

- **方案 A（推荐）**：以新 `ProjectileData` / `ProjectileLinearData` effect-entity 路径为 canonical path，legacy `AbilityEffectExtend` 冻结为非主线 runtime，并保留长期 Hook Bridge Layer。
- **方案 B**：继续扩展 legacy `ProjectileBase` 路径，让新旧双轨长期共存。
- **方案 C**：同时重构两条路径并追求 feature parity。

推荐 A 的理由：

- `Projects/test` 样例已经在 author 新路径。
- 新路径和 `EffectPending` / `ProjectileArrived` / 下游效果 gate 的结构更接近当前 effect runtime 主线。
- legacy 路径当前更像局部遗留链路；保留 hook surface 比继续扩它的 movement runtime 更稳。

## Capabilities

### New Capabilities
- `canonical-projectile-movement-architecture`: 定义 canonical projectile runtime path、movement family/space contract、bezier control-point contract 与 safe lifecycle mutation architecture。

### Modified Capabilities
- `ecs-native-effect-separation`
- `ability-numeric-attribute-ownership`

## Impact

- `War3Frame/`: **直接影响**。projectile 组件、effect runtime flow、movement/lifecycle ownership 都在此处。
- `War3Frame.Generator/`: **当前无直接代码改动承诺**，但若后续 authoring contract / registration 需要 generator 协作，必须在实现提案中单独说明。
- `FrameBuild/`: **当前无直接改动承诺**。
- `CSharpWar3Frame/`: **当前无直接改动承诺**。
- `Projects/*`: **预期受影响**。`Projects/test` 已使用新 `ProjectileData` 路径；若 canonical contract 变化，样例和集成验证必须随之更新。
- 本次变更仅新增 OpenSpec 工件，不进入任何运行时代码修改。

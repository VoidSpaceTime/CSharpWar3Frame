## Why

当前仓库中的 effect / projectile 流程已经隐含出一个较健康的方向：effect logic 倾向于以 ECS entity 为中心，native effect handle 倾向于作为执行层资源。但这一边界还没有被正式固化，因此 helper、native system 与持续特效更新之间仍存在 ownership 混杂：有些行为是 entity-driven，有些行为又实际变成 helper-driven 或 native-first。

本提案要正式确立一套统一规则：

- effect logic 保持 entity-driven
- native effect handle 仅作为 execution-only resource
- helpers 保持以立即创建/一次性请求为主，而不拥有长期真相
- 持续特效更新统一走 `0.02s` cadence
- effect motion 直接复用现有 `Position` 组件，不发明第二套 position-truth model
- Lua/native integration 可以提供执行能力，但不得定义 ECS semantic ownership

## What Changes

- 正式定义 ECS 为 long-lived effect semantics 的唯一真相源。
- 正式定义 native effect handle 为 execution-only resource。
- 定义 effect 的 attachment、lifetime、appearance、animation request、projectile visual ownership 与 motion 语义边界。
- 规定 effect motion 必须复用 `Position`，不得引入第二套运动真相组件。
- 规定持续特效 reconciliation 统一由 `0.02s` 的更新路径推进。
- 规定 helper 保留即时创建/一次性请求 ergonomics，但不得继续拥有 sustained truth。

## Capabilities

### New Capabilities
- `ecs-native-effect-separation`: 定义 effect entity 与 native effect handle 的 ownership 边界，以及 Position-based motion 的统一规则。

## Impact

- 直接影响 `War3Frame` 的 effect architecture、native effect execution、helper 边界与 projectile visual synchronization。
- `War3Frame.Generator` 预期无直接行为变更，但后续实现要确认生成/注册逻辑不依赖旧 effect helper ownership。
- `FrameBuild` 预期无直接行为变更，但后续实现要确认构建编排不依赖旧 effect runtime ownership。
- `CSharpWar3Frame` 预期无直接行为变更，但后续实现要确认 CLI / tooling / 初始化入口不受影响。
- `Projects/*` 在实现后必须重新验证点特效、附着特效、持续特效、投射物视觉与到达/销毁语义。
- 本次变更仅新增 OpenSpec 工件，不进入运行时代码修改。

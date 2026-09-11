## ADDED Requirements

### Requirement: The repository MUST define one canonical projectile runtime path
repository MUST 明确 projectile runtime 的 canonical path，并 SHALL NOT 长期让 legacy `AbilityEffectExtend` projectile path 与 newer effect-entity projectile path 同时作为平行主线扩展。

#### Scenario: A new projectile movement family is introduced
- **WHEN** 仓库需要新增新的 projectile movement family
- **THEN** 该 family MUST 只挂到 canonical projectile runtime path 上
- **AND** legacy projectile path SHALL NOT 被继续扩成长期主线

### Requirement: Projectile movement topology MUST remain structure-owned
projectile movement space、movement family、curve degree 与 control-point topology MUST remain component-owned structure semantics，而 SHALL NOT 被误归为 generic numeric ownership。

#### Scenario: Ability template configures projectile movement type
- **WHEN** 某个 ability template 配置 projectile movement type、space mode 或 bezier degree
- **THEN** 这些字段 MUST 以普通组件或组件字段 author
- **AND** runtime MUST 将其视为 structure semantics rather than numeric attribute ownership

### Requirement: `EffectTargetInfo` MUST remain generic target semantics
`EffectTargetInfo` MUST 继续只承载通用 effect target semantics，而 SHALL NOT 吸收 projectile-family-specific path data such as relative bezier offsets, snake waveform structure, or orbit relation semantics.

#### Scenario: Projectile bezier path is authored
- **WHEN** 某个 projectile 被 author 为 bezier movement
- **THEN** 相对起点/终点控制点偏移 MUST 存放在 projectile-specific movement component 中
- **AND** they SHALL NOT be stored on `EffectTargetInfo`

### Requirement: Projectile motion tunables MUST have canonical numeric ownership direction
projectile speed、distance、duration、arrival threshold 以及新增 motion tunables（例如 snake amplitude/frequency 或 orbit radius/angular speed）MUST 有明确的 canonical numeric ownership 方向，而 SHALL NOT 永久散落在多条 projectile 路径的私有字段中。

#### Scenario: Runtime reads projectile speed or orbit radius
- **WHEN** projectile runtime systems 读取 speed、distance、arrival threshold 或其他 motion tunables
- **THEN** 这些读取 MUST 有单一 canonical ownership 方向
- **AND** temporary bridges, if any, MUST have explicit removal conditions

### Requirement: Bezier projectile movement MUST support explicit cubic and quartic control-point contracts
projectile bezier movement MUST 成为正式 movement family，并至少定义 cubic 与 quartic 两种 control-point topology contract。

#### Scenario: Ability authors a cubic projectile bezier path
- **WHEN** 某个 projectile 被 author 为 cubic bezier movement
- **THEN** contract MUST 要求与 cubic topology 对应的 control-point data
- **AND** Phase 1 MUST express those control points as offsets relative to the projectile start/end anchors

#### Scenario: Ability authors a quartic projectile bezier path
- **WHEN** 某个 projectile 被 author 为 quartic bezier movement
- **THEN** contract MUST 要求与 quartic topology 对应的 control-point data
- **AND** Phase 1 MUST express those control points as offsets relative to the projectile start/end anchors

### Requirement: Special movement families MUST be explicit rather than hidden ad-hoc modifiers
snake、orbit 等特殊 projectile movement families MUST 作为明确 family contract 被建模，而 SHALL NOT 仅通过某个 generic projectile mode 上的隐式 bool/offset hack 表达。

#### Scenario: Orbit projectile is authored
- **WHEN** 某个 projectile 被 author 为 orbit movement
- **THEN** repository MUST 有显式 orbit family contract
- **AND** orbit-specific structure semantics MUST be inspectable from ECS data

### Requirement: Projectile runtime MUST keep `Position` as the sole motion truth
无论 projectile movement family 是 planar、spatial、bezier、snake 还是 orbit，runtime 最终运动真相 MUST 继续落在 ECS `Position` 上，而 SHALL NOT 引入第二套长期位置真相模型。

#### Scenario: Projectile visual or downstream effect consumes movement result
- **WHEN** projectile movement 在运行时推进
- **THEN** downstream visuals and effect consumers MUST read the updated ECS `Position`
- **AND** movement-family-specific data MUST only parameterize that update rather than replace it

### Requirement: Projectile movement systems MUST NOT perform structural changes directly inside query iteration
projectile movement / tracking / arrival progression systems MUST NOT 在查询循环内直接 `AddTag`、`RemoveTag` 或 `DeleteEntity`；它们 MUST 使用 canonical safe mutation pattern（例如 query 外统一 apply，或 request/state + consumer system）。

#### Scenario: Projectile reaches arrival semantics during movement update
- **WHEN** 某个 projectile 在 movement progression 中进入 arrival condition
- **THEN** movement loop MUST NOT 直接执行 structural mutation
- **AND** arrival intent/state MUST 通过 canonical safe mutation pattern 被后续 apply/consume

### Requirement: Legacy projectile compatibility MUST be explicit and bounded
若 legacy `AbilityEffectExtend` projectile path 仍需保留兼容能力，其 compatibility surface MUST 被显式建模，并 SHALL NOT 重新成为 parallel primary runtime path。

#### Scenario: Legacy projectile authoring still exists
- **WHEN** 仓库仍需接受 legacy projectile authoring or callback path
- **THEN** proposal/design/tasks MUST 说明 compatibility strategy
- **AND** they MUST 明确该 compatibility surface 的长期角色边界

### Requirement: Legacy projectile callbacks MUST be frozen as compatibility-only hooks
`IProjectileOnStart`、`IProjectileOnTravel`、`IProjectileOnArrive` MUST 被视为 legacy compatibility hooks，而 SHALL NOT 再承载新 movement family 的主流程 ownership。

#### Scenario: New projectile movement family is introduced after canonicalization
- **WHEN** 仓库新增新的 projectile movement family
- **THEN** that family SHALL NOT depend on legacy `IProjectileOn*` callbacks as its primary execution surface
- **AND** any retained legacy callback integration MUST flow through an explicit adapter to the canonical projectile lifecycle

### Requirement: The repository SHOULD provide a clearer long-term projectile hook contract
repository SHOULD 定义一个更明确的长期 projectile hook contract（例如 `IProjectileHooksV2`），用于表达显式 hook decisions，而不是继续长期依赖模糊的 legacy `bool` return semantics。

#### Scenario: New projectile extension point is authored for travel behavior
- **WHEN** 仓库需要 author 新的 projectile travel extension behavior
- **THEN** the preferred long-term hook surface SHOULD keep a minimal lifecycle-shaped surface with `OnStart(...)`, `OnTravel(...)`, and `OnArrive(...)`
- **AND** only `OnTravel(...)` SHOULD return explicit outcomes or requests
- **AND** any legacy hook bridging MUST convert those outcomes into canonical projectile lifecycle state/request handling

#### Scenario: Legacy travel hook returns false
- **WHEN** legacy `IProjectileOnTravel` returns `false`
- **THEN** bridge behavior MUST interpret it as `SuppressArrivalThisTick`
- **AND** it SHALL NOT be interpreted as direct movement stop or lifecycle ownership transfer

#### Scenario: V2 travel hook requests expire
- **WHEN** `IProjectileHooksV2.OnTravel(...)` returns `RequestExpire`
- **THEN** canonical runtime MUST convert that result into lifecycle/state request handling
- **AND** expiration/cleanup SHALL still be resolved by canonical projectile lifecycle rather than direct hook-side structural mutation

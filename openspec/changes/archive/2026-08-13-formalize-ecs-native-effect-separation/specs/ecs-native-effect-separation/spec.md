## ADDED Requirements

### Requirement: ECS components MUST be the sole source of truth for long-lived effect state
long-lived effect semantics such as motion, attachment, lifetime, appearance, animation request, and projectile visual ownership MUST live in ECS and SHALL NOT depend on native or Lua-side truth.

#### Scenario: Sustained effect remains interpretable from ECS
- **WHEN** 某个 persistent effect entity 持续存在
- **THEN** 它的语义状态 MUST 能仅通过 ECS components 被完整解释

### Requirement: Native effect handles MUST be execution-only resources
native effect handles MUST represent execution resources only and SHALL NOT define long-lived effect semantics.

#### Scenario: Native handle is recreated
- **WHEN** 某个 effect entity 的 native handle 丢失并被重建
- **THEN** effect MUST be restorable from ECS-owned state without semantic loss

### Requirement: Movable effects MUST reuse `Position` as the only motion truth
movable effects 与 projectile visuals MUST 直接复用 `Position`，而 SHALL NOT 引入第二套 position-truth model。

#### Scenario: Projectile visual advances
- **WHEN** 某个 projectile-related effect 在运行时推进
- **THEN** visual execution MUST consume the updated ECS `Position`

### Requirement: Sustained effect updates MUST run at 0.02 second cadence
persistent effect reconciliation for follow behavior, motion synchronization, or lifetime progression MUST run at `0.02s`。

#### Scenario: Attached sustained effect follows owner
- **WHEN** attached sustained effect 随着 owner 持续移动
- **THEN** reconciliation MUST execute on the `0.02s` sustained update path

### Requirement: Helpers MUST remain immediate-oriented and SHALL NOT own long-lived truth
helpers MAY create effect entities or dispatch one-shot requests immediately, but SHALL NOT become the owner of sustained effect semantics.

#### Scenario: Helper creates a persistent effect
- **WHEN** helper 创建某个 persistent effect
- **THEN** helper 完成后，long-lived state MUST exist in ECS rather than helper-local ownership

### Requirement: Attachment MUST be modeled as ECS intent
attached effects MUST store target entity and attachment semantics in ECS even if native APIs perform the bind execution.

#### Scenario: Attachment target becomes invalid
- **WHEN** attached effect 的 target 被移除或失效
- **THEN** attach/detach/cleanup behavior MUST be determined from ECS-owned attachment semantics

### Requirement: Lifetime MUST be modeled explicitly in ECS
one-shot, timed, and manual-destroy effect lifetimes MUST be ECS-visible.

#### Scenario: Timed effect expires
- **WHEN** 某个 timed effect 的 ECS lifetime 到期
- **THEN** cleanup MUST be triggered from ECS-owned expiration state

### Requirement: Animation MUST be requested through ECS-visible intent
animation playback MUST be represented as ECS-visible request state and consumed by native execution systems.

#### Scenario: One-shot animation request is consumed
- **WHEN** effect entity 带有 pending animation request
- **THEN** native execution MUST play the animation and clear the request

### Requirement: Projectile visuals MUST be derived from projectile ECS state
projectile visuals MUST be derived from projectile ECS state rather than becoming a second motion owner.

#### Scenario: Projectile arrives
- **WHEN** projectile entity reaches its arrival semantics in ECS
- **THEN** projectile visual cleanup MUST follow ECS arrival state

### Requirement: Lua or native integration SHALL NOT define ECS truth ownership
Lua/Jass/native integrations MAY provide execution functions only and SHALL NOT define the ownership boundary for effect semantics.

#### Scenario: Native API performs attachment internally
- **WHEN** native API internally attaches an effect
- **THEN** ECS MUST remain the authoritative owner of attachment semantics

### Requirement: Proposal and design MUST include cross-project impact analysis
任何 effect architecture proposal / design MUST 明确说明对 `War3Frame`、`War3Frame.Generator`、`FrameBuild`、`CSharpWar3Frame` 与 `Projects/*` 的影响或非影响结论。

#### Scenario: An effect architecture change is proposed
- **WHEN** 新的 effect architecture proposal 被创建
- **THEN** proposal 与 design MUST document cross-project impact boundaries
- **AND** they MUST explain why the change is runtime-local or why other projects are affected

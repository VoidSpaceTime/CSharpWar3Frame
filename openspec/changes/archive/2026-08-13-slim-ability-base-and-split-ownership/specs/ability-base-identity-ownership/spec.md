## ADDED Requirements

### Requirement: `AbilityBase` MUST remain only as ability identity/minimal semantics
`AbilityBase` MUST 作为能力身份层 / 最小公共语义层保留，而 SHALL NOT 继续充当 numeric bucket、runtime-state bucket 或 behavior-structure bucket。

#### Scenario: Ability entity is queried by common casting logic
- **WHEN** 通用能力系统需要识别某个 entity 的基础技能语义
- **THEN** `AbilityBase` MAY 提供最小公共语义
- **AND** it SHALL NOT be required to also own numeric, runtime-state, or structure truth

### Requirement: `AbilityBase` whitelist MUST initially include only `level` and `targetType`
`AbilityBase` 的白名单字段 MUST 初始限定为 `level` 与 `targetType`。

#### Scenario: Template authors a level-aware ability
- **WHEN** 某个 ability template 配置技能实体
- **THEN** `AbilityBase` MUST be allowed to carry `level`

#### Scenario: Targeting semantics are needed
- **WHEN** casting or UI logic 需要基础目标类型语义
- **THEN** `AbilityBase` MUST be allowed to carry `targetType`

### Requirement: Tunable numeric fields SHALL NOT remain in `AbilityBase`
tunable numeric fields such as mana cost, cooldown, cast range, cast time, channel duration, radius, duration, projectile speed, projectile distance, width, arrival threshold, damage amount, heal amount, max targets, and charges SHALL NOT remain in `AbilityBase`。

#### Scenario: Runtime reads mana cost or cooldown
- **WHEN** runtime systems need mana cost, cooldown length, or cast range
- **THEN** these values MUST be read from canonical numeric ownership rather than `AbilityBase`

### Requirement: Runtime process values SHALL NOT remain in `AbilityBase`
runtime process values such as current cooldown, current charges, channel progress, ammo current, or recover remaining SHALL NOT remain in `AbilityBase`。

#### Scenario: Runtime tracks remaining cooldown
- **WHEN** a skill enters cooldown
- **THEN** remaining cooldown MUST be stored in runtime state ownership rather than `AbilityBase`

### Requirement: Behavior structure fields SHALL NOT remain in `AbilityBase`
projectile, area search, damage/heal structure, target filter, buff apply, model/effect path, and hit semantics SHALL NOT remain in `AbilityBase`。

#### Scenario: Ability uses projectile delivery and damage payload
- **WHEN** a skill is authored as projectile-based with damage payload
- **THEN** projectile and damage structure MUST remain component-owned rather than stored in `AbilityBase`

### Requirement: Ability numeric ownership MUST converge to `AbilityAttribute`
ability-owned numeric values MUST converge to `AbilityAttribute` ownership.

#### Scenario: Ability numeric values are modified by buffs or level scaling
- **WHEN** buffs, talents, equipment, or level changes alter ability numbers
- **THEN** those numeric values MUST be recalculated through canonical numeric ownership rather than `AbilityBase`

### Requirement: Proposal and design MUST include cross-project impact analysis
任何 ability-base architecture proposal / design MUST 明确说明对 `War3Frame`、`War3Frame.Generator`、`FrameBuild`、`CSharpWar3Frame` 与 `Projects/*` 的影响或非影响结论。

#### Scenario: An ability-base architecture change is proposed
- **WHEN** 新的 ability-base architecture proposal 被创建
- **THEN** proposal 与 design MUST document cross-project impact boundaries
- **AND** they MUST explain why the change is runtime-local or why other projects are affected

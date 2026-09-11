## ADDED Requirements

### Requirement: Ability numeric values MUST use the attribute/value/modifier model
ability numeric values such as mana cost, cooldown, cast range, damage, heal, radius, projectile speed, duration, width, distance, charges, and similar tunable numbers MUST 由 attribute/value/modifier 模型拥有。

#### Scenario: Ability template authors mana cost and cooldown
- **WHEN** 某个 ability template 按等级配置技能
- **THEN** mana cost 与 cooldown MUST 作为 attribute-backed numeric values 被 author
- **AND** runtime readers MUST 从该模型读取它们

### Requirement: Ability behavior and structure MUST remain ordinary components
ability behavior and structure fields MUST 保持为普通 ECS 组件或组件字段，而 MUST NOT 被重新定义为 numeric attribute ownership。

#### Scenario: Ability template configures target type and projectile mode
- **WHEN** 某个 ability template 配置 target type、projectile kind 或 search behavior
- **THEN** 这些字段 MUST remain component-owned
- **AND** only tunable numeric portions MAY move to attribute ownership

### Requirement: Ability templates MUST remain level-aware
ability templates MUST 保持 level-aware configuration semantics。

#### Scenario: Ability damage scales by level
- **WHEN** 某个 ability template 分别配置 level 1 与 level 3
- **THEN** 模板 MUST 能 author 不同的 base numeric values
- **AND** resulting runtime values MUST be readable through the attribute model

### Requirement: Unit and item templates MUST NOT be forced into ability level semantics
unit templates 与 item templates MUST NOT 因 ability template design 而被强制 adopt `Configure(Entity entity, int level)`。

#### Scenario: Unit template remains non-level-based
- **WHEN** 某个 unit template 通过其既有接口配置 entity
- **THEN** repository MUST NOT 因 ability-level semantics 而强制修改它的 authoring contract

#### Scenario: Item template remains non-level-based
- **WHEN** 某个 item template 通过其既有接口配置 entity
- **THEN** repository MUST NOT 因 ability-level semantics 而强制修改它的 authoring contract

### Requirement: Runtime ability numeric reads MUST converge to one authoritative source
runtime systems 在读取 ability numeric values 时 MUST 收敛到单一 authoritative attribute-backed source，而 SHALL NOT 长期混用旧字段与新 attribute ownership。

#### Scenario: Runtime cost/effect readers evaluate ability numbers
- **WHEN** runtime systems 读取 mana cost、cooldown、cast range、damage、heal、radius、projectile speed 或 duration
- **THEN** 这些读取 MUST NOT 长期处于双真相源状态
- **AND** temporary bridges, if any, MUST have explicit removal conditions

### Requirement: Ability numeric values MUST support modifier-driven recalculation
ability numeric values MUST 保持 flat、additive-percent 与 multiplicative-percent modifier-driven recalculation 能力。

#### Scenario: Buff modifies an ability numeric value
- **WHEN** 某个 modifier 修改 ability-owned numeric value
- **THEN** final numeric value MUST be recalculated through the repository’s modifier pipeline semantics

### Requirement: Proposal and design MUST include cross-project impact analysis
任何 ability numeric architecture proposal / design MUST 明确说明对 `War3Frame`、`War3Frame.Generator`、`FrameBuild`、`CSharpWar3Frame` 与 `Projects/*` 的影响或非影响结论。

#### Scenario: An ability numeric architecture change is proposed
- **WHEN** 新的 ability numeric architecture proposal 被创建
- **THEN** proposal 与 design MUST document cross-project impact boundaries
- **AND** they MUST explain why the change is runtime-local or why other projects are affected

## ADDED Requirements

### Requirement: Ability architecture MUST separate Mount, Trigger, Flow, and Settlement
能力系统 MUST 显式区分 Mount、Trigger、Flow、Settlement 四层。

#### Scenario: A talent ability is mounted without using a slot
- **WHEN** 某个能力以天赋/系统赋予方式挂载到单位上
- **THEN** the architecture MUST represent that through the mount layer without requiring slot ownership

#### Scenario: A passive ability triggers on damage taken
- **WHEN** 某个被动能力在单位受伤时触发
- **THEN** the architecture MUST represent that through the trigger layer rather than treating it as an active cast

### Requirement: Passive abilities SHALL be treated as trigger modes
被动技能 SHALL 作为 trigger mode 被建模，而不是完全独立于技能主架构之外。

#### Scenario: A death-triggered ability exists
- **WHEN** 某个能力在单位死亡时触发
- **THEN** it MUST still fit the same high-level ability architecture through a different trigger mode

### Requirement: Effect payload components SHALL be treated as flow node semantics
`DamageEffectData`、`HealEffectData`、`ApplyBuffData`、`AreaSearchData`、`ProjectileData` 等 SHALL 被视为 flow node semantics。

#### Scenario: A composite ability uses projectile then ground area damage
- **WHEN** 某个技能先发射弹道，再生成地面持续效果
- **THEN** the architecture MUST allow multiple flow stages/nodes rather than forcing a single monolithic payload definition

### Requirement: Composite skills MUST be representable as multi-stage flows
复合技能 MUST 能以多阶段 flow 形式表达。

#### Scenario: Lava ball spawns a lingering lava field on arrival
- **WHEN** 某个技能需要“飞行命中 + 落地持续伤害”两段效果
- **THEN** the architecture MUST support modeling it as a multi-stage flow

### Requirement: Settlement logic SHOULD remain centralized
伤害、治疗、Buff、长期属性贡献等结算逻辑 SHOULD 保持在各自的 settlement layer，而不应被散落到触发/流程层。

#### Scenario: Target has special buff that alters incoming damage
- **WHEN** 某个效果节点产生伤害请求
- **THEN** the final damage adjustment SHOULD be handled by the settlement layer rather than the trigger layer alone

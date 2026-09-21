## ADDED Requirements

### Requirement: 无敌或免疫期间施加的控制被吸收

当目标处于无敌（`Invulnerable > 0`）或持有与所施加控制属性对应的免疫属性（如 `StunImmunity`）时，系统 SHALL 使该次控制贡献计 0，且 MUST NOT 在无敌/免疫解除后延迟生效。吸收判定 SHALL 只作用于控制属性，且 SHALL 通过单一判定入口完成，buff 写入路径与属性贡献写入路径行为一致。

#### Scenario: 无敌期间施加眩晕

- **WHEN** 目标持有 `Invulnerable` 且随后被施加 Stun
- **THEN** Stun 属性最终值为 0

#### Scenario: 无敌解除后不延迟生效

- **WHEN** 无敌期间施加的 Stun 仍在持续，随后 `Invulnerable` 归零
- **THEN** Stun 属性最终值仍为 0，目标不进入暂停

#### Scenario: 免疫期间施加眩晕

- **WHEN** 目标持有 `StunImmunity` 且随后被施加 Stun
- **THEN** Stun 属性最终值为 0，移除免疫后仍为 0

#### Scenario: 无无敌与免疫时不受影响

- **WHEN** 目标既无 `Invulnerable` 也无对应免疫属性时被施加 Stun
- **THEN** Stun 属性最终值按贡献量正常累加

#### Scenario: 非控制属性不受吸收影响

- **WHEN** 目标持有 `Invulnerable` 且被施加非控制属性（如 `Health`）的贡献
- **THEN** 该贡献按原值写入，不被降为 0

#### Scenario: 两条写入路径行为一致

- **WHEN** 分别通过 buff 路径与属性贡献路径在无敌期间施加同一控制属性
- **THEN** 两条路径产生的贡献均为 0

### Requirement: 已施加的控制不被回收

吸收判定 SHALL 只影响施加时刻；无敌或免疫生效前已存在的控制贡献 SHALL 保留原值与时长。无敌期间这些贡献 SHALL 继续通过读取压制不生效，并在无敌解除后按原语义恢复。

#### Scenario: 先眩晕后无敌

- **WHEN** 目标先被施加 Stun（最终值为正），随后获得 `Invulnerable`
- **THEN** Stun 属性最终值保持为正，但有效值为 0

#### Scenario: 无敌解除后恢复

- **WHEN** 上述 `Invulnerable` 归零且 Stun 仍在持续
- **THEN** Stun 有效值恢复为正，目标重新进入暂停

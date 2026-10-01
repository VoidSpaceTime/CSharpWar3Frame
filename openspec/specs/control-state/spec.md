# control-state Specification

## Purpose
定义无敌或免疫期间新施加的控制贡献在施加时刻被吸收、解除后不延迟生效，且此前已存在的控制不被回收的当前行为。

## Requirements

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

### Requirement: Pause 由控制状态合成驱动

系统 SHALL 把单位的原生暂停状态定义为 `raw(Pause) > 0` 或 `effective(Stun) > 0` 或 `effective(CrackFly) > 0` 的合成结果，任一为真即暂停，全部归零才恢复。合成判定 SHALL 每轮基于当前有效值重新计算，并与上一轮 Pause 快照位对比。

#### Scenario: 眩晕进入暂停

- **WHEN** 单位获得 Stun 且未被免疫压制
- **THEN** 系统产生一次 `ControlStateNativeRequest(Pause, entered: true)`，由 Native 层执行 `PauseUnit(true)`

#### Scenario: 多来源叠加与部分解除

- **WHEN** 单位同时受 Stun 与 CrackFly 影响，随后仅解除其中一项
- **THEN** 单位保持暂停，不产生新的 Pause 请求

#### Scenario: 全部解除恢复

- **WHEN** 作用于单位的 Stun / CrackFly / 纯 Pause 全部归零
- **THEN** 系统产生恰好一次 `ControlStateNativeRequest(Pause, entered: false)`

#### Scenario: 同帧多控制变化去抖

- **WHEN** 同一轮内多个控制状态发生跳变
- **THEN** 只产生一次 Pause 请求，不出现重复抖动

#### Scenario: 属性整体移除的释放收敛

- **WHEN** 单位的控制属性被整体移除且 Pause 快照位仍为激活
- **THEN** 系统补发 `ControlStateNativeRequest(Pause, entered: false)`，不遗留永久暂停

### Requirement: 纯 Pause 属性独立且不可免疫

系统 SHALL 注册独立的 `Pause` 属性以支持过场、镜头等纯暂停场景，且 MUST NOT 为其提供免疫属性；合成判定中纯 Pause 的贡献 SHALL 直接读取属性最终值，不经免疫压制。

#### Scenario: 纯 Pause 生效

- **WHEN** 单位仅持有 `Pause` 属性且值大于 0
- **THEN** 系统产生 `ControlStateNativeRequest(Pause, entered: true)`

#### Scenario: 免疫不影响纯 Pause

- **WHEN** 单位持有眩晕免疫（`StunImmunity`）并同时获得 `Pause` 属性
- **THEN** 单位仍进入暂停

### Requirement: Stun 与 CrackFly 不直接产生原生请求

Stun / CrackFly 的跳变 SHALL 继续广播 `ControlStateChangedEvent` 供业务层响应特效与位移，但 MUST NOT 各自产生 `ControlStateNativeRequest`；其原生暂停动作统一由 Pause 合成承担。

#### Scenario: 眩晕跳变的事件与请求分离

- **WHEN** 单位的 Stun 从 0 变为正
- **THEN** 产生一条 `ControlStateChangedEvent(Stun, entered: true)`，且不存在 `ControlStateNativeRequest(Stun, ...)`

#### Scenario: 免疫压制使 Stun 不暂停

- **WHEN** 单位持有效眩晕且随后获得 `StunImmunity`
- **THEN** Stun 有效值归零并广播解除事件，同时产生 `ControlStateNativeRequest(Pause, entered: false)`

### Requirement: Pause 快照位宽覆盖全部控制类型

`ControlStateSnapshot.bits` SHALL 能表示全部 `ControlType` 成员的位；纯 Pause 属性也应使持有单位进入控制状态检测范围，即使该单位没有任何眩晕或击飞属性。

#### Scenario: Pause 位可被记录与读取

- **WHEN** 合成结果使单位进入暂停
- **THEN** 单位快照的 Pause 位被置位且可被读回

#### Scenario: 仅持纯 Pause 的单位被纳入检测

- **WHEN** 单位只有 `Pause` 属性、没有 Stun / CrackFly / 免疫属性
- **THEN** 该单位仍被控制状态检测系统纳入并驱动暂停

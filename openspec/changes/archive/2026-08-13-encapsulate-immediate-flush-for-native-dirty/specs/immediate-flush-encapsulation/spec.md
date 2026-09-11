## ADDED Requirements

### Requirement: Immediate flush MUST be exposed through a Game-level helper
系统 MUST 通过 `Game` 层的统一入口触发 immediate system flush，而不是让业务代码直接调用 `ImmediateRoot.Update(...)`。

#### Scenario: Business logic needs to flush immediate systems
- **WHEN** 某段业务逻辑需要在当前时刻立即执行 `ImmediateRoot` 下的系统
- **THEN** 该逻辑 MUST 调用 `Game` 提供的统一 flush 封装，而不是直接访问 `ImmediateRoot.Update(...)`

### Requirement: Native dirty helper MUST support immediate-mark semantics
原生脏位辅助层 MUST 提供一个立即版标记入口，用于表达“写入 dirty 后立即 flush immediate systems”的语义。

#### Scenario: Immediate lifecycle action marks a dirty flag
- **WHEN** 某个 native 生命周期动作需要在标记 dirty 后立即落地
- **THEN** 调用方 MUST 可以通过统一 helper 完成“标记 + immediate flush”，而无需手写分散的两步调用

### Requirement: Regular dirty marking MUST remain non-immediate
普通 dirty 标记语义 MUST 保持为“只标记，不立即 flush”，以避免 interval 同步与 immediate 生命周期语义混淆。

#### Scenario: Interval sync marks health or mana dirty
- **WHEN** interval system 因数值变化标记 `Health` 或 `Mana` 等普通 native dirty
- **THEN** 系统 MUST 仅写入 dirty 状态，而 SHALL NOT 自动触发 immediate system flush

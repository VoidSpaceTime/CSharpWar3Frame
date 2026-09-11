# system-register-immediate-contract 规格

## ADDED Requirements

### Requirement: SystemGenerator MUST resolve enum member names correctly

`SystemGenerator` SHALL 从 `SystemRegisterAttribute` 的第一个构造参数中解析出正确的枚举成员名（`Interval` / `Immediate`），而不是枚举的底层整数值字符串。

#### Scenario: Immediate system registration

- **WHEN** 一个类标注 `[SystemRegister(SystemKind.Immediate, order)]`
- **THEN** 生成器 MUST 将该类解析为 `"Immediate"` 成员名
- **AND** 生成代码 MUST 通过 `ImmediateRoot.Add(new X())` 注册该类型

#### Scenario: Interval system registration

- **WHEN** 一个类标注 `[SystemRegister(SystemKind.Interval, order)]`
- **THEN** 生成器 MUST 将该类解析为 `"Interval"` 成员名
- **AND** 生成代码 MUST 通过 `Root.Add(new X())` 注册该类型

### Requirement: Game MUST expose ImmediateRoot and a flush entry

`Game` SHALL 提供 `ImmediateRoot`（friflo `SystemRoot`）用于承接所有 Immediate 系统，并 SHALL 提供 `FlushImmediateSystems()` 作为统一刷新入口。

#### Scenario: ECS initialization creates ImmediateRoot

- **WHEN** 调用 `Game.ECSInit()`
- **THEN** `Game.ImmediateRoot` MUST 被初始化为非空 `SystemRoot`
- **AND** `RegisterGeneratedSystems()` MUST 在其初始化之后执行

#### Scenario: Flush entry

- **WHEN** 业务或主循环需要立即执行 Immediate 系统
- **THEN** 调用方 MUST 使用 `Game.FlushImmediateSystems()` 而非直接访问 `ImmediateRoot.Update(...)`

### Requirement: Immediate systems MUST be flushed on the main loop

War3 主循环 SHALL 在每个 tick 中刷新 `ImmediateRoot` 下的系统，确保立即系统在 request 产生后及时消费。

#### Scenario: Main timer callback flushes immediate systems

- **WHEN** 主计时器回调推进一次游戏 tick
- **THEN** 该回调 MUST 依次执行 `Root.Update(tick)` 与 `FlushImmediateSystems()`
- **AND** Immediate 系统 MUST NOT 因缺少 flush 调用而完全不执行

### Requirement: Interval system behavior MUST remain unchanged

`SystemKind.Interval` 系统的注册目标与调度语义 SHALL 保持与修复前一致。

#### Scenario: Interval systems still use Root

- **WHEN** 一个类标注 `[SystemRegister(SystemKind.Interval, order)]`
- **THEN** 该类型 MUST 继续注册到 `TimedSystemRoot`（`Game.Root`）
- **AND** 其 `ITimedSystem.Interval` 调度逻辑 MUST 不受本 change 影响

## ADDED Requirements

### Requirement: Test managed entry diagnosis MUST be staged
`Projects/test/Program.cs` 的诊断入口 MUST 采用 staged bootstrap，而 SHALL NOT 在第一阶段一次性执行所有 native startup 逻辑。

#### Scenario: Managed entry is reached successfully
- **WHEN** 模块成功进入托管侧
- **THEN** the first stage MUST be able to prove entry success without relying on high-risk native startup actions

### Requirement: Minimal managed entry MUST be validated before risky native calls
最小托管入口 MUST 先被验证，之后才允许逐步恢复高风险 native 调用。

#### Scenario: Crash still occurs before Stage 1 completes
- **WHEN** 崩溃发生在最小托管入口阶段之前
- **THEN** diagnosis MUST prioritize module-load/host issues over business logic

### Requirement: Native startup restoration SHOULD be incremental
高风险 native 调用 SHOULD 以增量方式恢复。

#### Scenario: Crash appears only after object creation is reintroduced
- **WHEN** 某阶段恢复到对象创建后才出现崩溃
- **THEN** diagnosis SHOULD focus on that stage’s native interaction rather than the earlier load path

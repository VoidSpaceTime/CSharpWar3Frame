## ADDED Requirements

### Requirement: 顶替只结束旧移动命令

当单位已有某个移动命令的执行状态，而当前 `MoveCommand` 的 token 与之不同时，移动系统 SHALL 以**旧**执行状态的 token 发出 `Overridden` 结果，并 SHALL 只移除旧执行状态。新 `MoveCommand` MUST 保留，并在同一轮继续进行到达与下发判定。

#### Scenario: 原生单位移动中收到新移动命令

- **WHEN** 带原生句柄的单位已下发 token 为 1 的移动，随后写入 token 为 2 的新 `MoveCommand`
- **THEN** 发出的 `Overridden` 结果 token 为 1
- **AND** token 为 2 的 `MoveCommand` 仍存在，并产生 token 为 2 的原生移动请求

### Requirement: 移动结果按 token 对齐消费

`MoveContinuation` SHALL 记录发起它的移动命令 token。施法与任务的移动后续 MUST 只消费 token 相同的 `MoveOutcome`；token 不同的结果 SHALL 被丢弃，后续继续等待。token 为 0 的后续 SHALL 不做校验。

#### Scenario: 遗留到达结果不会触发新施法

- **WHEN** 单位身上残留一个旧的 `Arrived` 结果，随后对超出施法距离的点发起施法
- **THEN** 单位进入 `MovingToCast`，而不是直接进入 `Casting`

### Requirement: 取消走位施法清理全部移动残留

走位施法被取消（受控、外部打断、移动失败、施法者死亡或被新施法替换）时，系统 SHALL 移除该技能的 `MoveContinuation`、该次 `MoveOutcome`，以及原因是施法的 `MoveCommand`、`MoveExecutionState` 与 `MovingTag`。

#### Scenario: 打断后可以再次走位施法

- **WHEN** 单位在 `MovingToCast` 期间被眩晕而取消施法，眩晕结束后再次对远处目标施法
- **THEN** 第二次施法进入 `MovingToCast`，并在到达后进入 `Casting`

#### Scenario: 走位移动被其他命令替换

- **WHEN** 单位在 `MovingToCast` 期间，其施法移动命令被另一个 token 的 `MoveCommand` 替换，或施法后续被移动任务替换
- **THEN** 施法被取消、技能回到 `Ready`，单位不残留 `MovingToCast`
- **AND** 替换方的移动命令与后续保留

### Requirement: 新施法请求结束当前施法

单位已有施法状态时，若新 `CastRequest` 通过全部校验，系统 SHALL 先结束当前施法再开始新施法：走位阶段按取消处理；前摇、引导、后摇阶段按打断处理并触发 `OnInterrupted`。校验未通过的请求 MUST NOT 影响当前施法。

#### Scenario: 前摇中改放另一技能

- **WHEN** 单位正在吟唱技能 A，收到对就绪技能 B 的有效请求
- **THEN** 技能 A 回到 `Ready`，单位进入技能 B 的施法

#### Scenario: 无效请求不打断

- **WHEN** 单位正在吟唱技能 A，收到对冷却中技能 B 的请求
- **THEN** 技能 A 的施法不受影响，请求被移除

### Requirement: 效果已提交后的打断进入冷却

施法被打断时，若效果已经提交（引导阶段或后摇阶段），技能 SHALL 按其冷却值进入冷却，冷却为 0 时回到 `Ready`。前摇阶段效果尚未提交时，打断 SHALL 让技能回到 `Ready` 且不进入冷却。

#### Scenario: 引导被沉默打断

- **WHEN** 冷却为 10 秒的引导技能在引导中被沉默
- **THEN** 技能进入 `Cooldown`，剩余冷却为 10 秒

#### Scenario: 前摇被眩晕打断

- **WHEN** 技能在前摇阶段被眩晕打断
- **THEN** 技能回到 `Ready`，且没有冷却状态

### Requirement: 死亡单位不能施法

拥有生命周期且已死亡的单位，其新的 `CastRequest` SHALL 被拒绝；正在进行的前摇、引导、后摇或走位施法 MUST 在下一次推进时被打断或取消。

#### Scenario: 前摇中死亡

- **WHEN** 单位在前摇阶段死亡
- **THEN** 前摇不提交效果，不扣除消耗，技能回到 `Ready`

### Requirement: 施法中移除技能清理施法状态

技能在被施放期间从单位移除时，系统 SHALL 在解除归属前清理引用该技能的请求、施法、引导与走位状态。技能实体被删除后，施法推进系统 MUST NOT 读取该实体的组件。

#### Scenario: 前摇中销毁技能

- **WHEN** 单位吟唱技能期间该技能被移除并销毁
- **THEN** 单位不再有该技能的施法状态，后续推进不抛异常

### Requirement: 范围内施法使用解析后的目标坐标

对单位目标施法且已在施法距离内时，施法状态记录的目标坐标 SHALL 等于目标单位当前位置，而不是请求里原样携带的坐标。

#### Scenario: 请求坐标为零

- **WHEN** 请求携带 `targetUnit` 且坐标为 (0,0)，目标位于 (100,50) 且在施法距离内
- **THEN** 施法状态的目标坐标为 (100,50)

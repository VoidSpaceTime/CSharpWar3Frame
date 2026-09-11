## ADDED Requirements

### Requirement: Unsupported Warcraft versions MUST fail closed before address resolution
当当前 Warcraft 版本无法映射到受支持的偏移表时，地址解析 MUST 显式失败，而 SHALL NOT 退化为 `game.dll + 0` 之类的伪有效地址。

#### Scenario: Version detection returns an unsupported build
- **WHEN** `TypeVersion` 无法识别当前 `game.dll` 版本
- **THEN** 后续 Jass 相关地址解析 MUST 停止并提供明确诊断

### Requirement: Current JassVM access MUST be validated before register reads
任何依赖当前 JassVM 的寄存器访问 MUST 先验证 VM 存在且可用，而 SHALL NOT 在空 VM 或无效 VM 状态下继续解引用。

#### Scenario: Native callback is invoked without an active current VM
- **WHEN** callback 分发路径请求当前 JassVM，但当前线程没有有效 VM
- **THEN** 实现 MUST 安全失败并停止该 callback 路径

### Requirement: Native callback dispatch MUST validate delegate indices
native callback dispatch MUST 校验寄存器数据类型、delegate index 范围与目标 delegate 是否存在，而 SHALL NOT 直接按未校验索引执行回调。

#### Scenario: Callback register payload is invalid
- **WHEN** callback 分发读取到非法 index、非法类型或不存在的 delegate
- **THEN** dispatch MUST 记录诊断并中止该次分发

### Requirement: Callback safety MUST remain aligned across source and generated callbacks
checked-in callback 与 Build 生成 callback 模板 MUST 保持一致的安全语义，避免重新生成后回退到旧的不安全路径。

#### Scenario: FrameBuild regenerates the callback file for Build-mode launch
- **WHEN** callback 文件被复制或重写到 `.temp/Build/<project>/map`
- **THEN** 生成结果 MUST 保持与源 callback 相同的 fail-closed 预期

## ADDED Requirements

### Requirement: Release publish MUST align with the actual runtime module model
Release 发布链 MUST 与项目的实际运行时模块模型保持一致，SHALL NOT 同时混用 AOT publish 假设与 DNNE/JIT 模块假设。

#### Scenario: Release run invokes publish for a DNNE-based project
- **WHEN** Release 运行路径对一个仍依赖 DNNE/JIT 模块模型的项目执行发布
- **THEN** 发布策略 MUST NOT 强行套用纯 AOT 假设

### Requirement: `Run.cs` publish strategy MUST be model-aware
`FrameBuild/CommandManager/Run.cs` MUST 依据目标项目的模块模型决定发布策略。

#### Scenario: Release mode runs a project with DNNE-specific packaging requirements
- **WHEN** Release 模式执行一个含有 DNNE 约束的项目
- **THEN** `Run.cs` MUST use a publish/build strategy compatible with that model

### Requirement: Callback module naming MUST match the generated module exactly
callback 写入的 `ModuleName` / `ModulePath` MUST 与最终实际生成并被运行时加载的模块严格一致。

#### Scenario: Runtime callback is generated for module loading
- **WHEN** callback 文件被写入地图输出目录
- **THEN** callback MUST point to the exact generated module artifact, not a mode-assumed placeholder name

### Requirement: Environment/toolchain issues SHALL NOT hide model mismatches
环境依赖问题 MAY 阻断构建，但 SHALL NOT 被用来掩盖产物模型设计不一致的问题。

#### Scenario: Publish fails due to missing Win10 SDK
- **WHEN** 发布流程因原生工具链缺失失败
- **THEN** the diagnosis MUST still determine whether the chosen publish path matched the project’s intended runtime module model

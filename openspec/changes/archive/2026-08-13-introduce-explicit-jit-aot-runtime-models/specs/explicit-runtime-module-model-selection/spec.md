## ADDED Requirements

### Requirement: Runtime module model MUST be explicit
仓库 MUST 对运行时模块模型进行显式建模，至少覆盖 `Jit` 与 `Aot`。

#### Scenario: A project supports multiple runtime models
- **WHEN** 某个项目既可能走 JIT/DNNE，也可能走 AOT
- **THEN** the selected runtime module model MUST be explicit rather than inferred from BuildMode alone

### Requirement: BuildMode MUST NOT implicitly define runtime module model
`BuildMode` SHALL NOT 继续隐式承担 runtime module model 的职责。

#### Scenario: Release mode is selected
- **WHEN** 构建模式为 `Release`
- **THEN** runtime module model MUST still be selected independently

### Requirement: A single publish path SHALL use exactly one runtime module model
一次发布链 MUST 只使用一种 runtime module model，SHALL NOT 混用 AOT 与 DNNE/JIT 假设。

#### Scenario: Release path publishes a DNNE-based project
- **WHEN** Release 路径对 DNNE/JIT 项目进行发布
- **THEN** the publish branch MUST NOT also impose AOT assumptions

### Requirement: Callback module naming MUST be runtime-model-aware
callback 的 `ModuleName` 与 `ModulePath` MUST 与所选 runtime module model 严格一致。

#### Scenario: Callback is generated for JIT runtime model
- **WHEN** callback 针对 JIT/DNNE 模型生成
- **THEN** callback MUST point to the JIT module artifact expected by that model

#### Scenario: Callback is generated for AOT runtime model
- **WHEN** callback 针对 AOT 模型生成
- **THEN** callback MUST point to the AOT artifact expected by that model

### Requirement: `Run.cs` publish strategy MUST be runtime-model-aware
`FrameBuild/CommandManager/Run.cs` MUST 根据 runtime module model 选择发布策略。

#### Scenario: AOT model is selected
- **WHEN** runtime module model 为 `Aot`
- **THEN** `PublishAot=true` MAY be used

#### Scenario: JIT model is selected
- **WHEN** runtime module model 为 `Jit`
- **THEN** `PublishAot=true` SHALL NOT be imposed by the publish branch

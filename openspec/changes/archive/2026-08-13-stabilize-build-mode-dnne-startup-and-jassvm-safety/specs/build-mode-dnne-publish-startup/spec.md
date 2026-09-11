## ADDED Requirements

### Requirement: Build-mode publish MUST drain process output and observe process completion
Build 模式下的 `dotnet publish` 执行 MUST 持续消费重定向的标准输出与标准错误，并显式处理退出结果，而 SHALL NOT 在未消费输出流的情况下直接无限等待进程结束。

#### Scenario: Build-mode publish emits verbose output
- **WHEN** `FrameBuild` 在 Build 模式对 DNNE 项目执行 `dotnet publish`
- **THEN** publish orchestration MUST 启动输出读取并给出明确的完成、失败或超时诊断

### Requirement: Build-mode staging SHALL NOT race publish into the same output tree
Build 模式下的地图 staging 与 publish SHALL NOT 对同一个 `BuildDstPath/map` 输出树进行未受保护的并发写入。

#### Scenario: Build mode prepares map files and publish output
- **WHEN** `BuildMap()` 与 `PublishProject()` 都需要写入 `.temp/Build/<project>/map`
- **THEN** 实现 MUST 通过串行化或隔离输出路径来避免竞争写入

### Requirement: Build-mode DNNE runtime payload MUST be staged side-by-side
Build 模式运行目录 MUST 包含 DNNE/JIT 启动所需的 canonical native shim、managed assembly、`runtimeconfig.json`、`deps.json` 与必要依赖文件。

#### Scenario: Fresh Build-mode output is prepared for map launch
- **WHEN** Build 模式完成 `test` 项目的 staging
- **THEN** `.temp/Build/test/map` MUST 具备可供 DNNE native hosting 使用的 side-by-side payload

### Requirement: Build-mode startup contract MUST expose one canonical export
Build 模式运行时模块 MUST 暴露一个唯一的 canonical export contract，仓内验证器与 callback/module 约定 MUST 与该契约严格一致。

#### Scenario: Repository loader verifies the Build-mode DNNE module
- **WHEN** `Projects/TestDnne` 或 Build-mode callback 对模块进行探测/调用
- **THEN** 它们 MUST 使用与实际导出一致的符号名与调用约定，而不是并存多套推测规则

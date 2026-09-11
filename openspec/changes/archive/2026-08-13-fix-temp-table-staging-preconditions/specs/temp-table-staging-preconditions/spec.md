## ADDED Requirements

### Requirement: Temp table staging MUST establish preconditions before refresh
当 `SyncW3xFile.cs` 把项目 `table` 资源同步到 `.temp/<project>` 时，实现在任何时间戳比较、删除或复制动作之前，MUST 先保证 `.temp/<project>/table` 已存在或可被安全创建，而 SHALL NOT 直接假设该目录已经存在。

#### Scenario: Fresh temp bootstrap is missing the table directory
- **GIVEN** `.temp/test` 只有 `.w3x`、`map/`、`w3x2lni/`，还没有 `table/`
- **AND** `Projects/test/w3x/table` 已存在并提供 `misc.ini`、`w3i.ini`
- **WHEN** `ProjectResourceToTemp()` 执行 table staging
- **THEN** 流程 MUST 先补齐 temp table 前置条件
- **AND** 流程 SHALL NOT 因缺失 `.temp/test/table` 抛出 `DirectoryNotFoundException`

### Requirement: Temp table staging MUST use a flat target layout
项目 `table` 资源同步到 temp 目录时，目标布局 MUST 是 flat `.temp/<project>/table`，其中 `misc.ini`、`w3i.ini` 等文件直接位于该目录下；实现 SHALL NOT 生成额外的 `.temp/<project>/table/table` 嵌套层。

#### Scenario: Project table content is copied into temp staging
- **WHEN** `Projects/<project>/w3x/table` 被同步到 temp staging
- **THEN** `.temp/<project>/table/misc.ini` MUST 存在
- **AND** `.temp/<project>/table/w3i.ini` MUST 存在
- **AND** `.temp/<project>/table/table` SHALL NOT 成为 table 内容的实际落点

### Requirement: Downstream build consumers MUST keep seeing `BuildDstPath/table/w3i.ini`
当 `Run.cs` 把 `TempProjectBuildPath` 复制到 `BuildDstPath` 后，downstream build consumer MUST 继续通过 `BuildDstPath/table/w3i.ini` 访问 table 数据，而不需要额外路径改写、fallback 或 consumer 侧补丁。

#### Scenario: Build staging inherits the temp table layout
- **GIVEN** temp staging 已经形成 flat `.temp/<project>/table`
- **WHEN** `BuildMap()` 把 `TempProjectBuildPath` 复制到 `.temp/<mode>/<project>`
- **THEN** `.temp/<mode>/<project>/table/w3i.ini` MUST 存在
- **AND** `AssetsBuild.cs` 这类 downstream consumer 仍可按 `BuildDstPath/table/w3i.ini` 读取

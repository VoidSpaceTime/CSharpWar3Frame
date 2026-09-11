## ADDED Requirements

### Requirement: Callback loader MUST leave stage markers in War3 diagnosis mode
`Projects/test/w3x/map/callback` 在执行 native loader 诊断时 MUST 通过目录副作用区分至少三个阶段：callback 入口已执行、`LoadLibraryA` 成功、`GetProcAddress("main")` 成功。

#### Scenario: Callback starts but DLL load fails
- **WHEN** War3 执行 callback `main`
- **THEN** callback MUST 至少留下“入口已执行”的 marker
- **AND** SHALL NOT 伪造后续阶段 marker

#### Scenario: DLL load succeeds but export lookup fails
- **WHEN** `LoadLibraryA(ModuleName)` 成功但 `GetProcAddress(moduleHandle, "main")` 失败
- **THEN** callback MUST 留下“入口已执行”和“LoadLibrary 成功”的 marker
- **AND** SHALL NOT 留下“GetProcAddress 成功”的 marker

#### Scenario: Export lookup succeeds
- **WHEN** callback 成功解析导出 `main`
- **THEN** callback MUST 留下前三个阶段 marker，以证明 native loader 已完成到导出解析层

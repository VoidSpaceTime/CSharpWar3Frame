## ADDED Requirements

### Requirement: Repository MUST generate a minimal DNNE smoke DLL for host diagnosis
仓库 MUST 能生成一个最小 DNNE smoke DLL，其导出 `main` 在进入托管后不执行任何业务逻辑并直接返回，用于隔离验证 War3 宿主与 DNNE/bootstrap 的兼容性。

#### Scenario: Smoke DLL is built for manual replacement testing
- **WHEN** 诊断项目被构建/发布
- **THEN** 输出 MUST 包含 native shim DLL、对应托管 DLL、`runtimeconfig.json` 与 `deps.json`
- **AND** native shim MUST 导出 `main`

#### Scenario: Smoke DLL is used to isolate host compatibility
- **WHEN** 用户用 smoke DLL 替换当前 `testNE.dll` 及其配套文件进行 War3 宿主实验
- **THEN** 该实验 SHALL NOT 引入 `War3Frame`、`Friflo` 或其他业务逻辑变量

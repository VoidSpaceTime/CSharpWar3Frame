## ADDED Requirements

### Requirement: Build/test MUST use a dedicated native bridge instead of DNNE-generated `testNE.dll`
`Projects/test` 的 Build/test 路线 MUST 通过一个独立的 native bridge 生成 `testNE.dll`，而 SHALL NOT 继续依赖 DNNE 在 `obj/.../dnne/bin` 生成的 native shim。

#### Scenario: Build/test stages its runtime payload
- **WHEN** `FrameBuild` 对 `test` 执行 Build/test 路线
- **THEN** staged payload MUST 包含由 bridge 产出的 `testNE.dll`
- **AND** SHALL NOT 依赖从 DNNE obj 路径复制 `testNE.dll`

### Requirement: The native bridge MUST load the managed test payload from the same directory without `ProjectReference`
native bridge MUST 在运行时从自身目录加载 `test.dll`，并调用固定的 managed entry；它 SHALL NOT 对 `Projects/test` 建立编译期 `ProjectReference`。

#### Scenario: Bridge is built and staged with the managed payload
- **WHEN** bridge DLL 与 `test.dll` 同目录部署
- **THEN** bridge MUST 能根据自身模块路径定位 `test.dll`
- **AND** its project file MUST NOT reference `Projects/test/test.csproj`

### Requirement: `Projects/test` MUST become a pure managed payload in Build/test mode
`Projects/test` 在 Build/test 路线 MUST 作为纯托管负载发布，不再承担 native export owner 职责。

#### Scenario: Test project publishes for Build/test
- **WHEN** `Projects/test/test.csproj` 被发布到 staged payload
- **THEN** 输出 MUST 包含 `test.dll`, `test.runtimeconfig.json`, `test.deps.json`
- **AND** the active test route SHALL NOT require `DNNE` package configuration to function

### Requirement: Callback contract MUST remain stable during the migration
迁移后 `callback` 仍 MUST 通过 `testNE.dll` 的 export `main` 进入 native bridge，以避免同时改变脚本层与 native 层契约。

#### Scenario: Warcraft III invokes the built test route
- **WHEN** map callback 运行于 Build/test payload
- **THEN** it MUST still resolve `main` from `testNE.dll`
- **AND** the underlying owner of that DLL MAY change from DNNE to the new bridge project

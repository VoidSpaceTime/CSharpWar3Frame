## Why

当前 `Build/test` 路径下，War3 真实宿主已经能走到 callback 的 `LoadLibraryA` 与 `GetProcAddress("main")`，但一旦真正调用 `main` 就会立即闪退。为了把问题从“当前复杂程序集/依赖链”与“DNNE/宿主 bootstrap 自身问题”中隔离出来，需要一个最小 DNNE smoke DLL：

- 仍然导出 `main`
- 不引用 `War3Frame`
- 不引用 `Friflo`
- 不启控制台
- 不调用任何 native/JASS API
- 仅在托管入口直接返回

如果这个最小 DLL 也在 War3 宿主里同样闪退，就强烈说明问题在 DNNE/bootstrap/宿主兼容层；如果它能正常返回，则说明问题在当前 `test` 程序集或其依赖链。

## What Changes

- 新增一个最小 `Projects/dnne_smoke` 项目。
- 使用与当前 `Projects/test` 相同的 DNNE 发布链生成 native shim DLL。
- 导出 `main`，其托管实现只返回 `0`。
- 产出一组可直接拿去替换测试的文件路径，供用户手动替换 `testNE.dll` 及其配套托管文件。

## Scope Boundaries

- 不修改现有 `Projects/test` 逻辑。
- 不修改 `War3Frame`、`Friflo` 或 callback 逻辑。
- 不把 smoke 项目扩展成长期功能；它仅用于宿主兼容性诊断。

## Capability

### New Capability
- `minimal-dnne-smoke-dll-for-war3-host-diagnosis`: 仓库能够生成一个最小 DNNE native shim 与对应托管程序集，用于隔离验证 War3 宿主对 DNNE/bootstrap 的兼容性。

## Impact

- 直接影响 `Projects/` 下新增的 smoke 诊断项目。
- 间接提升 War3 宿主排障效率。
- 不影响现有 `test` 或 Release/AOT 路线。

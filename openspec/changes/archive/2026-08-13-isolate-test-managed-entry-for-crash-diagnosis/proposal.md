## Why

当前 `Build/test` 模式下，打包产物和 callback 链路已经基本对齐：`testNE.dll`、`test.dll`、`test.runtimeconfig.json`、`test.deps.json` 与 `War3Frame.dll` 都已在 `.temp/Build/test/map` 中，callback 也已经指向 `testNE.dll` 并查找导出入口 `main`。

在这种情况下，继续只围绕打包/命名排查的价值已经下降。当前更高价值的问题是：

- 是 `testNE.dll` / hostfxr / CLR 在进入托管运行时前失败？
- 还是已经进入 `MainJIT()`，但 `Projects/test/Program.cs` 中非常早期的原生调用把 Warcraft 打崩？

最小、最稳、最有区分度的下一步，不是继续扩展打包逻辑，而是把 `Projects/test/Program.cs` 改造成一个**分阶段托管入口验证器**。

## What Changes

- 将 `Projects/test/Program.cs` 设计成分阶段的最小托管入口验证流程。
- 先验证“是否成功进入托管侧”，再逐步恢复高风险 native 调用。
- 用 staged bootstrap 的方式区分：加载失败 vs 托管入口崩溃。

## Capabilities

### New Capabilities
- `staged-managed-entry-crash-diagnosis`: 定义 test 项目的分阶段托管入口验证策略。

## Impact

- 直接影响 `Projects/test/Program.cs` 的启动流程。
- 间接影响 `FrameBuild` / callback / test map 的故障诊断效率。
- 本次变更仅新增 OpenSpec 工件，不进入运行时代码修改。

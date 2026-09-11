## Why

当前发布链的根因问题不只是“缺少 Win10 SDK”，而是仓库缺少一个显式的 runtime module model。`BuildMode` 目前同时承担了“构建形态”和“模块产物形态”的隐含职责，导致：

- `Run.cs` 在 Release 模式下默认假定 AOT publish
- `Projects/test` / `Projects/demo` 本体却仍然是 DNNE/JIT 导出模块模型
- callback 又根据 BuildMode 直接推断模块名

这使得 JIT 与 AOT 两种模型在同一条发布分支里被混用，最终在构建和运行时两边都产生歧义。

## What Changes

- 引入显式的 runtime module model（JIT / AOT）。
- 规定一次发布必须只选择一种模块模型。
- 规定 callback/module naming 必须绑定 runtime module model，而不是粗暴绑定 BuildMode。
- 规定 `Run.cs` 的发布策略必须是 model-aware。

## Capabilities

### New Capabilities
- `explicit-runtime-module-model-selection`: 定义 JIT/AOT 双模式并存时的显式选择与产物一致性规则。

## Impact

- 直接影响 `FrameBuild/CommandManager/Run.cs` 的发布分支设计。
- 直接影响 callback 生成逻辑与模块命名约定。
- 直接影响 `Projects/test` / `Projects/demo` 的发布路径与后续实现策略。
- 本次变更仅新增 OpenSpec 工件，不进入代码实现。

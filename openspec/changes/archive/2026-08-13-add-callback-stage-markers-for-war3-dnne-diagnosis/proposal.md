## Why

当前 `Build/test` 路径下，已经确认 `.temp/Build/test/map/callback` 会尝试加载 `testNE.dll` 并解析导出 `main`，但用户在真实 War3 宿主里仍无法稳定判断失败停在：

- callback 是否真的被执行
- `LoadLibraryA(ModuleName)` 是否成功
- `GetProcAddress(moduleHandle, "main")` 是否成功

现有 callback 没有任何分阶段副作用，失败时只会直接 `return`，导致 War3 实跑时无法区分 native loader 的具体卡点。

## What Changes

- 仅修改 `Projects/test/w3x/map/callback`。
- 在 callback `main` 中加入最小目录级 marker：
  - callback 入口已执行
  - `LoadLibraryA` 成功
  - `GetProcAddress("main")` 成功
- 不修改模板 callback。
- 不修改 C# 运行时代码、FrameBuild、DNNE 配置或 Friflo 相关逻辑。

## Scope Boundaries

- 本提案只为 War3 真实宿主诊断增加原生侧可观测副作用。
- 不尝试修复托管侧 `EntityStore` / Friflo 崩溃。
- 不把目录 marker 升级成文件写入；优先复用 callback 现有 `CreateDirectoryA` 能力。

## Capabilities

### New Capability
- `callback-stage-markers-for-war3-dnne-diagnosis`: 让 `Projects/test/w3x/map/callback` 在 War3 宿主里按阶段留下目录 marker，用于判断 DLL 调用链停在哪一层。

## Impact

- 直接影响 `Projects/test/w3x/map/callback` 的 native loader 诊断行为。
- 间接影响 `.temp/Build/test/map/callback` 的生成结果与 War3 侧排障效率。
- 不影响 Release/AOT 路线，不影响模板 callback。

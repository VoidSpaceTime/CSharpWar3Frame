## 0. 基本信息

- Change ID: `replace-test-dnne-route-with-cppcli-bridge`
- 提案等级: `architecture`
- 目标一句话: 将 `Projects/test` 的 Build/JIT 路线从 DNNE native shim 迁移到无 `ProjectReference` 的 C++/CLI 薄桥，并清理 test 路线中的 DNNE 依赖与构建路径。
- 请求来源: 用户明确要求 1) 实现一个按同目录加载 managed `project.dll` 并调用固定入口的 C++/CLI 项目；2) 清理测试项目中所有 DNNE 相关内容。

### 0.1 工件矩阵

- `proposal.md`
- `design.md`
- `tasks.md`
- `specs/test-build-native-bridge/spec.md`

## 1. 分级判定

### 1.1 为什么是 architecture 级

- 影响范围覆盖 `Projects/test`、新建原生桥接项目、`FrameBuild/`、solution wiring、Build/test staged payload 结构与验证器。
- 这不是单个项目内的局部实现，而是 **native/managed 边界与构建发布模型** 的替换。
- 需要显式约束非目标：不扩展到 `demo`、不改 Release/AOT 决策、不继续沿用 test 路线 DNNE 假设。

### 1.2 升级触发器检查

- [x] 涉及 `Projects/` 下现有项目与新增项目协同
- [x] 涉及 `FrameBuild/` 的发布/拷贝/验证逻辑
- [x] 涉及 solution 配置与构建图
- [x] 涉及 callback 的 native module contract
- [ ] 涉及 `War3Frame.Generator/` 输出或契约
- [ ] 涉及 Release/AOT 路线调整（显式排除）

## 2. 背景 / Why

当前 `Build/test` 路线已经证明：

1. `callback` 能执行到 `LoadLibraryA(testNE.dll)` 与 `GetProcAddress("main")`。
2. 但无论是完整 `test` 负载还是最小 `dnne_smoke` 负载，只要真正调用进 DNNE 导出的 `main`，War3 宿主都会立即闪退。
3. 删除 `testNE.dll / test.dll / test.runtimeconfig.json / test.deps.json` 后，War3 仍能正常进图，说明真正的崩点在 **DNNE / runtime bootstrap**，而不是 map 打包、callback 存在性或托管业务代码。

与此同时，本地对照项目 `D:/Downloads/CSharpWar3Map` 已证明另一条路径可以跑通：

- `callback -> LoadLibrary -> exported native main -> C++/CLI wrapper -> managed MainCLR()`

因此本提案选择用一个 **x86 C++/CLI 薄桥** 替换 `Projects/test` 当前的 DNNE native shim，同时保留 `callback -> testNE.dll -> export main` 这条外层契约不变。

## 3. 变更范围 / What

### 3.1 本提案要做的事

- 新增一个 `Projects/test.bridge/`（名称可在实施前微调）的 C++/CLI x86 动态库项目。
- 该桥接项目：
  - 导出稳定的 native `main`
  - 不对 `Projects/test` 建立 `ProjectReference`
  - 运行时从自身目录按路径加载 `test.dll`
  - 调用托管侧一个固定、桥接可调用的入口方法
- 将 `Projects/test` 从“拥有 native export 的 DNNE 项目”调整为“纯托管负载项目”。
- 调整 `FrameBuild` 的 Build/test 发布逻辑，不再从 `obj/.../dnne/bin` 复制 `testNE.dll`，改为发布/复制 bridge + managed payload。
- 清理 test 路线中的 DNNE 配置、生成物假设与验证器措辞。

### 3.2 本提案明确不做的事

- 不修改 `Projects/demo`。
- 不把 Release/AOT 路线一起迁移到 C++/CLI。
- 不解决与 native bridge 无关的 `Friflo` / `EntityStore` 业务崩溃。
- 不继续投入 `Projects/dnne_smoke` 作为生产路径；它只保留为历史诊断痕迹，是否后续删除另案处理。

## 4. 全局影响分析

- `Projects/test`：从 DNNE export owner 变成 bridge-callable managed payload。
- 新 bridge 项目：成为 Build/test 路线新的 native module owner，生成 `testNE.dll`。
- `FrameBuild/CommandManager/Run.cs`：发布与 staged payload 验证逻辑将从 DNNE copy path 转为 bridge staging path。
- `callback`：保留对 `testNE.dll` 的外层契约，但不再假设该 DLL 来自 DNNE。
- `Projects/TestDnne`：若继续保留，需适配并验证新的 `testNE.dll` 来源与行为。
- solution：新增 `.vcxproj`，但不要求把 `Projects/TestDnne` 或 `dnne_smoke` 纳入 solution。

## 5. 关键设计决策

### 5.1 保持外层 native contract 不变

`callback` 与测试验证器已经围绕 `testNE.dll` + export `main` 建立了固定契约。迁移时不改这个外层协议，避免同时引入 callback 命名变更。

### 5.2 桥接项目不使用 `ProjectReference`

桥接项目必须以“薄桥”方式存在：

- 不在编译期引用 `Projects/test/test.csproj`
- 不直接写死 `War3Frame.Game.MainCLR()` 这类跨项目强类型调用
- 运行时按同目录定位 `test.dll`，再通过反射或等价机制定位固定托管入口

### 5.3 仍保留同目录 payload 模型

Build/test 输出目录继续以 `.temp/Build/test/map` 为 runtime payload 根，桥接和托管文件同目录放置。验收时必须继续具备：

- `testNE.dll`
- `test.dll`
- `test.runtimeconfig.json`
- `test.deps.json`
- `War3Frame.dll` 与其依赖

### 5.4 test.csproj 不再拥有 native 导出职责

`Projects/test/Program.cs` 当前的 `[UnmanagedCallersOnly]` 导出不再是 test 路线的生产入口。test 项目应只暴露一个桥接可调用的 managed entry，返回码由桥接层负责整合成 native `main` 契约。

## 6. 风险与兼容性

- C++/CLI `net10.0` + `Win32`/`x86` 的混合部署要求与 DNNE 不同，桥接产物旁可能还需要 `ijwhost.dll` 与桥接自己的 `runtimeconfig.json`。
- 取消编译期 `ProjectReference` 后，运行时路径、构建顺序与 stale output 风险上升，必须由 `FrameBuild` 明确拥有 staging 顺序。
- 若桥接采用反射调用，固定入口签名必须稳定，否则构建虽成功但运行时会晚失败。
- `Projects/TestDnne` 目前硬编码读取 `.temp/Build/test/map/testNE.dll`，若继续保留，需验证它对新桥接项目仍有意义。

## 7. 验证计划

- 构建 `Projects/test` 时，不再生成 `obj/.../dnne/bin/testNE.dll` 作为 test 路线所依赖的产物。
- `FrameBuild run test -b -n` 后，`.temp/Build/test/map` 中必须存在 bridge 产出的 `testNE.dll` 与同目录的 managed payload。
- `Projects/TestDnne` 或等价验证器必须能加载 `testNE.dll` 并成功调用 export `main`。
- callback 不需要改 DLL 名称，仍指向 `testNE.dll`。
- `Projects/test/test.csproj` 中不再出现 `DNNE`、`EnableDynamicLoading`、`DnneAddGeneratedBinaryToProject`。

## 8. 建议的实施顺序

1. 落架构级 spec / design / tasks。
2. 新建 bridge 项目并接入 solution。
3. 暴露 bridge-callable managed entry，移除 test 项目对 DNNE 导出的所有生产依赖。
4. 重写 `Run.cs` 的 publish/stage/verify 路径。
5. 校正验证器与 callback 假设。
6. 完整构建、验证、再考虑是否清理 `dnne_smoke` 残留。

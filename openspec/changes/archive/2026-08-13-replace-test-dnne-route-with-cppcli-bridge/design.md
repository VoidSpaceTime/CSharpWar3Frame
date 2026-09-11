## Context

当前仓内证据表明，`Projects/test` 的 Build/JIT 路线由以下链路构成：

- `war3map.j` 调 `StartCampaignAI(..., "callback")`
- `callback` 在 Build/Test 路径下 `LoadLibraryA(testNE.dll)`
- `GetProcAddress("main")`
- 调用导出 `main`
- 由 `Projects/test` 的 DNNE shim 再跳转到 `War3Frame.Game.ExportMain`

而实测与 smoke 实验已证明：War3 宿主中的闪退点发生在 **调用进入 DNNE 导出之后**，甚至在最小 smoke payload 中也一样。由此，本设计不再尝试修补 DNNE，而是直接替换其职责。

## Goals

- 为 `Projects/test` 提供一条稳定的、非 DNNE 的 Build/JIT 路线。
- 保留 `callback -> testNE.dll -> export main` 的外层契约不变。
- 让 native bridge 不对托管业务项目建立编译期 `ProjectReference`。
- 让桥接层只承担“加载同目录 managed payload 并转发入口”的职责。

## Non-Goals

- 不在本变更中改造 `demo`。
- 不在本变更中设计 Release/AOT 的最终形态。
- 不把 `Projects/TestDnne` 重新设计成长期正式组件；它只作为辅助验证器，保留与否后续再议。
- 不借机修复托管业务层 bug。

## Decisions

### 1. Build/test native module owner becomes a dedicated bridge project

新增一个独立的 C++/CLI x86 DLL 项目作为 `testNE.dll` 的 owner。它是 test 路线新的 native 层边界，负责提供 `extern "C" int main()` 并转发到托管入口。

### 2. The bridge loads `test.dll` from its own directory at runtime

桥接项目不得以 `ProjectReference` 或强类型直接引用 `Projects/test`。桥接层在运行时从 `GetModuleFileNameW(thisModule)` 推导自身目录，并在同目录寻找 `test.dll` 与其依赖文件。

### 3. Managed payload remains framework-dependent and side-by-side

`Projects/test` 仍发布为 framework-dependent payload，继续输出 `test.dll + test.runtimeconfig.json + test.deps.json`。桥接项目不是 DNNE 的替代打包器，而是新的 native entry owner。

### 4. `Projects/test` exports a bridge-callable managed entry, not a native export

托管项目需要有一个普通 managed 可调用入口，例如 `public static int BridgeMain()`，由桥接层通过反射或等价机制调用。它不再承担 `[UnmanagedCallersOnly]` 生产职责。

### 5. `FrameBuild` owns build order and payload staging explicitly

迁移后不能再通过“publish + copy DNNE native DLL from obj”来完成 test 路线 staging。`FrameBuild` 必须显式：

- 发布 `Projects/test`
- 构建桥接项目
- 把桥接 DLL 与 managed payload 放入同一个 `.temp/Build/test/map`

## Candidate bridge invocation styles

### Reflection-only bridge

- 优点：最少共享类型约束，不需要额外契约程序集。
- 缺点：运行时失败更晚，方法名/签名问题都延迟到实际加载时暴露。

### Custom `AssemblyLoadContext` + `AssemblyDependencyResolver`

- 优点：更适合有依赖树的 payload，更接近官方插件式按路径加载模式。
- 缺点：桥接代码更复杂。

### Chosen initial direction

先按“可落地最小实现”设计：桥接层按路径加载 `test.dll`，定位固定入口并调用；是否引入自定义 ALC 作为实现细节，在批准后的编码阶段基于官方约束再最终确定。

## Verification Matrix

| Area | Verification | Expected Result |
|---|---|---|
| Solution wiring | bridge 项目能加入 `.slnx` 并独立构建 | 无 `ProjectReference` 到 `Projects/test` |
| Managed payload | `Projects/test` 发布输出 | 仍有 `test.dll/runtimeconfig/deps`，但无 DNNE `testNE.*` 生成依赖 |
| Native bridge | x86 build output | 生成 `testNE.dll` 并导出 `main` |
| Staging | `.temp/Build/test/map` | bridge 与 payload 同目录齐全 |
| Loader contract | callback / verifier | 仍对 `testNE.dll` + `main` 生效 |
| Cleanup | repo search | test 路线不再含 DNNE 包和 DNNE copy path |

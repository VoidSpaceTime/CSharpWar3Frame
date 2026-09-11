## Context

当前仓库里同时存在“目标目录”和“可参考样例”，但还没有一份正式文档把两者关系说透。

1. `BridgeToJIT/BridgeToJIT.vcxproj` 目前还是普通 native DLL 工程，保留 Win32 与 x64 双平台配置，没有 `CLRSupport=NetCore`、`TargetFramework`、托管引用等内容。这说明 root-level `BridgeToJIT/` 还只是一个占位项目，不具备 JIT-capable C++/CLI bridge 的正式约束。
2. `BridgeToJIT/dllmain.cpp` 目前只有模板式 `DllMain`，没有托管加载逻辑。这既是现状证据，也是在提醒我们，后续若做托管 bootstrap，不能把它塞进 `DllMain`。
3. `Projects/test.bridge/testNE.vcxproj` 给出了一条仓内可参考的 C++/CLI 路线，它启用了 `CLRSupport=NetCore`、`TargetFramework=net10.0`，并引用了 `System.Runtime.Loader`。
4. `Projects/test.bridge/BridgeMain.cpp` 给出了同目录 payload 加载的近似实现思路，包括通过 `GetModuleFileNameW` 定位自身目录、通过 `AssemblyDependencyResolver` 与 `AssemblyLoadContext` 按路径加载 `test.dll`、以及通过固定托管入口做转发。
5. 用户本轮要求的是单独的 `formalize-manual-bridge-to-jit-bootstrap` docs-only change，并明确要求目标是 standalone root-level `BridgeToJIT/`，不是直接复用或修改已有 `replace-test-dnne-route-with-cppcli-bridge` change。

## Goals

- 为独立 `BridgeToJIT/` 定义正式 bridge contract，而不是继续依赖样例项目口头说明。
- 固定“32 位 Warcraft III native host 加载一个稳定 native entry，再按同目录 `project.dll` 进入托管负载”的行为模型。
- 明确未来实现的技术边界，尤其是 x86/Win32、C++/CLI `/clr:netcore`、runtime prerequisite 和 `DllMain` 禁区。
- 在不改任何代码的前提下，为后续手工实施准备出可审查的路线图。

## Non-Goals

- 本变更不实现 `BridgeToJIT/` 的项目转换或代码改造。
- 本变更不改 `FrameBuild/`。
- 本变更不改 callback 脚本。
- 本变更不讨论 64 位路线。
- 本变更不做 AOT migration。
- 本变更不重写 `Projects/test.bridge`，它只保留为参考样例。

## Hard Constraints

### 1. 宿主约束是 32 位 Warcraft III native host

- 目标宿主是 32 位原生进程，因此 bridge 的正式支持平台必须是 Win32/x86。
- `BridgeToJIT/BridgeToJIT.vcxproj` 当前虽然还保留 x64 配置，但本提案明确把 x64 路线排除在外，不允许把它作为本方案的实现目标。

### 2. 未来实现方向是 C++/CLI `/clr:netcore`

- 参考证据是 `Projects/test.bridge/testNE.vcxproj`，它已经展示了仓内可用的 `CLRSupport=NetCore` 和 `TargetFramework=net10.0` 组合。
- standalone `BridgeToJIT/` 若要承担 JIT bootstrap，就需要转向兼容该模式的 C++/CLI `/clr:netcore` 路线，而不是继续停留在纯 native DLL 模板。

### 3. managed runtime prerequisite 必须显式记录

- 这条路线不是只要有 `project.dll` 就能运行。
- 未来实现仍需要满足 JIT-capable .NET runtime path 的 prerequisite，例如受支持的 .NET runtime、bridge 自身与 payload 所需的 runtime companion files，以及可解析托管依赖的同目录布局。
- 本 docs-only 变更先把“prerequisite 必须被显式声明”定为规则，不在本轮锁死每一个文件名细节。

### 4. `DllMain` 不能承担 managed bootstrap

- `BridgeToJIT/dllmain.cpp` 当前是空模板，这种状态可以继续保留为“被动装载入口”。
- 管理运行时的初始化、payload 定位和托管入口调用，都必须发生在稳定 native export 被显式调用之后，而不是在 `DllMain` 中抢跑。

## Options

### Option A. 直接复用 `Projects/test.bridge` 模式作为正式 contract

**做法：** 把 `Projects/test.bridge/testNE.vcxproj` 和 `Projects/test.bridge/BridgeMain.cpp` 当成正式 owner，未来只是在样例目录上继续演进。

**优点：**

- 仓内已有相近实现，参考成本低。
- same-directory payload、`AssemblyLoadContext`、固定 managed entry 这些关键思路都有现成例子。

**问题：**

- `Projects/` 目录的语义更接近样例、测试或集成验证，不适合作为 root-level `BridgeToJIT/` 的正式 owner。
- 现有样例围绕的是 `test.dll` 与 `testNE.dll` 的测试命名，直接复用会把测试语义带进未来正式 bridge contract。
- 用户已经明确要求“为 standalone root-level `BridgeToJIT/` 建新 change package”，如果继续让 `Projects/test.bridge` 充当正式 owner，会偏离请求边界。

### Option B. 为 root-level `BridgeToJIT/` 建立独立 contract，`Projects/test.bridge` 只保留为参考

**做法：** 把 `BridgeToJIT/` 视为未来正式 bridge 的 owner。本次 change 只写文档，不改代码，但在文档中吸收 `Projects/test.bridge` 已验证的结构思路。

**优点：**

- 与用户要求完全一致。
- 可以把 bridge 的 owner 边界、future payload 名称 `project.dll`、32 位宿主限制和 runtime prerequisite 写成独立契约，不再被测试样例命名牵着走。
- 不需要修改现有 `replace-test-dnne-route-with-cppcli-bridge` change，也不会把两个 change 的目标混在一起。

**问题：**

- 后续实现时，需要把当前 plain native `BridgeToJIT/BridgeToJIT.vcxproj` 转成符合约束的 C++/CLI 工程。
- 还需要单独准备手工实施指导，避免实现者只会照抄 `Projects/test.bridge`。

### Recommendation

推荐 Option B。

原因不是抽象上的“更优雅”，而是它更符合这次任务的真实边界：用户要的是一个 root-level `BridgeToJIT/` 的独立 docs-only change。`Projects/test.bridge` 的价值是证据和样例，不是本变更的 owner。

## Decisions

### 1. `BridgeToJIT/` 是未来正式 bridge contract 的 owner

本提案把 root-level `BridgeToJIT/` 定义为未来正式 bridge 的 owner，`Projects/test.bridge` 只是参考样例。这样可以把正式契约和测试样例分开维护。

### 2. 正式宿主契约是“稳定 native entry”，但本轮不改 callback 脚本

本提案只要求未来 bridge 导出一个稳定 native entry，并保持与宿主现有脚本约定一致。本 docs-only 轮次不去改 callback，也不在这里绑定新的脚本命名方案。

### 3. bridge 必须按自身模块路径定位同目录 `project.dll`

当前可参考的路径是 `Projects/test.bridge/BridgeMain.cpp` 里通过 `GetModuleFileNameW` 计算桥接 DLL 自身目录，再在同目录查找托管 payload。standalone `BridgeToJIT/` 的正式 contract 采用同类原则，但 payload 名字切换为 `project.dll`。

### 4. 当 `project.dll` 存在时，bridge 通过 JIT-capable .NET runtime path 进入托管负载

- 该路径的核心是“同目录 payload + 受支持的托管 runtime prerequisite + 显式入口调用”。
- 参考实现可以借鉴 `Projects/test.bridge/BridgeMain.cpp` 里的 `AssemblyLoadContext` 与 `AssemblyDependencyResolver` 模式，或使用满足同等行为约束的等价机制。
- 这份文档只固定行为，不在本轮强绑某一段代码实现。

### 5. 当 `project.dll` 不存在时，bridge 走跳过路径

同目录 payload 缺失不是“去别的目录继续找”的信号，而是一个受控的跳过场景。bridge 应在这种情况下停止 managed dispatch，并返回一个已处理结果，而不是假定 payload 一定存在。

### 6. `DllMain` 明确排除出 managed bootstrap 路径

`DllMain` 只能作为 native DLL 的被动装载入口，不能承担托管运行时启动、payload 探测或托管方法调用。这个决定是为了避免 loader lock、初始化时序和宿主稳定性风险。

## Rejected Direction

### 把 managed bootstrap 放进 `DllMain`

这是一个明确拒绝的方向。

- `BridgeToJIT/dllmain.cpp` 当前是模板式空实现，它的正确角色是“允许 DLL 被加载”，而不是“在加载瞬间启动托管运行时”。
- 32 位 Warcraft III 宿主是原生进程，bridge 还要处理位数、运行时 prerequisite 和路径定位问题，在 `DllMain` 抢跑只会让问题更难诊断。
- 因此，本提案要求所有 managed bootstrap 行为都放到稳定 native export 的显式调用路径里。

## Migration Plan

本轮只做文档，但后续若获批准，可以按下面的阶段理解实施顺序。

1. 先完成本次 docs-only proposal、design、tasks、spec 的审核。
2. 再整理手工实施指导，明确如何把 `BridgeToJIT/BridgeToJIT.vcxproj` 从 plain native 模板转换到 x86 C++/CLI `/clr:netcore` 路线。
3. 明确稳定 native entry、同目录 `project.dll`、managed entry 契约和 prerequisite 文档。
4. 只有在新的实现轮次获得批准后，才真正修改 `BridgeToJIT/` 或讨论是否需要后续 `FrameBuild/` 接线。

## Rollback Strategy

- 由于当前 change 是 docs-only，回滚只需要停止采用或归档该变更包，不涉及二进制回滚。
- 若未来实现轮次中验证失败，优先保持 `BridgeToJIT/` 继续停留在当前 plain native 模板状态，而不是强行半落地一个不稳定的托管 bootstrap。

## Long-term Maintenance Impact

- 这份文档会把 `BridgeToJIT/` 从“目前还没用起来的目录”提升为“未来正式 bridge owner 的明确候选”，后续任何人实现时都需要先对齐这份 contract。
- 维护成本会体现在 prerequisite 说明、位数约束和入口契约的持续同步上。
- 好处是，后续不会再混淆“测试样例里的 bridge 写法”和“正式 root-level bridge 的长期责任”。

## Context Examples

1. `BridgeToJIT/BridgeToJIT.vcxproj` 当前仍是 plain native DLL 模板，因此本 spec 记录的是未来 bridge contract，而不是已完成实现。
2. `BridgeToJIT/dllmain.cpp` 当前只有模板式 `DllMain`，可作为“不得在 `DllMain` 中做 managed bootstrap”的现状证据。
3. `Projects/test.bridge/testNE.vcxproj` 与 `Projects/test.bridge/BridgeMain.cpp` 只提供同目录 JIT 加载的参考样例，不是本 spec 的 owner。

## ADDED Requirements

### Requirement: Standalone `BridgeToJIT/` MUST expose a stable native entry for a 32-bit Warcraft III host
standalone root-level `BridgeToJIT/` bridge MUST 作为 32 位 Warcraft III native host 的稳定 native 边界存在。正式支持路线 MUST 面向 Win32/x86，SHALL NOT 把 x64 输出或 AOT migration 作为本 contract 的组成部分。

#### Scenario: Host enters the supported bridge route
- **WHEN** 32 位 Warcraft III native host 加载并调用该 bridge 的正式入口
- **THEN** bridge MUST 提供一个稳定的 exported native entry
- **AND** 该入口对应的正式支持平台 MUST 是 Win32/x86
- **AND** 本 docs-only change SHALL NOT 通过 callback 脚本改名来定义这个契约

### Requirement: The bridge MUST locate `project.dll` from its own directory and skip invocation when the payload is absent
bridge MUST 按已加载模块的自身路径计算 bridge 目录，并在同目录检查 `project.dll`。若 payload 缺失，bridge MUST 跳过 managed invocation，SHALL NOT 把其他工作目录当成隐式回退来源。

#### Scenario: Payload exists beside the bridge
- **WHEN** 稳定 native entry 被调用，且同目录 `project.dll` 存在
- **THEN** bridge MUST 根据自身模块路径解析 bridge 所在目录
- **AND** MUST 从该目录解析 `project.dll`
- **AND** MUST 继续进入 JIT-capable .NET runtime path

#### Scenario: Payload is absent
- **WHEN** 稳定 native entry 被调用，但同目录 `project.dll` 不存在
- **THEN** bridge MUST 跳过 managed invocation
- **AND** MUST 返回一个已处理结果，而不是假定托管逻辑已经执行
- **AND** SHALL NOT 去扫描无关目录寻找替代 payload

### Requirement: Managed bootstrap MUST use a JIT-capable .NET runtime path outside `DllMain`
未来 bridge 实现 MUST 采用兼容 C++/CLI `/clr:netcore` 的 JIT-capable .NET runtime path，并且必须满足相应 managed runtime prerequisite。托管 bootstrap MUST 发生在稳定 native entry 的显式调用路径中，`DllMain` SHALL NOT 拥有托管启动职责。

#### Scenario: Payload exists and prerequisites are satisfied
- **WHEN** 同目录 `project.dll` 存在，且 bridge 所需的 managed runtime prerequisite 可满足
- **THEN** bridge MUST 通过显式调用路径启动托管负载
- **AND** SHALL NOT 在 `DllMain` 中执行托管 bootstrap
- **AND** MAY 采用 `AssemblyLoadContext`、`AssemblyDependencyResolver` 或满足同等行为约束的等价机制

### Requirement: The bridge MUST fail safely when bitness or runtime prerequisites are incompatible
如果 bridge 位数、宿主位数或 managed runtime prerequisite 不匹配，bridge MUST 在进入托管入口前停止 bootstrap，并返回受控失败结果，SHALL NOT 把该状态当作成功路径。

#### Scenario: Wrong bitness or runtime prerequisite is missing
- **WHEN** bridge 不是 Win32/x86 兼容输出，或所需 managed runtime prerequisite 缺失或不兼容
- **THEN** bridge MUST 在调用托管入口前终止 bootstrap
- **AND** MUST 返回受控失败结果
- **AND** SHALL NOT 将该情况当作成功启动

### Requirement: The bridge MUST treat a missing managed entry as a handled failure
即使 `project.dll` 本身存在，bridge 仍 MUST 要求其中存在约定的 managed entry。若约定的类型、方法或签名缺失或不兼容，bridge MUST 把它视为受控失败，而不是沉默成功。

#### Scenario: Managed entry is missing
- **WHEN** `project.dll` 可被加载，但约定的 managed entry 类型、方法或签名不存在或不匹配
- **THEN** bridge MUST 返回受控失败结果
- **AND** SHALL NOT 把该调用视为成功
- **AND** SHALL NOT 试图通过 `DllMain` 补做托管入口逻辑

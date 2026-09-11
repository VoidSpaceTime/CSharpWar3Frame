## Context

本提案只解决 Build 模式当前已被代码证实的问题，不扩展到 Release/AOT 产物模型重构。

已知仓内证据：

- `FrameBuild/CommandManager/Run.cs` 中 `PublishProject()` 对 `dotnet publish` 启用了重定向，但没有启动输出读取；随后直接 `WaitForExit()`。
- 同一文件中的 `Run()` 会并行执行 `BuildMap()` 与 `PublishProject()`，两者都写 `BuildDstPath/map`。
- `Projects/test/Program.cs` 同时存在 `MainAOT()` 与 `MainJIT()` 两个 `EntryPoint = "main"`。
- `Projects/TestDnne/Program.cs` 会优先探测 `_main@0` 并按 `StdCall` 调用，而 `MainJIT()` 被显式声明为 `Cdecl`。
- `War3Frame/Library/JassVM/TypeVersion.cs` 在不支持版本下仍会返回 `game.dll + 0`。
- `War3Frame/Library/JassVM/Native.cs` 与 `JassVM.cs` 在当前 VM 与 delegate index 无效时没有 fail-closed 防护。

已知外部约束：

- DNNE 当前路径本质上是 framework-dependent native hosting，而不是“特殊 CLR 模式”。
- Build 模式侧需要 side-by-side 的 managed assembly、`runtimeconfig.json`、`deps.json` 与 DNNE native shim。
- `win-x86` 目标要求位数匹配、x86 运行时可用，且导出名/调用约定必须和 loader 对齐。

## Goals / Non-Goals

**Goals**

- 稳定 `Build/test` 模式下的 `dotnet publish` 与地图产物 staging。
- 定义并落地单一 canonical Build-mode DNNE export contract。
- 让 `Projects/TestDnne` 成为仓内最小验证器，和 canonical export 严格对齐。
- 为 unsupported version / 无当前 JassVM / 非法 callback index 增加 fail-closed 安全边界。

**Non-Goals**

- 不决定 Release 应全面保留 DNNE/JIT 还是切换到 AOT。
- 不修复与本问题无关的 gameplay、ECS 或业务层逻辑。
- 不引入架构级新测试框架。

## Decisions

### 1. Build-mode publish MUST be deterministic

**Decision:** `Run.cs` 在 Build 模式下 MUST 正确消费 `dotnet publish` 的标准输出/错误输出，并 SHALL NOT 在同一输出树上继续保留无保护的并发写入。

**Rationale:** 当前卡住问题至少由“未消费重定向流”与“共享输出树并发写入”两个因素共同构成；无论哪一个是首要根因，修复都必须让链路具备确定性。

### 2. Build-mode runtime payload MUST remain side-by-side

**Decision:** Build 模式下地图运行目录 MUST 形成 DNNE/JIT 所需的 side-by-side payload，包含 canonical native shim、managed DLL、runtimeconfig、deps 与依赖项。

**Rationale:** DNNE native hosting 对目录布局有明确要求；即便入口契约修复，如果 side-by-side 产物不闭合，启动仍会失败。

### 3. Build-mode export contract MUST be singular and probeable

**Decision:** `Projects/test` 在 Build 模式 SHALL 暴露一个 canonical export；`Projects/TestDnne` MUST 按同一个导出名和调用约定探测与调用。

**Rationale:** 当前“重复 main 导出 + 仓内验证器使用另一套探测规则”让启动失败诊断失去唯一事实源。

### 4. Unsupported version and invalid VM state MUST fail closed

**Decision:** `TypeVersion`、当前 JassVM 获取、native callback dispatch 在发现 unsupported version、null VM、非法寄存器值或非法 delegate index 时 MUST 终止该路径并给出显式诊断，而 SHALL NOT 继续走非法地址或非法索引。

**Rationale:** 当前静默落到 `game.dll + 0` 或直接取无效索引，会把“可诊断失败”升级成“难定位崩溃”。

### 5. Callback safety MUST stay source/template aligned

**Decision:** checked-in callback 与生成 callback 模板 MUST 保持相同的安全语义，避免 Build 重新生成后回退到旧的不安全行为。

**Rationale:** 如果只修源文件而不修模板，下一次 Build 会重新引入风险。

## Risks / Trade-offs

- 串行化或隔离输出树会改变当前 Build 时序，但这是换取稳定性的必要代价。
- 统一导出契约后，任何仍依赖旧符号装饰名的外部 loader 都需要同步；本提案先确保仓内契约闭合。
- fail-closed 会暴露更多显式错误日志，但这比隐式崩溃更利于定位。

## Verification Matrix

| Area | Verification | Expected Result |
|---|---|---|
| Publish process | 执行 Build 模式 `test` 构建 | `dotnet publish` 明确退出，不再卡死 |
| Payload layout | 检查 `.temp/Build/test/map` | side-by-side 产物齐全 |
| Export contract | 运行 `Projects/TestDnne` | 成功探测并调用 canonical export |
| Version safety | 人工触发 unsupported version 路径或最小替身验证 | 返回显式失败，不落到 `game.dll + 0` |
| Callback safety | 验证无当前 VM / 非法 index 路径 | 安全失败，不解引用空地址、不越界调用 |
| Scope guard | 检查 Release 路径 | 未新增 Release/AOT 设计扩张 |

## Implementation Outline

1. 先在 spec 中固定 Build-mode publish/startup 与 JassVM safety 的能力要求。
2. 再让 `Run.cs` 的 publish/staging 链路具备确定性。
3. 收敛 `Projects/test` 与 `Projects/TestDnne` 的 canonical export contract。
4. 最后补齐 JassVM / callback fail-closed 防护，并验证模板一致性。

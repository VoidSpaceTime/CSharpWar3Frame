## 0. 基本信息

- Change ID: `stabilize-build-mode-dnne-startup-and-jassvm-safety`
- 提案等级: `full`
- 目标一句话: 稳定 `Build/test` 模式下的发布与 DNNE/JIT 启动链，并为 JassVM 版本/回调路径补齐最小安全防护。
- 请求来源: 用户反馈 `dotnet publish` 卡住、War3 启动后 DNNE 失败、jassVMess/JassVM 代码存在可疑问题。

### 0.1 工件矩阵

- `proposal.md`
- `design.md`
- `tasks.md`
- `specs/build-mode-dnne-publish-startup/spec.md`
- `specs/jassvm-callback-version-safety/spec.md`

## 1. 分级判定

### 1.1 为什么是这个等级

- 影响范围: `FrameBuild/`、`War3Frame/`、`Projects/test/`、`Projects/TestDnne/`
- 风险等级: 高；涉及构建链、运行时入口契约、原生地址解析与回调分发
- 可逆性: 中；可通过回滚相关文件恢复，但错误实现会直接影响 Build 模式启动
- 是否跨项目: 是
- 是否改公共契约: 是；会修改 Build 模式下模块产物、入口探测与运行时安全语义

### 1.2 升级触发器检查

- [x] 涉及 `War3Frame/` 与其他项目联动
- [ ] 涉及 `War3Frame.Generator/` 输出或契约
- [x] 涉及 `FrameBuild/`、构建链路或发布流程
- [ ] 涉及 `CSharpWar3Frame/` 入口行为
- [x] 涉及 `Projects/` 示例或集成验证行为
- [x] 涉及公共 API / 数据结构 / 配置契约
- [ ] 涉及架构边界、目录结构、依赖关系重组

## 4. FULL 提案

### 4.1 背景 / Why

当前证据已经收敛到两个 Build 模式问题簇：

1. `FrameBuild/CommandManager/Run.cs` 中的 `PublishProject()` 重定向了 `stdout/stderr`，但没有启动异步读取，随后直接 `WaitForExit()`；同时 `Run()` 又并行执行 `BuildMap()` 与 `PublishProject()`，两者共同写入 `.temp/Build/<project>/map`，这使 `dotnet publish` 卡住与同目录竞争都成为真实风险。
2. `Projects/test/Program.cs` 同时导出了两个 `EntryPoint = "main"`，而 `Projects/TestDnne/Program.cs` 又优先以 `StdCall` 方式探测 `_main@0`；这与 Build 模式 JIT/DNNE 路线的单一入口契约不一致。与此同时，`War3Frame/Library/JassVM/TypeVersion.cs` 在不支持版本下会退化为 `game.dll + 0`，`Native.cs` / `JassVM.cs` 的当前 VM 与回调分发路径也缺少 fail-closed 防护。

外部 DNNE 官方约束进一步说明：当前场景应按 framework-dependent native hosting 处理，运行所需的 managed DLL、`runtimeconfig.json`、`deps.json` 与 DNNE native shim 必须 side-by-side，且 `win-x86` 场景需要匹配的 x86 运行时与正确的导出名/调用约定。

### 4.2 变更范围 / What

- 仅针对 `Build/test` 模式修复发布卡住与 DNNE/JIT 启动失败。
- 让 Build 模式下的 publish/staging 链路具备确定性：不再在相同输出树上发生未受控并发写入，且能正确消费子进程输出并观测退出状态。
- 将 Build 模式运行时入口收敛为单一 canonical export，并使仓内验证器与该入口严格一致。
- 为 `TypeVersion`、`GetCurrentJassVM`、native callback dispatch 及 callback 模板补齐最小安全防护，使其在无效版本、无当前 VM、非法回调索引时 fail closed。
- 明确本变更不进入 Release/AOT 模型重构；相关架构问题继续留在既有 OpenSpec change 中处理。

### 4.3 全局影响分析

- `War3Frame/`: 直接受影响；将调整版本检测、当前 VM 获取与 native callback 分发的安全行为。
- `War3Frame.Generator/`: 不受影响；本次不涉及 Source Generator 输出或契约。
- `FrameBuild/`: 直接受影响；将调整 Build 模式 `dotnet publish` orchestration、输出消费与产物 staging。
- `CSharpWar3Frame/`: 预计不直接改动；CLI 入口与命令参数保持不变，只消费修复后的构建链结果。
- `Projects/`: `test` 与 `TestDnne` 直接受影响；`demo` 默认不在首轮验收范围内，仅在共享 callback/template 必须同步时被动受影响。

### 4.4 设计要点

#### 已证实问题

- `PublishProject()` 在启用重定向后未调用 `BeginOutputReadLine()` / `BeginErrorReadLine()`。
- `Run()` 当前将 `BuildMap()` 与 `PublishProject()` 并行执行到同一 Build 输出树。
- `Projects/test/Program.cs` 存在重复 `main` 导出。
- `Projects/TestDnne/Program.cs` 的导出探测与调用约定与 `MainJIT` 声明不一致。
- `TypeVersion.SelectVersion()` 在不支持版本时返回 `moduleHandle + 0`。
- `NativeCodeCallback()` 与 `GetCurrentJassVM()` 下游访问缺少边界检查。

#### 高概率推断

- “publish 卡住”的首要直接原因是重定向流未消费；同目录竞争会放大或掩盖这一症状。
- Build 模式下的 DNNE 启动失败至少包含导出/调用约定不一致这一已知因素；side-by-side 产物完整性也需要在实现阶段重新验证。
- JassVM 相关缺陷更像运行时崩溃放大器，不一定是第一失败点，但属于同一启动稳定性变更必须收敛的安全边界。

#### 方案边界

- 不处理 Release `PublishAot=true` 设计；该问题继续由 `align-release-publish-with-runtime-module-model` 追踪。
- 不在本提案阶段扩展到 gameplay / ECS 逻辑修复。
- 不引入新的大规模测试基础设施；优先使用最小 smoke/diagnostic 验证。

### 4.5 风险、兼容性、迁移

- 风险: 调整 `Run.cs` 执行顺序后，Build 模式总耗时与日志顺序会变化。  
  缓解: 明确以“稳定退出与正确产物”为首要目标，并在验证中记录新的可观察行为。

- 风险: 统一导出名与调用约定后，现有仓内或外部未同步的 loader 可能失效。  
  缓解: 以 `Projects/TestDnne` 作为仓内最小验证器，保证仓内契约先闭合。

- 风险: fail-closed 防护会让原先“静默崩溃”变成显式失败。  
  缓解: 这是期望行为；实现时补充必要诊断日志，避免回退到非法解引用。

- 回滚: 回退 `Run.cs`、`Projects/test/Program.cs`、`Projects/TestDnne/Program.cs`、JassVM 相关文件与 callback 模板即可恢复现状。

### 4.6 验证计划

- 用 Build 模式重新执行 `test`，确认 `dotnet publish` 不再无限等待，且能观测到退出结果。
- 检查 `.temp/Build/test/map` 是否具备 Build 模式 DNNE/JIT side-by-side 所需产物：`testNE.dll`、`test.dll`、`test.runtimeconfig.json`、`test.deps.json` 与依赖 DLL。
- 运行 `Projects/TestDnne`，确认仓内验证器能探测并调用 canonical export。
- 针对 unsupported version / 无当前 JassVM / 非法 callback index 路径执行最小安全验证，确认行为为显式失败而非非法地址访问。
- 确认本变更不扩展 Release/AOT 行为，只保持原有范围不变。

### 4.7 拆分任务

- Formalize capability requirements for Build-mode publish/startup stability.
- Formalize capability requirements for JassVM version/callback safety.
- Stabilize `Run.cs` Build-mode orchestration and artifact staging.
- Align `Projects/test` export contract with `Projects/TestDnne` probe contract.
- Harden JassVM version/current-VM/callback safety and keep callback template parity.
- Run Build-mode smoke verification and summarize residual risk.

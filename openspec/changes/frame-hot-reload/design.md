## Context

### 当前 post-merge 构建链事实

以下行号以当前 `HEAD=3a5f1ba` 工作树为准；历史线索只用于解释需求来源，不代表现有能力。

- `FrameBuild/CommandManager/Run.cs:15-21` 通过 `ProcessRunner.RunAsync` 执行外部命令；`:27-53` 先在最终地图目录旁生成随机 staging 文件，成功后才返回待提交产物。
- `Run.cs:56-75` 负责同步地图、复制工作区和资源准备；`:77-92` 生成 callback，并在 `:86-89` 按 `BuildMode` 写入模块路径、模块名和 `IsNative`。
- `Run.cs:95-125` 执行 `dotnet publish`。`:98` 把 `BuildModeEnum.Release` 映射为 `isNative=true`；`:101-102` 分别启用/禁用 `PublishAot`。非 native 分支在 `:117-123` 复制 `BridgeToJIT.dll`、runtime/deps 文件和 `Ijwhost.dll`。
- `Run.cs:146-173` 的主流程是准备工作区 → publish → `w3x2lni` 打包 → `BuildWorkspace.Commit` → 可选启动客户端。它没有运行中载荷握手、版本协商或重新加载步骤。
- `FrameBuild/Execution/BuildWorkspace.cs:13-31` 使用唯一 staging/previous 目录；`:34-57` 在目录替换或地图移动失败时恢复旧目录。该机制是构建产物回滚，不是运行时程序集回滚。
- `FrameBuild/Execution/ProcessRunner.cs:24-73` 只拥有单次子进程的生命周期，并在取消时只终止自己创建的进程树；它不管理 War3、WE 或 BridgeToJIT 的长期会话。
- `FrameBuild/CommandManager/WE.cs:10-40` 启动 `KKWE.exe` 或 `WE.exe`，不重定向输出、不等待退出，并以 `.we` 文件标记编辑器工作区。WE 是独立 UI 进程，不是当前 `ProcessRunner` 的一次性构建子进程。

### JIT 事实与限制

- `BridgeToJIT/BridgeEntry.cpp:40-61` 创建名为 `ProjectPayload` 的 `AssemblyLoadContext`，第二个参数为 `false`，当前代码没有显示的可卸载上下文。
- `BridgeEntry.cpp:87-129` 从 bridge 所在目录寻找 `project.dll`，以 `LoadFromAssemblyPath` 加载，查找 `War3Frame.Bootstrap.BridgeMain` 并反射调用；缺少载荷时返回 0，异常统一返回 -100。代码没有载荷版本、健康检查、卸载、状态迁移或重启协议。
- `BridgeToJIT/BridgeToJIT.vcxproj:30-43` 显示 Debug/Release 均为 Win32 `DynamicLibrary`、`CLRSupport=NetCore`、`TargetFramework=net10.0`。因此未来 JIT 设计必须同时考虑 native DLL 生命周期与 .NET AssemblyLoadContext 生命周期，不能只改 `dotnet publish` 参数。
- 历史 `git show bb05837:FrameBuild/CommandManager/Run.cs` 曾在启动 War3 后保留 `//Hot();` 占位（约第 65-66 行）；当前 `Run.cs` 与 `FrameBuild/Execution/*` 已没有该入口。该历史内容证明需求线索存在，不证明存在可调用的热更 API。

### AOT 与 WE 事实

- 当前 Release 发布路径把 `PublishAot=true`、`--self-contained true` 和 `win-x86` 组合到 `dotnet publish`；设计不能把 JIT 的程序集装载模型套到 native AOT 产物上。
- 当前 callback 在 Release 选择 `project.dll` 且 `IsNative=true`，但 `PublishProject` 仍检查 `project.dll` 存在（`Run.cs:111-114`）。这是现有构建协议，具体运行时加载方式和 AOT 产物含义必须在真实 War3 环境核实，不能在提案中推断为可替换 native image。
- WE 的启动和地图同步在 `WE.cs:18-38`，地图最终发布又经过 `Run.cs:157-166` 的 `w3x2lni`/War3 路径。DLL 更新不能假定 WE 会自动重新读取 callback 或地图内容。

## Goals / Non-Goals

**Goals:**

- 把“热更”限制为可验证的运行时语义，并与普通增量构建、地图重打包、客户端重启区分。
- 保留 JIT 可探索的最小方向，同时明确 AOT 的边界和不可承诺项。
- 为任何未来实施建立版本化载荷、健康检查、原子发布、失败回滚和可观测性要求。
- 给出从不改行为的调查到最小验证、JIT 试点、CLI/WE 集成和最终裁决的阶段门。

**Non-Goals:**

- 本 change 不实现 DLL 替换、AssemblyLoadContext 卸载、BridgeToJIT 重启协议、AOT 热更、WE 插件或 CLI 参数。
- 不改变 `War3Frame`、`War3Frame.Generator`、ECS 状态模型、Bootstrap API、`ProcessRunner` 或 `BuildWorkspace`。
- 不把 .NET Hot Reload、编辑器文件监听或普通地图重载包装成已支持的热更能力。
- 不承诺在真实 War3 客户端未验证前具备零重启、零状态迁移或零停机效果。

## 热更语义定义

本提案采用以下严格定义：**热更是运行中的 War3/JIT 会话在不退出承载会话的前提下，让后续入口调用切换到新版本代码载荷，并通过健康检查确认接管成功。** 至少要证明载荷身份、入口可调用、旧载荷不再接收新调用以及失败时仍可使用旧版本。

下列行为不单独称为热更：

1. 在 `map` 目录写入新 `project.dll`，但现有 AssemblyLoadContext 继续执行旧程序集。
2. 重新 `dotnet publish` 后下次启动才加载新文件。
3. 重打包 `.w3x`、重新打开 WE、重启 War3 或重新加载地图。
4. 只更新静态资源而不改变正在运行的代码载荷。

## 方案比较

| 方案 | 目标语义 | 当前证据可行性 | 优点 | 主要拒绝/保留理由 |
| --- | --- | --- | --- | --- |
| A. 原路径覆盖 `project.dll` | 同一会话直接看到新代码 | 不可接受 | 实现表面最小 | 当前 bridge 已加载程序集；没有版本握手、替换时序、旧调用排空或回滚协议，文件覆盖不等于程序集替换；不得作为实施方案 |
| B. 版本化文件 + 新 `AssemblyLoadContext` | 同一 bridge 进程装载新载荷 | 未证明、条件可行 | 可避免直接覆盖已使用文件 | 当前 ALC 不可卸载，且 `BridgeMain` 状态迁移未知；若旧上下文/原生回调仍存活会泄漏或双重执行，必须先做隔离 spike |
| C. JIT bridge/载荷会话重启 | 结束旧 JIT 会话后启动新版本 | 最值得验证的未来方向 | 状态边界清晰，回滚可依赖上一版本载荷 | 需要确认 War3 能否安全触发/接受 bridge 重启，且需定义 ECS/native 状态重建；不保证“无感” |
| D. War3/地图重启 | 通过完整客户端生命周期切换 | 高概率可行但不是严格热更 | 复用现有 `Run`、WE、打包链和启动路径，状态语义最清楚 | 停机明显，不满足零重启语义；可作为 AOT 和 JIT 失败时的明确 fallback |
| E. .NET Hot Reload | 对已运行代码做 metadata/IL 更新 | 当前不适合作为方案 | 理论上减少进程重启 | 当前 bridge/native/War3 宿主未展示 Hot Reload agent 或可控 ApplyUpdate 契约；AOT 不适用，诊断和回滚边界未知；不应从名称推导支持 |
| F. 仅设计、暂缓实施 | 冻结边界，等待用户裁决和环境实验 | 当前最诚实 | 不引入未经验证的运行时契约，成本可控 | 暂不提供用户可见热更；推荐采用此项 |

## 推荐方案与未来迁移路径

推荐先选择 F，并把 C 作为未来批准后的 JIT 试点方向，D 作为所有无法满足安全门时的 fallback。迁移路径如下：

1. **证据冻结**：确认 bridge 导出入口由谁调用、调用线程/重入模型、`BridgeMain` 静态状态、War3 插件行为、AOT 真实产物和 WE 保存/重载行为。调查期间不得改代码。
2. **载荷契约**：若 spike 通过，定义版本化载荷目录、manifest、入口版本、健康检查、最大并发版本和旧载荷排空状态。严禁把固定 `project.dll` 覆盖当成协议。
3. **JIT 隔离试点**：只在可控测试宿主中验证版本化载荷装载、失败回退和会话重启；不要先接入真实 War3 的默认 `Run` 路径。
4. **构建链接入**：复用 `ProcessRunner` 的取消/诊断和 `BuildWorkspace` 的 staging/previous 思路，增加明确的载荷发布状态，而不是把运行时控制逻辑塞进 `Run.cs` 的地图打包步骤。
5. **WE/CLI 体验**：只有 JIT 试点通过后，才讨论 CLI 命令、文件监听、WE 地图重载提示和客户端状态；所有长生命周期进程必须有明确 owner、停止/超时和日志。
6. **AOT 裁决**：Release 维持“重新 publish + 地图/客户端重启”语义，除非独立实验确认 AOT 运行时具备完全不同且受支持的更新机制；不为了统一 UX 而伪装 AOT 热更。

## 回滚策略

- **构建产物回滚**：继续依赖 `BuildWorkspace.Commit` 已有的 staging/previous 机制；在任何未来扩展中，发布新载荷前必须校验完整文件集和 manifest，失败不得触碰当前可用目录。
- **JIT 载荷回滚**：新版本先进入不可见的版本目录，完成入口探测/健康检查后再切换 active pointer；旧版本在新版本确认可用前保留。切换失败时只恢复 active pointer，不删除旧版本。
- **会话回滚**：若新载荷已开始执行但健康检查失败，优先停止新会话并用上一版本重新初始化；无法证明状态可恢复时，回退到 D 的完整 War3/地图重启，不尝试在不确定状态上继续运行。
- **WE 回滚**：WE 与地图文件保持独立回滚；`w3x2lni` 失败或最终地图移动失败时，不替换现有地图。不得因热更实验删除用户已有编辑器或客户端进程。
- **清理策略**：旧版本、staging 和诊断日志必须由本次发布生成的唯一标识管理；取消、超时和异常路径只清理由本次操作创建的资源。

## 阶段拆分与门禁

| 阶段 | 内容 | 通过条件 | 失败处置 |
| --- | --- | --- | --- |
| P0 设计裁决 | 用户确认语义、推荐 F/C/D 顺序和“不承诺 AOT” | 提案批准，开放问题有 owner | 维持仅设计，不改代码 |
| P1 宿主事实 spike | 在隔离环境记录 bridge 调用、重入、退出、静态状态和 ALC 行为 | 有可重复日志和成功/失败矩阵 | 否决同进程方案，保留完整重启 fallback |
| P2 JIT 载荷边界 | 只做版本化载荷与会话重启原型 | 旧载荷排空、健康检查、失败回退均可验证 | 不接真实 War3，回滚原型 |
| P3 构建链集成 | 接入 staging、manifest、CLI/日志和取消超时 | 构建失败不污染旧载荷，ProcessRunner/BuildWorkspace 责任清晰 | 关闭热更入口，仍可普通构建 |
| P4 WE/War3 验证 | 验证地图编辑、保存、打包、JIT 载荷切换的组合 | 真实环境 happy/failure 场景均通过 | 回退 D，标明需重启 |
| P5 AOT 单独裁决 | 仅验证 Release 行为，不把 JIT 结论外推 | 明确 supported/unsupported 及用户提示 | AOT 永久采用重发布+重启 |

## 风险 / Trade-offs

- **状态迁移风险**：程序集切换不等于 ECS、native handle、计时器和 callback 状态切换；缓解是先定义 session boundary，未知状态不得自动复制。
- **native/托管双重生命周期风险**：`BridgeToJIT` 是 Win32 DynamicLibrary 与 .NET `AssemblyLoadContext` 的组合；缓解是禁止未经 spike 的同进程卸载，优先会话重启。
- **文件竞争风险**：WE、w3x2lni、War3 与 bridge 可能同时访问同一目录；缓解是唯一版本目录、staging 和提交后切换，不做原路径覆盖。
- **回归范围风险**：构建链变化会影响 `FrameBuild`、CLI、Projects 和外部工具；缓解是至少 R3 五视角审查，并把真实 War3 验证列为实施阶段门。
- **体验取舍**：完整重启更可靠但不满足“零停机”；在没有状态迁移契约之前，可靠性优先于表面无感。

## 跨项目影响核对（架构师视角）

- `War3Frame/`：设计阶段不受影响；若 P2 需要状态重建 API，将升级为公共运行时契约并另立 delta。
- `War3Frame.Generator/`：设计阶段不受影响；除非未来要求生成版本/入口 manifest，否则不改 Generator。
- `FrameBuild/`：核心潜在影响面，涉及 `Run.cs`、`Execution/*`、publish、staging 和外部进程；当前只记录边界。
- `CSharpWar3Frame/`：当前不受影响；未来 CLI 热更命令和参数必须在 P3 单独评审。
- `Projects/`：当前不受影响；未来需作为隔离宿主和回归场景承载 JIT/AOT/WE 组合验证。
- `BridgeToJIT/`：当前不受影响；未来任何导出、ALC 或 native 生命周期修改都必须与 C++/CLI 工具链和 Win32 目标一起验证。
- `WE/Vendor/`：当前不修改第三方/外部工具；未来只通过已有命令边界集成，不修改 Vendor 代码。

## 验证计划（本 change）

本 change 是文档和治理提案，直接验证为 `openspec validate frame-hot-reload`；不运行构建，也不运行真实 War3/WE，因为没有实现代码且本任务明确停在审核门。未来实施必须追加：受影响 .NET 项目构建、BridgeToJIT 在具备 VS C++ 工具链机器上的构建、Projects/Regression 具名场景、JIT/AOT 发布产物检查、WE/War3 人工或授权环境验证，以及失败回滚证据。

## Open Questions

1. War3/插件是否会在同一进程内再次调用 `BridgeToJIT` 的导出 `main`，还是一次加载后永不重入？
2. `War3Frame.Bootstrap.BridgeMain` 是否创建或持有必须显式销毁的 native handle、ECS store、timer 或静态事件订阅？
3. 真实客户端是否允许在不退出 War3 的情况下替换 callback 指向的 payload，文件锁和线程安全边界是什么？
4. AOT `project.dll` 在当前 `win-x86` 发布链中的真实角色是什么，是否为 native host/启动包装，能否被任何受支持机制重新加载？
5. WE 保存地图后 `.we` 标记与 `w3x2lni` 拆包/打包的并发时序，是否允许载荷目录与地图目录独立切换？
6. 目标开发机和 CI 是否安装能构建 `BridgeToJIT.vcxproj` 的 Visual C++/C++/CLI 工具链？

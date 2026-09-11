## 0. 基本信息

- Change ID: `formalize-manual-bridge-to-jit-bootstrap`
- 提案等级: `architecture`
- 目标一句话: 为仓库根目录独立 `BridgeToJIT/` 手工 bridge 到 JIT managed payload 的路线建立正式架构文档，定义 32 位 Warcraft III 原生宿主到同目录 `project.dll` 的稳定 bootstrap 契约。
- 请求来源: 用户明确要求为 standalone root-level `BridgeToJIT/` manual bootstrap concept 创建新的 OpenSpec docs-only change package，并要求附带正式提案、方案比较与后续手工实施准备说明。

### 0.1 工件矩阵

- `proposal.md`
- `design.md`
- `tasks.md`
- `specs/manual-bridge-to-jit-bootstrap/spec.md`

### 0.2 docs-only 说明

- 本变更只新增 `openspec/changes/formalize-manual-bridge-to-jit-bootstrap/` 下的 OpenSpec 文档。
- 本变更不修改 `BridgeToJIT/`、`War3Frame/`、`War3Frame.Generator/`、`FrameBuild/`、`CSharpWar3Frame/`、`Projects/` 或任何已有 change package。
- 本变更的作用，是先把 bridge contract、运行时约束、非目标、准备步骤和评审边界写清楚，等待后续审批后再决定是否进入实现轮次。

## 1. 分级判定

### 1.1 为什么是 architecture 级

- 本提案讨论的是独立 `BridgeToJIT/` 的长期角色、native 与 managed 的边界、32 位宿主兼容约束，以及未来同目录 payload 的正式契约，不是局部实现细节。
- 用户要求把现有 `Projects/test.bridge` 作为参考样例，但又明确要求单独的 root-level `BridgeToJIT/` change package，因此必须显式比较目录归属、依赖方向和维护边界。
- 即使本轮只写文档，这份文档也会决定未来 `BridgeToJIT/` 是否作为独立 bridge owner、是否坚持 x86/Win32、是否采用 C++/CLI `/clr:netcore`、以及为什么 `DllMain` 不能承担托管 bootstrap。
- 这些内容一旦进入实现，将直接影响宿主兼容性、运行时 prerequisite、目录布局与后续构建接线，因此应按架构级治理处理。

### 1.2 升级触发器检查

- [ ] 涉及 `War3Frame/` 与其他项目联动的代码实现
- [ ] 涉及 `War3Frame.Generator/` 输出或契约变更
- [x] 涉及 `FrameBuild/` 发布与 staged payload 边界的未来契约定义，但本轮不直接改代码
- [ ] 涉及 `CSharpWar3Frame/` 入口行为变更
- [x] 涉及 `Projects/` 下现有桥接样例的复用边界与参考方式
- [x] 涉及原生宿主与托管负载之间的公共 bootstrap 契约
- [x] 涉及目录边界、依赖关系与长期维护归属的架构决策

## 2. 背景 / Why

当前仓库已经同时给出“占位项目”和“参考样例”两类证据，但还没有把 standalone `BridgeToJIT/` 路线写成正式规范。

1. `BridgeToJIT/BridgeToJIT.vcxproj` 当前是普通 native DLL 模板，仍保留 `Debug|Win32`、`Release|Win32`、`Debug|x64`、`Release|x64` 四组配置，`Keyword` 仍是 `Win32Proj`，文件中没有 `CLRSupport=NetCore`、`TargetFramework`、托管引用等内容。这说明仓库根目录的 `BridgeToJIT/` 还不是一个 JIT-capable C++/CLI bridge。
2. `BridgeToJIT/dllmain.cpp` 当前只有模板式 `DllMain`，除 `switch` 分支外没有任何托管启动行为。这一现状恰好说明，后续若要做 managed bootstrap，也不应把工作塞进 `DllMain`。
3. `Projects/test.bridge/testNE.vcxproj` 与 `Projects/test.bridge/BridgeMain.cpp` 提供了一个邻近参考：它是 x86 C++/CLI DLL，项目文件里启用了 `CLRSupport=NetCore`、`TargetFramework=net10.0`、`System.Runtime.Loader`，实现里通过 `GetModuleFileNameW` 计算自身目录，再用 `AssemblyLoadContext` 与 `AssemblyDependencyResolver` 从同目录加载 managed payload。
4. 用户本次明确要求的是“为独立 root-level `BridgeToJIT/` 手工 bootstrap 概念建立正式提案和逐步准备说明”，而不是复用或修改现有 `replace-test-dnne-route-with-cppcli-bridge` change，也不是现在就进入实现。

因此，本变更的正确动作不是改代码，而是先为 standalone `BridgeToJIT/` 建立可审查、可复用、可后续落地的架构级 docs-only 包。

## 3. 变更范围 / What

### 3.1 本提案要做的事

- 把仓库根目录 `BridgeToJIT/` 定义为一个独立 bridge contract 的候选 owner，而不是继续停留在“普通 native DLL 模板”状态。
- 明确记录目标运行时行为：bridge 导出稳定的 native entry，按已加载模块路径计算自身目录，检查同目录 `project.dll`，在 payload 缺失时跳过 managed 调用，在 payload 存在时通过 JIT-capable .NET runtime path 进入托管负载。
- 明确记录硬约束：目标宿主是 32 位 Warcraft III native host，bridge 路线只讨论 x86/Win32，未来实现方向是 C++/CLI `/clr:netcore`，并且需要满足相应 managed runtime prerequisites。
- 明确说明 `DllMain` 不是 managed bootstrap 的落点，托管启动必须发生在显式导出入口或其后续显式调用路径中。
- 记录后续手工实施指导需要覆盖的准备点，但把实际代码实现、构建接线和宿主脚本调整留到审批后的下一轮。

### 3.2 本提案明确不做的事

- 不实现任何 `BridgeToJIT/` 代码修改。
- 不把 `BridgeToJIT/BridgeToJIT.vcxproj` 立即改成 C++/CLI。
- 不修改 `BridgeToJIT/dllmain.cpp`。
- 不改 `FrameBuild/` 的 wiring、publish、staging 或 copy 逻辑。
- 不改 callback 脚本，不改导出名对应的脚本接入方式。
- 不改 `War3Frame/`、`War3Frame.Generator/`、`CSharpWar3Frame/`、`Projects/` 里的任何现有项目。
- 不走 64 位路线。
- 不做 AOT migration。
- 不更新现有 change，例如 `replace-test-dnne-route-with-cppcli-bridge`。
- 不创建 git commit。

## 4. 全局影响分析

### 4.1 `BridgeToJIT/`

- 本次文档把 `BridgeToJIT/` 的未来职责正式化，但仍不直接改动该目录下任何项目文件。
- `BridgeToJIT/BridgeToJIT.vcxproj` 和 `BridgeToJIT/dllmain.cpp` 只作为现状证据，用来说明为什么需要独立提案，以及为什么不能把 managed bootstrap 放进 `DllMain`。

### 4.2 `War3Frame/`

- 本次不直接修改 `War3Frame/`。
- `War3Frame/` 属于后续 managed payload 可能调用的业务或运行时层，但本 docs-only change 只定义 bridge 与 payload 的加载边界，不触及业务代码。

### 4.3 `War3Frame.Generator/`

- 本次不直接修改 `War3Frame.Generator/`。
- standalone `BridgeToJIT/` bootstrap contract 不改变 Source Generator 输出、语义或契约，因此这里只需要明确“无直接变化”。

### 4.4 `FrameBuild/`

- 本次不直接修改 `FrameBuild/`。
- 后续若真的要把 `BridgeToJIT/` 接进发布或 staging，必须另行审批；当前提案只先记录“未来可能受影响，但本轮 docs-only 不动构建链”。

### 4.5 `CSharpWar3Frame/`

- 本次不直接修改 `CSharpWar3Frame/`。
- CLI 或入口项目并不是这次 bootstrap contract 的 owner，因此不需要为 docs-only 提案同步改动。

### 4.6 `Projects/`

- 本次不直接修改 `Projects/`。
- `Projects/test.bridge/testNE.vcxproj` 与 `Projects/test.bridge/BridgeMain.cpp` 只作为邻近参考样例，帮助说明 x86 C++/CLI、`System.Runtime.Loader`、同目录 payload 加载可以怎样组织；它们不是本变更的直接目标，也不会在本轮被编辑。

## 5. 架构目标与候选方案

### 5.1 架构目标

- 为独立 root-level `BridgeToJIT/` 建立清晰 owner 边界，避免把未来正式桥接契约混在 `Projects/` 样例目录里。
- 固定 32 位 Warcraft III native host 的兼容前提，避免把 x64 和 AOT 讨论混入本路线。
- 固定“稳定 native entry + 同目录 `project.dll` + JIT runtime path + `DllMain` 只做被动入口”的长期 bootstrap 规则。
- 让后续手工实施指导可以按正式 spec 准备，而不是凭零散口头约定动手。

### 5.2 候选方案比较

| 方案 | 内容 | 优点 | 问题 |
|---|---|---|---|
| A. 直接复用 `Projects/test.bridge` 模式 | 把现有 `Projects/test.bridge` 视为正式模板，沿用其项目归属和测试命名模式 | 仓内已有现成 x86 C++/CLI 参考，same-directory JIT 加载思路清晰 | 目录归属仍是 `Projects/` 样例语义，容易把 `test.dll` / `testNE.dll` 的测试命名耦合进正式 `BridgeToJIT/` 契约，也不满足用户要求的“单独 root-level `BridgeToJIT/` change” |
| B. 为 root-level `BridgeToJIT/` 建立独立 contract | 把 `Projects/test.bridge` 仅作为例子，正式规范只服务于 `BridgeToJIT/` | 满足用户要求，owner 边界清楚，后续可以单独定义 `project.dll`、稳定导出入口和 runtime prerequisite | 后续实现前需要额外把当前 plain native `BridgeToJIT/` 转成符合约束的 C++/CLI bridge |

**推荐结论：** 选择方案 B。原因很简单，用户明确要求的是“独立 root-level `BridgeToJIT/` 的新 docs-only change package”，而不是把已有 `Projects/test.bridge` 直接升格成正式 owner。现有样例保留为证据和对照即可。

### 5.3 阶段拆分

- Phase 1，当前轮次：完成 docs-only proposal、design、tasks、spec，形成正式审核材料。
- Phase 2，评审准备：基于本提案整理逐步的手工实施指导和 readiness checklist，但仍不改代码。
- Phase 3，后续单独审批后：才允许开始 `BridgeToJIT/` 的实际项目转换、导出入口接入和运行时验证。

### 5.4 迁移与回滚策略

- 当前轮次没有运行时迁移，只做文档落地，因此不存在实际二进制回滚成本。
- 若本提案不获批准，直接停止在文档阶段即可，仓库现有 bridge、test 样例和构建链都保持原样。
- 若未来实现阶段验证失败，也应优先保持 `BridgeToJIT/` 继续停留在当前 plain native 状态，而不是在未验证前替换现有其他路线。

### 5.5 长期维护影响

- 一旦采用本规范，`BridgeToJIT/` 就会成为明确的 native/managed bootstrap 边界，其 owner 归属和约束会比“拿样例工程直接顶上”更清楚。
- 维护者需要持续遵守 x86/Win32、C++/CLI `/clr:netcore`、同目录 `project.dll`、runtime prerequisite 明确化、`DllMain` 不承担 managed bootstrap 等规则。
- 这种做法会增加文档要求，但能减少宿主位数、目录定位和托管启动位置上的歧义。

## 6. 风险与兼容性

- `BridgeToJIT/` 当前仍有 x64 配置，如果后续实现没有明确收缩到 Win32/x86，32 位 Warcraft III 宿主兼容性会出问题。
- C++/CLI `/clr:netcore` 路线天然依赖受支持的 .NET runtime 与桥接所需 companion files，若 prerequisite 不齐，bridge 只能走失败或跳过路径，不能假定一定可启动。
- 若未来实现把 managed bootstrap 放进 `DllMain`，会把 loader lock、托管初始化时机和宿主稳定性风险放大，因此本提案明确把它列为禁止方向。
- `Projects/test.bridge/BridgeMain.cpp` 是有效参考，但它针对的是 `test.dll` 命名和测试样例，不应被机械复制成正式 `BridgeToJIT/` 合约。
- 本轮没有运行时验证，所有结论都停留在“仓内证据 + 正式设计约束”层面，后续实现仍需单独验证。

## 7. 验证计划

- 验证目标 1：确认新 change package 只创建在 `openspec/changes/formalize-manual-bridge-to-jit-bootstrap/` 下。
- 验证目标 2：确认 `proposal.md` 明确标注 `architecture` 等级，并写清 docs-only 边界。
- 验证目标 3：确认文档中明确出现 x86/Win32、C++/CLI `/clr:netcore`、managed runtime prerequisites、`DllMain` 禁止承担 bootstrap、以及不做 64 位路线和 AOT migration。
- 验证目标 4：确认存在 `War3Frame/`、`War3Frame.Generator/`、`FrameBuild/`、`CSharpWar3Frame/`、`Projects/` 的全局影响分析，且都说明“本轮不直接改动”。
- 验证目标 5：确认 spec 中覆盖 payload 存在、payload 缺失、位数或 runtime prerequisite 不匹配、managed entry 缺失四类行为场景。

## 8. 建议的后续审查顺序

1. 先审核这份 docs-only architecture proposal 是否准确限定了范围、非目标和参考证据。
2. 再审核 standalone `BridgeToJIT/` 是否确实应独立于 `Projects/test.bridge` 成为正式 contract owner。
3. 在未批准前，不进入任何项目文件修改。
4. 如后续批准，再单独进入手工实施指导完善和实现轮次。

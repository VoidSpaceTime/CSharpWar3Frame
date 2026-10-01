## 状态

- 状态：待审核
- 等级：architecture
- 复盘强度：R3 Comprehensive
- 范围：仅架构设计与可行性评估；本 change 不包含实现

## Why

Todo 21（W4.5）要求评估构建链热更，但合并后的构建链已经不同于早期设计线索：`FrameBuild/CommandManager/Run.cs` 不再包含历史 `Hot()` 占位，而是把子进程和工作区提交分别收敛到 `FrameBuild/Execution/ProcessRunner.cs` 与 `BuildWorkspace.cs`。当前系统同时存在 JIT 调试路径、AOT 发布路径和独立的 WE 长生命周期进程，直接把“重新发布 DLL”称为热更会掩盖运行时装载、状态迁移和回滚问题。

本提案先冻结术语、边界和验证门，回答“哪些热更语义可行、成本是多少、哪些路径应明确不支持”。设计结论可以是「仅设计、暂缓实施」；在用户明确批准前不实现任何 hot reload、JIT、AOT、WE 或 CLI 行为。

## What Changes

1. 定义严格的热更语义：运行中的 War3/JIT 会话内替换正在使用的代码载荷，并完成新载荷接管；构建完成、文件覆盖、下次启动加载或 WE 重新打开地图不单独算热更。
2. 以当前 post-merge 代码和历史 `git show bb05837:FrameBuild/CommandManager/Run.cs` 证据分析 JIT、AOT、WE、地图打包和失败回滚边界。
3. 比较运行时 DLL 覆盖、版本化载荷/新加载上下文、JIT 宿主重启、War3/地图重启和 .NET Hot Reload 等候选方案，记录拒绝理由与待验证假设。
4. 给出未来若实施时的分阶段迁移、回滚、验证与安全门；任何实现任务必须在本提案获批后另行确认或按批准范围进入实施。

## 推荐裁决

**推荐选项：仅设计、暂缓实施。**

理由是当前证据足以否定“同一路径覆盖 DLL 即可热更”，但不足以证明 War3 对 `BridgeToJIT` 导出入口的重新进入、载荷状态重建和旧程序集卸载具备安全契约。若用户希望继续推进，建议只批准后续“JIT 可控会话重启 + 版本化载荷”的最小验证阶段；不得先承诺 AOT 热更或同进程无状态迁移。

## Capabilities

### New Capabilities

- 无。本 change 是纯架构设计/可行性评估，不新增当前系统行为契约。

### Modified Capabilities

- 无。`.openspec.yaml` 使用 `skip_specs: true`，因为本 change 没有可归档的行为 delta；若用户批准实施，应另立实现 change 并补充正式 capability delta（如实现改变 CLI 或运行时契约）。

## Impact

- `FrameBuild/`：仅作为设计证据和未来实现边界；当前不改 `Run.cs`、`Execution/*`、WE 流程或项目文件。
- `BridgeToJIT/`：仅核对现有 C++/CLI bridge 的装载边界；当前不改 `BridgeEntry.cpp`、`BridgeToJIT.vcxproj` 或生成物。
- `War3Frame/`：当前不改运行时状态、ECS、Source Generator 或 Bootstrap 契约；未来载荷状态迁移若需要运行时 API，必须另立公共契约评审。
- `War3Frame.Generator/`：无影响；热更设计不改变生成输出。
- `CSharpWar3Frame/`：当前不增加命令、参数或日志契约；未来 CLI 入口需在实现 change 中单独定义。
- `Projects/`：当前不增加回归场景；批准实施后应补 JIT happy/failure、AOT 明确不支持、WE 不受破坏等场景。
- `openspec/specs/`：无变更；这是设计阶段，不能把未验证方案写入当前真相层。

## 验收标准

- `proposal.md`、`design.md`、`tasks.md` 齐全，均以当前 post-merge 结构为事实基础，并保留历史线索与其非能力性质。
- `design.md` 明确定义热更语义、JIT/AOT/WE 约束、方案比较、迁移路径、回滚策略和阶段拆分。
- 方案比较明确包含并评价“仅设计、暂缓实施”，且推荐选项与可行性结论一致。
- `openspec validate frame-hot-reload` 通过；由于没有行为 delta，`.openspec.yaml` 的 `skip_specs: true` 与文档理由一致。
- 提交后停在用户审核门，不创建任何实现代码、构建链变更或实现测试。

## 风险与开放问题

- 未知 `BridgeToJIT` 导出入口由 War3/插件如何调用、调用期间是否可能重入，以及重新加载期间能否安全停止所有回调；必须通过真实宿主实验确认。
- 未知 `Bootstrap.BridgeMain` 是否持有不可迁移的静态状态、原生句柄或 ECS store；不能假设重新装载程序集会恢复一致状态。
- 未知目标机器是否具备构建 AOT 与 BridgeToJIT 所需的完整工具链；现有仓库历史已记录解决方案级构建可能受本机缺少 VS C++ 工具链影响。
- WE 的打开地图、保存回写和 `w3x2lni` 打包是长生命周期/外部工具协作，不应由 DLL 替换隐式改变；需要单独验证文件观察和地图重载行为。

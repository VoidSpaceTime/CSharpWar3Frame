## 0. 基本信息

- Change ID: `fix-temp-table-staging-preconditions`
- 提案等级: `full`
- 目标一句话: 修复首次 temp staging 缺少 `table/` 前置条件导致的构建失败，并把临时 table 布局收敛为 flat `.temp/<project>/table`。
- 请求来源: 用户在 `BuildMap()` 期间遇到 `System.IO.DirectoryNotFoundException: Could not find a part of the path 'D:\CSharp\CSharpWar3Frame\.temp\test\table'`，定位到 `FrameBuild/CommandManager/SyncW3xFile.cs:93`，明确要求先生成 full 级 OpenSpec 提案。

### 0.1 工件矩阵

- `proposal.md`
- `design.md`
- `tasks.md`
- `specs/temp-table-staging-preconditions/spec.md`

## 1. 分级判定

### 1.1 为什么是这个等级

- 影响范围: `FrameBuild/` 构建 staging 链路直接受影响，`Projects/test/` 作为输入与验证样本被间接涉及。
- 风险等级: 中；实现文件预计只落在 `FrameBuild/CommandManager/SyncW3xFile.cs`，但它属于 build staging 前置条件，一旦处理错误会继续影响后续 `BuildDstPath` 目录形态。
- 可逆性: 高；预期只需回退 `SyncW3xFile.cs` 的局部 staging 逻辑即可恢复现状。
- 是否跨项目: 是；`FrameBuild/` 负责 staging，`Projects/` 提供 table 输入资源，`BuildDstPath` 还会被下游构建消费。
- 是否改公共契约: 是；虽然实现范围收敛到单文件，但会明确 temp/build staging 的目录契约，属于构建链路契约修正。

### 1.2 升级触发器检查

- [ ] 涉及 `War3Frame/` 与其他项目联动
- [ ] 涉及 `War3Frame.Generator/` 输出或契约
- [x] 涉及 `FrameBuild/`、构建链路或发布流程
- [ ] 涉及 `CSharpWar3Frame/` 入口行为
- [x] 涉及 `Projects/` 示例或集成验证行为
- [x] 涉及公共 API / 数据结构 / 配置契约
- [ ] 涉及架构边界、目录结构、依赖关系重组

## 4. FULL 提案

### 4.1 背景 / Why

当前失败已经有足够仓内证据，说明问题集中在 temp table staging 前置条件，而不是上游资源缺失：

- `FrameBuild/CommandManager/CommandManager.cs` 把 `TempProjectBuildPath` 派生为 `.temp/<project>`，把 `BuildDstPath` 派生为 `.temp/<mode>/<project>`，说明 temp staging 的目录形态会直接传递给后续 build staging。
- `FrameBuild/CommandManager/SyncW3xFile.cs` 在首次 temp bootstrap 时只创建 `w3x2lni` 和 `.w3x`，没有同时建立 `table/`。
- 同一文件中的 `ProjectResourceToTemp()` 目前把 `tempTableDir` 视为已存在目录，直接比较时间并在无 guard 的情况下 `Directory.Delete(tempTableDir, true)`，而复制目标布局也没有被写成显式契约；本次诊断要求把它收敛为 flat `.temp/<project>/table`，不再允许落成 `table/table`。
- `Projects/test/w3x/table` 实际存在，目录下有 `misc.ini` 与 `w3i.ini`。
- `FrameBuild/Template/lni/table` 也实际存在，目录下同样有 `misc.ini` 与 `w3i.ini`。
- 当前 `.temp/test` 仅看到 `.w3x`、`map/`、`w3x2lni/`，缺少 `table/` 与 `resource/`，这与“在 table 分支先失败，尚未走到 resource staging”完全一致。
- `FrameBuild/CommandManager/Run.cs` 在 `BuildMap()` 中直接把 `TempProjectBuildPath` 复制到 `BuildDstPath`，因此 downstream build staging 会继承 temp staging 的目录形态。
- `FrameBuild/CommandManager/AssetsBuild.cs` 后续会读取 `BuildDstPath/table/w3i.ini`，说明 downstream 消费方要求的是 flat `table` 目录，而不是额外嵌套一层的路径。

### 4.2 变更范围 / What

- 主要实现范围只限 `FrameBuild/CommandManager/SyncW3xFile.cs`。
- 明确 temp table staging 的前置条件，保证首次 bootstrap 与增量同步都会得到可用的 `.temp/<project>/table`。
- 在刷新 table staging 前对缺失的 `tempTableDir` 做 guard，而不是假设它已存在。
- 把 table 复制目标显式固定为 flat `.temp/<project>/table`，不让 staging 语义滑向 `table/table`。
- `Run.cs`、`AssetsBuild.cs`、`Projects/test/`、`FrameBuild/Template/lni/table` 只作为本次分析与验证的上下游事实源，不纳入实现改动范围。

### 4.3 全局影响分析

- `War3Frame/`: 不受影响；本次不修改运行时、回调或 `BridgeToJIT` 逻辑。
- `War3Frame.Generator/`: 不受影响；不涉及 Source Generator 输出或契约。
- `FrameBuild/`: 直接受影响；实现范围聚焦在 `SyncW3xFile.cs`，但目标是修正整个 build staging 的 table 前置条件。
- `CSharpWar3Frame/`: 不受影响；CLI/入口参数与命令分发不改。
- `Projects/`: 不直接改内容；`Projects/test/w3x/table` 只是证明输入资源存在，并作为后续验证样本。

### 4.4 设计要点

- 根因不是 `Projects/test/w3x/table` 缺失，而是 temp staging 没有先把 `table/` 前置条件建立好。
- `BuildDstPath` 通过复制 `TempProjectBuildPath` 继承目录形态，所以最小修复点应放在 temp staging 源头，而不是在 downstream 再补防御。
- flat `.temp/<project>/table` 是已被 `AssetsBuild.cs` 依赖的消费契约，提案需要把这一点写成显式能力要求。
- 可能的最小修复方向是：规范化 temp `table` staging 前置条件，guard 缺失的 temp table 目录，并把复制目标收敛到 flat `.temp/<project>/table`，而不是嵌套 `table/table`。

### 4.5 风险、兼容性、迁移

- 风险: 如果错误改动了 temp 目录布局，可能影响已有增量同步结果。  
  缓解: 实现范围只限 `SyncW3xFile.cs`，并用 `Projects/test` 的现有输入和 `BuildDstPath/table/w3i.ini` 的 downstream 读取结果做回归验证。

- 风险: 首次 bootstrap 与增量刷新路径可能存在不同分支。  
  缓解: 提案要求两个路径都收敛到同一个 flat `table` 布局，而不是分别维护不同目录形态。

- 回滚: 仅需回退 `FrameBuild/CommandManager/SyncW3xFile.cs` 的对应 staging 调整。

**明确非目标**

- 不修改 callback、`BridgeToJIT`、`War3Frame` 运行时逻辑。
- 不修改 `FrameBuild/Template/lni/table` 的模板内容。
- 不发散为 `Run.cs`、`AssetsBuild.cs`、模板资产或项目资源的广义 staging 重构。

### 4.6 验证计划

- 审核通过后，重跑触发失败的 `BuildMap()` 路径，确认不再在 `SyncW3xFile.cs:93` 因缺失 `.temp/<project>/table` 抛出 `DirectoryNotFoundException`。
- 检查 `.temp/test/table` 是否以 flat 目录形态出现，并直接包含 `misc.ini`、`w3i.ini`。
- 检查 `.temp/<mode>/test/table/w3i.ini` 是否仍可通过 `Run.cs` 的目录复制进入 build staging。
- 确认 `AssetsBuild.cs` 仍能按 `BuildDstPath/table/w3i.ini` 的现有路径消费 table 数据，无需额外代码改动。
- 确认未改动 callback、`BridgeToJIT`、模板内容与广义 staging 结构。

### 4.7 拆分任务

- 在 spec 中固定 temp table staging 前置条件与 flat 目录契约。
- 在 `SyncW3xFile.cs` 内部收敛首次 bootstrap 与增量刷新逻辑。
- 只用现有 `Projects/test` 与 downstream build 结果验证目录形态与消费兼容性。

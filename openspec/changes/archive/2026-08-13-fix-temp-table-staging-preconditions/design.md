## Context

本提案处理的是一个已经被仓内证据收敛的构建前置条件问题，目标不是重构整个 staging 子系统。

已确认事实：

- `CommandManager.cs` 把 `TempProjectBuildPath` 定义为 `.temp/<project>`，把 `BuildDstPath` 定义为 `.temp/<mode>/<project>`。
- `SyncW3xFile.cs` 的首次 temp bootstrap 只复制 `w3x2lni` 与 `.w3x`，没有同时准备 `table/`。
- `ProjectResourceToTemp()` 在 table 分支中会直接读取、删除 `tempTableDir`，但没有先保证该目录存在，也没有把 flat table 目标写成显式 staging 契约。
- `Projects/test/w3x/table` 与 `FrameBuild/Template/lni/table` 都存在，说明失败不是源资源缺失。
- 当前 `.temp/test` 只看到 `.w3x`、`map/`、`w3x2lni/`，缺少 `table/` 与 `resource/`，这和报错停在 table 分支一致。
- `Run.cs` 会把 `TempProjectBuildPath` 整体复制到 `BuildDstPath`，`AssetsBuild.cs` 之后又依赖 `BuildDstPath/table/w3i.ini`。

## Goals / Non-Goals

**Goals**

- 为 temp table staging 定义稳定的前置条件。
- 把 table staging 目标收敛为 flat `.temp/<project>/table`。
- 保证 downstream build consumer 继续通过 `BuildDstPath/table/w3i.ini` 读取数据。
- 把实现范围严格收敛到 `FrameBuild/CommandManager/SyncW3xFile.cs`。

**Non-Goals**

- 不修改 `Run.cs`、`AssetsBuild.cs` 的消费逻辑。
- 不修改 callback、`BridgeToJIT`、`War3Frame` 运行时逻辑。
- 不修改 `FrameBuild/Template/lni/table` 或 `Projects/test/w3x/table` 的内容。
- 不借题扩展为广义的 staging 重构。

## Options

### Option A, 在 `SyncW3xFile.cs` 做窄范围一文件修复

做法：

- 在首次 temp bootstrap 或 table 刷新前显式建立 `.temp/<project>/table` 的前置条件。
- 在访问 `Directory.GetLastWriteTime(tempTableDir)` 与 `Directory.Delete(tempTableDir, true)` 之前，先处理目录缺失情况。
- 把复制目标固定为 flat `.temp/<project>/table`，不允许再出现 `table/table` 这种嵌套目标。

优点：

- 根因就在 temp staging 源头，修复点与失败栈一致。
- 保持 `Run.cs` 与 `AssetsBuild.cs` 的既有消费契约不变。
- 风险最小，回滚简单，也符合本次用户明确要求的批准范围。

缺点：

- 只修 table 分支，不会顺手统一 map/resource 的所有 staging 约定。
- 需要在单文件内部把首次 bootstrap 与增量刷新两个路径都梳理清楚。

### Option B, 扩展为更广的 staging 重构

做法：

- 在 `Run.cs`、`SyncW3xFile.cs`、`AssetsBuild.cs` 之间重新定义 staging 生命周期。
- 为 `table`、`resource`、`map` 建立统一初始化和 downstream 容错。
- 可能同时改 template bootstrap、build copy 和 consumer fallback。

优点：

- 理论上可以一次性统一更多 staging 规则。
- 未来也许更容易把目录契约集中管理。

缺点：

- 远超当前失败的必要修复范围。
- 会把单点构建故障升级为多文件、多模块变更，审核成本和实现风险都明显提高。
- 还可能掩盖真正根因，让 downstream consumer 带着额外容错继续吞掉源头问题。

## Recommendation

推荐 **Option A**。

理由很直接：当前失败栈、目录现状、上下游消费契约都指向 `SyncW3xFile.cs` 里的 temp table staging 前置条件。只要在源头保证 flat `.temp/<project>/table` 始终成立，`Run.cs` 的整体复制与 `AssetsBuild.cs` 的 `BuildDstPath/table/w3i.ini` 读取就会自然恢复一致，不需要引入更大的 staging 重构。

## Decisions

### 1. Temp table staging MUST 建立前置条件

`SyncW3xFile.cs` 在刷新 table staging 之前，必须先让 `.temp/<project>/table` 处于“已存在”或“可安全创建”的状态，而不是直接假设目录已经存在。

### 2. Temp table staging MUST 保持 flat 目录布局

table staging 的目标目录必须是 flat `.temp/<project>/table`。实现不应把目录再嵌套成 `.temp/<project>/table/table`。

### 3. Downstream build staging MUST 继承 flat table 布局

因为 `Run.cs` 直接复制 `TempProjectBuildPath` 到 `BuildDstPath`，所以 temp staging 的目录布局本身就是 downstream build 契约的一部分。修复必须让 `BuildDstPath/table/w3i.ini` 继续存在。

## Risks / Trade-offs

- 如果只补 delete guard，却不统一 flat 目标布局，后续仍可能留下不一致的 table 目录形态。
- 如果改动超出 `SyncW3xFile.cs`，会把一个局部 staging 故障扩展成更大的回归面。
- 如果一味在 downstream consumer 加容错，报错会消失，但 temp staging 的错误状态仍会被保留。

## Verification Matrix

| Area | Verification | Expected Result |
|---|---|---|
| Temp bootstrap | 清空或复现首次 `.temp/test` 状态后执行构建 | 不再因缺失 `.temp/test/table` 抛异常 |
| Temp table layout | 检查 `.temp/test/table` | `misc.ini`、`w3i.ini` 直接位于该目录下 |
| Build staging inheritance | 检查 `.temp/<mode>/test/table/w3i.ini` | `Run.cs` 复制后仍保留 flat table 布局 |
| Downstream consumer | 复核 `AssetsBuild.cs` 的读取路径 | 继续按 `BuildDstPath/table/w3i.ini` 工作，无需改 consumer |
| Scope guard | 检查变更文件清单 | 实现文件仅限 `FrameBuild/CommandManager/SyncW3xFile.cs` |

## Implementation Outline

1. 先在 spec 中固定 temp table staging 的前置条件与 flat 布局要求。
2. 再在 `SyncW3xFile.cs` 中处理首次 bootstrap、缺失目录 guard、flat 目录复制目标。
3. 最后验证 `.temp/<project>/table` 与 `.temp/<mode>/<project>/table` 的路径形态没有偏移，并确认 downstream consumer 仍按原路径读取。

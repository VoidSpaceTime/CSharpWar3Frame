# 任务清单：Native 同步三模式规则落地

## 1. 治理文档

- [x] `AGENTS.md` 录入「Native 同步三模式规则」章节（决策树 / Dirty 契约 / Helper-Modifier 分层 / 迁移原则）
- [x] `openspec/changes/native-sync-policy/proposal.md` 架构级提案

## 2. Player 领域改造

- [x] `War3Frame/Src/Components/Player.cs`：
  - [x] 新增 `PlayerDirtyFlags` 枚举（None/Name/Color/Alliance）
  - [x] 新增 `PlayerDirty : IComponent`（flags）
  - [x] 新增 `PlayerAllianceState : IComponent`（bits 位数组 + dirty 增量标记）
  - [x] 删除 `PlayerNameNativeRequest` / `PlayerColorNativeRequest` / `PlayerAllianceNativeRequest`
- [x] `War3Frame/Src/Helpers/PlayerHelper.cs`：
  - [x] `SetName/SetColor` 写 `PlayerNative` + 合并 Dirty
  - [x] `SetAlliance/SetNeutral` 双向写位 + Dirty；`SetVision/SetControl/SetFullControl` 单向
  - [x] `SetAllianceBit` 用 TryGetComponent 防未初始化崩溃，并打 dirty[target] 标记
  - [x] 删除写 Request 的调用
- [x] `War3Frame/Src/Systems/Native/PlayerNativeSystem.cs`：
  - [x] 三系统合并为 `PlayerNativeSyncSystem`（QuerySystem<PlayerNative, PlayerDirty>）
  - [x] 增量同步 dirty 目标，同步后清除 Dirty 与 dirty[target]
  - [x] Basic/Neutral 互斥：PASSIVE = isNeutral || isBasic

## 3. 全项目审计

- [x] 审计 `UnitCreateNativeSystem` / `UnitMoveNativeSystem` / `ItemCreateNativeSystem` 模式归类
- [x] 全仓无 `PlayerNameNativeRequest` / `PlayerColorNativeRequest` / `PlayerAllianceNativeRequest` 残留
- [x] 输出问题清单（与本次范围无关或需后续决策的发现）

## 4. 验证

- [x] `dotnet build War3Frame/War3Frame.csproj` 0 错误
- [x] 写 `summary.md`，提案状态标 `已实施`
- [x] 问题清单总结给用户，调 review 代理审核
- [x] 按审核报告修复：PASSIVE 冲突 / 全量重放 / 单向同步 / OpenSpec 缺件
- [x] 补 `design.md` + `specs/native-sync.md`
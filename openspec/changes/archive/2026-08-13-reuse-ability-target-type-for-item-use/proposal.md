## 0. 基本信息

- Change ID: `reuse-ability-target-type-for-item-use`
- 提案等级: `full`
- 目标一句话: 删除 `ItemUseTargetKind`，让物品使用请求直接沿用 `AbilityTargetType`。
- 请求来源: 用户确认 ItemUse 已统一走 companion Ability，不应继续维护独立目标类型。

## 1. 背景与目标

当前 `ItemUseTargetKind` 与 `AbilityTargetType` 重复表达目标类型，但前者缺少 `Area`，运行时因此额外维护 `Point -> Point/Area` 兼容映射。ItemUse 已经只是 companion Ability 的触发入口，这层独立目标类型不再提供额外领域价值。

本变更删除 `ItemUseTargetKind`，将 `ItemUseTarget.kind` 改为 `AbilityTargetType`。请求类型必须与 companion `AbilityBase.targetType` 精确一致；`Area` 使用 `targetX/targetY` 表达范围中心点。

## 2. 影响范围

- `War3Frame/`：受影响。修改 ItemUse 公共请求组件、helper 和目标规范化逻辑。
- `War3Frame.Generator/`：不受影响。不新增、删除或调整系统注册。
- `FrameBuild/`：不受影响。不改变构建、发布或 JIT/AOT 流程。
- `CSharpWar3Frame/`：不受影响。不改变 CLI 和配置。
- `Projects/`：受影响。测试场景需要使用 `AbilityTargetType`，并明确区分 `Point` 与 `Area`。

## 3. 方案摘要

- 删除公共 enum `ItemUseTargetKind`。
- `ItemUseTarget.kind` 直接使用 `AbilityTargetType`。
- `ItemUseSystem` 分别规范化 `None`、`Unit`、`Point`、`Area`。
- 删除 `Point -> Area` 兼容映射，改为请求类型与 Ability 类型精确匹配。
- 不保留兼容 shim；所有调用方直接迁移到 `AbilityTargetType`。

## 4. 风险、迁移与回滚

- 风险：公共类型删除会使旧调用方编译失败；原先以 `Point` 请求触发 `Area` Ability 的代码必须改为 `Area`。
- 迁移：全仓库替换 `ItemUseTargetKind`，范围目标显式传入 `AbilityTargetType.Area`。
- 回滚：整体恢复 `ItemUseTargetKind` 与旧兼容矩阵；本变更不涉及持久化数据迁移。

## 5. 验收标准

- 全仓库不存在 `ItemUseTargetKind`。
- `ItemUseTarget.kind` 类型为 `AbilityTargetType`。
- `None/Unit/Point/Area` 请求仅能匹配相同的 `AbilityBase.targetType`。
- `Area` 请求保留区域中心坐标并清空 `targetUnit`。
- `War3Frame`、`Projects/test` Release 构建通过，验证场景为 0 failures。
- 真实 War3 客户端验证单独记录。

## 6. 实施前置

本提案及其 `design.md`、`tasks.md`、`specs/item-use-target-contract/spec.md` 经用户审核批准后，才能修改运行时代码。

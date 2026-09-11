## Capability: item-use-target-contract

### Requirement: AbilityTargetType is the single target-type source

系统 MUST 使用 `AbilityTargetType` 作为物品使用目标类型的唯一来源。系统 MUST 删除 `ItemUseTargetKind`，且 MUST NOT 保留别名、兼容映射或降级分支。

### Requirement: ItemUse target type matches the companion exactly

有效 `ItemUseRequest` 的 `ItemUseTarget.kind` MUST 与 companion `AbilityBase.targetType` 完全相等，才能派发 `CastRequest`。

- `None` MUST 只匹配 `None`。
- `Unit` MUST 只匹配 `Unit`。
- `Point` MUST 只匹配 `Point`。
- `Area` MUST 只匹配 `Area`。

任何不相等的组合 MUST 被消费并拒绝，且 MUST NOT 创建 `CastState` 或 Effect。原有 `Point` 兼容 `Point/Area` 的规则由本 requirement 明确废止。

### Requirement: Each target type has explicit normalization semantics

- `None` MUST 规范化为使用者自身及其位置快照。
- `Unit` MUST 校验目标实体同 Store，并使用目标单位的位置快照。
- `Point` MUST 携带有限且位于既有边界内的点坐标，并清空 `targetUnit`。
- `Area` MUST 携带有限且位于既有边界内的区域中心坐标，并清空 `targetUnit`。

### Requirement: Downstream Ability workflow remains unchanged

规范化结果 MUST 继续通过既有 `CastRequest.targetUnit/targetX/targetY/itemOrigin` 进入 Ability workflow。本变更 MUST NOT 修改 Casting、Channeling、Cooldown 或 Effect 的长期语义，也 MUST NOT 新增 War3 Native 调用。

### Requirement: Migration is complete and verifiable

全仓库 MUST 不再引用 `ItemUseTargetKind`。验证场景 MUST 覆盖四种精确匹配以及 `Point -> Area`、`Area -> Point` 拒绝，并以 0 failures 结束。`War3Frame` 与 `Projects/test` Release 构建 MUST 通过；真实 War3 客户端结果 MUST 单独记录。

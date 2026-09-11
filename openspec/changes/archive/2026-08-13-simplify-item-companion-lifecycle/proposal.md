# Proposal: simplify-item-companion-lifecycle

**等级**: light  
**审查**: R1 Focused  
**状态**: 用户指示实施（"先按方案a来修正"）

## 背景

`ItemCompanionAbilityHelper` 当前存在三个可直接简化的问题：

1. **`ItemCompanionSource` 冗余反向指针**：`ItemActiveAbility` 已是 `ILinkComponent`（forward link item→companion），`ItemCompanionSource` 是多余的 back-pointer，仅用于 `IsOwnedCompanion` 的防御性校验。
2. **`TrySynchronizeLevel` 重建换链**：等级变化时创建新实体、搬运所有运行时状态、原子换链、删旧实体。可直接在现有 companion 上调用 `AbilityTemplate.Apply` 原地重配，消除实体重建。
3. **helper→System 分层违规**：`ItemCompanionCastCleanup.CleanupUnit` 直接调用 `CastingSystem.EnterCooldownOrReady`（internal static）。该方法逻辑属于纯 ability 状态操作，应移入 `AbilityHelper`。

## 目标

- 删除 `ItemCompanionSource` 组件
- 将 `TrySynchronizeLevel` 改为原地 `AbilityTemplate.Apply` + 保留运行时状态
- 将 `CastingSystem.EnterCooldownOrReady` 移入 `AbilityHelper`，修复分层

## 非目标

- 不改变 companion 生命周期的外部语义
- 不改变 `HasReferences`/`CollectReferencingUnits` 的全表扫实现
- 不合并 `simplify-inline-item-ability-authoring`（独立变更）

## 影响范围

| 文件 | 操作 |
|------|------|
| `War3Frame/Src/Components/Item.cs` | 删除 `ItemCompanionSource` struct |
| `War3Frame/Src/Helpers/AbilityHelper.cs` | 新增 `EnterCooldownOrReady` 方法 |
| `War3Frame/Src/Helpers/ItemCompanionAbilityHelper.cs` | 重写 `TrySynchronizeLevel`，简化 `IsOwnedCompanion`，更新 using |
| `War3Frame/Src/Systems/Ability/CastingSystem.cs` | 删除 `internal static EnterCooldownOrReady`，改调 `AbilityHelper` |

## 验收标准

- 构建通过，无编译错误
- `ItemCompanionSource` 从代码库中完全移除
- `CleanupUnit` 不再引用 `CastingSystem` 任何成员
- `TrySynchronizeLevel` 不再创建替代实体

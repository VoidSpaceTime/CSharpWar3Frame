# 总结：清理死代码、未消费字段与未实现占位

对应提案：`proposal.md`（`remove-dead-code-and-unread-fields`，`light`）
状态：`已实施`
日期：2026-09-11

## 已处理（删除）

| # | 项 | 位置 |
|---|---|---|
| D1 | 重复枚举 `ModifierSourceType`（`War3Frame` 版零引用） | `Components/Attribute/Attribute.cs` |
| D6 | `AttackState` / `CritState` / `SightState` | `Components/Attribute/Combat.cs` |
| D6 | `AbilityType` / `AbilitySettlementType` / `AbilitySettlementInfo` / `AbilityBan` | `Components/Ability/Ability.cs` |
| D6 | `PlayerState` | `Components/Player.cs` |
| D7 | 未读字段 `EffectBase.effectAttachType`（仅写不读，附着点真相在 `EffectAttachment`） | `Components/Effects.cs` + `Helpers/EffectHelper.cs` 赋值 |
| D8 | `UnitControlNativeSystem` 中大段注释掉的死代码块（替换为一行说明） | `Systems/Native/UnitControlNativeSystem.cs` |

删除前对每个符号做了全仓（含 `Projects/`）引用核对：均为 1 处（仅声明）。删除后编译 0 error，仅保留 `War3Frame.Components.ModifierSourceType`（在用）。

## 按已定决策保留

| # | 项 | 处置 |
|---|---|---|
| D2 | `ItemCreateNativeSystem` / `ItemCreateNativeRequest` | **保留**（用户后续按需实现） |
| D4 | `ModifyValue.priority` / `AttributeContributionEntry.priority` / `AttributeContributionSource.kind` | **保留**（避免公共 API 收缩；后续若做多来源优先级再实现） |
| D7 | `UnitLifeState.rebornTime` | **保留**（与 `RebornPending` 占位一同待实现） |
| D9 | `AuraHelper` | **保留**（已被生命周期终态清理调用） |

## 延后（需与既有工作协调 / 待确认）

| # | 项 | 原因 |
|---|---|---|
| D5 | `Components/Projectile.cs`（`TrajectoryType` / `ProjectileModel` / `ProjectilePositionDirty`） | 与活跃变更 `remove-dead-projectile-template-hooks` 重叠，交由该变更处理 |
| D9 | `TeamHelper`（`Library/Helper`，零引用） | 提案标注“待确认后删”，本次未确认，暂留 |

## 验证

- 删除符号全仓引用核对为 0；`enum ModifierSourceType` 仅剩 Components 版。
- War3Frame / test / CSharpWar3FrameConsole / FrameBuild / demo 编译 0 error。
- 10 个本地验证场景全 PASS。

## 后续

- D5 / D9 见上；如确认可删，另行提案或并入相关变更。

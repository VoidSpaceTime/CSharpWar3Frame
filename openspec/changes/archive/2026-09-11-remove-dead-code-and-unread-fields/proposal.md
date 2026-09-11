# 提案：清理死代码、未消费字段与未实现占位

## 元信息

- **状态**：已实施
- **等级**：`light`
- **变更 ID**：`remove-dead-code-and-unread-fields`
- **日期**：2026-09-11
- **请求来源**：仓库疏漏扫描（ULW 只读审计）
- **默认实施后审查强度**：`R0 Direct`（编译 + 静态引用核对）

## 1. 背景与目标

疏漏扫描发现一批**零引用类型/字段**、**未实现占位系统**与**只写不读字段**，增加维护噪声并制造“能力已存在”的错觉。本变更清理这些项，或明确标注保留原因。

## 2. 影响范围（逐项决策）

| # | 项 | 位置 | 处置 |
|---|---|---|---|
| D1 | 重复枚举 `ModifierSourceType` | `Components/Attribute/Attribute.cs:11`（`War3Frame`，零引用） vs `Components/Attribute/ModifyValue.cs:41`（`War3Frame.Components`，在用） | 删除前者 |
| D2 | `ItemCreateNativeSystem` 抛 `NotImplementedException` | `Systems/Native/ItemCreateNativeSystem.cs:15` | **保留**（用户计划后续按需实现，不在本变更移除；对 `ItemCreateNativeRequest` 亦保留） |
| D3 | 目标筛选占位 | `Helpers/TargetFilterRegistry.cs:117-192` | 保留骨架但移除误导性“已通过”默认，或补 TODO 文档化；默认**补文档 + 保留** |
| D4 | 未消费字段 `ModifyValue.priority` / `AttributeContributionEntry.priority` / `AttributeContributionSource.kind` | 多处 | **保留 + 注释说明未实现**（按推荐方案；避免公共 API 收缩风险。后续若做“多来源优先级取舍”再实现求值） |
| D5 | 死类型 `Projectile.cs`（`TrajectoryType`/`ProjectileModel`/`ProjectilePositionDirty`） | `Components/Projectile.cs` | 与 `remove-dead-projectile-template-hooks` 协调后删除 |
| D6 | 死组件 `AttackState`/`CritState`/`SightState`、`AbilityBan`/`AbilityType`/`AbilitySettlementInfo`、`PlayerState` | `Components/Combat.cs`、`Ability.cs`、`Player.cs` | 删除（零引用） |
| D7 | 未读字段 `UnitLifeState.rebornTime`、`EffectBase.effectAttachType`、`DamageBase.absorbed` 等 | 多处 | 删除或标注保留（`absorbed` 为文档化扩展位，保留） |
| D8 | 注释掉的死代码块 | `UnitControlNativeSystem` 等 | 删除 |
| D9 | `AuraHelper`/`TeamHelper` 整类零引用 | `Helpers/AuraHelper.cs`、`TeamHelper.cs` | AuraHelper 将被生命周期清理调用（见 `fix-unit-lifecycle-and-finalize-cleanup`）→ **保留**；TeamHelper 待确认后删 |

> 每项必须在 `tasks.md` 中以“删除 / 保留+注释”二选一落定，并在提交信息中记录理由。

## 3. 非目标

- 不实现未完成功能（仅清理或标注）。
- 不改公共行为语义。

## 4. 风险与回滚

- **风险**：误删“仅由反射/生成器/外部脚本使用”的成员。缓解：删除前对每个符号做全仓（含 `Projects/`）引用核对；生成器消费类型与 `[SystemRegister]` 系统不纳入删除。
- **回滚**：逐项 revert；无数据影响。

## 5. 验收标准

1. 每个被删符号全仓零引用（含 `Projects/`）经核对。
2. 编译 0 error。
3. 每个“保留”项有注释说明其保留原因（预留/扩展位/待实现）。
4. 与 `remove-dead-projectile-template-hooks` 无冲突重叠。

## 6. 相关文档

- `Components/Projectile.cs`、`Components/Combat.cs`、`Components/Attribute/Attribute.cs`
- `openspec/changes/remove-dead-projectile-template-hooks/`

# 提案：技能等级重算补齐施法阶段字段

## 元信息

- **Change ID**：`fix-ability-level-cast-phase-rebuild`
- **提案等级**：`light`
- **状态**：`待审核`
- **日期**：2026-09-09
- **目标一句话**：`AbilityLevelStatRebuildSystem` 在 `LevelStatDirty` 重算时，除 `baseValues` 外同步重算施法阶段 LevelValue（前摇/后摇/引导时长/tick 间隔），修复"技能升级后施法阶段按 1 级卡住"的既有缺陷。
- **请求来源**：`introduce-ability-skill-point-upgrade` 对抗审查发现的重算缺口（与加点无关，熟练度升级同样受影响），从主 change 拆出独立修复。
- **默认实施后审查强度**：`R0 Direct`
- **命中的审查升级触发器**：无新增契约；仅扩展现有重算系统行为
- **最终实施后审查强度**：`R0 Direct`
- **Oracle 可用性与 `R1` 回退方式**：不需要 `R1`
- **完整 `review-work` 授权来源**：无

---

## 背景

`AbilitySpecBuilder.Apply` 创建技能时会把施法阶段写入 AbilityStat（`CastTime`←`castPoint`、`BackswingDuration`←`backswing`、`ChannelDuration`←`channelDuration`、`ChannelTickInterval`←`channelTickInterval`）。但 `AbilityLevelStatRebuildSystem`（`War3Frame/Src/Systems/LevelExperienceSystem.cs` L109-129）只遍历 `spec.baseValues` 重算，**不重算这四个施法阶段字段**。任何路径把技能升到 2 级以上（熟练度经验或未来的技能点加点）后，前摇/后摇/引导仍停留在 1 级解析值。

## 影响范围

- 模块：`War3Frame/`
- 文件：`War3Frame/Src/Systems/LevelExperienceSystem.cs`（仅 `AbilityLevelStatRebuildSystem` 一个方法体）
- 不受影响：`War3Frame.Generator/`、`FrameBuild/`、`CSharpWar3Frame/`、`Projects/`（无行为契约变化，验证场景可选补）

## 方案摘要

`AbilityLevelStatRebuildSystem.OnUpdate` 在遍历 `spec.baseValues` 之后追加与 `AbilitySpecBuilder.Apply` 相同的四行写入：

```csharp
AbilityHelper.SetBaseValue(ability, AbilityHelper.CastTime, specData.spec.castPoint.Resolve(abilityBase.level));
AbilityHelper.SetBaseValue(ability, AbilityHelper.BackswingDuration, specData.spec.backswing.Resolve(abilityBase.level));
AbilityHelper.SetBaseValue(ability, AbilityHelper.ChannelDuration, specData.spec.channelDuration.Resolve(abilityBase.level));
AbilityHelper.SetBaseValue(ability, AbilityHelper.ChannelTickInterval, specData.spec.channelTickInterval.Resolve(abilityBase.level));
```

无新组件、无新系统、无公开 API 变化。

## 风险与回滚

- 风险：重算窗口从"仅 baseValues"扩到"含施法阶段"，若有模板在 Apply 后手动覆盖过施法阶段 AbilityStat，`LevelStatDirty` 会把覆盖值还原为 spec 解析值。缓解：施法阶段统一由 spec 推导，手动覆盖本就不受支持（Apply 是唯一写入口）；如发现覆盖场景可改为仅写脏后按需同步。
- 回滚：删除追加的四行即可。

## 验收标准

1. 模板 `castPoint = PerLevel(0.5, 0.1)`，等级 1→2 触发 `LevelStatDirty` 后，`CastTime` baseValue = 0.6。
2. 其余三个字段（后摇/引导/tick）同样随等级解析。
3. `dotnet build War3Frame/War3Frame.csproj` 0 error；`dotnet build Projects/test/test.csproj` 0 error。

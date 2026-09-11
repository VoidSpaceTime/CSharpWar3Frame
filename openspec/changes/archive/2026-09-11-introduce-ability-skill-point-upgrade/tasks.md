# 任务：技能升级加点（击杀奖励经验 + 单位技能点 + 加点）

> change：`introduce-ability-skill-point-upgrade`（full）。全部任务在提案批准后执行。
> 说明：Phase 2 的 `KillRewardHelper` 薄入口已在后续 change `unit-died-event-broadcast` 迁移为 `KillRewardSystem` 事件消费；此处按实施当时范围记录。

## Phase 1：Authoring 与数据契约

- [x] T1.1 `LevelExperience.cs` 或邻近：新增 `SkillPointPool`（unspent/earned/perLevel）。
- [x] T1.2 `UnitItemAuthoringSpec.cs`：`UnitSpec` 增加 `expReward`（LevelValue）、`skillPointsPerLevel`、`initialSkillPoints`；新增运行时组件 `UnitKillRewardData { LevelValue expReward }`。
- [x] T1.3 `AbilitySpec` 增加 `maxLevel`（默认 0）；`AbilitySpecBuilder.MaxLevel(int)`。
- [x] T1.4 `UnitSpecBuilder`：`ExpReward(LevelValue)`、`SkillPoints(perLevel, initial=0)`；`BuildTo` 挂 `UnitKillRewardData`，`perLevel>0` 时挂 `SkillPointPool`。
- [x] T1.5 新增 `AbilityUpgradeRequest`、`UnitLeveledEvent`、`AbilityUpgradedEvent`；`EventTypeRegistry.RegisterBuiltIn` 登记两个事件。

## Phase 2：击杀奖励经验（真实入口）

- [x] T2.1 `SkillPointHelper`（新）：`GrantLevels(unit, delta)`（读池改 unspent/earned）、`Upgrade(unit, ability, levels)`（只写 Request）。
- [x] T2.2 `KillRewardHelper`（新，薄入口）：`TryDispatch(killer, victim)`——读 `UnitKillRewardData.expReward`，按 victim 等级解析 >0 时创建 `ExperienceGainRequest(target: killer, amount)`。
- [x] T2.3 `DamageResolveSystem` 死亡分支（`remaining<=0 && !immune`）在 `UnitHelper.KillUnit` 后调用 `KillRewardHelper.TryDispatch(request.source, request.target)`。
- [x] 验证：击杀 expReward=100 野怪 → 击杀者收到 ExperienceGainRequest；无 ExperienceData 击杀者静默忽略。

## Phase 3：升级发点 + 事件广播

- [x] T3.1 `ExperienceSystem`：处理 Unit 升级时记录 before/after，同步 `SkillPointHelper.GrantLevels(unit, delta)`，创建 `UnitLeveledEvent` + `TriggerEventMarker`。
- [x] T3.2 `ExperienceSystem` 守卫：Ability 目标 `spec.maxLevel > 0` 时忽略熟练度升级（经验仍累计）。
- [x] 验证：`SkillPoints(1)` 单位 1→3：unspent=2、earned=2、一条 `UnitLeveledEvent{1→3}`；无池单位不发点但事件照发。

## Phase 4：加点工作流

- [x] T4.1 `AbilityUpgradeWorkflowSystem`（Immediate，QuerySystem<AbilityUpgradeRequest>）：校验 owner / Slot / maxLevel>0 / levels>0 / level+levels<=maxLevel / unspent>=levels；成功扣点、升 `AbilityBase.level`、`AddTag<LevelStatDirty>`、发 `AbilityUpgradedEvent`；失败删请求不发事件。
- [x] 验证：1 点升 1 级；点数不足/满级/非 owner/companion/maxLevel<=0 全失败且状态不变。

## Phase 5：验证

- [x] T5.1 `Projects/test/Scripts/Process/SkillPointUpgradeScenario.cs`：击杀奖励→升级→发点→加点→baseValues 随等级；失败矩阵；事件带 `TriggerEventMarker`。
- [x] T5.2 `dotnet build War3Frame/War3Frame.csproj` 0 error。
- [x] T5.3 `dotnet build Projects/test/test.csproj` 0 error。
- [x] T5.4 场景 PASS。
- [x] T5.5 写 `summary.md`。

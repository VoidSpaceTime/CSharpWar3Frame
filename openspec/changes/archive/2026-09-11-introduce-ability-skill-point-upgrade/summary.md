# 总结：技能升级加点

> change：`introduce-ability-skill-point-upgrade`（full）。状态：`已实施`（2026-09-09）。

## 改动范围

新增「击杀奖励经验 → 单位升级发点 → 技能点加点升级槽位技能」完整闭环，均以 ECS 为真相、不调用 War3 原生。

**新增组件/请求/事件**
- `SkillPointPool`（unit：unspent/earned/perLevel）、`UnitKillRewardData`（unit：击杀奖励曲线）
- `AbilityUpgradeRequest`（unit + ability + levels）
- `UnitLeveledEvent`、`AbilityUpgradedEvent`（独立事件实体 + `TriggerEventMarker`，登记 `EventTypeRegistry`）
- 文件：`Components/LevelExperience.cs`（追加）、`Components/Unit/UnitItemAuthoringSpec.cs`（追加）

**Authoring**
- `UnitSpecBuilder.ExpReward(LevelValue)`、`SkillPoints(perLevel, initial)`
- `AbilitySpecBuilder.MaxLevel(n)` / `AbilitySpec.maxLevel`

**新增逻辑**
- `Helpers/KillRewardHelper.cs`：击杀奖励薄入口，读 victim.expReward → 为 killer 发经验请求
- `Helpers/SkillPointHelper.cs`：GrantLevels/SpendPoints/Upgrade 薄入口
- `Systems/Ability/AbilityUpgradeWorkflowSystem.cs`（Immediate）：校验 owner/Slot/maxLevel/点数后扣点、升等级、打 `LevelStatDirty`、发事件
- `Systems/Ability/AbilityEffectSystems.cs`：`DamageResolveSystem` 死亡分支接入 `KillRewardHelper.TryDispatch`
- `Systems/LevelExperienceSystem.cs`：Unit 升级同步发点 + 广播 `UnitLeveledEvent`；`AbilitySpec.maxLevel>0` 技能守卫（经验只累计不自动升级）

**必要修复（接线暴露的既有缺陷）**
- `ExperienceSystem` / `AbilityLevelStatRebuildSystem` 原在 Query 迭代内做结构变更（从未被触发过），现改为"先收集快照、循环外处理"模式，与仓库其余结算系统一致。

**验证**
- `Projects/test/Scripts/Process/SkillPointUpgradeScenario.cs`：完整断言场景，已在宿主 `Program.cs` 注册。

## 全局影响

- `War3Frame/`：见上；新增系统走 `SystemRegisterAttribute`，无生成器契约变化。
- `War3Frame.Generator/`、`FrameBuild/`、`CSharpWar3Frame/`：无影响。
- `Projects/`：test 增加验证场景与注册；未配 `SkillPoints`/`expReward`/`MaxLevel` 的既有模板行为不变。

## 验证覆盖

1. `dotnet build War3Frame/War3Frame.csproj`：0 错误。
2. `dotnet build Projects/test/test.csproj`：0 错误。
3. 行为验证：因 test 宿主需真实 War3 环境，在临时无 native runner（引用 `War3Frame`）中驱动**真实** `ExperienceSystem` / `AbilityUpgradeWorkflowSystem` / `AbilityLevelStatRebuildSystem` 执行全部断言，输出 `PASS`，覆盖：
   - 击杀奖励量（victim 2 级 PerLevel(50,25) → 75）派发
   - 经验结算：升级 2 级、perLevel=1 发 1 点、一条 `UnitLeveledEvent{1→2}` + marker、请求删除
   - 加点：技能 1→2、扣 1 点、`AbilityUpgradedEvent{1→2, 1点}` + marker、rebuild 后 `LevelStatDirty` 清除且 `DamageAmount` baseValue = 150（按 2 级）
   - 失败矩阵：无点 / 满级 / Item companion / `maxLevel<=0` / 非 owner 均拒绝且状态不变
   - 熟练度守卫：`maxLevel>0` 技能收 200 经验不升级、经验累计
4. `SkillPointUpgradeScenario` 已注册进 test 宿主入口，首次真实 War3 运行时自动执行（断言为纯 ECS 语义，与 runner 等价；本地无 War3 环境故未在真实宿主执行——非阻塞理由：无 native 依赖）。

## 风险与后续

- 击杀奖励只给最后击杀者（`DamageRequest.source`），无范围分摊；团队经验列后续。
- 无「学习未拥有技能」；英雄技能组清单与 Learn 留待后续提案。
- `UnitLevelStatRebuildSystem` / `ItemLevelStatRebuildSystem` 仍为"Query 迭代内结构变更"写法（同接线前缺陷），当前无输入未触发；接入前需先收集化。
- `fix-ability-level-cast-phase-rebuild`（light）为独立 change，未随本 change 实施：加点后前摇/后摇/引导的 LevelValue 重算待其落地。

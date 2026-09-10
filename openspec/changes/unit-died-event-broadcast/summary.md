# 总结：单位死亡事件广播（UnitDiedEvent）+ 击杀奖励迁移

> change：`unit-died-event-broadcast`（full）。状态：`已实施`（2026-09-10）。

## 改动范围

引入 `UnitDiedEvent` 死亡事实广播，击杀奖励从 `DamageResolveSystem` 死亡分支的内联派发迁移为独立 `KillRewardSystem` 事件消费；死亡入口与死后副作用解耦。均为纯 ECS 操作，不调用 War3 原生。

**新增组件/事件/系统**
- `UnitDiedEvent`（unit/source，独立事件实体）：`Components/Unit/UnitLifecycle.cs`；登记 `EventTypeRegistry`（`TriggerComponents.cs`）。
- `Systems/KillRewardSystem.cs`（Interval 126）：消费 `UnitDiedEvent`，source 非空 + 死者 `expReward>0` 时创建 `ExperienceGainRequest(target: source)`；不删事件，留给 `EventCleanupSystem`。

**行为变更**
- `UnitHelper.KillUnit(Entity unit, Entity source = default)`：仅 Alive→Death 转移返回 true 并广播 `UnitDiedEvent`；无击杀者（source 空）也能广播但奖励不派发；已死/无生命周期返回 false。
- `DamageResolveSystem` 死亡分支收敛为 `KillUnit(request.target, request.source)`，不再内联 `TryDispatch`。
- `RemoveUnit` / `CleanupFinalizeEntityDispose` 注释明确"移除 ≠ 死亡，不广播事件"，删除单位用它们、勿用 `KillUnit` 当删除手段。
- 删除 `Helpers/KillRewardHelper.cs`（派发逻辑并入 `KillRewardSystem`；引用面已核实仅 2 处随本 change 迁移）。

**验证场景**
- `Projects/test/Scripts/Process/SkillPointUpgradeScenario.cs` 拆为 4 条独立验证流：
  - A 击杀 → `UnitDiedEvent` → `KillRewardSystem`(75 经验) → `ExperienceSystem` 升级发点 + `UnitLeveledEvent`
  - B 加点成功/失败矩阵/熟练度守卫（承接上 change）
  - C 同帧超杀经真实 `DamageResolveSystem`：恰一条死亡事件 + 一条经验请求
  - D 非击杀死亡（source 空）/ 无 `expReward` 死者：事件照发、奖励不派发

## 全局影响

- `War3Frame/`：新增事件组件/登记/系统；`KillUnit` 增加 source 参数并广播；伤害死亡分支收敛；helper 删除。
- `War3Frame.Generator/`、`FrameBuild/`、`CSharpWar3Frame/`：无影响（新系统走 `SystemRegisterAttribute`）。
- `Projects/`：test 场景迁移与扩展；`KillRewardHelper` 直接调用点全部清除。
- `KillUnit` 现有生产唯一调用点即 `DamageResolveSystem`，无其它调用方受影响。

## 验证覆盖

1. `dotnet build War3Frame/War3Frame.csproj`：0 error。
2. `dotnet build Projects/test/test.csproj`：0 error。
3. 行为验证：临时无 native runner（引用 `War3Frame` + 链接场景源文件）驱动真实 `KillRewardSystem` / `ExperienceSystem` / `DamageResolveSystem` / `AbilityUpgradeWorkflowSystem` 执行全部断言，输出 `SkillPointUpgradeScenario: PASS`，覆盖：
   - `KillUnit(victim, hero)` → 恰一条 `UnitDiedEvent{unit=victim, source=hero}` + marker
   - 同帧内 `KillRewardSystem` 按 victim 等级解析（PerLevel(50,25) @2 级 = 75）→ `ExperienceGainRequest`；升级后 `currentExp==25` 反证奖励量
   - `ExperienceSystem` 升级发点 1、`UnitLeveledEvent{1→2}` + marker、请求消费删除
   - 加点成功/失败矩阵/守卫不回归（B 流）
   - 同帧两发致死：恰一条 `UnitDiedEvent` + 一条经验请求（C 流）
   - source 空（非击杀死亡）与无 `expReward` 死者：事件照发、经验请求 0（D 流）
4. `SkillPointUpgradeScenario` 仍注册于 test 宿主入口，首次真实 War3 运行自动执行；本地无 War3 环境故未在真实宿主执行——非阻塞理由：断言为纯 ECS 语义，与 runner 等价。

## 风险与后续

- `KillRewardSystem` 依赖 `EventCleanupSystem`（order 132）当帧清理事件实体；本地 root 若注册消费系统必须同时注册 cleanup，否则事件残留会导致重复派发（生产 order 已保证；测试 C 流刻意不注册以保留事件计数断言）。
- 事件模型已就绪，后续可直接扩展掉落、任务、击杀计数等死亡监听方；原生死亡入口统一进 ECS 死亡路径仍留待后续提案。
- 承接的上 change `introduce-ability-skill-point-upgrade` 仍在工作区未提交（openspec 目录被 `.gitignore` 忽略，提交需 `git add -f`）。

# 提案：单位死亡事件广播（UnitDiedEvent）+ 击杀奖励迁移

## 元信息

- **Change ID**：`unit-died-event-broadcast`
- **提案等级**：`full`
- **状态**：`已实施`（用户 2026-09-10 批准并实施，见 summary.md）
- **日期**：2026-09-10
- **目标一句话**：引入 `UnitDiedEvent` 死亡事实广播，把击杀奖励从 `DamageResolveSystem` 死亡分支的内联派发迁移为独立 `KillRewardSystem` 事件消费，使死亡入口与死后副作用解耦。
- **请求来源**：`introduce-ability-skill-point-upgrade` 对抗审查发现"击杀奖励重复派发"缺陷；修复方向讨论中用户确认按**设计最优**收敛：建通用死亡事件模型，击杀奖励迁移为事件监听（原提案第 8 节"后续提案"提前落地）。
- **默认实施后审查强度**：`R2 Targeted`
- **命中的审查升级触发器**：公共事件契约；死亡结算 → 经验奖励的跨系统协作；核心死亡路径
- **最终实施后审查强度**：`R2 Targeted`
- **完整 `review-work` 授权来源**：无

### 0.1 工件矩阵

- `full`：`proposal.md`、`design.md`、`tasks.md`、`specs/unit-died-event/spec.md`
- 关联前置 change：`introduce-ability-skill-point-upgrade`（已实施未提交；本提案在其 `KillUnit` bool 门闩基础上增量）

---

## 1. 分级判定

- 影响范围：`War3Frame/` 新增事件组件/登记、`KillUnit` 签名与内部、新增 `KillRewardSystem`、`DamageResolveSystem` 死亡分支收敛；`Projects/test` 场景迁移。
- 风险等级：中。改动核心死亡路径与事件契约，但**默认关闭语义不变**：未配 `expReward` / 非击杀死亡不派发奖励；重复派发缺陷被门闩收敛。
- 可逆性：高。删除事件/系统并把死亡分支恢复为内联调用即可回滚。
- 是否跨项目：`War3Frame/` 为主，`Projects/` 验证受影响。
- 是否改公共契约：**是**（新增 `UnitDiedEvent`；`KillUnit` 增加击杀者参数并开始广播事件）。

升级触发器：跨模块协作 + 公共事件契约 + 核心死亡路径 → `full` / `R2`，不触发 `architecture`/`R3`。

---

## 2. 背景 / Why（2026-09-10 代码核实）

- `introduce-ability-skill-point-upgrade` 将击杀奖励实现为 `DamageResolveSystem` 死亡分支内联调用 `KillRewardHelper.TryDispatch`；同帧超杀存在重复派发风险，修复方向确定为 `KillUnit` 仅 Alive→Death 返回 true。
- 全仓**没有死亡事实事件**：`DamageResolveSystem` 是唯一死亡入口（`remaining<=0 && !immune → KillUnit`），死后副作用只能各自在伤害分支加调用。
- 仓库事件基础设施齐备：`EventTypeRegistry` + `TriggerEventMarker`（order 132 由 `EventCleanupSystem` 清理）；事件实体允许多监听（触发器规则 + 系统）。
- `KillRewardHelper` 引用面仅 2 处：`DamageResolveSystem` 死亡分支、`SkillPointUpgradeScenario` Phase 1。

问题：死亡副作用（击杀奖励）耦合在伤害结算分支；新增死亡路径（脚本杀、环境杀、原生死亡桥接）会重复内联同类逻辑，且伤害结算系统职责随死后副作用数量膨胀。设计最优形态是"死亡入口广播事实，奖励等副作用作为独立消费方"。

---

## 3. 变更范围 / What

### 3.1 做

1. **新增 `UnitDiedEvent`**（独立事件实体）：`unit`（死者）+ `source`（击杀者；`IsNull` = 非击杀死亡）。挂 `TriggerEventMarker` + 登记 `EventTypeRegistry`。
2. **`KillUnit` 语义升级**：签名 `KillUnit(Entity unit, Entity source = default)`。仅 Alive→Death 转移时：保留现有状态转移 + 尸体清理 `TimerTask`，并在**同一转移点**创建 `UnitDiedEvent`（`source` 透传；可选参数，无击杀者的非击杀死亡可不传）。返回 bool 语义不变。
   - **死亡 ≠ 移除**：`RemoveUnit` / `CleanupFinalizeEntityDispose` 是"移除/清理"路径，**不广播死亡事件**；删除单位用它们，禁止用 `KillUnit` 当删除手段，不新增"静默 KillUnit"变体。
3. **新增 `KillRewardSystem`**（事件消费，order 126，`Interval`，QuerySystem<UnitDiedEvent>）：读事件 `unit.expReward` 按死者等级解析 >0 且 `source` 非空时创建 `ExperienceGainRequest(target: source)`；不删除事件实体（由 `EventCleanupSystem` order 132 清理，多监听共存）。
4. **`DamageResolveSystem` 死亡分支收敛**：移除内联 `TryDispatch`，只保留 `KillUnit(request.target, request.source)`。
5. **删除 `KillRewardHelper`**：派发逻辑并入 `KillRewardSystem`（原 helper 注释自述"未来可迁移为事件监听"即本提案落地）。
6. **验证迁移**：`SkillPointUpgradeScenario` 由直接调 `KillRewardHelper.TryDispatch` 改为驱动真实 `KillUnit` + `KillRewardSystem`；Phase 6 同帧超杀断言改为"事件恰 1 条 + 经验请求恰 1 条"；补"无 expReward 死亡不派发 / source 为空（非击杀死亡）不派发"。

### 3.2 非目标

- 不做尸体停留、死亡清理、复活、单位池等生命周期扩展（保持现状，`KillUnit` 已有 `TimerTask` 尸体清理不变）。
- 不做技能点/掉落/任务等其它死亡监听方——事件模型为它们预留，但本提案只迁移击杀奖励一个消费者。
- 不新增原生死亡入口（`UnitNativeEventBridge` 原生死亡统一进 ECS 死亡路径留待后续）。
- 不改触发框架：`TriggerSystem` 规则仍可监听 `UnitDiedEvent`，与 `KillRewardSystem` 并存（事件允许多监听）。

---

## 4. 全局影响分析

- `War3Frame/`：新增事件组件/登记、系统；`KillUnit` 增加参数与广播；伤害死亡分支收敛；helper 删除。
- `War3Frame.Generator/`：无变化（新系统走 `SystemRegisterAttribute`）。
- `FrameBuild/`、`CSharpWar3Frame/`：无影响。
- `Projects/`：test 场景迁移；`KillRewardHelper` 直接调用点清理。
- `KillUnit` 现有唯一生产调用点即本提案修改的 `DamageResolveSystem`；无其它调用方受影响（`RemoveUnit` 语义独立，不广播）。

---

## 5. 方案摘要

```text
DamageResolveSystem（order 125）
  死亡判定：remaining<=0 && !immune
    → KillUnit(unit, source)：仅 Alive→Death 时
        状态转移 + 尸体 TimerTask（现状）
        + 创建 UnitDiedEvent { unit, source }（TriggerEventMarker + Registry 登记）
    → 不再内联派发奖励

KillRewardSystem（order 126，QuerySystem<UnitDiedEvent>）
  source 为空 → 忽略（非击杀死亡）
  victim.expReward.Resolve(victimLevel) > 0
    → 创建 ExperienceGainRequest(target: source)
  不删事件（EventCleanupSystem order 132 统一清理）

ExperienceSystem（order 0，下帧）
  击杀者无 ExperienceData → 静默忽略（现状）
  有 ExperienceData → 加经验/可能升级发点
```

- 死亡 → 事件 → 奖励 → 经验 均为同帧或隔一帧的确定性链路；`KillRewardSystem` order 126 保证与事件同帧消化、经验下帧结算（现状语义一致）。
- 重复派发门闩位置不变：事件只在 Alive→Death 当次创建，事件条数 = 死亡次数。

---

## 6. 风险与回滚

- 风险：`KillUnit` 从纯生命周期入口变为"状态转移 + 事件广播"。
  - 缓解：事件创建仍为纯 ECS 结构变更，不调 War3 原生；死亡唯一入口 = 唯一广播点，未来新死亡路径自动带事件，符合设计最优。
- 风险：`KillUnit` 在 Friflo Query 迭代内被调用会抛结构变更异常。
  - 缓解：现状唯一调用在 `DamageResolveSystem` 的快照循环外；`design.md` 显式记录"调用点必须不在 Query 迭代内"约束（`KillUnit` 原有 `AddComponent TimerTask` 已隐含此约束）。
- 风险：删除 `KillRewardHelper` 影响面。
  - 缓解：引用面已核实仅 2 处（伤害分支 + 测试场景 Phase 1），本提案同时迁移。
- 风险：事件广播增加每死一个单位创建 1 个事件实体的开销。
  - 缓解：与 `DamageEvent`/`HealEvent` 同级、同清理路径，量级可接受。
- 回滚：删除 `UnitDiedEvent`/`KillRewardSystem`，`KillUnit` 还原为无事件签名，伤害分支恢复内联派发。

---

## 7. 验收标准

1. 击杀配 `expReward` 的野怪：`KillUnit` 创建恰一条 `UnitDiedEvent`（含 `source`、`TriggerEventMarker`）；`KillRewardSystem` 消费后创建一条 `ExperienceGainRequest(target: 击杀者)`。
2. 同帧两发致死伤害：恰一条 `UnitDiedEvent`、一条经验请求（重复派发缺陷不再复现）。
3. `source` 为空（非击杀死亡）：事件照发、奖励不派发。
4. 无 `expReward` 的死者：事件照发、奖励不派发。
5. 击杀者无 `ExperienceData`：经验系统静默忽略（现状不回归）。
6. `EventCleanupSystem`（order 132）正常清理 `UnitDiedEvent`；监听方 order < 132 均可见（`KillRewardSystem` order 126）。
7. `KillRewardHelper` 删除后无编译残留。
8. `dotnet build War3Frame/War3Frame.csproj`、`dotnet build Projects/test/test.csproj` 0 error；验证场景 PASS。

---

## 8. 后续提案（不在本 change）

- 原生死亡入口统一进 ECS（`UnitNativeEventBridge` 死亡事件 → `KillUnit`）。
- 掉落、任务、击杀计数等更多死亡监听方（复用 `UnitDiedEvent`）。
- 尸体停留 / 复活 / 单位池生命周期扩展。

---

## 9. 请审核

本提案承接已实施未提交的 `introduce-ability-skill-point-upgrade`。批准前可指出：`KillUnit` 增加 `source` 参数是否符合预期；`KillRewardSystem` 是否应与 `TriggerSystem` 规则机制合并（本提案立场：击杀奖励是框架默认成长规则，定位独立系统）。

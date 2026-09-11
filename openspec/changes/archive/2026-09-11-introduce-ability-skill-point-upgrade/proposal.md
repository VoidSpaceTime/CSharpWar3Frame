# 提案：技能升级加点（击杀奖励经验 + 单位技能点 + 技能加点）

## 元信息

- **Change ID**：`introduce-ability-skill-point-upgrade`
- **提案等级**：`full`
- **状态**：`已实施`（用户 2026-09-09 批准并实施，见 summary.md；独立 light change 暂缓）
- **日期**：2026-09-09（修订：按对抗审查收敛范围并补真实入口）
- **目标一句话**：补齐"击杀 → 给击杀者发经验 → 单位升级 → 同步发技能点 → 玩家花点升级槽位技能"的完整加点闭环。
- **请求来源**：用户要求制定「技能升级加点」落地提案，并拍板经验量写在被击杀单位身上、死亡时触发。
- **默认实施后审查强度**：`R2 Targeted`
- **命中的审查升级触发器**：公共 authoring/运行时契约；伤害死亡结算 → 经验 → 技能点的跨系统协作
- **最终实施后审查强度**：`R2 Targeted`
- **Oracle 可用性与 `R1` 回退方式**：`R2` 以代码审查 + 测试场景支撑；技术准确性复核优先 Oracle，不可用时记录等价回退
- **完整 `review-work` 授权来源**：无

### 0.1 工件矩阵

- `full`：`proposal.md`、`design.md`、`tasks.md`、`specs/ability-skill-point-upgrade/spec.md`
- 关联独立 change：`fix-ability-level-cast-phase-rebuild`（light，施法阶段重算补齐，主 change 不依赖）

---

## 1. 分级判定

- 影响范围：`War3Frame/` 组件、请求/事件、经验系统、伤害死亡分支、Unit/Ability authoring；`Projects/test` 验证。
- 风险等级：中。改动核心成长与死亡路径，但**默认关闭**：未配 `SkillPoints` / `expReward` 的单位行为不变。
- 可逆性：高。可移除点池/请求/事件增量，保留经验升级与 `LevelStatDirty`。
- 是否跨项目：`War3Frame/` 为主，`Projects/` 验证受影响。
- 是否改公共契约：**是**（新增 `SkillPointPool`、`expReward`、`AbilitySpec.maxLevel`、Request/Event）。

升级触发器：涉及 `Projects/` + 公共 authoring/数据结构契约 → `full` / `R2`，不触发 `architecture`/`R3`。

---

## 2. 背景 / Why（2026-09-09 代码核实）

- `ExperienceSystem`（`LevelExperienceSystem.cs`）已能消费 `ExperienceGainRequest` 升 Unit/Ability/Item 等级并打 `LevelStatDirty`。
- **全仓没有任何 `ExperienceGainRequest` 创建方**：经验升级是无人接线的孤儿能力。
- 单位死亡判定唯一在 `DamageResolveSystem`（`remaining<=0 && !immune` 时 `UnitHelper.KillUnit`），击杀者即 `DamageRequest.source`；**没有通用死亡事件实体**。
- 单位升级**不发技能点**，没有"花点升技能"的工作流。
- `AbilityLevelStatRebuildSystem` 不重算施法阶段 LevelValue —— 已拆出独立 change `fix-ability-level-cast-phase-rebuild`，本 change 不实现。

缺口：击杀经验入口、单位升级发点、加点工作流三段都是空。

---

## 3. 变更范围 / What

### 3.1 做

1. **击杀奖励经验**：被击杀单位模板加 `expReward`（`LevelValue`，按被杀者自身等级解析）。`DamageResolveSystem` 死亡分支调用薄入口 `KillRewardHelper`：读取 `victim.expReward` > 0 时创建 `ExperienceGainRequest(target: killer=source)`。
2. **技能点池** `SkillPointPool`（挂单位：`unspent` / `earned` / `perLevel`）。
3. **升级同步发点**：`ExperienceSystem` 处理 **Unit** 升级时记录 `from→to`，同步对挂点池单位 `SkillPointHelper.GrantLevels(unit, delta)`（`delta=(to-from)*perLevel`），并创建 `UnitLeveledEvent`（对外广播，供 UI/触发）。不再引入中间发点系统。
4. **加点工作流**：`AbilityUpgradeRequest`（unit + ability + levels），由 `AbilityUpgradeWorkflowSystem`（Immediate）校验后扣点、升 `AbilityBase.level`、打 `LevelStatDirty`、发 `AbilityUpgradedEvent`。
5. **Authoring**：`UnitSpecBuilder.SkillPoints(perLevel, initial)`、`UnitSpecBuilder.ExpReward(LevelValue)`、`AbilitySpecBuilder.MaxLevel(n)`。
6. **成长互斥守卫**：`AbilitySpec.maxLevel > 0` 表示"加点成长技能"；`ExperienceSystem` 对 **Ability** 目标若 `spec.maxLevel > 0` 则**忽略其熟练度经验升级**（经验可累计但不再自动升等级），避免同一 `AbilityBase.level` 被两条路径双写。
7. **对外事实**：`UnitLeveledEvent`、`AbilityUpgradedEvent` 挂 `TriggerEventMarker` 并登记 `EventTypeRegistry`。
8. `Projects/test` 增加代表性加点验证场景。

### 3.2 非目标

- 不做「学习未拥有技能」：英雄技能组清单、Learn 工作流留待后续提案（本阶段技能授予仍由创建方显式装配，装配即当前等级）。
- 不做洗点、多点消耗曲线、天赋互斥、团队/范围经验分摊。
- 不建通用 `UnitDied` 事件模型；击杀奖励只在现有死亡判定分支派发，未来有通用死亡事件再迁移。
- 不调用 `UnitModifySkillPoints` / `IncUnitAbilityLevel` native；不做技能面板 UI。
- Item companion / SystemGranted 技能不接受加点。

---

## 4. 全局影响分析

- `War3Frame/`：新增组件/请求/事件/helper；经验系统补"Unit 升级发点 + 广播 + Ability 双写守卫"；伤害结算死亡分支补击杀奖励派发；authoring 扩展。
- `War3Frame.Generator/`：无变化（新系统走现有 `SystemRegisterAttribute`）。
- `FrameBuild/`、`CSharpWar3Frame/`：无影响。
- `Projects/`：新增验证场景；不配新 authoring 则行为不变。

---

## 5. 方案摘要

```text
击杀奖励
  DamageResolveSystem 死亡分支
    → KillRewardHelper.Dispatch(killer, victim)：victim.expReward>0
    → ExperienceGainRequest(target=killer)

升级发点（同步，无中间系统）
  ExperienceSystem 消费请求 → 目标为 Unit 且升级
    → SetLevel；SkillPointHelper.GrantLevels(unit, delta)（有池才加）
    → UnitLeveledEvent { from, to }（广播）
    → LevelStatDirty（已有）
  目标为 Ability 且 spec.maxLevel>0 → 忽略经验升级（守卫）

加点
  SkillPointHelper.Upgrade(unit, ability, levels) 只写 AbilityUpgradeRequest
  → AbilityUpgradeWorkflowSystem（Immediate）
      校验：owner/Slot/maxLevel/level+levels<=maxLevel/unspent>=levels
      成功：扣点 → AbilityBase.level += levels → AddTag<LevelStatDirty>
            → AbilityUpgradedEvent
  → AbilityLevelStatRebuildSystem 消费 tag 重算 baseValues（施法阶段见独立 change）
```

默认关闭：未配 `SkillPoints` 不发点；未配 `expReward` 击杀不给经验；`maxLevel<=0` 技能拒绝加点。

---

## 6. 风险与回滚

- 风险：击杀奖励从伤害结算分支派发，伤害系统职责扩大。
  - 缓解：只做一次性 Request 派发（薄入口、无长期语义）；未来通用死亡事件可迁移；现无更好锚点。
- 风险：`AbilityBase.level` 双写。缓解：运行时互斥守卫（`maxLevel>0` 的技能忽略熟练度升级），authoring 侧如发现同技能既配经验又配 `maxLevel` 提示二选一。
- 风险：加点在施法中发生导致当次施法数值变。缓解：加点只改等级与打标，不打断施法；下次施法生效点读取新值；当次已发起请求不回滚。
- 风险：经验结算延迟一帧（死亡在 order 125 派发请求，经验系统在 order 0）。缓解：可接受，`ExperienceSystem` 下帧消费；已在设计中显式声明。
- 回滚：删除新增组件/请求/事件/helper，还原经验系统与伤害分支增量。

---

## 7. 验收标准

1. 击杀配置 `expReward=100` 的野怪：击杀者（有 `ExperienceData`）收到经验；击杀者无 `ExperienceData` 则静默忽略。
2. 未配 `SkillPoints` 的单位升级后无点池、不发点。
3. 配置 `SkillPoints(perLevel:1)` 的单位从 1 升到 3：`unspent=2`、`earned=2`，且收到一条 `UnitLeveledEvent{1→3}`。
4. 槽位技能 `MaxLevel(3)`、当前 1 级、有 1 点：升级成功 → 技能 2 级、点数 0、`LevelStatDirty`；`DamageAmount` 等 `baseValues` 按 2 级解析。
5. 点数不足 / 已满级 / 非 owner / Item companion / `maxLevel<=0`：请求失败，等级与点数不变。
6. 加点技能同时被发熟练度经验：`AbilityBase.level` 不被经验系统改动。
7. `dotnet build War3Frame/War3Frame.csproj`、`dotnet build Projects/test/test.csproj` 0 error；验证场景 PASS。

---

## 8. 后续提案（不在本 change）

- 英雄技能组清单与「学习未拥有技能」（Learn）。
- 通用 `UnitDied` 事件模型 + 击杀奖励迁移到事件监听。
- 洗点/返还、多点消耗曲线、团队经验分摊。
- `fix-ability-level-cast-phase-rebuild`：施法阶段随等级重算（已另立 light change）。

---

## 9. 请审核

本提案与独立 change `fix-ability-level-cast-phase-rebuild` 一同待审。批准前可指出：击杀奖励是否只给"最后击杀者"（当前设计）、加点消耗是否必须为 1 点/级。

# 设计：技能升级加点（击杀奖励经验 + 单位技能点 + 加点）

> 对应 change：`introduce-ability-skill-point-upgrade`（full）。
> 关联独立 change：`fix-ability-level-cast-phase-rebuild`（light，施法阶段随等级重算）。

## 1. 问题定义与三条路径

| 路径 | 入口 | 现状 |
|---|---|---|
| 击杀奖励经验 | 死亡结算分支 | 无（`ExperienceGainRequest` 无任何创建方） |
| 单位升级发点 | `ExperienceSystem` Unit 升级 | 无 |
| 加点升级已有技能 | 玩家/UI 发起 | 无 |

本 change 把三段接成一条闭环；不改已有属性/施法/伤害语义。

## 2. 职责边界

```text
DamageResolveSystem
  只：结算伤害，死亡判定分支调用 KillRewardHelper 派发经验请求
  KillRewardHelper：读 victim.expReward > 0 → 创建 ExperienceGainRequest(killer)
  不：改经验、改等级、调 native

ExperienceSystem
  只：加减经验、改等级、打 LevelStatDirty
  仅 Unit 升级时同步：
    SkillPointHelper.GrantLevels(unit, delta)（有池才加）
    创建 UnitLeveledEvent（对外广播）
  守卫：Ability 目标若 AbilitySpec.maxLevel>0 → 忽略熟练度经验升级
  不：改技能等级、调 native

SkillPointHelper（薄入口）
  GrantLevels：读 SkillPointPool，改 unspent/earned（一次性便利，不长期持真相）
  Upgrade：只写 AbilityUpgradeRequest
  GetUnspent/SetLevel 类读取与状态改写收敛于此，供 UI/测试读池

AbilityUpgradeWorkflowSystem（Immediate）
  只：消费 AbilityUpgradeRequest，校验后扣点并改 AbilityBase.level、打 LevelStatDirty、发事件
  不：重算数值（交给 Rebuild）、调 native

AbilityLevelStatRebuildSystem
  只：按 AbilityBase.level 解析 spec 数值（施法阶段补齐见独立 change）
```

## 3. 数据契约

### 3.1 UnitSpec / 被杀单位奖励

```csharp
public sealed class UnitSpec
{
    // 现有字段...
    public LevelValue expReward = LevelValue.Fixed(0f); // 击杀者获得的经验，按被杀者自身等级解析
    public int skillPointsPerLevel; // 0 = 不启用发点
    public int initialSkillPoints;
}
```

- `UnitSpecBuilder.ExpReward(LevelValue)`、`SkillPoints(perLevel, initial=0)`。
- `BuildTo` 写入：`UnitKillRewardData { expReward }`（挂单位运行时）与（`perLevel>0` 时）`SkillPointPool`。
- 经验量语义：**写在"被杀死的单位"身上**（用户拍板）。野怪随自身等级成长用 `LevelValue.PerLevel/LevelTable`，固定用 `Fixed`。

### 3.2 SkillPointPool（挂单位）

```csharp
public struct SkillPointPool : IComponent
{
    public int unspent;   // 未分配
    public int earned;    // 累计获得（含 initial）
    public int perLevel;  // 每升 1 英雄级发放量（>0 才挂池）
}
```

- 只配了 `SkillPoints` 的单位才挂。
- `unspent` 不为负；扣点前 `unspent >= cost`。`earned` 只增不减（无洗点）。

### 3.3 AbilitySpec.maxLevel（加点上限 + 成长模式标记）

```csharp
public sealed class AbilitySpec
{
    // 现有字段...
    public int maxLevel; // <=0：不可加点且保留熟练度经验路径；>0：加点技能，经验升级被守卫禁用
}
```

- `AbilitySpecBuilder.MaxLevel(n)`。
- 与 `ExperienceData.maxLevel` 分离：熟练度上限只约束"非加点"技能的经验升级。

### 3.4 Request / Event

```csharp
public struct AbilityUpgradeRequest : IComponent
{
    public Entity unit;
    public Entity ability;
    public int levels; // 必须 > 0；本阶段 1 级 = 1 点
}

public struct UnitLeveledEvent : IComponent
{
    public Entity unit;
    public int fromLevel;
    public int toLevel; // 一次经验结算连升多级只发一条
}

public struct AbilityUpgradedEvent : IComponent
{
    public Entity unit;
    public Entity ability;
    public string templateName;
    public int fromLevel;
    public int toLevel;
    public int pointsSpent;
}
```

事件挂 `TriggerEventMarker` + `EventTypeRegistry` 登记；失败不发事件；监听系统 order < 132。

## 4. 主流程

### 4.1 击杀奖励（真实入口）

```text
DamageRequest(source, target)
  → DamageResolveSystem（Interval 125）
     remaining<=0 && !immune
       → UnitHelper.KillUnit(target)
       → KillRewardHelper.TryDispatch(killer: source, victim: target)
            victim 有 UnitKillRewardData && expReward.Resolve(victimLevel) > 0
              → create ExperienceGainRequest { target: killer, amount: 该值 }
  → ExperienceSystem（Interval 0，下帧）
     killer 无 ExperienceData → 忽略（天然）
     killer 有 ExperienceData → 加经验/可能升级
```

- 经验结算相对死亡延迟一帧，显式接受（order 125 vs 0）。
- 只给最后造成死亡的来源（`DamageRequest.source`）；团队/范围分摊见后续。

### 4.2 升级发点（同步，无中间系统）

```csharp
// ExperienceSystem.ApplyExperience 内
var before = GetLevel(target);
... 现有 while 升级 ...
if (target 是 Unit && 升级 && target.TryGetComponent<SkillPointPool>(out var pool))
{
    var delta = (after - before) * pool.perLevel;
    SkillPointHelper.GrantLevels(target, delta);   // unspent/earned += delta
    create UnitLeveledEvent { unit, from: before, to: after } + TriggerEventMarker;
}
// Unit 无池：不发点，事件仍发（广播/UI 用）
```

不引入"消费事件的发点系统"，避免 order 依赖与丢点窗口；事件只作对外广播事实。

### 4.3 加点已有技能

```text
SkillPointHelper.Upgrade(unit, ability, levels)
  → create AbilityUpgradeRequest
  → AbilityUpgradeWorkflowSystem（Immediate，消费后删请求）
      校验失败（删请求，状态不变）：
        ability 有效；AbilityOwner.owner == unit；mountType == Slot
        spec.maxLevel > 0；levels > 0；ability.level + levels <= maxLevel
        SkillPointPool.unspent >= levels
      成功：
        pool.unspent -= levels
        AbilityBase.level += levels
        ability.AddTag<LevelStatDirty>()
        create AbilityUpgradedEvent { from, to, pointsSpent } + TriggerEventMarker
```

### 4.4 熟练度/加点互斥守卫

```text
ExperienceSystem 处理 Ability 目标时：
  if (ability.TryGetComponent<AbilitySpecData>(out var data)
      && data.spec.maxLevel > 0)
      → 该次经验只累计，不触发 SetLevel（canLevelUp=false 效果）
```

同一 `AbilityBase.level` 单一控制路径：`maxLevel>0` → 加点路径；否则 → 经验路径（保持现有孤儿能力不破坏）。

## 5. 系统 order

| 系统 | kind / order | 说明 |
|---|---|---|
| `ExperienceSystem` | 保持 Interval 0 | 升级 + 同步发点 + 广播事件，无外部顺序依赖 |
| `AbilityUpgradeWorkflowSystem` | Immediate（新增） | 消费 Request，尽早于 Interval Rebuild |
| `AbilityLevelStatRebuildSystem` | 保持 Interval 0 | tag 驱动；同帧或下帧消费均可（最终一致） |
| `DamageResolveSystem` | 保持 Interval 125 | 死亡分支派发经验请求，下帧被经验系统处理 |

帧内先后不构成正确性依赖：`LevelStatDirty` tag 每帧被 Rebuild 消费；经验请求隔帧结算显式声明。

## 6. Native / 分层

- 全程不新增 War3 原生调用；点池、等级、经验都是 ECS 真相。
- 工作流系统只写 ECS 状态与 Request/Event。
- `KillRewardHelper` / `SkillPointHelper` 是薄入口：一次性便利调用或 Request 创建，不承载长期语义。

## 7. 兼容与默认关闭

- 未配 `SkillPoints`：无池，升级只多发一条可忽略事件。
- 未配 `expReward`：击杀不发经验，死亡路径与现在一致（多一次零值读取）。
- 未配 `MaxLevel`：技能不可加点；熟练度经验路径保持。
- Item companion / 非 Slot：加点拒绝。
- 现有模板、场景、Trigger 均无行为回归。

## 8. 候选方案

| 方案 | 结论 |
|---|---|
| A. 击杀奖励写被杀单位 + 死亡分支派发 | **采用**（用户拍板；现死亡锚点唯一） |
| B. 建通用 UnitDied 事件再让奖励系统监听 | 更松耦合，但需整套死亡事件模型；列后续 |
| C. 发点经事件 + 独立 GrantSystem | 引入顺序依赖与丢点窗口；拒绝 |
| D. Learn 未拥有技能 | 需英雄技能组 authoring；列后续 |
| E. native 英雄点当真相 | 违反 ECS 真相；拒绝 |

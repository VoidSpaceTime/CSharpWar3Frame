## Context

现有技能系统的核心路径是：

1. ability entity 持有 `EffectSpecData` 或旧版 `*EffectData` payload。
2. `AbilityEffectHelper` 在施法完成后创建运行时 effect entity。
3. `ProjectileSystem`、`AreaSearchSystem`、`DamageEffectSystem`、`HealEffectSystem`、`BuffEffectSystem` 消费 effect entity。
4. 结算系统生成或消费 `DamageRequest`、`HealRequest`、`BuffApplyRequest`。

这套模型对一次性效果是清晰的，但它缺少“地面区域自身是运行时实体”的概念。`AreaSearchData` 只是一次查询，不会留下区域；`BuffDuration` 只属于单位身上的 Buff，不属于地面区域；`TimerTaskKind.PeriodicEffect` 已存在枚举迹象，但没有完整的地面周期效果语义。`GroupHelper` 已经有 `FindInLine` / `FindInCone`，但 `EffectSpec` 没有线形/扇形 step。`AuraSystem` 已经具备“区域内单位获得/离开移除属性 Buff”的相近机制，但它绑定 owner、缺少地面区域生命周期和周期伤害，不适合作为油污/燃烧地面的直接语义 owner。

用户提出的两个技能暴露了该缺口：

- `凝固汽油`: 丢出汽油桶，对目标范围内所有地面单位造成 -20 移速，持续 10 秒。
- `喷火`: 造成直线喷火伤害 100 点；如果接触到地面有汽油，则对范围内所有单位造成每秒 10 点燃烧伤害，持续 5 秒。

如果只用当前 `SetEffectSpec`，只能表达“施法瞬间对当前范围内单位施加减速”或“直线即时伤害”的弱化版本，无法表达油污留存、后续进入区域、油火反应和持续点燃。

## Goals / Non-Goals

**Goals:**

- 让地面持续区域成为 ECS entity，而不是 helper 内部状态或一次性 effect 的副产物。
- 支持地面区域的显式位置、半径、持续时间、语义标签和来源。
- 支持区域持续 Buff：单位进入区域时施加 Buff，离开或区域消失时移除。
- 支持区域周期伤害：按固定 tick 产生 `DamageRequest`，不直接扣血。
- 支持区域反应：例如 `Oil` 区域被 `Fire` 命中后转换为 `BurningGround`。
- 支持线形/扇形搜索 step，复用现有 `GroupHelper.FindInLine` / `FindInCone`。
- 保持 ECS/Native 分层：地面区域语义在 ECS，native/visual 只做表现或执行。

**Non-Goals:**

- 不在本提案阶段实现运行时代码。
- 不让 `AbilityHelper.SetEffectSpec` 直接承载复杂行为逻辑或 delegate action。
- 不把地面区域状态保存在 helper、template class 或 native handle 中。
- 不引入第二套位置真相；地面区域使用现有 `Position`。
- 不要求本轮实现完整元素系统，只覆盖最小的 `Oil + Fire` 反应模型。

## Decisions

### 1. Ground area is a runtime ECS entity

**Decision:** 地面持续区域 MUST 表达为 ECS entity，至少包含 `Position`、半径、生命周期、来源、语义标签。

**Rationale:** 油污、燃烧地面、毒雾等区域是持续存在的运行时对象，必须能被后续技能查询、反应和清理。

### 2. Ground area tags are semantic data, not behavior

**Decision:** `Oil`、`Fire`、`Burning` 等标签只描述区域语义；真正行为由系统解释。

**Rationale:** 避免把行为塞进 component 或 template delegate，保持 ECS 数据驱动。

### 3. Area buff and periodic damage are separate systems

**Decision:** 区域 Buff 与周期伤害 SHOULD 分成独立系统：

- Area buff system 负责进入/离开范围时施加/移除 Buff。
- Area periodic damage system 负责按 tick 生成 `DamageRequest`。

**Rationale:** Buff 与伤害生命周期不同，拆分能避免单个系统同时承担过多职责。

### 4. Reactions transform or spawn ground areas

**Decision:** 区域反应 SHOULD 由 reaction system 消费“命中事件/反应请求”，把匹配区域转换或生成新区域。

示例：

- `Oil` 被 `Fire` 命中。
- 原 `Oil` 区域过期或被替换。
- 新建 `BurningGround` 区域，持续 5 秒，每秒伤害 10。

**Rationale:** 反应是区域和技能之间的跨实体协作，不应放进 `EffectSpecBuilder` 的普通 step 里硬编码。

### 5. Line and cone search become first-class effect steps

**Decision:** `EffectSpec` SHOULD 增加线形/扇形搜索 step，或引入等价的 shape search payload，使技能能表达“直线喷火”“扇形喷吐”等命中形状。

**Rationale:** 底层 `GroupHelper` 已有能力，但当前 effect chain 只能表达圆形 AreaSearch。

### 6. Templates remain examples, not behavior owners

**Decision:** `凝固汽油` 与 `喷火` 模板只配置数据，不直接查询地面区域、不直接创建伤害、不直接调用 native。

**Rationale:** 模板是配置入口，不应成为工作流系统。

## Proposed Runtime Shape

### Core components

候选组件名称可在实现阶段微调：

- `GroundAreaEffect`: 地面区域基础数据，例如半径、剩余时间、总时长。
- `GroundAreaSource`: 来源 caster / ability。
- `GroundAreaTag`: 区域语义标签，例如 `Oil`、`BurningGround`。
- `GroundAreaBuffData`: 区域内单位应获得的 Buff 描述。
- `GroundAreaPeriodicDamageData`: tick 间隔、每 tick 伤害、目标过滤。
- `GroundAreaReactionData`: 反应输入标签、输出区域标签、转换规则。
- `GroundAreaExpired`: 区域过期标签。

### Core systems

- `GroundAreaLifetimeSystem`: 推进区域持续时间，过期后打标。
- `GroundAreaBuffSystem`: 查询区域内单位，施加/移除区域 Buff。
- `GroundAreaPeriodicDamageSystem`: 按 tick 查询范围内单位并创建 `DamageRequest`。
- `GroundAreaReactionSystem`: 消费火焰命中等反应请求，触发 `Oil -> BurningGround`。
- `GroundAreaCleanupSystem`: 清理过期区域以及由区域产生的长期 Buff。

### EffectSpec extension

候选新增 step：

- `LineSearch`: 起点、终点或方向 + 长度 + 宽度 + filter。
- `ConeSearch`: 方向、距离、角度 + filter。
- `CreateGroundArea`: 在目标点创建地面区域。
- `ReactGroundArea`: 对命中的地面区域发出反应请求。

也可以把 `LineSearch` / `ConeSearch` 设计为通用 `ShapeSearch`，实现阶段再比较。

## Ability Mapping

### 凝固汽油

期望表达：

1. 投掷汽油桶到目标点。
2. 到达后创建 `Oil` ground area，持续 10 秒，半径 R。
3. 区域内地面单位获得 `napalm_slow` Buff，`MoveSpeed -20`。
4. 单位离开区域或区域过期后，Buff 移除。

### 喷火

期望表达：

1. 沿施法者朝目标点方向做 `LineSearch`。
2. 命中的单位受到 100 点火焰伤害。
3. 同一线段检测命中的 `Oil` ground area。
4. 命中的 `Oil` 区域转换为 `BurningGround`。
5. `BurningGround` 持续 5 秒，每 1 秒对范围内单位产生 10 点 `DamageRequest`。

## Risks / Trade-offs

- [风险] 新模型过早泛化为完整元素系统。  
  [缓解] 第一阶段只实现地面区域 + `Oil -> BurningGround` 反应。

- [风险] 区域 Buff 与普通 Buff 清理关系混乱。  
  [缓解] 通过 `AuraBuffLink` 类似关系或专用 link 标记区域来源，区域过期时只清理自己创建的 Buff。

- [风险] `EffectSpec` step 继续膨胀。  
  [缓解] 保持 step 为数据描述；复杂运行时协作交给系统和请求，不在 builder 里写行为。

- [风险] 地面区域视觉与语义混在一起。  
  [缓解] 语义区域只依赖 ECS；视觉由后续 Native/Effect execution 派生。

## Migration / Phasing

1. OpenSpec 审核通过。
2. 增加最小组件与系统，先支持地面区域生命周期。
3. 支持区域 Buff，完成 `凝固汽油` 慢速样板。
4. 支持 line search step，完成 `喷火` 直线伤害。
5. 支持区域反应与周期伤害，完成 `Oil -> BurningGround` 点燃样板。
6. 更新 `Projects/test/Scripts/Template/Ability.cs`，添加真实可运行模板。
7. 构建 `War3Frame` 与 `Projects/test`，必要时补集成验证。

## Open Questions

- 地面单位筛选是否已有稳定组件表达，还是第一阶段只用 `TargetFilter` + 自定义 filter 占位。
- `MoveSpeed` 已在 `War3Frame/Src/Components/Combat.cs` 注册，可作为凝固汽油减速 Buff 的目标属性；仍需确认单位初始化是否为该属性创建 Attr entity。
- 线形喷火的方向应由 target point 推导，还是引入明确朝向组件。
- 区域反应是使用通用 reaction table，还是先写 `Oil + Fire` 专用最小实现。


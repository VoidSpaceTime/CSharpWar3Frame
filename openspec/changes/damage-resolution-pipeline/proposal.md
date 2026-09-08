# 提案：战斗结算管线（伤害可注册计算 + 护甲/魔抗减伤 + 暴击激活 + 无敌拦截伤害与控制）

## 元信息

- **状态**：已批准（用户 2026-09-08 批准；四项决策 + 拦截点方案 B 均已确认）→ 已实施（2026-09-08，见 summary.md）
- **等级**：full
- **变更 ID**：damage-resolution-pipeline
- **日期**：2026-09-08（修订 2026-09-08：确认 RNG 走 War3 同步、无敌复用 Invulnerable 叠加态、免疫命中仍发事件、无敌免疫控制纳入本次）
- **复盘强度**：R2 Targeted（结算契约 + 跨系统协作；不升 R3：非安全敏感、非架构级基础设施替换）
- **请求来源**：用户启动"战斗数值与结算层完善"（GAS 对标方向）；此前差距盘点（见 OpenViking 存档 2026-09-08）确认此为优先级 #1。

## 0. 基本信息

- **目标一句话**：把当前"裸扣血"的 `DamageResolveSystem` 升级为带**无敌拦截（伤害+控制免疫）+ 可注册伤害计算器（内置暴击判定 → 护甲/魔抗减免）+ 落地后钩子（吸血/护盾预留扩展点）**的完整结算管线，同时激活 Armor/MagicResist/CritChance/CritMultiplier 这些"死属性"。
- **默认实施后审查强度**：`R2 Targeted`
- **命中的审查升级触发器**：公共结算契约；伤害/治疗/死亡核心业务流程；多系统跨边界协作（Damage→Trigger→Kill→Native）
- **最终实施后审查强度**：`R2 Targeted`
- **Oracle 可用性与 `R1` 回退方式**：`R2` 视角以代码审查 + 测试证据支撑；技术准确性复核优先 Oracle，不可用时记录等价回退
- **完整 `review-work` 授权来源**：无

### 0.1 工件矩阵

- 本 change 为 `full`：`proposal.md`、`design.md`、`tasks.md`、`specs/damage-pipeline.md`

### 0.2 总结深度矩阵

- 实施完成后写完整 `summary.md`，覆盖改动范围、全局影响、验证、风险与后续。

---

## 1. 分级判定

### 1.1 为什么是这个等级

- 影响范围：`War3Frame/` 内 Components（Damage 数据契约）、Helpers（新增注册表 + 属性激活）、Systems（DamageResolveSystem 重构 + 新增计算/钩子系统）；`Projects/` 的 test 验证场景与既有模板引用 DamageEvent 的地方需同步核对。
- 风险等级：中高。伤害是核心业务流程，改动会影响所有技能/平A/Trigger 的伤害路径；需保证默认行为（无任何护甲/暴击来源时）与现状一致，避免数值回归。
- 可逆性：中。核心是"裸扣→管线化"，可通过保留默认 Calculator 还原；但 DamageBase/DamageRequest 数据契约会扩展。
- 是否跨项目：`War3Frame/` 运行时为主；`Projects/*` 示例/验证受影响（编译与行为），`War3Frame.Generator/`、`FrameBuild/`、`CSharpWar3Frame/` 不受影响。
- 是否改公共契约：**是**。DamageRequest/DamageEvent 数据契约扩展、新增 DamageCalc 注册表与结算上下文。

### 1.2 升级触发器检查

- [x] 涉及 `War3Frame/` 与其他项目联动（`Projects/` 编译与验证受影响）
- [ ] 涉及 `War3Frame.Generator/` 输出或契约
- [ ] 涉及 `FrameBuild/`、构建链路或发布流程
- [ ] 涉及 `CSharpWar3Frame/` 入口行为
- [x] 涉及 `Projects/` 示例或集成验证行为（test 场景引用 DamageEvent 需同步）
- [x] 涉及公共 API / 数据结构 / 配置契约（Damage 系组件扩展）
- [ ] 涉及架构边界、目录结构、依赖关系重组

### 1.3 实施后审查升级触发器

- [x] 公共 API、生成器输出、配置格式、构建链或发布契约
- [ ] 持久化、迁移、数据兼容性或数据丢失风险
- [ ] 性能回归、资源泄漏、实时性或大规模数据影响（伤害管线在战斗高频路径，需避免每跳 GC/分配）
- [x] 多系统、多项目或跨边界状态协作

**结论**：命中契约 + 跨系统，`R2 Targeted` 即可，不触发 `R3`。

### 1.4 工具授权与回退

- [ ] 未因 `R3` 自动启用完整 `review-work`
- [ ] 完整 `review-work` 仅当用户明确要求全面复盘/完整 QA/指定工具时启用

---

## 2. 背景 / Why

### 2.1 现状核实（代码证据，2026-09-08）

伤害链路当前为一条极简路径，缺少所有中间修饰：

```
DamageRequest ──> DamageResolveSystem(order 125) ──> finalDamage = max(0, request.damage)
                 ──> AttributeHelper.ModifyCurrent(target, Health, -finalDamage)
                 ──> 创建 DamageEvent (+TriggerEventMarker) ──> remaining<=0 → UnitHelper.KillUnit
                 ──> 删除 request 实体
```

证据：
- `War3Frame/Src/Systems/Ability/AbilityEffectSystems.cs` L869-906：`DamageResolveSystem` 为 `QuerySystem<DamageRequest>`，OnUpdate 内 `finalDamage = MathF.Max(0f, request.damage.damage)` 后直接 `ModifyCurrent`。**无任何减伤/免伤步骤**。
- `War3Frame/Src/Helpers/AttributeHelper.cs` L172-186：`ModifyCurrent` 仅做 `current += delta; Clamp(0, finalValue)`，无护盾/吸收概念。
- `War3Frame/Src/Components/Attribute/Combat.cs`：注册了 `Armor`/`MagicResist`/`CritChance`/`CritMultiplier` 等属性。
- **全仓唯一读取战斗属性的系统是 `AttackSimulationSystem`(order 124) L73 读 `AttackDamage`**。`Armor`/`MagicResist`/`CritChance`/`CritMultiplier` **没有任何消费方**，是"死属性"——能被 Buff/物品贡献，但结算时从不生效。
- `DamageType` 枚举已分 Physical/Magical/Real，`DamageSrc` 分 Melee/Ranged/Skill，但 `DamageResolveSystem` 不区分它们。
- 无敌/免疫现状：`ControlType.Invulnerable` 存在但 `UnitControlNativeSystem` 中**原生动作整段 TODO 注释掉**（L57-72），且伤害结算完全没有无敌/免疫前置检查（收到伤害直接扣血）。`Invulnerable` 作为属性叠加态（finalValue>0 即生效）存在。
- RNG 约束：`TriggerConditionRegistry.cs` L13 注释明确"注册表内禁止使用 Random/DateTime 等非确定性源，保证锁步一致"。**用户确认：随机数一律走 War3 同步 RNG `JassApi.GetRandomInt/GetRandomReal`**（引擎保证全端同种子），不自建确定性 RNG。该调用无句柄、无表现副作用、纯读同步随机状态，登记为 Native 分层例外（薄封装，不承载长期语义）。

### 2.2 为什么按 GAS 形态而不是 xlik Flow 链

GAS（UE）中伤害 = Instant GameplayEffect 对 Health 属性施加修正，核心扩展单位是**可注册的 ExecCalc（执行计算）**：捕获源/目标属性快照 → 内聚跑公式（暴击/护甲/抗性/类型克制）→ 输出最终值。复杂逻辑在一个 calc 内聚，免疫用 GE 前置 Tags 检查拦截，落地后触发 PostGameplayEffectExecute 钩子（吸血/死亡/伤害数字）。

xlik 用 `Flow("damage")` 多步骤顺序注册。但本框架的伤害路径是**单一主流程**（每次伤害都跑同一套：免疫检查→暴击→减免→扣血→事件→死亡），用"一个主 DamageCalculator + 钩子"比"多步骤链"更贴 GAS、更可控，且天然支持"护甲公式可切换"。

### 2.3 设计目标

1. 默认无任何修饰来源时，行为与现状完全一致（`max(0, damage)` 后直扣），避免回归。
2. 激活 `Armor`（对 Physical）/`MagicResist`（对 Magical）/`CritChance`/`CritMultiplier`。
3. 护甲减伤公式可注册可切换（内置 War3 公式与 Dota2 式两种，默认 War3），Real 伤害无视抗性。
4. 伤害计算器按 `damageType`/`damageSrc` 或自定义 key 可注册（ExecCalc 映射）。
5. **无敌拦截（Invulnerable 叠加态 finalValue>0）**：
   - 伤害侧：不扣血、直接结算 0，仍发 `DamageEvent`（`isImmune=true`），不 KillUnit。
   - 控制侧：**免疫大部分控制**——`ControlHelper.GetEffectiveValue` 读取侧压制五类控制（Stun/Silence/NoAttack/Root/CrackFly 返回 0）；控制 Buff 落地但无效，无敌结束若未到期自动恢复。与既有免疫机制（StunImmunity 等）同构。
6. 为护盾吸收（落地前）与吸血/反射/溅射/格挡（落地后）预留钩子扩展点，本提案不实现。

---

## 3. 变更范围 / What

### 3.1 新增

- `Components/Damage.cs`：扩展 `DamageBase`/`DamageRequest`/`DamageEvent`（增加 `isCrit`、`mitigatedAmount`、`isImmune` 等上下文字段；设计见 spec/design）。
- 新增 `DamageContext`（一次伤害结算的流转上下文：源/目标/类型/来源/暴击标记/中间值）。
- 新增 `IDamageCalculator` / `DamageCalculatorRegistry`（按 damageType/damageSrc/自定义 key 注册计算器；内置默认计算器）。
- 新增无敌拦截判定：伤害侧并入结算系统；控制侧在控制 Buff 施加路径统一检查（Invulnerable finalValue>0 忽略新施加，不驱散已生效控制）。
- 新增 `ArmorFormula` 注册表（`War3`/`Dota2` 双实现 + 默认 War3），支持切换。
- 新增结算钩子点骨架：`DamagePostProcess`（吸血/反射预留，本提案只留注册位）。
- 暴击 RNG：薄封装 `War3 RNG`（走 `JassApi.GetRandomInt/GetRandomReal`），不新增确定性 RNG 设施。

### 3.2 修改

- `Systems/Ability/AbilityEffectSystems.cs` `DamageResolveSystem`：从"裸扣"改为走管线（拦截→calculator→减免→落地→事件→死亡）。
- `ControlHelper.GetEffectiveValue`：增加无敌读取侧压制（Invulnerable>0 → 五类控制返回 0），复用现有免疫出口。
- `Components/Combat.cs` 或相关：确认属性注册语义。
- `Projects/test`：验证场景补结算管线测试（无护甲默认一致 / 有护甲减免 / 暴击 / 无敌拦截伤害 / 无敌忽略控制施加 / Real 无视）。

### 3.3 明确不做（非目标）

- 不实现吸血/护盾/反射/溅射/格挡的实际效果（只留扩展点）。
- 不改 `HealResolveSystem`（治疗加成/减疗后续提案）。
- 不做 `DamageType` 可扩展 table（保留枚举，本提案只在枚举内分派）。
- 不引入 GameplayTag 全量系统（用现有 Invulnerable 叠加态 + BuffTag 表达）。
- **不做无敌主动驱散/净化**：无敌只"压制读取"而非删除已生效控制 Buff，无敌结束后若 buff 未到期自动恢复（与 2026-09-02"独占控制不引入免疫/递减"路线一致，免疫用既有 GetEffectiveValue 出口表达）。

---

## 4. 全局影响分析

- `War3Frame/`：Components（Damage 契约）、Helpers（DamageCalculatorRegistry/ArmorFormula/确定性 RNG）、Systems（DamageResolveSystem 重构 + 可能新增 DamagePostProcessSystem）。核心运行时。
- `War3Frame.Generator/`：无生成器契约变化，不受影响。
- `FrameBuild/`：构建链不涉及，不受影响。
- `CSharpWar3Frame/`：CLI/入口不涉及，不受影响。
- `Projects/`：test 的 TriggerValidationScenario 引用 `DamageEvent`（读 finalDamage），需核对扩展后字段兼容；demo/test 的模板若直接构造 `DamageRequest` 需确认编译兼容（字段新增为可选则无感）。

---

## 5. 设计要点

> 完整设计见 `design.md`，规格见 `specs/damage-pipeline.md`。

- **结算管线**：`DamageRequest` → ①前置检查（目标死亡/实体失效；目标 `Invulnerable` finalValue>0 → 结算 0 仍发 Event，isImmune=true，不 KillUnit）→ ②查 `DamageCalculatorRegistry` 取计算器（默认内置）→ ③计算器内部：暴击判定（War3 同步 RNG）→ 按 `damageType` 取护甲/魔抗公式 → 减免 → 输出 `DamageContext` → ④落地扣血 `ModifyCurrent` → ⑤发 `DamageEvent`（含 isCrit/减免后值）→ ⑥`remaining<=0 → KillUnit` → ⑦钩子位（吸血/反射预留，默认空）。
- **无敌拦截控制**：`ControlHelper.GetEffectiveValue` 读取出口增加 Invulnerable>0 → 五类控制返回 0（通用免疫压制，复用既有 StunImmunity 等免疫机制）；控制 Buff 落地但无效，无敌结束自动恢复。不新增施加入口拦截、不驱散已有 Buff。
- **默认兼容**：无护甲（finalValue=0）时 War3/Dota2 公式都输出 0 减免 → 与现状一致。
- **Real 伤害**：跳过 Armor/MagicResist，直接满额。
- **暴击**：Physical/Melee|Ranged 可暴击（Skill 默认不暴击，由计算器可配），暴击 roll 走 War3 同步 RNG `JassApi.GetRandomReal`（或 GetRandomInt 归一）；`CritChance`>0 时判定，命中后 `damage × CritMultiplier`。
- **免伤公式注册表**：key = `armor`（Physical）/`magicResist`（Magical），value = 公式实现；静态切换当前生效公式（全局统一，或后续按伤害类型各配）。
- **Native 分层例外登记**：暴击 RNG 直接调用 War3 原生随机（`JassApi.GetRandomInt/GetRandomReal`），属"纯读同步随机状态、无句柄/表现副作用"的一次性便利调用，按 AGENTS Native 分层例外登记；收口为薄封装（如 `War3Random`/静态方法），不承载长期语义。

---

## 6. 风险、兼容性、迁移

### 风险

- **数值回归**：护甲/暴击/无敌拦截接入后若公式或判定有误会改变既有玩法数值。缓解：默认无护甲/无暴击/非无敌来源时行为不变 + 测试锁定现状基线。
- **同步风险**：暴击 roll 必须走 War3 同步 RNG；若误用 `System.Random`/DateTime 会造成 desync。缓解：roll 点统一走 `JassApi.GetRandomReal/GetRandomInt` 薄封装；审查所有 roll 点。
- **性能**：伤害在战斗高频路径。缓解：结算上下文用 struct/池化，避免每跳堆分配；查询护甲走现有 `GetFinalValue`（已 O(relations) 或优化）。
- **事件契约破坏**：`DamageEvent` 已有 Trigger 场景读取。缓解：新增字段为追加不改既有字段语义；旧字段保持兼容。
- **无敌控制免疫影响面**：无敌期间 Stun 等读取为 0（不产生控制效果），但 Buff 仍落地计时；若现有玩法依赖"无敌期间可被晕"则会变化。属预期行为修正；测试覆盖无敌+控制场景。

### 兼容与迁移

- `DamageRequest.damage.damage` 仍是入口数值，新增字段均有默认值，`Projects/*` 现有构造无感。
- 默认 `DamageCalculator` 输出 = 旧行为 + 可选修饰，未注册护甲属性/暴击属性的单位零变化。
- 回滚：保留旧 `DamageResolveSystem` 逻辑分支或 revert commit 即可还原为裸扣。

---

## 7. 验证计划

1. **构建**：`dotnet build War3Frame/War3Frame.csproj`（0 error）+ `dotnet build Projects/test/test.csproj`。
2. **单元/场景验证**（Projects/test 场景，纯 ECS 无真实客户端）：
   - 基线：无护甲/无暴击单位受 100 伤害 → remaining = maxHP-100，Event.finalDamage=100（与现状一致）。
   - 护甲减免：目标 Armor=10（War3 公式）→ 物理伤害被减免，计算值符合公式；切换 Dota2 公式 → 减免不同。
   - 魔抗：Magical 伤害受 MagicResist，Physical 不受；反之亦然。
   - Real：无视 Armor/MagicResist 满额。
   - 暴击：CritChance 高值下多次结算出现 isCrit=true 且 finalDamage=damage×multiplier；无暴击属性时永不 crit。
   - 免疫拦截（伤害）：目标 Invulnerable>0 → Event.finalDamage=0、isImmune=true，HP 不变，不触发 KillUnit。
   - 无敌拦截控制：目标 Invulnerable>0 时 GetEffectiveValue(Stun) 返回 0（Buff 落地但无效）；无敌结束若 buff 未到期恢复生效。
   - 回归：TriggerValidationScenario 现有伤害 Trigger 仍触发，读到的 finalDamage 语义不变。
3. **代码审查核对**：roll 点唯一走 War3 同步 RNG、管线各阶段只读/只写约束、无残留"死属性"。

---

## 8. 拆分任务

> 见 `tasks.md`。粗粒度：
> 1. War3 同步 RNG 薄封装（JassApi 随机）。
> 2. Damage 数据契约扩展 + DamageContext。
> 3. Armor/MagicResist 公式注册表（War3/Dota2）。
> 4. DamageCalculatorRegistry + 默认计算器（暴击/减免）。
> 5. DamageResolveSystem 重构接入管线 + 无敌伤害拦截 + 事件扩展。
> 6. ControlHelper.GetEffectiveValue 无敌读取侧压制（Invulnerable>0 → 五类控制返回 0）。
> 7. 结算钩子扩展位。
> 8. Projects/test 验证场景 + 构建验证。

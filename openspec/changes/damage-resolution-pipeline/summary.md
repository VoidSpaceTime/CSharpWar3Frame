# 实施总结：战斗结算管线（伤害管线 + 无敌拦截伤害与控制）

**状态**：已实施
**等级**：full（R2 Targeted）
**实施日期**：2026-09-08

## 实际改动范围

### 新增文件（War3Frame/Src/）
- `Helpers/War3Random.cs`：War3 同步随机数薄封装（转发 `JassApi.GetRandomReal/GetRandomInt`）。含 `Next01Provider`（默认指向 War3 引擎 RNG；仅供本地验证替换 fake，生产必须保持默认）。登记 AGENTS Native 分层例外（无句柄、无表现副作用、纯读同步随机态）。
- `Helpers/ArmorFormulaRegistry.cs`：护甲/魔抗减免公式注册表，物理槽 + 魔法槽可分别 `SetPhysicalFormula/SetMagicalFormula` 运行时切换；内置 War3 公式（k=0.06）与 Dota2 式公式（k=0.052），默认都指向 War3。`armor=0` 输出零减免（无回归根基）。
- `Helpers/DamageCalculatorRegistry.cs`：伤害计算器注册表（ExecCalc 映射，GAS 对应物）。内置默认计算器按 `damageType` 分派：Physical/Magical 可暴击 + 减免；Real 不查减免。暴击 roll 走 `War3Random`（Melee/Ranged 可暴击，Skill 默认禁暴）。
- `Helpers/DamagePostProcessRegistry.cs`：伤害落地后钩子注册表（吸血/反射等扩展位，本提案不注册任何内置实现）。

### 修改文件（War3Frame/Src/）
- `Components/Damage.cs`：扩展 `DamageBase.calcKey`、`DamageEvent.isCrit/mitigatedAmount/isImmune`（均有默认值，存量读取方无感）；新增 `DamageContext` 流转上下文。
- `Helpers/AttributeHelper.cs`：
  1. 新增 `Invulnerable` 属性位（叠加态载体，Invulnerable>0 判定伤害/控制免疫的基础）。
  2. **修复属性 ID 撞号缺陷**（见下"实施中发现缺陷"）。
- `Helpers/ControlHelper.cs`：`GetEffectiveValue` 读取出口最前面增加无敌压制——`Invulnerable` finalValue>0 时五类控制全部返回 0（复用既有免疫机制，单点压制、无施加入口漏拦）。
- `Systems/Ability/AbilityEffectSystems.cs`：
  - `DamageResolveSystem`（order 125）重构：Query 内不再做结构变更，改为**先收集 request 快照、循环外统一结算**（修复 Friflo StructuralChangeException）；结算管线 = PreCheck（空目标/无敌拦截）→ 计算器 → 扣血 → DamageEvent → PostProcess 钩子 → 死亡判定。
  - `HealResolveSystem`（order 126）同款结构变更修复（收集后结算），事件落 `target.Store`（同 store 语义不变）。

### Projects/test
- `Scripts/Process/DamagePipelineValidationScenario.cs`：8 Phase 验证（基线裸扣一致 / War3 护甲减免 / 切 Dota2 公式 / 魔抗独立 / Real 无视 / 暴击激活 / 无敌伤害拦截 / 无敌控制读取压制）；`Program.cs` 注册。

## 实施中发现缺陷（fast 级相邻修复，未单独提案）

1. **`AttributeHelper` 属性 ID 撞号**：`AttributeHelper` 是 partial 类，`_types = new()`/`_nextId = 0` 显式字段初始化器跨文件（AttributeHelper.cs vs Combat.cs）执行顺序未定义；当 Combat.cs 的 `Register(...)` 字段先执行后被 `_nextId=0` 重置，导致 Armor 与 Stun 等属性拿到相同 ID（实测 Armor=6 且 Stun=6）。历史未暴露因为 Armor/Crit 是"死属性"从未被消费。修复：去掉两个字段显式初始化器，改用 `Register` 内 `_types ??=` 惰性初始化 + `_nextId` 连续递增，杜绝重置撞号。修复后实测 ID 唯一（Armor=6、MagicResist=7、CritChance=8、Stun=19）。
2. **`DamageResolveSystem`/`HealResolveSystem` Query 内结构变更**：原实现 `Query.ForEachEntity` lambda 内 `ModifyCurrent`（AddComponent）与 `CreateEntity` 触发 Friflo `StructuralChangeException`。历史从未暴露（无真实伤害/治疗请求触发）。修复：收集 request 快照后统一在循环外结算。

## 验证结果

- `dotnet build War3Frame/War3Frame.csproj`：0 错误（182 个存量 nullable warning，与本次无关）。
- `dotnet build Projects/test/test.csproj`：0 错误（1 个存量 warning）。
- **本地同步 runner**（临时工程，已清理）：`DamagePipelineVerify: PASS`，8 个 Phase 全部断言通过，覆盖 spec §3 验收矩阵全部行（基线一致 / War3 公式 62.5 / Dota2 公式 65.79 / 魔抗独立 / Real 满额 / 暴击 isCrit+200 / CritChance=0 不暴击 / 无敌伤害 0+isImmune / 无敌控制读取压制与恢复）。
- 属性 ID 唯一性实测通过（修复后 rel 遍历无撞号）。

## 全局影响

- `War3Frame/`：核心运行时。伤害结算契约扩展 + 结算管线化 + 无敌拦截。**有护甲/暴击/无敌的单位行为按预期变化（激活死属性）；无这些属性的单位零变化（无回归）**。
- `War3Frame.Generator/`、`FrameBuild/`、`CSharpWar3Frame/`：未涉及，不受影响。
- `Projects/`：test 新增验证场景；存量场景（TriggerValidationScenario 读 DamageEvent.finalDamage）语义不变，编译通过。

## 剩余风险与后续事项

1. **暴击 roll / 原生表现未做真实 War3 客户端验证**（本地无句柄环境，用 fake RNG 验证逻辑）。建议后续在 War3 测试客户端跑一次暴击/眩晕/无敌流程确认锁步与表现。列为非阻塞遗留（同 control-state 先例）。
2. **ArmorFormulaRegistry 为全局静态切换**：多地图/多规则并行时如需按单位差异化公式，需演进为按实体配置（当前单一全局足够）。
3. **War3 负护甲公式**采用统一平滑近似（`1 - k·armor/(1+k·|armor|)`）；若需严格还原 1.27 负护甲线性增伤表，可后续微调公式实现，不影响管线结构。
4. **护盾/吸血/反射**只留钩子扩展位，待专项提案落地。
5. `HealResolveSystem` 结构变更修复属相邻最小改动；治疗加成/减疗等语义扩展不在本提案范围。

## 归档建议

本 change 已满足"已实施"全部条件（范围完成、验证通过、summary 存在、无阻塞项）。建议在后续提交时归档至 `openspec/changes/archive/2026-09-08-damage-resolution-pipeline/`。

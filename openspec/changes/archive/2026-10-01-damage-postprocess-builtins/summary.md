# 总结：damage-postprocess-builtins

## 范围

批准依据：`.omo/evidence/gates/user-ruling-2026-10-01.md` 第 5 项（W4，含 20 伤害后处理，批准实施）。本总结不自证批准。

本变更把伤害护盾吸收、吸血、反射作为**内置处理器**接入 `DamagePostProcessRegistry`，伤害主流程不内联这三类业务分支：

- **属性来源**：`AttributeHelper` 新增 `DamageShield`、`Lifesteal`、`DamageReflect` 三个注册 ID。护盾读受击者 `DamageShield.current`（上限 `finalValue`）；吸血读攻击者 `ctx.source` 的 `Lifesteal.finalValue`；反射读受击者 `DamageReflect.finalValue`。两个比例均夹紧到 `[0, 1]`。数值不从技能表、Buff tick 或硬编码常量读取。
- **注册表两段**：`DamagePostProcessRegistry` 拆为 `absorb`（扣血前，仅内置护盾）与 `post`（伤害事件后，内置吸血、反射 + 外部扩展）。外部 `Register` 只进入 `post`，无法插入护盾吸收之前，杜绝双重吸收。
- **结算顺序**：护盾吸收 → 扣减生命 → `DamageEvent` → 吸血 → 反射 → 死亡判定。护盾在 Apply 前唯一允许写 `ctx.absorbed` 与 `ctx.finalDamage`；吸血与反射只读 ctx，只创建 `HealRequest` / `DamageRequest`。
- **契约字段**：`DamageRequest` 新增默认 `false` 的 `skipPostProcess`；`DamageEvent` 新增 `absorbed`（与既有 `mitigatedAmount` 分开报告减免与吸收）；`DamageContext` 注释更新。
- **互斥**：无敌（`isImmune`）不跑护盾/吸血/反射任何一段；实际扣血为 0 时不吸血、不反射；反射请求为 `DamageType.Real` 且 `skipPostProcess=true`，其后续结算仍可被护盾吸收、仍扣血，但不再吸血、不再反射。
- **无新增原生调用**：处理器只操作 ECS 属性与请求实体；不出现 `JassApi` / `KKApi` / `YDApi` / `DzApi`。

## 全局影响

| 区域 | 是否受影响 | 理由 |
|---|---|---|
| `War3Frame/` | 是 | `AttributeHelper` 三个属性、`Damage.cs` 两个字段、`DamagePostProcessRegistry` 两段内置处理器、`DamageResolveSystem` 调用时序。 |
| `War3Frame.Generator/` | 否 | 不新增系统、不改 `SystemRegisterAttribute` 契约、不改生成输出。 |
| `FrameBuild/` | 否 | 不改构建、模板、WE、JIT/AOT。 |
| `CSharpWar3Frame/` | 否 | 不改 CLI 参数与命令入口。 |
| `Projects/` | 是 | `Projects/Regression` 新增 `DamagePostProcessRegression`（5 个 `runtime` 场景）与 `Program.cs` 注册行；`Projects/test` 现有 `DamagePipelineValidationScenario` 不涉及新属性，作为“无属性行为不变”的既有回归继续运行。 |

## 验证

- `dotnet build War3Frame/War3Frame.csproj --nologo`：0 错误（见 `.omo/evidence/task-20-build.txt`）。
- `dotnet run --project Projects/Regression -c Release`：Executed 71; passed 71; failed 0（见 `.omo/evidence/task-20-regression.txt`）。基线 66 → 71，新增 5 项全绿。
- 新增具名场景（`runtime` 组）：`伤害后处理数值`、`反射不双重结算`、`无敌与零伤互斥`、`无属性基线不变`、`主流程无内置特例`，均 PASS。
- 既有 66 项（含 `existing/DamagePipelineValidationScenario` 的 8 个相位）仍全绿，证明未配置新属性时扣血、事件、死亡与基线一致。
- 真实 War3 客户端表现未在本回归执行；按批准范围不作为归档阻塞项，客户端验证留在本总结记录。
- 说明：本机 CSharpLanguageServer 无法解析 `Friflo` 引用并超时，`lsp_diagnostics` 输出不可信；以 `dotnet build` 与回归运行为准。

## R2 专项视角

### 1. 结算顺序与互斥

`DamageResolveSystem`（order 125）现为：PreCheck → Calculator → `RunAbsorb`（仅非免疫）→ `ModifyCurrent(Health, -finalDamage)` → `DamageEvent` → `RunPost`（非免疫且非 `skipPostProcess`）→ 死亡判定。护盾处理器读 `DamageShield.current`，吸收 `min(current, finalDamage)`，经 `AttributeHelper.SetCurrent` 回写剩余（自动夹紧 `[0, finalValue]`），并同步扣减 `ctx.finalDamage`。`DamageEvent` 同时写实际扣血与吸收量。外部 `Register` 只进 `post`，语义上不可能抢在护盾之前改本次扣血。`runtime/伤害后处理数值` 验证 100/40 → 扣 60、吸收 40，吸血 15、反射 30。

### 2. 无双重结算

反射请求携带 `skipPostProcess=true`。其结算仍走 PreCheck → Calculator → 护盾吸收 → 扣血 → 事件 → 死亡，但跳过 `RunPost`，因此不再产生吸血或二次反射；同时反射为 `Real`，不吃护甲/魔抗。`runtime/反射不双重结算` 验证：第一帧原伤害 100 产生 1 条吸血请求与 1 条反射请求；第二帧反射先被攻击者护盾吸收 30、实际扣 70，吸血请求数仍为 1、伤害请求数归 0，事件 `finalDamage=70`、`absorbed=30`。这排除了“反射再吸血/再反射”的双重结算。

### 3. 无敌与零伤边界

无敌路径在 `RunAbsorb` 之前返回，`finalDamage` 保持 0，不消耗护盾、不发治疗、不发反射；`DamageEvent` 标记 `isImmune=true`、`absorbed=0`。护盾完全吸收时实际扣血为 0，吸血与反射处理器以 `ctx.finalDamage <= 0` 提前返回，不产生任何请求。`runtime/无敌与零伤互斥` 验证两种情形；`runtime/无属性基线不变` 验证未配置三属性时扣血与事件与基线一致。`runtime/主流程无内置特例` 以源码断言固定 `AbilityEffectSystems.cs` 不含 `AttributeHelper.Lifesteal` / `DamageReflect` / `DamageShield`，且注册表不含任何原生 API 符号并暴露两段入口。

## 风险

- **Apply 前吸收与旧“钩子只读”注释不一致**：只开放 `absorb` 列表给内置护盾，外部注册进不了该段；注册表注释已写明，收窄可审计。
- **同帧多次伤害**：每次请求独立读取当前 `DamageShield.current`，先结算者先消耗护盾；同帧新建的反射请求按 `pending` 快照顺延到下一帧。
- **吸血与击杀同帧**：先发 `HealRequest`（order 126 落地）再执行死亡判定；若目标已不可治疗，按既有空目标规则丢弃。本变更不改死亡语义。
- **比例超额**：处理器把两个比例夹紧到 `[0, 1]`，不出现超额治疗或超额反射。
- **旧场景读取 `finalDamage`**：无护盾时数值不变；有护盾时 `finalDamage` 变为实际扣血，吸收量改由 `absorbed` 单独表达。
- **未验证项**：真实 War3 客户端上的护盾/吸血/反射表现未执行，仅覆盖本地 ECS 结算与请求产生。

## 回滚

移除三个内置注册与 `absorb` 列表、恢复 `DamageResolveSystem` 单段 `Run` 调用、移除 `skipPostProcess` / `absorbed` 字段写入；`DamageRequest.skipPostProcess` 默认 `false`，无持久化格式需要迁移。新属性缺省为 0，不配置的单位行为与本变更前一致。

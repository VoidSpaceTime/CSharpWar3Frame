# 提案：原生能力挂载投影评估（native-ability-projection-eval）

## 状态

- 状态：待用户裁决
- 等级：fast（纯评估，不改行为契约）
- 复盘强度：R0 Direct（代码检索 + `openspec validate`）
- 范围：只评估是否另立「ECS → 原生能力投影」实施提案；本 change 不包含实现

## Why

W5.5 要求评估原生能力挂载投影。ECS 技能链已经完整：槽位、挂载/移除、施法状态机、冷却、等级重算都在组件上推进。绑定层也有 `UnitAddAbility` / `UnitRemoveAbility` / `SetUnitAbilityLevel` 和整组 `EXGet/EXSetAbility*`，但 `War3Frame/Src` 与 `Projects/` 没有任何调用。技能实体用字符串 `templateName` 标识，没有物编 rawcode，也没有命令卡按钮。

并行提案 `native-bridge-completion` 已经把 `UnitAddAbility('ACsp')` 用作沉默的一次性控制开关，并明确不给 `Locust` / `Invulnerable` / `Invisible` / `Sorcery` 做 ability 模板。那是「借一个原版技能当控制位」，不是「把 ECS 技能栏投影到原生技能栏」。两件事不能并成一个变更。

本提案只冻结现状、候选接入点、同步模式和成本。用户裁决前不新增组件、系统、Helper 或原生调用。

## 现状

### ECS 技能是完整的，且不经过原生技能

挂载只改组件，不创建原生 ability：

- `AbilityAttachWorkflowSystem`（`War3Frame/Src/Systems/Ability/AbilitySlotSystem.cs:23-90`）消费 `AbilityAttachRequest`，写入 `AbilitySlotIndex` / `AbilityOwner` / `AbilityMountInfo`，再触发 `OnGranted`。
- `AbilityRemoveWorkflowSystem`（同文件 `:96-150`）消费 `AbilityRemoveRequest`，触发 `OnRemoved` 后摘掉槽位关系。
- `AbilitySlotHelper.AddAbilityToSlot`（`War3Frame/Src/Helpers/AbilitySlotHelper.cs:25-42`）按模板名 `CreateAbility`，再 `AttachAbilityToSlot`。模板注册表的键是字符串（`AbilityTemplateAttribute.cs:15-18`），不是四字符码。
- 施法由 `CastRequest` 进入 `CastingSystem`（`CastingSystem.cs:288`）。`AbilityState`（`Ability.cs:5-13`）含 `Ready/Casting/Channeling/Backswing/Cooldown/Ban`，全部写在 `AbilityBase.state` 上。
- 禁用与沉默走 ECS：`ControlHelper.CanCast` 挡住新施法；`AbilityState.Ban` 是组件状态。没有原生「技能按钮变灰」。

`War3Frame/Src` 里仅有的 `IssueImmediateOrder` / `IssuePointOrder` 在 `UnitNativeRequestSystems.cs:86-92`，命令串是 `move` / `stop` / `holdposition`，与技能施法无关。

### 绑定存在，业务未调用

全仓检索 `UnitAddAbility`、`UnitRemoveAbility`、`UnitMakeAbilityPermanent`、`GetUnitAbilityLevel`、`SetUnitAbilityLevel`、`EXGetUnitAbility`、`EXSetAbilityState`、`EXSetAbilityDataReal`、`EXSetAbilityDataInteger`、`EXSetAbilityDataString`：

| 符号 | 声明位置 | `War3Frame/Src` | `Projects/` |
|---|---|---|---|
| `UnitAddAbility` / `UnitRemoveAbility` / `UnitMakeAbilityPermanent` | `JassApi.cs:639-641`，函数体 `:5691-5709` | 0 | 0 |
| `GetUnitAbilityLevel` / `SetUnitAbilityLevel` | `JassApi.cs:564,567`，函数体 `:5056,5083` | 0 | 0 |
| `EXGetUnitAbility` 及 `EXSetAbility*` | `YDApi.cs:5-16`，函数体 `:41-160` | 0 | 0 |

`YDApi.YDWEUnitTransform`（`YDApi.cs:151-161`）是整段注释，其中的 `UnitAddAbility` 不会编译进调用图。

结论与实体矩阵一致：能力是纯 ECS。原生技能 API 只活在绑定文件里。

### 与 native-bridge-completion 的边界

`openspec/changes/native-bridge-completion/proposal.md` §5.2 把 `ControlType.Silence` 映射为：进入时 `UnitAddAbility(ACsp)` + `IssueImmediateOrder(..., "spellshield")`，解除时再切一次后 `UnitRemoveAbility`。同表把 `Locust` / `Invulnerable` / `Invisible` / `Sorcery` 留在空分支，理由是「依赖地图 ability，不新增 ability 模板」。

那条路径的真相仍是 `ControlState`。`ACsp` 是控制投影的实现细节，等级固定为 1，不读 `AbilityBase.level`，不进 `AbilitySlotContainer`。本评估若被纳入，不得把沉默开关改造成通用技能投影，也不得把那四个空分支收进本 change。

## 候选需求

下面是「什么时候游戏需要原生技能表面镜像 ECS」的接入点。当前代码里零处这样用；UI 技能栏走的是自绘 Frame，不读原生命令卡。

| 接入点 | 为什么需要原生表面 | 今天谁在做 |
|---|---|---|
| 原生命令卡 / 被动图标 | 选中单位时，War3 底栏要显示与 ECS 槽位一致的图标、热键、等级、冷却遮罩 | 无。`AbilitySlotIndex.hotkey`（`AbilitySlot.cs:36`）只是可选字符串，没有按钮投影 |
| 原生施法交互 | 玩家点原生技能按钮或按原生热键，应变成 `CastRequest`，而不是让 War3 自己跑技能逻辑 | 无。施法只消费 ECS `CastRequest` |
| 原生触发器 | 地图 Jass/触发器读 `GetUnitAbilityLevel`、`GetSpellAbilityId`，或依赖「单位身上有某技能」 | 无。框架技能对原生触发器不可见 |
| 禁用态 | `AbilityState.Ban`、沉默、冷却要反映到原生按钮不可点 | 无。只挡 ECS 施法入口 |
| 控制向原生技能 | 沉默、蝗虫、无敌等要用某个物编技能当开关 | **不属于本评估**。沉默已在 `native-bridge-completion` 用 `ACsp` 单独立项；其余四个保持空分支 |

若纳入，投影的最小语义是：ECS 槽位与状态是真相；原生 `abilityId` 只是代理。挂上槽写 `UnitAddAbility`，摘槽写 `UnitRemoveAbility`，等级变化写 `SetUnitAbilityLevel`，禁用写一次原生禁用请求。原生按钮被点击时，只产生已有的 `CastRequest`，不让原生技能结算伤害。

## 同步方向与模式

ECS 是唯一真相，原生技能是投影。禁止从 `GetUnitAbilityLevel` 回写 `AbilityBase.level`，否则物编、`ACsp` 和地图预置技能会覆盖框架等级。

按 `AGENTS.md` 的三模式，本域不能默认 Compare-Sync：

| 数据 | 建议模式 | 理由 |
|---|---|---|
| 槽位占用（有/无某个 rawcode） | Dirty-Driven | 改动集中在挂载/移除工作流，大部分时间不变；需要查询「当前投影了哪张槽」 |
| 等级 | Dirty-Driven | 只有升级/重建时变。`SetUnitAbilityLevel` 是离散写入，不适合每帧对比 |
| 禁用 / 冷却遮罩 | Request | 一次副作用；冷却数字仍以 `AbilityCooldownState` 为准，不把原生冷却当真相 |
| 原生按钮点击 | Request（反向，仅意图） | 点击变成 `CastRequest` 后删除。不把原生施法状态机并进 `CastState` |
| 不采用 Compare-Sync | — | 技能不是血蓝那种多路径连续量；每帧 `GetUnitAbilityLevel` 扫描既贵又会把原生状态读成真相 |

Dirty 只表示「等待同步到原生」。同帧多次挂载合并后，由单独的 Native 系统消费，挂载工作流本身不得调用 `JassApi` / `YDApi`。

## 成本与风险

| 路线 | 成本 | 风险 |
|---|---|---|
| A. 不纳入（建议） | 无代码。技能继续纯 ECS；自绘 UI 读槽位组件。控制向技能开关继续只走 `native-bridge-completion` | 低。原生命令卡、地图触发器、原生被动图标永远看不到框架技能 |
| B. 另立实施提案：只投影显示 | 为每个 ECS 模板绑定一个物编 rawcode；Native 系统按 Dirty 做 Add/Remove/Level；禁用单独 Request | 中。1.27a 没有 `BlzSetAbility*` 改图标/热键/提示，显示内容被物编钉死。模板是运行时字符串，物编是编译期四字符码，对不齐就无法投影 |
| C. 另立实施提案：显示 + 原生点击回灌 | B 的全部，再加命令卡点击 → `CastRequest`，并抑制原生技能自己的效果 | 高。双向同步。原生施法、冷却、魔法消耗会和 `CastingSystem` 抢真相。`ACsp` 若被通用投影误加/误删，会打坏沉默开关 |

1.27a 约束（`ARCHITECTURE.md` 目标版本）：

- `EXSetAbility*` 走 `War3.GetNativeFunction`。未命中返回 `0`（`Library/JassVM/Native.cs`），不抛异常；1.27a 上 YDWE 扩展函数不可用。显示投影只能依赖原版 `UnitAddAbility` / `UnitRemoveAbility` / `SetUnitAbilityLevel` 和地图里预先做好的技能。
- 原版 API 不能在运行时改图标、热键、提示。一个 ECS 模板若没有对应物编技能，就没有可投影的表面。
- `UnitAddAbility` 不返回句柄，等级用 `GetUnitAbilityLevel` 探测。同一 rawcode 在单位上只有一份。`native-bridge-completion` 已占用 `ACsp`：通用投影必须把该 id 列为保留，不得复用。
- 句柄配对：ability 不是 `CreateUnit` 那种独立句柄，不进 `HandleAdd`。配对是「Add 与 Remove 在同一 Native 系统内」。漏 Remove 会把技能留在命令卡上，直到单位销毁。

路线 B/C 都是跨 Native 与技能工作流的公共契约，实施时至少 `full`，不能塞进本评估 change。

## 建议

**建议：不纳入。** 标记：**待用户裁决**。

理由：当前没有原生命令卡、原生热键或地图触发器在读框架技能；自绘 UI 已经能读 ECS 槽位。1.27a 又不能运行时改图标和热键，投影的显示价值取决于事先做好的物编，而模板系统根本没有 rawcode 字段。纳入会把「槽位组件」和「物编技能」变成双真相，并和 `ACsp` 沉默开关抢同一个 `UnitAddAbility`。控制向的单点技能开关继续留在 `native-bridge-completion`，不升级成通用投影。

可选裁决（实施不在本 change 内）：

1. **不纳入**（建议）：本评估归档关闭（`skip_specs: true`）。不新增能力投影代码。
2. **纳入**：另立实施提案。默认只评估路线 B（显示投影，ECS 单向写出）。路线 C（原生点击回灌）必须在裁决里点名，否则不做。`ACsp` 以及 `native-bridge-completion` 的四个空分支不在该提案里重做。

## Capabilities

### New Capabilities

- 无。本 change 不新增当前系统行为。

### Modified Capabilities

- 无。`.openspec.yaml` 声明 `skip_specs: true`。不把未批准的投影语义写入 `openspec/specs/`。

## Impact

- `War3Frame/`：不受影响。不改 `AbilitySlotSystem`、`CastingSystem`、`AbilitySlotHelper`、`JassApi`、`YDApi`。
- `War3Frame.Generator/`：不受影响。不新增 `[SystemRegister]` 类型，不改技能模板生成。
- `FrameBuild/`：不受影响。不改构建、物编或 WE。
- `CSharpWar3Frame/`：不受影响。不改 CLI。
- `Projects/`：不受影响。不新增 Regression 场景。

## 验收标准

- `proposal.md` 用检索证明 `War3Frame/Src` 与 `Projects/` 没有 `UnitAddAbility` / `UnitRemoveAbility` / `EXSetAbility*` 调用。
- 写明候选接入点：原生命令卡、原生点击、地图触发器、禁用态；并写明 `ACsp` 不属于本评估。
- 同步模式按 Dirty / Request 分析，并写明为什么不用 Compare-Sync、为什么不从原生等级回写 ECS。
- 建议明确为「不纳入」，并标记「待用户裁决」。
- `openspec validate native-ability-projection-eval` 通过。
- 提交后停止。不写组件、系统、绑定调用或测试。

## 风险与开放问题

- 若产品确定要使用 War3 原生底栏而不是自绘技能栏，路线 A 不够，应在裁决里改选路线 B 或 C，而不是在本 change 里补实现。
- `AbilitySlotIndex.hotkey` 今天没有消费者。把它当成「已有原生热键」是错的。
- `native-bridge-completion` 仍是待审核提案。它落地前，`ACsp` 只是计划中的保留 id；本评估不提前实现该保留，也不改那个提案。

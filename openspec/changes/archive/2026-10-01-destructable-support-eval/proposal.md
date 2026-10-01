## 状态

- 状态：待审核
- 等级：light（纯评估；不改行为契约）
- 复盘强度：R0 Direct（仅文档与 `openspec validate`）
- 范围：只评估 destructable 域是否值得纳入框架。本 change 不包含实现、不写 tasks、不写 spec delta。

## 背景

Todo 25（W5.1）要求回答：War3 可破坏物（destructable）要不要进入 ECS。实体矩阵结论是「无 ECS 接线」。本提案只冻结现状、候选用法、成本和建议，供用户裁决。

裁决前不创建组件、系统、Helper、事件桥或测试。若裁决为纳入，必须另立实施提案；本 change 不预写实施任务。

## 现状证据

绑定存在，运行时没有调用方。

| 事实 | 位置 | 含义 |
|---|---|---|
| 句柄类型 | `War3Frame/Library/Api/JassType.cs:103-116` `JDestructable : JWidget` | 只有三个构造函数，无行为 |
| 创建 | `JassApi.cs:4064-4097` `CreateDestructable` / `CreateDestructableZ` / `CreateDeadDestructable` / `CreateDeadDestructableZ` | 返回新 `JDestructable`，调用方不存在 |
| 杀死 / 删除 | `JassApi.cs:4104-4115` `RemoveDestructable` / `KillDestructable` | 直接 `CallNative`，无 `HandleRemove` |
| 生命 / 显示 / 动画 | `JassApi.cs:4118-4252` | 生命、无敌、枚举、坐标、动画、遮挡高度、名称均有绑定 |
| 事件读取 | `JassApi.cs:2977-2990`、`3694-3697`、`3777-3780`、`4255-4258`、`5455-5458` | Filter / Enum / OrderTarget / SpellTarget / Trigger / Rally 均未使用 |
| 死亡事件与 widget 生命 | `JassApi.cs:3867` `TriggerRegisterDeathEvent(JWidget)`；`4038-4045` `GetWidgetLife` / `SetWidgetLife`；`Blizzard.cs:803` `EVENT_WIDGET_DEATH` | 绑定存在，无注册点 |
| 攻击命令 | `JassApi.cs:5928-5939` `IssueTargetOrder*` 目标是 `JWidget` | 可指向可破坏物，框架未发该命令 |
| 非 1.27 扩展 | `DzApi.cs:33`、`381-383` `DzDestructablePosition` | 1.27 基线不能依赖它做位移 |
| 索引注释 | `NativeEntityIndex.cs:10-11` | 注释预留「有 `DestructableNative` 是可破坏物」；该组件不存在 |

检索结论（2026-10-01，`ECS-Framework`）：

- `War3Frame/Src` 中 `Destructable` 只命中 `NativeEntityIndex.cs:10-11` 的注释。没有 `DestructableNative`、创建/销毁系统、生命周期系统或事件实体。
- `Projects/` 无匹配。
- `CreateDestructable`、`RemoveDestructable`、`KillDestructable`、`GetTriggerDestructable`、`DzDestructablePosition` 的方法体只出现在 `Library/Api`。

对照已落地的单位模式（本提案不改这些文件）：

- 创建：`UnitCreateNativeSystem.cs:30-32` 在 `CreateUnit` 后相邻 `HandleAdd` + `NativeEntityIndex.Register`。
- 销毁：`UnitRemoveNativeSystem.cs:57-60` 在 `RemoveUnit` 前相邻 `Unregister` + `HandleRemove`。生命周期系统只推进 phase，不直接调原生。
- 事件：`NativePlayerEventBridge.cs:32-70` 只桥接 `EVENT_PLAYER_UNIT_ATTACKED`，反查失败即丢弃；没有 widget/destructable 死亡桥。
- 伤害：`Damage.cs:34-39` 的 `DamageRequest.source/target` 都是 `Entity`。没有可破坏物实体时，结算管线无法把树、门、箱子当目标。

## 候选需求

以下都是可能用法，不是本 change 的实施范围。

1. **运行时创建的可破坏物生命周期。** 业务写 `DestructableCreateNativeRequest`，Native 系统调用 `CreateDestructable*`，随后 `HandleAdd` + `Register`，回写 `DestructableNative`。死亡用 `KillDestructable`，移除用 `RemoveDestructable` 前 `Unregister` + `HandleRemove`。阶段推进与原生副作用分开，对齐单位生命周期。
2. **地图预置可破坏物接入。** 枚举或触发器拿到已有句柄后登记，不重复 `CreateDestructable`。与单位预置接入（计划 Todo 32(a)）同类，但当前没有可破坏物枚举消费方。
3. **死亡 / 受击事实。** `TriggerRegisterDeathEvent` 加 `GetTriggerDestructable`，反查后发独立 `DestructableDiedEvent`（带 `TriggerEventMarker`）。监听者不得删事件实体。
4. **与战斗联动。** 单位 `IssueTargetOrder` 攻击可破坏物，或 ECS 伤害结算在目标是可破坏物时改写 `SetDestructableLife` / `KillDestructable`。现有 `DamageRequest` 只认 `Entity`，不能直接复用。
5. **显式不做。** 不把可破坏物并进单位属性、Buff、控制或施法状态机。它没有玩家、技能、物品和移动命令。`DzDestructablePosition` 不作为 1.27 位移方案。

## 成本与风险

- **爆炸半径（若另立实施提案）：** 主要在 `War3Frame/`：新组件、1 个创建 Native 系统、1 个销毁 Native 系统、可选死亡事件桥、可选战斗目标分支。`SystemRegisterAttribute` 会让 `War3Frame.Generator/` 的生成注册多两个系统，但不改生成器本身。`FrameBuild/`、`CSharpWar3Frame/` 不受影响。`Projects/` 只在批准实施后补场景。
- **句柄配对：** 创建后必须 `HandleAdd`，销毁前必须 `HandleRemove`，并与 `NativeEntityIndex` 成对。可破坏物与单位、物品、特效共用 handle id 空间；漏注销会让反查串到错误实体。死亡（`Kill`）与移除（`Remove`）不是同一动作，不能合成一次 `HandleRemove`。
- **1.27 API：** 创建、杀死、删除、生命、无敌、矩形枚举、死亡触发器均在 1.27 绑定内。没有「可破坏物受到伤害」的专用事件；受击要么轮询 `GetDestructableLife`，要么只在 ECS 自己造成伤害时记账。坐标读取是 `[R]`（`GetDestructableX/Y`），位移依赖 `DzDestructablePosition`，超出 1.27 基线。
- **与现有战斗 / 生命周期：** `NativePlayerEventBridge` 只处理玩家单位被攻击。可破坏物死亡必须走 `TriggerRegisterDeathEvent(JWidget)`，不能塞进 `EVENT_PLAYER_UNIT_*`。伤害管线假定目标有单位生命组件；直接把可破坏物送进 `DamageRequest` 会误伤单位结算。
- **预置物规模：** 地图树、岩石可能是数百到数千个。若全量建成 ECS 实体并每帧 Compare-Sync 生命，成本高于单位同步。默认应只接入运行时创建的，或显式标记的交互物。
- **本 change 的风险：** 无代码风险。唯一风险是把评估误读成已批准实施。

## 建议

**建议：不纳入当前框架范围。** 绑定保留为 API 层可用函数，不新增 ECS 域。

理由：

- 没有任何调用方、示例或测试需要可破坏物。注释里的 `DestructableNative` 只是索引设计预留。
- 最小可用闭环并不小：创建、句柄配对、死亡与移除分相、死亡事件、以及一条不污染单位伤害管线的目标路径。
- 预置树木若全量接入，会把低频场景对象变成持续同步成本。
- 地图上的树和门继续由 War3 自己处理即可。框架单位打它们不需要 ECS 真相，除非玩法要监听「这棵树被推倒」。

若用户否决本建议、决定纳入，另立实施提案，且第一阶段只做：运行时创建 + 句柄配对 + `Kill`/`Remove` 分相 + 死亡事件。不接伤害管线，不枚举全图预置物，不使用 `DzDestructablePosition`。

## 待裁决项

**待用户裁决。** 本提案不代用户选择。

- 选项 A：不纳入。归档本评估，不另立实施提案。
- 选项 B：纳入。另立实施提案，范围限于上一节的第一阶段。
- 选项 C：维持现状并挂起。不归档、不实施，等出现具体玩法再开。

未收到用户回复前，本 change 停在审核门。

## Capabilities

### New Capabilities

- 无。纯评估，不新增当前系统行为契约。

### Modified Capabilities

- 无。`.openspec.yaml` 设置 `skip_specs: true`。没有可归档的行为 delta。

## Impact

- `War3Frame/`：仅作证据。不改 API、组件、系统、Helper 或事件桥。
- `War3Frame.Generator/`：无影响。未新增 `[SystemRegister]` 类型。
- `FrameBuild/`：无影响。
- `CSharpWar3Frame/`：无影响。
- `Projects/`：无影响。未加示例或回归场景。
- `openspec/specs/`：无变更。

## 验收标准

- `proposal.md` 覆盖背景、现状证据、候选需求、成本与风险、建议、待裁决项。
- 证据含文件与行号，并区分「绑定存在」与「运行时未使用」。
- 建议明确，同时标为「待用户裁决」，不把建议写成已批准。
- `openspec validate destructable-support-eval` 通过。
- 不创建实现代码、`tasks.md` 实施清单或 spec delta。

> 裁决（2026-10-01，用户确认）：不纳入；变更归档关闭。

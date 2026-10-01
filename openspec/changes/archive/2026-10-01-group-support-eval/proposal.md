## 状态

- 状态：待审核
- 等级：light（纯评估；不改行为契约）
- 复盘强度：R0 Direct（仅文档与 `openspec validate`）
- 范围：只评估原生单位组（`JGroup`）要不要进入框架。本 change 不包含实现、不写 tasks、不写 spec delta。

## 背景

Todo 28（W5.4）要求回答：War3 原生单位组要不要接线。绑定层已有 `JGroup` / `CreateGroup`，但运行时没有调用方。同名的 `GroupHelper` 已经在用，它是 ECS 空间查询，不是原生组。

本提案只冻结这个区分、互操作缺口、成本和建议。裁决前不创建组件、系统、句柄包装或测试。若裁决为纳入，必须另立实施提案；本 change 不预写实施任务。

## 现状证据

### 两套「组」不是同一件事

| | `GroupHelper`（已在用） | `JGroup`（未用） |
|---|---|---|
| 位置 | `War3Frame/Src/Helpers/GroupHelper.cs:10` | `War3Frame/Library/Api/JassType.cs:178-191` |
| 持有什么 | `SpatialGrid Grid`（`:13`，cellSize 256）和一份复用的 `List<Entity>`（`:16`） | 三个构造函数，包一个原生句柄，无行为 |
| 查询 | `FindInCircle` `:36`、`FindInRect` `:63`、`FindInLine` `:91`、`FindInCone` `:121`、`FindNearest` `:143` | `CreateGroup` / `GroupEnum*` / `ForGroup` / `FirstOfGroup` |
| 筛选 | `ApplyFilter` `:178` 走 `TargetFilterRegistry`，对象是 ECS `Entity` | `JBoolExpr` + `GetFilterUnit` / `GetEnumUnit`，对象是原生 `unit` |
| 时间 | 调用当下的快照；网格由 `SpatialGridSystem` 每轮清空重建 | 引擎里的可变单位组句柄，直到 `DestroyGroup` |
| 副作用 | 无。调用方自己决定下一步 | 可对组下命令（`Group*Order*`），注释写明最多 12 个单位 |

`GroupHelper` 不调用 `JassApi`，不持有 `JGroup`，也不登记句柄。把它改成原生组，或在它上面再包一层 `CreateGroup`，都会把已经落地的 ECS 查询换成只认识原生单位的句柄。

索引写入点：`SpatialGridSystem`（`War3Frame/Src/Systems/SpatialGridSystem.cs:13`，`[SystemRegister(SystemKind.Interval, 99)]`）每轮 `GroupHelper.Grid.Clear()`（`:19`）后，把带 `Position` 的实体 `Insert`（`:23`）。`openspec/specs/target-selection/spec.md:26-31` 已要求空间索引在位置写回之后、范围查询之前更新。

已有调用方（全部走 `GroupHelper`，没有一个走到 `JGroup`）：

- `AbilityEffectSystems.cs:565` `FindInCircle`（区域效果）
- `AbilityEffectSystems.cs:646` `FindInLine`（线形效果）
- `AbilityEffectSystems.cs:1258` `FindInCircle`（地面区域 Buff）
- `AbilityEffectSystems.cs:1309` `FindInCircle`（地面区域周期伤害）
- `Projects/Regression/DomainRegression.cs:363` `FindInCircle`（`spatial-search`）

检索（2026-10-01，`ECS-Framework`）：`JGroup`、`CreateGroup`、`DestroyGroup`、`ForGroup`、`FirstOfGroup`、`GroupEnum*`、`GroupAddUnit` 的方法体只在 `War3Frame/Library/Api/JassApi.cs` 与 `KKApi.cs`。`War3Frame/Src` 与 `Projects/` 零匹配。

### 原生绑定存在，但没有消费方

| 事实 | 位置 | 含义 |
|---|---|---|
| 句柄类型 | `JassType.cs:178-191` `JGroup : JAgent` | 只有构造函数 |
| 创建 / 销毁 | `JassApi.cs:2335-2348` `CreateGroup` / `DestroyGroup` | 返回新句柄；`DestroyGroup` 前没有 `HandleRemove` |
| 成员 | `JassApi.cs:2354-2375` `GroupAddUnit` / `GroupRemoveUnit` / `GroupClear` | 注释写明不影响单位本身 |
| 枚举 | `JassApi.cs:2377-2445` `GroupEnumUnitsOfType/OfPlayer/InRect/InRange/Selected` 及 Counted 变体 | 结果写进传入的组，不返回 ECS 实体 |
| 遍历 | `JassApi.cs:2523-2535` `ForGroup` / `FirstOfGroup` | 回调或「取走第一个」；`ForGroup` 注释禁止在组动作里 `等待` |
| 成员测试 | `JassApi.cs:5465` `IsUnitInGroup` | 无调用方 |
| 存档句柄 | `JassApi.cs:7333` `SaveGroupHandle`、`:8023` `LoadGroupHandle` | 无调用方；本评估不把它当成存档需求 |
| 非 1.27 扩展 | `KKApi.cs:3299` `DzGroupGetCount`、`:3308` `DzGroupGetUnitAt`、`:3545-3559` `DzQueueGroup*OrderById` | 1.27 基线不能依赖下标读取或队列命令 |

对照已落地的原生调用（本提案不改这些文件）：

- 单位创建：`UnitCreateNativeSystem.cs:30-32` 在 `CreateUnit` 后相邻 `HandleAdd` + `NativeEntityIndex.Register`。组没有对应系统。
- 移动命令：`UnitNativeRequestSystems.cs:86-92` 对**单个** `UnitNative.unit` 发 `IssuePointOrder` / `IssueImmediateOrder`。没有 `GroupPointOrder` / `GroupImmediateOrder` / `GroupTargetOrder`。
- 句柄引用：`HandleRefHelper.cs:5-13` 的 `HandleAdd` / `HandleRemove` 只包 `War3.AddHandleReference` / `SubHandleReference`。组若要长期持有，必须走同一配对；今天没有登记点。

## 原生组互操作需求

以下是「只有原生组能做、现有 ECS 查询做不到」的用法。当前代码里零处这样用。它们不是本 change 的实施范围。

1. **把一份 ECS 单位名单交给只接受 `group` 的原生函数。** 1.27 里这主要是 `GroupImmediateOrder` / `GroupPointOrder` / `GroupTargetOrder`（`JassApi.cs:2451-2517`）。框架今天的移动是一单位一条 `MoveNativeRequest`，不需要把多名单位塞进一个组再下命令。组命令还有「最多 12 个单位」的引擎限制，不能当成批量移动的真相层。
2. **读取引擎已经建好的组。** 例如玩家当前选择（`GroupEnumUnitsSelected`，`:2442`）或地图/JASS 脚本留下的组句柄。框架没有选择同步，也没有 JASS 脚本回调把 `group` 交进来。`GetSpellTargetUnit`（`JassApi.cs:3797`）返回的是单个 `unit`，不是组；施法目标已由 ECS 的 `EffectTargetInfo` 表达。
3. **用引擎枚举代替空间索引。** `GroupEnumUnitsInRange`（`:2406`）只看见原生单位，看不见没有 `UnitNative` 的效果点、地面区域或纯 ECS 实体，也不能复用 `TargetFilter`。`GroupHelper` 已经覆盖圆、矩形、线段、扇形。这条不是缺口。
4. **显式不做。** 不把 `GroupHelper` 改名为别的东西来「避开混淆」——那是无关重命名。不把 `ForGroup` 回调当成业务系统。不把 `DzGroupGetUnitAt` 当成 1.27 可用的遍历手段。不把 `SaveGroupHandle` 当成持久化方案（持久化另见 W5.7）。

若将来真有第 1 或第 2 类调用方，形态应是一次性的 Native 翻译，而不是新的组领域：

```text
业务系统写出 Entity 名单或收到外部 group 句柄
  -> Native 系统 CreateGroup / GroupAddUnit，或只读已有句柄
  -> 调那一个原生函数，立刻 DestroyGroup（自建的组）
  -> 需要回写时发已有的 XxxEvent / XxxOutcome，不把 JGroup 留在组件上当真相
```

长期把 `JGroup` 存在组件里，会让原生句柄成为成员名单的 owner，和「ECS 持有真相」冲突，还要处理单位死亡、移除与 `HandleRemove` 的先后。

## 成本与风险

- **本 change：** 无代码风险。唯一风险是把评估读成已批准实施，或把 `GroupHelper` 误当成未接线的原生组。
- **若另立实施提案：** 最小闭环仍落在 `War3Frame/`：一个 Native 系统负责 `CreateGroup` → `GroupAddUnit` → 原生调用 → `DestroyGroup`，创建后 `HandleAdd`、销毁前 `HandleRemove`。`War3Frame.Generator/` 只因新的 `[SystemRegister]` 多注册一个系统，不改生成器。`FrameBuild/`、`CSharpWar3Frame/` 不受影响。`Projects/` 只在批准实施后补场景。
- **双真相：** `GroupHelper` 的名单是「这一刻网格里的实体」。`JGroup` 的名单是引擎单位，含死亡、不含隐藏（`ForGroup` 注释），且不含无句柄实体。两套同时当成员真相，筛选结果会分叉。
- **句柄：** `CreateGroup` 的返回值与单位、物品、特效共用 handle id 空间。漏 `HandleRemove` 会让 `NativeEntityIndex` 反查串实体。组的销毁点必须只有一个。
- **回调：** `ForGroup` 在原生回调里跑。回调内不能推进施法、直接改 ECS 结构，或再调会重入的 Jass。`FirstOfGroup` 会把单位从组里取走，不能当成只读枚举。
- **1.27：** 创建、销毁、枚举、`ForGroup`、组命令都在 1.27 绑定内。按下标读成员要 `DzGroupGetUnitAt`，超出基线。组命令的 12 单位上限是引擎行为，框架包一层也消不掉。
- **没有调用方时的成本：** 现在做这层，只会得到无人消费的句柄包装，并和已有 `target-selection` 规格抢「谁负责找目标」。

## 建议

**建议：不纳入。** 绑定留在 API 层。`GroupHelper` 继续作为空间查询入口，不改名、不改成原生组。

理由：

- 范围查询、区域效果、线形效果、地面区域已经走 `GroupHelper`，并有 `spatial-search` 回归。
- 原生组相对它的增量只有「把名单交给 group 参数」和「读引擎里已有的组」。这两类今天没有调用方。
- 组命令有 12 单位上限，不适合作为批量移动或批量施法的真相。
- 长期持有 `JGroup` 会制造第二份成员名单，并带上句柄配对和回调重入。

若用户否决本建议、决定纳入，另立实施提案，且第一阶段只做：某个已经点名的原生函数所需的一次性组（创建、加入、调用、立刻销毁、句柄配对）。不持久保存组，不替换 `GroupHelper`，不使用 `DzGroupGetUnitAt`。

## 待裁决项

**待用户裁决。** 本提案不代用户选择。

- 选项 A：不纳入。归档本评估，不另立实施提案。
- 选项 B：纳入。另立实施提案，范围限于上一节的第一阶段，且必须先点名那个原生函数。
- 选项 C：维持现状并挂起。不归档、不实施，等出现具体互操作调用方再开。

未收到用户回复前，本 change 停在审核门。

## Capabilities

### New Capabilities

- 无。纯评估，不新增当前系统行为契约。

### Modified Capabilities

- 无。`.openspec.yaml` 设置 `skip_specs: true`。没有可归档的行为 delta。

## Impact

- `War3Frame/`：仅作证据。不改 `GroupHelper`、`SpatialGridSystem`、`JassApi`、`KKApi` 或任何 Native 系统。
- `War3Frame.Generator/`：无影响。未新增 `[SystemRegister]` 类型。
- `FrameBuild/`：无影响。
- `CSharpWar3Frame/`：无影响。
- `Projects/`：无影响。未加示例或回归场景；`spatial-search` 保持现状。
- `openspec/specs/`：无变更。`target-selection` 继续只约束 ECS 空间索引。

## 验收标准

- `proposal.md` 覆盖背景、现状证据、原生组互操作需求、成本与风险、建议、待裁决项。
- 写明 `GroupHelper` 与 `JGroup` 的区别，并给出文件与行号。
- 建议明确为「不纳入」，同时标为「待用户裁决」，不把建议写成已批准。
- `openspec validate group-support-eval` 通过。
- 不创建实现代码、`tasks.md` 实施清单或 spec delta。

> 裁决（2026-10-01，用户确认）：不纳入；变更归档关闭。

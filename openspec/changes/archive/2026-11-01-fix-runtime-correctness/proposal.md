# 提案：运行时正确性缺陷修复（fix-runtime-correctness）

## 元信息

- **Change ID**：`fix-runtime-correctness`
- **提案等级**：`full`
- **状态**：`已实施`（2026-10-23 用户批准：范围 A–F 全部；D-1 打断旧施法；D-2 进入冷却；D-3 按属性区分实例）
- **日期**：2026-10-23
- **目标一句话**：修复全仓审查中已复现或已逐行确认的运行时缺陷（施法/移动、tick 中断异常、死亡语义、Buff/属性贡献、效果管线），每项配回归用例。
- **请求来源**：用户要求“检查是否有其他问题并解决落地”。
- **默认实施后审查强度**：`R2 Targeted`（跨多系统协作 + 公共组件新增字段）
- **完整 `review-work` 授权来源**：无
- **真实 War3 客户端验证**：**非阻塞**。本变更不新增、不改写任何原生调用语义；唯一触及 Native 层的是 `EffectNativeSystem` 的空目标守卫（只减少调用）。原生行为相关问题全部列入“延后”，不在本变更修复。

### 0.1 工件矩阵

`proposal.md`、`design.md`、`tasks.md`、`specs/{cast-move-workflow,runtime-execution,target-selection,ui-control,item-ground}/spec.md`

## 1. Why

主 tick 链路（`War3Init.cs:25-31` → `TimedSystemRoot.Update`）没有异常隔离。任一系统抛异常，本 tick 后续系统全部跳过，包括 `EventCleanupSystem(132)`；而抛异常的请求实体通常未被删除，下一 tick 重放再抛。所以“会抛异常”的缺陷等于整局停摆。

另一类缺陷不抛异常，但让玩法永久错误，例如单位一次被打断后再也不能走位施法、复活后第一次受击必死。

现有 78 项回归全绿，但没有一项覆盖下列缺陷。

## 2. What Changes（按组，证据见 design.md §1）

| 组 | 编号 | 缺陷 | 证据 |
|---|---|---|---|
| **A 施法/移动** | A1 | `MoveSystem` 顶替判定方向反了：原生单位移动中写入新 `MoveCommand`，新命令被当成“被顶替”删除 | 探针 `[A] outcome=Overridden@2 cmdAlive=False` |
| | A2 | `MoveOutcome` 不按 token 对齐：遗留 `Arrived` 让超距施法原地开始吟唱 | 探针 `[D2] unitX=0 targetX=2000 range=100 castPhase=Casting` |
| | A3 | `CancelCastMovement` 残留 `MoveOutcome`/`MoveContinuation`，单位永久不能走位施法，物品 companion 引用永不释放 | 探针 `[1]`/`[1b]` |
| | A4 | `CastRequestSystem` 不看已有 `CastState`，覆盖施法后旧技能永久卡在 `Casting` | 探针 `[C] A.state=Casting castState=False` |
| | A5 | 死亡施法者照常完成前摇/引导；死亡/入池单位的 `MoveCommand` 永不结束 | 探针 `[2]`；静态 |
| | A6 | 效果已提交（引导/后摇）后被打断直接回 Ready，不进冷却；与 `ItemCompanionCastCleanup` 语义矛盾 | 静态，两处代码对照 |
| | A7 | 范围内直接施法写入请求原始坐标，而非解析后的目标坐标 | 静态 `CastingSystem.cs:50-56` vs `113-114` |
| | A8 | 施法中途移除技能不清理施法状态；技能被删除后 `CastingSystem` 读已删实体 | 静态 |
| **B tick 中断异常** | B1 | `RemoveByKey` 在 Query 循环内 `AddComponent`，必抛 `StructuralChangeException` 且每 tick 重抛 | 探针 `[F] StructuralChangeException` |
| | B2 | 无 `source` 的近战/远程伤害 NRE | 探针 `[5b]` |
| | B3 | `UISyncResolveSystem` 无 `finally`，处理器异常后同步请求每 tick 重放 | 静态，对照 `ItemUseSystem` |
| | B4 | `CreateNativeEffect` 对已删除附着目标 NRE，本轮已建句柄泄漏 | 静态 |
| **C 死亡/生命语义** | C1 | 没有 `Health` 属性的存活单位被 0 伤害击杀 | 探针 `[5]` |
| | C2 | 尸体回血/可被治疗：`Alive=false` 且 `HP>0` | 探针 `[4]`/`[4b]` |
| | C3 | 复活、出池不恢复 ECS `Health.current`；原生满血而 ECS 为 0，复活后第一次受击必死 | 静态，`TimerTaskSystem.cs:117-126` / `UnitRebornNativeSystem.cs:32` |
| | C4 | 光环作用于尸体，死亡 owner 仍发光环 | 静态 |
| | C5 | 闪电链会跳到尸体 | 静态 `AttackSimulationSystem.cs:154` |
| | C6 | `finalValue` 下降后 `current` 不夹紧，护盾过期后仍能多吸收一次（违反 `damage-postprocess` 已有契约） | 静态 |
| **D Buff/属性贡献** | D1 | 永久 + 可叠层 Buff 不能叠层 | 探针 `[3]`/`[3b]` |
| | D2 | 同 `buffId` 施加到不同属性时复用并覆盖旧 Buff，新属性没有 Buff | 探针 `[6]` |
| | D3 | 移除带属性贡献的技能后，贡献永久残留 | 探针 `[E] granted=1.75 afterRemove=1.75` |
| **E 效果/弹道** | E1 | 曲线弹道每帧重算起点：抛物线 5 倍速到达且停在 z=1294 | 探针 `[G] expected≈100 actual=20 endZ=1294.6` |
| | E2 | `LineSearch` 父 effect 也自己结算，主目标挨两次伤害 | 静态 `CanSettle` 只拦 `AreaSearchData` |
| | E3 | 子 effect 目标坐标清零，按目标点的视觉/地面区域落在 (0,0) | 静态 |
| | E4 | 效果管线 10 处硬编码 `Game.Store`，独立 Store 下请求写错 Store | 静态 |
| | E5 | 地面周期伤害丢弃溢出时间；引导最后一跳在边界被跳过 | 静态 |
| **F 维护** | F1 | `GroupHelper` 静态共享 buffer，过滤回调内再查询会破坏遍历 | 静态 |
| | F2 | `War3NativeBootstrap.cs:45` 残留 `Console.WriteLine` | 静态 |
| | F3 | `UIInput.Pump` 无输入根时不清队列，积压事件被误判为双击 | 静态 |
| | F4 | 同 tick 两个拾取请求抢同一槽位：第二个挂载抛异常，物品变成隐形 | 静态 |
| | F5 | integration 回归固定 3 分钟超时，冷启动 NuGet 恢复时假失败 | 本次已实际发生 |

### 2.1 待用户决定（design.md §3 有推荐值）

1. **D-1 施法中收到新施法请求**：推荐“打断当前施法再开始新施法”（War3 新命令即打断）；备选“拒绝新请求”（与 `ItemUseSystem` 一致）。
2. **D-2 效果已提交后被打断**：推荐“进入冷却”（与 `ItemCompanionCastCleanup` 一致；前摇未提交时仍回 Ready、不进冷却，保持 `cast-interrupt-and-validation` 既有行为）；备选“回 Ready”。
3. **D-3 同 `buffId` 不同目标属性**：推荐“按 (buffId, 目标属性) 区分实例；`RemoveBuff`/`HasBuff` 仍按 buffId 作用于全部同名实例”；备选“检测到时抛异常”或“保持现状只写文档”。

## 3. Non-Goals（延后，见 design.md §6）

- 同类效果步骤互相覆盖（`poison_bite` 的减速丢失）：需要改 `EffectSpec → ECS` 映射模型，单独提案。
- UI 原生输入（鼠标坐标整数除法、Y 轴方向、多面板输入根、指针事件玩家来源）：依赖 DzAPI 语义，需真实客户端验证，单独提案。
- 原生映射（Root/NoPath 的 `SetUnitPathing` 极性、`IsImmobilized` 未接入移动、复活原生有效性、打断不发 Stop、附着特效早于原生单位、特效 Z、`ApplySilence` 误删地图自带技能）：均需客户端验证，且部分与现有 spec“映射保持不变”冲突。
- 设计取舍：`UnitTemplate` 写死 `hfoo`、击杀者已死仍得经验、`RemoveUnit` 在 Death 阶段静默无效、`BuffApplyRequest` 无 `maxStacks`、地面 Buff 只作用敌人、主 tick 不做异常隔离。
- 架构项：统一请求校验层、统一生命状态查询、系统 order 命名常量、放开 `WFGEN004`、`AuraSystem` 性能。

## 4. Capabilities

### New Capabilities
- `cast-move-workflow`：移动结果 token 对齐、顶替语义、施法替换、打断冷却、死亡施法者与施法中移除技能。

### Modified Capabilities（仅 ADDED requirement）
- `runtime-execution`：伤害/治疗/资源的死亡与缺属性语义、复活恢复生命、Buff 实例身份与永久叠层、技能贡献回收、效果管线结算与 Store 归属、曲线弹道、周期计时、请求消费异常安全。
- `target-selection`：光环与闪电链排除死亡单位；空间查询可重入。
- `ui-control`：无输入根时清空队列。
- `item-ground`：同批拾取请求不重复占用同一物品或槽位。

## 5. Impact（全局影响分析）

| 区域 | 影响 |
|---|---|
| `War3Frame/` | **受影响**。`Src/Systems/{Ability,Unit,Time,Item,UI,Native,Attribute}`、`AuraSystem`、`Src/Helpers/{AttributeHelper,BuffHelper,GroupHelper,AbilitySlotHelper,UnitHelper}`、`Src/Components/{MoveCommand,Ability/AbilityEffect}`、`Src/Core/UI/UIInput.cs`。公共 API 只做加法：`MoveContinuation.commandToken`、`ProjectileRuntimeState.startX/startY/startZ/startInitialized`。不删除、不改名任何公共成员。 |
| `War3Frame.Generator/` | **不受影响**。不新增/删除/改 order 任何 `[SystemRegister]` 系统，生成的注册表不变。 |
| `FrameBuild/` | **不受影响**。不改构建、发布、CLI 契约。 |
| `CSharpWar3Frame/` | **不受影响**。 |
| `Projects/` | `Regression/` 新增回归文件并注册；`IntegrationRegression` 超时由 3 改 10 分钟。`test/`、`demo/` 不改；`existing` 组全部场景须保持通过。 |

## 6. 验收标准

1. `dotnet build War3Frame/War3Frame.csproj -c Debug` 0 error，警告数不增加。
2. 新增回归覆盖 A1–A8、B1–B4、C1–C6、D1–D3、E1–E5、F1、F3、F4，每项至少一个先红后绿的断言。
3. `dotnet run --project Projects/Regression -c Debug` 全部通过（现有 78 项 + 新增项）。
4. `openspec validate fix-runtime-correctness` 通过。
5. R2：技术准确性、QA/回归覆盖两个独立视角各有证据与 verdict。

## 7. 风险

- **行为变更面广**：多数修复改变的是“错误行为”，但 A4/A6/D2 涉及语义选择，已列为待决项。
- **A1 改变原生命令下发**：修复后新移动命令会真正下发（以前被吞掉）。这是修复目标，原生调用种类不变。
- **E4 机械替换 10 处**：生产只有一个 Store，回归用独立 Store 验证。
- **回滚**：每组改动相互独立，可按组 revert；新增字段默认值保持旧行为（`commandToken=0` 视为不校验）。

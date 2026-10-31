# 设计：fix-runtime-correctness

## Context

本变更只修复审查中**已复现**或**逐行核对确认**的缺陷。复现用的临时控制台探针放在 `%TEMP%\opencode\wfprobe*`，引用 `War3Frame.csproj`，不进仓库；实施时这些探针逐条转成 `Projects/Regression` 下的正式用例。

约束：

- ECS 是唯一真相；不新增原生调用，不改原生调用参数。
- 不新增、不删除、不改 order 任何 `[SystemRegister]` 系统（生成器输出不变）。
- 公共 API 只加不减。
- Friflo 3.6.0：Query 循环内增删组件/Tag 抛 `StructuralChangeException`；泛型 `CreateEntity(...)` 与 `DeleteEntity()` 不在检查范围（已对照 XML 文档与 `UnitControlNativeSystem` 现状）。

## 1. 证据索引

| 编号 | 复现输出（修复前） |
|---|---|
| A1 | `[A] first=1 second=2 cmdAlive=False outcome=Overridden@2` |
| A1→施法 | `[B] castAlive=False moveCmd=False abilityState=Ready`（原生单位移动中发起超距施法被立即取消） |
| A2 | `[D2] after 1 tick: unitX=0 targetX=2000 range=100 castPhase=Casting` |
| A3 | `[1] after interrupt: castState=False outcome=True continuation=True`；`[1b] castStateSurvives=False` |
| A4 | `[C] A.state=Casting B.state=Ready castState=False` |
| A5 | `[2] dead unit cast: castStateRemains=False abilityState=Ready` |
| B1 | `[F] StructuralChangeException: within query loop` |
| B2 | `[5b] NullReferenceException: entity is null. id: 0` |
| C1 | `[5] zero-damage on unit without Health: alive=False` |
| C2 | `[4] corpse health after 1s regen=10 alive=False`；`[4b] after heal=60` |
| D1 | `[3] permanent stackable stacks=1`；`[3b] timed stackable stacks=2` |
| D2 | `[6] healthBuffValue=5 manaBuffCount=0` |
| D3 | `[E] manaRegen granted=1.75 afterRemove=1.75 modifiers=1` |
| E1 | `[G] expected≈100 steps; actual=20 endZ=1294.6` |

其余各项的证据是代码行对照，在 tasks.md 每项旁边写明了文件和行号。

## 2. 关键设计

### A 组：施法与移动

**A1 顶替方向。** `MoveSystem` 发现 `MoveExecutionState.commandToken != MoveCommand.commandToken` 时，说明**旧命令**已被替换。修复后：

- 用 `execution.commandToken`（旧 token）发出 `Overridden`；
- 只移除陈旧的 `MoveExecutionState`；
- 新命令在同一轮继续做到达/移动判定，未下发则正常下发。

死亡单位（`UnitLifeState.isAlive == false`）按 `Interrupted` 处理，与受控同一分支（A5）。

**A2 token 对齐。** `MoveContinuation` 新增 `public int commandToken`。`CastRequestSystem`（走位分支）与 `UnitHelper.MoveToTask` 写入同一 token。`MoveToCastSystem` / `MoveToTaskSystem` 只消费 `outcome.commandToken == continuation.commandToken` 的结果；不匹配视为陈旧，只移除 `MoveOutcome`，continuation 继续等待。`commandToken == 0`（外部旧代码手工构造的 continuation）不校验，保持旧行为。

**A3 统一清理。** 新增 `CastMovement.Clear(unit, ability)`（internal，`CastingSystem.cs` 内）：移除匹配该技能的 `MoveContinuation`；若 `MoveCommand.reason == CastingAbility`，同时移除 `MoveCommand`、`MoveExecutionState`、`MovingTag`；只有移除了施法后续、或单位已没有其他后续时才移除 `MoveOutcome`（不吞掉移动任务的结果）。`CancelCastMovement`、Arrived 分支、失败分支、孤儿分支全部走它。

`MoveToCastSystem` 的查询由 `CastState + MoveOutcome + MoveContinuation` 改为 `CastState`（只处理 `MovingToCast` 或仍挂着施法后续的单位）：受控、死亡、外部打断标记不必等移动结果即可取消；无结果时执行原有的“目标移动超过 100 跟随”分支。该分支在旧查询下不可达（查询要求有结果，而所有结果类型都被前两个分支处理），实施后可达，回归 `A3-cast-move-follows-target` 覆盖。`CastState` 已不在 `MovingToCast` 时清理遗留的施法后续。

R2 复核补充的两个取消条件（回归 `A3-replaced-cast-move-cancels-cast`，已逐一确认关掉守卫时失败）：

- `MovingToCast` 但单位上没有本技能的施法后续（例如被 `UnitHelper.MoveToTask` 替换）：取消施法。这是修复前就存在的残留路径。
- 没有可消费的结果，且本次移动命令已不存在或 token 已变（外部直接写入新 `MoveCommand`）：取消施法。不加这一条时，token 对齐会让施法永久停在 `MovingToCast`。MoveSystem 移除本命令时一定写入同 token 结果，结果分支先处理，因此不会误判。

`CancelCastMovement` 同时移除 `CastInterruptedTag`，与 `InterruptCast` / `InterruptChannel` 一致。

**A4 施法替换（决策 D-1）。** `CastRequestSystem` 先按现有条件校验新请求；**只有校验通过**才处理已有 `CastState`：

| 当前阶段 | 处理 | `OnInterrupted` | 技能状态 |
|---|---|---|---|
| `MovingToCast` | `CancelCastMovement` | 否（与现有取消一致） | Ready |
| `Casting`（未提交） | `InterruptCast` | 是 | Ready |
| `Channeling` | `InterruptChannel` | 是 | 冷却或 Ready（A6） |
| `Backswing` | `InterruptCast` | 是 | 冷却或 Ready（A6） |

校验失败时丢弃请求，已有施法不受影响。同技能重复请求在 `Casting/Channeling/Backswing` 时仍因 `state != Ready` 被拒；`MovingToCast` 时技能仍是 Ready，会取消旧走位并重新开始。

**A5 死亡施法者。** 内部判定 `UnitHelper.IsDead(unit)`：**有** `UnitLifeState` 且 `isAlive == false`。没有生命周期组件的实体（现有测试大量使用）视为非死亡，避免误伤。应用点：`CastRequestSystem` 校验、`CastingSystem`/`ChannelingSystem` 循环开头（先于 Tag 判断）、`MoveToCastSystem`、`MoveSystem`（A1）。

**A6 打断冷却（决策 D-2）。** `InterruptCast`：`cast.effectCommitted` 为真（后摇）时调用 `AbilityHelper.EnterCooldownOrReady`，否则保持 Ready。`InterruptChannel` 一律 `EnterCooldownOrReady`（进入引导前效果已提交）。冷却为 0 时结果仍是 Ready，因此 `CastValidationScenario` 第 3 阶段（冷却 0）不变。

**A7 坐标。** `StartCasting` 改为接收解析后的 `targetX/targetY`。

**A8 施法中移除技能。** `AbilitySlotHelper.RemoveAbilityFromSlot` 与 `AbilityRemoveWorkflowSystem.RemoveAbility` 在拆除 `AbilityOwner` 之前调用 `ItemCompanionCastCleanup.Cleanup(ability)`。该方法按技能实体身份清理请求、施法、引导与走位，本身与物品无关；补一行注释说明。另在 `CastingSystem`/`ChannelingSystem` 开头对 `cast.ability.IsNull` 做防御：只移除单位上的施法组件，不触发行为效果。

### B 组：tick 中断异常

- **B1**：`RemoveLinkedEffects` 先收集再 `Destroy`，查询改用 `owner.Store`。
- **B2**：`AttributeHelper.GetAttr` / `GetAllAttrs` 对 `entity.IsNull` 返回空。所有读取类入口随之空安全，`GetFinalValue(default)` 返回 0。
- **B3**：`UISyncResolveSystem` 每条请求 `try { Dispatch } finally { DeleteEntity }`，与 `ItemUseSystem` 模式一致。异常仍向上抛（不吞），但同一请求不会再重放。
- **B4**：`CreateNativeEffect` 在读附着目标前检查 `attachment.target.IsNull`，为空时走点特效分支。原生调用种类不变。

### C 组：死亡与生命

- **C1**：`DamageResolveSystem` 只在目标**有** `Health` 属性且 `remaining <= 0` 时 `KillUnit`。事件 `remainingHealth` 照旧。
- **C2**：`HealthSystem`、`ManaSystem` 跳过 `IsDead` 单位；`HealResolveSystem` 对 `IsDead` 目标直接删除请求，不改血、不发 `HealEvent`。
- **C3**：`TimerTaskSystem` 的 RebornWait 激活与 `UnitHelper.AcquirePooled` 回到 Alive 时，把 ECS `Health.current` 设为 `finalValue`。这与 `UnitRebornNativeSystem` 已有的“原生生命拉满”一致，ECS 不再落后于原生；随后 `UnitNativeSystem` 的 compare-sync 会发现差异并同步一次。法力不改（原生层同样不改）。
- **C4**：`AuraSystem.ShouldAffectUnit` 排除 `IsDead` 目标；owner `IsDead` 时本轮目标集为空，原有“离开范围即删除”逻辑回收其全部 Buff。owner 复活后下一轮自然重建。
- **C5**：闪电链下一跳筛选从 `TargetFilter.Enemy` 改为 `Enemy | Alive`。
- **C6**：`AttrCalculationSystem` 重算后若 `current > finalValue`，夹到 `max(0, finalValue)`。只向下夹，不抬高 current；Health/Mana 原本就有每 tick 的上限夹紧，结果不变。孤儿属性回收判据使用**夹紧前**的现值，夹紧本身不让属性在同一轮变成可回收（`AttributeLifecycleValidationScenario` 第 1 阶段“现值非 0 保留”契约保持；实施中首轮全量回归曾因此失败，已修正）。

### D 组：Buff 与属性贡献

- **D1**：`HandleExistingBuff` 先处理 `AddStack` / `RefreshAndStack`（永久 Buff 只叠层、不刷时长），再保留原来的“非 Replace 永久 Buff 返回既有”。对 `maxStacks == 1` 的 Buff，叠层是空操作，结果与旧行为相同。
- **D2（决策 D-3）**：`ApplyBuff` 查重改为 `(buffId, spec.attrTypeId)`：只在目标属性实体的入边中查找。`RemoveBuff` 删除该单位上**所有**同 buffId 实例；`HasBuff` / `GetBuffRemainingTime` / `GetBuffStacks` 仍返回首个匹配。对现有用法（同 buffId 只用于一个属性），行为不变。
- **D3**：两条技能移除路径在删除 `AbilityOwner` **之前**直接调用 `ModifyHelper.RemoveModifiersFromSource(ability)`，并移除未消费的 `AbilityAttrApplyRequest`。不再依赖 `AbilityAttrRemoveRequest`（它与 `AbilityOwner` 同时消失，查询永远匹配不到）。`AbilityAttributeRemoveSystem` 保留不动。

### E 组：效果与弹道

- **E1**：`ProjectileRuntimeState` 新增 `startX/startY/startZ/startInitialized`。曲线轨迹首帧锁定起点，此后起点固定、终点跟随目标；`totalDist` 用固定起点计算，抛物线高度相对 `startZ` 叠加（`t=1` 时回到起点高度）。Tracking/Linear 不受影响。
- **E2**：`CanSettle` 同时拦截 `LineSearchData`，与 `AreaSearchData` 对称。现有模板中 `flamethrower`（点目标）父 effect 本来就不扣血，表现不变。
- **E3**：`CreateChildEffect` 的 `targetX/targetY` 取命中目标的 `Position`，无位置时回退父 effect 的目标点。
- **E4**：`AbilityEffectSystems.cs` 内 10 处 `Game.Store` 改为相关实体的 Store（`owner.Store` / `effectEntity.Store` / `request.target.Store` / `areaEntity.Store`）。`EffectHelper`、`ItemHelper`、`UnitTemplate` 的 `Game.Store` 不在本变更范围（它们没有实体上下文，需要 API 变更）。
- **E5**：地面周期伤害 `timeSinceTick -= interval`（保留余数，每次更新最多一跳，与现有节奏一致）。引导 tick 改为按“下一跳时刻 ≤ 引导总时长”判定，使 tick 次数为 `floor(duration / interval)`，与 `runtime-execution` 中 Buff 尾段规则一致。**这是数值变化**：`duration` 为 `interval` 整数倍的引导技能会多出最后一跳。

### F 组：维护

- **F1**：`GroupHelper` 删除静态 `_tempBuffer`，每次查询使用本地列表；有筛选时原地压缩，分配次数不变。
- **F2**：删除 `War3NativeBootstrap.cs:45` 的 `Console.WriteLine`。
- **F3**：`UIInput.Pump` 在输入根为空或已销毁时仍 `Drain()` 丢弃队列。
- **F4**：`ItemPickupWorkflowSystem` 本批次记录已认领的物品与 `(owner, slot)`，重复者跳过（请求照常删除）。
- **F5**：`IntegrationRegression` 子进程超时 3 → 10 分钟。

## 3. 待决项与推荐

| 决策 | 推荐 | 理由 | 备选 |
|---|---|---|---|
| D-1 施法中新请求 | 打断当前施法再开始 | War3 新命令打断当前施法；同时修复旧技能卡死 | 拒绝新请求（与 `ItemUseSystem` 一致，改动更小） |
| D-2 效果提交后被打断 | 进入冷却 | 资源已扣、效果已触发；与 `ItemCompanionCastCleanup` 同语义 | 回 Ready（保持现状，存在免冷却连放） |
| D-3 同 buffId 不同属性 | 按 (buffId, 属性) 区分实例 | 多属性 Buff 可用；现有单属性用法不变 | 抛异常禁止；或只写文档 |

## 4. 验证计划（R2 Targeted）

1. **R0 直接验证**：`dotnet build War3Frame/War3Frame.csproj -c Debug`；`dotnet run --project Projects/Regression -c Debug`（全部组）；`openspec validate fix-runtime-correctness`。
2. **新增回归**：`Projects/Regression/RuntimeCorrectnessRegression.cs`，组名 `correctness`（可单独运行）。每项先在未修复代码上确认失败（红），再修复转绿。B4 放在 `NativeProjectionRegression` 的记录器框架中（`native-projection/deleted-attach-target`）。
3. **视角 1 技术准确性**：逐项核对修复点与 Friflo 结构变更约束、系统 order、token 生命周期。
4. **视角 2 QA/回归**：核对每个缺陷都有断言；`existing` 组 11 个场景（含 `CastValidationScenario`、`ItemCompanionAbilityValidationScenario`、`SkillPointUpgradeScenario`）全部通过。

真实 War3 客户端验证：非阻塞（见 proposal 元信息），在 summary.md 记录未执行原因与剩余风险。

## 5. 风险与回滚

| 风险 | 缓解 |
|---|---|
| A1 修复后新移动命令真正下发，原生层行为与以前不同 | 这是修复目标；原生调用种类不变；回归断言 `MoveNativeRequest` 带新 token |
| A4/A6 改变打断后的状态 | 按决策执行；冷却为 0 时结果不变；`CastValidationScenario` 回归保护前摇打断不进冷却 |
| C2 让 `isAlive=false` 的池内单位不再回血 | 池内单位本就不应回血；出池时 C3 回满 |
| D2 改变查重键 | 现有调用全是单属性；回归覆盖 Aura、刷新、叠层、控制 Buff |
| E5 引导多一跳 | 在 summary 中列为数值变化 |

回滚：各组改动互不依赖，可按组 revert；新增字段默认值（`commandToken=0`、`startInitialized=false`）等价旧行为。

## 6. 延后清单（不在本变更修复）

| 项 | 原因 |
|---|---|
| 同类效果步骤互相覆盖（`poison_bite` 丢减速） | 需改 `EffectSpec → ECS` 映射模型 |
| UI：鼠标坐标整数除法、Y 轴方向、多面板输入根、指针事件玩家来源、`UISyncBridge.Reset` 后重复注册 | 依赖 DzAPI 语义与客户端验证；只修除法而 Y 轴仍错没有意义 |
| Root/NoPath `SetUnitPathing` 极性、`IsImmobilized` 未接入移动、Hide 极性 | 与 `native-state-projection` 现有“映射保持不变”冲突，需客户端验证 |
| 复活原生有效性（`KillUnit` 后 `SetUnitState`）、打断不发原生 Stop | 依赖 War3 引擎行为 |
| 附着特效早于原生单位退化为 (0,0)、特效 Z 绝对 0 | 需原生层重试设计与客户端验证 |
| `ApplySilence` 以 level==1 判断框架添加 | 需原生验证 |
| `UnitTemplate` 写死 `hfoo` | 需新增模板字段（API 加法），单独提案 |
| 已死击杀者得经验、`RemoveUnit` 在 Death 阶段静默无效、对尸体的伤害仍触发吸血/反射 | 设计取舍，需确认 |
| `RemoveModifiersFromSource` 对技能 stat 打错脏标记、预置单位代理无 `HandleRemove` | 无现有调用点 / 泄漏有上界 |
| Linear 弹道无命中判定、速度 0 弹道常驻、地面 Buff 只作用敌人、`BuffApplyRequest` 无 `maxStacks`、helper 授予路径不触发 `OnGranted/OnRemoved`、物品使用（`ItemUseSystem`）在施法中仍拒绝而非打断（与 D-1 不一致） | 功能缺口 |
| 主 tick 无异常隔离、统一请求校验层、统一生命状态查询、order 命名常量、`WFGEN004`、`AuraSystem` O(N×M) | 架构项 |
| `FrameBuild` 的 `Clear()` / `FindFileUpwards` 有缺陷但无调用方 | 死代码，单独清理 |

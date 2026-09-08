# 提案：施法打断 + 控制校验 + 目标合法性二次确认

## 元信息

- **状态**：已批准（用户 2026-09-08 确认语义符合预期；Silence 不打断前摇吟唱、打断持续引导）→ 已实施（2026-09-08，见 summary.md）
- **等级**：light（补 design.md；跨施法状态机/控制状态/移动桥接多系统协作，含时序边界 → 需 design）
- **变更 ID**：cast-interrupt-and-validation
- **日期**：2026-09-08
- **复盘强度**：R1 Focused（技术准确性 1 视角；Oracle 可用时优先，否则等价复核并记录）
- **请求来源**：用户选择"完善技能与施法"方向中的 **施法打断+控制校验** 范围；前置 change `control-state-superposition`（已实施）与 `synthesize-pause-from-controls`（待审）提供了控制状态检测/合成基础。

## 1. 分级判定

- 影响范围：`War3Frame/Src/Systems/Ability/CastingSystem.cs`、`War3Frame/Src/Components/Ability/CastState.cs`、可能新增小系统/扩展 `CastInterruptedTag` 语义。局限 War3Frame 内施法域。
- 风险等级：中低。施法是核心流程，打断时序错误会造成"无法打断"或"误打断"；改动集中在检查点，默认无打断来源时行为不变。
- 可逆性：高。全部是新增检查点 + 触发接线，回滚即删除对应检查/监听。
- 是否跨项目：否（仅 `War3Frame/`）。
- 是否改公共契约：否。复用现有 `CastInterruptedTag` 与 `ControlStateChangedEvent`/`ControlHelper`，不新增请求组件契约；可能微调状态机阶段语义（document 说明）。

## 2. 背景 / Why（现状核实，2026-09-08 代码证据）

施法状态机已完整（前摇→生效→后摇→冷却、持续吟唱、移动施法桥接），但存在三处实质缺口：

**缺口 1：打断机制空转——`CastInterruptedTag` 无任何 AddTag 来源。**
- `CastingSystem.cs` L311 / L413、`ChannelingSystem.cs` L460-506、`MoveToCastSystem.cs` L207-210 均在检查 `unit.Tags.Has<CastInterruptedTag>()` 后执行 `InterruptCast`/`CancelCastMovement`。
- 全仓 grep：`CastInterruptedTag` 仅这 3 处读取 + `CastState.cs` 定义，**没有任何系统/Helper 添加该 Tag**。即"被打断回调链"（OnInterrupted 触发、冷却不进入、状态回 Ready）代码完备但**永远不会被触发**。
- 目标行为：单位进入眩晕/击飞等"禁止行动"控制时，正在吟唱/持续施法应被打断（符合 War3 玩家直觉与 GAS/Dota2：受控取消施法）。

**缺口 2：施法入口无视控制状态。**
- `CastRequestSystem.Process`（CastingSystem.cs L37-47）只检查 `ability.state==Ready` + `CheckCost` + 物品来源合法性，**没有查眩晕/沉默/缴械**。
- 结果：被眩晕/沉默的单位仍可发起新施法请求进入 Casting。`MoveToCastSystem` 阶段有 `IsControlled` 检查（L201）但仅在"移动到施法范围"路径生效；直接施法路径（已在范围内）无控制校验。

**缺口 3：吟唱中/引导前不复查目标合法性。**
- `CastRequestSystem` 只在发起时校验距离（+ 移动路径跟踪目标位移 L233-255），**进入吟唱后不再复查**目标是否存活/是否仍是合法目标（如目标死亡、被隐身、无敌）。
- 目标：施法在吟唱/引导期间至少应处理"目标死亡/目标变为非法"情形（具体语义见 design 决策点）。

## 3. 变更范围 / What

### 3.1 新增
- 施法打断接线：单位进入禁止行动类控制（Stun/CrackFly；语义含沉默的打断范围待 design 定）时，若正在 `Casting/Channeling` 阶段 → 添加 `CastInterruptedTag`（复用既有打断链）或直接调用打断（按 design 选型）。
- 施法入口控制校验：`CastRequestSystem.Process` 增加眩晕/沉默等检查，禁止受控单位发起新施法。
- 目标合法性复查点（design 定：吟唱完成前/持续引导 tick 前）。

### 3.2 修改
- `Systems/Ability/CastingSystem.cs`：入口校验补充控制状态；视方案在 tick 处补充受控即打断或依赖新增接线。
- `Components/Ability/CastState.cs`（或文档）：补充 `CastInterruptedTag` 语义说明（现在"谁添加、谁消费"）。
- `Projects/test`：验证场景覆盖"受控打断施法 / 受控禁止发起施法 / 目标死亡取消施法"。

### 3.3 明确不做（非目标）
- 不做公共冷却/类别冷却（单技能冷却保留）。
- 不做技能升级加点、被动技能树。
- 不做 `OnOwnerDamaged/OnDeath` 受击触发接线（枚举已存在但触发源属后续方向）。
- 不改冷却/消耗/资源语义。

---

## 4. 全局影响分析

- `War3Frame/`：施法状态机与控制状态的协作边界。核心运行时。
- `War3Frame.Generator/`：无生成器契约变化。
- `FrameBuild/`、`CSharpWar3Frame/`：不涉及。
- `Projects/`：test 场景补充；现有模板/流程语义影响：此前"受控仍可施法/施法永不被控打断"的行为将变为"受控打断"，属预期修复；若无技能依赖该旧行为则零破坏。

## 5. 设计要点

> 完整时序与选型见 `design.md`。核心开放点：
> 1. 打断接线方式：事件监听（`ControlStateChangedEvent` entered→打断） vs 施法 tick 轮询 `ControlHelper.IsIncapacitated`。
> 2. 哪些控制打断施法：Stun/CrackFly 必打断；Silence 是否打断"已开始的吟唱"（常见 MOBA：沉默只禁发起新施法，不打断已有吟唱，但打断持续引导）——需定语义。
> 3. 目标合法性复查：目标死亡→取消施法（走 OnInterrupted）；目标变敌/隐身/无敌是否取消按语义定。

## 6. 风险、兼容性、迁移

- 误打断：打断判定过宽会取消不应取消的施法。缓解：默认只对"禁止行动/施法被禁"类控制打断，清单在 design 明示。
- 时序边界：施法系统 order 20/21（Interval），控制事件在 order 46 发出。若走事件监听，打断系统 order 需在 46 后、效果结算前；若走 tick 轮询无 order 依赖但需处理同帧进入控制边界。design 选型权衡。
- 回滚：全部为检查点与接线，删除即还原；不改数据契约。

## 7. 验证计划

1. `dotnet build War3Frame/War3Frame.csproj`（0 error）。
2. `dotnet build Projects/test/test.csproj`（0 error）。
3. 本地 ECS 场景（独立 store + 系统树驱动）：
   - 单位吟唱中进入 Stun（属性叠加）→ 施法被打断，发 OnInterrupted，状态回 Ready，无生效点结算。
   - 单位受控（Stun/Silence 生效）时提交 CastRequest → 请求被拒绝，不进入 Casting。
   - 吟唱中目标死亡 → 施法取消（或按语义继续到生效点前取消）。
   - 回归：无控制状态下施法全流程不变。
4. 代码审查：打断路径唯一消费/添加点、无重复打断、目标复查不引入每 tick 额外全量扫描（仅施法单位）。

## 8. 拆分任务

> 待 design 定选型后细化。粗粒度：
> 1. 打断接线（事件或轮询）+ CastInterruptedTag 语义补注。
> 2. CastRequestSystem 入口控制校验。
> 3. 目标合法性复查点。
> 4. Projects/test 验证场景 + 构建验证。

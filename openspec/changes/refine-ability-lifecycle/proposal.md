## 基本信息

- Change ID: `refine-ability-lifecycle`
- 提案等级: `full`
- 目标一句话: 将技能生效入口从 `OnCast` 明确为 `OnEffect`，补齐前摇、持续施法、后摇与资源消耗边界。
- 请求来源: 用户要求 `OnCast` 改名为 `OnEffect`，并确认 `OnEffect` / `Channel` 判断条件通过后消耗资源，打断不做释放失败返还。

## Why

当前 `AbilitySpecBuilder.OnCast(...)` 的命名容易把“开始施法”和“技能生效”混为一谈。实际技能生命周期需要区分：

- 释放请求与移动到施法范围。
- 释放前摇 / cast point。
- 技能真正生效点。
- 持续吟唱 / channel tick。
- 释放后摇 / backswing。
- 打断与结束清理。

现有 `CastState` / `ChannelState` / `CastingSystem` 已有施法状态机雏形，但资源当前在开始施法时扣除。用户明确希望资源在 `OnEffect` / `Channel` 条件通过后才消耗，打断不再需要“释放失败返还资源”的复杂流程。因此需要正式修正技能生命周期契约。

## What Changes

计划调整：

- 将 `AbilityBehaviorTrigger.OnCast` 语义迁移为 `OnEffect`。
- 将 `AbilitySpecBuilder.OnCast(...)` 改名为 `OnEffect(...)`。
- 增加生命周期配置：
  - `CastPoint(...)`：释放前摇，生效前可被打断。
  - `Backswing(...)`：释放后摇，生效后进入硬直/收招阶段。
  - `Channel(duration, tickInterval)`：持续吟唱与 tick 频率。
- 增加行为触发边界：
  - `OnEffect(...)`：前摇完成并通过生效条件后触发。
  - `OnChannelTick(...)`：持续吟唱每跳条件通过后触发。
  - `OnInterrupted(...)`：前摇或持续吟唱被打断时触发可选清理。
  - `OnFinished(...)`：完整结束后触发可选收尾。
- 调整资源消耗时机：
  - 释放请求阶段只做资源可用性预检查，不扣资源。
  - `OnEffect` 生效前重新检查条件；通过后扣资源并触发生效效果。
  - `OnChannelTick` 每跳按配置判断是否需要扣资源；通过后触发 tick 效果。
  - 打断不做资源返还，因为未消耗或已按通过的生效/tick 消耗。
- 示例模板迁移到新命名入口。

## 非目标

- 不新增 War3 原生调用。
- 不把 Native 命令塞进施法工作流系统。
- 不重写 Projectile / Damage / Buff / GroundArea 的效果执行系统。
- 不改变 Source Generator 注册机制。
- 不在本阶段实现复杂编辑器、技能树或多资源 UI。
- 不为旧 `OnCast` 增加长期兼容 shim；若实现阶段需要短期保留以保持构建，将在任务中明确并尽快迁移调用点。

## 全局影响分析

- `War3Frame/`: 受影响。需要调整 authoring spec、builder、施法状态组件和 `CastingSystem` 资源消耗/触发时机。
- `War3Frame.Generator/`: 预期无直接影响。若新增系统仍使用现有 `SystemRegisterAttribute`，不改生成器契约。
- `FrameBuild/`: 预期无影响。
- `CSharpWar3Frame/`: 预期无影响。
- `Projects/`: 受影响。`Projects/test/Scripts/Template/Ability.cs` 需要从 `.OnCast` 迁移为 `.OnEffect`，并可补充一个 channel 示例。

## 设计要点

- 命名边界：
  - `Cast` 表示开始释放或施法过程。
  - `Effect` 表示技能真正生效点。
  - `ChannelTick` 表示持续吟唱期间的周期生效点。
  - `Backswing` 表示生效后的硬直/后摇。
- 资源边界：
  - 请求阶段允许预检查，避免明显不能释放的技能进入流程。
  - 真正扣资源只发生在 `OnEffect` 或 `OnChannelTick` 条件通过后。
  - 打断只清理状态，不做资源返还。
- ECS 分层：
  - 施法系统只推进状态、检查条件、写 effect request/entity。
  - 效果系统继续解释 `EffectSpec`，处理伤害、治疗、Buff、弹道等语义。
  - Native/Execution 层仍负责原生副作用。

## 风险、兼容性、迁移

- 风险: `OnCast` 改名破坏现有模板调用。
  - 缓解: 全仓库同步迁移到 `OnEffect`，构建验证。
- 风险: 资源消耗从开始施法移动到生效点后，当前 gameplay 行为会变化。
  - 缓解: 提案中明确这是目标行为；打断不返还资源。
- 风险: Channel tick 扣费策略不明确。
  - 缓解: 第一阶段只定义基础结构，可默认 channel 不逐跳扣费；若需要每跳扣费，使用显式配置字段。
- 风险: 后摇是否可被移动/新命令取消存在策略差异。
  - 缓解: 第一阶段把后摇作为状态字段和时间推进，不引入复杂取消策略；后续单独扩展。
- 回滚: 回退 builder/spec/系统与示例迁移，恢复 `OnCast` 和当前开始施法扣费逻辑。

## 验收标准

- `AbilitySpecBuilder.OnEffect(...)` 可替代现有 `.OnCast(...)` 表达技能生效点。
- `AbilityBehaviorTrigger` 不再把生效点命名为 `OnCast`。
- 技能可配置释放前摇、持续吟唱参数、后摇。
- 资源消耗不在开始施法时扣除，而在 `OnEffect` 条件通过后扣除。
- Channel 具备 tick 触发边界，后续可承载 `OnChannelTick(...)` 效果。
- 打断流程不执行资源返还逻辑。
- `Projects/test/Scripts/Template/Ability.cs` 使用 `.OnEffect(...)`。
- `dotnet build War3Frame/War3Frame.csproj` 通过。
- `dotnet build Projects/test/test.csproj` 通过。

## Why

当前 `EffectSpec` 适合描述一次性效果链：`Projectile -> AreaSearch -> Damage / Heal / Buff`。这能覆盖普通弹道、范围伤害、范围 Buff，但无法自然表达以下能力：

- 地面残留物：例如凝固汽油在目标点留下一片油污区域。
- 持续区域效果：例如区域存在 10 秒，期间持续影响进入区域的单位。
- 周期性结算：例如点燃后每秒造成 10 点伤害，持续 5 秒。
- 条件联动：例如喷火接触油污后，把油污转为燃烧区域。
- 直线/扇形命中语义：底层 `GroupHelper.FindInLine` / `FindInCone` 已存在，但 `EffectSpec` 未暴露对应 step。

因此继续把这些能力硬塞进 `AbilityHelper.SetEffectSpec` 会让模板看起来能表达，实际运行时却缺少地面区域、周期结算和反应模型，容易误导后续技能开发。

## What Changes

本提案引入一个运行时能力模型：`ground-area-effect-model`。

它将新增或明确以下语义：

- ground area effect entity：地面持续区域实体，持有位置、半径、生命周期和语义标签。
- area aura / area buff：区域存在期间对进入范围的单位施加或移除 Buff。
- area periodic damage：区域存在期间按固定间隔生成伤害请求。
- area reaction：技能或效果命中某类 ground area 后触发转换，例如 `Oil + Fire -> BurningGround`。
- line / cone search step：把已有 `GroupHelper` 线形/扇形搜索能力纳入技能效果链。

## Capabilities

### New Capabilities

- `ground-area-effect-model`: 定义地面持续区域、区域周期结算、区域 Buff、区域反应和线形/扇形效果搜索的 ECS 语义。

## Impact

- `War3Frame/`: 直接影响技能效果模型、Ability effect systems、区域查询、Buff/Damage request 生成和可能新增的地面区域系统。
- `War3Frame.Generator/`: 预期无直接影响；需确认新增系统仍通过现有 `[SystemRegister]` 注册即可。
- `FrameBuild/`: 预期无直接影响；若后续模板引用新视觉资源，再由具体实现提案说明资源影响。
- `CSharpWar3Frame/`: 预期无直接影响；CLI/build 入口不变。
- `Projects/`: 后续会新增或更新 test 技能模板，用 `凝固汽油` 与 `喷火` 作为验收样板。

本次变更只创建 OpenSpec 工件，不进入运行时代码实现。

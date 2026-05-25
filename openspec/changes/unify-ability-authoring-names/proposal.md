## 基本信息

- Change ID: `unify-ability-authoring-names`
- 提案等级: `full`
- 目标一句话: 统一技能配置命名与职责边界，形成 `AbilitySpec` / `AbilityBehavior` / `AbilityEffect` 三层 authoring 模型。
- 请求来源: 用户确认“可以听取你的建议，但是要统一函数名以及类名”。

## Why

当前技能系统已经有 `AbilityBase`、`EffectSpec`、`EffectSpecBuilder`、`AbilityEffectHelper`、`AbilityBehavior` 相关枚举迹象，以及后续新增的 ground area / line search 能力。随着 `治疗之鸟` 这类被动、周期、目标选择、弹道结束后治疗的技能进入设计范围，仅继续扩展 `EffectSpecBuilder` 会让它承担“触发时机、目标选择、流程等待、效果结算”所有职责，最终退化成另一种脚本语言。

因此需要先统一命名与分层：

- `AbilitySpec`: 完整技能定义与模板 authoring 入口。
- `AbilityBehavior`: 技能何时触发、如何重复、如何选目标、如何等待异步流程。
- `AbilityEffect`: 触发后真正产生的伤害、治疗、Buff、GroundArea 等结算效果。
- `AbilityValue`: 统一表达常量、技能数值、caster/target 属性、比例公式。

这不是一次单纯重命名，而是公共 authoring 契约和后续被动技能模型的基础，因此按 `full` 级处理。

## What Changes

本提案仅定义命名、职责和迁移策略；不在提案阶段直接实现运行时代码。

计划新增或规范以下能力：

- 顶层 authoring 命名：`AbilitySpec`、`AbilitySpecData`、`AbilitySpecBuilder`。
- 流程层命名：`AbilityBehaviorSpec`、`AbilityBehaviorData`、`AbilityBehaviorBuilder`、`AbilityBehaviorSystem`。
- 效果层命名：逐步将 `EffectSpec` / `EffectStepSpec` / `EffectSpecData` / `EffectSpecBuilder` 迁移到 `AbilityEffectSpec` / `AbilityEffectStepSpec` / `AbilityEffectSpecData` / `AbilityEffectSpecBuilder`。
- 数值层命名：引入 `AbilityValue` 或等价类型，表达 `Constant`、`AbilityStat`、`OwnerAttr`、`CasterAttr`、`TargetAttr`、`Formula`。
- 目标选择命名：明确 `TargetSelector`、`TargetFilter`、`TargetMetric` 的边界。
- 行为入口：支持表达 `OnCast`、`OnGranted`、`OnRemoved`、`Repeat`、`SearchCircle`、`SearchLine`、`SelectLowest`、`Projectile`、`OnArrive`、`Do`、`StopWhenOwnerDead` 等流程语义。

## Capabilities

### New Capabilities

- `ability-authoring-model`: 定义技能 authoring 的三层命名、职责边界、行为流程与效果结算关系。

## Impact

- `War3Frame/`: 直接影响技能配置 API、EffectSpec 命名、行为层组件/系统、公式和目标选择模型。
- `War3Frame.Generator/`: 预期第一阶段不改生成器；若后续需要模板自动注册新的 builder attribute 或生成别名，再单独说明。
- `FrameBuild/`: 预期无直接影响；构建流程不变。
- `CSharpWar3Frame/`: 预期无直接影响；CLI 入口不变。
- `Projects/`: 示例模板需要逐步迁移到统一命名，尤其是 `Projects/test/Scripts/Template/Ability.cs`。

## Non-Goals

- 不在本提案中引入 Lua 脚本桥接或热重载。
- 不立刻移除旧 `EffectSpec` API，避免破坏现有示例和使用方。
- 不把 `AbilityBehaviorBuilder` 设计成任意脚本执行器。
- 不改变 War3 Native 分层；行为层只写 ECS 意图和请求。

## 验证计划

- 文档阶段：检查 `proposal.md`、`design.md`、`tasks.md`、`spec.md` 内容完整。
- 实现阶段：构建 `War3Frame/War3Frame.csproj` 与 `Projects/test/test.csproj`。
- API 阶段：至少迁移或新增一个主动技能示例、一个被动/周期技能示例，证明命名可读。
- 分层阶段：确认新增行为系统不直接调用 War3 native。

## 基本信息

- Change ID: `introduce-experience-leveling-system`
- 提案等级: `full`
- 目标一句话: 为 Unit / Ability / Item 引入统一经验系统，经验系统只负责经验曲线、获得经验、升级判定，并在等级变化后添加 `LevelStatDirty`。
- 请求来源: 用户确认按“经验系统两个责任：升级曲线、获得经验后判断升级并添加等级 dirty 标签”的边界生成提案。

## Why

当前 Unit 必须具备经验系统，Ability / Item 也存在成长型需求，例如技能熟练度、成长型物品、杀敌数驱动升级等。如果每个模块各自实现经验和升级逻辑，会导致：

- Unit / Ability / Item 的成长规则不一致。
- 杀敌数、倍率经验、技能/物品成长无法统一表达。
- 升级后属性重算容易和 `LevelStatRebuildSystem` 边界混淆。

因此需要引入通用经验模型。经验系统只处理“经验 -> 等级变化”，属性变化继续由 `LevelStatRebuildSystem` 处理。

## What Changes

计划新增或规范：

- 通用经验数据组件，例如 `ExperienceData`。
- 通用经验曲线规格，例如 `ExperienceCurve` / `ExperienceCurveKind`。
- 获得经验请求，例如 `ExperienceGainRequest`。
- 经验处理系统，例如 `ExperienceSystem`。
- 支持 Unit / Ability / Item 三类对象绑定经验数据。
- 升级后更新对象当前等级，并添加 `LevelStatDirty`。

## 非目标

- 不在经验系统中计算属性、技能数值或物品属性贡献。
- 不处理 Buff 最终合成。
- 不处理装备属性聚合。
- 不调用 War3 native。
- 不负责击杀判定、任务完成判定或奖励来源判定。
- 不在第一阶段实现复杂多经验条或外部编辑器。

## 依赖关系

本提案依赖或对接：

- `introduce-level-stat-rebuild`: 经验系统升级后的出口是 `LevelStatDirty`。
- Unit / Ability / Item 当前等级组件或字段：经验系统只修改等级，不直接重算属性。

## 影响范围

- `War3Frame/`: 新增经验组件、曲线规格、请求和系统；影响 Unit / Ability / Item 等级流转。
- `War3Frame.Generator/`: 预期无直接影响；新增系统使用现有 `SystemRegisterAttribute`。
- `FrameBuild/`: 预期无影响。
- `CSharpWar3Frame/`: 预期无影响。
- `Projects/`: 后续需要示例验证 Unit 获得经验、Ability/Item 成长、杀敌数可映射为经验。

## 验收标准

- 能为 Unit 配置经验曲线和当前经验。
- 能为 Ability / Item 配置经验曲线和当前经验。
- `ExperienceGainRequest` 支持基础经验与倍率经验。
- 经验满足升级需求时，系统提升等级并添加 `LevelStatDirty`。
- 经验系统不直接修改攻击力、耗蓝、物品属性贡献等等级属性结果。
- `dotnet build War3Frame/War3Frame.csproj` 与 `dotnet build Projects/test/test.csproj` 通过。

## 风险与回滚

- 风险: 经验系统和等级属性系统职责混杂。
  - 缓解: 明确经验系统只负责经验和等级变化；属性重算交给 `LevelStatRebuildSystem`。
- 风险: 杀敌数和经验值概念混淆。
  - 缓解: 第一阶段可把杀敌数视为经验的一种来源；若后续需要展示不同经验类型，再引入 `ExperienceKind`。
- 风险: Unit / Ability / Item 等级来源不统一。
  - 缓解: 设计阶段明确各对象当前等级来源，经验系统只写对应等级字段或组件。
- 回滚: 可移除经验组件、请求和系统，不影响 `LevelValue` 和 `LevelStatRebuildSystem` 的独立运行。

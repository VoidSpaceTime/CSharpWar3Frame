# 统一 Effect authoring 示例与层级说明

## 0. 基本信息

- Change ID: `standardize-effect-authoring-guidance`
- 提案等级: `light`
- 目标一句话: 统一仓库内 Effect authoring 示例为 `EffectChainBuilder` lambda 风格，并补充一份面向模板作者的层级说明。
- 请求来源: Effect Builder 收层完成后，需要巩固新的 authoring 入口，避免模板、历史术语和底层数据类型继续造成认知混淆。

## 1. 分级判定

### 1.1 为什么是 `light`

- 影响范围: `Projects/test` 的技能/物品示例与一份 Effect authoring 说明文档。
- 风险等级: 低；不新增运行时能力，不修改效果执行语义、公共数据契约或 native 调用。
- 可逆性: 高；可通过恢复示例和删除说明文档回滚。
- 是否跨项目: 示例位于 `Projects/test`，说明文档固定放在 `War3Frame/Src/Helpers/EffectAuthoring.md`，但不改变项目依赖。
- 是否改公共契约: 否。

### 1.2 升级触发器检查

- [ ] 涉及 `War3Frame.Generator/` 输出或契约
- [ ] 涉及 `FrameBuild/`、构建链路或发布流程
- [ ] 涉及 `CSharpWar3Frame/` 入口行为
- [x] 涉及 `Projects/` 示例或集成验证行为
- [ ] 涉及公共 API / 数据结构 / 配置契约
- [ ] 涉及架构边界、目录结构、依赖关系重组

本变更只整理已存在的 authoring 能力和说明，不新增 `EffectChainBuilder` API，因此维持 `light`。

## 2. 背景与目标

`consolidate-effect-builder-authoring` 已将公开效果链入口收敛为 `EffectChainBuilder`，并删除 `AbilityEffectSpecBuilder` / `EffectSpecBuilder` 的并行 Builder 层。当前 `Projects/test` 已主要使用：

- `AbilitySpecBuilder.OnEffect(e => ...)`
- `AbilitySpecBuilder.OnGranted(e => ...)`
- `AbilitySpecBuilder.OnRemoved(e => ...)`
- `ItemSpecBuilder.UseEffect(e => ...)`

但仓库尚缺少一份集中说明，用于解释：

- `AbilitySpecBuilder` / `ItemSpecBuilder` 负责整体 authoring。
- `EffectChainBuilder` 负责效果步骤链。
- `EffectSpec` 是底层效果链数据契约。
- `AbilityValue` 是模板作者友好的数值表达。
- `EffectValueSpec` 是运行时可解析数值规格，并非另一层 Builder。

本提案目标是让新模板作者能够从一组一致示例和一份简短说明理解 Effect authoring，而无需查阅历史提案或已删除类型。

## 3. 影响范围

- 计划检查与必要时修改：
  - `Projects/test/Scripts/Template/Ability.cs`
  - `Projects/test/Scripts/Template/Item.cs`
- 计划新增一份简明中文说明文档：
  - `War3Frame/Src/Helpers/EffectAuthoring.md`
- 全局影响：
  - `War3Frame/`: 只新增说明文档，不改运行时代码。
  - `War3Frame.Generator/`: 不受影响。
  - `FrameBuild/`: 不受影响。
  - `CSharpWar3Frame/`: 不受影响。
  - `Projects/`: 只整理 `Projects/test` 示例写法与注释，不改变示例行为。

## 4. 方案摘要

1. 以 lambda authoring 作为仓库示例的首选写法：

   ```csharp
   .OnEffect(e => e
       .Area(...)
       .Damage(...))
   ```

2. 生命周期和物品效果同样使用统一入口：

   ```csharp
   .OnGranted(e => e.Effect(...))
   .OnRemoved(e => e.RemoveEffectByKey(...))
   .UseEffect(e => e.Heal(...))
   ```

3. 允许框架内部或高级调用方传入预构建 `EffectSpec`，但模板与教学示例不以该写法为默认入口。
4. 文档说明步骤执行顺序、目标上下文变化以及 Projectile arrive 嵌套链的基本语义。
5. 文档明确禁止模板或业务 authoring 直接调用 `JassApi` / `KKApi` / `YDApi` / `DzApi`。
6. 不为统一示例而新增当前系统不存在的效果能力；无法真实表达的场景不得用占位 API 伪装为已支持。

## 5. 非目标

- 不新增或修改 `EffectChainBuilder` 方法。
- 不修改 `EffectSpec` / `EffectValueSpec` 数据结构。
- 不重构 Ability、Item、Projectile、Visual、Aura 或 Buff 运行时。
- 不迁移历史 OpenSpec 文档中的旧类型名称；历史提案保留当时语境。
- 不修改 Native/Execution 分层。

## 6. 风险与回滚

- 风险: 示例注释与实际运行时语义不一致。
  - 控制: 只记录当前源码可验证的 API 和执行边界，并通过项目构建验证示例。
- 风险: 为追求统一而把高级 `EffectSpec` 用法描述为禁止。
  - 控制: 明确其仍是底层数据/预构建入口，只是不作为普通模板作者的首选写法。
- 风险: 历史 OpenSpec 中仍出现旧 Builder 名称，搜索结果看起来不统一。
  - 控制: 文档说明历史提案不做追溯性改写；一致性要求只适用于当前源码、模板和新文档。
- 回滚方式: 恢复模板修改并删除新增说明文档。

## 7. 验收标准

- `Projects/test` 当前技能与物品模板不引用 `AbilityEffectSpecBuilder` 或 `EffectSpecBuilder`。
- 普通效果、Projectile arrive、生命周期长期视觉和物品使用效果均有可编译的 `EffectChainBuilder` lambda 示例。
- 新说明文档准确解释 `AbilitySpecBuilder`、`ItemSpecBuilder`、`EffectChainBuilder`、`EffectSpec`、`AbilityValue`、`EffectValueSpec` 的职责。
- 文档明确 ECS/Native 边界，不鼓励业务模板直接执行 War3 native 调用。
- `dotnet build Projects/test/test.csproj` 通过。
- 如文档引用 `War3Frame` 公共 API，`dotnet build War3Frame/War3Frame.csproj` 通过。

## 8. 审核 Gate

- 用户批准前不修改模板或新增正式说明文档。
- 若实施中发现需要新增 Builder API 或修改执行语义，本 change 自动升级或另开 `full` 提案。

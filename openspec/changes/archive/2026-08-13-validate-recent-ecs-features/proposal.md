# 验证近期 ECS 功能与生成代码

## 0. 基本信息

- Change ID: `validate-recent-ecs-features`
- 提案等级: `light`
- 目标一句话: 在继续构建新功能前，对近期 Ability 生命周期、等级经验、Tag 规范与生成注册输出做一次本地静态验收。
- 请求来源: 用户说明暂时没有 War3 测试环境，也尚未检查此前生成/修改的代码，同意先按静态验收路线推进。

## 1. 分级判定

### 1.1 为什么是 light

- 影响范围: 主要是验证 `War3Frame/`、`War3Frame.Generator/` 输出和 `Projects/test` 构建，不改变运行时代码。
- 风险等级: 低；本 change 的执行内容是构建、生成代码检查、OpenSpec 对照和未提交文件审查。
- 可逆性: 高；若只产出验收记录或后续建议，不影响现有行为。
- 是否跨项目: 验证会读取/构建多个项目，但不修改多个项目。
- 是否改公共契约: 否；不新增 API、不改 Source Generator、不改 ECS 语义。

### 1.2 升级触发器检查

- [ ] 涉及 `War3Frame/` 与其他项目联动
- [ ] 涉及 `War3Frame.Generator/` 输出或契约
- [ ] 涉及 `FrameBuild/`、构建链路或发布流程
- [ ] 涉及 `CSharpWar3Frame/` 入口行为
- [ ] 涉及 `Projects/` 示例或集成验证行为
- [ ] 涉及公共 API / 数据结构 / 配置契约
- [ ] 涉及架构边界、目录结构、依赖关系重组

说明: 本提案允许检查生成输出，但不修改生成器或生成契约。若验收发现必须修改 Source Generator、公共组件契约、构建链路或跨项目示例行为，应停止当前 light 范围并另开或升级为 `full`。

## 2. 背景与目标

近期已经完成等级经验系统、Ability 生命周期调整、Tag 分类规范文档和硬件输入 Tag 处理提案。当前仍存在两个现实约束：

- 用户尚未人工检查生成注册代码和近期改动结果。
- 暂时没有真实 War3/WE 测试环境，不能依赖运行时手测。

目标是在继续实现硬件输入、Native 输入或更多技能系统之前，先通过本地静态手段确认当前基础是否可靠。

## 3. 影响范围

- 模块:
  - `War3Frame/`: 读取近期 ECS 组件、系统、helper 与 Tag 文档，执行项目构建。
  - `War3Frame.Generator/`: 构建生成器并检查生成的系统注册输出。
  - `Projects/test`: 构建测试项目，确认近期 Ability 模板与运行时 API 可编译。
- 文件:
  - 本提案只新增 OpenSpec 记录。
  - 验收阶段可读取 `War3Frame/Src/Components/TagTaxonomy.md`、`War3Frame/Src/Helpers/AuraHelper.cs` 和生成的 `Game.SystemRegistration.g.cs`。
- 不受影响区域:
  - `FrameBuild/`: 不修改构建编排。
  - `CSharpWar3Frame/`: 不修改 CLI。
  - `Projects/demo`: 本轮默认不修改示例；必要时只作为构建检查对象。

## 4. 方案摘要

### 4.1 构建验收

按从小到大的顺序执行本地构建：

1. `dotnet build War3Frame.Generator/War3Frame.Generator.csproj`
2. `dotnet build War3Frame/War3Frame.csproj`
3. `dotnet build Projects/test/test.csproj`

如前两项和 `Projects/test` 都通过，再视时间和错误风险决定是否补充 `Projects/demo/demo.csproj` 或解决方案级构建。

### 4.2 生成代码检查

构建 `War3Frame` 后，定位 `Game.SystemRegistration.g.cs`，检查：

- 近期新增或调整的 Ability / Level / Experience 系统是否被注册。
- `SystemKind.Immediate` 与 interval/root 注册位置是否符合预期。
- 注册顺序是否与 `[SystemRegister(..., order)]` 一致。
- 是否出现遗漏、重复或命名异常。

本轮只检查输出，不修改 `War3Frame.Generator/`。

### 4.3 OpenSpec 与未收口文件审查

读取并对照近期 OpenSpec 工件：

- `introduce-experience-leveling-system`
- `refine-ability-lifecycle`
- `standardize-tag-taxonomy`
- `define-war3-native-input-tags`

同时检查当前未收口文件：

- `War3Frame/Src/Components/TagTaxonomy.md`
- `War3Frame/Src/Helpers/AuraHelper.cs`

审查目标是判断它们是否应后续提交、补提案、继续修改或暂缓，不在本 change 内顺手修复运行时代码。

### 4.4 输出验收报告

最终输出简短验收报告，包含：

- 构建结果。
- 生成注册检查结论。
- 未收口文件处理建议。
- 是否可以进入下一轮功能建设。
- 若发现需要代码修复，说明应另开 `fast` / `light` / `full` 提案。

## 5. 非目标

- 不实现 Q/E 硬件输入或 War3 Native 输入层。
- 不修改 `War3Frame.Generator/`。
- 不修改 Ability、等级经验、Tag 或 Aura 的运行时代码。
- 不批量迁移 Tag 命名。
- 不提交 git commit，除非用户后续明确要求。

## 6. 风险与回滚

- 风险: 构建或生成检查可能暴露既有问题，导致需要新增修复提案。
- 控制: 本 change 只做验收与报告，发现问题时停止扩大范围。
- 回滚方式: 删除本提案或放弃验收报告即可，不影响运行时行为。

## 7. 验收标准

- 已执行 `War3Frame.Generator`、`War3Frame`、`Projects/test` 的本地构建并记录结果。
- 已定位并检查生成的 `Game.SystemRegistration.g.cs`。
- 已审查当前未收口的 `TagTaxonomy.md` 与 `AuraHelper.cs`。
- 已对照近期 OpenSpec 工件，确认是否存在范围缺口。
- 已输出是否适合继续新功能建设的结论。

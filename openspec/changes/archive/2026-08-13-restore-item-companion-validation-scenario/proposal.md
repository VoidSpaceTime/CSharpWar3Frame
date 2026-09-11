# 恢复 Item companion ability 验证场景

## 0. 基本信息

- Change ID: `restore-item-companion-validation-scenario`
- 提案等级: `light`
- 状态: 已批准，实施与本地验证完成，等待真实 War3 客户端验收
- 目标一句话: 恢复 `Projects/test` 缺失的 `ItemCompanionAbilityValidationScenario`，修复测试入口编译错误并补回 companion ability 集成验证。
- 请求来源: 用户删除测试代码后，`Projects/test/Program.cs` 仍引用该场景，并确认按轻量方案恢复且补充简要中文注释。

## 1. 分级判定

- 影响范围: 仅新增 `Projects/test` 验证场景与本 OpenSpec 记录。
- 风险等级: 低；不修改运行时行为、公共契约或生成输出。
- 可逆性: 高；可删除恢复的场景，并同步清理测试入口引用。
- 是否跨项目: 否，仅影响 `Projects/test`。
- 是否改公共契约: 否。

若恢复过程中发现必须修改 `War3Frame/` 生产代码、公共 API、Source Generator 或构建链路，本提案立即停止并升级范围后重新审核。

## 2. 背景与目标

`Projects/test/Program.cs` 当前仍调用 `ItemCompanionAbilityValidationScenario.Initialize(...)` 与 `Update()`，但对应源码和 `War3Frame.Scripts.Process` 命名空间已经不存在，导致测试项目无法编译。

归档能力 `item-companion-ability` 要求测试场景覆盖 companion 创建、目标派发、来源传播和生命周期。当前生产实现及配套 Item/Ability 模板仍在，因此优先恢复验证场景，而不是回滚生产实现或删除全部验证入口。

## 3. 影响范围

- 新增 `Projects/test/Scripts/Process/ItemCompanionAbilityValidationScenario.cs`。
- 复用现有 `Projects/test/Program.cs` 初始化和时钟驱动入口；仅在签名对齐确有必要时做最小调整。
- 复用现有 `ItemSpecBuilder` 与 `AbilitySpecBuilder` authoring API，不扩展生产 API 或测试模板。
- 新增的类、函数和关键状态处理必须带最简要的中文职责注释。

不受影响区域：

- `War3Frame/`: 不修改组件、Helper、System 或 Native/Execution 行为。
- `War3Frame.Generator/`: 不修改生成器与系统注册契约。
- `FrameBuild/`: 不修改构建编排。
- `CSharpWar3Frame/`: 不修改 CLI 与入口行为。
- `Projects/demo`: 不修改示例项目。

## 4. 方案摘要

- 根据现有 Item/Ability authoring API 和归档 `item-companion-ability` 规范重建验证场景，不臆造已删除源码的实现细节。
- 保留 `Initialize(JPlayer)` 与 `Update()` 入口，使现有测试客户端无需新的调度分支。
- 场景至少验证 companion 创建与唯一性、ItemUse 目标派发、item/user origin 保留，以及解绑或删除后的生命周期收口。
- 验证失败应输出明确上下文并计入失败结果，不以吞异常或静默跳过代替验证。

## 5. 风险与回滚

- 风险: 被删场景从未进入 Git，无法逐字恢复原实现。
  - 控制: 以当前模板、运行时契约和归档 capability spec 为事实源，重建最小可维护场景。
- 风险: 真实 War3 Native 行为无法仅靠本地构建证明。
  - 控制: 本轮完成静态构建与场景代码检查，真实 War3 客户端运行仍作为独立验收项记录。
- 回滚方式: 删除新增场景，并同时移除 `Program.cs` 中对应 `using`、`Initialize` 与 `Update` 调用；不回滚生产实现。

## 6. 验收标准

- `ItemCompanionAbilityValidationScenario` 可被 `Projects/test/Program.cs` 正确解析。
- 场景类、函数和关键状态处理具有最简要的中文职责注释。
- 场景覆盖 companion 创建与唯一性、目标派发、来源传播和至少一条生命周期清理路径。
- `dotnet build War3Frame/War3Frame.csproj -c Release` 成功。
- `dotnet build Projects/test/test.csproj -c Release` 成功。
- 本变更不修改 `War3Frame/` 生产代码，不新增 War3 Native 调用。
- 实际 War3 客户端运行结果单独说明，不用本地构建替代。

## 7. 审核 Gate

用户明确批准本提案后才能恢复场景代码；批准前不修改 `Projects/test` 实现。

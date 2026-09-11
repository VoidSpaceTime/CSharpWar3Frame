# Proposal: fix-system-register-immediate-contract

**等级**: full
**状态**: 待审核

## 0. 基本信息

- Change ID: `fix-system-register-immediate-contract`
- 目标一句话: 修复 `SystemRegister` 生成器对 `SystemKind.Immediate` 的契约断裂，恢复立即系统的注册与刷新语义。
- 请求来源: 仓库结构方向审查（生成器枚举取值 bug + `ImmediateRoot` 缺失）。
- 默认实施后审查强度: `R2 Targeted`
- 命中的审查升级触发器: 生成器输出契约、核心业务流程、跨模块状态协作
- 最终实施后审查强度: `R2 Targeted`
- Oracle 可用性与 `R1` 回退方式: 不适用（`full` 默认 `R2`）
- 完整 `review-work` 授权来源: 无

### 0.1 工件矩阵

`full`：proposal / design / tasks / specs/system-register-immediate-contract/spec.md

### 0.2 总结深度矩阵

`full`：完整总结，覆盖改动范围、全局影响、验证覆盖、风险与后续建议。

### 0.3 实施后审查强度矩阵

命中"生成器输出契约""核心业务流程""跨系统状态协作"，至少 `R2 Targeted`，需覆盖生成器输出正确性、Immediate 调度时序、注册集合完备性三个视角。

## 1. 分级判定

### 1.1 为什么是这个等级

- 影响范围: `War3Frame.Generator`（生成器输出契约）+ `War3Frame`（运行时注册与主循环）+ 全部 17 处 Immediate 标注系统
- 风险等级: 高——修复后 Immediate 系统从"误入 Root 按 interval 轮询"变为"进入 ImmediateRoot"，调度语义改变；若无 flush 调用点则系统完全不执行
- 可逆性: 中——改动集中在生成器与 `Game` 初始化，可整体回滚
- 是否跨项目: 是——`War3Frame.Generator` + `War3Frame`
- 是否改公共契约: 是——`SystemGenerator` 生成的 `Game.SystemRegistration.g.cs` 输出契约

### 1.2 升级触发器检查

- [x] 涉及 `War3Frame/` 与其他项目联动
- [x] 涉及 `War3Frame.Generator/` 输出或契约
- [ ] 涉及 `FrameBuild/`、构建链路或发布流程
- [ ] 涉及 `CSharpWar3Frame/` 入口行为
- [x] 涉及 `Projects/` 示例或集成验证行为
- [x] 涉及公共 API / 数据结构 / 配置契约
- [ ] 涉及架构边界、目录结构、依赖关系重组

### 1.3 实施后审查升级触发器

- [x] 公共 API、生成器输出、配置格式、构建链或发布契约
- [ ] 持久化、迁移、数据兼容性或数据丢失风险
- [x] 性能回归、资源泄漏、实时性或大规模数据影响
- [x] 多系统、多项目或跨边界状态协作

### 1.4 工具授权与回退

- [x] `R2` 的视角与 verdict 已记录，不按代理数量计数
- [x] 未因 `R3` 自动启用完整 `review-work`
- [x] 完整 `review-work` 无授权来源

## 2. 背景 / Why

### 2.1 生成器枚举取值 bug

`War3Frame.Generator/SystemGenerator.cs:60`：

```csharp
var kindArg = attributeData.ConstructorArguments[0];
var kind = kindArg.Value?.ToString();
```

Roslyn 的 `TypedConstant.Value` 对枚举参数返回的是**底层整数值**（`SystemKind.Interval = 0`、`SystemKind.Immediate = 1`），`ToString()` 得到 `"0"` / `"1"`，而不是枚举成员名 `"Interval"` / `"Immediate"`。

因此 `SystemGenerator.cs:122` 的判断：

```csharp
var rootName = systemInfo.Kind == "Immediate" ? "ImmediateRoot" : "Root";
```

**永远走 `Root` 分支**。全部 17 处标注 `[SystemRegister(SystemKind.Immediate, order)]` 的系统（`CastRequestSystem`、`ItemUseSystem`、`UnitCreateNativeSystem`、`UnitRemoveNativeSystem`、`UnitLifecycleTransitionSystem`、`AbilityAttachWorkflowSystem` 等）都被注册进 `TimedSystemRoot`，按 `DefaultInterval = 0.03125s` 轮询执行。

**后果**：`SystemKind.Immediate` 的"立即执行"语义彻底丢失，立即系统退化为低频轮询。这正是生成器引用 `ImmediateRoot` 但 dll 中无该符号却能编译通过的原因——该分支根本不会命中。

### 2.2 `Game.ImmediateRoot` 缺失

git 历史 `57ca030`（2026-03-22）中 `Game` 曾定义：

```csharp
public static SystemRoot ImmediateRoot { get; private set; }
// ECSInit 中: ImmediateRoot = new SystemRoot(Store);
public static void FlushImmediateSystems() => ImmediateRoot?.Update(default);
```

但在 `13a3c16`（2026-05-15）中 `ImmediateRoot` 与 `FlushImmediateSystems` 被删除，生成器代码未同步。当前 `Game`（`initialization/ECSInit.cs`）只声明 `Root`。

### 2.3 无任何 flush 调用点

全仓搜索无 `FlushImmediateSystems()` / `ImmediateRoot.Update` 调用。`War3Init.cs` 主循环只调 `Root.Update(tick)`。

**含义**：若只修好枚举 bug 而不恢复 `ImmediateRoot` 与 flush 机制，所有 Immediate 系统将进入一个"从未被 Update 的 root"，完全不再执行——比当前"误轮询"更糟。

### 2.4 `encapsulate-immediate-flush-for-native-dirty` 声明与实际不符

该 change 的 `tasks.md` 全部勾选、`spec.md` 声明 `Game` 层 flush 入口与 `UnitNativeDirtyHelper` 立即标记入口，但当前代码中上述符号全部不存在，且 git log 无对应落地提交。说明该 change 是"文档先行、代码从未落地或已被移除"。本提案是修复同一契约断点，非重复工作。

## 3. 变更范围 / What

1. 修复 `SystemGenerator` 的枚举取值：改为通过 Roslyn 解析枚举成员名（`INamedTypeSymbol` 遍历 `IFieldSymbol` 匹配 `ConstantValue`），得到 `"Interval"` / `"Immediate"`。
2. 恢复 `Game.ImmediateRoot`（`SystemRoot`）与 `Game.FlushImmediateSystems()`。
3. 确定 flush 时机：主循环每 tick 在 `Root.Update(tick)` 后调用一次 `FlushImmediateSystems()`（推荐方案，详见 design.md）。
4. 保持 Interval 系统注册行为不变。
5. 为项目与 test 补充编译级验证，确认 Immediate 系统进入 `ImmediateRoot`。

## 4. 全局影响分析

- `War3Frame/`：`Game` 增加 `ImmediateRoot`/`FlushImmediateSystems`；`War3Init.cs` 主循环增加 flush；17 处 Immediate 系统的执行时机从 0.03125s 轮询变为每 tick flush。
- `War3Frame.Generator/`：`SystemGenerator.cs` 枚举解析修复；生成输出从"全进 Root"变为"Immediate 进 ImmediateRoot、Interval 进 Root"。
- `FrameBuild/`：不受影响——不涉及构建编排或发布链路。
- `CSharpWar3Frame/`：不受影响——不涉及 CLI 参数或命令行为。
- `Projects/`：`test` 运行时验证场景将观察到 Immediate 系统执行时机变化；`demo` 仅冒烟，不受影响。

## 5. 设计要点

详见 `design.md`。核心决策：

- **枚举修复**：生成器解析枚举成员名，保留 `== "Immediate"` 字符串契约不变，改动最小。
- **ImmediateRoot 类型**：使用 friflo 原生 `SystemRoot`（非 `TimedSystemRoot`），因为 Immediate 系统不需要 interval 调度，每次 flush 全部执行一次。
- **flush 时机**：主循环每 tick flush（每 0.01s），使"写 request → 立即消费"的语义近似成立，且无需逐个补 helper 调用点。
- **兼容性**：修复前所有 Immediate 系统按 interval 轮询；修复后每 tick 执行。已实现 `ITimedSystem` 的 Immediate 系统（如 `CastRequestSystem` Interval=0.02）不再读取其 Interval，统一按 flush 执行。

## 6. 风险、兼容性、迁移

- **风险 A（无 flush 则全挂）**：若恢复 ImmediateRoot 后主循环不 flush，所有 Immediate 系统不执行。→ 必须在同一 change 内完成 flush 接线，不可拆分提交。
- **风险 B（执行频率上升）**：Immediate 系统从 0.03125s 变为 0.01s tick 执行，`CastRequestSystem`、`ItemUseSystem` 等轮询型系统频率上升 3 倍。这些系统内部有 request 组件守卫（无 request 时空转），开销可控；但需在验证阶段确认无性能回归。
- **风险 C（行为时序变化）**：原先依赖"Immediate 系统延迟到 interval tick 才消费 request"的隐性依赖会消失。需在 `Projects/test` 验证场景中确认施法/物品/单位创建/生命周期流程仍正确。
- **回滚**：还原 `SystemGenerator.cs` 与 `Game` 三处改动即可回到"全进 Root"状态；Immediate 系统重新按 interval 轮询。

## 7. 验证计划

- `dotnet build War3Frame.Generator/War3Frame.Generator.csproj`
- `dotnet build War3Frame/War3Frame.csproj`（生成代码引用 `ImmediateRoot` 必须可解析）
- `dotnet build Projects/test/test.csproj`
- 静态检查生成代码：确认 `Game.SystemRegistration.g.cs` 中 17 个 Immediate 系统出现在 `ImmediateRoot.Add(...)`，Interval 系统出现在 `Root.Add(...)`
- 反编译/字符串搜索 War3Frame.dll 确认 `ImmediateRoot` 符号存在
- 运行时行为：`Projects/test` 的 `ItemCompanionAbilityValidationScenario` 及施法/单位创建流程手测记录

## 8. 拆分任务

详见 `tasks.md`。

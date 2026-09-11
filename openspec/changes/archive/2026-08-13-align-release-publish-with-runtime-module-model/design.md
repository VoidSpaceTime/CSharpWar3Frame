## Context

当前证据已经足够闭合：

- `CSharpWar3Frame/Program.cs` 中默认 `run <Project>` 走 `BuildModeEnum.Test`，而 `run <Project> -r` 才走 `Release`。
- `FrameBuild/CommandManager/Run.cs` 在 Release 下执行 `PublishProject(true, projectsPath, publishDir)`，从而追加 `-p:PublishAot=true`。
- `Projects/test/test.csproj` 与 `Projects/demo/demo.csproj` 都显式引用了 `DNNE`，并启用了动态加载相关属性。
- `Projects/test/Program.cs` 当前导出的入口是 `MainJIT()`，不是 `MainAOT()`。
- callback/runtime integration 又明确以模块 DLL 的形式加载运行时模块。

这说明当前 Release 发布链混合了两套不一致的产物模型：

1. `Run.cs` 假定 Release 应走 AOT publish。
2. `test.csproj` / `demo.csproj` 仍然是 DNNE/JIT 导出模块模型。

结果就是：publish 实际进入 DNNE 原生导出工具链，并在缺少 Win10 SDK 时失败；即便补环境，当前 callback/module 约定仍未被正式统一到单一产物模型上。

## Goals / Non-Goals

**Goals:**
- 定义 Release 下唯一允许的运行时模块产物模型。
- 明确 `Run.cs` 发布策略应如何与 csproj 产物模型对齐。
- 明确 callback/module naming 必须与最终生成物严格一致。

**Non-Goals:**
- 本提案不立即决定全部项目都切 AOT 还是全部保留 DNNE。
- 本提案不在提案阶段修复本机 Win10 SDK 缺失。
- 本提案不实施运行时代码修改。

## Decisions

### 1. Release 发布链必须只服务一种产物模型
**Decision:** Release 发布链 MUST 明确采用单一运行时模块模型，SHALL NOT 同时混用 AOT 发布假设与 DNNE/JIT 模块假设。

**Rationale:** 当前黑屏/发布失败的直接根因就是上层发布假设与项目本体产物模型冲突。

### 2. `Run.cs` 的发布策略必须与项目 csproj 产物模型一致
**Decision:** `FrameBuild/CommandManager/Run.cs` MUST 依据项目的实际产物模型（DNNE/JIT 或 AOT）执行对应发布策略，而 SHALL NOT 对当前项目一律强加 `PublishAot=true`。

**Rationale:** `Projects/test` 目前不是纯 AOT 产物模型，强加 AOT 分支会直接导致发布链进入错误轨道。

### 3. Callback/module naming 必须与最终模块名严格一致
**Decision:** callback 写入的 `ModuleName` 与 `ModulePath` MUST 以最终生成模块为唯一事实源，不能继续依赖模式假设写死 `*.dll` 名称。

**Rationale:** 即便环境补齐，如果 callback 仍指向错误模块名，黑屏问题仍会保留。

### 4. 环境缺失是次级问题，不是主设计修复
**Decision:** Win10 SDK 缺失 MAY 导致 DNNE 工具链失败，但这被视为环境阻断；主设计修复仍是统一发布链与产物模型。

**Rationale:** 当前问题不是“机器少一个 SDK”这么简单，而是代码本身对产物类型的预期已经混乱。

## Risks / Trade-offs

- [风险] 只补环境而不统一模型，会让发布链在不同机器上继续表现不一致。  
  [缓解] 先统一产物规范，再决定环境依赖。

- [风险] 直接切纯 AOT 会影响 callback、导出入口、模块命名和上层加载逻辑。  
  [缓解] 在实现前明确产物协议，不在本提案阶段仓促切换。

- [风险] 保留 DNNE 但仍走 `publish` 主链，会继续要求原生工具链。  
  [缓解] 在实现方案中明确“消费 DNNE 构建产物”与“publish 产物”谁是主链。

## Migration Plan

1. 审核通过本次 OpenSpec 提案。
2. 明确 `Projects/test` / `Projects/demo` 当前的目标模块模型。
3. 决定 Release 下主链是 “AOT publish” 还是 “DNNE/JIT 模块输出”。
4. 调整 `Run.cs` 发布策略与 callback 写入逻辑，使其严格匹配选定模型。
5. 在验证通过后，才决定是否需要环境层补充 Win10 SDK。

## Open Questions

- `test` / `demo` 是否都应统一切向 AOT，还是保留 DNNE/JIT 模型。
- callback 是否应继续以 `.dll` 为统一模块名，还是根据产物模型做条件化处理。
- `PublishProject()` 是否仍应承担“主模块生成”，还是只在某一模型下使用。

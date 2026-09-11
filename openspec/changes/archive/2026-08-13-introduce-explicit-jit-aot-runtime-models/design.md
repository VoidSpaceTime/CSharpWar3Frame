## Context

当前仓库实际上已经同时存在两种运行时模块模型：

1. **JIT / DNNE 模型**
   - 项目引用 `DNNE`
   - 导出 `main`
   - callback 以模块 DLL 形式加载运行时模块

2. **AOT 模型**
   - `Run.cs` 的 Release 路径通过 `PublishAot=true` 假定 AOT 发布物

问题不在于仓库支持两种模式，而在于：

- 这两种模式没有被显式建模
- 当前代码通过 `BuildMode` 和若干隐含分支去“猜”应该走哪一种
- callback/module naming 也依赖这种隐含推断

因此，最小正确修复不是“继续往 `BuildMode` 上打补丁”，而是引入一个单独的 runtime module model 概念。

## Goals / Non-Goals

**Goals:**
- 显式定义 JIT / AOT 两种 runtime module model。
- 保证一次发布只能选择一种模型。
- 保证 callback/module naming 与所选模型严格一致。
- 保证 `Run.cs` 依据模型而不是 BuildMode 选择发布行为。

**Non-Goals:**
- 本提案不直接决定所有项目必须切向 AOT 或 JIT。
- 本提案不在提案阶段修改 csproj / callback / 发布代码。
- 本提案不讨论 Win10 SDK 安装细节。

## Decisions

### 1. Runtime module model is a first-class concept
**Decision:** 仓库 MUST 引入显式的 runtime module model 概念，至少覆盖 `Jit` 与 `Aot` 两种模式。

**Rationale:** 当前错误不是“支持双模式”，而是“同时支持但没有显式建模”。

### 2. BuildMode and RuntimeModuleModel must be separate axes
**Decision:** `BuildMode` SHALL NOT 再隐式承担运行时模块产物形态的职责；runtime module model MUST 作为独立轴存在。

**Rationale:** `Test / Build / Release` 与 `Jit / Aot` 不是同一个维度。

### 3. One publish path, one runtime model
**Decision:** 一次发布 MUST 只选择一种 runtime module model，SHALL NOT 在同一条发布链里混用 AOT 与 DNNE/JIT 假设。

**Rationale:** 当前黑屏与发布失败的根因就是同一条链路混用了两套模型。

### 4. Callback/module naming must be model-aware
**Decision:** callback 的 `ModuleName` 与 `ModulePath` MUST 根据 runtime module model 生成，而 SHALL NOT 仅凭 BuildMode 推断。

**Rationale:** callback 当前直接决定地图运行时去加载什么模块，因此它必须以最终产物模型为事实源。

### 5. `Run.cs` publish strategy must be model-aware
**Decision:** `Run.cs` MUST 根据所选 runtime module model 执行对应发布策略；`PublishAot=true` 只能出现在 AOT 路径里。

**Rationale:** 这是把当前 AOT 假设从 DNNE/JIT 项目上解绑的最小必要条件。

## Risks / Trade-offs

- [风险] 继续把 runtime 模型藏在 BuildMode 分支里，会导致后续再加一个项目时重复踩坑。  
  [缓解] 显式引入 `RuntimeModuleModel`，让调用方与 callback 都统一依赖它。

- [风险] callback/module naming 改造会触及现有加载约定。  
  [缓解] 在实现前先固定每种模型的模块命名协议，再做代码迁移。

- [风险] 两种模式并存会增加维护复杂度。  
  [缓解] 允许双模式，但每次发布只能选一种，避免链路交叉污染。

## Migration Plan

1. 审核通过本次 OpenSpec 提案。
2. 引入显式 `RuntimeModuleModel` 概念。
3. 定义 `BuildMode x RuntimeModuleModel` 的合法组合。
4. 调整 `Run.cs`，让发布策略按模型分流。
5. 调整 callback/module naming，让其按模型生成。
6. 验证 `test` / `demo` 在 JIT 与 AOT 下的实际模块产物和加载行为。

## Open Questions

- `test` / `demo` 的默认 runtime module model 是否应保持 JIT，AOT 作为显式 opt-in。
- callback 是否继续统一以 `.dll` 为模块载体，还是根据 AOT/JIT 产物协议显式区分。
- `PublishProject()` 是否应继续作为统一入口，还是拆成 JIT/AOT 两条明确分支。

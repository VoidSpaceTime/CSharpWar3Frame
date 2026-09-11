# 提案：SyncContext 同步/异步上下文断言

**状态**：待审核
**等级**：fast
**提案日期**：2026-08-31
**请求来源**：分析 lik 框架（`async.lua`/`sync.lua` 的 `async._id` + `must()` 断言）后提出的借鉴需求；用户指示"先生成需求暂时不落地"。

---

## 目标

把框架中"UI 事件是异步的、SyncHelper 回调是同步的"这条**注释约定**（`SyncHelper.cs` 头部注释）变成代码级 Debug 断言，在调试期抓住两类 War3 高危错误：

1. **在同步回调（`SyncHelper.OnSyncReceived`）里再次调用 `AsyncHelper.Send`** —— 递归同步/逻辑混乱。
2. **在异步上下文（UI 回调）直接执行会改变 handle 序号的本地操作** —— handle 漂移导致 desync（lik 框架 `async.must()` 解决的同类问题）。

## 影响文件

- `War3Frame/Src/Core/Sync/SyncContext.cs`（新增，约 40 行）
- `War3Frame/Src/Core/Sync/SyncHelper.cs`（`OnSyncReceived` 入口置同步标记）
- 使用方示例：`Projects/test` 的 UI 回调入口置异步标记（文档说明 + 示例各 1 行）

## 方案摘要

```csharp
// SyncContext：静态上下文标记，Debug 断言用
public static class SyncContext
{
    public static bool IsAsync { get; private set; }
    public static void EnterAsync()   { Debug.Assert(!IsAsync); IsAsync = true; }
    public static void ExitAsync()    { IsAsync = false; }
    public static void MustSync()     { Debug.Assert(!IsAsync, "syncCheck"); }
    public static void MustAsync()    { Debug.Assert(IsAsync, "asyncCheck"); }
}
```

- `SyncHelper.OnSyncReceived` 入口调用 `MustSync()`（同步回调内不得再发同步请求）。
- UI 回调入口用 `EnterAsync()/ExitAsync()` 包裹（或 `using` 模式），回调体内 `MustAsync()`。
- 断言全部基于 `Debug.Assert`：**发布 AOT 构建自动消除**，零运行时开销，等价于 lik 的 test/release 双版本且无需双版本编译。
- 不改动任何发送/接收行为路径。

## 低风险理由

- 纯增量工具类 + 断言调用，不改变任何行为语义。
- 不触碰网络发送路径与协议格式。
- `Debug.Assert` 在发布构建中完全消除，无性能影响。
- 可快速回滚（删除断言调用即可）。

## 验证方式

1. `dotnet build War3Frame/War3Frame.csproj` 0 错误。
2. Debug 模式构造两个反例场景：同步回调内 `Send`、异步上下文直接原生操作 —— 断言分别触发。
3. Release/AOT 构建确认断言代码被消除（反编译或行为验证）。

## 后续事项

- 若后续决定落地 `batched-sync-send`，`SyncContext` 断言可顺带覆盖"flush 系统不得在异步上下文执行"的检查。
- 该提案与 `batched-sync-send` 相互独立，可分别实施。
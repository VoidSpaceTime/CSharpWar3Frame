# 总结：jassvm-native-args-extend

## 范围

批准依据：`.omo/evidence/gates/user-ruling-2026-10-01.md` 第 5 项（W4 批准门，含 22 JassVM 参数，裁决为「全部批准实施」）。本总结不自证批准。

本变更只改 `War3Frame/Library/JassVM/Native.cs` 的 `CallCdeclFunction`：

- 追加 `case 13` 至 `case 16`，每档为手写 `delegate* unmanaged[Cdecl]<int…, int>`，形参个数等于槽数，实参按 `args[0]` 到 `args[n-1]` 传入。
- `case 0` 至 `case 12` 的签名、参数顺序与调用体未改；`CallNative` / `CallNative<T>` 的公开签名与封送循环未改。
- `default` 由「暂时不支持」改为 `NotSupportedException($"不支持 {args.Length} 个参数的调用，参数上限为 16 个")`，仍不截断参数后调用。

类型边界未放宽：可封送类型仍仅 `int` / `nint` / `float` / `bool` / `string` / `Action` / `DzAction`；`double`、`long`、`ulong`、`uint`、`byte`、`short`、`enum` 继续 `InvalidCastException`；`nint` 继续 `Convert.ToInt32`；`float` 参数槽继续传该浮点值的地址而非位型。

## 全局影响

| 区域 | 是否受影响 | 理由 |
|---|---|---|
| `War3Frame/` | 是 | 仅 `CallCdeclFunction` 分发扩展与 `default` 文案。 |
| `War3Frame.Generator/` | 否 | 不新增系统、不改 `SystemRegisterAttribute` 或生成输出。 |
| `FrameBuild/` | 否 | 不改构建、模板、WE、发布。 |
| `CSharpWar3Frame/` | 否 | 不改 CLI 参数或命令。 |
| `Projects/` | 是 | `Projects/Regression` 新增一个 `runtime` 场景并在 `Program.cs` 注册。 |

未新增 War3 原生 API 绑定，未把调用挪进业务系统；本次不涉及任何 `Native` 层以外的新原生调用。

## 验证

- `dotnet build War3Frame/War3Frame.csproj -c Release`：0 错误。`dotnet build Projects/Regression/Regression.csproj -c Release`：0 错误（既有警告）。证据 `.omo/evidence/task-22-build.txt`。
- `dotnet run --project Projects/Regression -c Release`：`Executed 72; passed 72; failed 0`，退出码 0。基线 71 → 72，新增 `runtime/JassVmNativeArgsDispatch` 通过。证据 `.omo/evidence/task-22-regression.txt`。
- 新增场景用托管 `[UnmanagedCallersOnly(CallConvCdecl)]` 桩经真实 `War3.CallNative<int>` 分发：13 槽断言返回最后槽且槽序与入参一致；16 槽同样断言；17 槽断言抛 `NotSupportedException`、桩未被进入、异常信息含 17 与上限 16；`double` 参数断言抛 `InvalidCastException`。

## 专项视角

提案定级 `light`（默认 R1 Focused）；本节按 closeout 要求用三个可执行专项覆盖，达到 R2 强度。

### 1. 13–16 参数封送一致性

新增四档分支的函数指针分别有 14/15/16/17 个 `int` 类型实参（即 13/14/15/16 槽 + 返回值），与既有 `case n → n+1` 的形态一致；实参严格按 `args[0]…args[n-1]` 顺序展开，未引入 `float` 位型直传或 `double` 槽。场景对 13 与 16 两档断言返回值等于最后一个槽、且桩记录的槽序列与入参 `SequenceEqual`，因此通过不依赖「未抛异常」。

### 2. 超过 16 槽必须失败且不截断

`default` 分支在任何调用前抛出，消息含实际个数与上限 16。场景以 17 个 `int` 调用，断言异常类型为 `NotSupportedException`、异常信息同时含 `17` 与 `16`，并断言桩的静态记录仍为 `null`（未被进入），证明未取前 16 槽继续调用。

### 3. 0 至 12 槽路径不回归

实现约束为只追加 `case 13..16` 并替换 `default` 文案，未触碰 `case 0..12`。完整回归 72/72 通过，其中既有 `runtime/*`、`native-projection/production-source-call-order`、`generator/*` 等场景在扩展前后结果一致。

## 风险

- **16 仍是硬顶**：出现 >16 槽真实 native 时另开变更，本变更不更换调用机制。
- **手写参数顺序**：新分支顺序由场景断言锁定，不靠「能跑到不抛」判定。
- **真实客户端未验证**：本次用托管 cdecl 桩验证分发，未启动 War3；真实 native 客户端验证按仓库惯例记为非阻塞，不作为本变更门禁。
- **发布目标为 win-x86，回归在 x64 主机**：cdecl 下每个 `int` 槽 4 字节、调用方清栈，四档签名与展开顺序与既有 0–12 路径同构；x86 下的实际栈行为未在本次运行中执行。

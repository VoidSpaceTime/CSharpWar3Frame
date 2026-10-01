## Context

见 `proposal.md` 的 Why。当前结构（行号以合并后树为准，与 `.omo/evidence/post-merge-key-files.md` 的 Native.cs:486 一致）：

- `CallNative`（`Native.cs:279`）与 `CallNative<T>`（`:329`）各自把 `object[]` 收成 `int[] values`。`float` 不按位塞进槽，而是把 `float[]` 元素地址写入槽（`:299-301`、`:350-352`）。
- 两处都调用 `CallCdeclFunction(func, values)`（`:321`、`:373`）。返回值在泛型路径按 `int` / `float` / `bool` / `string` 解包（`:381-411`）。
- `CallCdeclFunction`（`:415-488`）是 `switch (args.Length)`：`case 0` 到 `case 12` 各铸一个 `delegate* unmanaged[Cdecl]<int…, int>` 并展开调用；`default` 在 `:486` 抛 `NotSupportedException`。
- 发布目标是 `win-x86`（`FrameBuild/CommandManager/Run.cs:99`）。cdecl 下每个 `int` 槽占 4 字节，调用方清栈。

仓库内 `CallNative` 实参逗号数最高为 11（函数指针 + 11 个用户参数 = 11 槽），出现在 `JassApi.UnitDamagePoint` 与 `CreateImage`。没有 12 槽或 13+ 槽的现有绑定。

## Goals / Non-Goals

**Goals:**

- 13 至 16 个 int 槽可以按声明顺序进入被调函数，返回的 32 位值仍走现有解包。
- 0 至 12 槽的函数指针类型、实参顺序、返回值与异常类型保持不变。
- 封送失败与超限失败仍然是异常，不静默截断参数列表。

**Non-Goals:**

- 不改封送类型集合，不新增 `double` / `long` / `uint` 路径。
- 不改 `float` 的“槽内存地址”约定，不改 `nint` 的 `Convert.ToInt32`。
- 不引入运行时栈拼装、反射 `DynamicInvoke`，或不限上限的通用调用。
- 不新增具体 JASS / Dz / KK native 绑定。
- 不在没有 War3 进程时声称验证了真实 native。

## Decisions

### 1. 继续按个数展开 `delegate*`，上限 16

在 `CallCdeclFunction` 增加 `case 13` 至 `case 16`，形态与 `case 12`（`:480-484`）相同：多一个 `int` 形参，按 `args[0]`…`args[n-1]` 传入。`case 0` 至 `case 12` 的代码块不改。

选择 16 而不是“写到刚好 13”：`delegate*` 不能用数组一次性展开，每档都是手写分支；16 覆盖当前最高 11 槽再留 5 槽，仍是一个 switch，不需要第二套调用机制。

备选：

- 只加 `case 13`。能满足计划里的“13+”字面验收，但 14 槽立刻再次撞墙，和本次要去掉的硬顶是同一类问题。
- `stackalloc` + 手写 x86 调用桩。上限可调，但要自己处理 cdecl 清栈与返回寄存器，AOT/`win-x86` 上比多四个 case 更危险。否决。
- `Marshal.GetDelegateForFunctionPointer`。多一次委托分配，且与现有 `delegate*` 路径分裂。否决。

### 2. 超过 16 槽失败，不截断

`default` 仍抛 `NotSupportedException`，消息写明当前上限为 16 以及实际个数。禁止只取前 16 个槽调用。

### 3. 数值边界保持现有封送，不在本变更放宽

两处 `switch (args[i])`（`:291-318`、`:342-369`）已是类型边界，本变更不改它们。审核时按下面的表验收；实现时不得“顺便”补类型。

| 输入 | 当前槽内容 | 本变更 |
|---|---|---|
| `int` | 原值 | 不变 |
| `nint` | `Convert.ToInt32`；超出 `int` 范围抛 `OverflowException` | 不变。`win-x86` 上指针本身是 32 位 |
| `float` | 槽 = `float[]` 元素地址，不是 `BitConverter` 位型 | 不变。返回 `float` 才按位解包（`:391`） |
| `bool` | `true→1`，`false→0` | 不变 |
| `string` | 临时 native string 地址，调用后 `DestoryNativeString` | 不变 |
| `Action` / `DzAction` | native code / 具名函数 id | 不变 |
| `double` | 无分支，落入 `default`，`InvalidCastException` | 继续拒绝。JASS `real` 是 32 位 float；若把 `double` 收成一个 `int` 槽会丢高 32 位 |
| `long` / `ulong` | 同上，拒绝 | 继续拒绝。一个槽只有 32 位 |
| `uint` | 无分支。即使数值 `< int.MaxValue` 也拒绝 | 继续拒绝，避免和 `int` 槽的符号解释分叉 |
| `byte` / `short` / `enum` | 无分支，拒绝 | 继续拒绝。调用方应先转成 `int` |
| `null` | `args[i].GetType()` 空引用 | 不新增空槽语义 |

`float` 与返回 `float` 不是同一条路：参数槽里是指针，返回值才是 EAX 里的 float 位型。扩展参数个数不得把参数 `float` 改成位型直传，否则现有 `real` 绑定会坏。

## Risks / Trade-offs

- [16 仍是硬顶] → 异常写明上限。若将来出现 >16 槽的真实 native，另开变更，不在本变更里换调用机制。
- [新分支抄错参数顺序] → 回归场景用 13 个可区分的 `int`，断言顺序与返回值；不靠“能跑到不抛”当通过。
- [改动误伤 0–12] → 实现约束是只追加 `case 13..16` 并替换 `default` 消息；场景外再人工对照 `case 0..12` 函数指针签名未改。
- [无法在无 War3 时调用真实 native] → 验收用托管 cdecl 桩验证分发。真实客户端验证按仓库惯例记为非阻塞，不作为本变更门禁。

## Migration Plan

无数据迁移。回滚即还原 `Native.cs` 的 `CallCdeclFunction`。0–12 槽调用方没有签名变化。

## Open Questions

无。上限 16、不放宽类型、不改 0–12 路径已在本设计定死；若审核否决 16 改成别的有限上限，只改 case 终点与异常文案，不改变方案。

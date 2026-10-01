# 提案：JassVM 原生调用参数上限扩展

## Why

`War3.CallNative` 把每个参数收成一个 32 位槽，再按个数走 `CallCdeclFunction` 的固定 `delegate*`。`switch` 只写到 12，13 个及以上直接抛 `NotSupportedException`（`Native.cs:486`，`暂时不支持 {args.Length} 个参数的调用`）。仓库内现有绑定最高 11 槽（`UnitDamagePoint`、`CreateImage`），所以今天没有调用方踩线；但分发本身拒绝任何 13+ 槽调用，后续多参 native 无法落地。

## What Changes

- 把 cdecl 分发从 12 槽扩到 16 槽。13 至 16 走新增分支；0 至 12 的分支体、参数顺序和返回值保持原样。
- 不改 `CallNative` / `CallNative<T>` 的公开签名，不改参数封送规则。
- 超过 16 槽仍失败，异常信息改为明确上限，不再使用“暂时不支持”。
- 数值类型边界维持现状并写进契约：只有 `int` / `nint` / `float` / `bool` / `string` / `Action` / `DzAction` 可封送；`double`、`long`、`uint`、`byte`、`enum` 继续拒绝。`nint` 与 `float` 的既有截断/指针槽语义不变。

## Capabilities

### New Capabilities

- `jassvm-native-call`: JassVM cdecl 调用的参数槽上限、既有封送类型、数值边界，以及 0 至 12 槽路径不得改变行为。

### Modified Capabilities

- 无。`openspec/specs/` 没有 JassVM 调用约定 capability。`runtime-execution` 只覆盖 ECS 执行链、Buff 与等级重算，不描述 `CallNative`。

## Impact

- `War3Frame/`：只改 `War3Frame/Library/JassVM/Native.cs` 的 `CallCdeclFunction`。`CallNative` 与 `CallNative<T>` 的封送循环不动。
- `Projects/`：在 `Projects/Regression` 增加一个具名场景，用 13 槽 cdecl 桩验证分发与参数顺序。不改 demo / test 模板。
- `War3Frame.Generator/`、`FrameBuild/`、`CSharpWar3Frame/`：不受影响。不新增系统、不改生成器、不改 `win-x86` 发布或 CLI。
- 不新增 War3 原生 API 绑定，不把调用挪进业务系统。

## 分级与审查

- 等级：`light`。单文件分发扩展，公开签名不变，不跨项目、不改生成器或构建契约，不升 `full`。
- 状态：待审核。未批准前不实现。
- 复盘：`R1 Focused`。light 默认 `R0`，但这里是 unsafe 调用约定且版本敏感（发布目标 `win-x86`），补一个调用约定视角。未授权完整 `review-work`。
- 工件：`proposal.md`、`design.md`、`tasks.md`、`specs/jassvm-native-call/spec.md`。`design.md` 只记录分发方案与数值边界，避免把实现选择藏进提案正文。实施后写短段落 `summary.md`。

## 1. 分发扩展

- [ ] 1.1 在 `CallCdeclFunction` 追加 `case 13` 至 `case 16`，每个分支使用 `delegate* unmanaged[Cdecl]`，形参个数等于槽数，实参按 `args[0]` 到 `args[n-1]` 传入。`case 0` 至 `case 12` 的签名与调用不改。验证：diff 只触及 `default` 之前的新增 case，以及 `default` 的异常消息。
- [ ] 1.2 将 `default` 改为抛出 `NotSupportedException`，消息包含实际个数与上限 16，且不得截断后继续调用。验证：构造 17 槽调用，异常类型为 `NotSupportedException`，被调函数未被进入。

## 2. 可执行验证

- [ ] 2.1 在 `Projects/Regression` 增加具名场景：注册一个 13 个 `int` 形参、返回 `int` 的 `unmanaged[Cdecl]` 桩，经 `War3.CallNative<int>` 传入 13 个互不相同的值。验证：返回值与参数顺序符合桩约定，进程不抛 `NotSupportedException`。
- [ ] 2.2 同一场景覆盖失败路径：传入一个非封送类型（`double` 或 `long`）时期望 `InvalidCastException`；传入 17 个 `int` 时期望 `NotSupportedException`。验证：`dotnet run --project Projects/Regression -- <group>` 中该场景通过。
- [ ] 2.3 构建 `War3Frame/War3Frame.csproj` 与 `Projects/Regression/Regression.csproj`，确认 0 错误。不把真实 War3 客户端调用列为阻塞项。

## 3. 收口

- [ ] 3.1 写 light 短段落 `summary.md`（范围、13 槽场景结果、0–12 未改、数值边界未放宽、真实客户端未跑及剩余风险），然后 `openspec archive jassvm-native-args-extend --yes`。验证：`openspec validate jassvm-native-args-extend` 在归档前通过，归档后主 spec 含本 delta。

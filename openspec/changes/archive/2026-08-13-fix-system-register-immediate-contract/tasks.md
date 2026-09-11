# 任务清单

## 1. 审核

- [ ] 1.1 用户审核并批准 `proposal.md` / `design.md` / `tasks.md` / `spec.md`。
- [ ] 1.2 确认提案等级为 `full`，实施后审查强度为 `R2 Targeted`。
- [ ] 1.3 确认本 change 只修复生成器契约与 Immediate 刷新机制，不迁移 GameProcessor 直调方向。

## 2. 生成器枚举解析修复

- [ ] 2.1 在 `SystemGenerator.cs` 中新增枚举成员名解析辅助方法：接收 `TypedConstant`，从其 `Type`（`INamedTypeSymbol`）遍历 `IFieldSymbol`，按 `ConstantValue` 匹配底层值，返回枚举成员名。
- [ ] 2.2 将 `GetSystemInfo` 中的 `kindArg.Value?.ToString()` 替换为枚举成员名解析，确保 `"Interval"` / `"Immediate"` 正确。
- [ ] 2.3 验证 `kindArg.Type` 为 null 或非枚举时的回退行为（默认 `"Interval"` 或保留空串走 `Root`）。

## 3. 运行时恢复 ImmediateRoot

- [ ] 3.1 在 `initialization/ECSInit.cs` 的 `Game` 中恢复 `public static SystemRoot ImmediateRoot { get; private set; }`。
- [ ] 3.2 在 `ECSInit()` 中初始化 `ImmediateRoot = new SystemRoot(Store);`（先于 `RegisterGeneratedSystems()`）。
- [ ] 3.3 新增 `Game.FlushImmediateSystems()`，内部调用 `ImmediateRoot?.Update(default)`。

## 4. 主循环 flush 接线

- [ ] 4.1 在 `initialization/War3Init.cs` 主计时器回调中，于 `Root.Update(tick)` 后调用 `FlushImmediateSystems()`。
- [ ] 4.2 确认 `TimeSpan` 累加顺序不受 flush 影响。
- [ ] 4.3 确认 `Projects/test/Program.cs` 自建主循环同样接入 flush（如有独立驱动路径）。

## 5. 验证

- [ ] 5.1 `dotnet build War3Frame.Generator/War3Frame.Generator.csproj`。
- [ ] 5.2 `dotnet build War3Frame/War3Frame.csproj`（生成代码引用 `ImmediateRoot` 必须可解析）。
- [ ] 5.3 `dotnet build Projects/test/test.csproj`。
- [ ] 5.4 检查生成代码：17 个 Immediate 系统在 `ImmediateRoot.Add(...)`，Interval 系统在 `Root.Add(...)`。
- [ ] 5.5 反编译/字符串搜索 War3Frame.dll，确认 `ImmediateRoot` 与 `RegisterGeneratedSystems` 存在。
- [ ] 5.6 `Projects/test` 运行时场景：施法、物品使用、单位创建/移除、生命周期推进行为正确。
- [ ] 5.7 确认 Interval 系统（如 `UnitNativeSystem`）注册与行为与修复前一致。
- [ ] 5.8 总结本 change 的改动范围、验证结果、剩余手测项。

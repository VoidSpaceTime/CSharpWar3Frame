# 任务清单：UI 面板分层

状态：`[ ]` 待办 / `[x]` 完成。未批准前不开始实现。

## P0：层桶与按层注册

- [x] 新增 `UILayer` 层桶枚举，顺序即层序：背景 → 面板 → 弹窗 → 模态 → 提示。
- [x] `UIManager` 支持按 `UILayer` 注册面板，创建顺序按层序；同层内保持注册顺序（不按名字排序）。
- [x] `UIComponent.ZIndex` 保持 `private set`；不新增 `Raise` / `BringToFront`。
- [x] 注册只创建控件树与层序；初始可见性由业务 `Show` 控制。
- [x] 验证：`dotnet build War3Frame/War3Frame.csproj` 0 错误。

## P1：测试与门禁

- [x] `Projects/test` 新增层序场景：按层创建后 `CompareZ` 胜者 = 视觉最上层；显隐不改 `ZIndex`；同层后注册者胜出。
- [x] 验证：`dotnet build Projects/test/test.csproj` 0 错误，`War3Frame.Regression existing` 全通过。
- [x] grep：不存在修改已有控件 `ZIndex` 的公开方法；UI 层无 `SetPriority` 调用方。
- [x] 真实 War3 多面板叠层未自动化；在 `summary.md` 记录是否已手动执行。

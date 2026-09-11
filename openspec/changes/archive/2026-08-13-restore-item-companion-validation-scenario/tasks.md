## 1. 场景恢复

- [x] 新增 `Projects/test/Scripts/Process/ItemCompanionAbilityValidationScenario.cs`，恢复 `Initialize(JPlayer)` 与 `Update()` 入口。
- [x] 使用现有 Item/Ability authoring API 创建验证对象，不修改生产实现或新增测试专用运行时分支。
- [x] 为新增类、函数和关键状态处理添加最简要的中文职责注释。

## 2. 行为验证

- [x] 验证每个主动 Item 最多关联一个 companion，且 companion owner 与 Item owner 一致。
- [x] 验证代表性的 ItemUse 目标派发，并确认 item/user origin 进入效果链。
- [x] 验证至少一条解绑或受控删除路径能够完成 companion 生命周期收口。
- [x] 让失败结果包含场景名称与必要上下文，禁止吞异常或静默跳过。

## 3. 静态验收

- [x] 对新增文件执行诊断；LSP 因未加载 Friflo 传递依赖产生误报，已由项目构建和独立运行验证交叉确认。
- [x] 执行 `dotnet build War3Frame/War3Frame.csproj -c Release`。
- [x] 执行 `dotnet build Projects/test/test.csproj -c Release`。
- [x] 记录真实 War3 客户端验证尚未执行，并保留该独立验收项。

# 实施总结：UI 面板分层

新增 `UILayer` 固定层桶（背景 / 面板 / 弹窗 / 模态 / 提示），并让 `UIManager` 从 `SortedDictionary`（按名字排序）改为按 `(层序, 注册序)` 排序的注册表：`Register` 只记录，`Build` 才按层序创建面板。这样创建序由层序决定，而不是注册调用序——匹配 War3「原生层序 = 创建序且不可重排」的约束。`UIComponent.ZIndex` 保持 `private set`，未新增任何运行期提层入口（无 `Raise`/`BringToFront`）；同层重叠由注册顺序固定；面板创建后保持隐藏，由业务显式 `Show`。

**验证**
- `Projects/Regression`：0 错误，`Executed 13; passed 13; failed 0`，含新 `UIPanelLayeringScenario`。
- `Projects/test`：0 错误。
- grep：无 `ZIndex` 运行期写入、无 `Raise/BringToFront`；UI 层无 `SetPriority` 调用方。
- `UIManager.Initialize` 改为非原生环境不调 `FrameHelper.GetGameUI()`，使层序可在断言路径验证（不触 DzApi）。

**未执行**：真实 War3 多面板叠层未手动验证（无客户端自动化）。**影响**：仅 UI 核心 + 测试；生成器 / FrameBuild / CLI 未动。

# 实施总结：UI 控件层对齐 Godot

控件层改为 Godot `Control` 式作者体验：`Position` / `Size` / `Visible` / `Alpha` / `MouseFilter` 直接写属性，`AddChild` 组树，命中后直投控件 C# 事件（`Pressed` / `DoublePressed` / `PointerEntered` / `PointerExited` / `Drag`）。日常点击不进 ECS。`UIEvent`、`UIInputSystem`(133)、`UIEventCleanupSystem`(140) 已删除。需要同步时走 `DzSyncData` → `SyncHelper` → `UISyncRequest` → `UISyncResolveSystem` 分发消费后删除。

**落地**

- 几何用 `UIVec`。`Position` 是锚点中心，`Size` 是宽高（屏幕比例）。旧左下角坐标换算为中心：`(x + w/2, y + h/2)`。
- `UIPreset` + `ApplyPreset`：`FullRect` 铺满结构父节点并写 `Size`；其余预设按同名锚点 `SetPoint`。不调用 `DzFrameSetAllPoints`。
- 四容器 `VBox` / `HBox` / `Center` / `Margin` 走脏重排（`_arranging` + 子项/尺寸钩子），不每帧全量。
- 键盘只注册一次（退格/Tab/回车/Esc/空格、方向、数字、字母）。`Pump` 遇到 `KeyDown` / `KeyUp` 直接跳过，不派焦点。Focus / 键盘导航仍是二期。
- 保留：创建期 `UILayer`、`FrameHelper` 为唯一 `DzFrame*` 写入口、锚点/命中内核、根参考 `{0.4, 0.3, 0.8, 0.6}`。`UIPanel` / `UIKit` 的 `Show` / `Hide` 是面板生命周期，不是控件 `Visible`。

**接线补全（复盘后）**

复盘发现首轮交付只有实现、没有调用点：`SyncHelper.Initialize` 从未被调用（接收 trigger 不存在）、`UISyncBridge.Register` 从未被调用（前缀无处理器）、`UISyncRequest` 无消费与删除（真实游戏会持续泄漏实体）。本次补线：

- `War3Init` 增加 `SyncHelper.Initialize(Store)`（创建接收 trigger 并注册同步事件）；`SyncHelper.Initialize` 补 `HandleHelper.HandleAdd(_syncTrigger)` 句柄登记。
- `UISyncBridge` 收口为单入口 `Consume(prefix, handler)`：登记消费处理器；原生环境再接入 `SyncHelper.Register`（非原生守卫、重复调用幂等）；`internal Dispatch` 供消费系统分发；`Reset` 连带清空处理器与接收注册。原公开 `Register`（无调用方）被替换。
- 新增 `UISyncResolveSystem`（`[SystemRegister(SystemKind.Immediate, 5)]`）：收集 `UISyncRequest` → `Dispatch` 分发到消费处理器 → 删除实体；未注册前缀直接丢弃；不触碰 131/132。生成器已将其注册进 `RegisterGeneratedSystems`（`MoveToCastSystem` 与 `UnitRemoveNativeSystem` 之间）。
- `HeroSelectExample` 改为全链路断言：`Consume` → 双击 `Send` → `Flush` 建实体 → Resolve 分发消费 → 断言消费值且 `Count == 0` 无残留。

**验证**

- 串行构建 0 错误：`War3Frame`、`Projects/test`、`Projects/Regression`。既有警告未处理（`Program.cs:109` CS8625、`SkillPointUpgradeScenario` CS8629、FrameBuild nullable）。
- `dotnet run --project Projects/Regression/Regression.csproj --no-build -- existing`：`Executed 14; passed 14; failed 0`，含 `UIControlValidationScenario`（中心换算、容器、`FullRect`）、`UIPanelLayeringScenario`、`HeroSelectExample`（补线后全链路版）。
- 静态：无 `class UIEvent` / `UIInputSystem` / `UIEventCleanupSystem` / `BlocksLower` / `UIManager` / `ShowInternal`。控件层 `DzFrame*` 只在 `FrameHelper`。`git diff --stat` 对 `TriggerSystems.cs` 为空，131/132 未改。生成注册含 `UISyncResolveSystem`（order 5）。

**未执行与剩余风险**

- 真实 War3 鼠标 / 键盘 / IME / resize / 多面板叠层不能自动化，本轮未跑客户端。
- 真实客户端的 `DzSyncData` 双端往返（含自收）未实测；本轮验证覆盖 `Send → 入队 → Flush → Resolve → 消费` 的非原生链路，原生接收段（`SyncHelper.OnSyncReceived` 触发）依赖 `War3Init` 调用正确。
- 按键 `status` 按鼠标约定推断：1 = 按下，0 = 松开。KK/YD 文档未坐实。
- 非原生路径：`UIRuntime.NativeReady == false` 时同步请求入队，`Flush(store)` 才建 `UISyncRequest`。
- 影响：`War3Frame` UI 核心与同步桥、`Projects/test` 场景。生成器 / FrameBuild / CLI 行为契约未改。未 `openspec archive`，未 commit。

## R3 视角

| 视角 | 证据 | verdict |
|---|---|---|
| 目标/约束 | 属性 + `AddChild` + C# 事件 + 四容器 + `UIPreset`；点击不进 ECS；无 Win32；131/132 无 diff；同步调用点全部接通（`War3Init` / `Consume` / `UISyncResolveSystem`） | pass |
| 技术质量 | 三项目串行构建 0 错误；`DzFrame*` 仅 `FrameHelper`；容器脏重排；消费系统先收集后删除（无迭代内结构变更） | pass |
| 安全 | 无新凭据/网络面；接收回调只入队，游戏状态只由系统在 tick 内改写；未注册前缀丢弃 | pass |
| QA | `existing` 14/14（含补线后全链路 `HeroSelectExample`） | pass |
| 上下文 | 键盘只入队；War3 客户端未跑；`status` 约定未由官方文档确认；真实 `DzSyncData` 双端往返未实测 | pass（风险已记录，不阻断本轮自动化） |

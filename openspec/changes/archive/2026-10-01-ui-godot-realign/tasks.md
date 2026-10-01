# 任务清单：UI 控件层对齐 Godot

状态：`[ ]` 待办 / `[x]` 完成。

## P0：控件属性化 + 事件直投

- [x] `UIComponent` 属性化：`Position` / `Size` / `Alpha`；`AddChild` 为准（构造不隐式挂树）；`Visible` 属性级联替换 `Show(bool)` / `ShowInternal`。
- [x] `MouseFilter`（`Stop` / `Pass` / `Ignore`）替换 `BlocksLower`；`ZIndex` 保持只读。
- [x] 输入层命中后**直接投递控件 C# 事件**（`Pressed` / `DoublePressed` / `PointerEntered` / `PointerExited` / `Drag`）；内建双击（窗口默认 0.3s）；保留指针捕获语义。
- [x] 删除 `UIEvent`、`UIInputSystem`(order 133)、`UIEventCleanupSystem`(order 140)；输入层改为 OOP 服务，仍只注册一次 War3 鼠标/滚轮/移动/resize/键盘。
- [x] 验证：`dotnet build War3Frame/War3Frame.csproj` 0 错误。

## P1：anchors + 容器

- [x] anchors/preset（比例）+ 相对关系：`UIPreset` + `ApplyPreset`（`FullRect` 铺满父矩形）；`SetPoint` 保留。控件层不留 `SetPos` / `Show` / `Hide`。
- [x] 容器 `VBoxContainer` / `HBoxContainer` / `CenterContainer` / `MarginContainer`，接入现有脏批算（尺寸/子集变化重算，不每帧全量）。
- [x] 验证：容器排布场景，`Projects/test` 0 错误。

## P2：kit + 同步桥

- [x] `UIKit` 实例（`Add(panel, layer)` / `Build` / `Show` / `Hide` / `Destroy`）；按 `(层序, 注册序)` 创建；key 缓存幂等创建。
- [x] `UIPanel` 瘦身为「子树 + 生命周期钩子」；删除全局静态 `UIManager` 与全局 `Tooltip`。
- [x] UI→ECS 同步桥（框架系统）：`DzSyncData` 发送 + `SyncHelper.Register` 接收 → 写 ECS 请求 → `UISyncResolveSystem` 分发消费后删除。
- [x] `Projects/test` 示例改为 C# 事件 + 同步请求版。

## P2 补线：同步链路接线（复盘补全）

- [x] `War3Init` 调用 `SyncHelper.Initialize(Store)`（创建接收 trigger）；`SyncHelper.Initialize` 补 `HandleHelper.HandleAdd` 句柄登记。
- [x] `UISyncBridge` 收口为单入口 `Consume(prefix, handler)`：登记消费处理器 + 原生接收注册（非原生守卫、重复调用幂等）；`Dispatch` 内部分发；`Reset` 连带清空处理器与注册。
- [x] 新增 `UISyncResolveSystem`（`SystemKind.Immediate`, order 5）：收集 `UISyncRequest` → 分发到消费处理器 → 删除实体；未注册前缀丢弃；不触碰 131/132。
- [x] `HeroSelectExample` 全链路断言：`Consume` → 双击 `Send` → `Flush` 建实体 → Resolve 分发消费 → 无残留。
- [x] 复验：串行构建 0 错误；`existing` 14/14。

## 验证与门禁

- [x] `Projects/test` 与 `Projects/Regression` 构建 0 错误，`existing` 回归全通过。
- [x] grep：无 `UIEvent` 类型/引用；无 UI 专用 order 133/140；控件层无直接 `DzFrame*`（只经 `FrameHelper`）。
- [x] `TriggerSystem(131)` / `EventCleanupSystem(132)` 未被改动。
- [x] 真实 War3 鼠标/键盘/IME/resize 未自动化；在 `summary.md` 记录未执行原因与剩余风险。
- [x] 公共 API BREAKING 逐项核对调用点（`SetXxx`→属性、`UIEvent` 删除）。

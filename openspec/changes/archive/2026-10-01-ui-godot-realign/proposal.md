**状态**：已实施
**等级**：architecture
**提案日期**：2026-09-25
**请求来源**：现有 UI 层 DX 差（不如 xlik），定案对齐 Godot `Control` 模型。

## Why

现有 UI 层能用但作者体验很差：一次点击必须写 `QuerySystem<UIEvent>` 并塞进 133/140 窗口（消费者项目还被 `WFGEN004` 禁止自注册系统），双击要自己用时间窗算，排版要手算坐标，创建还要 `UIManager.Register/Build/Show` 工序。根因是**控件本是本地 OOP，却被绑进了 ECS 事件系统**，且缺少 Godot 那种「属性 + 事件 + 容器」。本提案把 UI 重新对齐 Godot `Control` 模型，让日常 UI 交互回到纯 OOP。

## What Changes

- **创建/组织**：`AddChild` 组合树；`kit` 分组（吸收 xlik）；key 缓存幂等创建（吸收 xlik）。
- **控件 API 属性化**：`Position / Size / Visible / Alpha / MouseFilter`（`Size` 用比例；控件层不留 `SetPos` / `Show` / `Hide` 兼容方法）。`ZIndex` 只读。`SetPoint` 与面板生命周期 `UIPanel` / `UIKit` 的 `Show` / `Hide` 保留。
- **布局**：比例 anchors + preset（`FullRect/Center/TopLeft/...`）+ 容器 `VBox / HBox / Center / Margin`。
- **交互**：输入层命中后**直接投递控件的 C# 事件**（`Pressed / DoublePressed / PointerEntered / PointerExited / Drag...`），**内建双击**。**不再创建 `UIEvent`**。
- **输入源**：只走 War3/DzApi（鼠标 / 滚轮 / 移动 / resize / 键盘），**不引入 Win32**；`UIInputQueue` 保持可插拔抽象。
- **同步**：UI 处理器发 `DzSyncData`；`SyncHelper.Register` 接收；接收处理器只落 ECS 请求/状态，由**框架内置通用桥**消费（不在 sync 回调里直接改游戏状态）。
- **删除/降级**：`UIEvent` + `UIInputSystem(133)` + `UIEventCleanupSystem(140)` 移除（若需"原始 UI 事实流"再作可选通道，默认关）。
- **保留**：`UILayer` **创建期**层序（War3 原生 z 不可运行时重排）、`FrameHelper` 原生执行层、锚点与命中注册表内核。
- **BREAKING**：`UIEvent` 相关公共类型移除；`UIComponent` 的 `SetPos` / `Show` / `Hide` 改为属性，不留兼容方法；`UIPanel / UIManager` 精简为 kit 形态。

## Capabilities

### New Capabilities

<!-- 暂无。若 UI→ECS 同步桥独立成型，再评估新增 ui-sync-bridge 能力。 -->

### Modified Capabilities

- `ui-control`：控件模型对齐 Godot（属性 / C# 事件 / 容器）；交互脱离 ECS；原 `UIEvent` 相关 requirement 移除。

## Impact

- `War3Frame/Src/Core/UI/`：控件属性化、事件、容器、kit；`UIManager / UIPanel` 重构。
- `War3Frame/Src/Systems/UI/`：删除 `UIInputSystem(133)`、`UIEventCleanupSystem(140)`；鼠标/键盘改由 OOP 输入层单次注册并直投。
- `War3Frame/Src/Components/UI/UIEvent.cs`：移除（或降级为可选）。
- `War3Frame/Src/Core/Sync/`：新增 UI→ECS 通用桥（框架内置）。
- `Projects/test`：示例与回归按新 API 重写；`HeroSelectExample` 改为 C# 事件直投 + 同步请求。
- **全局影响分析**：
  - `War3Frame.Generator/`：**不受影响**（不新增注册契约，反而移除了两个系统）。
  - `FrameBuild/`、`CSharpWar3Frame/`、`BridgeToJIT/`：**不受影响**。
  - `HandleHelper`：Frame 仍不登记。
  - ECS 事件系统 `TriggerSystem(131)` / `EventCleanupSystem(132)`：**不动**，只移除 UI 专用的 133/140。

## 非目标

- 不引入 Win32 硬件输入。
- 不照抄 Godot 的 `Window / Popup / ScrollContainer / Tree / Theme` 等大件；Focus / 键盘导航二期。
- 不做运行时 z 重排。
- 不改 `131/132` 事件系统本身。

## 分级判定

- 架构级：改公共 UI API、动系统 order 边界（删 133/140）、新增 UI→ECS 桥。
- 工件：`proposal.md`、`design.md`、`tasks.md`、`specs/ui-control/spec.md`（delta）。
- 复盘强度：`R3`（公共 API + 跨边界）。
